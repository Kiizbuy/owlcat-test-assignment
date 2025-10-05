using System;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Views
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterView))]
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private string movementParameter;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private CharacterView view;
        [SerializeField] private float smoothTime = 5f;
        
        private float _currentSpeed;
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            view = GetComponent<CharacterView>();
        }

        public void SetDestination(Vector3 point)
        {
            agent.ResetPath();
            agent.SetDestination(point);
        }

        private void Update()
        {
            if (IsInvalidAnimatorParameter())
            {
                Debug.LogError("Movement parameter is null. check that", this);
                return;
            }
            UpdateMovementParameter();
        }

        private void UpdateMovementParameter()
        {
            if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime * smoothTime);
            }
            else
            {
                var speed = agent.velocity.magnitude / agent.speed;
                _currentSpeed = Mathf.Lerp(_currentSpeed, speed, Time.deltaTime * smoothTime);
            }

            view.SetFloatParameter(movementParameter, _currentSpeed);
        }

        private bool IsInvalidAnimatorParameter()
        {
            return string.IsNullOrEmpty(movementParameter);
        }
    }
}