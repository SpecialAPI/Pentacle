using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent
{
    public static class CustomTriggers
    {
        /// <summary>
        /// Same as TriggerCalls.OnAbilityUsed, but sends an AbilityUsedContext as args.
        /// </summary>
        public static readonly string OnAbilityPerformedContext = $"{MOD_PREFIX}_AbilityPerformedContext";
        /// <summary>
        /// Triggers before on a unit before it performs an ability. Sends an AbilityUsedContext as args.
        /// </summary>
        public static readonly string OnBeforeAbilityEffects = $"{MOD_PREFIX}_BeforeAbilityEffects";
        /// <summary>
        /// Triggers on CharacterCombat.SetUpDefaultAbilities(). Sends an IntegerReference as args which can be modified to change the rank of the character's abilities.
        /// </summary>
        public static readonly string ModifyAbilityRank = $"{MOD_PREFIX}_ModifyAbilityRank";
        /// <summary>
        /// Triggers to modify how much wrong pigment is used by an ability. Sends an IntegerReference as args which can be modified to change the amount of wrong pigment.
        /// </summary>
        public static readonly string ModifyWrongPigmentAmount = $"{MOD_PREFIX}_ModifyWrongPigmentAmount";
        /// <summary>
        /// Triggers to modify how much wrong pigment is displayed in the UI. Sends an IntegerReference as args which can be modified to change the amount of wrong pigment.
        /// </summary>
        public static readonly string ModifyWrongPigmentAmount_UI = $"{MOD_PREFIX}_ModifyWrongPigmentAmount_UI";
        /// <summary>
        /// Triggers when the lucky pigment roll succeeds and lucky pigment is successfully produced. Sends an IntegerReference as args, whose value is the amount of lucky pigment produced.
        /// </summary>
        public static readonly string OnLuckyPigmentSuccess = $"{MOD_PREFIX}_OnLuckyPigmentSuccess";
        /// <summary>
        /// Triggers when the lucky pigment roll fails. This doesn't happen if the pigment bar is full.
        /// </summary>
        public static readonly string OnLuckyPigmentFailure = $"{MOD_PREFIX}_OnLuckyPigmentFailure";
        /// <summary>
        /// Triggers to check if pigment can be produced. Sends a CanProducePigmentColorReference as args, whose BooleanReference can be modified to change if pigment can be produced.
        /// </summary>
        public static readonly string CanProducePigmentColor = $"{MOD_PREFIX}_CanProducePigmentColor";
        public static readonly string OnAnyoneMoved = $"{MOD_PREFIX}_OnAnyoneMoved";
        public static readonly string OverrideEnemyAbilityUsage = $"{MOD_PREFIX}_OverrideEnemyAbilityUsage";
        public static readonly string OnPlayerTurnStartUniversal = $"{MOD_PREFIX}_OnPlayerTurnStartUniversal";

        /// <summary>
        /// Triggers when processing forbidden fruit to get a unit's forbidden fruit ID. Sends a StringReference as args which can be modifed to set a unit's forbidden fruit ID.
        /// </summary>
        public static readonly string GetForbiddenFruitID = $"{MOD_PREFIX}_GetForbiddenFruitID";
        /// <summary>
        /// Triggers to check whether this unit can form a Forbidden Fruit match with another unit. Sends a ForbiddenFruitMatchInfo as args whose BooleanReference can be modified to set if the match can be formed.
        /// </summary>
        public static readonly string CanForbiddenFruitMatch = $"{MOD_PREFIX}_CanForbiddenFruitMatch";
        /// <summary>
        /// Triggers on the first unit of a successful forbidden fruit match. Sends a ForbiddenFruitMatchInfo as args for information about the other match unit.
        /// </summary>
        public static readonly string TriggerForbiddenFruit = $"{MOD_PREFIX}_TriggerForbiddenFruit";

        public static readonly string StatusEffectFirstAppliedToAnyone = $"{MOD_PREFIX}_StatusEffectFirstApliedToAnyone";
        public static readonly string StatusEffectAppliedToAnyone = $"{MOD_PREFIX}_StatusEffectAppliedToAnyone";
        public static readonly string StatusEffectIncreasedOnAnyone = $"{MOD_PREFIX}_StatusEffectIncreasedOnAnyone";

        // TODO:
        //  public static readonly string ModifyTargetting = $"{MOD_PREFIX}_ModifyTargetting";
        //  public static readonly string ModifyTargettingIntents = $"{MOD_PREFIX}_ModifyTargettingIntents";
    }
}
