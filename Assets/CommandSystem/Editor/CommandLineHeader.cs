namespace CommandSystem.Editor
{
    public static class CommandLineHeader
    {
        public static string HeaderText = "Unity Terminal Interface";
        public static string VersionText = "v0.0.1";
        public static string AuthorText = "by ETdoFresh";

        public static string GetHeader()
        {
            return $"{HeaderText}\n{VersionText} {AuthorText}\n\n";
        }
    }
}