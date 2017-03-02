using UnityEngine;
using System.Collections;

// Этот грёбанный костыль должен вырубать меню на старте, но после того как отработает автоматический
// рестор покупок в кнопке ads. Хз работает или нет, нагуглил, что так должно работать.


public class RestoreKostil : MonoBehaviour {

	public GameObject[] setInactive;

	// Use this for initialization
	void Start () {

		foreach (GameObject g in setInactive)
		{
			g.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
