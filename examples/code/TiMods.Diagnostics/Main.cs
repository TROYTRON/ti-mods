using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityModManagerNet;

namespace TiMods.Diagnostics
{
    public sealed class Settings : UnityModManager.ModSettings
    {
        public string LocalizationKey = "TIOrgTemplate.displayName.Handbook_OpenResearchGroup";
        public string OrgDataName = "Handbook_OpenResearchGroup";
        public bool LogAfterTemplateValidation = true;
        public override void Save(UnityModManager.ModEntry entry) => Save(this, entry);
    }

    public static class Main
    {
        private static UnityModManager.ModEntry entry;
        private static Harmony harmony;
        private static Settings settings;
        private static bool enabled;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            entry = modEntry;
            settings = UnityModManager.ModSettings.Load<Settings>(entry);
            harmony = new Harmony(entry.Info.Id);
            entry.OnToggle = OnToggle;
            entry.OnUnload = e => OnToggle(e, false);
            entry.OnSaveGUI = e => settings.Save(e);
            entry.OnGUI = e =>
            {
                GUILayout.Label("Localization key");
                settings.LocalizationKey = GUILayout.TextField(settings.LocalizationKey);
                GUILayout.Label("Org dataName");
                settings.OrgDataName = GUILayout.TextField(settings.OrgDataName);
                settings.LogAfterTemplateValidation = GUILayout.Toggle(settings.LogAfterTemplateValidation,
                    "Log after template validation");
                if (GUILayout.Button("Log probes"))
                    LogProbes("manual");
            };
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value == enabled)
                return true;
            try
            {
                if (value)
                {
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                    enabled = true;
                    LogProbes("enabled");
                }
                else
                {
                    enabled = false;
                    harmony.UnpatchAll(entry.Info.Id);
                }
                return true;
            }
            catch (Exception exception)
            {
                enabled = false;
                harmony.UnpatchAll(entry.Info.Id);
                entry.Logger.Error(exception.ToString());
                return false;
            }
        }

        private static void Probe(string label, Func<string> read)
        {
            try { entry.Logger.Log(label + "=" + read()); }
            catch (Exception exception) { entry.Logger.Log(label + " unavailable: " + exception.Message); }
        }

        private static void LogProbes(string reason)
        {
            if (!enabled)
                return;
            entry.Logger.Log("Probes: " + reason);
            Probe("globalConfig.notificationReceiveInputDelay", () => TemplateManager.global.notificationReceiveInputDelay.ToString());
            Probe("globalConfig.strategyLayerSpeedSettings", () => string.Join(",", TemplateManager.global.strategyLayerSpeedSettings));
            Probe("ModernOrgTemplates.templateNames", () => string.Join(",",
                TemplateManager.Find<TIMetaTemplate>("ModernOrgTemplates", false).templateNames));
            Probe("org." + settings.OrgDataName, () =>
            {
                var org = TemplateManager.Find<TIOrgTemplate>(settings.OrgDataName, false);
                return org == null ? "missing" : "present; displayName=" + org.displayName;
            });
            Probe("localization." + settings.LocalizationKey, () => Loc.T(settings.LocalizationKey));
            Probe("activePlayer", () => GameControl.control?.activePlayer?.displayName ?? "none");
            Probe("factions", () => string.Join(",", GameStateManager.AllFactions().Select(f => f.displayName)));
            Probe("selectedFactionsForScenario", () => string.Join(",", GameStateManager.MetaData().selectedFactionsForScenario));
        }

        [HarmonyPatch(typeof(TemplateManager), "ValidateAllTemplates")]
        private static class AfterTemplateValidation
        {
            private static void Postfix()
            {
                if (settings.LogAfterTemplateValidation)
                    LogProbes("templates validated");
            }
        }
    }
}
