using UnityEngine;
using ProjectSpace.Models;

namespace ProjectSpace.Controllers
{
    public class Player : MonoBehaviour
    {

        private GameController gameController;
        private bool keyPressed = false;
        public bool placingRoom = false;

        // Use this for initialization
        void Start()
        {
            gameController = FindObjectOfType<GameController>();
        }

        // Update is called once per frame
        void Update()
        {
            float x = transform.position.x;
            float y = transform.position.y;
            MoveDirection moveDirection = MoveDirection.NORTH;
            Room originRoom = gameController.GameBoard.getRoomAt(x, y);

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (originRoom.HasWestExit == false)
                {
                    return;
                }
                x -= 1f;
                keyPressed = true;
                moveDirection = MoveDirection.WEST;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (originRoom.HasEastExit == false)
                {
                    return;
                }
                x += 1f;
                keyPressed = true;
                moveDirection = MoveDirection.EAST;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (originRoom.HasNorthExit == false)
                {
                    return;
                }
                y += 1f;
                keyPressed = true;
                moveDirection = MoveDirection.NORTH;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (originRoom.HasSouthExit == false)
                {
                    return;
                }
                y -= 1f;
                keyPressed = true;
                moveDirection = MoveDirection.SOUTH;
            }
            if (keyPressed)
            {
                Room r = gameController.GameBoard.getRoomAt(x, y);

                Room roomType = gameController.GameBoard.getRandomUnusedRoomType();
                if (r == null && roomType != null)
                {
                    placingRoom = true;
                    gameController.spawnPreviewRoomAt(roomType, x, y, moveDirection);
                }
                else if (r != null)
                {
                    handleMoveToExistingRoom(r, x, y, moveDirection);
                }
                else if (roomType == null)
                {
                    Debug.LogWarning("Out of rooms!");
                }
                keyPressed = false;
            }
        }

        private void handleMoveToExistingRoom(Room targetRoom, float x, float y, MoveDirection direction)
        {
            Point p = new Point(x, y);
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
        }
    }
}
