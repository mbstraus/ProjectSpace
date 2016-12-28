using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

    private Dictionary<Point, Room> rooms;
    public GameObject roomPrefab;
    public GameObject playerPrefab;
    
	void Start () {
        rooms = new Dictionary<Point, Room>();

        // Add the four starting airlocks in a diamond shape.
        Point room1Point = new Point(0, 2);
        Point room2Point = new Point(-2, 0);
        Point room3Point = new Point(0, -2);
        Point room4Point = new Point(2, 0);

        Room room1 = new Room(roomPrefab, true, true, false, true);
        Room room2 = new Room(roomPrefab, true, true, true, false);
        Room room3 = new Room(roomPrefab, false, true, true, true);
        Room room4 = new Room(roomPrefab, true, false, true, true);

        rooms.Add(room1Point, room1);
        rooms.Add(room2Point, room2);
        rooms.Add(room3Point, room3);
        rooms.Add(room4Point, room4);

        Instantiate(room1.GameObject, new Vector3(room1Point.X, room1Point.Y, 0f), Quaternion.identity);
        Instantiate(room2.GameObject, new Vector3(room2Point.X, room2Point.Y, 0f), Quaternion.identity);
        Instantiate(room3.GameObject, new Vector3(room3Point.X, room3Point.Y, 0f), Quaternion.identity);
        Instantiate(room4.GameObject, new Vector3(room4Point.X, room4Point.Y, 0f), Quaternion.identity);

        Instantiate(playerPrefab, new Vector3(room1Point.X, room1Point.Y, 0f), Quaternion.identity);
    }
	
	void Update () {
		
	}

    public Room getRoomAt(Point p)
    {
        Room r = null;
        rooms.TryGetValue(p, out r);
        return r;
    }

    public void spawnRoomAt(Point p)
    {
        Room room = new Room(roomPrefab, true, true, true, true);
        rooms.Add(p, room);
        Instantiate(room.GameObject, new Vector3(p.X, p.Y, 0f), Quaternion.identity);
    }
}
