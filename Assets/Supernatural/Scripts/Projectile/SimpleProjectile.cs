using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile, ICanTakeDamage
{
	public int Damage;
	public GameObject DestroyOtherEffect;
	public GameObject DestroyOnTakeDamageEffect;
	public int pointToGivePlayer;
	public float timeToLive;

	public AudioClip soundHitEnemy;
	public AudioClip soundHitNothing;

	
	// Update is called once per frame
	void Update ()
	{
		if ((timeToLive -= Time.deltaTime) <= 0) {
			DestroyProjectile ();
			return;
		}

		transform.Translate ((Direction + new Vector2 (InitialVelocity.x, 0)) * Speed * Time.deltaTime, Space.World);
	}

	void DestroyProjectile(GameObject destroyEffect = null)
    {
		if (destroyEffect != null)
			Instantiate (destroyEffect, transform.position, Quaternion.identity);
		
		Destroy (gameObject);
	}


	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (pointToGivePlayer != 0) {
			var projectile = instigator.GetComponent<Projectile> ();
			if (projectile != null && projectile.Owner.GetComponent<Player> () != null) {
				//GameManager.Instance.AddPoint (pointToGivePlayer);
				//GameManager.Instance.ShowFloatingText ("+" + pointToGivePlayer, transform.position,Color.yellow);
			}
		}

		SoundManager.PlaySfx (soundHitNothing);
		DestroyProjectile (DestroyOtherEffect);
	}

	protected override void OnCollideOther (Collider2D other)
	{
		SoundManager.PlaySfx (soundHitNothing);
		DestroyProjectile (DestroyOtherEffect);
	}

	protected override void OnCollideTakeDamage (Collider2D other, ICanTakeDamage takedamage)
	{
		takedamage.TakeDamage (Damage, Vector2.zero, gameObject);
		SoundManager.PlaySfx (soundHitEnemy);
		DestroyProjectile (DestroyOnTakeDamageEffect);
	}
}

