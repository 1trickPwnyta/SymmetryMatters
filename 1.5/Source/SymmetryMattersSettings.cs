﻿using UnityEngine;
using Verse;

namespace SymmetryMatters
{
    public class SymmetryMattersSettings : ModSettings
    {
        public static float RoomImpressivenessEffect = 1f;
        public static float AsymmetryPunishFactor = 4.2f;
        public static int LargeRoomCacheExpiryTicks = 1250;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);

            RoomImpressivenessEffect = listingStandard.SliderLabeled("SymmetryMatters_RoomImpressivenessEffect".Translate(RoomImpressivenessEffect.ToStringDecimalIfSmall()), RoomImpressivenessEffect, 0f, 10f, 0.5f, "SymmetryMatters_RoomImpressivenessEffectDesc".Translate());
            AsymmetryPunishFactor = listingStandard.SliderLabeled("SymmetryMatters_AsymmetryPunishFactor".Translate(AsymmetryPunishFactor.ToStringDecimalIfSmall()), AsymmetryPunishFactor, 1f, 10f, 0.5f, "SymmetryMatters_AsymmetryPunishFactorDesc".Translate());
            LargeRoomCacheExpiryTicks = (int)listingStandard.SliderLabeled("SymmetryMatters_LargeRoomCacheExpiryTicks".Translate(LargeRoomCacheExpiryTicks.ToString()), LargeRoomCacheExpiryTicks, 0, 2500, 0.5f, "SymmetryMatters_LargeRoomCacheExpiryTicksDesc".Translate());

            listingStandard.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref RoomImpressivenessEffect, "RoomImpressivenessEffect", 1f);
            Scribe_Values.Look(ref AsymmetryPunishFactor, "AsymmetryPunishFactor", 4.2f);
            Scribe_Values.Look(ref LargeRoomCacheExpiryTicks, "LargeRoomCacheExpiryTicks", 1250);
        }
    }
}
