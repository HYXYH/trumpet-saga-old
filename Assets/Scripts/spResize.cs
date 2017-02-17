using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (SpriteRenderer))]
public class spResize : MonoBehaviour {

	// Use this for initialization
	public bool debugBg = false;

	#region METHODS
	private void Awake()
	{
		Resize();
	}

	private void Update()
	{
		#if UNITY_EDITOR
		Resize();
		#endif
	}

	void Resize()
	{
		if(this.gameObject.tag != "Background" && !debugBg)
		{
			GameObject bg = GameObject.FindGameObjectWithTag("Background");
			Vector3 scale = bg.transform.localScale;
			if(this.gameObject.tag == "RightPipe"){
				scale.x *= -1;
//				Debug.Log("RightPipe Found!");
			}
			transform.localScale = scale;

			return;
		}

		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		float worldScreenHeight = Camera.main.orthographicSize * 2;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		transform.localScale = new Vector3(
			worldScreenWidth / sr.sprite.bounds.size.x,
			worldScreenHeight / sr.sprite.bounds.size.y, 1);
	}
	#endregion
}
