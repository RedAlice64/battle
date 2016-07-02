using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleJSON;
using System;
using System.Text;
using System.IO;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 0.5f;
	public float turnDelay = 2f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
	private int level = 1;
    private int waveCount;
    private int currentWave = -1;
    private List<int> waves=new List<int>();
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;
    private float lastSpawnTime;
    private float spawnCoolDown;

    

    // Use this for initialization
    void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void OnLevelWasLoaded (int index)
	{
		level++;

		InitGame ();
	}

	void InitGame()
	{
		doingSetup = true;

        currentWave = -1;

        lastSpawnTime = Time.time;
        spawnCoolDown = 5f;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Start";
		//levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();
		boardScript.SetupScene (level);

        //read in the script file

        string jsonString="";
        string line=null;
        
        try
        {
            StreamReader reader= new StreamReader("Assets//jsonscript.txt", Encoding.Default);
            
            jsonString = reader.ReadToEnd();
            reader.Close();
        }
        catch(Exception e)
        {
            throw;
        }
        




        var N = JSON.Parse(jsonString);
        waveCount = N["waves"].AsInt;
        waves.Clear();
        for(int i=0;i<waveCount;i++)
        {
            waves.Add(N["enemyArray"][i]["type1"].AsInt);
        }

	}

	private void HideLevelImage()
	{
		levelImage.SetActive (false);
		doingSetup = false;
	}

	public void GameOver()
	{
		levelText.text = "You lose";
		levelImage.SetActive (true);
		enabled = false;
	}

    public void GameWin()
    {
        levelText.text = "You win!";
        levelImage.SetActive(true);
        enabled = false;
    }

    public void AttackEnemies(float y, int minusHp)
    {
        foreach(Enemy e in enemies)
        {
            if (e == null) continue;
            e.EnemyAttacked(y, minusHp);
        }
    }
	
	// Update is called once per frame
	void Update () {
		//if (playersTurn || enemiesMoving || doingSetup)
		//    return;
        //if (enemiesMoving || doingSetup)
          //  return;

        StartCoroutine (MoveEnemies ());

        if(Time.time-lastSpawnTime>spawnCoolDown && currentWave<waveCount)
        {
            if(currentWave>0)Spawn(waves[currentWave]);
            lastSpawnTime = Time.time;
            currentWave++;
        }
        else if(currentWave>=waveCount)
        {
            int count = 0;
            foreach(Enemy e in enemies)
            {
                if (e != null) count++;
            }
            if (count < 1) GameWin();
        }
	}

    private void Spawn(int s)
    {
        boardScript.LayoutObjectAtRandomSpawn(s);
    }

    public void AddEnemyToList(Enemy script)
	{
		enemies.Add (script);
	}

	IEnumerator MoveEnemies()
	{
        turnDelay = 0.5f;
        enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}
        lock(enemies)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                int count = 0;
                foreach (Enemy e in enemies)
                {
                    if (e != null) count++;
                }
                if (count < 1) break;
                if (enemies[i] == null) continue;
                if(enemies[i]!=null)enemies[i].MoveEnemy();
                /*
                if (enemies[i].hp <= 0)
                {
                    Enemy temp = enemies[i];
                    enemies.RemoveAt(i);
                    //temp.SetActive(false);
                    Destroy(temp);
                    i--;
                }*/

                if (enemies.Count < 1) yield return new WaitForSeconds(turnDelay);
                if (i < 0) yield return new WaitForSeconds(0.5f);
                yield return new WaitForSeconds(0.5f);
            }
        }

		playersTurn = true;
		enemiesMoving = false;
	}


}
