#region License
// ==============================================================================
// Project Space Copyright (C) 2016 Mathew Strauss
// ==============================================================================
#endregion

using System.Collections.Generic;
using UnityEngine;
using ProjectSpace.Models;

namespace ProjectSpace.Controllers {

    /// <summary>
    /// Game Controller game object. Handles the visual aspects of the game board.
    /// </summary>
    public class GameController : MonoBehaviour {
        /// <summary>
        /// Represents the player game object that will be instantiated.
        /// </summary>
        public GameObject playerPrefab;
        /// <summary>
        /// Game object model that handles all of the logic dealing with manipulating the game board.
        /// </summary>
        public GameBoard GameBoard { get; private set; }

        /// <summary>
        /// Dictionary containing a look-up for all of the game objects in the game world representing the game board. Indexed by the room name.
        /// </summary>
        private Dictionary<string, GameObject> roomGameObjects;
        /// <summary>
        /// Dictionary containing a look-up for all of the room prefab objects, used for instantiating rooms. Indexed by the room name.
        /// </summary>
        private Dictionary<string, GameObject> roomPrefabs;
        /// <summary>
        /// Direction in which the player is currently moving for a preview room.
        /// </summary>
        private MoveDirection moveDirection;
        /// <summary>
        /// Room representing the preview room being currently manipulated.
        /// </summary>
        private Room previewRoom;
        /// <summary>
        /// Game object representing the preview room being currently manipulated.
        /// </summary>
        private GameObject previewRoomGameObject;
        private Object[] resources;

        /// <summary>
        /// Runs at startup, initialization
        /// </summary>
        void Start() {
            GameBoard = new GameBoard();
            GameBoard.registerSpawnPreviewRoomAction(spawnPreviewRoomAt);
            GameBoard.registerRotatePreviewRoomHandler(rotatePreviewRoom);
            GameBoard.registerSpawnRoomHandler(spawnRoom);
            GameBoard.registerPlayerSpawnHandler(spawnPlayer);
            resources = Resources.LoadAll("Prefabs/Rooms");
            roomPrefabs = new Dictionary<string, GameObject>();
            for (int i = 0; i < resources.Length; i++) {
                if (resources[i] is GameObject) {
                    GameObject go = (GameObject) resources[i];
                    roomPrefabs.Add(go.name, go);
                }
            }
            GameBoard.LoadRooms();
            GameBoard.registerInitializeGameBoardHandler(initializeGameBoard);
            GameBoard.initializeNewGameBoard();
        }

        /// <summary>
        /// Runs every frame
        /// </summary>
        void Update() {
            if (previewRoomGameObject != null) {
                if (Input.GetKeyDown(KeyCode.R)) {
                    GameBoard.rotatePreviewRoom(previewRoom);
                }
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (GameBoard.canSpawnRoom(moveDirection, previewRoom)) {
                        GameBoard.spawnRoom(previewRoom.Name, previewRoom.Point.X, previewRoom.Point.Y, previewRoom.HasNorthExit, previewRoom.HasWestExit, previewRoom.HasSouthExit, previewRoom.HasEastExit, true);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the game board for a new game. Clears any existing rooms from the game world.
        /// </summary>
        public void initializeGameBoard() {
            // TODO: Should this be responsible for clearing out the players too?  Probably
            // Clear out any existing rooms here.
            if (roomGameObjects != null) {
                foreach (GameObject roomGameObject in roomGameObjects.Values) {
                    Destroy(roomGameObject);
                }
            }
            roomGameObjects = new Dictionary<string, GameObject>();
            previewRoom = null;
            previewRoomGameObject = null;
            moveDirection = MoveDirection.NONE;
        }

        /// <summary>
        /// Spawns a player at the specified X and Y location.
        /// </summary>
        /// <param name="playerIndex">Player's index, 1 - 4, representing the player being spawned</param>
        /// <param name="x">X position of the room the player is being spawned at</param>
        /// <param name="y">Y position of the room the player is being spawned at</param>
        public void spawnPlayer(int playerIndex, float x, float y) {
            // TODO: Maybe have anchor points for each player?  This will result in the players being on top of each other if I keep it this way.
            Instantiate(playerPrefab, new Vector3(x, y, 0f), Quaternion.identity);
            // TODO: Need to do more initialization stuff here for the player
        }

        /// <summary>
        /// Spawns a room at the specified point. If the room is being spawned from a preview, the preview room will be removed.
        /// </summary>
        /// <param name="name">Name of the room prefab being spawned</param>
        /// <param name="p">Point in the world where the room is being spawned</param>
        /// <param name="isFromPreview">If true, the room is being spawned from a preview room (player discovered)</param>
        private void spawnRoom(string name, Point p, bool isFromPreview) {
            GameObject roomGameObjectPrefab = null;
            roomPrefabs.TryGetValue(name, out roomGameObjectPrefab);
            GameObject roomGameObject = Instantiate(roomGameObjectPrefab, new Vector3(p.X, p.Y, 0f), Quaternion.identity);
            if (isFromPreview) {
                spawnRoomFromPreview(roomGameObject);
            }
            if (roomGameObjects.ContainsKey(name)) {
                Debug.LogErrorFormat("Room with name {0} already exists?", name);
            }
            roomGameObjects.Add(name, roomGameObject);
        }

        /// <summary>
        /// Handles the additional logic of spawning a room from a preview. Removes the preview room and orients the new room to
        /// the same rotation as the preview room.
        /// </summary>
        /// <param name="spawningRoomGameObject">Game object representing the new room</param>
        private void spawnRoomFromPreview(GameObject spawningRoomGameObject) {
            Transform t = previewRoomGameObject.transform;
            spawningRoomGameObject.transform.rotation = t.rotation;

            Destroy(previewRoomGameObject);
            previewRoom = null;
            previewRoomGameObject = null;
        }

        /// <summary>
        /// Spawns a "preview" room at the specified X and Y coordinates. The preview room is a slightly transparent version of the real
        /// room, and can be rotated in order for it to have a valid enterance according to the direction the player is moving in.
        /// </summary>
        /// <param name="roomType">Room type of the room to spawn the preview for</param>
        /// <param name="x">X location to spawn the preview at</param>
        /// <param name="y">Y location to spawn the preview at</param>
        /// <param name="moveDirectionAttempted">Move direction the player is moving into the room at</param>
        public void spawnPreviewRoomAt(Room roomType, float x, float y, MoveDirection moveDirectionAttempted) {
            Point p = new Point(x, y);
            GameObject roomPrefab = null;

            roomPrefabs.TryGetValue(roomType.Name, out roomPrefab);

            GameObject go = Instantiate(roomPrefab, new Vector3(p.X, p.Y, 0f), Quaternion.identity);
            SpriteRenderer renderer = null;
            for (int i = 0; i < go.transform.childCount; i++) {
                Transform child = go.transform.GetChild(i);
                if (child.tag == "RoomVisual") {
                    renderer = child.GetComponent<SpriteRenderer>();
                }
            }
            if (renderer == null) {
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
        /// Rotates the currently active preview room 90 degrees.
        /// </summary>
        public void rotatePreviewRoom() {
            // TODO: Should probably allow for reverse rotation here too.
            previewRoomGameObject.transform.Rotate(new Vector3(0, 0, 90));
        }
    }
}