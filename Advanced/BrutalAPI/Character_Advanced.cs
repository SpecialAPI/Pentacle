using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.BrutalAPI
{
    /// <summary>
    /// An advanced version of Character that has more options.
    /// </summary>
    [HarmonyPatch]
    public class Character_Advanced : Character
    {
        /// <summary>
        /// This character builder's character object as AdvancedCharacterSO.
        /// </summary>
        public AdvancedCharacterSO advancedCharacter;

        /// <summary>
        /// Gets or sets this character's hidden effects.
        /// </summary>
        public List<HiddenEffectSO> HiddenEffects
        {
            get => advancedCharacter.hiddenEffects;
            set => advancedCharacter.hiddenEffects = value;
        }

        /// <summary>
        /// Makes a new Advanced Character with id_CH as its id and displayName as its in-game name.
        /// </summary>
        /// <param name="displayName">This character's in-game name.</param>
        /// <param name="id_CH">This character's internal id.</param>
        public Character_Advanced(string displayName, string id_CH) : base("_", "_")
        {
            character = advancedCharacter = ScriptableObject.CreateInstance<AdvancedCharacterSO>();

            ID_CH = id_CH;
            EntityID = id_CH;
            Name = displayName;
            menuCharacter = null;
            ignoredSupport = null;
            ignoredDPS = null;

            advancedCharacter.basicCharAbility = AbilityDB.SlapAbility;

            advancedCharacter.unitTypes = [];
            advancedCharacter.m_BossAchData = [];
            advancedCharacter.passiveAbilities = [];
            advancedCharacter.rankedData = [];
            advancedCharacter.movesOnOverworld = true;
        }

        [HarmonyPatch(typeof(Character), MethodType.Constructor, typeof(string), typeof(string))]
        [HarmonyPrefix]
        private static bool PreventNonAdvancedTempCharacterCreation_Prefix(Character __instance)
        {
            if (__instance is Character_Advanced)
                return false; // lol

            return true;
        }
    }
}
