using ProjectSpace.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSpace.Controllers {
    public class CameraController : MonoBehaviour {

        private Camera mainCamera;

        private Vector3 origin;
        private Vector3 destination;
        private GameController gameController;
        private bool isTracking;
        private float elapsedTime = 0f;
        protected float timeToCenter = 1f;

        // Use this for initialization
        void Start() {
            gameController = gameObject.GetComponent<GameController>();
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update() {
            if (isTracking) {
                elapsedTime += Time.deltaTime;
                mainCamera.transform.position = Vector3.Lerp(origin, destination, (elapsedTime / timeToCenter));

                if (elapsedTime >= timeToCenter) {
                    isTracking = false;
                }
            }
        }

        public void trackToNextPlayer(PlayerModel currentPlayer, PlayerModel nextPlayer) {
            GameObject playerGameObject = null;
            gameController.PlayerGameObjects.TryGetValue(nextPlayer.PlayerNumber, out playerGameObject);
            origin = mainCamera.transform.position;

            destination = new Vector3(playerGameObject.transform.position.x, playerGameObject.transform.position.y, mainCamera.transform.position.z);

            isTracking = true;
            elapsedTime = 0f;
        }
    }
}
