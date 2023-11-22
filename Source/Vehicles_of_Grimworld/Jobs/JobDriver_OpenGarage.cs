namespace Vehicles_of_Grimworld
{
    public sealed class JobDriver_OpenGarage : JobDriver_GarageControlBase
    {
        public override void DoWork() =>
            GarageDoor?.StartOpening();

        public override bool RequestCancelled() =>
            Map.designationManager.DesignationOn(TargetThingA, VehiclesOfGrimworldDefOf.GW_OpenGarageDoorRequest) is null;
    }
}
