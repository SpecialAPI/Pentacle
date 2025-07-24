using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides intent-related tools,
    /// </summary>
    public static class IntentTools
    {
        /// <summary>
        /// Returns an intent for dealing a certain amount of damage.
        /// </summary>
        /// <param name="damage">The amount of damage for the intent.</param>
        /// <returns>The intent type for dealing the given amount of damage.</returns>
        public static string IntentForDamage(int damage)
        {
            return damage switch
            {
                <= 2 => IntentType_GameIDs.Damage_1_2.ToString(),
                <= 6 => IntentType_GameIDs.Damage_3_6.ToString(),
                <= 10 => IntentType_GameIDs.Damage_7_10.ToString(),
                <= 15 => IntentType_GameIDs.Damage_11_15.ToString(),
                <= 20 => IntentType_GameIDs.Damage_16_20.ToString(),

                _ => IntentType_GameIDs.Damage_21.ToString()
            };
        }

        /// <summary>
        /// Returns an intent for healing a certain amount of health.
        /// </summary>
        /// <param name="healing">The amount of healing for the intent.</param>
        /// <returns>The intent type for healing the given amount of health.</returns>
        public static string IntentForHealing(int healing)
        {
            return healing switch
            {
                <= 4 => IntentType_GameIDs.Heal_1_4.ToString(),
                <= 10 => IntentType_GameIDs.Heal_5_10.ToString(),
                <= 20 => IntentType_GameIDs.Heal_11_20.ToString(),

                _ => IntentType_GameIDs.Heal_21.ToString(),
            };
        }

        /// <summary>
        /// Shorthand for creating an IntentTargetInfo.
        /// </summary>
        /// <param name="target">The targeting that determines where the intents will be displayed on the field.</param>
        /// <param name="intents">Intent types that will be displayed on the targeted slots.</param>
        /// <returns>The created IntentTargetInfo object.</returns>
        public static IntentTargetInfo TargetIntent(BaseCombatTargettingSO target, params string[] intents)
        {
            return new()
            {
                targets = target,
                intents = intents
            };
        }
    }
}
