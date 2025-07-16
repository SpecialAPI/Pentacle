using Google.Protobuf.Collections;
using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Yarn.Unity;

namespace Pentacle.CustomFogs
{
    /// <summary>
    /// A tool for adding custom final bosses to Fogs' final boss selection.
    /// </summary>
    public static class FogsCustomBossManager
    {
        internal static readonly Dictionary<string, string> CustomFogsBosses = [];

        /// <summary>
        /// Adds a custom final boss to Fogs' final boss selection.
        /// </summary>
        /// <param name="bossId">The custom boss' internal bossId.</param>
        /// <param name="dialogueOptionName">How the boss should be called in Fogs' dialogue options.</param>
        public static void AddCustomFinalBossToFogs(string bossId, string dialogueOptionName)
        {
            if (CustomFogsBosses.ContainsKey(bossId))
                Debug.LogError($"A custom final boss with the bossId \"{bossId}\" has already been added to Fogs.");

            CustomFogsBosses[bossId] = dialogueOptionName;
        }
    }
}
