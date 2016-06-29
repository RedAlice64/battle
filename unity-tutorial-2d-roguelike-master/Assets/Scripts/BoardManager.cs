using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 1;
	public int rows = 1;
	public Count wallCount = new Count (1,2);
	public Count foodCount = new Count (1,2);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;
	private List <Vector3> gridPositions = new List <Vector3> ();

	void InitialiseList()
	{
        columns = 8;
        rows = 3;
        gridPositions.Clear ();

		for (int x = 4; x < columns - 1; x++) {
			for (int y = 0; y < rows ; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup ()
	{
        columns = 8;
        rows = 3;
        boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 )
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent (boardHolder);
			}
		}
	}

	Vector3 RandomPosition()
	{
        columns = 8;
        rows = 3;
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{

		for (int i = 0; i < maximum; i++) {
			Vector3 randomPosition = RandomPosition ();
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
		}
	}

    public void LayoutObjectAtRandomSpawn(int count)
    {

        List<Vector3> positions = new List<Vector3>(){new Vector3(7f,0f,0f), new Vector3(7f,1f,0f), new Vector3(7f,2f,0f) };
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, positions.Count);
            Vector3 randomPosition = positions[randomIndex];
            positions.RemoveAt(randomIndex);
            GameObject tileChoice = enemyTiles[Random.Range(0, enemyTiles.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
	{
        columns = 8;
        rows = 3;
        BoardSetup ();
		InitialiseList ();
		//LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		//LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
		int enemyCount = (int)Mathf.Log (level, 2f);
        enemyCount = 3;
        LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		//Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0F), Quaternion.identity);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
