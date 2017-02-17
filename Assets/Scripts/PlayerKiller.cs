using UnityEngine;
using System.Collections;


public class PlayerKiller : MonoBehaviour {

	private float killTime = 1f;
	private float fullKillTime;
	private float flyTime = 1f;
	private float fullFlyTime;
	private bool isDying = false;
	public GameObject[] pls;
	private GameObject AirLevel;
	private float levelPos;

	private Player player;

	// Use this for initialization
	void Start () {
		isDying = false;

		fullFlyTime = flyTime;
		fullKillTime = killTime;
	}
	
	// Update is called once per frame
	void Update () {

		if(isDying){
			if(killTime < 0)
			{
				player.fallDownToStart(flyTime + 0.1f);
				if(flyTime < 0)
				{
					GameObject.FindGameObjectWithTag("Generator").GetComponent<Menu>().killedRestart();
					if(this.gameObject.transform.parent.GetComponent<PipePair>().firstPipeInGame)
					{
						isDying = false;
						flyTime = fullFlyTime;
						killTime = fullKillTime;
					}
					foreach(GameObject pl in pls)
					{
						if(!pl.GetComponent<PipeLine>().pipes[0].firstPipeInGame)
							Destroy(pl.gameObject);
					}
				}
				else
				{
					flyTime -= Time.deltaTime;
					moveLevel();
				}
			}
			else
				killTime -= Time.deltaTime;
		}
	
	}

	//найдём все лайны и пэйеры и вырубим, гг поморгает, потом вычисляем скорость, 
    //за которою уровень поедет вниз за 2 секунды все пайпы уезжают, через 2 секунды их удаляем
	//нужно выключить коллайдер игрока на время падения

	void moveLevel()
	{
		float newPos = levelPos * (flyTime / fullFlyTime);
		if(flyTime<0){
			newPos  = 0;
		}
		AirLevel.transform.position = new Vector3(0, newPos, 0);
	}


	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == "Player")
		{
			Debug.Log("watch your head!");
			player = other.gameObject.GetComponent<Player>();
			player.die();

			PipeLineGenerator pgn = GameObject.FindGameObjectWithTag("Generator").GetComponent<PipeLineGenerator>();
			pls = GameObject.FindGameObjectsWithTag("PipeLine");

			pgn.enabled = false;
			AirLevel = pgn.AirLevel;
			levelPos = pgn.AirLevel.transform.position.y;


			foreach(GameObject pl in pls)
			{
				PipeLine p = pl.GetComponent<PipeLine>();
				p.transform.parent = AirLevel.transform;
				p.enabled = false;
				foreach(PipePair pair in p.pipes) {
					if (pair.firstPipeInGame)
					{
						p.transform.parent = null;
					}
					else
						pair.enabled = false;
				}
			}
			isDying = true;
		}
	}
}
