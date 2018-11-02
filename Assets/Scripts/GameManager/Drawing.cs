using UnityEngine;
using System.Collections;

public class Drawing : MonoBehaviour {

	public Board board{ get; set;}
	private GameObject[,] pieces;

	public Drawing(Board b){
		board = b;
	}

	// Use this for initialization
	public void Start () {
		pieces = new GameObject[10,18];

				for (int y = 0; y < 18; y++) {
					for (int x = 0; x < 10; x++) {
						pieces[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
						//pieces[x,y].AddComponent<RigidbodyD>();
						pieces[x,y].GetComponent<Renderer>().material.color = Color.red;
						pieces[x,y].transform.position = new Vector3(x, 0, y);
				pieces[x,y].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

					}
				}
	}
	
	// Update is called once per frame
	public void Update () {
		for (int y = 0; y < 18; y++) {
			for (int x = 0; x < 10; x++) {
				pieces [x,y].SetActive (board.board[x,y]);
			}
		}
//		var cube = GameObject.Find("Cube");
//		var cube2 = GameObject.Find("Cube (1)");
////		if (board != null) {
//			if (board.board [0, 0])
//				cube.SetActive (false);
//			else
//				cube.SetActive (true);
//			if (board.board [0, 1])
//				cube2.SetActive (false);
//			else
//				cube2.SetActive (true);
//		}
	}
}
