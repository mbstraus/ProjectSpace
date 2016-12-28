using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

    private Dictionary<Point, Room> rooms;
    public GameObject roomPrefab;
    public GameObject playerPrefab;

    private Dictionary<string, Sprite> roomTextures;

    void Start()
    {
        Object[] resources = Resources.LoadAll("Rooms/Images");
        roomTextures = new Dictionary<string, Sprite>();
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i] is Sprite)
            {
                Sprite texture2D = (Sprite)resources[i];
                roomTextures.Add(texture2D.name, texture2D);
                Debug.Log(texture2D.name);
            }
        }

        rooms = new Dictionary<Point, Room>();

        // Add the four starting airlocks in a diamond shape.
        Point room1Point = new Point(0, 2);
        Point room2Point = new Point(-2, 0);
        Point room3Point = new Point(0, -2);
        Point room4Point = new Point(2, 0);
        Sprite airlockTexture = null;
        roomTextures.TryGetValue("Room_N_W_S", out airlockTexture);

        GameObject room1GO = Instantiate(roomPrefab, new Vector3(room1Point.X, room1Point.Y, 0f), Quaternion.identity);
        room1GO.GetComponent<SpriteRenderer>().sprite = airlockTexture;
        room1GO.transform.Rotate(0, 0, -90);
        GameObject room2GO = Instantiate(roomPrefab, new Vector3(room2Point.X, room2Point.Y, 0f), Quaternion.identity);
        room2GO.GetComponent<SpriteRenderer>().sprite = airlockTexture;
        GameObject room3GO = Instantiate(roomPrefab, new Vector3(room3Point.X, room3Point.Y, 0f), Quaternion.identity);
        room3GO.GetComponent<SpriteRenderer>().sprite = airlockTexture;
        room3GO.transform.Rotate(0, 0, 90);
        GameObject room4GO = Instantiate(roomPrefab, new Vector3(room4Point.X, room4Point.Y, 0f), Quaternion.identity);
        room4GO.GetComponent<SpriteRenderer>().sprite = airlockTexture;
        room4GO.transform.Rotate(0, 0, 180);

        Room room1 = new Room(room1GO, true, true, false, true);
        Room room2 = new Room(room2GO, true, true, true, false);
        Room room3 = new Room(room3GO, false, true, true, true);
        Room room4 = new Room(room4GO, true, false, true, true);

        rooms.Add(room1Point, room1);
        rooms.Add(room2Point, room2);
        rooms.Add(room3Point, room3);
        rooms.Add(room4Point, room4);

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
        Sprite texture = null;
        roomTextures.TryGetValue("Room_N_W_S_E", out texture);

        GameObject go = Instantiate(roomPrefab, new Vector3(p.X, p.Y, 0f), Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = texture;

        Room room = new Room(roomPrefab, true, true, true, true);
        rooms.Add(p, room);
    }
}
