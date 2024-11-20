namespace GameBuildScripts.Editor
{
public static class AnsiStyle
{
    public const string ResetAll = "0";

    // Text AnsiStyle
    public const string Bold = "1";
    public const string Italic = "2";
    public const string Underline = "4";
    public const string StrikeThrough = "9";
    public const string Overline = "53";

    // Reset AnsiStyle
    public const string ResetBold = "22";
    public const string ResetItalic = "23";
    public const string ResetUnderline = "24";
    public const string ResetStrikeThrough = "29";
    public const string ResetOverline = "55";

    // Foreground Colors (Standard)
    public const string BlackFG = "30";
    public const string RedFG = "31";
    public const string GreenFG = "32";
    public const string YellowFG = "33";
    public const string BlueFG = "34";
    public const string MagentaFG = "35";
    public const string CyanFG = "36";
    public const string WhiteFG = "37";

    // Foreground Colors (Bright)
    public const string BrightBlackFG = "90";
    public const string BrightRedFG = "91";
    public const string BrightGreenFG = "92";
    public const string BrightYellowFG = "93";
    public const string BrightBlueFG = "94";
    public const string BrightMagentaFG = "95";
    public const string BrightCyanFG = "96";
    public const string BrightWhiteFG = "97";

    // Background Colors (Standard)
    public const string BlackBG = "40";
    public const string RedBG = "41";
    public const string GreenBG = "42";
    public const string YellowBG = "43";
    public const string BlueBG = "44";
    public const string MagentaBG = "45";
    public const string CyanBG = "46";
    public const string WhiteBG = "47";

    // Background Colors (Bright)
    public const string BrightBlackBG = "100";
    public const string BrightRedBG = "101";
    public const string BrightGreenBG = "102";
    public const string BrightYellowBG = "103";
    public const string BrightBlueBG = "104";
    public const string BrightMagentaBG = "105";
    public const string BrightCyanBG = "106";
    public const string BrightWhiteBG = "107";

    public static string ColorText(this string text, params string[] styles)
    {
	    const string csi = "\u001b[";
	    return $"{csi}{string.Join(';', styles)}m{text}{csi}{AnsiStyle.ResetAll}m";
    }
}
}
