using UnityEngine;
using System.Collections;

public class ItemAddPoint : MonoBehaviour,IPlayerRespawnListener {

	public int pointToAdd;
	public GameObject PointEffect;
	public AudioClip soundEffect;

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> () == null)
			return;

        AllServices.Instance.GetService<FactoryFloatText>().
			BuildFloatingText(transform.position, pointToAdd.ToString(), Color.white);
        AllServices.Instance.GetService<LevelManager>().Point += pointToAdd;

        if (PointEffect != null)
			Instantiate (PointEffect, transform.position, Quaternion.identity);
		SoundManager.PlaySfx (soundEffect);

		gameObject.SetActive (false);
	}

	void IPlayerRespawnListener.OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		gameObject.SetActive (true);
	}
}
