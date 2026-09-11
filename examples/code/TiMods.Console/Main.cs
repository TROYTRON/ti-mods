using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta.Debugging;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using UnityModManagerNet;

namespace TiMods.Console
{
    public static class Main
    {
        private const string Command = "timods_echo";
        // 1.0.53a exposes registration but no public unregister operation.
        // Check this private field again after game updates.
        private static readonly FieldInfo CommandsField = AccessTools.Field(typeof(TerminalController), "commands");
        private static readonly Dictionary<TerminalController, CommandRegistration> Registrations =
            new Dictionary<TerminalController, CommandRegistration>();
        private static Harmony harmony;
        private static UnityModManager.ModEntry entry;
        private static bool enabled;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            if (CommandsField == null)
            {
                modEntry.Logger.Error("TerminalController.commands was not found; check this example against your game build.");
                return false;
            }

            entry = modEntry;
            harmony = new Harmony(entry.Info.Id);
            entry.OnToggle = OnToggle;
            entry.OnUnload = e => OnToggle(e, false);
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (enabled == value)
                return true;
            try
            {
                if (value)
                {
                    enabled = true;
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                    // Register for an existing terminal too; its initialization may predate UMM.
                    var terminal = GlobalInstaller.container?.TryResolve<Terminal>();
                    if (terminal?.controller != null)
                        Register(terminal.controller);
                }
                else
                {
                    Disable();
                }
                return true;
            }
            catch (Exception exception)
            {
                Disable();
                entry.Logger.Error(exception.ToString());
                return false;
            }
        }

        private static void Disable()
        {
            enabled = false;
            harmony.UnpatchAll(entry.Info.Id);
            foreach (var pair in Registrations)
            {
                var commands = (Dictionary<string, CommandRegistration>)CommandsField.GetValue(pair.Key);
                // Preserve a replacement installed by another mod after us.
                if (commands != null && commands.TryGetValue(Command, out var current) && ReferenceEquals(current, pair.Value))
                    commands.Remove(Command);
            }
            Registrations.Clear();
        }

        internal static void Register(TerminalController controller)
        {
            if (!enabled || Registrations.ContainsKey(controller))
                return;

            var commands = (Dictionary<string, CommandRegistration>)CommandsField.GetValue(controller);
            if (commands == null)
                return; // A destroyed terminal clears its dictionary.
            if (commands.ContainsKey(Command))
            {
                entry.Logger.Error(Command + " already exists; leaving its owner unchanged.");
                return;
            }

            controller.RegisterCommand(Command, args =>
            {
                if (!enabled)
                    return;
                // The game's parser represents a bare command as one empty string.
                string text = args == null ? null : string.Join(" ", args);
                if (string.IsNullOrWhiteSpace(text))
                    controller.OutputError("Usage: timods_echo <text>");
                else
                    controller.Output("TI Mods echo: " + text);
            }, "timods_echo <text>: print arguments without changing game state");
            Registrations.Add(controller, commands[Command]);
            entry.Logger.Log("Registered " + Command);
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.Initialize))]
        private static class TerminalInitializedPatch
        {
            private static void Postfix(Terminal __instance)
            {
                if (__instance.controller != null)
                    Register(__instance.controller);
            }
        }
    }
}
