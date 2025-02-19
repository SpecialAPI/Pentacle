using Pentacle.Advanced;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class AbilityBuilder
    {
        private static readonly Dictionary<string, AbilitySO> AbilityReferences = [];

        public static AbilitySO AbilityReference(string id, Assembly callingAssembly = null)
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if(!ProfileManager.TryGetProfile(callingAssembly, out var profile))
                return null;

            return AbilityReference(id, profile.Prefix);
        }

        public static AbilitySO AbilityReference(string id, string prefix)
        {
            var key = $"{prefix}_{id}";

            if (!AbilityReferences.TryGetValue(key, out var ab))
                return null;

            return ab;
        }

        public static AdvancedAbilitySO NewAbility(string id_A, Assembly callingAssembly = null)
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            return NewAbility<AdvancedAbilitySO>(id_A, callingAssembly);
        }

        public static T NewAbility<T>(string id_A, Assembly callingAssembly = null) where T : AbilitySO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if (!ProfileManager.TryGetProfile(callingAssembly, out var profile))
                return null;

            var ab = CreateScriptable<T>();

            ab.name = profile.GetID(id_A);
            ab.effects = [];
            ab.intents = [];

            ab.priority = Priority.Normal;
            ab.abilitySprite = EnemyDB.DefaultAbilitySprite;

            AbilityReferences[ab.name] = ab;

            return ab;
        }

        public static T SetName<T>(this T ab, string name) where T : AbilitySO
        {
            ab._abilityName = name;

            return ab;
        }

        public static T SetDescription<T>(this T ab, string description) where T : AbilitySO
        {
            ab._description = description;

            return ab;
        }

        public static T SetSprite<T>(this T ab, string spriteName, Assembly callingAssembly = null) where T : AbilitySO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();
            ab.abilitySprite = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return ab;
        }

        public static T SetSprite<T>(this T ab, Sprite sprite) where T : AbilitySO
        {
            ab.abilitySprite = sprite;

            return ab;
        }

        public static T SetBasicInformation<T>(this T ab, string name, string description, string spriteName = null, Assembly callingAssembly = null) where T : AbilitySO
        {
            ab._abilityName = name;
            ab._description = description;

            if (spriteName != null)
            {
                callingAssembly ??= Assembly.GetCallingAssembly();
                ab.abilitySprite = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);
            }

            return ab;
        }

        public static T SetBasicInformation<T>(this T ab, string name, string description, Sprite sprite) where T : AbilitySO
        {
            ab._abilityName = name;
            ab._description = description;
            ab.abilitySprite = sprite;

            return ab;
        }

        public static T SetEffects<T>(this T ab, List<EffectInfo> effects) where T : AbilitySO
        {
            ab.effects = [.. effects];

            return ab;
        }

        public static T SetIntents<T>(this T ab, List<IntentTargetInfo> intents) where T : AbilitySO
        {
            ab.intents ??= [];
            ab.intents.Clear();
            ab.intents.AddRange(intents);

            return ab;
        }

        public static T AddIntent<T>(this T ab, BaseCombatTargettingSO target, params string[] intents) where T : AbilitySO
        {
            return ab.AddIntent(IntentTools.TargetIntent(target, intents));
        }

        public static T AddIntent<T>(this T ab, IntentTargetInfo intent) where T : AbilitySO
        {
            ab.intents ??= [];
            ab.intents.Add(intent);

            return ab;
        }

        public static T SetVisuals<T>(this T ab, AttackVisualsSO visuals, BaseCombatTargettingSO animationTarget) where T : AbilitySO
        {
            ab.visuals = visuals;
            ab.animationTarget = animationTarget;

            return ab;
        }

        public static T SetStoredValue<T>(this T ab, UnitStoreData_BasicSO storedValue) where T : AbilitySO
        {
            ab.specialStoredData = storedValue;

            return ab;
        }

        public static T SetPriority<T>(this T ab, PrioritySO priority) where T : AbilitySO
        {
            ab.priority = priority;

            return ab;
        }

        public static T AddToCharacterDatabase<T>(this T ab, bool addToPool = true) where T : AbilitySO
        {
            if (addToPool)
                AbilityDB.AddNewCharacterAbility(ab.name, ab);
            else
                AddExternalCharacterAbility(ab.name, ab);

            return ab;
        }

        public static T AddToEnemyDatabase<T>(this T ab, bool addToPool = true) where T : AbilitySO
        {
            if(addToPool)
                AbilityDB.AddNewEnemyAbility(ab.name, ab);
            else
                AddExternalEnemyAbility(ab.name, ab);

            return ab;
        }

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

        public static CharacterAbility CharacterAbility<T>(this T ab, params ManaColorSO[] cost) where T : AbilitySO
        {
            return new()
            {
                ability = ab,
                cost = cost
            };
        }

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

        public static ExtraAbilityInfo ExtraAbility<T>(this T ab, params ManaColorSO[] cost) where T : AbilitySO
        {
            return new()
            {
                ability = ab,
                cost = cost
            };
        }

        public static ExtraAbilityInfo ExtraAbility<T>(this T ab, RaritySO rarity, params ManaColorSO[] cost) where T : AbilitySO
        {
            return new()
            {
                ability = ab,
                rarity = rarity,
                cost = cost
            };
        }

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
