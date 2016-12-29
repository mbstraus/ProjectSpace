using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSpace
{
    public class Room
    {

        public Room()
        {

        }

        public Room(GameObject go, bool hasNorthExit, bool hasWestExit, bool hasSouthExit, bool hasEastExit)
        {
            this.GameObject = go;
            this.HasNorthExit = hasNorthExit;
            this.HasWestExit = hasWestExit;
            this.HasSouthExit = hasSouthExit;
            this.HasEastExit = hasEastExit;
        }

        public GameObject GameObject { get; set; }
        public bool HasNorthExit { get; set; }
        public bool HasWestExit { get; set; }
        public bool HasSouthExit { get; set; }
        public bool HasEastExit { get; set; }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}