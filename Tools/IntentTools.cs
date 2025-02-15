using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class IntentTools
    {
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
