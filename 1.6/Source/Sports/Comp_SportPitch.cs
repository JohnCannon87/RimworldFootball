using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace Rimball
{
    class Comp_SportPitch : ThingComp
    {

        public List<IntVec3> sideACells = new List<IntVec3>();
        public List<IntVec3> sideBCells = new List<IntVec3>();
        public string soundDefName;

        CompProperties_SportPitch Comp_SportPitchProps
        {
            get
            {
                return (CompProperties_SportPitch)this.props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            //jcLog.Message($"[Comp_SportPitch] PostSpawnSetup called for {parent.Label} at {parent.Position}");
            base.PostSpawnSetup(respawningAfterLoad);

            extractActualCells(sideACells, Comp_SportPitchProps.sideACoords, "sideA");
            extractActualCells(sideBCells, Comp_SportPitchProps.sideBCoords, "sideB");
            soundDefName = Comp_SportPitchProps.soundDefName;
        }
        public void TryGiveTraitToPawn(Pawn pawn)
        {
            // Get the chance from the mod settings
            float traitGainChance = float.Parse(RimballMod.settings.traitGainChance);

            foreach (TraitEntry traitEntry in Comp_SportPitchProps.potentialTraits)
            {
                // Check if the pawn should gain the trait based on the configured chance
                if (Rand.Value < traitGainChance)
                {
                    TraitDef traitDef = DefDatabase<TraitDef>.GetNamed(traitEntry.traitDefName, true);

                    // Check if the pawn already has the trait
                    if (!pawn.story.traits.HasTrait(traitDef))
                    {
                        // Add the trait to the pawn
                        pawn.story.traits.GainTrait(new Trait(traitDef, traitEntry.degree));
                        SendTraitGainedLetter(pawn, traitDef, traitEntry.degree);

                        // Optional: Log a message to indicate the pawn gained the trait
                        Log.Message($"{pawn.Name} has gained the trait: {traitDef.label}");
                    }
                    else
                    {
                        // Handle the case where the pawn already has the trait but with a different degree
                        Trait existingTrait = pawn.story.traits.GetTrait(traitDef);
                        if (existingTrait.Degree != traitEntry.degree)
                        {
                            // Update the trait to the new degree
                            pawn.story.traits.RemoveTrait(existingTrait);
                            pawn.story.traits.GainTrait(new Trait(traitDef, traitEntry.degree));
                            SendTraitGainedLetter(pawn, traitDef, traitEntry.degree);

                            Log.Message($"{pawn.Name} had {traitDef.label} with degree: {existingTrait.Degree}. Updated to degree: {traitEntry.degree}");
                        }
                    }
                }
            }
        }
        private void SendTraitGainedLetter(Pawn pawn, TraitDef traitDef, int degree)
        {
            string letterText;
            if (degree != 0)
            {
                letterText = $"{pawn.Name} has gained the trait {traitDef.degreeDatas[degree].label} from exercise at their sport, good job !.";
            }
            else
            {
                letterText = $"{pawn.Name} has gained the trait {traitDef.label} from exercise at their sport, good job !.";
            }
            string letterLabel = "Trait Gained";

            // Send the letter
            Find.LetterStack.ReceiveLetter(
                letterLabel,
                letterText,
                LetterDefOf.PositiveEvent,
                new LookTargets(pawn),
                null,
                null);
        }

        private void extractActualCells(List<IntVec3> actualCells, List<IntVec2> coords, string sideName)
        {
            // Log the start of the extraction
            //jcLog.Message($"[Comp_SportPitch] Extracting actual cells for {sideName}. Initial position: {parent.Position}, Offset count: {coords.Count}");

            // Clear the list of actual cells
            actualCells.Clear();

            // Get the position of the Thing this comp is attached to
            IntVec3 thingPosition = new IntVec3(parent.Position.x-parent.def.size.x/2, parent.Position.y, parent.Position.z - parent.def.size.z / 2);

            // Convert the offsets to actual map cells
            foreach (IntVec2 offset in coords)
            {
                // Calculate the actual cell by adding the offset to the Thing's position
                IntVec3 actualCell = thingPosition + new IntVec3(offset.x, 0, offset.z);
                actualCells.Add(actualCell);
                //jcLog.Message($"[Comp_SportPitch] Calculated actual cell for {sideName}: {actualCell} (Offset: {offset})");
                if (RimballSettings.drawPlayerSpots) { drawCellOverlay(actualCell); }
            }

            // Log the final result of the cell extraction
            //jcLog.Message($"[Comp_SportPitch] Finished extracting cells for {sideName}. Total cells: {actualCells.Count}");
        }

        public void drawCellOverlay(IntVec3 cell)
        {
            // Add map component if not present
            MapComponent_Overlay overlayComponent = parent.Map.GetComponent<MapComponent_Overlay>();
            if (overlayComponent == null)
            {
                overlayComponent = new MapComponent_Overlay(parent.Map);
                parent.Map.components.Add(overlayComponent);
            }
            overlayComponent.AddOverlayCell(cell, Color.green);
            try
            {
                GenDraw.DrawFieldEdges(new List<IntVec3> { cell }, Color.green);
            }
            catch (System.NullReferenceException e)
            {
                Log.Error(e.StackTrace);
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();

            // Save/load the list of IntVec2 coordinates
            Scribe_Collections.Look(ref Comp_SportPitchProps.sideACoords, "sideACoords", LookMode.Value);
            Scribe_Collections.Look(ref Comp_SportPitchProps.sideBCoords, "sideBCoords", LookMode.Value);
        }
    }

}
