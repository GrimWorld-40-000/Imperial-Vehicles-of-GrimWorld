using Verse.AI;
using Verse;

namespace Vehicles_of_Grimworld
{
    public sealed class WorkGiver_OpenGarage : WorkGiver_ManageGarage
    {
        public sealed override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return pawn.CanReserveAndReach(t, PathEndMode, MaxPathDanger(pawn), ignoreOtherReservations: forced) &&
                !(t.Map.designationManager.DesignationOn(t, VehiclesOfGrimworldDefOf.GW_OpenGarageDoorRequest) is null);
        }

        public sealed override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(VehiclesOfGrimworldDefOf.GW_OpenGarageDoor, t);
        }
    }
}