using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;

namespace BetterCodeTemplatePatchExample
{
    public class TemplatePatchExample {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry) {
            // Boilerplate.
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            mod = modEntry;
            modEntry.OnToggle = OnToggle;

            // Reload templates upon start to ensure they are ready for
            // skirmish mode.
            TemplateManager.ClearAllTemplates();
            GlobalInstaller.container.Resolve<TemplateManager>().Initialize(
                Application.streamingAssetsPath + Utilities.templateFolder);

            return true;
        }

        // Boilerplate.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            enabled = value;
            return true;
        }
    }

    // Intercept `TemplateManager.ValidateAllTemplates`.
    [HarmonyPatch(typeof(TemplateManager), "ValidateAllTemplates")]
    static class UseAlternateTemplateLoadManager {
        // Intercept the method prior execution.
        static void Prefix() {
            // Use `dataName` of the template you wish to modify to find it.
            // among others of the same type.
            TILaserWeaponTemplate template =
                TemplateManager.Find<TILaserWeaponTemplate>("PointDefenseLaserTurret");
            if (template == null) {
                return;
            }

            // Modify properties of the template
            template.targetingRange_km = 1337;
        }
    }
}
