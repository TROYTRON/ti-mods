using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

namespace SaveStateExample {
    class SpaceShipVeterancyManager {

        public static SpaceShipVeterancyManager singleton = new SpaceShipVeterancyManager();

        private Dictionary<GameStateID, TISpaceShipVeterancyState> 
            SpaceShipVeterancyStateMapping =
            new Dictionary<GameStateID, TISpaceShipVeterancyState>();

        public TISpaceShipVeterancyState this[TISpaceShipState ship] {
            get { 
                if (ship == null) {
                    return null;
                }

                if (!SpaceShipVeterancyStateMapping.ContainsKey(ship.ID)) {
                    this.RegisterShip(ship);
                }

                return SpaceShipVeterancyStateMapping[ship.ID];
            }
        }

        public void RegisterShip(TISpaceShipState ship, TISpaceShipVeterancyState veterancy = null) {
            if (SpaceShipVeterancyStateMapping.ContainsKey(ship.ID)) {
                return;
            }

            if (veterancy == null) {
                veterancy = GameStateManager.CreateNewGameState<TISpaceShipVeterancyState>();
                veterancy.InitWithSpaceShipState(ship);
            }

            SpaceShipVeterancyStateMapping.Add(ship.ID, veterancy);
        }

        public void UnregisterShip(TISpaceShipState ship) {
            if (!SpaceShipVeterancyStateMapping.ContainsKey(ship.ID)) {
                TISpaceShipVeterancyState shipVeterancyState = this[ship];
                if (GameStateManager.RemoveGameState<TISpaceShipVeterancyState>(shipVeterancyState.ID, false)) {
                    SpaceShipVeterancyStateMapping.Remove(ship.ID);
                }
            }
        }

        public void ResetState() {
            SpaceShipVeterancyStateMapping.Clear();
        }
    }
}
