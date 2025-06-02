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
        /// Dictionary which maps out an ability's string ID to it's class info.
        /// </summary>
        private static readonly Dictionary<string, AbilitySO> AbilityReferences = [];
        /// <summary>
        /// Retrieve an AbilitySO object that corresponds to the ability ID.
        /// </summary>
        /// <param name="id">String ID of the ability, a.k.a its name.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The target AbilitySO object.</returns>
        public static AbilitySO AbilityReference(string id, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            return AbilityReference(id, profile.Prefix);
        }
        /// <summary>
        /// Retrieve an AbilitySO object that corresponds to the ability ID.
        /// </summary>
        /// <param name="id">String ID of the ability, a.k.a its name.</param>
        /// <param name="prefix">String prefix of your mod.</param>
        /// <returns>The target AbilitySO object.</returns>
        public static AbilitySO AbilityReference(string id, string prefix)
        {
            var key = $"{prefix}_{id}";

            if (!AbilityReferences.TryGetValue(key, out var ab))
                return null;

            return ab;
        }
        /// <summary>
        /// Adds a new ability to the game based on the behaviour defined by the ability class.
        /// </summary>
        /// <param name="id_A">The raw name for the ability (without the prefix).</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An instance of the created ability.</returns>
        public static AdvancedAbilitySO NewAbility(string id_A, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            return NewAbility<AdvancedAbilitySO>(id_A, profile);
        }
        /// <summary>
        /// Adds a new ability to the game based on the behaviour defined by the ability class.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="id_A">The raw name for the ability (without the prefix).</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An instance of the created ability.</returns>
        public static T NewAbility<T>(string id_A, ModProfile profile = null) where T : AbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            var ab = CreateScriptable<T>();

            ab.name = profile.GetID(id_A);
            ab.effects = [];
            ab.intents = [];

            ab.priority = Priority.Normal;
            ab.abilitySprite = EnemyDB.DefaultAbilitySprite;

            AbilityReferences[ab.name] = ab;

            return ab;
        }
        /// <summary>
        /// Sets the in-game display name for the ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The Ability object instance.</param>
        /// <param name="name">The name which will be set for the ability.</param>
        /// <returns>The instance of the ability with its modified name.</returns>
        public static T SetName<T>(this T ab, string name) where T : AbilitySO
        {
            ab._abilityName = name;

            return ab;
        }
        /// <summary>
        /// Sets the in-game description for the ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The Ability object instance.</param>
        /// <param name="description">The description for the ability.</param>
        /// <returns>The instance of the ability with its modified description.</returns>
        public static T SetDescription<T>(this T ab, string description) where T : AbilitySO
        {
            ab._description = description;

            return ab;
        }
        /// <summary>
        /// Sets the in-game sprite that is displayed for the ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="spriteName">The name of the sprite image in the project files.\nYou can also give it a full file path if you replace all "/" with "."</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the ability with its modified sprite.</returns>
        public static T SetSprite<T>(this T ab, string spriteName, ModProfile profile = null) where T : AbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            ab.abilitySprite = profile.LoadSprite(spriteName);

            return ab;
        }
        /// <summary>
        /// Sets the in-game sprite that is displayed for the ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="sprite">The target sprite as a Sprite object.</param>
        /// <returns>The instance of the ability with its modified sprite.</returns>
        public static T SetSprite<T>(this T ab, Sprite sprite) where T : AbilitySO
        {
            ab.abilitySprite = sprite;

            return ab;
        }
        /// <summary>
        /// General method for setting the name, description and sprite for the ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="name">The in-game name of the ability.</param>
        /// <param name="description">The in-game description of the ability.</param>
        /// <param name="spriteName">The name of the sprite image in the project files.\nYou can also give it a full file path if you replace all "/" with "."</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the ability with all the modified information.</returns>
        public static T SetBasicInformation<T>(this T ab, string name, string description, string spriteName = null, ModProfile profile = null) where T : AbilitySO
        {
            ab._abilityName = name;
            ab._description = description;

            if (spriteName != null)
            {
                profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
                ab.abilitySprite = profile.LoadSprite(spriteName);
            }

            return ab;
        }
        /// <summary>
        /// General method for setting the name, description and sprite for the ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="name">The in-game name of the ability.</param>
        /// <param name="description">The in-game description of the ability.</param>
        /// <param name="sprite">The ability sprite as a Sprite object.</param>
        /// <returns>The instance of the ability with all the modified information.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="effects">List of EffectInfo objects that constitutes the ability's effects.</param>
        /// <returns>The instance of the ability with the new list of effects.</returns>
        public static T SetEffects<T>(this T ab, List<EffectInfo> effects) where T : AbilitySO
        {
            ab.effects = [.. effects];

            return ab;
        }
        /// <summary>
        /// Sets the intents for the ability, will clear any existing intents. 
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="intents">List of IntentTargetInfo objects that will be set as the ability's intents.</param>
        /// <returns>The instance of the ability with the new list of intents.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="target">The BaseCombatTargettingSO object that makes up its respective property in IntentTargetInfo.</param>
        /// <param name="intents">(Infinitely Repeatable) The string IDs of the intents.</param>
        /// <returns>The instance of the ability with the modified list of intents.</returns>
        public static T AddIntent<T>(this T ab, BaseCombatTargettingSO target, params string[] intents) where T : AbilitySO
        {
            return ab.AddIntent(IntentTools.TargetIntent(target, intents));
        }
        /// <summary>
        /// Adds a new intent to the existing list of intents for this ability.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="intent">The IntentTargetInfo object that will be added.</param>
        /// <returns>The instance of the ability with the modified list of intents.</returns>
        public static T AddIntent<T>(this T ab, IntentTargetInfo intent) where T : AbilitySO
        {
            ab.intents ??= [];
            ab.intents.Add(intent);

            return ab;
        }
        /// <summary>
        /// Sets the visual effects of the ability on activation.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="visuals">An AttackVisualsSO object which defines what visual effects play.</param>
        /// <param name="animationTarget">A BaseCombatTargettingSO object which controls where the visual effects play.</param>
        /// <returns>The instance of the ability with the new visuals.</returns>
        public static T SetVisuals<T>(this T ab, AttackVisualsSO visuals, BaseCombatTargettingSO animationTarget) where T : AbilitySO
        {
            ab.visuals = visuals;
            ab.animationTarget = animationTarget;

            return ab;
        }
        /// <summary>
        /// Sets the visual tooltip of the "Stored value" e.g. mutualism value of the Ability object.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="storedValue">The object of a UnitStoreData type that defines the stored value to be displayed.</param>
        /// <returns>The instance of the ability with the modified stored value object.</returns>
        public static T SetStoredValue<T>(this T ab, UnitStoreData_BasicSO storedValue) where T : AbilitySO
        {
            ab.specialStoredData = storedValue;

            return ab;
        }
        /// <summary>
        /// Sets the priority of the ability for helping determing the enemy timeline order. Does nothing for player abilities.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="priority">The PrioritySO object which defines the order in the enemy timeline.</param>
        /// <returns>The instance of the ability with the priority set.</returns>
        public static T SetPriority<T>(this T ab, PrioritySO priority) where T : AbilitySO
        {
            ab.priority = priority;

            return ab;
        }
        /// <summary>
        /// Adds the ability to the Character Ability database.
        /// </summary>
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="addToPool">Determines whether in-game systems, such as the ability "Alchemical Abomination" will be able to access this ability or not.</param>
        /// <returns>The instance of the ability with the priority set.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="addToPool">Determines whether in-game systems will be able to access this ability or not.</param>
        /// <returns>The instance of the ability with the priority set.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.></typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">How rarely an enemy with select this ability to use in the timeline.</param>
        /// <param name="priority">The priority of the ability to go first in the enemy timeline.</param>
        /// <returns>An EnemyAbilityInfo object for use in enemies.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="cost">The pigment cost of the ability.</param>
        /// <returns>A CharacterAbility object for use in characters.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The rarity in which this ability occurs.</param>
        /// <param name="priority">The priority the ability has to appear first in the enemy timeline.</param>
        /// <returns>An ExtraAbilityInfo object with the cost field as null.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="cost">The pigment cost of the ability.</param>
        /// <returns>An ExtraAbilityInfo object with the rarity and priority fields as null.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The rarity in which this ability can appear.</param>
        /// <param name="cost">The pigment cost for characters for this ability.</param>
        /// <returns>An ExtraAbilityInfo object with the priority fields as null.</returns>
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
        /// <typeparam name="T">The class which defines the ability behaviour. Must be a type of AbilitySO.</typeparam>
        /// <param name="ab">The object instance of the ability.</param>
        /// <param name="rarity">The rarity in which this ability can appear.</param>
        /// <param name="priority">The priority this ability has to appear first in the enemy timeline.</param>
        /// <param name="cost">The pigment cost for characters of this ability.</param>
        /// <returns>An ExtraAbilityInfo object with all relevant fields assigned to.</returns>
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
