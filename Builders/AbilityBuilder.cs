using Pentacle.Advanced;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating abilities.
    /// </summary>
    public static class AbilityBuilder
    {
        /// <summary>
        /// Get a "reference" to an ability of the given type, even before that ability is created. This only works on abilities made on the same mod profile.
        /// <para>More specifically, if the ability isn't created when this method is called, this method creates and returns an empty ability object of the given type. When the ability is created, instead of creating a new ability object to modify, this empty ability object will be modified instead.</para>
        /// <para>The empty ability objects created by this method should not be modified. This method should only be used for referencing ability objects, not modifying them.</para>
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="id">The ID of the ability to reference.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>A "reference" to the ability with the given ID.</returns>
        public static T AbilityReference<T>(string id, ModProfile profile = null) where T : AbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            if(!profile.abilityReferences.TryGetValue(typeof(T), out var abReferencesForType))
                profile.abilityReferences[typeof(T)] = abReferencesForType = [];

            if (!abReferencesForType.TryGetValue(id, out var ab))
            {
                var newAb = CreateScriptable<T>();
                abReferencesForType[id] = newAb;
                return newAb;
            }

            return (T)ab;
        }

        /// <summary>
        /// Get a "reference" to an AdvancedAbilitySO ability, even before that ability is created. This only works on abilities made on the same mod profile.
        /// <para>More specifically, if the ability isn't created when this method is called, this method creates and returns an empty AdvancedAbilitySO object. When the ability is created, instead of creating a new ability object to modify, this empty ability object will be modified instead.</para>
        /// <para>The empty ability objects created by this method should not be modified. This method should only be used for referencing ability objects, not modifying them.</para>
        /// </summary>
        /// <param name="id">The ID of the ability to reference.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>A "reference" to the ability with the given ID.</returns>
        public static AdvancedAbilitySO AdvancedAbilityReference(string id, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return AbilityReference<AdvancedAbilitySO>(id, profile);
        }

        /// <summary>
        /// Creates a new ability using Pentacle's custom AdvancedAbilitySO class.
        /// </summary>
        /// <param name="id_A">The string ID of the ability. Naming convention: AbilityName_A</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created ability.</returns>
        public static AdvancedAbilitySO NewAbility(string id_A, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return NewAbility<AdvancedAbilitySO>(id_A, profile);
        }

        /// <summary>
        /// Creates a new ability of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created ability. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="id_A">The string ID of the ability. Naming convention: AbilityName_A</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created ability.</returns>
        public static T NewAbility<T>(string id_A, ModProfile profile = null) where T : AbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var ab = GetOrCreateNewAbilityReference<T>(id_A, profile);
            ab.name = profile.GetID(id_A);
            ab.effects = [];
            ab.intents = [];

            ab.priority = Priority.Normal;
            ab.abilitySprite = EnemyDB.DefaultAbilitySprite;

            return ab;
        }

        private static T GetOrCreateNewAbilityReference<T>(string id, ModProfile profile) where T : AbilitySO
        {
            if (!profile.abilityReferences.TryGetValue(typeof(T), out var abReferencesForType))
                profile.abilityReferences[typeof(T)] = abReferencesForType = [];

            var exists = abReferencesForType.TryGetValue(id, out var ab);
            if (exists && !string.IsNullOrEmpty(ab.name))
            {
                PentacleLogger.LogWarning($"{profile.Guid}: Replacing an ability reference with the ID {id} because it's not empty. You should not create abilities with duplicate IDs or modify empty ability references.");
                exists = false;
            }

            if (!exists)
            {
                var newAb = CreateScriptable<T>();
                abReferencesForType[id] = newAb;
                return newAb;
            }

            return (T)ab;
        }

        /// <summary>
        /// Sets the in-game display name for the ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="name">The name which will be set for the ability.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetName<T>(this T ab, string name) where T : AbilitySO
        {
            ab._abilityName = name;

            return ab;
        }

        /// <summary>
        /// Sets the in-game description for the ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="description">The description for the ability.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetDescription<T>(this T ab, string description) where T : AbilitySO
        {
            ab._description = description;

            return ab;
        }

        /// <summary>
        /// Sets the in-game sprite that is displayed for the ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetSprite<T>(this T ab, string spriteName, ModProfile profile = null) where T : AbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return ab;

            ab.abilitySprite = profile.LoadSprite(spriteName);

            return ab;
        }

        /// <summary>
        /// Sets the in-game sprite that is displayed for the ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="sprite">The target sprite as a Sprite object.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetSprite<T>(this T ab, Sprite sprite) where T : AbilitySO
        {
            ab.abilitySprite = sprite;

            return ab;
        }

        /// <summary>
        /// General method for setting the name, description and sprite for the ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="name">The in-game name of the ability.</param>
        /// <param name="description">The in-game description of the ability.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetBasicInformation<T>(this T ab, string name, string description, string spriteName = null, ModProfile profile = null) where T : AbilitySO
        {
            ab._abilityName = name;
            ab._description = description;

            if (spriteName != null)
            {
                profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
                if (!ProfileManager.EnsureProfileExists(profile))
                    return ab;

                ab.abilitySprite = profile.LoadSprite(spriteName);
            }

            return ab;
        }

        /// <summary>
        /// General method for setting the name, description and sprite for the ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="name">The in-game name of the ability.</param>
        /// <param name="description">The in-game description of the ability.</param>
        /// <param name="sprite">The ability sprite as a Sprite object.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetBasicInformation<T>(this T ab, string name, string description, Sprite sprite) where T : AbilitySO
        {
            ab._abilityName = name;
            ab._description = description;
            ab.abilitySprite = sprite;

            return ab;
        }

        /// <summary>
        /// Sets the list of effects for the ability, a list of EffectInfo objects. Will overwrite anything already there.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="effects">List of EffectInfo objects that constitutes the ability's effects.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetEffects<T>(this T ab, List<EffectInfo> effects) where T : AbilitySO
        {
            ab.effects = [.. effects];

            return ab;
        }

        /// <summary>
        /// Sets the intents for the ability, will clear any existing intents. 
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="intents">List of IntentTargetInfo objects that will be set as the ability's intents.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetIntents<T>(this T ab, List<IntentTargetInfo> intents) where T : AbilitySO
        {
            ab.intents ??= [];
            ab.intents.Clear();
            ab.intents.AddRange(intents);

            return ab;
        }

        /// <summary>
        /// Adds a new intent to the existing list of intents for this ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="target">The targeting for the intents, as a BaseCombatTargettingSO object.</param>
        /// <param name="intents">The string IDs of the intents.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T AddIntent<T>(this T ab, BaseCombatTargettingSO target, params string[] intents) where T : AbilitySO
        {
            return ab.AddIntent(IntentTools.TargetIntent(target, intents));
        }

        /// <summary>
        /// Adds a new intent to the existing list of intents for this ability.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="intent">The intent to add, as an IntentTargetInfo object.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T AddIntent<T>(this T ab, IntentTargetInfo intent) where T : AbilitySO
        {
            ab.intents ??= [];
            ab.intents.Add(intent);

            return ab;
        }

        /// <summary>
        /// Sets the ability's use animation.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="visuals">An AttackVisualsSO object which defines what animation plays.</param>
        /// <param name="animationTarget">A BaseCombatTargettingSO object which controls where the animation plays.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetVisuals<T>(this T ab, AttackVisualsSO visuals, BaseCombatTargettingSO animationTarget) where T : AbilitySO
        {
            ab.visuals = visuals;
            ab.animationTarget = animationTarget;

            return ab;
        }

        /// <summary>
        /// Sets the visual tooltip of the "Stored value" e.g. mutualism value of the Ability object.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="storedValue">The stored value that will be displayed, as a UnitStoreData_BasicSO object.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetStoredValue<T>(this T ab, UnitStoreData_BasicSO storedValue) where T : AbilitySO
        {
            ab.specialStoredData = storedValue;

            return ab;
        }

        /// <summary>
        /// Sets the priority of the ability that determines the enemy timeline order. Does nothing for player abilities.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="priority">The ability's priority which defines the order in the enemy timeline, as a PrioritySO object.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T SetPriority<T>(this T ab, PrioritySO priority) where T : AbilitySO
        {
            ab.priority = priority;

            return ab;
        }

        /// <summary>
        /// Adds the ability to the Character Ability database.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="addToPool">Determines whether in-game systems, such as the ability "Alchemical Abomination" will be able to access this ability or not.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T AddToCharacterDatabase<T>(this T ab, bool addToPool = true) where T : AbilitySO
        {
            if (addToPool)
                AbilityDB.AddNewCharacterAbility(ab.name, ab);
            else
                AddExternalCharacterAbility(ab.name, ab);

            return ab;
        }

        /// <summary>
        /// Adds the ability to the Enemy Abiliy database
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="addToPool">Determines whether in-game systems will be able to access this ability or not.</param>
        /// <returns>The instance of the ability, for method chaining.</returns>
        public static T AddToEnemyDatabase<T>(this T ab, bool addToPool = true) where T : AbilitySO
        {
            if(addToPool)
                AbilityDB.AddNewEnemyAbility(ab.name, ab);
            else
                AddExternalEnemyAbility(ab.name, ab);

            return ab;
        }

        /// <summary>
        /// Creates a new EnemyAbilityInfo instance that contains the ability and relevant enemy ability information for use with enemies.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The ability's rarity (how rarely enemies will select this ability), as a RaritySO object.</param>
        /// <param name="priority">The priority of the ability to go first in the enemy timeline. Optional, use only if the priority for the ability wasn't already set.</param>
        /// <returns>The created EnemyAbilityInfo object.</returns>
        public static EnemyAbilityInfo EnemyAbility<T>(this T ab, RaritySO rarity, PrioritySO priority = null) where T : AbilitySO
        {
            if (priority != null)
                ab.priority = priority;

            return new()
            {
                ability = ab,
                rarity = rarity,
            };
        }

        /// <summary>
        /// Creates a new CharacterAbility instance that contains the ability and relevant character ability information for use with characters.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="cost">The pigment cost of the ability.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The created CharacterAbility object.</returns>
        public static CharacterAbility CharacterAbility<T>(this T ab, params ManaColorSO[] cost) where T : AbilitySO
        {
            return new()
            {
                ability = ab,
                cost = cost
            };
        }

        /// <summary>
        /// Creates a new ExtraAbilityInfo instance that contains the ability and, in this override, enemy ability information for use in Misc applications.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The ability's rarity (how rarely enemies will select this ability), as a RaritySO object.</param>
        /// <param name="priority">The priority of the ability to go first in the enemy timeline. Optional, use only if the priority for the ability wasn't already set.</param>
        /// <returns>The created ExtraAbilityInfo object.</returns>
        public static ExtraAbilityInfo ExtraAbility<T>(this T ab, RaritySO rarity, PrioritySO priority = null) where T : AbilitySO
        {
            if (priority != null)
                ab.priority = priority;

            return new()
            {
                ability = ab,
                rarity = rarity
            };
        }

        /// <summary>
        /// Creates a new ExtraAbilityInfo instance that contains the ability and, in this override, character ability information for use in Misc applications. 
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="cost">The pigment cost of the ability.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The created ExtraAbilityInfo object.</returns>
        public static ExtraAbilityInfo ExtraAbility<T>(this T ab, params ManaColorSO[] cost) where T : AbilitySO
        {
            return new()
            {
                ability = ab,
                cost = cost
            };
        }

        /// <summary>
        /// Creates a new ExtraAbilityInfo instance that contains the ability and, in this override, the rarity and pigment cost of the ability for use in Misc applications.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The ability's rarity (how rarely enemies will select this ability), as a RaritySO object.</param>
        /// <param name="cost">The pigment cost for characters for this ability.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The created ExtraAbilityInfo object.</returns>
        public static ExtraAbilityInfo ExtraAbility<T>(this T ab, RaritySO rarity, params ManaColorSO[] cost) where T : AbilitySO
        {
            return new()
            {
                ability = ab,
                rarity = rarity,
                cost = cost
            };
        }

        /// <summary>
        /// Creates a new ExtraAbilityInfo instance that contains the ability and, in this override, both enemy and character ability information for use in Misc applications.
        /// </summary>
        /// <typeparam name="T">The ability's custom type. Must either be AbilitySO or a subclass of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The ability's rarity (how rarely enemies will select this ability), as a RaritySO object.</param>
        /// <param name="priority">The priority of the ability to go first in the enemy timeline. Optional, use only if the priority for the ability wasn't already set.</param>
        /// <param name="cost">The pigment cost for characters of this ability.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The created ExtraAbilityInfo object.</returns>
        public static ExtraAbilityInfo ExtraAbility<T>(this T ab, RaritySO rarity, PrioritySO priority, params ManaColorSO[] cost) where T : AbilitySO
        {
            if (priority != null)
                ab.priority = priority;

            return new()
            {
                ability = ab,
                rarity = rarity,
                cost = cost
            };
        }
    }
}
