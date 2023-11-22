using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Vehicles_of_Grimworld
{
    public sealed class GarageAutodoor : GarageDoor
    {
        protected override IEnumerable<Gizmo> GetGarageDoorGizmos()
        {
            switch (_doorState)
            {
                case DoorState.Closed:
                    var openAction = new Command_Action
                    {
                        defaultLabel = "GW_Open".Translate(),
                        defaultDesc = "GW_Autodoor_OpenDescription".Translate(),
                        icon = ContentFinder<Texture2D>.Get("Things/Building/GarageDoors/GW_GarageDoor_Open"),
                        action = StartOpening
                    };
                    if (!GetComp<CompPowerTrader>().PowerOn)
                        openAction.Disable("NoPower".Translate());
                    yield return openAction;
                    break;
                case DoorState.Open:
                    var closeAction = new Command_Action
                    {
                        defaultLabel = "GW_Close".Translate(),
                        defaultDesc = "GW_Autodoor_CloseDescription".Translate(),
                        icon = ContentFinder<Texture2D>.Get("Things/Building/GarageDoors/GW_GarageDoor_Close"),
                        action = StartClosing
                    };
                    if (!GetComp<CompPowerTrader>().PowerOn)
                        closeAction.Disable("NoPower".Translate());
                    yield return closeAction;
                    break;
                case DoorState.Opening:
                case DoorState.Closing:
                default:
                    break;
            }
        }

        protected override void SpawnGarage(GarageDoor replacementGarage)
        {
            base.SpawnGarage(replacementGarage);
            if (replacementGarage.CompPower is null)
                return;

            replacementGarage.CompPower.PowerOn = true;
            replacementGarage.CompPower.SetUpPowerVars();
            PowerConnectionMaker.TryConnectToAnyPowerNet(replacementGarage.CompPower);
        }
    }
}