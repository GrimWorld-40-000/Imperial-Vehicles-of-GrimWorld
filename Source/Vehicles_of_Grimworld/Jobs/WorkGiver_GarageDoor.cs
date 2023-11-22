using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using Verse;

namespace Vehicles_of_Grimworld
{
    public abstract class WorkGiver_ManageGarage : WorkGiver_Scanner
    {
        public sealed override PathEndMode PathEndMode => PathEndMode.Touch;

        public sealed override Danger MaxPathDanger(Pawn pawn) =>
            Danger.Deadly;

        public sealed override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) =>
            GarageDoor.GarageDoors.Where(x => x?.Map == pawn.Map);
    }
}
