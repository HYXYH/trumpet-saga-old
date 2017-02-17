using UnityEngine;
using System.Collections;

public class CloudMover : MonoBehaviour {


	public PipeLineGenerator pipelineGenerator;
	public SpriteRenderer[] bigClouds;
	public SpriteRenderer[] smallClouds;

	public float bigCloudsSpeedScale = 0.5f;
	public float smallCloudsSpeedScale = 0.25f;

	int currBig = 0;
	int currSmall = 0;
	private float speed = 0;


	// Use this for initialization
	void Start () {
		if(pipelineGenerator == null)
		{
			pipelineGenerator = GameObject.FindGameObjectWithTag("Generator").GetComponent<PipeLineGenerator>();
		}

		speed = pipelineGenerator.currentSpeed;
	
		reset();
	}
	
	// Update is called once per frame
	void Update () {
		speed = pipelineGenerator.currentSpeed;
		float bigSpeed = speed * bigCloudsSpeedScale;
		float smallSpeed = speed * smallCloudsSpeedScale;
		if(pipelineGenerator.enabled) {
			
//			this.transform.Translate(0, -bigSpeed * Time.deltaTime, 0);

			foreach(SpriteRenderer sr in bigClouds)
			{
				sr.transform.Translate(0, -bigSpeed * Time.deltaTime, 0);
			}
			if(bigClouds[currBig].transform.localPosition.y < -10)
			{
				bigClouds[currBig].transform.Translate(0,10 * bigClouds.Length - 1,0);
				currBig = (currBig + 1) % bigClouds.Length;
				this.transform.parent = null;
			}

			foreach(SpriteRenderer sr in smallClouds)
			{
				sr.transform.Translate(0, -smallSpeed * Time.deltaTime, 0);
			}
			if(smallClouds[currSmall].transform.localPosition.y < -10)
			{
				smallClouds[currSmall].transform.Translate(0,10 * smallClouds.Length - 1,0);
				currSmall = (currSmall + 1) % smallClouds.Length;
			}
		}
	}

	public void reset() {
		this.transform.parent = pipelineGenerator.AirLevel.transform;
		currBig = 0;
		currSmall = 0;
		for(int i = 0; i < bigClouds.Length; i++)
		{
			bigClouds[i].transform.localPosition = new Vector3(0, i * 10 ,0);
		}

		for(int i = 0; i < smallClouds.Length; i++)
		{
			smallClouds[i].transform.localPosition = new Vector3(0, i * 10 ,0);
		}

		foreach(SpriteRenderer sr in smallClouds) {
			Color c = sr.color;
			c.a = 1f;
			sr.color = c;
		}

		foreach(SpriteRenderer sr in bigClouds) {
			Color c = sr.color;
			c.a = 1f;
			sr.color = c;
		}
	}
}
