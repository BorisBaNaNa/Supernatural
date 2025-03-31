using Assets.Supernatural.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class MeleeAttack : MonoBehaviour
    {
        public bool AttackIsReady => Time.time >= _nextAttack;

        [Tooltip("Какие слои можно бить")]
        [SerializeField] private LayerMask _collisionMask;
        [Tooltip("Можно ли быть сразу несколько врагов")]
        [SerializeField] private bool _multiDamage = false;
        [Tooltip("Урон врагу или объекту")]
        [SerializeField] private float _damageToGive;
        [Tooltip("Применить силу к врагу, при попадании, только для объектов с Rigid body")]
        [SerializeField] private Vector2 _pushObject;
        [SerializeField] private Transform _meleePoint;
        [SerializeField] private float _areaSize;

        private float _attackRate = 0.2f;
        private float _nextAttack = 0;

        public void Attack()
        {
            if (!AttackIsReady)
                return;

            Debug.Log("fire");
            _nextAttack = Time.time + _attackRate;
            CheckTargetCo();
        }

        private void CheckTargetCo()
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(_meleePoint.position, _areaSize, Vector2.zero, 0, _collisionMask);

            if (hits == null)
                return;

            foreach (RaycastHit2D hit in hits)
            {
                if (!hit.collider.gameObject.TryGetComponent<ICanTakeDamage>(out var damage))
                    continue;

                damage.TakeDamage(_damageToGive, _pushObject, gameObject);
                if (!_multiDamage)
                    break;
            }
        }

        void OnDrawGizmos()
        {
            if (_meleePoint == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_meleePoint.position, _areaSize);
        }
    }
}
