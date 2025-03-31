using System;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class PleyerHealthController : MonoBehaviour
    {
        public event Action OnDie;

        public float CurrentHealth { get; private set; }

        [SerializeField] private bool _godMode;
        [SerializeField] private float _maxHealth; // temp. Move to config

        public void Initialize()
        {
            CurrentHealth = _maxHealth;
        }


        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0 || _godMode)
                return;

            CurrentHealth -= damage;

            if (CurrentHealth <= 0)
                OnDie?.Invoke();
        }

        public void TakeHealth(float takenHearth)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + takenHearth, _maxHealth);
            ////////////GameManager.Instance.ShowFloatingText("+" + takenHearth, transform.position, Color.red);
        }
    }
}
