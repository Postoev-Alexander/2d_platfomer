namespace GameBuildScripts.Editor
{
	internal interface IPlatformBuilder
	{
		void SetUp(BuildType buildType);
		string GetExtension();
	}
}
