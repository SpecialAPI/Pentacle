using BepInEx;
using Pentacle.CustomFogs;
using HarmonyLib;
using System;

namespace Pentacle
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class PentaclePlugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "BrutalOrchestraModding.Pentacle";
        public const string MOD_NAME = "Pentacle";
        public const string MOD_VERSION = "1.0.0";
        public const string MOD_PREFIX = "Pentacle";

        internal static Harmony HarmonyInstance;

        internal void Awake()
        {
            HarmonyInstance = new Harmony(MOD_GUID);
            HarmonyInstance.PatchAll();

            UnitExtTools.BuildUnitExtData();
            FogsCustomBossManager.AddFogsTranspiler();
        }
    }
}
