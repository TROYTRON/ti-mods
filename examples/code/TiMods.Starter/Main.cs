using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityModManagerNet;

namespace TiMods.Starter
{
    public static class Main
    {
        internal static bool Enabled;
        internal static Settings Settings;
        private static Harmony harmony;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            harmony = new Harmony(modEntry.Info.Id);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = entry => Settings.Save(entry);
            modEntry.OnUnload = OnUnload;
            modEntry.Logger.Log("Loaded. Toggle on to apply patches; no campaign is required for the probe.");
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry entry, bool value)
        {
            try
            {
                if (value == Enabled)
                    return true;

                if (value)
                {
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                    Enabled = true;
                }
                else
                {
                    Enabled = false;
                    // Never call the parameterless form: it removes other mods' patches too.
                    harmony.UnpatchAll(entry.Info.Id);
                }

                entry.Logger.Log("Enabled=" + Enabled + "; Probe: " + Probe.Message());
                return true;
            }
            catch (Exception exception)
            {
                Enabled = false;
                harmony.UnpatchAll(entry.Info.Id);
                entry.Logger.Error(exception.ToString());
                return false;
            }
        }

        private static bool OnUnload(UnityModManager.ModEntry entry)
        {
            return OnToggle(entry, false);
        }

        private static void OnGUI(UnityModManager.ModEntry entry)
        {
            GUILayout.Label("Harmony probe: " + Probe.Message());
            Settings.PatchProbe = GUILayout.Toggle(Settings.PatchProbe, "Enable probe postfix");
            Settings.PreciseProjects = GUILayout.Toggle(Settings.PreciseProjects,
                "Display numeric councilor project contribution (requires a campaign)");
            if (GUILayout.Button("Write probe to log"))
                entry.Logger.Log("Probe: " + Probe.Message());
        }
    }

    public sealed class Settings : UnityModManager.ModSettings
    {
        public bool PatchProbe = true;
        public bool PreciseProjects = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }

    // A deliberately local target makes the first smoke test independent of campaign state.
    internal static class Probe
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string Message() => "original";
    }

    [HarmonyPatch(typeof(Probe), nameof(Probe.Message))]
    internal static class ProbePatch
    {
        private static void Postfix(ref string __result)
        {
            if (Main.Enabled && Main.Settings.PatchProbe)
                __result = "patched";
        }
    }

    // Verified target signature in stable 1.0.53a. This changes display text only.
    [HarmonyPatch(typeof(TICouncilorState), nameof(TICouncilorState.projectContributionString), MethodType.Getter)]
    internal static class PreciseProjectContributionPatch
    {
        private static void Postfix(TICouncilorState __instance, ref string __result)
        {
            if (Main.Enabled && Main.Settings.PreciseProjects && __result != "-")
                __result = __instance.GetMonthlyIncome(FactionResource.Projects).ToString("N0");
        }
    }
}
