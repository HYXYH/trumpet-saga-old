using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	public GameObject AxePrefab;

	public float maxSpeed = 5;
	public float speed = -50f;

	public bool inAir = false;

	private Animator anim;
	private Rigidbody2D rbody;
	public SpriteRenderer spr;

	private bool isFalling = false;
	private float fallTime = 2;

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

	public bool isFlashing = false;
	private float flash;
	public float flashTime = 1f;
	public int flashAmount = 3;


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

		if(isFalling)
		{
			fallTime-= Time.deltaTime;
			if(fallTime < 0){
				GameObject.Find("Player").GetComponent<Player>().isFighting = false;
				GameObject.Find("Background").GetComponent<ArenaController>().BossPipeLine.GetComponent<PipeLine>().pipes[0].fullTime = 0.1f;
				Destroy(this.gameObject);
			}
		}
		timeToFireLeft-= Time.deltaTime;
		if(timeToFireLeft < 0 && !inAir)
		{
			fire();
			resetFireTime();
		}

		anim.SetBool("inAir", inAir);
//		anim.SetFloat("speed", speed);

		if(isFlashing)
			processFlashing();
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
		//fire Axe
		Vector3 pos = transform.position;
		pos.y-=0.5f;
		Instantiate(AxePrefab, pos, Quaternion.identity);
		audioFire.Play();
	}

	void resetFireTime()
	{
		timeToFireLeft = timeToFire + timeToFireDelta * Random.value * Mathf.Pow(-1f, Random.Range(0,2));
	}

	void shotDown()
	{
		Debug.Log("Shot Down (" + healthPoints.ToString() +" hp)");
		healthPoints--;
		audioCollide.Play();
		if(healthPoints <= 0)
		{
			rbody.AddForce(new Vector2(0,5), ForceMode2D.Impulse);
			this.GetComponent<BoxCollider2D>().enabled = false;
			inAir = true;
			isFalling = true;

			GameObject.FindGameObjectWithTag("Generator").SendMessage("bossDead"); //.GetComponent<ArenaController>().bossDead();
		}

		flash = flashTime;
		isFlashing = true;

	}


	void processFlashing() {
		//flashing
		flash -= Time.deltaTime;
		if(flash < 0)
		{
			flash = flashTime;
			spr.material.SetFloat("_FlashAmount", 0);
			isFlashing = false;
			return;
		}

		float intervals = 2 * flashAmount;
		float intervalLength = flashTime / intervals;
		int currInterval = (int)(flash / intervalLength);
		if(currInterval % 2 == 0)
		{
			spr.material.SetFloat("_FlashAmount", 0);
		}
		else
		{
			spr.material.SetFloat("_FlashAmount", 1);
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
