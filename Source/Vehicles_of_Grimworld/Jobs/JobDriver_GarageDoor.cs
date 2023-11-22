using System.Collections.Generic;
using Verse.AI;

namespace Vehicles_of_Grimworld
{
    public abstract class JobDriver_GarageControlBase : JobDriver
    {
        protected GarageDoor GarageDoor => job.targetA.Thing as GarageDoor;

        public sealed override bool TryMakePreToilReservations(bool errorOnFailed) =>
            pawn.Reserve(job.targetA, job, 1, errorOnFailed: errorOnFailed);

        protected sealed override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(RequestCancelled);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(15).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return new Toil
            {
                initAction = DoWork,
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        public abstract void DoWork();

        public abstract bool RequestCancelled();

    }
}