using UnityEngine;
using System.Collections;

public class CloudMover : MonoBehaviour {


	public PipeLineGenerator pipeineGenerator;
	public SpriteRenderer[] bigClouds;
	public SpriteRenderer[] smallClouds;

	int currBig = 0;
	int currSmall = 0;
	private float speed = 0;


	// Use this for initialization
	void Start () {
		if(pipeineGenerator == null)
		{
			pipeineGenerator = GameObject.FindGameObjectWithTag("Generator").GetComponent<PipeLineGenerator>();
		}

		speed = pipeineGenerator.currentSpeed;
	
		reset();
	}
	
	// Update is called once per frame
	void Update () {
		speed = pipeineGenerator.currentSpeed;
		float bigSpeed = speed/2;
		float smallSpeed = speed/4;
		if(pipeineGenerator.enabled) {
			
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
		this.transform.parent = pipeineGenerator.AirLevel.transform;
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
