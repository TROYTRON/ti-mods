using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using PavonisInteractive.TerraInvicta.Modding;

namespace TiMods.MergeChecks
{
    internal static class Program
    {
        private static string managedDirectory;

        private static int Main(string[] args)
        {
            if (args.Length != 1 || !Directory.Exists(args[0]))
            {
                Console.Error.WriteLine("Usage: TiMods.MergeChecks.exe <TerraInvicta_Data/Managed directory>");
                return 2;
            }
            managedDirectory = Path.GetFullPath(args[0]);
            AppDomain.CurrentDomain.AssemblyResolve += ResolveLocalGameAssembly;
            try
            {
                return RunChecks();
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                return 1;
            }
        }

        private static Assembly ResolveLocalGameAssembly(object sender, ResolveEventArgs args)
        {
            string file = Path.Combine(managedDirectory, new AssemblyName(args.Name).Name + ".dll");
            return File.Exists(file) ? Assembly.LoadFrom(file) : null;
        }

        // Keep game-dependent JIT compilation after registration of the local resolver.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int RunChecks()
        {
            var controller = new JsonController();
            Console.WriteLine("Actual helper: " + typeof(JsonController).Assembly.Location);
            Console.WriteLine("Standalone managed helper checks; the Unity loader/game pipeline is not running.");

            // All fixture objects contain dataName. Malformed-input paths in this
            // game helper call Unity logging and are deliberately outside this harness.
            var original = Objects("{'dataName':'Example','values':[1,2,3],'keep':7}");
            var patch = Objects("{'dataName':'Example','values':[9]}");
            var merged = controller.CombineJson(original, patch, false);
            Assert("default array index merge", JToken.DeepEquals(merged[0]["values"], JArray.Parse("[9,2,3]")));
            Assert("omitted property preserved", (int)merged[0]["keep"] == 7);

            merged = controller.CombineJson(Objects("{'dataName':'Example','values':[1,2,3]}"),
                Objects("{'dataName':'Example','values':[9]}"), false, MergeArrayHandling.Concat);
            Assert("concat appends array entries", JToken.DeepEquals(merged[0]["values"], JArray.Parse("[1,2,3,9]")));

            merged = controller.CombineJson(Objects("{'dataName':'Example','values':[1,2,3],'keep':7}"),
                Objects("{'dataName':'Example','values':[9]}"), false, MergeArrayHandling.Replace);
            Assert("replace replaces array only", JToken.DeepEquals(merged[0]["values"], JArray.Parse("[9]"))
                && (int)merged[0]["keep"] == 7);

            merged = controller.CombineJson(Objects("{'dataName':'Example','value':'keep'}"),
                Objects("{'dataName':'Example','value':null}"), false);
            Assert("null does not erase existing property", (string)merged[0]["value"] == "keep");

            merged = controller.CombineJson(Objects("{'dataName':'Example','value':1}"),
                Objects("{'dataName':'example','value':2}"), false);
            Assert("dataName matching is case sensitive", merged.Count == 2
                && (int)merged.Single(o => (string)o["dataName"] == "Example")["value"] == 1);

            merged = controller.CombineJson(Objects("{'dataName':'First','value':1}"),
                Objects("{'dataName':'Second','value':2}"), false);
            Assert("new dataName appended for non-DLC input", merged.Count == 2
                && (string)merged[1]["dataName"] == "Second");

            merged = controller.CombineJson(Objects("{'dataName':'First','value':1}"),
                Objects("{'dataName':'Second','value':2}"), true);
            Assert("DLC flag suppresses new dataName", merged.Count == 1);

            merged = controller.CombineJson(Objects("{'dataName':'Example','value':1}"),
                Objects("{'dataName':'Example','Value':2}"), false);
            Assert("property-name comparison is ordinal", (int)merged[0]["value"] == 1
                && (int)merged[0]["Value"] == 2);

            Console.WriteLine("PASS: 9 assertions against the installed JsonController.CombineJson implementation.");
            return 0;
        }

        private static List<JObject> Objects(string json) => new List<JObject> { JObject.Parse(json) };

        private static void Assert(string name, bool condition)
        {
            if (!condition)
                throw new InvalidOperationException("FAIL: " + name);
            Console.WriteLine("PASS: " + name);
        }
    }
}
