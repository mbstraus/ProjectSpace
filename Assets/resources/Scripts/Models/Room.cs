using System;
using System.Runtime.CompilerServices;

namespace ProjectSpace.Models
{
    public class Room
    {

        public Room()
        {

        }

        public Room(string roomName, Point p, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit)
        {
            this.Name = roomName;
            this.HasNorthExit = hasNorthExit;
            this.HasWestExit = hasWestExit;
            this.HasSouthExit = hasSouthExit;
            this.HasEastExit = hasEastExit;
            this.Point = p;
        }

        public Room(string roomName, bool hasBeenPlaced, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit)
        {
            this.Name = roomName;
            this.HasNorthExit = hasNorthExit;
            this.HasWestExit = hasWestExit;
            this.HasSouthExit = hasSouthExit;
            this.HasEastExit = hasEastExit;
            this.HasBeenPlaced = hasBeenPlaced;
        }

        public string Name { get; set; }
        public bool HasNorthExit { get; set; }
        public bool HasWestExit { get; set; }
        public bool HasSouthExit { get; set; }
        public bool HasEastExit { get; set; }
        public Point Point { get; set; }
        public bool HasBeenPlaced { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (RuntimeHelpers.ReferenceEquals(this, obj))
            {
                return true;
            }
            Room otherRoom = (Room)obj;
            return RuntimeHelpers.Equals(this.Name, otherRoom.Name) && RuntimeHelpers.Equals(this.Point, otherRoom.Point);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this.Name) + RuntimeHelpers.GetHashCode(this.Point);
        }
    }
}