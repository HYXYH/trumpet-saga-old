using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine.Analytics;

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
	public string killedBy = "";
	float startPlayTime = 0;
	int gamesPlayed = 0;

	string screenshot_path;

	bool adsDisabled = false;
	bool adLeavingApplication = false;



	bool isPlaying = false;

	PipeLineGenerator pipeGenerator;
	float timeToMaxSpeed;
	float currentSpeed;

	InterstitialAd interstitial;

	// Use this for initialization
	void Start () {
		adsDisabled = PlayerPrefs.HasKey("NoAds");
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
		gamesPlayed++;
		startPlayTime = Time.time;
		killedBy = "";
		isPlaying = true;
		level.GetComponent<Animation>().Play("startGame");
		allMenu.SetActive(false);
		player.GetComponent<Player>().enabled = true;
		player.GetComponent<Player>().score = 0;
		cCurrent.enabled = true;

		if(!adsDisabled){
			if(interstitial != null)
				interstitial.Destroy();
			RequestInterstitial();
		}
	}

	public void killedRestart() {
		if(!adsDisabled){
			if ((interstitial != null) && interstitial.IsLoaded()) {
				interstitial.Show();
			}
		}

		Analytics.CustomEvent("gameOver", new Dictionary<string, object>
			{
				{ "score", lastScore },
				{ "killedBy", killedBy },
				{ "PlayTime", Time.time - startPlayTime }
			});


		isPlaying = false;
		Debug.Log("Restart");
		allMenu.SetActive(true);
		replayMenu.SetActive(true);
		shareMenu.SetActive(false);
		cCurrent.enabled = false;
		this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
		if(firstPipeline == null || firstPipeline.name.Contains("Boss")) {
			firstPipeline = (GameObject)GameObject.Instantiate(firstPipelinePrefab, new Vector3(0, 3.25f, 0), Quaternion.identity);
			firstPipeline.GetComponent<Animation>().Play("FirstPipeOut");
		}
		pipeGenerator.lastPipeLine = firstPipeline;
		pipeGenerator.timeToMaxSpeed = timeToMaxSpeed;
		pipeGenerator.currentSpeed = currentSpeed;
	}




	private void RequestInterstitial()
	{
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940599218656149/5725830911";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().
		AddTestDevice("03007c0c307b75cc").
		Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);


		// Called when the user returned from the app after an ad click.
		interstitial.OnAdClosed += HandleOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

	}
	
		public void HandleOnAdClosed(object sender, System.EventArgs ea)
	{
		print("OnAdClosed event received.");

		Analytics.CustomEvent("AdClosed", new Dictionary<string, object>{
		{"AdLeavedApplication", adLeavingApplication}
		});
		adLeavingApplication = false;
	}

		public void HandleOnAdLeavingApplication(object sender, System.EventArgs ea)
	{
		print("OnAdLeavingApplication event received.");
		adLeavingApplication = true;
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
			Analytics.CustomEvent("Share", new Dictionary<string, object>{});


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


	public void noAdsPurchased(){
		PlayerPrefs.SetString("NoAds","Ads Disabled");
		adsDisabled = true;
	}
		

	public void openShareMenu() {
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
				if(firstPipeline.GetComponent<PipeLine>().pipes[0].firstPipeInGame)
				{
					firstPipeline.GetComponent<PipeLine>().pipes[0].firstPipeInGame = false;// for correct kill processing
					play();
				}
				else{
					Debug.Log("Level complete! " + firstPipeline.GetComponent<PipeLine>().pipes[0].firstPipeInGame.ToString());
					firstPipeline.GetComponent<PipeLine>().enabled = true;
					firstPipeline = null;
					pipeGenerator.enabled = true;

				}
				this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}
		}
	}
	


	void OnApplicationQuit() {
		Analytics.CustomEvent("QuitGame", new Dictionary<string, object>{
		{"TotalPlayTime", Time.time},
		{"GamesPlayed", gamesPlayed}
		});
		Debug.Log("Application ending after " + Time.time + " seconds");
	}

}
