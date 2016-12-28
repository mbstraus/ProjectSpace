using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private GameController gameController;
    private bool keyPressed = false;

	// Use this for initialization
	void Start () {
        gameController = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        Point p = new Point(transform.position.x, transform.position.y);
        Room originRoom = gameController.getRoomAt(p);

		if( Input.GetKeyDown(KeyCode.LeftArrow) )
        {
            if (originRoom.HasWestExit == false)
            {
                return;
            }
            p.X -= 1f;
            keyPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (originRoom.HasEastExit == false)
            {
                return;
            }
            p.X += 1f;
            keyPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (originRoom.HasNorthExit == false)
            {
                return;
            }
            p.Y += 1f;
            keyPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (originRoom.HasSouthExit == false)
            {
                return;
            }
            p.Y -= 1f;
            keyPressed = true;
        }
        if (keyPressed)
        {
            Room r = gameController.getRoomAt(p);
            if (r == null)
            {
                gameController.spawnRoomAt(p);
            }
            this.transform.position = new Vector3(p.X, p.Y, 0);
            keyPressed = false;
        }
    }
}
