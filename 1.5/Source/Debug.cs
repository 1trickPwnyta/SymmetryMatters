namespace SymmetryMatters
{
    public static class Debug
    {
        public static void Log(string message)
        {
#if DEBUG
            Verse.Log.Message($"[{SymmetryMattersMod.PACKAGE_NAME}] {message}");
#endif
        }
    }
}
