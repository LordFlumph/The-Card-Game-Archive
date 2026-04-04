namespace CardGameArchive
{
	using UnityEngine;
	using UnityEngine.InputSystem;

	/// <summary>
	/// Handles all code related to managing player input
	/// </summary>
	public class InputManager : MonoBehaviour
	{
		public const string InteractableLayerName = "Interactable";
		public static InputManager Instance { get; private set; }

		[SerializeField] InputActionAsset InputActions;
		InputAction tapAction, pressedAction, pointerPositionAction;

		private Camera mainCamera;

		MonoBehaviour currentDraggable = null;
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

			if (pointerPositionAction != null)
				pointerPositionAction.performed += PointerPositionChanged;
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

			if (pointerPositionAction != null)
				pointerPositionAction.performed += PointerPositionChanged;
		}

		private void TapActionPerformed(InputAction.CallbackContext context)
		{
			if (!InputEnabled)
				return;

			ITappable tappable = GetTappableAtPointer();

			if (tappable != null)
			{
				if (tappable is CardObject card)
				{
					card.SetAutoMove(true);
				}
				tappable.OnTap();
			}
		}

		private void PressedActionPerformed(InputAction.CallbackContext context)
		{
			if (!InputEnabled)
				return;

			currentDraggable = GetTappableAtPointer() as IDraggable as MonoBehaviour;

			if (currentDraggable != null)
			{
				dragOffset = currentDraggable.transform.position - mainCamera.ScreenToWorldPoint(pointerPositionAction.ReadValue<Vector2>());
				dragOffset.z = GameBoard.TopCardZ;

				if (currentDraggable is CardObject card)
				{
					card.SetAutoMove(false);
				}
			}
		}

		void PressedActionCanceled(InputAction.CallbackContext context)
		{
			if (currentDraggable != null)
			{
				if (tapAction.WasPerformedThisFrame())
				{
					if (currentDraggable is CardObject card)
					{
						card.SetAutoMove(true);
					}

					currentDraggable = null;
					return;
				}
				else
				{
					if (currentDraggable is CardObject card)
					{
						card.SetAutoMove(true);
					}

					currentDraggable.GetComponent<IDraggable>().OnDrop();
					currentDraggable = null;
				}
			}
		}

		void PointerPositionChanged(InputAction.CallbackContext context)
		{
			if (currentDraggable != null)
			{
				Vector3 targetPosition = mainCamera.ScreenToWorldPoint(pointerPositionAction.ReadValue<Vector2>()) + dragOffset;
				targetPosition.z = -10;
				currentDraggable.transform.position = targetPosition;
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
