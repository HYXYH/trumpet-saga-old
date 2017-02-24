using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

	private float lifeTime = 10;
	private Animation anim;
	public float speed = 1f;

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
			anim.Play("Fireball");
		}
		transform.Translate(new Vector3(0f,speed * Time.deltaTime,0f));
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.collider.gameObject.tag == "Boss") {
			coll.collider.gameObject.SendMessage("shotDown");
			Destroy(this.gameObject);
		}
		Debug.Log("Fireball collidied!");

		if (coll.collider.gameObject.tag == "Axe") {
			Destroy(coll.collider.gameObject);
			Destroy(this.gameObject);
		}
	}
}
