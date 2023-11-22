using HarmonyLib;
using UnityEngine;
using Verse;

namespace Vehicles_of_Grimworld.Patches
{
    [HarmonyPatch(typeof(GhostDrawer), nameof(GhostDrawer.DrawGhostThing))]
    public static class GhostDrawer_DrawGhostThing
    {
        public static void Prefix(ThingDef thingDef, ref Vector3 __state)
        {
            if (typeof(GarageDoor).IsAssignableFrom(thingDef.thingClass))
                thingDef.graphic.drawSize = new Vector2(thingDef.size.x, thingDef.size.z);
        }
    }
}