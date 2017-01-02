#region License
// ==============================================================================
// Project Space Copyright (C) 2016 Mathew Strauss
// ==============================================================================
#endregion

using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectSpace.Models {
    /// <summary>
    /// Represents a room on the game board.
    /// </summary>
    [XmlRootAttribute("Room")]
    public class Room {

        /// <summary>
        /// Name of the room
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// If true, the room has a North facing exit
        /// </summary>
        [XmlAttribute]
        public bool HasNorthExit { get; set; }
        /// <summary>
        /// If true, the room has a West facing exit
        /// </summary>
        [XmlAttribute]
        public bool HasWestExit { get; set; }
        /// <summary>
        /// If true, the room has a South facing exit
        /// </summary>
        [XmlAttribute]
        public bool HasSouthExit { get; set; }
        /// <summary>
        /// If true, the room has an East facing exit
        /// </summary>
        [XmlAttribute]
        public bool HasEastExit { get; set; }
        /// <summary>
        /// Point on the game board that the room is being placed (X & Y position)
        /// </summary>
        [XmlElement("Point", IsNullable = true)]
        public Point Point { get; set; }
        /// <summary>
        /// If true, the room has been placed on the game board
        /// </summary>
        [XmlAttribute]
        public bool HasBeenPlaced { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Room() {

        }

        /// <summary>
        /// Constructor used for when a room exists on the game board at an X and Y position.
        /// </summary>
        /// <param name="roomName">Name of the room</param>
        /// <param name="p">Point on the game board that the room is being placed (X & Y position)</param>
        /// <param name="hasNorthExit">If true, the room has a North facing exit</param>
        /// <param name="hasWestExit">If true, the room has a West facing exit</param>
        /// <param name="hasSouthExit">If true, the room has a South facing exit</param>
        /// <param name="hasEastExit">If true, the room has an East facing exit</param>
        public Room(string roomName, Point p, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit) {
            this.Name = roomName;
            this.HasNorthExit = hasNorthExit;
            this.HasWestExit = hasWestExit;
            this.HasSouthExit = hasSouthExit;
            this.HasEastExit = hasEastExit;
            this.Point = p;
        }

        /// <summary>
        /// Constructor used for defining a type of room, that hasn't been placed on the game board.
        /// </summary>
        /// <param name="roomName">Name of the room</param>
        /// <param name="hasBeenPlaced">If true, the room has been placed on the game board</param>
        /// <param name="hasNorthExit">If true, the room has a North facing exit</param>
        /// <param name="hasWestExit">If true, the room has a West facing exit</param>
        /// <param name="hasSouthExit">If true, the room has a South facing exit</param>
        /// <param name="hasEastExit">If true, the room has an East facing exit</param>
        public Room(string roomName, bool hasBeenPlaced, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit) {
            this.Name = roomName;
            this.HasNorthExit = hasNorthExit;
            this.HasWestExit = hasWestExit;
            this.HasSouthExit = hasSouthExit;
            this.HasEastExit = hasEastExit;
            this.HasBeenPlaced = hasBeenPlaced;
        }

        /// <summary>
        /// Determines equality of two rooms. Equality is determined by the name of the room, and the position of the room in the game board.
        /// </summary>
        /// <param name="obj">Other object to check for equality</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            Room otherRoom = (Room) obj;
            return RuntimeHelpers.Equals(Name, otherRoom.Name);
        }

        /// <summary>
        /// Gets the hashcode of the room, determined by the name and point.
        /// </summary>
        /// <returns>Hashcode of the room</returns>
        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}