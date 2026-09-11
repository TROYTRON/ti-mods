using System;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityModManagerNet;

namespace TiMods.TemplatePatch
{
    public static class Main
    {
        private static UnityModManager.ModEntry entry;
        private const float ExampleRangeKm = 1337f;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            entry = modEntry;
            var harmony = new Harmony(entry.Info.Id);
            try
            {
                if (GameControl.control != null && GameControl.control.activePlayer != null)
                    throw new InvalidOperationException("Load this example from a fresh game start, before opening a campaign.");

                harmony.PatchAll(Assembly.GetExecutingAssembly());
                // UMM may load after the first template pass. Modify existing objects;
                // do not clear and recreate the entire template database.
                Apply();
                // No OnToggle: disabling requires restart because consumers can cache values.
                return true;
            }
            catch (Exception exception)
            {
                harmony.UnpatchAll(entry.Info.Id);
                entry.Logger.Error(exception.ToString());
                return false;
            }
        }

        private static void Apply()
        {
            var template = TemplateManager.Find<TILaserWeaponTemplate>("PointDefenseLaserTurret", false);
            if (template == null)
            {
                entry.Logger.Log("PointDefenseLaserTurret not loaded; waiting for the validation hook.");
                return;
            }
            template.targetingRange_km = ExampleRangeKm;
            entry.Logger.Log("PointDefenseLaserTurret.targetingRange_km=" + ExampleRangeKm);
        }

        [HarmonyPatch(typeof(TemplateManager), "ValidateAllTemplates")]
        private static class BeforeTemplateValidation
        {
            private static void Prefix() => Apply();
        }
    }
}
