using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject firstPipeline;
	public GameObject allMenu;
	public GameObject replayMenu;
	public GameObject shareMenu;
	public GameObject player;

	public UnityEngine.UI.Text cLast;
	public UnityEngine.UI.Text cBest;
	public UnityEngine.UI.Text cCurrent;

	public int lastScore = 0;
	public int maxScore = 0;

	public GameObject level;

//	bool starting = false;

	// Use this for initialization
	void Start () {
		lastScore = PlayerPrefs.GetInt("Last Score");
		maxScore = PlayerPrefs.GetInt("Max Score");

		cLast.text = lastScore.ToString();
		cBest.text = maxScore.ToString();
	
	}
	
	// Update is called once per frame
	void Update () {

		lastScore = player.GetComponent<Player>().score;

		cCurrent.text = lastScore.ToString();
		cLast.text = cCurrent.text;
		PlayerPrefs.SetInt("Last Score", lastScore);
		if(lastScore > maxScore){
			maxScore = lastScore;
			cBest.text = cCurrent.text;
			PlayerPrefs.SetInt("Max Score", maxScore);
		}
	
	}

	public void play() {
		Debug.Log("Play!");
		level.GetComponent<Animation>().Play("startGameDown");
		firstPipeline.GetComponent<PipeLine>().enabled = true;
		this.GetComponent<PipeLineGenerator>().enabled = true;

	}

	public void buttonPlayPressed() {
		level.GetComponent<Animation>().Play("startGame");
		allMenu.SetActive(false);
		player.GetComponent<Player>().enabled = true;
	}

	public void killedRestart() {
		Debug.Log("Restart");
		allMenu.SetActive(true);
		replayMenu.SetActive(true);
		shareMenu.SetActive(false);
	}


	public void openShareMenu() {
		replayMenu.SetActive(false);
		shareMenu.SetActive(true);
	}

	public void backFromShare() {
		replayMenu.SetActive(true);
		shareMenu.SetActive(false);
	}



	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "Player")
		{
			if(other.transform.position.y > this.transform.position.y)
			{
				play();
				GameObject.Destroy(this.gameObject.GetComponent<BoxCollider2D>());
			}
		}
	}

}
