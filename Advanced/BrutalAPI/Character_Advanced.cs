using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.BrutalAPI
{
    /// <summary>
    /// An extended version of BrutalAPI's Character builder that creates characters using Pentacle's AdvancedCharacterSO class.
    /// </summary>
    [HarmonyPatch]
    public class Character_Advanced : Character
    {
        /// <summary>
        /// This character builder's AdvancedCharacterSO character object.
        /// </summary>
        public AdvancedCharacterSO advancedCharacter;

        /// <summary>
        /// Gets or sets this character's hidden passive effects.
        /// </summary>
        public List<HiddenEffectSO> HiddenEffects
        {
            get => advancedCharacter.hiddenEffects;
            set => advancedCharacter.hiddenEffects = value;
        }

        /// <summary>
        /// Creates a new AdvancedCharacterSO character.
        /// </summary>
        /// <param name="displayName">The in-game display name of the character.</param>
        /// <param name="id_CH">The string ID of the character. Naming convention: CharacterName_CH</param>
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
