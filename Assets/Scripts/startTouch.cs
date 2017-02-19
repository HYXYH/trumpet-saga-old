using UnityEngine;
using System.Collections;

public class startTouch : MonoBehaviour {

	public GameObject notPressed;
	public GameObject pressed;

	bool isPressed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
		{
			notPressed.SetActive(false);
			pressed.SetActive(true);
			isPressed = true;

		}
		else if (isPressed)
		{
			this.gameObject.SetActive(false);
			GameObject.FindGameObjectWithTag("Generator").GetComponent<Menu>().buttonPlayPressed();
		}
	}
}
