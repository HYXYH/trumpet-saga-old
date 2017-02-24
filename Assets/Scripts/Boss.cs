using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	public float maxSpeed = 5;
	public float speed = -50f;

	public bool inAir = false;

	private Animator anim;
	private Rigidbody2D rbody;
	public SpriteRenderer spr;

	private bool isFalling = false;
	private float fallTime = 0;
	private float fullFallTime = 0;
	private Vector3 fallPos;

//	public AudioClip clipStep;
	public AudioClip clipCollide; 
	public AudioClip clipFire;

//	private AudioSource audioStep;
//	private AudioSource audioJump;
	private AudioSource audioCollide;
	private AudioSource audioFire;


	public float timeToFire = 1;
	private float timeToFireLeft = 0;
	public float timeToFireDelta = 0.5f;

	public int healthPoints = 5;


	// Use this for initialization
	void Start () {
		rbody = this.GetComponent<Rigidbody2D>();
		anim = this.GetComponent<Animator>();
		spr = this.GetComponent<SpriteRenderer>();
	
		inAir = true;
	}

	void Awake() {
//		audioStep = AddAudio(clipStep, false, false, 0.8f);
		audioCollide = AddAudio(clipCollide, false, false, 1f);
		audioFire = AddAudio(clipFire, false, false, 0.8f); 
	}
	
	// Update is called once per frame
	void Update () {
		timeToFireLeft-= Time.deltaTime;
		if(timeToFireLeft < 0)
		{
			fire();
			resetFireTime();
		}

		anim.SetBool("inAir", inAir);
//		anim.SetFloat("speed", speed);
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
	}

	void fire()
	{

	}

	void resetFireTime()
	{
		timeToFireLeft = timeToFire + timeToFireDelta * Random.value * Mathf.Pow(-1f, Random.Range(0,2));
	}


	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip; 
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol; 
		return newAudio; 
	}


	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Background" || coll.gameObject.tag == "PipePair" || coll.gameObject.tag == "PipeLine") {
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
}
