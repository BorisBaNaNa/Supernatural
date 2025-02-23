using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_StartScreen : MonoBehaviour {

	public Text levelName;
	public Text lives;

	void Start () {
		levelName.text = AllServices.Instance.GetService<LevelManager>().LevelName;
		lives.text = "x" + AllServices.Instance.GetService < GameManager>().LivesCount;
	}
}
