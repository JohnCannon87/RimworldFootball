using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;

namespace Rimball
{
    class RimballSettings : ModSettings
    {
        internal static bool drawPlayerSpots = false;
        public string traitGainChance = "0.01";

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref drawPlayerSpots, "drawPlayerSpots", false);
            Scribe_Values.Look(ref traitGainChance, "traitGainChance", "0.01");
        }

        public void DoWindowContents(Rect rect)
        {
            Listing_Standard list = new Listing_Standard()
            {
                ColumnWidth = rect.width
            };

            list.Begin(rect);

            list.Label("Trait Gain Chance (0.0 to 1.0):");
            traitGainChance = list.TextEntry(traitGainChance);

            // Parse and validate the input as a float
            if (!float.TryParse(traitGainChance, out float parsedChance) || parsedChance < 0f || parsedChance > 1f)
            {
                traitGainChance = "0.01";  // Reset to a default value if parsing fails
                Log.Error("Invalid input for trait gain chance. Value must be between 0.0 and 1.0.");
            }

            list.Label("Rimball Debug Settings");
            list.CheckboxLabeled("Draw Player Spots", ref drawPlayerSpots);

            list.End();
        }
    }
}
