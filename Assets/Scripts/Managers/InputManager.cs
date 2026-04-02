namespace CardGameArchive
{
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
		Vector3 dragOffset = Vector3.zero;

		public bool InputEnabled { get; set; } = false;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
				return;
			}


			tapAction = InputActions.FindAction("Tap");
			pressedAction = InputActions.FindAction("Pressed");
			pointerPositionAction = InputActions.FindAction("PointerPosition");
		}

		private void OnEnable()
		{
			if (tapAction != null)
				tapAction.performed += TapActionPerformed;

			if (pressedAction != null)
			{
				pressedAction.performed += PressedActionPerformed;
				pressedAction.canceled += PressedActionCanceled;
			}
		}

		private void OnDisable()
		{
			if (tapAction != null)
				tapAction.performed -= TapActionPerformed;

			if (pressedAction != null)
			{
				pressedAction.performed -= PressedActionPerformed;
				pressedAction.canceled -= PressedActionCanceled;
			}
		}

		private void TapActionPerformed(InputAction.CallbackContext context)
		{
			if (!InputEnabled)
				return;

			GetTappableAtPointer()?.OnTap();
		}

		private void PressedActionPerformed(InputAction.CallbackContext context)
		{
			if (!InputEnabled)
				return;
			return;
			currentDraggable = GetTappableAtPointer() as IDraggable;

			if (currentDraggable != null)
			{
				dragOffset = mainCamera.ScreenToWorldPoint(pointerPositionAction.ReadValue<Vector2>()) - (currentDraggable as MonoBehaviour).transform.position;
				dragOffset.z = -10;
			}
		}

		void PressedActionCanceled(InputAction.CallbackContext context)
		{
			return;
			if (currentDraggable != null)
			{
				currentDraggable.OnDrop();
				currentDraggable = null;
			}
		}

		private ITappable GetTappableAtPointer()
		{
			if (mainCamera == null)
				mainCamera = Camera.main;

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
