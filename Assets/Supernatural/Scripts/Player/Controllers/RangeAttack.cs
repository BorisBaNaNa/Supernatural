using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class RangeAttack : MonoBehaviour
    {
        public bool AttackIsReady => BulletCount > 0 && Time.time >= nextFire;

        [SerializeField] private Transform _firePoint;
        [Tooltip("Запуск снарядя после задержки, полезно для синхронизации с анимацией")]
        [SerializeField] private float _fireRate;

        public int BulletCount { get; set; } = 100;

        float nextFire = 0;

        private void Awake()
        {
            //BulletCount = AllServices.Instance.GetService<GameManager>().Bullet;
        }

        public void Fire()
        {
            if (!AttackIsReady)
                return;

            Debug.Log("fire");
            BulletCount--;
            nextFire = Time.time + _fireRate;
            return;
        }
    }
}
