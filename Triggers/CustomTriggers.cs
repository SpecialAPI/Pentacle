using System;
using System.Collections.Generic;
using System.Text;
using Pentacle.Triggers.Args;

namespace Pentacle.Triggers
{
    /// <summary>
    /// Static class that stores the names of all custom triggers in Pentacle.
    /// </summary>
    public static class CustomTriggers
    {
        /// <summary>
        /// Gets sent to a unit after it perform an ability (like TriggerCals.OnAbilityUsed).
        /// <para>Sends an <see cref="AbilityUsedContext"/> as args, providing extra info about the ability that was performed.</para>
        /// </summary>
        public static readonly string OnAbilityPerformedContext = $"{MOD_PREFIX}_AbilityPerformedContext";
        /// <summary>
        /// Gets sent to a unit when it uses an ability, before the abiity's effects happen.
        /// <para>Sends an <see cref="AbilityUsedContext"/> as args, providing extra info about the ability that is being performed.</para>
        /// </summary>
        public static readonly string OnBeforeAbilityEffects = $"{MOD_PREFIX}_BeforeAbilityEffects";
        /// <summary>
        /// Gets sent to a character when its abilities get initialized.
        /// <para>Sends a <see cref="ModifyAbilityRankReference"/> as args. Its abilityRank value can be modified to change which level the character's abilities will be.</para>
        /// </summary>
        public static readonly string ModifyAbilityRank = $"{MOD_PREFIX}_ModifyAbilityRank";
        /// <summary>
        /// Gets sent to a character when calculating the amount of wrong pigment used for an ability.
        /// <para>Sends a <see cref="ModifyWrongPigmentReference"/> as args. Its wrongPigmentAmount value can be modified to change the amount of wrong pigment used for the ability.</para>
        /// </summary>
        public static readonly string ModifyWrongPigmentAmount = $"{MOD_PREFIX}_ModifyWrongPigmentAmount";
        /// <summary>
        /// Gets sent to a character when calculating the amount of wrong pigment in the cost slots of the perform ability button.
        /// <para>Sends a <see cref="ModifyWrongPigmentReference"/> as args. Its wrongPigmentAmount value can be modified to change the button's displayed amount of wrong pigment.</para>
        /// </summary>
        public static readonly string ModifyWrongPigmentAmount_UI = $"{MOD_PREFIX}_ModifyWrongPigmentAmount_UI";
        /// <summary>
        /// Gets sent to all enemies and characters when lucky pigment gets produced.
        /// <para>Sends an <see cref="OnLuckyPigmentSuccessReference"/> as args, providing info about how much lucky pigment was produced.</para>
        /// </summary>
        public static readonly string OnLuckyPigmentSuccess = $"{MOD_PREFIX}_OnLuckyPigmentSuccess";
        /// <summary>
        /// Gets sent to all enemies and characters when the lucky pigment chance fails. This trigger doesn't get sent if the pigment bar is full.
        /// <para>Sends <see langword="null"/> as args.</para>
        /// </summary>
        public static readonly string OnLuckyPigmentFailure = $"{MOD_PREFIX}_OnLuckyPigmentFailure";
        /// <summary>
        /// Gets sent to all enemies and characters when checking if a pigment color can be produced.
        /// <para>Sends a <see cref="CanProducePigmentColorReference"/>, providing info about the pigment color. Its canProduce value can be modified to change if the pigment color can be produced.</para>
        /// </summary>
        public static readonly string CanProducePigmentColor = $"{MOD_PREFIX}_CanProducePigmentColor";
        /// <summary>
        /// Gets sent to all enemies and party members when any enemy or party member gets moved (this includes both manual and non-manual movement).
        /// <para>Sends an <see cref="OnAnyoneMovedContext"/> as args, providing info about the moved unit and their slot before moving.</para>
        /// </summary>
        public static readonly string OnAnyoneMoved = $"{MOD_PREFIX}_OnAnyoneMoved";
        /// <summary>
        /// Gets sent to an enemy when an enemy rolls which abilities it will perform.
        /// <para>Sends an <see cref="EnemyAbilityOverrideReference"/> as args. If ability indexes are added the args' overrideAbiltyIDs list, the abilities the enemy rolls will be replaced by abilities of those indexes. -1 (or any other invalid ability index) can be added to the list to make the enemy not roll any abilities at all (as long as no valid ability indexes are in the list).</para>
        /// </summary>
        public static readonly string OverrideEnemyAbilityUsage = $"{MOD_PREFIX}_OverrideEnemyAbilityUsage";
        /// <summary>
        /// Gets sent to all enemies, characters and slots at the start of the player's turn.
        /// <para>Sends <see langword="null"/> as args.</para>
        /// </summary>
        public static readonly string OnPlayerTurnStartUniversal = $"{MOD_PREFIX}_OnPlayerTurnStartUniversal";
        /// <summary>
        /// Gets sent to all enemies and party members when a status effect is applied to a unit who doesn't already have it.
        /// <para>Sends a <see cref="TargetedStatusEffectApplication"/> as args, providing info about the unit who the status effect was applied to, the status effect and the applied amount.</para>
        /// </summary>
        public static readonly string StatusEffectFirstAppliedToAnyone = $"{MOD_PREFIX}_StatusEffectFirstApliedToAnyone";
        /// <summary>
        /// Gets sent to all enemies and party members when a status effect is applied to a unit.
        /// <para>Sends a <see cref="TargetedStatusEffectApplication"/> as args, providing info about the unit who the status effect was applied to, the status effect and the applied amount.</para>
        /// </summary>
        public static readonly string StatusEffectAppliedToAnyone = $"{MOD_PREFIX}_StatusEffectAppliedToAnyone";
        /// <summary>
        /// Gets sent to all enemies and party members when a status effect is either applied to a unit who already has it or when is increased in amount through other means.
        /// <para>Sends a <see cref="TargetedStatusEffectApplication"/> as args, providing info about the unit who the status effect was applied to, the status effect and the applied amount.</para>
        /// </summary>
        public static readonly string StatusEffectIncreasedOnAnyone = $"{MOD_PREFIX}_StatusEffectIncreasedOnAnyone";

        //public static readonly string GetForbiddenFruitID = $"{MOD_PREFIX}_GetForbiddenFruitID";
        //public static readonly string CanForbiddenFruitMatch = $"{MOD_PREFIX}_CanForbiddenFruitMatch";
        //public static readonly string TriggerForbiddenFruit = $"{MOD_PREFIX}_TriggerForbiddenFruit";

        // TODO:
        //  public static readonly string ModifyTargetting = $"{MOD_PREFIX}_ModifyTargetting";
        //  public static readonly string ModifyTargettingIntents = $"{MOD_PREFIX}_ModifyTargettingIntents";
    }
}
