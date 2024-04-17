using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Vehicles_of_Grimworld
{
    public class GarageDoor : Building
    {
        public static List<GarageDoor> GarageDoors { get; set; } = new List<GarageDoor>();
        public CompPowerTrader CompPower { get; set; }

        private const float OpenSpeed = 100f;

        protected DoorState _doorState;
        private int _transitionEnd;
        private bool _transitionRequested = false;

        public void StartOpening()
        {
            _transitionRequested = false;
            Map.designationManager.DesignationOn(this, VehiclesOfGrimworldDefOf.GW_OpenGarageDoorRequest)?.Delete();
            _transitionEnd = Find.TickManager.TicksGame + (int)OpenSpeed;
            _doorState = DoorState.Opening;
        }

        public void StartClosing()
        {
            _transitionRequested = false;
            Map.designationManager.DesignationOn(this, VehiclesOfGrimworldDefOf.GW_CloseGarageDoorRequest)?.Delete();
            _transitionEnd = Find.TickManager.TicksGame + (int)OpenSpeed;
            _doorState = DoorState.Closing;
        }

        public sealed override Color DrawColor
        {
            get
            {
                var color = base.DrawColor;
                const float MinAlpha = 0.6f; // Any lower than this and you'll get artefacting around larger doors
                switch (_doorState)
                {
                    case DoorState.Open:
                        color.a = MinAlpha;
                        break;
                    case DoorState.Opening:
                        color.a = CalculateProgress().Map(0, 1, MinAlpha, 1);
                        break;
                    case DoorState.Closing:
                        color.a = CalculateProgress().Map(0, 1, 1, MinAlpha);
                        break;
                    case DoorState.Closed:
                    default:
                        break;
                }

                return color;
            }
        }

        public sealed override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            GarageDoors.Add(this);
            CompPower = this.TryGetComp<CompPowerTrader>();
        }

        public sealed override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            GarageDoors.Remove(this);
        }

        public sealed override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (var doorGizmo in GetGarageDoorGizmos())
            {
                yield return doorGizmo;
            }
        }

        public sealed override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _doorState, "doorState", DoorState.Closed);
            Scribe_Values.Look(ref _transitionEnd, "transitionEnd");
            Scribe_Values.Look(ref _transitionRequested, "transitionRequested");
        }

        public sealed override void Tick()
        {
            base.Tick();
            switch (_doorState)
            {
                case DoorState.Opening:
                    if (Find.TickManager.TicksGame >= _transitionEnd)
                        Open();
                    break;
                case DoorState.Closing:
                    if (Find.TickManager.TicksGame >= _transitionEnd)
                        Close();
                    break;
                case DoorState.Closed:
                case DoorState.Open:
                default:
                    break;
            }
        }

        protected sealed override void DrawAt(Vector3 pos, bool y)
        {
            float offset;
            switch (_doorState)
            {
                case DoorState.Open:
                    offset = 1;
                    break;
                case DoorState.Opening:
                    offset = 1 - CalculateProgress();
                    break;
                case DoorState.Closing:
                    offset = CalculateProgress();
                    break;
                case DoorState.Closed:
                default:
                    offset = 0;
                    break;
            }

            var size = new Vector2(def.size.x, def.size.z);
            var drawPos = DrawPos;

            drawPos.z += Rotation == Rot4.North ? -offset :
                         Rotation == Rot4.South ? offset : 0;

            drawPos.x += Rotation == Rot4.East ? -offset :
                         Rotation == Rot4.West ? offset : 0;

            Graphics.DrawMesh(MeshPool.GridPlane(size), drawPos, Rotation.AsQuat,
                def.graphicData.GraphicColoredFor(this).MatAt(Rotation),
                0);

            Graphic.ShadowGraphic?.DrawWorker(drawPos, Rotation, def, this, 0f);
            Comps_PostDraw();
        }

        private float CalculateProgress() =>
            (_transitionEnd - Find.TickManager.TicksGame) / OpenSpeed;

        protected virtual IEnumerable<Gizmo> GetGarageDoorGizmos()
        {
            if (_transitionRequested)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Cancel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel"),
                    action = Cancel
                };
            }
            else
            {
                switch (_doorState)
                {
                    case DoorState.Closed:
                        yield return new Command_Action
                        {
                            defaultLabel = "GW_Open".Translate(),
                            defaultDesc = "GW_OpenDescription".Translate(),
                            icon = ContentFinder<Texture2D>.Get("Things/Building/GarageDoors/GW_GarageDoor_Open"),
                            action = RequestOpen
                        };
                        break;
                    case DoorState.Open:
                        yield return new Command_Action
                        {
                            defaultLabel = "GW_Close".Translate(),
                            defaultDesc = "GW_CloseDescription".Translate(),
                            icon = ContentFinder<Texture2D>.Get("Things/Building/GarageDoors/GW_GarageDoor_Close"),
                            action = RequestClose
                        };
                        break;
                    case DoorState.Opening:
                    case DoorState.Closing:
                    default:
                        break;
                }
            }
        }

        protected virtual void SpawnGarage(GarageDoor replacementGarage)
        {
            replacementGarage.HitPoints = HitPoints;
            replacementGarage.Graphic.drawSize = new Vector2(def.size.x, def.size.z);
            replacementGarage.SetFaction(Faction);
            replacementGarage.ChangePaint(PaintColorDef);

            var pos = Position;
            var map = Map;
            var selected = Find.Selector.IsSelected(this);
            Destroy();
            GenSpawn.Spawn(replacementGarage, pos, map, Rotation);
            if (selected)
                Find.Selector.Select(replacementGarage);
        }

        private void Open()
        {
            var openGarage = ThingMaker.MakeThing(ThingDef.Named(def.defName.Replace("Closed", "Open")), Stuff) as GarageDoor;
            openGarage._doorState = DoorState.Open;
            SpawnGarage(openGarage);
        }

        private void Close()
        {
            var closedGarage = ThingMaker.MakeThing(ThingDef.Named(def.defName.Replace("Open", "Closed")), Stuff) as GarageDoor;
            closedGarage._doorState = DoorState.Closed;
            SpawnGarage(closedGarage);
        }

        private void Cancel()
        {
            _transitionRequested = false;
            Map.designationManager.TryRemoveDesignationOn(this, VehiclesOfGrimworldDefOf.GW_OpenGarageDoorRequest);
            Map.designationManager.TryRemoveDesignationOn(this, VehiclesOfGrimworldDefOf.GW_CloseGarageDoorRequest);
        }

        private void RequestOpen()
        {
            _transitionRequested = true;
            if (Map.designationManager.DesignationOn(this, VehiclesOfGrimworldDefOf.GW_OpenGarageDoorRequest) is null)
                Map.designationManager.AddDesignation(new Designation(this, VehiclesOfGrimworldDefOf.GW_OpenGarageDoorRequest));
        }

        private void RequestClose()
        {
            _transitionRequested = true;
            if (Map.designationManager.DesignationOn(this, VehiclesOfGrimworldDefOf.GW_CloseGarageDoorRequest) is null)
                Map.designationManager.AddDesignation(new Designation(this, VehiclesOfGrimworldDefOf.GW_CloseGarageDoorRequest));
        }
    }
}