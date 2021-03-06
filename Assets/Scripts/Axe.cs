﻿using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour {

	private float lifeTime = 10;
	private Animation anim;
	public float speed = -7f;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animation>();
	}

	// Update is called once per frame
	void Update () {
		lifeTime -= Time.deltaTime;
		if(lifeTime < 0)
		{
			Destroy(this.gameObject);
		}
		if(!anim.isPlaying)
		{
			anim.Play("AxeRotation");
		}
		transform.position = transform.position + new Vector3(0f,speed * Time.deltaTime,0f);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			if(coll.collider.GetComponent<Player>().isCheating) {
				return;
			}

			// stop boss
			ArenaController arena = GameObject.FindGameObjectWithTag("Generator").GetComponent<ArenaController>();
//			arena.enabled = false;
			arena.Boss.transform.parent = arena.BossPipeLine.transform;

			GameObject.Find("Player").GetComponent<Player>().inAir = true;
			GameObject.Find("Player").GetComponent<Player>().isFighting = false;
			GameObject.FindGameObjectWithTag("Generator").GetComponent<Menu>().killedBy = "Boss";
//			GameObject.FindGameObjectWithTag("PipeLine").GetComponent<PipeLine>().pipes[0].LeftPipe.GetComponent<PlayerKiller>().OnTriggerEnter2D(coll);
			Debug.Log("Axe is calling playerkiller");
			GameObject.FindGameObjectWithTag("PipeLine").GetComponent<PipeLine>().pipes[0].LeftPipe.GetComponent<PlayerKiller>().OnTriggerEnter2D(coll.collider.gameObject.GetComponent<BoxCollider2D>());


			Destroy(this.gameObject);
		}
		Debug.Log("Axe collidied!");
	}
}
