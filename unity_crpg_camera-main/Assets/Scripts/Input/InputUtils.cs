using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Input
{
    public static class InputUtils
    {
        public static bool IsPointerOverUI(Vector2 screenPos)
        {
            if (EventSystem.current == null) return false;
            
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPos
            };
            
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            return results.Count > 0;
        }
        
        public static Vector2 GetCurrentPointerPosition()
        {
            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.position.ReadValue();

            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();

            return Vector2.zero;
        }
        
        public static bool RaycastFromScreen(this Camera camera, Vector2 screenPos, out RaycastHit hit, LayerMask layerMask = default)
        {
            hit = default;

            if (!new Rect(0, 0, Screen.width, Screen.height).Contains(screenPos))
                return false;

            var ray = camera.ScreenPointToRay(screenPos);
            var mask = layerMask.value != 0 ? layerMask.value : Physics.DefaultRaycastLayers;

            var result = Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
            return result;
        }
        
        public static bool SphereCastFromScreen(this Camera camera, Vector2 screenPos, float radius, out RaycastHit hit, LayerMask layerMask = default)
        {
            hit = default;

            if (!new Rect(0, 0, Screen.width, Screen.height).Contains(screenPos))
                return false;

            var ray = camera.ScreenPointToRay(screenPos);
            var mask = layerMask.value != 0 ? layerMask.value : Physics.DefaultRaycastLayers;

            var result = Physics.SphereCast(ray, radius, out hit, Mathf.Infinity, mask);
            return result;
        }
    }
}