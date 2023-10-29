namespace CommandSystem.Editor
{
    public static class CommandLineHeader
    {
        private const string HeaderText = "Unity Command Line Interface";
        private const string ShortHeaderText = "UNITY/CLI";
        private const string VersionText = "0.0.1";
        private const string AuthorText = "ETdoFresh";
        private const string CompanyText = "ETdoFresh, not Inc.";
        private const string YearsText = "2023-2023";

        public static string GetHeader()
        {
            //UNITY/CLI Professional Editor Mode  Version 0.0.1
            // Copyright (c) ETdoFresh, not Inc. 2023-2023
#if UNITY_EDITOR
            const string mode = "Editor";
#else
            const string mode = "Run-time";
#endif
            return $"{ShortHeaderText} Professional {mode} Mode  Version {VersionText}\n" +
                   $"Copyright (c) {CompanyText} {YearsText}\n\n";
        }
    }
}