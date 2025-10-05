using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputService : IDisposable
    {
        public event Action<ClickInfo> Clicked;
        public event Action<Vector2> MovementChanged;
        public event Action<float> ZoomChanged;
        public event Action<float> RotationChanged; 

        private readonly Camera _mainCamera;
        private readonly InputAction _clickAction;
        private readonly InputSettings _inputSettings;

        private Vector2 _touchStartPos;
        private bool _waitingForRelease;

        public InputService(InputSettings inputSettings)
        {
            _inputSettings = inputSettings;
            _mainCamera = Camera.main;

            _clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
            _clickAction.started += OnTouchStarted;
            _clickAction.canceled += OnTouchCanceled;
            _clickAction.Enable();

            if (_inputSettings.MoveAction != null)
            {
                _inputSettings.MoveAction.action.Enable();
            }

            if (_inputSettings.ZoomAction != null)
            {
                _inputSettings.ZoomAction.action.Enable();
            }

            if (_inputSettings.RotateAction != null)
            {
                _inputSettings.RotateAction.action.Enable();
            }
        }

        public void Update()
        {
            MovePerform();
            RotatePerform();
            ZoomPerform();
        }

        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            _touchStartPos = InputUtils.GetCurrentPointerPosition();
            _waitingForRelease = true;
        }

        private void OnTouchCanceled(InputAction.CallbackContext context)
        {
            if (!_waitingForRelease) return;

            _waitingForRelease = false;
            var releasePos = InputUtils.GetCurrentPointerPosition();

            if (InputUtils.IsPointerOverUI(releasePos)) return;
            if (Vector2.Distance(_touchStartPos, releasePos) > _inputSettings.MaxTapDistance) return;
            if (!_mainCamera.SphereCastFromScreen(releasePos, _inputSettings.SpherecastRadius, out var hit,
                    _inputSettings.ClickObjectsMask)) return;

            Clicked?.Invoke(new ClickInfo()
            {
                WorldPosition = hit.point,
                Normal = hit.normal,
                ClickedObject = hit.collider != null ? hit.collider.gameObject : null
            });
        }
        private void MovePerform()
        {
            MovementChanged?.Invoke(_inputSettings.MoveAction.action.ReadValue<Vector2>());
        }

        private void ZoomPerform()
        {
            ZoomChanged?.Invoke(_inputSettings.ZoomAction.action.ReadValue<float>());
        }

        private void RotatePerform()
        {
            RotationChanged?.Invoke(_inputSettings.RotateAction.action.ReadValue<float>());
        }
        public void Dispose()
        {
            _clickAction?.Dispose();

            if (_inputSettings.MoveAction != null)
            {
                _inputSettings.MoveAction.action.Dispose();
            }

            if (_inputSettings.ZoomAction != null)
            {
                _inputSettings.ZoomAction.action.Dispose();
            }

            if (_inputSettings.RotateAction != null)
            {
                _inputSettings.RotateAction.action.Dispose();
            }
        }
    }
}
