namespace CardGameArchive
{
	using CardGameArchive.MainMenu;
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
		InputAction tapAction, pressedAction, pointerPositionAction, backAction, screenshotAction;

		private Camera mainCamera;

		MonoBehaviour currentDraggable = null;
		Vector3 dragOffset = Vector3.zero;

		public bool InputEnabled { get; private set; } = true;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}


			tapAction = InputActions.FindAction("Tap");
			pressedAction = InputActions.FindAction("Pressed");
			pointerPositionAction = InputActions.FindAction("PointerPosition");
			backAction = InputActions.FindAction("Back");
			screenshotAction = InputActions.FindAction("Screenshot");
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

			if (backAction != null)
				backAction.performed += BackActionPerformed;

#if UNITY_EDITOR
			if (screenshotAction != null)
				screenshotAction.performed += ScreenshotActionPerformed; 
#endif
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

			if (backAction != null)
				backAction.performed -= BackActionPerformed;

#if UNITY_EDITOR
			if (screenshotAction != null)
				screenshotAction.performed -= ScreenshotActionPerformed; 
#endif
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
					if (card.CanDrag)
					{
						card.SetAutoMove(false);
					}
					else
					{
						currentDraggable = null;
					}
				}
			}
		}

		void PressedActionCanceled(InputAction.CallbackContext context)
		{
			if (currentDraggable != null)
			{
				if (tapAction.WasPerformedThisFrame())
				{
					ReleaseDraggable();
				}
				else
				{
					currentDraggable.GetComponent<IDraggable>().OnDrop();
					ReleaseDraggable();
				}
			}
		}

		public void ReleaseDraggable()
		{
			if (currentDraggable is CardObject card)
			{
				card.SetAutoMove(true);
			}
			currentDraggable = null;
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

		void BackActionPerformed(InputAction.CallbackContext context)
		{
			if (!InputEnabled)
				return;

			if (StandardGameManager.Instance != null)
			{
				PopupMenuManager.Instance.BackButtonPressed();
			}
			else if (MainMenuManager.Instance != null)
			{
				MainMenuManager.Instance.BackButtonPressed();
			}
		}

#if UNITY_EDITOR
		void ScreenshotActionPerformed(InputAction.CallbackContext context)
		{
			string directory = System.IO.Path.Combine("Screenshots",
				//Application.persistentDataPath,
				UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

			if (!System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);

			int screenshotCount = 0;
			screenshotCount = System.IO.Directory.GetFiles(directory, "*.png").Length+1;

			string filePath = System.IO.Path.Combine(directory, screenshotCount + ".png");
			ScreenCapture.CaptureScreenshot(filePath);
			Debug.Log("Screenshot captured: " + filePath);

			screenshotCount++;
		}
#endif

		public void EnableInput()
		{
			InputEnabled = true;
		}

		public void DisableInput()
		{
			ReleaseDraggable();
			InputEnabled = false;
		}
	}
}
