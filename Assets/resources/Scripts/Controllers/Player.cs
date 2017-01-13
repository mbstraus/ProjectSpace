#region License
// ==============================================================================
// Project Space Copyright (C) 2016 Mathew Strauss
// ==============================================================================
#endregion

using UnityEngine;
using ProjectSpace.Models;

namespace ProjectSpace.Controllers {
    /// <summary>
    /// Represents the Player game object.
    /// </summary>
    public class Player : MonoBehaviour {
        /// <summary>
        /// Pointer to the game controller game object that holds the game board state.
        /// </summary>
        private GameController gameController;
        /// <summary>
        /// If true, the move key was pressed.
        /// </summary>
        private bool keyPressed = false;

        /// <summary>
        /// Use for initialization
        /// </summary>
        void Start() {
            gameController = FindObjectOfType<GameController>();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update() {
        }

        public void playerUpdate() {
            if (gameController.GameBoard.IsPlacingRoom == false) {
                MoveDirection moveDirection = MoveDirection.NONE;
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    moveDirection = MoveDirection.WEST;
                    keyPressed = true;
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    moveDirection = MoveDirection.EAST;
                    keyPressed = true;
                } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    moveDirection = MoveDirection.NORTH;
                    keyPressed = true;
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    moveDirection = MoveDirection.SOUTH;
                    keyPressed = true;
                }
                if (keyPressed) {
                    gameController.GameBoard.beginMoveIntoRoom(transform.position.x, transform.position.y, moveDirection);
                    keyPressed = false;
                }
            }
        }

        /// <summary>
        /// Callback for when the player moves into an existing room. Moves the player model to the room.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void handleMoveToExistingRoom(float x, float y) {
            transform.position = new Vector3(x, y, 0);
        }

        /// <summary>
        /// Callback for when the player moves into a new room. Only does the move if the room is being spawned
        /// from a preview.
        /// </summary>
        /// <param name="roomName">Name of the room (unused, from callback call)</param>
        /// <param name="p">Point of the new room</param>
        /// <param name="isFromPreview">If true, the room is spawned from a preview</param>
        public void handleMoveToNewRoom(string roomName, Point p, bool isFromPreview) {
            // TODO: Check player index
            if (isFromPreview) {
                transform.position = new Vector3(p.X, p.Y, 0);
            }
        }
    }
}
