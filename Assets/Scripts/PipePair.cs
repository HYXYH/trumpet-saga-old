using UnityEngine;
using System.Collections;

public class PipePair : MonoBehaviour {

	private bool isClosing = false;
	private bool isOpening = false;
	private bool isMoving = false;

	private float openTime = 0.5f;
	private float closeTime = 0.5f;
	public  float fullTime = 0.5f;
	private float moveSpeed = 1f;

	// coord in screen width percent, 0 - means left edge, 1 - means right edge.
	private float holeCoord = 0.5f;
	// width in screen width percent, 0 means no hole, 1 means 100% hole.
	private float holeWidth = 0.4f;

	public bool firstPipeInGame;
	public GameObject LeftPipe;
	public GameObject RightPipe;


	// Use this for initialization
	void Start () {
		if(LeftPipe == null || RightPipe == null)
			foreach(Transform child in transform){
				if(child.CompareTag("LeftPipe"))
					LeftPipe = child.gameObject;
				if(child.CompareTag("RightPipe"))
					RightPipe = child.gameObject;
			}
	
		if(firstPipeInGame)
			calcHole();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(isClosing)
		{
			closeTime -= Time.deltaTime;
			processClosing();
		}

		if(isOpening)
		{
			openTime -= Time.deltaTime;
			processOpening();
		}

		if(isMoving)
		{
			processMoving();
		}

		if(isOpening && isClosing)
		{
			Debug.LogWarning("Trying to open and close pipe at the same time!");
			isOpening = false;
		}
	}

	public void initPipes(float coord, float width)
	{
		holeCoord = coord;
		holeWidth = width;

		float coordInUnits = Screen.width * (holeCoord - 0.5f) / 200f;
		float holeOffsetInUnits = (Screen.width * holeWidth/2) / 200f;

		LeftPipe.gameObject.transform.localPosition = new Vector3(coordInUnits - holeOffsetInUnits,0,0);
		RightPipe.gameObject.transform.localPosition = new Vector3(coordInUnits + holeOffsetInUnits,0,0);
	}

	public void open(float _openTime) {
		isClosing = false;
		isOpening = true;
		if(closeTime > 0){
			_openTime -= closeTime;
		}
		openTime = _openTime;
		fullTime = _openTime;
	}

	public void close(float _closeTime) {
		isOpening = false;
		isClosing = true;
		if(openTime > 0){
			_closeTime -= openTime;
		}
		closeTime = _closeTime;
		fullTime = _closeTime;
	}

	public void enableMoving(float _moveSpeed) {
		isMoving = true;
		moveSpeed = _moveSpeed;
	}

	private void calcHole()
	{
		float hoiu = LeftPipe.gameObject.transform.localPosition.x - RightPipe.gameObject.transform.localPosition.x;
		hoiu /= -2f;
		holeWidth = hoiu * 400f / Screen.width;
		holeCoord = 0.5f;
	}

	private void processClosing()
	{
		isClosing = true;
		float openPart = closeTime / fullTime;
		if( openPart < 0)
		{
			openPart = 0;
			isClosing = false;
		}
		float currentHoleWidth = holeWidth * openPart;

		float coordInUnits = Screen.width * (holeCoord - 0.5f) / 200f;
		float holeOffsetInUnits = (Screen.width * currentHoleWidth/2) / 200f;

		LeftPipe.gameObject.transform.localPosition = new Vector3(coordInUnits - holeOffsetInUnits,0,0);
		RightPipe.gameObject.transform.localPosition = new Vector3(coordInUnits + holeOffsetInUnits,0,0);
	}

	private void processOpening()
	{
		isOpening = true;
		float openPart = 1 - openTime / fullTime;
		if( openPart > 1)
		{
			openPart = 1;
			isOpening = false;
		}
		float currentHoleWidth = holeWidth * openPart;

		float coordInUnits = Screen.width * (holeCoord - 0.5f) / 200f;
		float holeOffsetInUnits = (Screen.width * currentHoleWidth/2) / 200f;

		LeftPipe.gameObject.transform.localPosition = new Vector3(coordInUnits - holeOffsetInUnits,0,0);
		RightPipe.gameObject.transform.localPosition = new Vector3(coordInUnits + holeOffsetInUnits,0,0);
	}

	private void processMoving()
	{
		if(moveSpeed > 0)
		{
			if(holeCoord < 1 - holeWidth/2)
			{
				holeCoord += Time.deltaTime * moveSpeed;
			}
			else
			{
				moveSpeed*= -1;
			}
		}

		if(moveSpeed < 0)
		{
			if(holeWidth/2 < holeCoord )
			{
				holeCoord += Time.deltaTime * moveSpeed;
			}
			else
			{
				moveSpeed*= -1;
			}
		}

		initPipes(holeCoord, holeWidth);
	}


	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "Player")
		{
			if(other.transform.position.y > this.transform.position.y)
			{
				firstPipeInGame = false; // for correct kill processing
				this.GetComponent<Collider2D>().isTrigger = false;
				isMoving = false;
				close(fullTime);

				GameObject.FindGameObjectWithTag("Player").SendMessage("scoreUp");
			}
		}
	}

}
