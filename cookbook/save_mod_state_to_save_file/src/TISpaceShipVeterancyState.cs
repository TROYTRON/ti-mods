using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Entities;
using UnityEngine;


namespace SaveStateExample {
    class TISpaceShipVeterancyState : TIGameState {
        new public TISpaceShipState ref_ship;

        [SerializeField]
        public int battlesSurvived { get; private set; } = 0;

        public void RecordBattle() {
            battlesSurvived += 1;
        }

        public void InitWithSpaceShipState(TISpaceShipState ship) {
            if (ship.template == null) {
                return;
            }

            this.ref_ship = ship;
        }

        public override void PostInitializationInit_4() {
            SpaceShipVeterancyManager.singleton.RegisterShip(this.ref_ship, this);
        }
    }
}
