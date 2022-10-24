using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;

namespace SaveStateExample {

    [HarmonyPatch(typeof(FleetsScreenController), "UpdateIndividualDataScreen")]
    static class PatchUpdateIndividualDataScreen {
        static void Postfix(ref FleetsScreenController __instance) {
            if (SpaceShipVeterancyManager.singleton[__instance.selectedShip].battlesSurvived > 0) {
                __instance.indiv_ShipName.SetText("*" + __instance.selectedShip.displayName, true);
            }
        }
    }


    [HarmonyPatch(typeof(FleetsScreenController), "OnClickSaveName")]
    static class PatchOnClickSaveName {
        static void Postfix(ref FleetsScreenController __instance) {
            if (SpaceShipVeterancyManager.singleton[__instance.selectedShip].battlesSurvived > 0) {
                __instance.indiv_ShipName.SetText("*" + __instance.selectedShip.displayName, true);
            }
        }
    }

    [HarmonyPatch(typeof(SolarSystemBootstrap), "LoadGame")]
    static class LoadGamePatch {
        static void Prefix() {
            SpaceShipVeterancyManager.singleton.ResetState();
        }
    }

    [HarmonyPatch(typeof(ViewControl), "ClearGameData")]
    static class ClearGameDataPatch {
        static void Prefix() {
            SpaceShipVeterancyManager.singleton.ResetState();
        }
    }

    [HarmonyPatch(typeof(TISpaceShipState), "InitWithTemplate")]
    static class InitWithTemplatePatch {
        // In this case the argument is `rawTemplate`, but in others it might be
        // just `template`.
        static void Postfix(TIDataTemplate rawTemplate, TISpaceShipState __instance) {
            if (rawTemplate as TISpaceShipTemplate != null) {
                SpaceShipVeterancyManager.singleton.RegisterShip(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(TISpaceShipState), "DestroyShip")]
    static class DestroyShipPatch {
        static void Postfix(ref TISpaceShipState __instance) {
            SpaceShipVeterancyManager.singleton.UnregisterShip(__instance);
        }
    }

    [HarmonyPatch(typeof(TISpaceFleetState), "PostCombat")]
    static class PostCombatPatch {
        static void Postfix(ref TISpaceFleetState __instance) {
            foreach (var ship in __instance.ships) {
                if (!ship.ShipDestroyed()) {
                    SpaceShipVeterancyManager.singleton[ship].RecordBattle();
                }
            }
        }
    }
}
