using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;


namespace CodeTemplatePatchExample
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

    // Intercept `TemplateManager.RegisterFileBasedTemplate`.
    [HarmonyPatch(typeof(TemplateManager), "RegisterFileBasedTemplate")]
    static class UseAlternateTemplateLoadManager {
        // Intercept the method post execution.
        static void Postfix(string templateFile) {
            // Get the textual type name of last loaded templates.
            string templateTypeName = Path.GetFileNameWithoutExtension(templateFile);

            // Compare the last loaded template type name with the type name of
            // the template you wish to modify.
            if (templateTypeName != "TILaserWeaponTemplate") {
                return;
            }

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
