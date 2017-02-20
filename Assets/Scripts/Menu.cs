using UnityEngine;
using System.IO;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject firstPipelinePrefab;
	public GameObject firstPipeline;
	public GameObject allMenu;
	public GameObject replayMenu;
	public GameObject shareMenu;
	public GameObject startMenu;
	public GameObject volOn;
	public GameObject volOff;

	public GameObject player;

	public UnityEngine.UI.Text cLast;
	public UnityEngine.UI.Text cBest;
	public UnityEngine.UI.Text cCurrent;

	public int lastScore = 0;
	public int maxScore = 0;

	public GameObject level;

	string screenshot_path;


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




	public void ShareMe()
	{
		StartCoroutine( ShareScreenshot() );

	}

	// Сам код
	public IEnumerator ShareScreenshot()
	{
		// Настройки плагина
		string shareText  = "Trumpet Saga\n";
		string gameLink = "Here are my results! Come on join me! "+"\nhttps://play.google.com/store/apps/details?id=???";
		string subject = "";
		string imageName = "Screenshot"; // without the extension, for iinstance, MyPic 


		// wait for graphics to render
		yield return new WaitForEndOfFrame();
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
		// create the texture
		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
		// put buffer into texture
		screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
		// apply
		screenTexture.Apply();
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
		byte[] dataToSave = screenTexture.EncodeToPNG();
		string destination = Path.Combine(Application.persistentDataPath,System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
		File.WriteAllBytes(destination, dataToSave);
		if(!Application.isEditor)
		{
			// block to open the file and share it ------------START
			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

			intentObject.Call<AndroidJavaObject> ("setType", "text/plain");
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText + gameLink);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");

			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

//			currentActivity.Call("startActivity", intentObject);
			AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
			currentActivity.Call("startActivity", jChooser);
		}

	}


//	public IEnumerator make_score_screenshot() {
//		// wait for graphics to render
//		yield return new WaitForEndOfFrame();
//		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
//		// create the texture
//		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
//		// put buffer into texture
//		screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
//		// apply
//		screenTexture.Apply();
//		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
//		byte[] dataToSave = screenTexture.EncodeToPNG();
//		screenshot_path = Path.Combine(Application.persistentDataPath,System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
//		File.WriteAllBytes(screenshot_path, dataToSave);
//	}
//


	public void ShareIt (string app) {
		string message = "My score in Trupet Saga: " + cLast.text;


		if (app != "ok")
			Sharing.ShareVia (app, message);
		else
			Sharing.ShareVia (app, message, string.Format("{0};{1}", App.OdnoklassnikiAppId, App.OdnoklassnikiSecretId));
	}


	public void openShareMenu() {
//		StartCoroutine( make_score_screenshot() );

//		replayMenu.SetActive(false);
//		shareMenu.SetActive(true);

		ShareMe();
	}
		

	public void backFromShare() {
		replayMenu.SetActive(true);
		shareMenu.SetActive(false);
	}


	public void switchVolumeToOn() {
		volOff.SetActive(false);
		volOn.SetActive(true);
		AudioListener.volume = 1f;
	}

	public void switchVolumeToOff() {
		volOn.SetActive(false);
		volOff.SetActive(true);
		AudioListener.volume = 0f;
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
