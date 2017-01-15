using ProjectSpace.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectSpace.Controllers {
    public class TurnUIController : MonoBehaviour {
        
        private bool isOpen;
        private double elapsedOpenTime = 0f;
        public GameObject turnUI;
        public double timeOnScreen = 1f;
        
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (isOpen) {
                elapsedOpenTime += Time.deltaTime;
                if (elapsedOpenTime >= timeOnScreen) {
                    isOpen = false;
                    turnUI.SetActive(false);
                }
            }
        }

        public void showTurnUI(PlayerModel currentPlayer, PlayerModel nextPlayer) {
            Text text = turnUI.GetComponentInChildren<Text>();

            text.text = "Player " + nextPlayer.PlayerNumber + "'s Turn!";
            turnUI.SetActive(true);

            isOpen = true;
            elapsedOpenTime = 0f;
        }
    }
}
