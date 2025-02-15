using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.BrutalAPI
{
    /// <summary>
    /// An advanced version of Enemy that has more options.
    /// </summary>
    [HarmonyPatch]
    public class Enemy_Advanced : Enemy
    {
        /// <summary>
        /// This enemy builder's enemy object as AdvancedEnemySO.
        /// </summary>
        public AdvancedEnemySO advancedEnemy;

        /// <summary>
        /// Gets or sets this enemy's hidden effects.
        /// </summary>
        public List<HiddenEffectSO> HiddenEffects
        {
            get => advancedEnemy.hiddenEffects;
            set => advancedEnemy.hiddenEffects = value;
        }

        /// <summary>
        /// Makes a new Advanced Enemy with id_EN as its id and displayName as its in-game name.
        /// </summary>
        /// <param name="displayName">This enemy's in-game name.</param>
        /// <param name="id_EN">THis enemy's internal id.</param>
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
