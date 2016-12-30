using System;
using System.Collections.Generic;

namespace ProjectSpace.Models
{
    // TODO: Replace with a file or something?


    public class GameBoard
    {
        private Dictionary<Point, Room> rooms;
        private List<Room> roomTypes;

        public GameBoard()
        {
            rooms = new Dictionary<Point, Room>();
            roomTypes = new List<Room>();
        }

        /// <summary>
        /// Finds the room located at the specified X and Y coordinates.
        /// </summary>
        /// <param name="x">X position of the room</param>
        /// <param name="y">Y position of the room</param>
        /// <returns>Room at the location specified, or NULL if no room exists at the specified location.</returns>
        public Room getRoomAt(float x, float y)
        {
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
        public void addRoom(Point p, Room r)
        {
            rooms.Add(p, r);
        }

        public void addRoomType(Room r)
        {
            roomTypes.Add(r);
        }

        public void markRoomAsUsed(string name)
        {
            Room r = new Room();
            r.Name = name;
            roomTypes[roomTypes.IndexOf(r)].HasBeenPlaced = true;
        }

        public Room getRandomUnusedRoomType()
        {
            List<Room> unusedRoomsList = new List<Room>();
            foreach (Room r in roomTypes)
            {
                if (r.HasBeenPlaced == false)
                {
                    unusedRoomsList.Add(r);
                }
            }
            if (unusedRoomsList.Count == 0)
            {
                return null;
            }
            return unusedRoomsList[UnityEngine.Random.Range(0, unusedRoomsList.Count)];
        }
    }
}
