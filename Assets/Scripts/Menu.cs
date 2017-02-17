using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject firstPipelinePrefab;
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


	bool isPlaying = false;

	PipeLineGenerator pipeGenerator;
	float timeToMaxSpeed;
	float currentSpeed;

	// Use this for initialization
	void Start () {
		lastScore = PlayerPrefs.GetInt("Last Score");
		maxScore = PlayerPrefs.GetInt("Max Score");

		cLast.text = lastScore.ToString();
		cBest.text = maxScore.ToString();
		cCurrent.enabled = false;
	
		pipeGenerator = this.GetComponent<PipeLineGenerator>();
		timeToMaxSpeed = pipeGenerator.timeToMaxSpeed;
		currentSpeed = pipeGenerator.currentSpeed;
	}
	
	// Update is called once per frame
	void Update () {


		if(isPlaying) {
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
		else{
			lastScore = PlayerPrefs.GetInt("Last Score");
			if(lastScore == -777)
				cLast.text = "GOD MODE";
			else
				cLast.text = lastScore.ToString();
			maxScore = PlayerPrefs.GetInt("Max Score");
		}
	
	}

	public void play() {
		Debug.Log("Play!");
		level.GetComponent<Animation>().Play("startGameDown");
		firstPipeline.GetComponent<PipeLine>().enabled = true;
		firstPipeline = null;
		pipeGenerator.enabled = true;

	}

	public void buttonPlayPressed() {
		isPlaying = true;
		level.GetComponent<Animation>().Play("startGame");
		allMenu.SetActive(false);
		player.GetComponent<Player>().enabled = true;
		player.GetComponent<Player>().score = 0;
		cCurrent.enabled = true;
	}

	public void killedRestart() {
		isPlaying = false;
		Debug.Log("Restart");
		allMenu.SetActive(true);
		replayMenu.SetActive(true);
		shareMenu.SetActive(false);
		cCurrent.enabled = false;
		this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
		if(firstPipeline == null) {
			firstPipeline = (GameObject)GameObject.Instantiate(firstPipelinePrefab, new Vector3(0, 3.25f, 0), Quaternion.identity);
			firstPipeline.GetComponent<Animation>().Play("FirstPipeOut");
		}
		pipeGenerator.lastPipeLine = firstPipeline;
		pipeGenerator.timeToMaxSpeed = timeToMaxSpeed;
		pipeGenerator.currentSpeed = currentSpeed;
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
				this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}
		}
	}

}
