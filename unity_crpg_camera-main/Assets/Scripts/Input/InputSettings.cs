using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [Serializable]
    public class InputSettings
    {
        public LayerMask ClickObjectsMask;
        public float MaxTapDistance = 15f;
        public float SpherecastRadius = 0.2f;
        
        public InputActionReference MoveAction;   // Vector2 (WASD)
        public InputActionReference ZoomAction;   // float (scroll)
        public InputActionReference RotateAction;
    }
}