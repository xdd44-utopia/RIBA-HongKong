using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
	public GameObject prefab;

	private const int n = 5;
	private const float cellSize = 3;

	private int cx = 0;
	private int cy = 0;
	private int cd = 0;
	private int[] dx = {1, 0, -1, 0};
	private int[] dy = {0, 1, 0, -1};
	private Quaternion[] rots = {
		Quaternion.Euler(0, 0, 90),
		Quaternion.Euler(-90, 0, 0),
		Quaternion.Euler(0, 0, -90),
		Quaternion.Euler(90, 0, 0),
		Quaternion.Euler(0, 0, 180),
		Quaternion.Euler(0, 0, 0)
	};
	private Vector3[] poss = {
		new Vector3(0.5f, 0.5f, 0),
		new Vector3(0, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0),
		new Vector3(0, 0.5f, -0.5f),
		new Vector3(0, 1, 0),
		new Vector3(0, 0, 0)
	};
	private struct Cell {
		public int idx;
		public bool[] adj;
		public int father;

		public Cell(int i) {
			idx = i;
			adj = new bool[4]{false, false, false, false};
			father = idx;
		}
	}
	private Cell[] map = new Cell[n * n];
	private int[] steps = new int[n * n];
	private int curStep = 0;
	private CameraController cam;

	void Start()
	{
		cam = Camera.main.gameObject.GetComponent<CameraController>();
		for (int i=0;i<n*n;i++) {
			map[i] = new Cell(i);
		}
		for (int i=0;i<n*n-1;i++) {
			int px = -1;
			int py = -1;
			int lx = -1;
			int ly = -1;
			int dir = -1;
			int cnt = 0;
			do {
				px = Random.Range(0, n);
				py = px == n - 1 ? Random.Range(0, n - 1) : Random.Range(0, n);
				dir = px == n - 1 ? 1 : (py == n - 1 ? 0 : Random.Range(0, 2));
				lx = px + dx[dir];
				ly = py + dy[dir];
				cnt++;
			} while (find(px * n + py) == find(lx * n + ly) && cnt < 100);
			union(px * n + py, lx * n + ly);
			map[px * n + py].adj[dir] = true;
			map[lx * n + ly].adj[dir + 2] = true;
		}
		for (int i=0;i<n*n;i++) {
			for (int d=0;d<4;d++) {
				if (!map[i].adj[d]) {
					Instantiate(prefab, new Vector3(cellSize * (i / n), 0, cellSize * (i % n)) + poss[d] * cellSize, rots[d]);
				}
			}
			Instantiate(prefab, new Vector3(cellSize * (i / n), 0, cellSize * (i % n)) + poss[4] * cellSize, rots[4]);
			Instantiate(prefab, new Vector3(cellSize * (i / n), 0, cellSize * (i % n)) + poss[5] * cellSize, rots[5]);
		}
		steps[0] = -1;
	}

	// Update is called once per frame
	void Update()
	{
		cx = (int)Mathf.Floor((Camera.main.transform.position.x + cellSize / 2) / cellSize);
		cy = (int)Mathf.Floor((Camera.main.transform.position.z + cellSize / 2) / cellSize);
		if (steps[cx * n + cy] < curStep) {
			curStep++;
			steps[cx * n + cy] = curStep;
			int prevcd = cd;
			while (cx + dx[cd] < 0 || cy + dy[cd] < 0 || cx + dx[cd] >= n || cy + dy[cd] >= n || !map[(cx) * n + cy].adj[cd] ||
				((map[(cx) * n + cy].adj[prevcd ^ 1] || map[(cx) * n + cy].adj[prevcd ^ 1 ^ 2]) && Random.Range(0, 2) == 0)) {
				if (map[(cx) * n + cy].adj[prevcd ^ 1] || map[(cx) * n + cy].adj[prevcd ^ 1 ^ 2]) {
					cd = Random.Range(0, 2) == 0 ? prevcd ^ 1 : prevcd ^ 1 ^ 2;
				}
				else {
					cd = prevcd ^ 2;
				}
			}
			cam.addTarget(new Vector3((cx + dx[cd]) * cellSize, 1.5f, (cy + dy[cd]) * cellSize));
		}
	}

	private void union(int x, int y) {
		if (find(x) != find(y)) {
			map[find(x)].father = map[y].father;
		}
	}

	private int find(int x) {
		int p = map[x].father;
		while (p != map[p].father) {
			p = map[p].father;
		}
		map[x].father = p;
		return p;
	}
}
