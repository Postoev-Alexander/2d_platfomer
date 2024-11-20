using System;
using UnityEditor;
using UnityEngine;

namespace GameBuildScripts.Editor
{
	internal class AndroidBuilder : IPlatformBuilder
	{
		public void SetUp(BuildType buildType)
		{
			// Do not set target architecture, use default settings
			//TrySetTargetArchitecture();
			Debug.Log($"Use architecture {PlayerSettings.Android.targetArchitectures}");

			TrySetKeystore(buildType);
			TrySetBuildAppBundle();
			SetMinify(buildType);
		}

		public string GetExtension()
		{
			return EditorUserBuildSettings.buildAppBundle ? "aab" : "apk";
		}

		private void TrySetTargetArchitecture()
		{
			var architecture = GetArchitecture();
			Debug.Log($"Set {architecture}");
			PlayerSettings.Android.targetArchitectures = architecture;

			static AndroidArchitecture GetArchitecture()
			{
				if (CommandLineExtensions.TryGetArg("--AndroidTargetArchitecture", out string architecture))
				{
					return architecture switch
					{
						"arm64" => AndroidArchitecture.ARM64,
						"armv7" => AndroidArchitecture.ARMv7,
						_ => AndroidArchitecture.ARMv7
					};
				}

				return AndroidArchitecture.ARM64;
			}
		}

		private static void TrySetKeystore(BuildType buildType)
		{
			if (buildType == BuildType.Debug)
				PlayerSettings.Android.useCustomKeystore = false;
			else if (buildType == BuildType.Release)
			{
				if (!CommandLineExtensions.TryGetArg("--AndroidKeystorePass", out string keystorePass))
					throw new Exception("Can't find keystore password for Release build");
				if (!CommandLineExtensions.TryGetArg("--AndroidKeyaliasPass", out string keyaliasPass))
					throw new Exception("Can't find keyalias password for Release build");

				PlayerSettings.Android.useCustomKeystore = true;
				PlayerSettings.Android.keystorePass = keystorePass;
				PlayerSettings.Android.keyaliasPass = keyaliasPass;
			}
		}

		private static void TrySetBuildAppBundle()
		{
			if (CommandLineExtensions.TryGetArg("--AndroidBuildAppBundle", out bool appBundle))
			{
				EditorUserBuildSettings.buildAppBundle = appBundle;
				Debug.Log($"Set buildAppBundle: {appBundle}");
			}
			else
			{
				Debug.Log($"Use buildAppBundle: {EditorUserBuildSettings.buildAppBundle}");
			}
		}

		private static void SetMinify(BuildType buildType)
		{
			var isRelease = buildType == BuildType.Release;
			PlayerSettings.Android.minifyRelease = isRelease;
			PlayerSettings.Android.minifyDebug = !isRelease;
		}
	}
}
