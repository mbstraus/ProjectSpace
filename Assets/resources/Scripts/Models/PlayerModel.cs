using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSpace.Models {
    public class PlayerModel {

        public int PlayerNumber { get; protected set; }
        public bool IsActive { get; set; }

        public PlayerModel(int playerNumber, bool isActive) {
            PlayerNumber = playerNumber;
            IsActive = isActive;
        }
    }
}
