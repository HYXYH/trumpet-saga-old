using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public bool isCheating = false;
	public bool isFighting = false;

	public float maxSpeed = 5;
	public float speed = -50f;

	public float jumpSpeed = 2;
	public bool inAir = false;
	public float lastPipeSpeed = 0;

	public int score = 0;

	public bool incorrectParent = false;
	private bool isJumping = false;
	private Animator anim;
	private Rigidbody2D rbody;
	public SpriteRenderer spr;


	bool sameTouch = false;
	private bool isFalling = false;
	private float fallTime = 0;
	private float fullFallTime = 0;
	private Vector3 fallPos;

	public AudioClip clipStep;
	public AudioClip clipJump;
	public AudioClip clipCollide; 
	public AudioClip clipFire;

	private AudioSource audioStep;
	private AudioSource audioJump;
	private AudioSource audioCollide;
	private AudioSource audioFire;



	//todo:
	// звуки шагов
	// google rank (рейтинг игроков)


	// Use this for initialization
	void Start () {
		rbody = this.GetComponent<Rigidbody2D>();
		anim = this.GetComponent<Animator>();
		spr = this.GetComponent<SpriteRenderer>();

		incorrectParent = false;
	}

	public void Awake(){
		// add the necessary AudioSources:
		audioStep = AddAudio(clipStep, false, false, 0.8f);
		audioStep.pitch = 1.8f;

		audioJump = AddAudio(clipJump, false, false, 0.8f);
		audioCollide = AddAudio(clipCollide, false, false, 1);
		//audioFire = AddAudio(clipFire, false, false, 0.8); 
	} 
	
	// Update is called once per frame
	void Update () {

		if(isFalling){
			fallTime -= Time.deltaTime;
			if(fallTime < 0)
			{
				isFalling = false;
				this.transform.position = new Vector3(0,-2.61f,0);
				this.GetComponent<BoxCollider2D>().enabled = true;
				rbody.gravityScale = 10;
				enabled = false;

				return;
			}
			Vector3 pos = this.transform.position;
			pos.x = fallPos.x * fallTime/ fullFallTime;
			pos.y = (fallPos.y - (-2.61f)) * fallTime/ fullFallTime + (-2.61f);
			this.transform.position = pos;

		}
		else
		{
			

			if(Input.touchCount == 0)
			{
				sameTouch = false;
			}

			if(isFighting)
			{

				if (!sameTouch && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
				{
					//fire
				}
			}
			else {
				
				checkIfFailed();

				if (!inAir && !sameTouch && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !incorrectParent)
				{
					rbody.velocity = new Vector2(0, rbody.velocity.y);
					rbody.gravityScale = 0;
					audioStep.Stop();
					audioJump.Play();
					inAir = true;
					isJumping = true;
					sameTouch = true;
				}

				anim.SetBool("inAir", inAir);

			}
				
			anim.SetFloat("speed", speed);
				
			if(incorrectParent)
			{
				setNewParent();
			}

			if (!inAir && !audioStep.isPlaying)
			{
				audioStep.Play();
			}
		}
	}

	void FixedUpdate()
	{
		if(!inAir) {
			rbody.AddForce(Vector2.right * speed);
			if (rbody.velocity.x > maxSpeed)
				rbody.velocity = new Vector2(maxSpeed, rbody.velocity.y);

			if (rbody.velocity.x < -maxSpeed)
				rbody.velocity = new Vector2(-maxSpeed, rbody.velocity.y);
		}
		else{
			rbody.velocity = new Vector2(0, rbody.velocity.y);
		}

		if(isJumping)
		{
			processJumping();
		}
	}


	void checkIfFailed() {
		if(transform.parent == null || this.transform.localPosition.y < -3 && transform.parent.GetComponent<PipeLine>() != null)
		{
			if(isCheating)
			{
				transform.position = Vector3.zero;
				rbody.velocity = Vector2.zero;

				rbody.velocity = new Vector2(0, rbody.velocity.y);
				rbody.gravityScale = 0;
				audioJump.Play();
				inAir = true;
				isJumping = true;

				return;
			}
			inAir = true;
			rbody.velocity = Vector2.zero;
			GameObject.FindGameObjectWithTag("Generator").GetComponent<Menu>().killedBy = "FellDown";
			transform.parent.GetComponent<PipeLine>().pipes[0].LeftPipe.GetComponent<PlayerKiller>().OnTriggerEnter2D(GetComponent<BoxCollider2D>());
		}
	}


	void setNewParent(){
		GameObject parent = this.transform.parent.gameObject.GetComponent<PipeLine>().nextPipeLine;
		if(parent != null) {
			incorrectParent = false;
			this.gameObject.transform.parent = parent.transform;
		}
	}

	void scoreUp()
	{
		score++;
	}

	void successJump(GameObject pipeLine)
	{
		rbody.gravityScale = 10;
		isJumping = false;

		GameObject parent = pipeLine.GetComponent<PipeLine>().nextPipeLine;
		if(parent == null) {
			incorrectParent = true;
			this.gameObject.transform.parent = pipeLine.transform;
		} 
		else
			this.gameObject.transform.parent = parent.transform;
		
		float pipeSpeed = pipeLine.GetComponent<PipeLine>().speed;
		if(lastPipeSpeed == 0){
			lastPipeSpeed = pipeSpeed;
			return;
		}

		float rel = pipeSpeed / lastPipeSpeed;
		speed *= rel;
		maxSpeed *= rel;
		lastPipeSpeed = pipeSpeed;
		audioStep.pitch *= rel;

	}

	void processJumping() {
		Vector3 pos = transform.position;

		pos.y += jumpSpeed;
		transform.position = pos;
	}

	public void die() {
		isJumping = false;
		audioStep.pitch = 1.8f;
		rbody.gravityScale = 0;
		rbody.velocity = new Vector3(0,0,0);
		this.GetComponent<BoxCollider2D>().enabled = false;
		this.transform.parent = GameObject.FindGameObjectWithTag("Generator").GetComponent<PipeLineGenerator>().AirLevel.transform;
		audioCollide.Play();
	}

	public void fallDownToStart(float fTime)
	{
	if(!isFalling){
		isFalling = true;
		fullFallTime = fTime;
		fallTime = fTime;
		fallPos = this.transform.position;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Background" || coll.gameObject.tag == "PipePair") {
			inAir = false;
		}

		if (coll.gameObject.tag == "WallLeft")
		{
			speed *= -1;
			spr.flipX = true;
			Debug.Log("turn right");
		}

		if (coll.gameObject.tag == "WallRight")
		{
			speed *= -1;
			spr.flipX = false;
			Debug.Log("turn left");
		}
	}

	void OnCollisionStay2D(Collision2D coll) {
		if (coll.gameObject.tag == "Background" || coll.gameObject.tag == "PipePair") {
			inAir = false;
		}

		if (coll.gameObject.tag == "WallLeft" && speed < 0)
		{
			speed *= -1;
			spr.flipX = true;
			Debug.Log("turn right");
		}

		if (coll.gameObject.tag == "WallRight" && speed > 0)
		{
			speed *= -1;
			spr.flipX = false;
			Debug.Log("turn left");
		}
	}



	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip; 
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol; 
		return newAudio; 
	}


	public void changeCheatMode() {
		if(isCheating)
		{
			PlayerPrefs.SetInt("Last Score", 0);
			isCheating = false;
		}
		else {
			PlayerPrefs.SetInt("Last Score", -777);
			isCheating = true;
		}

	}

}
