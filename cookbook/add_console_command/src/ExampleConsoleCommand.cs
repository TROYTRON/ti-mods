using System.Reflection;

using HarmonyLib;
using UnityModManagerNet;

using PavonisInteractive.TerraInvicta.Debugging;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;

namespace ExampleConsoleCommand
{
    public class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;
        public static TerminalCustomCommandBinding terminalBindingHolder;

        static bool Load(UnityModManager.ModEntry modEntry) {
            // Boiler plate initialization
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            mod = modEntry;
            modEntry.OnToggle = OnToggle;

            var container = GlobalInstaller.container;
            var terminalController = container.Resolve<Terminal>().controller;
            terminalBindingHolder = new TerminalCustomCommandBinding(terminalController);

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            enabled = value;
            return true;
        }
    }

    public class TerminalCustomCommandBinding {
        public TerminalCustomCommandBinding(TerminalController terminalController) {
            this.terminalController = terminalController;
            this.terminalController.RegisterCommand(
                "MySimpleCommand",
                new CommandHandler(this.MySimpleCommand),
                "MySimpleCommand is an exmple of the a custom console command," +
                " prints back one argument it is give");
        }

        public void MySimpleCommand(string[] args) {
            if (args.Length < 1) {
                this.terminalController.OutputError("MySimpleCommand requires one argument");
                return;
            }
            this.terminalController.Output("You called MySimpleCommand with: " + args[0]);
        }

        private TerminalController terminalController;
    }
}
