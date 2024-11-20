using System;
using UnityEditor;
using UnityEngine;

namespace GameBuildScripts.Editor
{
	public class BuildAgent
	{
		private readonly Settings _settings;

		public BuildAgent(Settings settings)
		{
			_settings = settings;
		}

		public LogGroup CreateLogGroup(string name, string description = null)
			=> new LogGroup(this, name, description);

		public void ReportBuildSucceeded()
		{
			Log(_settings.buildSucceeded);
		}

		public void ReportBuildFailed(string message)
		{
			Log(_settings.buildFailed, message);
		}

		private void Log(string format, params object[] args)
		{
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, format, args);
		}

		private void OpenLogGroup(string name, string description = null)
		{
			if (description != null)
				Log(_settings.logGroupSettings.openNameDescriptionFormat, name, description);
			else
				Log(_settings.logGroupSettings.openNameFormat, name);
		}

		private void CloseLogGroup(string name)
		{
			Log(string.Format(_settings.logGroupSettings.closeFormat, name));
		}

		public readonly struct LogGroup : IDisposable
		{
			private readonly BuildAgent _buildAgent;
			private readonly string _name;

			public LogGroup(BuildAgent buildAgent, string name, string description = null)
			{
				_buildAgent = buildAgent;
				_name = name;
				_buildAgent.OpenLogGroup(name, description);
			}

			public void Dispose()
			{
				_buildAgent.CloseLogGroup(_name);
			}
		}

		public struct Settings
		{
			public LogGroupSettings logGroupSettings;
			public string buildSucceeded;
			public string buildFailed;

			public struct LogGroupSettings
			{
				public string openNameFormat;
				public string openNameDescriptionFormat;
				public string closeFormat;
			}
		}

		public static Settings CreateTeamcitySettings()
		{
			return new Settings()
			{
				logGroupSettings = new Settings.LogGroupSettings()
				{
					openNameFormat = "##teamcity[blockOpened name='{0}']",
					openNameDescriptionFormat = "##teamcity[blockOpened name='{0}' description='{1}']",
					closeFormat = "##teamcity[blockClosed name='{0}']",
				},
				buildSucceeded = "##teamcity[buildStatus status='SUCCESS' text='{{build.status.text}}']",
				buildFailed = "##teamcity[buildProblem description='{0}']",
			};
		}

		public static Settings CreateDefaultSettings()
		{
			return new Settings()
			{
				logGroupSettings = new Settings.LogGroupSettings()
				{
					openNameFormat = "##[blockOpened name='{0}']",
					openNameDescriptionFormat = "##[blockOpened name='{0}' description='{1}']",
					closeFormat = "##[blockClosed name='{0}']",
				},
				buildSucceeded = "##[buildStatus status='SUCCESS']",
				buildFailed = "##[buildProblem description='{0}']",
			};
		}
	}
}
