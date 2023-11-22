using RimWorld;
using UnityEngine;
using Verse;

namespace Vehicles_of_Grimworld
{
    public class Blueprint_GarageDoor : Blueprint_Build
    {
        public override Graphic Graphic
        {
            get
            {
                base.Graphic.drawSize = new Vector2(def.size.x, def.size.z);
                return base.Graphic;
            }
        }
    }
}