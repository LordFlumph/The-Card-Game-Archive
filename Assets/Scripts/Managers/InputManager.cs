namespace CardGameArchive
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using UnityEngine.InputSystem.Controls;
	using UnityEngine.InputSystem.EnhancedTouch;

	public class InputManager : MonoBehaviour
	{
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
			GetClickableAtPointer()?.OnClick();
		}

		private void PressedActionPerformed(InputAction.CallbackContext context)
		{			
			currentDraggable = GetClickableAtPointer() as IDraggable;
		}

		private IClickable GetClickableAtPointer()
		{
			if (Physics.Raycast(mainCamera.ScreenPointToRay(pointerPositionAction.ReadValue<Vector2>()), out RaycastHit hit))
			{
				return hit.collider.GetComponent<IClickable>();
			}
			return null;
		}
	} 
}
