using System;
using CameraMovement;
using Input;
using UnityEngine;

namespace Bootstrap
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField] private InputSettings _inputSettings;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Core.Views.CharacterController _characterController;
        
        private InputService _inputService;

        private void Awake()
        {
            _inputService = new InputService(_inputSettings);
            _inputService.MovementChanged += _cameraController.UpdateMovement;
            _inputService.RotationChanged += _cameraController.UpdateRotation;
            _inputService.ZoomChanged += _cameraController.UpdateZoom;
            _inputService.Clicked += SendCharacterToPosition;
        }

        private void Update()
        {
            _inputService.Update();
        }

        private void SendCharacterToPosition(ClickInfo info)
        {
            _characterController.SetDestination(info.WorldPosition);
        }

        private void OnDestroy()
        {
            _inputService.MovementChanged -= _cameraController.UpdateMovement;
            _inputService.RotationChanged -= _cameraController.UpdateRotation;
            _inputService.ZoomChanged -= _cameraController.UpdateZoom;
            _inputService.Clicked -= SendCharacterToPosition;
            _inputService.Dispose();
        }
    }
}