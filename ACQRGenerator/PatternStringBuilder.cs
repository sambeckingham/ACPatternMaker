namespace ACPatternMaker
{
    public class PatternStringBuilder
    {
        private const string FrontMatter =
            "410043005000610074007400650072006E004D0061006B00650072000000000000000000000000000000B6EC530061006D00000000000000000000000000000044C547006900740068007500620000000000000000001931";

        public static string Build(string paletteString, string pixelString)
        {
            var completeString = FrontMatter + paletteString + "CC0A090000" + pixelString;
            return completeString;
        }
    }
}