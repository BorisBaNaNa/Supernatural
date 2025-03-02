using UnityEngine;

namespace Assets.Supernatural.Scripts.Interfaces
{
    public interface ICanTakeDamage
    {

        void TakeDamage(float damage, Vector2 force, GameObject instigator);
    }
}
