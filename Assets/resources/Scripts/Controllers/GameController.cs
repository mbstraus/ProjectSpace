using System.Collections.Generic;
using UnityEngine;
using ProjectSpace.Models;

namespace ProjectSpace.Controllers
{
    public enum MoveDirection { NORTH, SOUTH, EAST, WEST };

    public class GameController : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameBoard GameBoard { get; private set; }

        private Dictionary<string, GameObject> roomGameObjects;
        private Dictionary<string, GameObject> roomPrefabs;
        private GameObject player;
        private MoveDirection moveDirection;

        private Room previewRoom;
        private GameObject previewRoomGameObject;

        /// <summary>
        /// Runs at startup, initialization
        /// </summary>
        void Start()
        {
            GameBoard = new GameBoard();
            Object[] resources = Resources.LoadAll("Prefabs/Rooms");
            roomPrefabs = new Dictionary<string, GameObject>();
            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i] is GameObject)
                {
                    GameObject go = (GameObject)resources[i];
                    roomPrefabs.Add(go.name, go);
                    Room r = null;
                    // TODO: This should be loaded from a file
                    if (go.name.Equals("NorthAirlock"))
                    {
                        r = new Room(go.name, false, true, true, false, true);
                    }
                    else if (go.name.Equals("WestAirlock"))
                    {
                        r = new Room(go.name, false, true, true, true, false);
                    }
                    else if (go.name.Equals("SouthAirlock"))
                    {
                        r = new Room(go.name, false, false, true, true, true);
                    }
                    else if (go.name.Equals("EastAirlock"))
                    {
                        r = new Room(go.name, false, true, true, true, false);
                    }
                    else if (go.name.Equals("CurvedHallway"))
                    {
                        r = new Room(go.name, false, true, true, false, false);
                    }
                    else if (go.name.Equals("Crossroad"))
                    {
                        r = new Room(go.name, false, true, true, true, true);
                    }
                    this.GameBoard.addRoomType(r);
                }
            }

            roomGameObjects = new Dictionary<string, GameObject>();

            spawnRoom("NorthAirlock", 0, 2, true, true, false, true);
            spawnRoom("WestAirlock", -2, 0, true, true, true, false);
            spawnRoom("SouthAirlock", 0, -2, false, true, true, true);
            spawnRoom("EastAirlock", 2, 0, true, false, true, true);

            player = Instantiate(playerPrefab, new Vector3(0f, 2f, 0f), Quaternion.identity);
        }

        /// <summary>
        /// Runs every frame
        /// </summary>
        void Update()
        {
            if (previewRoomGameObject != null)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    rotatePreviewRoom();
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

        /// <summary>
        /// Spawns a new room at the specified coordinates, and initializes it with the specified exits.
        /// </summary>
        /// <param name="name">Name of the room prefab to spawn.</param>
        /// <param name="x">X coordinate to place the room at</param>
        /// <param name="y">Y coordinate to place the room at</param>
        /// <param name="hasNorthExit">If true, the room has a north-facing exit</param>
        /// <param name="hasWestExit">If true, the room has a west-facing exit</param>
        /// <param name="hasSouthExit">If true, the room has a south-facing exit</param>
        /// <param name="hasEastExit">If true, the room has a east-facing exit</param>
        /// <returns>Instantiated game object representing the new room</returns>
        public GameObject spawnRoom(string name, float x, float y, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit)
        {
            Point point = new Point(x, y);
            GameObject roomGameObjectPrefab = null;
            roomPrefabs.TryGetValue(name, out roomGameObjectPrefab);
            GameObject roomGameObject = Instantiate(roomGameObjectPrefab, new Vector3(x, y, 0f), Quaternion.identity);
            Room room = new Room(name, point, hasNorthExit, hasWestExit, hasSouthExit, hasEastExit);
            GameBoard.addRoom(point, room);
            roomGameObjects.Add(name, roomGameObject);
            GameBoard.markRoomAsUsed(name);
            return roomGameObject;
        }

        /// <summary>
        /// Instantiates a real room at the position of the preview. This assumes that the room is in the correct orientation and can
        /// be placed at the preview's location. This will also clear the room preview from the world.
        /// </summary>
        public void spawnRoomFromPreview()
        {
            Transform t = previewRoomGameObject.transform;
            GameObject go = spawnRoom(previewRoom.Name, t.position.x, t.position.y, previewRoom.HasNorthExit, previewRoom.HasWestExit, previewRoom.HasSouthExit, previewRoom.HasEastExit);
            go.transform.rotation = t.rotation;
            player.GetComponent<Player>().placingRoom = false;
            player.transform.position = t.position;

            Destroy(previewRoomGameObject);
            previewRoom = null;
            previewRoomGameObject = null;
        }

        /// <summary>
        /// Spawns a "preview" room at the specified X and Y coordinates. The preview room is a slightly transparent version of the real
        /// room, and can be rotated in order for it to have a valid enterance according to the direction the player is moving in.
        /// </summary>
        /// <param name="x">X location to spawn the preview at</param>
        /// <param name="y">Y location to spawn the preview at</param>
        /// <param name="moveDirectionAttempted">Move direction the player is moving into the room at</param>
        public void spawnPreviewRoomAt(Room roomType, float x, float y, MoveDirection moveDirectionAttempted)
        {
            Point p = new Point(x, y);
            GameObject roomPrefab = null;

            roomPrefabs.TryGetValue(roomType.Name, out roomPrefab);

            GameObject go = Instantiate(roomPrefab, new Vector3(p.X, p.Y, 0f), Quaternion.identity);
            SpriteRenderer renderer = null;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (child.tag == "RoomVisual")
                {
                    renderer = child.GetComponent<SpriteRenderer>();
                }
            }
            if (renderer == null)
            {
                Debug.LogError("Couldn't find sprite renderer of room prefab!");
                return;
            }
            Color c = renderer.color;
            c.a = 0.5f;
            renderer.color = c;

            Room room = new Room(roomType.Name, p, roomType.HasNorthExit, roomType.HasWestExit, roomType.HasSouthExit, roomType.HasEastExit);

            previewRoomGameObject = go;
            previewRoom = room;
            moveDirection = moveDirectionAttempted;
        }

        /// <summary>
        /// Rotates the currently active preview room 90 degrees. This will update the game object's rotation, as well
        /// as modifying the exits to reflect the new orientation.
        /// </summary>
        public void rotatePreviewRoom()
        {
            previewRoomGameObject.transform.Rotate(new Vector3(0, 0, 90));

            bool oldNorth = previewRoom.HasNorthExit;
            bool oldWest = previewRoom.HasWestExit;
            bool oldSouth = previewRoom.HasSouthExit;
            bool oldEast = previewRoom.HasEastExit;

            previewRoom.HasNorthExit = oldEast;
            previewRoom.HasWestExit = oldNorth;
            previewRoom.HasSouthExit = oldWest;
            previewRoom.HasEastExit = oldSouth;
        }

        /// <summary>
        /// Determines whether the current room preview's position and orientation, taking the player's move direction into
        /// account, is a valid position to place a room.
        /// </summary>
        /// <returns>True if the preview room can be placed as a real room</returns>
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