using HarmonyLib;
using Verse;

namespace Vehicles_of_Grimworld
{
    public class Vehicles_of_GrimworldMod : Mod
    {
        private const string ModName = "Vehicles_of_Grimworld.Mod";
        private readonly Harmony _harmony = new Harmony(ModName);
        public Vehicles_of_GrimworldMod(ModContentPack content) : base(content)
        {
            _harmony.PatchAll();
        }
    }
}