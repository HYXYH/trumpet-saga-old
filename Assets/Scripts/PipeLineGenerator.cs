using UnityEngine;
using System.Collections;

public class PipeLineGenerator : MonoBehaviour {

	public float currentSpeed = 1;
	public float currentDistance = 1;

	public float timeToMaxSpeed = 100;
	public float maxSpeed = 20;

	public GameObject prefab_pl_simple;
	public GameObject prefab_pl_moving;
	public GameObject prefab_pl_moving2;
	public GameObject prefab_pl_stair2;
	public GameObject prefab_pl_stair3;
	public GameObject prefab_pl_stair4;

	public GameObject AirLevel;
	public GameObject lastPipeLine;

	private float fullTimeToMax = 0;
	private float startSpeed = 0;
	private Player player;

	PipeLine.PipeLineType nextPLtypename;
	public float distanceToNextPipe = 1;

	public int pipeCounter;

	int bossCounter = 0;

	// Use this for initialization
	public void Start () {
		bossCounter = 0;
		pipeCounter = 1;
		fullTimeToMax = timeToMaxSpeed;
		startSpeed = currentSpeed;

		nextPLtypename = PipeLine.PipeLineType.simple;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {

		currentDistance = Vector3.Distance(lastPipeLine.transform.position, new Vector3(0,5.5f,0));
		if ( distanceToNextPipe < currentDistance)
		{
			
			genNewPipeLine();
			genTypeForNextPipeLine();

		}

		if ( timeToMaxSpeed > -2 ){
			timeToMaxSpeed -= Time.deltaTime;
			currentSpeed = startSpeed +(maxSpeed - startSpeed) * ((fullTimeToMax - timeToMaxSpeed) / fullTimeToMax);
			AirLevel.transform.position = new Vector3(0,-10 - 39 * ((fullTimeToMax - timeToMaxSpeed) / fullTimeToMax),0);
		}

//		float newSpeed = (maxSpeed - startSpeed) * ((fullTimeToMax - timeToMaxSpeed) / fullTimeToMax);
//		if(currentSpeed < newSpeed)
//			currentSpeed = newSpeed;
	}

	void genTypeForNextPipeLine()
	{
//		float pscore = player.score;
		float pscore = 15 + bossCounter * 10;
		float ptype = 0;
		if(pscore > 60)
			ptype = 60 * Random.value;
		else
			ptype = pscore * Random.value;

		if(ptype > 50){
			nextPLtypename = PipeLine.PipeLineType.stair4;
		}
		else if(ptype > 40){
			nextPLtypename = PipeLine.PipeLineType.stair3;
		}
		else if(ptype > 30){
			nextPLtypename = PipeLine.PipeLineType.moving2;
		}
		else if(ptype > 20){
			nextPLtypename = PipeLine.PipeLineType.stair2;
		}
		else if(ptype > 10){
			nextPLtypename = PipeLine.PipeLineType.moving;
		}
		else{
			nextPLtypename = PipeLine.PipeLineType.simple;
		}
		Debug.Log("Next PipeType: " + nextPLtypename.ToString());

	}

	public void genNewPipeLine() {
		GameObject newPipeline;
		pipeCounter++;
		if(pipeCounter%20 == 0)
		{
			nextPLtypename = PipeLine.PipeLineType.simple;
			pipeCounter = 0;
			bossCounter++;
		}

		switch(nextPLtypename)
		{
		case PipeLine.PipeLineType.simple:
			newPipeline = (GameObject)Instantiate(prefab_pl_simple, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 2;
			break;

		case PipeLine.PipeLineType.moving:
			newPipeline = (GameObject)Instantiate(prefab_pl_moving, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 2;
			break;

		case PipeLine.PipeLineType.moving2:
			newPipeline = (GameObject)Instantiate(prefab_pl_moving2, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 2.55f;
			break;

		case PipeLine.PipeLineType.stair2:
			newPipeline = (GameObject)Instantiate(prefab_pl_stair2, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 2.55f;
			break;

		case PipeLine.PipeLineType.stair3:
			newPipeline = (GameObject)Instantiate(prefab_pl_stair3, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 3.1f;
			break;

		case PipeLine.PipeLineType.stair4:
			newPipeline = (GameObject)Instantiate(prefab_pl_stair4, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 3.65f;
			break;

		default:
			newPipeline = (GameObject)Instantiate(prefab_pl_simple, new Vector3(0, 5.5f, 0), Quaternion.identity);
			distanceToNextPipe = 2;
			break;
		}


		if(lastPipeLine)
		{
			lastPipeLine.GetComponent<PipeLine>().setNextPipeLine(newPipeline);
		}
		lastPipeLine = newPipeline;
		PipeLine npl = newPipeline.GetComponent<PipeLine>();
		npl.speed = currentSpeed;
		npl.type = nextPLtypename;
		npl.enabled = true;


		if(pipeCounter == 0)
		{
			ArenaController arena = this.GetComponent<ArenaController>();
			arena.arenaPipeLine = npl;
			arena.enabled = true;
			arena.isFighting = false;
			this.enabled = false;
		}
	}
}
