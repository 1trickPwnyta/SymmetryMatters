using HarmonyLib;
using Verse;

namespace SymmetryMatters
{
    public class SymmetryMattersMod : Mod
    {
        public const string PACKAGE_ID = "symmetrymatters.1trickPwnyta";
        public const string PACKAGE_NAME = "Symmetry Matters";

        public SymmetryMattersMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(PACKAGE_ID);
            harmony.PatchAll();

            Log.Message($"[{PACKAGE_NAME}] Loaded.");
        }
    }
}
