using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace Rimball
{
    class JoyGiver_PracticeFootball : JoyGiver_InteractBuilding
    {
        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            //jcLog.Message($"[JoyGiver_PracticeFootball] Attempting to give play job for pawn: {pawn.Name} with building: {t.Label}");

            if (t.HasComp<Comp_SportPitch>())
            {
                //jcLog.Message($"[JoyGiver_PracticeFootball] Thing has Comp_SportPitch. with pawn {pawn.Name} ");

                Comp_SportPitch comp_SportPitch = t.TryGetComp<Comp_SportPitch>();

                List<IntVec3> cells = new List<IntVec3>();
                cells.AddRange(comp_SportPitch.sideACells);
                cells.AddRange(comp_SportPitch.sideBCells);

                //jcLog.Message($"[JoyGiver_PracticeFootball] Total cells available for play: {cells.Count}");

                cells.Shuffle(); // Shuffle the cells to randomize the selection

                foreach (IntVec3 cell in cells)
                {
                    bool isStandable = cell.Standable(t.Map);
                    bool isThingForbidden = t.IsForbidden(pawn);
                    bool isCellForbidden = cell.IsForbidden(pawn);
                    bool isReserved = pawn.Map.pawnDestinationReservationManager.IsReserved(cell);
                    bool canReserve = pawn.CanReserveSittableOrSpot(cell);

                    //jcLog.Message($"[JoyGiver_PracticeFootball] Checking cell: {cell} | Standable: {isStandable} | Thing Forbidden: {isThingForbidden} | Cell Forbidden: {isCellForbidden} | Reserved: {isReserved} | Can Reserve: {canReserve}");

                    if (!pawn.IsPrisoner && isStandable && !isThingForbidden && !isCellForbidden && !isReserved && canReserve)
                    {
                        //jcLog.Message($"[JoyGiver_PracticeFootball] Assigning job to pawn {pawn.Name} at cell {cell}");
                        return JobMaker.MakeJob(def.jobDef, t, cell);
                    }
                }
            }

            //jcLog.Message($"[JoyGiver_PracticeFootball] No suitable cell found for pawn {pawn.Name}");
            return null;
        }

        public override Job TryGiveJobWhileInBed(Pawn pawn)
        {
            return null;
        }

        public override Job TryGiveJobInGatheringArea(Pawn pawn, IntVec3 gatheringSpot, float maxRadius = -1f)
        {
            return null;
        }
    }
}
