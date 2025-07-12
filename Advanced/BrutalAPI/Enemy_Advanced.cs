using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.BrutalAPI
{
    /// <summary>
    /// An extended version of BrutalAPI's Enemy builder that creates enemies using Pentacle's AdvancedEnemySO class.
    /// </summary>
    [HarmonyPatch]
    public class Enemy_Advanced : Enemy
    {
        /// <summary>
        /// This enemy builder's AdvancedEnemySO enemy object.
        /// </summary>
        public AdvancedEnemySO advancedEnemy;

        /// <summary>
        /// Gets or sets this enemy's hidden passive effects.
        /// </summary>
        public List<HiddenEffectSO> HiddenEffects
        {
            get => advancedEnemy.hiddenEffects;
            set => advancedEnemy.hiddenEffects = value;
        }

        /// <summary>
        /// Creates a new AdvancedEnemySO enemy.
        /// </summary>
        /// <param name="displayName">The in-game display name of the enemy.</param>
        /// <param name="id_EN">The string ID of the enemy. Naming convention: EnemyName_EN</param>
        public Enemy_Advanced(string displayName, string id_EN) : base("_", "_")
        {
            enemy = advancedEnemy = ScriptableObject.CreateInstance<AdvancedEnemySO>();
            ID_EN = id_EN;
            Name = displayName;
            Size = 1;
            Health = 1;

            advancedEnemy.priority = MiscDB.DefaultPriority;
            advancedEnemy.abilitySelector = MiscDB.RarityAbilitySelector;
            advancedEnemy.enemyLoot = new();

            advancedEnemy.unitTypes = [];
            advancedEnemy.passiveAbilities = [];
            advancedEnemy.abilities = [];
            advancedEnemy.enterEffects = [];
            advancedEnemy.exitEffects = [];
        }

        [HarmonyPatch(typeof(Enemy), MethodType.Constructor, typeof(string), typeof(string))]
        [HarmonyPrefix]
        private static bool PreventNonAdvancedTempEnemyCreation_Prefix(Enemy __instance)
        {
            if (__instance is Enemy_Advanced)
                return false; // lol

            return true;
        }
    }
}
