using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindSpawner : MonoBehaviour
{
	public GameObject prefab;
	private int n = 100;
	void Start()
	{
		for (int i=0;i<n;i++) {
			float px = Random.Range(0, MazeGenerator.n * MazeGenerator.cellSize) - MazeGenerator.cellSize / 2;
			float py = Random.Range(0, MazeGenerator.n * MazeGenerator.cellSize)- MazeGenerator.cellSize / 2;
			float wx = Random.Range(0.25f, MazeGenerator.cellSize / 2);
			float wy = Random.Range(0.25f, MazeGenerator.cellSize / 2);
			float h = Random.Range(MazeGenerator.cellSize / 4, MazeGenerator.cellSize / 2);
			GameObject blind = Instantiate(prefab, new Vector3(px, h / 2, py), Quaternion.identity);
			blind.transform.localScale = new Vector3(wx, h, wy);
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
