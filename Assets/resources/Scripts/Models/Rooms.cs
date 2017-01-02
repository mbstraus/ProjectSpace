#region License
// ==============================================================================
// Project Space Copyright (C) 2016 Mathew Strauss
// ==============================================================================
#endregion

using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ProjectSpace.Models {
    [XmlRoot("Rooms", IsNullable = false)]
    public class Rooms {
        [XmlArray("RoomList")]
        public Room[] RoomsArray;
    }
}
