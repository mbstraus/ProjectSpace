using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSpace
{
    public class Player : MonoBehaviour
    {

        private GameController gameController;
        public bool keyPressed = false;

        // Use this for initialization
        void Start()
        {
            gameController = FindObjectOfType<GameController>();
        }

        // Update is called once per frame
        void Update()
        {
            Point p = new Point(transform.position.x, transform.position.y);
            MoveDirection moveDirection = MoveDirection.NORTH;
            Room originRoom = gameController.getRoomAt(p);

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (originRoom.HasWestExit == false)
                {
                    return;
                }
                p.X -= 1f;
                keyPressed = true;
                moveDirection = MoveDirection.WEST;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (originRoom.HasEastExit == false)
                {
                    return;
                }
                p.X += 1f;
                keyPressed = true;
                moveDirection = MoveDirection.EAST;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (originRoom.HasNorthExit == false)
                {
                    return;
                }
                p.Y += 1f;
                keyPressed = true;
                moveDirection = MoveDirection.NORTH;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (originRoom.HasSouthExit == false)
                {
                    return;
                }
                p.Y -= 1f;
                keyPressed = true;
                moveDirection = MoveDirection.SOUTH;
            }
            if (keyPressed)
            {
                Room r = gameController.getRoomAt(p);
                if (r == null)
                {
                    gameController.spawnPreviewRoomAt(p, moveDirection);
                }
                else
                {
                    handleMoveToExistingRoom(r, p, moveDirection);
                }
            }
        }

        private void handleMoveToExistingRoom(Room targetRoom, Point p, MoveDirection direction)
        {
            if ((direction == MoveDirection.NORTH && targetRoom.HasSouthExit)
                || (direction == MoveDirection.WEST && targetRoom.HasEastExit)
                || (direction == MoveDirection.SOUTH && targetRoom.HasNorthExit)
                || (direction == MoveDirection.EAST && targetRoom.HasWestExit))
            {
                this.transform.position = new Vector3(p.X, p.Y, 0);
            }
            else
            {
                Debug.LogWarningFormat("Can't move to room at position [{0}, {1}], moving in direction {2}", p.X, p.Y, direction);
            }
            keyPressed = false;
        }
    }
}
