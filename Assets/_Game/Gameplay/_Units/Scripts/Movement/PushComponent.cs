using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Movement
{
    public interface IPushable
    {
        void Push(Vector2 direction, float impulseStrength, PushType pushType = PushType.Normal);
    }
    
    public enum PushType
    {
        Normal, 
        Forced
    }
    public class PushComponent : MonoBehaviour, IPushable
    {
        [SerializeField, Required] private Rigidbody2D _rigidbody2D;
        [SerializeField, Required] private AUnitMove _navigation;

        [SerializeField, ReadOnly] private bool _isPushable;

        private Coroutine _restoreCoroutine;

        public void Construct(bool isPushable)
        {
            _isPushable = isPushable;
        }

        public void Push(Vector2 direction, float impulse, PushType type = PushType.Normal)
        {
            if (type == PushType.Normal && !_isPushable)
                return;

            if (_restoreCoroutine != null)
                StopCoroutine(_restoreCoroutine);

            _navigation.Disable();

            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.AddForce(direction.normalized * impulse, ForceMode2D.Impulse);

            _restoreCoroutine = StartCoroutine(RestoreNavigationAfterDelay(0.5f));
        }

        private IEnumerator RestoreNavigationAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _navigation.Enable();
            _navigation.Move(_navigation.Position);
        }
        
        [Button]
        private void ManualInit()
        {
            _navigation = GetComponent<AUnitMove>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
    }
}