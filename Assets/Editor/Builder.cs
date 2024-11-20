using GameBuildScripts.Editor;
using UnityEditor;

namespace Game.Editor
{
    public static class GameBuilder
    {
        [MenuItem("Tools/Build/Build MacOs")]
        private static void BuildMacOs() => Builder.Build(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone);

        [MenuItem("Tools/Build/Build Windows")]
        private static void BuildWindows() => Builder.Build(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone);

        [MenuItem("Tools/Build/Build Android")]
        private static void BuildAndroid() => Builder.Build(BuildTarget.Android, BuildTargetGroup.Android);

        [MenuItem("Tools/Build/Build Linux")]
        private static void BuildLinux() => Builder.Build(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone);
    }
}
