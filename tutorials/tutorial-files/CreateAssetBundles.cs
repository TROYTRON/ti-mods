// Put this file in Assets/Editor in a Unity 2020.3.49f1 project.
// Baseline: TI Windows stable 1.0.53a, inspected 2026-09-06.
// Statically reviewed; no Unity bundle build was run for this guide.
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public static class CreateAssetBundles
{
    private const string ExpectedUnityVersion = "2020.3.49f1";
    private const BuildTarget Target = BuildTarget.StandaloneWindows64;

    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        if (Application.unityVersion != ExpectedUnityVersion)
        {
            throw new BuildFailedException(
                "This tutorial targets Unity " + ExpectedUnityVersion +
                ". Open the project with that Editor before building.");
        }

        if (EditorUserBuildSettings.activeBuildTarget != Target)
        {
            throw new BuildFailedException(
                "Select Windows x86_64 in File > Build Settings, then Switch Platform.");
        }

        if (AssetDatabase.GetAllAssetBundleNames().Length == 0)
        {
            throw new BuildFailedException(
                "Assign at least one asset to an AssetBundle in the Inspector.");
        }

        // Keep generated bundles outside Assets to prevent their re-import.
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string outputPath = Path.Combine(projectRoot, "AssetBundles", "Windows");
        Directory.CreateDirectory(outputPath);

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
            outputPath, BuildAssetBundleOptions.StrictMode, Target);

        if (manifest == null)
        {
            throw new BuildFailedException("AssetBundle build failed. Inspect the Unity Console.");
        }

        Debug.Log("AssetBundles built in " + outputPath +
            ". Package each content bundle with its matching .manifest file.");
    }
}
