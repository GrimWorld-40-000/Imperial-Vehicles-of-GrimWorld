namespace Vehicles_of_Grimworld
{
    public sealed class JobDriver_CloseGarage : JobDriver_GarageControlBase
    {
        public sealed override void DoWork() =>
            GarageDoor?.StartClosing();

        public sealed override bool RequestCancelled() =>
            Map.designationManager.DesignationOn(TargetThingA, VehiclesOfGrimworldDefOf.GW_CloseGarageDoorRequest) is null;
    }
}
