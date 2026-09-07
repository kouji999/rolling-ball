using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class PlayerBuilder
{
    [MenuItem("Autopilot/Build Windows Player")]
    public static void BuildWindows()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        var report = BuildPipeline.BuildPlayer(new[] { "Assets/Scenes/Main.unity" }, "Build/RollingBall", BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log($"[PlayerBuilder] result={report.summary.result} size={report.summary.totalSize / (1024 * 1024)}MB errors={report.summary.totalErrors}");
        EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
