// RimWorld.JobDriver_PlayBilliards
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Rimball
{
    class JobDriver_PracticeFootball : JobDriver //_WatchBuilding
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, job.def.joyMaxParticipants, 0, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A);
            Comp_SportPitch comp_SportPitch = base.TargetA.Thing.TryGetComp<Comp_SportPitch>();
            Toil chooseCell = FindRandomCellCanMoveTo(TargetIndex.A, TargetIndex.B, comp_SportPitch);
            yield return chooseCell;
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                job.locomotionUrgency = LocomotionUrgency.Sprint;
            };
            toil.tickAction = delegate
            {
                pawn.rotationTracker.FaceCell(base.TargetA.Thing.Position);
                if (ticksLeftThisToil == 300)
                {
                    SoundDef playSound = SoundDef.Named(comp_SportPitch.soundDefName);
                    //jcLog.Message($"[JobDriver_PracticeFootball] Successfully found playSound: {playSound} using soundDefName: {comp_SportPitch.soundDefName}");

                    // Play the sound at the pawn's position
                    comp_SportPitch.TryGiveTraitToPawn(pawn);
                    playSound.PlayOneShot((SoundInfo)new TargetInfo(pawn.Position, pawn.Map, false));
                }
                if (Find.TickManager.TicksGame > startTick + job.def.joyDuration)
                {
                    EndJobWith(JobCondition.Succeeded);
                }
                else
                {
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, 1f, (Building)base.TargetThingA);
                }
            };
            toil.handlingFacing = true;
            toil.socialMode = RandomSocialMode.Normal;
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 600;
            toil.AddFinishAction(delegate
            {
                JoyUtility.TryGainRecRoomThought(pawn);
            });
            yield return toil;
            yield return Toils_Reserve.Release(TargetIndex.B);
            yield return Toils_Jump.Jump(chooseCell);
        }
        public override object[] TaleParameters()
        {
            return new object[2]
            {
            pawn,
            base.TargetA.Thing.def
            };
        }

        public override bool CanBeginNowWhileLyingDown()
        {
            //jcLog.Message($"[JobDriver_PracticeFootball] Checking if job can begin while lying down for pawn: {pawn.Name}");
            return false;
        }

        public static Toil FindRandomCellCanMoveTo(TargetIndex areaInd, TargetIndex cellInd, Comp_SportPitch comp_SportPitch)
        {
            Toil findCell = ToilMaker.MakeToil("FindRandomCellInArea");
            findCell.initAction = delegate
            {
                Pawn actor = findCell.actor;
                Job curJob = actor.CurJob;
                LocalTargetInfo target = curJob.GetTarget(areaInd);

                if (!target.HasThing || !(target.Thing is Building))
                {
                    Log.Error($"{actor} could not find a cell within the specified area because the target is not a valid building or structure.");
                    actor.jobs.curDriver.EndJobWith(JobCondition.Errored);
                    return;
                }

                Building building = (Building)target.Thing;
                Map map = building.Map;

                if (map == null)
                {
                    Log.Error($"{actor} could not find a cell within the specified area because the map is null.");
                    actor.jobs.curDriver.EndJobWith(JobCondition.Errored);
                    return;
                }

                if (comp_SportPitch == null)
                {
                    Log.Error($"[JobDriver_PracticeFootball] Comp_SportPitch not found on targetA: {building}");
                    return;
                }

                List<IntVec3> cells = new List<IntVec3>();
                cells.AddRange(comp_SportPitch.sideACells);
                cells.AddRange(comp_SportPitch.sideBCells);

                //jcLog.Message($"[JoyGiver_PracticeFootball] Total cells available for play: {cells.Count}");

                cells.Shuffle(); // Shuffle the cells to randomize the selection

                int tries = 0;
                IntVec3 randomCell;
                int maxTries = cells.Count();

                do
                {
                    randomCell = cells[tries];
                    tries++;
                    if (tries > maxTries)
                    {
                        Log.Error($"{actor} could not find a suitable cell within the defined list of player positions.");
                        actor.jobs.curDriver.EndJobWith(JobCondition.Errored);
                        return;
                    }

                    //jcLog.Message($"[FindRandomCellInArea] Trying cell {randomCell}");

                }
                while (!actor.CanReserve(randomCell) || !actor.CanReach(randomCell, PathEndMode.OnCell, Danger.Deadly));

                //jcLog.Message($"[FindRandomCellInArea] Found valid cell {randomCell}");
                curJob.SetTarget(cellInd, randomCell);
            };

            return findCell;
        }

    }
}
