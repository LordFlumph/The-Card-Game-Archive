using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
	[Header("Slider Setup")]
	public bool Value { get; private set; }

	Slider slider;

	[Header("Animation")]
	[SerializeField, Range(0, 1f)] float animationDuration = 0.5f;
	[SerializeField] AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

	Coroutine animateSliderCoroutine;

	[Header("Colour Setup")]
	[SerializeField] Image backgroundImage;
	[SerializeField] Color backgroundOnColour = Color.white;
	[SerializeField] Color backgroundOffColour = Color.grey;

	[SerializeField] Image sliderImage;
	[SerializeField] Color handleOnColour = Color.white;
	[SerializeField] Color handleOffColour = Color.grey;

	[Header("Events")]
	[SerializeField] UnityEvent OnToggleOn, OnToggleOff;

	void Awake()
	{
		SetupSliderComponent();
	}

	protected void OnValidate()
	{
		SetupSliderComponent();
	}

	void SetupSliderComponent()
	{
		slider = GetComponent<Slider>();

		slider.interactable = false;
		var sliderColors = slider.colors;
		sliderColors.disabledColor = Color.white;
		slider.colors = sliderColors;
		slider.transition = Selectable.Transition.None;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Toggle();
	}

	public void Toggle()
	{
		Debug.Log("Toggle Clicked");

		Value = !Value;

		if (Value)
			OnToggleOn.Invoke();
		else
			OnToggleOff.Invoke();

		if (animateSliderCoroutine != null)
			StopCoroutine(animateSliderCoroutine);

		animateSliderCoroutine = StartCoroutine(AnimateSlider());
	}

	public void SetValue(bool value, bool invokeEvents = true)
	{
		Value = value;

		if (invokeEvents)
		{
			if (Value)
				OnToggleOn.Invoke();
			else
				OnToggleOff.Invoke();
		}

		if (animateSliderCoroutine != null)
			StopCoroutine(animateSliderCoroutine);

		animateSliderCoroutine = StartCoroutine(AnimateSlider());
	}

	IEnumerator AnimateSlider()
	{
		float startValue = slider.value;
		float endValue = Value ? 1 : 0;

		float time = 0;
		if (animationDuration > 0)
		{
			while (time < animationDuration)
			{
				time += Time.deltaTime;

				float lerpFactor = slideEase.Evaluate(time / animationDuration);
				slider.value = Mathf.Lerp(startValue, endValue, lerpFactor);

				backgroundImage.color = Color.Lerp(Value ? backgroundOffColour : backgroundOnColour, Value ? backgroundOnColour : backgroundOffColour, lerpFactor);
				sliderImage.color = Color.Lerp(Value ? handleOffColour : handleOnColour, Value ? handleOnColour : handleOffColour, lerpFactor);

				yield return null;
			}
		}

		slider.value = endValue;
		backgroundImage.color = Value ? backgroundOnColour : backgroundOffColour;
		sliderImage.color = Value ? handleOnColour : handleOffColour;
	}
}
