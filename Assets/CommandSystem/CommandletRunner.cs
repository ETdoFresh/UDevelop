using System.Linq;

namespace CommandSystem
{
    public static class CommandletRunner
    {
       

        private static void AttemptUpdateAliasMap()
        {
            CommandRunner.Update();
        }
    }
}