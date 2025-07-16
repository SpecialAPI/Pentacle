using Google.Protobuf.Collections;
using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Yarn.Unity;

namespace Pentacle.CustomFogs
{
    /// <summary>
    /// Static class that provides a tool for adding custom final bosses to Fogs' final boss selection.
    /// </summary>
    public static class FogsCustomBossManager
    {
        internal static readonly Dictionary<string, string> CustomFogsBosses = [];

        /// <summary>
        /// Adds a custom final boss encounter as an option to Fogs' final boss selection.
        /// </summary>
        /// <param name="bossId">The BossID of the boss encounter.<para>Not to be confused with the enemy ID of the actual boss enemy.</para></param>
        /// <param name="dialogueOptionName">The in-game title for the boss that will be displayed in Fogs' dialogue options.</param>
        public static void AddCustomFinalBossToFogs(string bossId, string dialogueOptionName)
        {
            if (CustomFogsBosses.ContainsKey(bossId))
                Debug.LogError($"A custom final boss with the bossId \"{bossId}\" has already been added to Fogs.");

            CustomFogsBosses[bossId] = dialogueOptionName;
        }
    }
}
