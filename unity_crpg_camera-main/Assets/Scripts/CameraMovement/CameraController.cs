using UnityEngine;

namespace CameraMovement
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        public Transform target;

        [SerializeField] private CameraSettings cameraSettings;
    
        [Header("Camera Boundary")]
        public BoxCollider boundaryCollider;
        public float boundaryPadding = 0.5f;

        private Vector3 _currentVelocity;
        private Vector3 _focusPosition;
        private Vector3 _lastDesiredPos;
        private Vector2 _move;

        private float _currentDistance;
        private float _yaw;
        private float _zoom;
        private float _rotate;

        private void Reset()
        {
            _currentDistance = cameraSettings.defaultDistance;
            _focusPosition = target ? target.position : Vector3.zero;
        }

        private void Start()
        {
            _currentDistance = cameraSettings.defaultDistance;
            if (target != null) _focusPosition = target.position;
            _yaw = transform.eulerAngles.y;
            _lastDesiredPos = transform.position;
        }
        
        public void UpdateMovement(Vector2 value)
        {
            _move = value;
        }

        public void UpdateRotation(float value)
        {
            _rotate = value;
        }

        public void UpdateZoom(float value)
        {
            _zoom = value;
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;

            if (target != null)
            {
                _focusPosition = target.position;
                _lastDesiredPos = transform.position;
            }
        }
        
        public void SetBoundary(BoxCollider newBoundary)
        {
            boundaryCollider = newBoundary;

            if (boundaryCollider != null)
            {
                var bounds = boundaryCollider.bounds;
                _focusPosition.x = Mathf.Clamp(_focusPosition.x, bounds.min.x + boundaryPadding, bounds.max.x - boundaryPadding);
                _focusPosition.z = Mathf.Clamp(_focusPosition.z, bounds.min.z + boundaryPadding, bounds.max.z - boundaryPadding);
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) 
                return;

            UpdateYaw(_rotate);
            UpdateFocusPosition(_move);
            UpdateCameraDistance(_zoom);

            var desiredPos = CalculateDesiredPosition();
            desiredPos = ApplyCollisionAndGrounding(desiredPos);
            desiredPos = ApplyStability(desiredPos);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _currentVelocity, cameraSettings.smoothTime);
            transform.rotation = Quaternion.Euler(cameraSettings.pitch, _yaw, 0f);
        }

        private void UpdateYaw(float rotateInput)
        {
            _yaw += rotateInput * cameraSettings.rotateSpeed * Time.deltaTime;
        }

        private void UpdateFocusPosition(Vector2 moveInput)
        {
            var yawRot = Quaternion.Euler(0f, _yaw, 0f);
            var right = yawRot * Vector3.right;
            var forward = yawRot * Vector3.forward;

            _focusPosition += (right * moveInput.x + forward * moveInput.y) * cameraSettings.panSpeed * Time.deltaTime;

            if (boundaryCollider != null)
            {
                var bounds = boundaryCollider.bounds;
                _focusPosition.x = Mathf.Clamp(_focusPosition.x, bounds.min.x + boundaryPadding, bounds.max.x - boundaryPadding);
                _focusPosition.z = Mathf.Clamp(_focusPosition.z, bounds.min.z + boundaryPadding, bounds.max.z - boundaryPadding);
            }
        }

        private void UpdateCameraDistance(float zoomInput)
        {
            _currentDistance = Mathf.Clamp(_currentDistance - zoomInput * cameraSettings.zoomSpeed * Time.deltaTime, cameraSettings.minDistance, cameraSettings.maxDistance);
        }

        private Vector3 CalculateDesiredPosition()
        {
            var rot = Quaternion.Euler(cameraSettings.pitch, _yaw, 0f);
            return _focusPosition - rot * Vector3.forward * _currentDistance;
        }


        private Vector3 ApplyCollisionAndGrounding(Vector3 desiredPos)
        {
            RaycastHit hit;
            var probeStart = desiredPos + Vector3.up * cameraSettings.collisionProbeHeight;

            var hitTerrain = Physics.Raycast(probeStart, Vector3.down, out hit, cameraSettings.collisionProbeHeight * 2f, cameraSettings.groundMask.value);
            if (hitTerrain)
            {
                desiredPos.y = Mathf.Max(desiredPos.y, hit.point.y + cameraSettings.minHeightAboveGround);
            }
            else if (Terrain.activeTerrain != null)
            {
                desiredPos.y = Mathf.Max(desiredPos.y, Terrain.activeTerrain.SampleHeight(desiredPos) + Terrain.activeTerrain.transform.position.y + cameraSettings.minHeightAboveGround);
            }

            var dir = desiredPos - _focusPosition;
            var dist = dir.magnitude;
            if (dist > 0.001f)
            {
                var dirNorm = dir / dist;
                if (Physics.SphereCast(_focusPosition, cameraSettings.cameraRadius, dirNorm, out hit, dist, cameraSettings.groundMask.value))
                    desiredPos = hit.point - dirNorm * (cameraSettings.cameraRadius + 0.05f);
            }

            return desiredPos;
        }

        private Vector3 ApplyStability(Vector3 desiredPos)
        {
            var maxDelta = cameraSettings.maxCorrectionSpeed * Time.deltaTime;
            var stablePos = Vector3.MoveTowards(_lastDesiredPos, desiredPos, maxDelta);
            _lastDesiredPos = stablePos;
            return stablePos;
        }
    }
}
