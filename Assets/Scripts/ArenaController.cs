using UnityEngine;
using System.Collections;

public class ArenaController : MonoBehaviour {

	public PipeLine arenaPipeLine;
	public GameObject BossPrefab;
	public GameObject BossPipeLinePrefab;

	GameObject BossPipeLine;
	GameObject Boss;

	bool isFighting;

	// Use this for initialization
	void Start () {
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
}
