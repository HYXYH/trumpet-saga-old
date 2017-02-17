using UnityEngine;
using System.Collections;



public class PipeLine : MonoBehaviour {

	public float speed = 1f;
	public float closeTime = 0.5f;
	public float openTime = 0.5f; 
	public float dieTime = 5f;

	// coord in screen width percent, 0 - means left edge, 1 - means right edge.
	public float holeCoord = 0.5f;
	// width in screen width percent, 0 means no hole, 1 means 100% hole.
	public float holeWidth = 0.4f;

	public PipePair[] pipes;

	public GameObject nextPipeLine;

	public enum PipeLineType {simple, stair2, stair3, stair4, stair5, moving, moving2, closing};
	public PipeLineType type;

	public bool isDying = false;


	private float deleteHeight = 0;


	// Use this for initialization
	void Start () {
		isDying = false;
		generatePipes();

	}
	
	// Update is called once per frame
	void Update () {	

		if(!isDying)
		{
			this.transform.Translate(0, -speed * Time.deltaTime, 0);
			if(this.transform.position.y < deleteHeight){
				GameObject.Destroy(this.gameObject);
			}
		}
		else{
			dieTime -= Time.deltaTime;
			if(dieTime < 0)
				Destroy(this.gameObject);
		}

	}

	public void generatePipes()
	{
		switch(type)
		{
		case PipeLineType.simple:
			holeWidth = 0.4f;
			holeCoord = Random.value * (1 - holeWidth) + holeWidth / 2;
			pipes[0].initPipes(holeCoord, holeWidth);
			deleteHeight = -5;

			break;

		case PipeLineType.moving:
			holeWidth = 0.4f;
			holeCoord = Random.value * (1 - holeWidth) + holeWidth / 2;
			pipes[0].initPipes(holeCoord, holeWidth);
			pipes[0].enableMoving(0.5f);

			deleteHeight = -5;
			break;

		case PipeLineType.moving2:
			holeWidth = 0.4f;
			holeCoord = Random.value * (1 - holeWidth) + holeWidth / 2;
			pipes[0].initPipes(holeCoord, holeWidth);
			pipes[0].enableMoving(0.5f);

			holeCoord = Random.value * (1 - holeWidth) + holeWidth / 2;
			pipes[1].initPipes(holeCoord, holeWidth);
			pipes[1].enableMoving(0.5f);

			deleteHeight = -6;
			break;

		case PipeLineType.stair2:
			holeWidth = 0.4f;
			holeCoord = Random.value * (1 - holeWidth);
			pipes[0].initPipes(holeCoord, holeWidth);

			holeCoord += 0.08f;
			pipes[1].initPipes(holeCoord, holeWidth);

			deleteHeight = -6;
			break;

		case PipeLineType.stair3:
			holeWidth = 0.4f;
			holeCoord = Random.value * (1 - holeWidth);
			pipes[0].initPipes(holeCoord, holeWidth);

			holeCoord += 0.08f;
			pipes[1].initPipes(holeCoord, holeWidth);

			holeCoord += 0.08f;
			pipes[2].initPipes(holeCoord, holeWidth);

			deleteHeight = -7;
			break;

		case PipeLineType.stair4:
			holeWidth = 0.4f;
			holeCoord = Random.value * (1 - holeWidth);
			pipes[0].initPipes(holeCoord, holeWidth);

			holeCoord += 0.08f;
			pipes[1].initPipes(holeCoord, holeWidth);

			holeCoord += 0.08f;
			pipes[2].initPipes(holeCoord, holeWidth);

			holeCoord += 0.08f;
			pipes[3].initPipes(holeCoord, holeWidth);

			deleteHeight = -8;
			break;
		}
	}

	public void setNextPipeLine(GameObject np)
	{
		nextPipeLine = np;
	}


	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "Player")
		{
			if(other.transform.position.y > this.transform.position.y)
			{
				Destroy(this.GetComponent<Collider2D>());
				GameObject.FindGameObjectWithTag("Player").SendMessage("successJump", this.gameObject);
			}
		}
	}
}

