using UnityEditor;
using UnityEngine;

public class BuildPlatformSettings : Editor
{
    [MenuItem("LightBuzz/Vitruvius/Apply Build Settings for.../Windows Desktop")]
    private static void BuildSettings_Windows()
    {
        SetBuildSettings(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, ScriptingImplementation.Mono2x);
    }

    [MenuItem("LightBuzz/Vitruvius/Apply Build Settings for.../Android")]
    private static void BuildSettings_Android()
    {
        SetBuildSettings(BuildTargetGroup.Android, BuildTarget.Android, ScriptingImplementation.Mono2x);
    }

    [MenuItem("LightBuzz/Vitruvius/Apply Build Settings for.../iOS")]
    private static void BuildSettings_iOS()
    {
        SetBuildSettings(BuildTargetGroup.iOS, BuildTarget.iOS, ScriptingImplementation.IL2CPP);
    }

    private static void SetBuildSettings(BuildTargetGroup group, BuildTarget target, ScriptingImplementation scripting)
    {
        PlayerSettings.SetScriptingBackend(group, scripting);
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;

        EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
    }
    
    [MenuItem("LightBuzz/Vitruvius/Report a problem")]
    private static void GitHub_Issues()
    {
        Application.OpenURL("https://vitruviuskinect.com/support");
    }

    [MenuItem("LightBuzz/Vitruvius/Contact us")]
    private static void Contact()
    {
        Application.OpenURL("https://lightbuzz.com/contact");
    }
}