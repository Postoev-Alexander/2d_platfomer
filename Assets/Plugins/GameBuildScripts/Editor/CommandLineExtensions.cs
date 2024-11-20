namespace GameBuildScripts.Editor
{
	public static class CommandLineExtensions
	{
		public static string GetArg(string key)
			=> TryGetArg(key, out string result) ? result : null;

		public static bool HasArg(string key)
			=> TryGetArg(key, out string _);

		public static bool TryGetArg(string key, out string value)
		{
			value = null;
			var args = System.Environment.GetCommandLineArgs();
			for (var i = 0; i < args.Length - 1; i++)
			{
				if (args[i] == key)
				{
					value = args[i + 1];
					return true;
				}
			}

			return false;
		}

		public static bool TryGetArg(string key, out bool value)
		{
			value = default;
			return TryGetArg(key, out string strValue) && bool.TryParse(strValue, out value);
		}

		public static bool TryGetArg(string key, out int value)
		{
			value = default;
			return TryGetArg(key, out string strValue) && int.TryParse(strValue, out value);
		}
	}
}
