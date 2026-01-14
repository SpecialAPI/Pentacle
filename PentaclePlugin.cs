using BepInEx;
using Pentacle.CustomFogs;
using HarmonyLib;
using System;
using Pentacle.Internal;
using BepInEx.Logging;

namespace Pentacle
{
    /// <summary>
    /// Pentacle's BepInEx plugin class.
    /// </summary>
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class PentaclePlugin : BaseUnityPlugin
    {
        /// <summary>
        /// Pentacle's BepInEx plugin GUID.
        /// </summary>
        public const string MOD_GUID = "BrutalOrchestraModding.Pentacle";
        /// <summary>
        /// Pentacle's BepInEx plugin name.
        /// </summary>
        public const string MOD_NAME = "Pentacle";
        /// <summary>
        /// Pentacle's current version.
        /// </summary>
        public const string MOD_VERSION = "0.0.1";
        /// <summary>
        /// Pentacle's mod prefix.
        /// </summary>
        public const string MOD_PREFIX = "Pentacle";

        internal static Harmony HarmonyInstance;
        internal static ManualLogSource PentacleLogger = BepInEx.Logging.Logger.CreateLogSource("Pentacle");

        internal void Awake()
        {
            HarmonyInstance = new Harmony(MOD_GUID);
            HarmonyInstance.PatchAll();

            UnitExtTools.BuildUnitExtData();
            FogsCustomBossPatches.AddFogsTranspiler();
        }

        internal void Start()
        {
            DelayedActions.ProcessPostAwakeActions();

            DamageReversePatches.Patch();
        }
    }
}
