namespace CardGameArchive
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;

	public class InputManager : MonoBehaviour
	{
		const string InteractableLayerName = "Interactable";
		public static InputManager Instance { get; private set; }

		[SerializeField] InputActionAsset InputActions;
		InputAction tapAction, pressedAction, pointerPositionAction;

		private Camera mainCamera;

		IDraggable currentDraggable = null;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			tapAction = InputActions.FindAction("Tap");
			pressedAction = InputActions.FindAction("Pressed");
			pointerPositionAction = InputActions.FindAction("PointerPosition");
		}

		private void Start()
		{
			mainCamera = Camera.main;
			DontDestroyOnLoad(gameObject);
		}

		private void OnEnable()
		{
			tapAction.performed += TapActionPerformed;
			pressedAction.performed += PressedActionPerformed;
		}

		private void OnDisable()
		{
			tapAction.performed -= TapActionPerformed;
			pressedAction.performed -= PressedActionPerformed;
		}

		private void TapActionPerformed(InputAction.CallbackContext context)
		{
			GetTappableAtPointer()?.OnTap();
		}

		private void PressedActionPerformed(InputAction.CallbackContext context)
		{			
			currentDraggable = GetTappableAtPointer() as IDraggable;
		}

		private ITappable GetTappableAtPointer()
		{
			RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(pointerPositionAction.ReadValue<Vector2>()), Vector3.forward, 
													100, LayerMask.GetMask(InteractableLayerName));
			if (hit.collider != null)
			{
				return hit.collider.GetComponent<ITappable>();
			}
			return null;
		}
	} 
}
