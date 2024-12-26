using HarmonyLib;
using UnityEngine;
using Verse;

namespace SymmetryMatters
{
    public class SymmetryMattersMod : Mod
    {
        public const string PACKAGE_ID = "symmetrymatters.1trickPwnyta";
        public const string PACKAGE_NAME = "Symmetry Matters";

        public static SymmetryMattersSettings Settings;

        public SymmetryMattersMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(PACKAGE_ID);
            harmony.PatchAll();

            Settings = GetSettings<SymmetryMattersSettings>();

            Log.Message($"[{PACKAGE_NAME}] Loaded.");
        }

        public override string SettingsCategory() => PACKAGE_NAME;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            SymmetryMattersSettings.DoSettingsWindowContents(inRect);
        }
    }
}
