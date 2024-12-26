using UnityEngine;
using Verse;

namespace SymmetryMatters
{
    public class SymmetryMattersSettings : ModSettings
    {
        public static float RoomImpressivenessEffect = 1f;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);

            RoomImpressivenessEffect = listingStandard.SliderLabeled("SymmetryMatters_RoomImpressivenessEffect".Translate(RoomImpressivenessEffect.ToStringDecimalIfSmall()), RoomImpressivenessEffect, 0f, 10f, 0.5f, "SymmetryMatters_RoomImpressivenessEffectDesc".Translate());

            listingStandard.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref RoomImpressivenessEffect, "RoomImpressivenessEffect", 1f);
        }
    }
}
