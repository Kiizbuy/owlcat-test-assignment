using System;
using UnityEngine;

namespace CameraMovement
{
    [Serializable]
    public class CameraSettings
    {
        [Header("Pan / Zoom")]
        public float panSpeed = 20f;
        public float zoomSpeed = 40f;
        public float minDistance = 5f;
        public float maxDistance = 20f;
        public float defaultDistance = 5;

        [Header("Angles & Height")]
        [Range(10f, 80f)] public float pitch = 45f;
        public float minHeightAboveGround = 7.5f;

        [Header("Rotation")]
        public float rotateSpeed = 120f; // degrees/sec

        [Header("Collision / Grounding")]
        public LayerMask groundMask = ~0;
        public float collisionProbeHeight = 7.5f;
        public float cameraRadius = 1;

        [Header("Smoothing")]
        public float smoothTime = 0.22f;

        [Header("Stability")]
        [Tooltip("Максимальная величина, на которую desiredPos может измениться за секунду.\nПомогает сгладить резкие коррекции (deocclusion/terrain) и убрать дерганья.")]
        public float maxCorrectionSpeed = 30f;
    }
}