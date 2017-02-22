using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.IO; 
using System.ComponentModel;
using System;

public class NetEye : MonoBehaviour {

	public UnityEngine.UI.Text netText;
	string url = "http://bazooka.16mb.com/NetEye.txt";


	IEnumerator Start() {
		if(netText == null)
		{
			netText = this.GetComponent<UnityEngine.UI.Text>();
			netText.text = "";
		}

		WWW www = new WWW(url);
		yield return www;
		netText.text = www.text;
	}
	
	// Update is called once per frame
	void Update () {

	}


}