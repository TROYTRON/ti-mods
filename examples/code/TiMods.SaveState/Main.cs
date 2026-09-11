using System;
using System.Collections.Generic;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityModManagerNet;

namespace TiMods.SaveState
{
    // Keep this public type's namespace, class name and assembly identity stable
    // after distributing a save-writing mod. The serializer records type identity.
    public sealed class CampaignCounterState : TIGameState
    {
        [SerializeField]
        public int SchemaVersion = 1;

        [SerializeField]
        public int Counter = 0;

        public override void PostInitializationInit_4()
        {
            base.PostInitializationInit_4();
            Main.Log("Restored campaign counter=" + Counter + "; schema=" + SchemaVersion);
        }
    }

    public static class Main
    {
        private static UnityModManager.ModEntry entry;
        private static bool enabled;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            entry = modEntry;
            entry.OnToggle = (e, value) => { enabled = value; return true; };
            entry.OnGUI = OnGUI;
            return true;
        }

        internal static void Log(string message) => entry?.Logger.Log(message);

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Use a disposable campaign. Creating a counter adds this mod's type to subsequent saves.");
            if (!enabled || GameControl.control == null || GameControl.control.activePlayer == null
                || GameControl.gameStartedUnloading)
            {
                GUILayout.Label("Enable the mod and finish loading a campaign to inspect or edit its counter.");
                return;
            }

            // Query live game state each time; do not retain references across campaign loads.
            var state = GameStateManager.FindGameState<CampaignCounterState>();
            GUILayout.Label(state == null ? "No counter in this campaign." : "Campaign counter: " + state.Counter);
            if (GUILayout.Button("Create or increment campaign counter"))
            {
                try
                {
                    state = state ?? GameStateManager.CreateNewGameState<CampaignCounterState>();
                    if (state == null)
                        throw new InvalidOperationException("The game did not create the counter state.");
                    checked { state.Counter++; }
                    Log("Campaign counter=" + state.Counter + "; save through the normal game menu to persist it.");
                }
                catch (Exception exception)
                {
                    entry.Logger.Error(exception.ToString());
                }
            }
            if (state != null && GUILayout.Button("Remove this campaign's counter before the next save"))
            {
                bool removed = GameStateManager.RemoveGameState<CampaignCounterState>(state.ID, false);
                bool removedBucket = removed && RemoveEmptyOwnedTypeBucket();
                Log("Removed counter=" + removed + "; removed empty owned type bucket=" + removedBucket
                    + "; existing save files are unchanged. Save to a new file before testing uninstall.");
            }
        }

        private static bool RemoveEmptyOwnedTypeBucket()
        {
            // 1.0.53a's public RemoveGameState leaves an empty type-keyed bucket.
            // The serializer writes that type key even with no instances, so a
            // save can still require this assembly. There is no public removal API.
            var field = AccessTools.Field(typeof(GameStateManager), "gamestates");
            var states = field?.GetValue(null) as Dictionary<Type, Dictionary<GameStateID, TIGameState>>;
            if (states == null)
            {
                entry.Logger.Error("Owned type cleanup unavailable: recheck GameStateManager.gamestates after this game update. Keep the mod installed.");
                return false;
            }
            if (!states.TryGetValue(typeof(CampaignCounterState), out var ownedBucket))
                return true;
            if (ownedBucket.Count != 0)
            {
                entry.Logger.Error("Owned type bucket still has instances; leaving it intact. Keep the mod installed.");
                return false;
            }
            // Remove only our own type, only once the game has removed its last instance.
            return states.Remove(typeof(CampaignCounterState));
        }
    }
}
