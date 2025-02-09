using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    [Tooltip("Какие слои можно бить")]
    public LayerMask CollisionMask;
    [Tooltip("Можно ли быть сразу несколько врагов")]
    public bool multiDamage = false;
    [Tooltip("Урон врагу или объекту")]
    public float damageToGive;
    [Tooltip("Применить силу к врагу, при попадании, только для объектов с Rigid body")]
    public Vector2 pushObject;
    public Transform MeleePoint;
    public float areaSize;

    public float attackRate = 0.2f;
    [Tooltip("Проверьте цель в пределах досягаемости после некоторой задержки, полезно для синхронизации правильного времени атаки анимации")]
    public float attackAfterTime = 0.15f;

    float nextAttack = 0;

    public bool Attack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            StartCoroutine(CheckTargetCo(attackAfterTime));
            return true;
        }
        else
            return false;
    }

    IEnumerator CheckTargetCo(float delay)
    {
        yield return new WaitForSeconds(delay);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(MeleePoint.position, areaSize, Vector2.zero, 0, CollisionMask);

        if (hits == null)
            yield break;

        foreach (RaycastHit2D hit in hits)
        {
            ICanTakeDamage damage = hit.collider.gameObject.GetComponent<ICanTakeDamage>();
            if (damage == null)
                continue;

            damage.TakeDamage(damageToGive, pushObject, gameObject);
            if (!multiDamage)
                break;
        }
    }

    void OnDrawGizmos()
    {
        if (MeleePoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(MeleePoint.position, areaSize);
    }
}
