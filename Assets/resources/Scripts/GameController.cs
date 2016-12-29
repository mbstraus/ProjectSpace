using System.Collections.Generic;
using UnityEngine;

namespace ProjectSpace
{
    public enum MoveDirection { NORTH, SOUTH, EAST, WEST };

    public class GameController : MonoBehaviour
    {

        private Dictionary<Point, Room> rooms;
        public GameObject roomPrefab;
        public GameObject playerPrefab;

        private Dictionary<string, Sprite> roomTextures;
        private Room previewRoom;
        private Point previewPoint;
        private GameObject player;
        private MoveDirection moveDirection;

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

            player = Instantiate(playerPrefab, new Vector3(room1Point.X, room1Point.Y, 0f), Quaternion.identity);
        }

        void Update()
        {
            if (previewRoom != null)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    rotateRoom();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (canSpawnRoom())
                    {
                        spawnRoomFromPreview();
                    }
                }
            }
        }

        public Room getRoomAt(Point p)
        {
            Room r = null;
            rooms.TryGetValue(p, out r);
            return r;
        }

        public void spawnRoomFromPreview()
        {
            Transform t = previewRoom.GameObject.transform;


            Sprite texture = null;
            roomTextures.TryGetValue("Room_N_W", out texture);

            GameObject go = Instantiate(roomPrefab);
            go.GetComponent<SpriteRenderer>().sprite = texture;


            go.transform.position = t.position;
            go.transform.rotation = t.rotation;
            player.GetComponent<Player>().keyPressed = false;
            player.transform.position = t.position;

            Room room = new Room(go, previewRoom.HasNorthExit, previewRoom.HasWestExit, previewRoom.HasSouthExit, previewRoom.HasEastExit);
            rooms.Add(previewPoint, room);

            Destroy(previewRoom.GameObject);
            previewRoom = null;
            previewPoint = null;
        }

        public void spawnPreviewRoomAt(Point p, MoveDirection moveDirectionAttempted)
        {
            Sprite texture = null;
            roomTextures.TryGetValue("Room_N_W", out texture);

            GameObject go = Instantiate(roomPrefab, new Vector3(p.X, p.Y, 0f), Quaternion.identity);
            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            renderer.sprite = texture;
            Color c = renderer.color;
            c.a = 0.5f;
            renderer.color = c;

            Room room = new Room(go, true, true, false, false);

            previewRoom = room;
            previewPoint = p;
            moveDirection = moveDirectionAttempted;
        }

        public void rotateRoom()
        {
            previewRoom.GameObject.transform.Rotate(new Vector3(0, 0, 90));

            bool oldNorth = previewRoom.HasNorthExit;
            bool oldWest = previewRoom.HasWestExit;
            bool oldSouth = previewRoom.HasSouthExit;
            bool oldEast = previewRoom.HasEastExit;

            previewRoom.HasNorthExit = oldEast;
            previewRoom.HasWestExit = oldNorth;
            previewRoom.HasSouthExit = oldWest;
            previewRoom.HasEastExit = oldSouth;
        }

        public bool canSpawnRoom()
        {
            if (moveDirection == MoveDirection.NORTH && previewRoom.HasSouthExit)
            {
                return true;
            }
            if (moveDirection == MoveDirection.WEST && previewRoom.HasEastExit)
            {
                return true;
            }
            if (moveDirection == MoveDirection.SOUTH && previewRoom.HasNorthExit)
            {
                return true;
            }
            if (moveDirection == MoveDirection.EAST && previewRoom.HasWestExit)
            {
                return true;
            }
            Debug.LogWarning("Can't place room here, exits are not connected!");
            return false;
        }
    }
}