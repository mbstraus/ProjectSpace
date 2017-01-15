#region License
// ==============================================================================
// Project Space Copyright (C) 2016 Mathew Strauss
// ==============================================================================
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ProjectSpace.Models {

    /// <summary>
    /// Enum representing the directions in which a player can move. NONE is the "NULL" move direction.
    /// </summary>
    public enum MoveDirection { NONE, NORTH, SOUTH, EAST, WEST };

    /// <summary>
    /// Handles the representation and manipulation of the game board.
    /// </summary>
    public class GameBoard {
        /// <summary>
        /// Property representing whether the player is currently placing a room, which indicates that a preview room is currently being placed. 
        /// </summary>
        public bool IsPlacingRoom { get; protected set; }
        /// <summary>
        /// Dictionary of all currently placed rooms. Each room is indexed by its position in the world (X / Y coordinate)
        /// </summary>
        private Dictionary<Point, Room> rooms;
        /// <summary>
        /// List of room types available for placement.  Only one of each room type may be placed in the game board.
        /// </summary>
        private List<Room> roomTypes;
        private List<PlayerModel> players;
        /// <summary>
        /// Callback for when a player has moved into an existing room.
        /// </summary>
        private Action<float, float> existingMoveHandler;
        /// <summary>
        /// Callback for when a room preview has been spawned.
        /// </summary>
        private Action<Room, float, float, MoveDirection> spawnPreviewRoomHandler;
        /// <summary>
        /// Callback for when a preview room has been rotated.
        /// </summary>
        private Action rotatePreviewRoomHandler;
        /// <summary>
        /// Callback for when a room has been spawned.
        /// </summary>
        private Action<string, Point, bool> spawnRoomHandler;
        /// <summary>
        /// Callback for when the game board has just been initialized.
        /// </summary>
        private Action initializeGameBoardHandler;
        /// <summary>
        /// Callback for when a player has been spawned.
        /// </summary>
        private Action<int, float, float, bool> playerSpawnHandler;
        private Action playerUpdateHandler;
        private Action<PlayerModel, PlayerModel> turnEndHandler;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GameBoard() {
            rooms = new Dictionary<Point, Room>();
            roomTypes = new List<Room>();
            players = new List<PlayerModel>();
        }

        /// <summary>
        /// Initializes a new game board. Calls the initializeGameBoardHandler, and spawns the initial rooms and players.
        /// </summary>
        public void initializeNewGameBoard() {
            rooms.Clear();

            initializeGameBoardHandler();

            // TODO: Randomize the locations of the starting airlocks?
            spawnRoom("NorthAirlock", 0, 2, true, true, false, true, false);
            playerSpawnHandler(1, 0, 2, true);
            spawnRoom("WestAirlock", -2, 0, true, true, true, false, false);
            playerSpawnHandler(2, -2, 0, false);
            spawnRoom("SouthAirlock", 0, -2, false, true, true, true, false);
            playerSpawnHandler(3, 0, -2, false);
            spawnRoom("EastAirlock", 2, 0, true, false, true, true, false);
            playerSpawnHandler(4, 2, 0, false);

            turnEndHandler(players[0], players[0]);
        }

        public void LoadRooms() {
            XmlSerializer serializer = new XmlSerializer(typeof(Rooms));
            // serializer.UnknownNode += new XmlNodeEventHandler();
            // serializer.UnknownAttribute += new XmlAttributeEventHandler();
            FileStream fs = new FileStream("Assets/Resources/Data/Rooms.xml", FileMode.Open);
            Rooms rooms = (Rooms) serializer.Deserialize(fs);

            foreach (Room r in rooms.RoomsArray) {
                r.HasBeenPlaced = false;
                roomTypes.Add(r);
            }
        }

        /// <summary>
        /// Finds the room located at the specified X and Y coordinates.
        /// </summary>
        /// <param name="x">X position of the room</param>
        /// <param name="y">Y position of the room</param>
        /// <returns>Room at the location specified, or NULL if no room exists at the specified location.</returns>
        public Room getRoomAt(float x, float y) {
            Point p = new Point(x, y);
            Room r = null;
            rooms.TryGetValue(p, out r);
            return r;
        }

        /// <summary>
        /// Adds a room to the game board at the specified point.
        /// </summary>
        /// <param name="p">Point at which the room is located</param>
        /// <param name="r">Room being placed</param>
        public void addRoom(Point p, Room r) {
            rooms.Add(p, r);
        }

        /// <summary>
        /// Adds a room type to the list of available room types.
        /// </summary>
        /// <param name="r">Room to add as a room type</param>
        public void addRoomType(Room r) {
            roomTypes.Add(r);
        }

        public void addPlayer(PlayerModel player) {
            players.Add(player);
        }

        /// <summary>
        /// Marks a room type as having been used. A room type may only be added to the board once.
        /// </summary>
        /// <param name="name">Name of the room that was used</param>
        public void markRoomTypeAsUsed(string name) {
            Room r = new Room();
            r.Name = name;
            int index = roomTypes.IndexOf(r);
            roomTypes[roomTypes.IndexOf(r)].HasBeenPlaced = true;
        }

        /// <summary>
        /// Retrieves a random room from the list of available room types (HasBeenPlaced == false).
        /// </summary>
        /// <returns>Random room from the list of available room types (HasBeenPlaced == false)</returns>
        public Room getRandomUnusedRoomType() {
            List<Room> unusedRoomsList = new List<Room>();
            foreach (Room r in roomTypes) {
                if (r.HasBeenPlaced == false) {
                    unusedRoomsList.Add(r);
                }
            }
            if (unusedRoomsList.Count == 0) {
                return null;
            }
            return unusedRoomsList[UnityEngine.Random.Range(0, unusedRoomsList.Count)];
        }

        /// <summary>
        /// Handles a player request of moving into a room. The room can either be un-explorered (so we are going to reveal a new room), explored (so we just move into the room),
        /// or we are out of available rooms, so nothing happens.
        /// </summary>
        /// <param name="sourceX">Player's original X position</param>
        /// <param name="sourceY">Player's original Y position</param>
        /// <param name="moveDirection">Direction in which the player is moving</param>
        public void beginMoveIntoRoom(float sourceX, float sourceY, MoveDirection moveDirection) {
            Room originRoom = getRoomAt(sourceX, sourceY);
            float destX = 0f, destY = 0f;
            if (moveDirection == MoveDirection.NORTH) {
                if (originRoom.HasNorthExit == false) {
                    return;
                }
                destX = sourceX;
                destY = sourceY + 1f;
            } else if (moveDirection == MoveDirection.WEST) {
                if (originRoom.HasWestExit == false) {
                    return;
                }
                destX = sourceX - 1f;
                destY = sourceY;
            } else if (moveDirection == MoveDirection.SOUTH) {
                if (originRoom.HasSouthExit == false) {
                    return;
                }
                destX = sourceX;
                destY = sourceY - 1f;
            } else if (moveDirection == MoveDirection.EAST) {
                if (originRoom.HasEastExit == false) {
                    return;
                }
                destX = sourceX + 1f;
                destY = sourceY;
            }
            Room destinationRoom = getRoomAt(destX, destY);
            Room roomType = getRandomUnusedRoomType();
            if (destinationRoom == null && roomType != null) {
                Debug.LogFormat("Spawning preview room {0} at [ {1}, {2} ] with move direction {3}", roomType.Name, destX, destY, moveDirection);
                spawnPreviewRoomHandler(roomType, destX, destY, moveDirection);
                IsPlacingRoom = true;
            } else if (destinationRoom != null) {
                Debug.LogFormat("Moving into existing room {0} at [ {1}, {2} ] with move direction {3}", destinationRoom.Name, destX, destY, moveDirection);
                handleMoveToExistingRoom(destinationRoom, moveDirection);
            } else if (roomType == null) {
                // TODO: Do something when we are out of rooms!
                Debug.LogWarning("Out of rooms!");
            }
        }

        /// <summary>
        /// Handler for when the player moves into an existing room. First determines if the player CAN move into the room (both rooms have an exit in the move
        /// direction), and if they do, then the existingMoveHandler callback is executed.
        /// </summary>
        /// <param name="targetRoom">Room that the player is attempting to move into</param>
        /// <param name="direction">Direction the player is moving</param>
        private void handleMoveToExistingRoom(Room targetRoom, MoveDirection direction) {
            Point p = new Point(targetRoom.Point.X, targetRoom.Point.Y);
            if ((direction == MoveDirection.NORTH && targetRoom.HasSouthExit)
                || (direction == MoveDirection.WEST && targetRoom.HasEastExit)
                || (direction == MoveDirection.SOUTH && targetRoom.HasNorthExit)
                || (direction == MoveDirection.EAST && targetRoom.HasWestExit)) {
                existingMoveHandler(p.X, p.Y);
            } else {
                Debug.LogWarningFormat("Can't move into room [ {0}, {1} ] moving in direction {2}", p.X, p.Y, direction);
            }
        }

        /// <summary>
        /// Determines whether the current room preview's position and orientation, taking the player's move direction into
        /// account, is a valid position to place a room.
        /// </summary>
        /// <param name="moveDirection">Direction in which the player is moving</param>
        /// <param name="previewRoom">Preview room being placed</param>
        /// <returns>True if the preview room can be placed as a real room</returns>
        public bool canSpawnRoom(MoveDirection moveDirection, Room previewRoom) {
            if (moveDirection == MoveDirection.NORTH && previewRoom.HasSouthExit) {
                return true;
            }
            if (moveDirection == MoveDirection.WEST && previewRoom.HasEastExit) {
                return true;
            }
            if (moveDirection == MoveDirection.SOUTH && previewRoom.HasNorthExit) {
                return true;
            }
            if (moveDirection == MoveDirection.EAST && previewRoom.HasWestExit) {
                return true;
            }
            Debug.LogWarning("Can't place room here, exits are not connected!");
            return false;
        }

        /// <summary>
        /// Rotates the supplied room object and returns it. Also calls the rotatePreviewRoomHandler callback for the visual updates.
        /// </summary>
        /// <param name="previewRoom">Room being rotated.</param>
        /// <returns>The room that has been rotated.</returns>
        public Room rotatePreviewRoom(Room previewRoom) {
            // TODO: Allow for reverse rotation?
            bool oldNorth = previewRoom.HasNorthExit;
            bool oldWest = previewRoom.HasWestExit;
            bool oldSouth = previewRoom.HasSouthExit;
            bool oldEast = previewRoom.HasEastExit;

            previewRoom.HasNorthExit = oldEast;
            previewRoom.HasWestExit = oldNorth;
            previewRoom.HasSouthExit = oldWest;
            previewRoom.HasEastExit = oldSouth;

            rotatePreviewRoomHandler();
            return previewRoom;
        }

        /// <summary>
        /// Spawns a room at the specified position, with the specified exits. Room name corresponds to the room type name.
        /// </summary>
        /// <param name="name">Room type name being spawned</param>
        /// <param name="x">X position on the board to spawn the room at</param>
        /// <param name="y">Y position on the board to spawn the room at</param>
        /// <param name="hasNorthExit">If true, the room has a North facing exit</param>
        /// <param name="hasWestExit">If true, the room has a West facing exit</param>
        /// <param name="hasSouthExit">If true, the room has a South facing exit</param>
        /// <param name="hasEastExit">If true, the room has an East facing exit</param>
        /// <param name="isFromPreview">If true, the room is being spawned from a preview room. Used in callbacks</param>
        public void spawnRoom(string name, float x, float y, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit, bool isFromPreview) {
            Point point = new Point(x, y);
            Room room = new Room(name, point, hasNorthExit, hasWestExit, hasSouthExit, hasEastExit);
            addRoom(point, room);
            markRoomTypeAsUsed(name);

            spawnRoomHandler(name, point, isFromPreview);
            IsPlacingRoom = false;
            if (isFromPreview) {
                playerTurnEnd();
            }
        }

        public void handlePlayerUpdate() {
            if (playerUpdateHandler != null) {
                playerUpdateHandler();
            }
        }

        public void playerTurnEnd() {
            for (int i = 0; i < players.Count; i++) {
                PlayerModel p = players[i];
                if (p.IsActive) {
                    PlayerModel nextPlayer = players[(i + 1) % players.Count];
                    turnEndHandler(p, nextPlayer);
                    break;
                }
            }
        }

        /// <summary>
        /// Registers a new callback for when a player moves into an existing room.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void registerExistingRoomMoveAction(Action<float, float> handler) {
            existingMoveHandler += handler;
        }

        /// <summary>
        /// Unregisters a callback for when a player moves into an existing room.
        /// </summary>
        /// <param name="handler">Handler to unregister</param>
        public void unregisterExistingRoomMoveAction(Action<float, float> handler) {
            existingMoveHandler -= handler;
        }

        /// <summary>
        /// Registers a new callback for when a new room preview is spawned.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void registerSpawnPreviewRoomAction(Action<Room, float, float, MoveDirection> handler) {
            spawnPreviewRoomHandler += handler;
        }

        /// <summary>
        /// Unregisters a callback for when a new room preview is spawned.
        /// </summary>
        /// <param name="handler">Handler to unregister</param>
        public void unregisterSpawnPreviewRoomAction(Action<Room, float, float, MoveDirection> handler) {
            spawnPreviewRoomHandler -= handler;
        }

        /// <summary>
        /// Registers a new callback for when a room preview is rotated.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void registerRotatePreviewRoomHandler(Action handler) {
            rotatePreviewRoomHandler += handler;
        }

        /// <summary>
        /// Unregisters a callback for when a room preview is rotated.
        /// </summary>
        /// <param name="handler">Handler to unregister</param>
        public void unregisterRotatePreviewRoomHandler(Action handler) {
            rotatePreviewRoomHandler -= handler;
        }

        /// <summary>
        /// Registers a new callback for when a room is spawned.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void registerSpawnRoomHandler(Action<string, Point, bool> handler) {
            spawnRoomHandler += handler;
        }

        /// <summary>
        /// Unregisters a callback for when a room is spawned.
        /// </summary>
        /// <param name="handler">Handler to unregister</param>
        public void unregisterSpawnRoomHandler(Action<string, Point, bool> handler) {
            spawnRoomHandler -= handler;
        }

        /// <summary>
        /// Registers a new callback for when the game board is initialized.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void registerInitializeGameBoardHandler(Action handler) {
            initializeGameBoardHandler += handler;
        }

        /// <summary>
        /// Unregisters a callback for when the game board is initialized.
        /// </summary>
        /// <param name="handler">Handler to unregister</param>
        public void unregisterInitializeGameBoardHandler(Action handler) {
            initializeGameBoardHandler -= handler;
        }

        /// <summary>
        /// Registers a new callback for when a player is spawned.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void registerPlayerSpawnHandler(Action<int, float, float, bool> handler) {
            playerSpawnHandler += handler;
        }

        /// <summary>
        /// Unregisters a callback for when a player is spawned.
        /// </summary>
        /// <param name="handler">Handler to unregister</param>
        public void unregisterPlayerSpawnHandler(Action<int, float, float, bool> handler) {
            playerSpawnHandler -= handler;
        }

        public void registerPlayerUpdateHandler(Action handler) {
            playerUpdateHandler += handler;
        }

        public void unregisterPlayerUpdateHandler(Action handler) {
            playerUpdateHandler -= handler;
        }

        public void registerTurnEndHandler(Action<PlayerModel, PlayerModel> handler) {
            turnEndHandler += handler;
        }

        public void unregisterTurnEndHandler(Action<PlayerModel, PlayerModel> handler) {
            turnEndHandler -= handler;
        }
    }
}
