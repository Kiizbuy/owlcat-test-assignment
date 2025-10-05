using UnityEngine;

namespace Core.Views
{
    [RequireComponent(typeof(Animator))]
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
        }

        public void SetIntParameter(string id, int value)
        {
            _animator.SetInteger(id, value);
        }
        
        public void SetFloatParameter(string id, float value)
        {
            _animator.SetFloat(id, value);
        }

        public void SetBooleanParameter(string id, bool value)
        {
            _animator.SetBool(id, value);
        }
    }
}