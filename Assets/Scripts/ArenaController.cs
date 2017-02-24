using UnityEngine;
using System.Collections;

public class ArenaController : MonoBehaviour {

	public PipeLine arenaPipeLine;
	public GameObject BossPrefab;
	public GameObject BossPipeLinePrefab;


	public GameObject BossPipeLine;
	public GameObject Boss;

	public bool isFighting;

	// Use this for initialization
	void Start () {
		isFighting = false;
	}

	void Awake() {
		isFighting = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(!isFighting && arenaPipeLine.transform.position.y < -2.61){
			arenaPipeLine.enabled = false;
			isFighting = true;

			GameObject.Find("Player").GetComponent<Player>().isFighting = true;

			BossPipeLine = (GameObject)Instantiate(BossPipeLinePrefab, new Vector3(0, 3.25f, 0), Quaternion.identity);
			this.GetComponent<PipeLineGenerator>().lastPipeLine.GetComponent<PipeLine>().setNextPipeLine(BossPipeLine);
			this.GetComponent<PipeLineGenerator>().lastPipeLine = BossPipeLine;
			BossPipeLine.GetComponent<Animation>().Play("BossPipeOut");

		}

		if(isFighting && Boss == null)
		{
			if(!BossPipeLine.GetComponent<Animation>().isPlaying)
			{
				Boss = (GameObject)Instantiate(BossPrefab, new Vector3(0, 5.5f, 0), Quaternion.identity);
			}
		}


	}

	public void bossDead()
	{
		isFighting = false;
		this.enabled = false;

		PipePair pp = BossPipeLine.GetComponent<PipeLine>().pipes[0];
		pp.initPipes(0.5f, 0.5f);
		pp.open(2);
		BossPipeLine.GetComponent<BoxCollider2D>().isTrigger = true;
		GameObject.Find("Player").GetComponent<Player>().transform.parent = GameObject.Find("AirLevel").transform;

		GameObject.Find("Background").GetComponent<BoxCollider2D>().enabled = true;
		GameObject.Find("Background").GetComponent<PipeLineGenerator>().lastPipeLine = BossPipeLine;
		GameObject.Find("Background").GetComponent<Menu>().firstPipeline = BossPipeLine;

		arenaPipeLine.transform.parent = BossPipeLine.transform;
		isFighting = false;
		Boss.GetComponent<Boss>().enabled = false;
		this.enabled = false;
	}
}
