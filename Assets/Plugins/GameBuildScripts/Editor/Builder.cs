using System;
using System.IO;
using System.Linq;
//using MobileConsole.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameBuildScripts.Editor
{
	public static class Builder
	{
		public static void Build(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
		{
			var buildAgent = GetBuildAgent();

			var buildType = GetBuildType();

			TrySetupScriptingBackend(buildTargetGroup);
			TrySetBuildNumber();
			//TrySetCheats(buildType);

			var platformBuilder = GetPlatformBuilder(buildTarget);
			platformBuilder.SetUp(buildType);

			var scenes = GetScenes();
			var locationPathName = GetBuildPath(buildTarget, platformBuilder);

			var buildOptions = GetBuildOptions(buildType);

			var report = BuildPipeline.BuildPlayer(scenes, locationPathName, buildTarget, buildOptions);
			var summary = report.summary;

			using (buildAgent.CreateLogGroup("Build result"))
			{
				PrintMessages(report, LogType.Assert, AnsiStyle.MagentaFG);
				PrintMessages(report, LogType.Warning, AnsiStyle.YellowFG);
				PrintMessages(report, LogType.Error, AnsiStyle.RedFG);
				PrintMessages(report, LogType.Exception, AnsiStyle.BrightRedFG);
			}

			if (summary.result == BuildResult.Succeeded)
			{
				Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null,
					$"Build succeeded: {summary.totalSize} bytes, path {locationPathName}");
				buildAgent.ReportBuildSucceeded();
			}
			else if (summary.result == BuildResult.Failed)
			{
				Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null,
					$"Build failed with {summary.totalErrors} errors, {summary.totalWarnings} warnings");

				var firstError = report.steps
					.SelectMany(step => step.messages)
					.FirstOrDefault(message => message.type is LogType.Error or LogType.Exception);
				//buildAgent.ReportBuildFailed(firstError.content);
			}

			static void PrintMessages(BuildReport buildReport, LogType logType, string color)
			{
				int messageNumber = 1;
				var format = $"[{logType}][{{0}}] {{1}}".ColorText(color) + "\n{2}";
				foreach (var step in buildReport.steps)
				{
					foreach (var message in step.messages)
					{
						if (message.type == logType)
						{
							Debug.LogFormat(logType, LogOption.NoStacktrace, null, format, messageNumber++, step, message.content);
						}
					}
				}
			}
		}

		private static EditorBuildSettingsScene[] GetScenes()
		{
			Debug.Log("Building with scenes:");
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				Debug.Log($" {(scene.enabled ? "+" : "-")} {scene.path}");
			}

			return EditorBuildSettings.scenes;
		}

		private static string GetBuildPath(BuildTarget buildTarget, IPlatformBuilder platformBuilder)
		{
			var buildPath = Path.Combine("Builds", buildTarget.ToString(), $"Game.{platformBuilder.GetExtension()}");
			Debug.Log($"Build path: {buildPath} ");
			return buildPath;
		}

		private static void TrySetupScriptingBackend(BuildTargetGroup buildTargetGroup)
		{
			if (CommandLineExtensions.TryGetArg("--Backend", out string backend))
			{
				var scriptingImplementation = (backend == "il2cpp")
					? ScriptingImplementation.IL2CPP
					: ScriptingImplementation.Mono2x;
				Debug.Log($"Set scriptingBackend: {scriptingImplementation}");
				PlayerSettings.SetScriptingBackend(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup),
					scriptingImplementation);
			}
		}

		private static BuildOptions GetBuildOptions(BuildType buildType)
		{
			return buildType switch
			{
				BuildType.Debug => BuildOptions.Development,
				BuildType.Release => BuildOptions.None,
				_ => throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null)
			};
		}

		private static BuildType GetBuildType()
		{
			if (CommandLineExtensions.TryGetArg("--BuildType", out string buildType))
			{
				switch (buildType)
				{
					case "debug":
						return BuildType.Debug;
					case "release":
						return BuildType.Release;
					default:
						throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null);
				}
			}

			return BuildType.Debug;
		}

		private static void TrySetBuildNumber()
		{
			if (CommandLineExtensions.TryGetArg("--BuildNumber", out int buildNumber))
			{
				PlayerSettings.Android.bundleVersionCode = buildNumber;
				PlayerSettings.iOS.buildNumber = buildNumber.ToString();
				PlayerSettings.macOS.buildNumber = buildNumber.ToString();

				Debug.Log($"Build number: {buildNumber}");
			}
		}

		/*private static void TrySetCheats(BuildType buildType)
		{
			if (CommandLineExtensions.TryGetArg("--CheatsState", out string cheatsState))
			{
				switch (cheatsState)
				{
					case "default":
						SetCheats(buildType == BuildType.Debug);
						return;
					case "enabled":
						SetCheats(true);
						return;
					case "disabled":
						SetCheats(false);
						return;
					default:
						throw new ArgumentOutOfRangeException(nameof(cheatsState), cheatsState, null);
				}
			}

			static void SetCheats(bool enable)
			{
				Debug.Log($"Mobile console: {enable}");
				if (enable)
					MobileconsoleSetupHelper.EnableMobileConsole();
				else
					MobileconsoleSetupHelper.DisableMobileConsole();
			}
		}*/

		private static IPlatformBuilder GetPlatformBuilder(BuildTarget buildTarget)
		{
			return buildTarget switch
			{
				BuildTarget.Android => new AndroidBuilder(),
				_ => new DefaultBuilder(buildTarget)
			};
		}

		private class DefaultBuilder : IPlatformBuilder
		{
			private readonly BuildTarget _buildTarget;

			public DefaultBuilder(BuildTarget buildTarget)
			{
				_buildTarget = buildTarget;
			}

			public void SetUp(BuildType buildType)
			{
			}

			public string GetExtension()
			{
				return _buildTarget switch
				{
					BuildTarget.StandaloneOSX => "app",
					BuildTarget.StandaloneWindows => "exe",
					BuildTarget.StandaloneWindows64 => "exe",
					_ => throw new ArgumentOutOfRangeException(nameof(_buildTarget), _buildTarget, null)
				};
			}
		}

		private static BuildAgent GetBuildAgent()
		{
			if (CommandLineExtensions.TryGetArg("--BuildAgent", out string buildAgent))
			{
				switch (buildAgent)
				{
					case "teamcity":
						return new BuildAgent(BuildAgent.CreateTeamcitySettings());
				}
			}

			return new BuildAgent(BuildAgent.CreateDefaultSettings());
		}
	}

	public enum BuildType
	{
		Debug,
		Release,
	}
}
