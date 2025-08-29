using Pentacle.Tools;
using Pentacle.TriggerEffects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating custom passives.
    /// </summary>
    public static class PassiveBuilder
    {
        /// <summary>
        /// Creates a new passive of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created passive. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="id_PA">The string database ID of the passive. Naming convention: PassiveName_PA</param>
        /// <param name="passiveId">The string passive ID of the passive used to check if two passives are the same.<para>This ID is not automatically prefixed. If you create a modded passive rather than a variation of a basegame passive, you should manually prefix this ID.</para></param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns></returns>
        public static T NewPassive<T>(string id_PA, string passiveId, ModProfile profile = null) where T : BasePassiveAbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var pa = CreateScriptable<T>();
            pa.name = profile.GetID(id_PA);
            pa.m_PassiveID = passiveId;

            pa._characterDescription = "This passive is not meant for party members.";
            pa._enemyDescription = "This passive is not meant for enemies.";

            return pa;
        }

        /// <summary>
        /// Sets the in-game name of the passive.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="name">The new display name for the passive.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetName<T>(this T pa, string name) where T : BasePassiveAbilitySO
        {
            pa._passiveName = name;

            return pa;
        }

        /// <summary>
        /// Sets the in-game sprite for the passive.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetSprite<T>(this T pa, string spriteName, ModProfile profile = null) where T : BasePassiveAbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return pa;

            pa.passiveIcon = profile.LoadSprite(spriteName);

            return pa;
        }

        /// <summary>
        /// Sets the in-game sprite for the passive.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="sprite">The Sprite object of the passive's sprite.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetSprite<T>(this T pa, Sprite sprite) where T : BasePassiveAbilitySO
        {
            pa.passiveIcon = sprite;

            return pa;
        }

        /// <summary>
        /// Sets the passive's name and sprite.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="name">The new display name for the passive.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetBasicInformation<T>(this T pa, string name, string spriteName, ModProfile profile = null) where T : BasePassiveAbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return pa;

            pa._passiveName = name;
            pa.passiveIcon = profile.LoadSprite(spriteName);

            return pa;
        }

        /// <summary>
        /// Sets the passive's name and sprite.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="name">The new display name for the passive.</param>
        /// <param name="sprite">The Sprite object of the passive's sprite.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetBasicInformation<T>(this T pa, string name, Sprite sprite) where T : BasePassiveAbilitySO
        {
            pa._passiveName = name;
            pa.passiveIcon = sprite;

            return pa;
        }

        /// <summary>
        /// Sets the passive's in-game description for characters.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="description">The new character description for the passive.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetCharacterDescription<T>(this T pa, string description) where T : BasePassiveAbilitySO
        {
            pa._characterDescription = description;

            return pa;
        }

        /// <summary>
        /// Sets the passive's in-game description for enemies.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="description">The new enemy description for the passive.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetEnemyDescription<T>(this T pa, string description) where T : BasePassiveAbilitySO
        {
            pa._enemyDescription = description;

            return pa;
        }

        /// <summary>
        /// Automatically sets the passive's descriptions for enemies and party members using the given template.
        /// <para>For characters, "ally" is replaced with "party member", "opponent" with "enemy", "allies" with "party members" and "opponents" with "enemies".</para>
        /// <para>For enemies, "ally" is replaced with "enemy", "opponent" with "party member", "allies" with "enemies" and "opponents" with "party members".</para>
        /// <para>The replacements done by this method keep the original word's case.</para>
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="descriptionTemplate">The teplate for the descriptions.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T AutoSetDescriptions<T>(this T pa, string descriptionTemplate) where T : BasePassiveAbilitySO
        {
            pa._characterDescription = DoPassiveDescriptionAutoInserts(descriptionTemplate, false);
            pa._enemyDescription = DoPassiveDescriptionAutoInserts(descriptionTemplate, true);

            return pa;
        }

        /// <summary>
        /// Makes a character/enemy passive description using the given template.
        /// <para>For characters, "ally" is replaced with "party member", "opponent" with "enemy", "allies" with "party members" and "opponents" with "enemies".</para>
        /// <para>For enemies, "ally" is replaced with "enemy", "opponent" with "party member", "allies" with "enemies" and "opponents" with "party members".</para>
        /// <para>The replacements done by this method keep the original word's case.</para>
        /// </summary>
        /// <param name="description">The template for the description.</param>
        /// <param name="enemy">Whether this should make an enemy description or a character description.</param>
        /// <returns>The made description.</returns>
        public static string DoPassiveDescriptionAutoInserts(string description, bool enemy)
        {
            for (int i = 0; i < CharacterDescriptionInserts.Length; i++)
            {
                var idx = i;

                if (enemy)
                    idx = i % 2 == 0 ? i + 1 : i - 1;

                if (idx >= CharacterDescriptionInserts.Length)
                    idx = i;

                var orig = (DescriptionInsert)i;
                var rep = CharacterDescriptionInserts[idx];

                description = description.ReplaceAndKeepCase(orig.ToString(), rep);
            }

            return description;
        }

        /// <summary>
        /// Sets the visual tooltip of the "Stored value" e.g. mutualism value of the passive.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="storeData">The stored value that will be displayed, as a UnitStoreData_BasicSO object.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetStoredValue<T>(this T pa, UnitStoreData_BasicSO storeData) where T : BasePassiveAbilitySO
        {
            pa.specialStoredData = storeData;

            return pa;
        }

        /// <summary>
        /// Adds the passive to the Glossary.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="glossaryDescription">The description that should be displayed in the Glossary.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T AddToGlossary<T>(this T pa, string glossaryDescription) where T : BasePassiveAbilitySO
        {
            Glossary.CreateAndAddCustom_PassiveToGlossary(pa._passiveName, glossaryDescription, pa.passiveIcon);

            return pa;
        }

        /// <summary>
        /// Adds the passive to the Passive Database.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must be a subclass of BasePassiveAbilitySO.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T AddToDatabase<T>(this T pa) where T : BasePassiveAbilitySO
        {
            Passives.AddCustomPassiveToPool(pa.name, pa._passiveName, pa);

            return pa;
        }

        /// <summary>
        /// Sets the trigger effects of a MultiCustomTriggerEffectPassive passive. This will remove any trigger effects the passive had before this.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must either be MultiCustomTriggerEffectPassive or a subclass of MultiCustomTriggerEffectPassive.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="triggerEffects">The new trigger effects for the passive, as EffectsAndTrigger objects.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetTriggerEffects<T>(this T pa, List<EffectsAndTrigger> triggerEffects) where T : MultiCustomTriggerEffectPassive
        {
            pa.triggerEffects = triggerEffects;

            return pa;
        }

        /// <summary>
        /// Sets the connection effects of a MultiCustomTriggerEffectPassive passive. This will remove any connection effects the passive had before this.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must either be MultiCustomTriggerEffectPassive or a subclass of MultiCustomTriggerEffectPassive.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="connectionEffects">The new connection effects for the passive, as TriggeredEffect objects.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetConnectionEffects<T>(this T pa, List<TriggeredEffect> connectionEffects) where T : MultiCustomTriggerEffectPassive
        {
            pa.connectionEffects = connectionEffects;

            return pa;
        }

        /// <summary>
        /// Sets the disconnection effects of a MultiCustomTriggerEffectPassive passive. This will remove any disconnection effects the passive had before this.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must either be MultiCustomTriggerEffectPassive or a subclass of MultiCustomTriggerEffectPassive.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="disconnectionEffects">The new disconnection effects for the passive, as TriggeredEffect objects.</param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T SetDisconnectionEffects<T>(this T pa, List<TriggeredEffect> disconnectionEffects) where T : MultiCustomTriggerEffectPassive
        {
            pa.disconnectionEffects = disconnectionEffects;

            return pa;
        }

        /// <summary>
        /// Adds a list of effects to a MultiCustomTriggerEffectPassive passive's list of trigger, connection and disconnection effects.
        /// </summary>
        /// <typeparam name="T">The passive's custom type. Must either be MultiCustomTriggerEffectPassive or a subclass of MultiCustomTriggerEffectPassive.</typeparam>
        /// <param name="pa">The object instance of the passive.</param>
        /// <param name="effects">The effects to add to the effect lists, as EffectsAndTrigger objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the passive, for method chaining.</returns>
        public static T AddEffectsToAll<T>(this T pa, params EffectsAndTrigger[] effects) where T : MultiCustomTriggerEffectPassive
        {
            pa.triggerEffects.AddRange(effects);
            pa.connectionEffects.AddRange(effects);
            pa.disconnectionEffects.AddRange(effects);

            return pa;
        }

        internal static readonly string[] CharacterDescriptionInserts =
        [
            "party member",
            "enemy",

            "party members",
            "enemies",
        ];

        /// <summary>
        /// The types of inserts that can be used in the description templates for AutoSetDescriptions and DoPassiveDescriptionAutoInserts.
        /// </summary>
        public enum DescriptionInsert
        {
            /// <summary>
            /// Replaced with "party member" for characters and "enemy" for enemies.
            /// </summary>
            ally,
            /// <summary>
            /// Replaced with "enemy" for characters and "party member" for enemies.
            /// </summary>
            opponent,

            /// <summary>
            /// Replaced with "party members" for characters and "enemies" for enemies.
            /// </summary>
            allies,
            /// <summary>
            /// Replaced with "enemies" for characters and "party members" for enemies.
            /// </summary>
            opponents
        }
    }
}
