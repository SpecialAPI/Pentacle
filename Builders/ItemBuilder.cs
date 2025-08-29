using Pentacle.Internal;
using Pentacle.TriggerEffects;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Profiling;
using UnityEngine.UIElements.UIR;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating custom items.
    /// </summary>
    public static class ItemBuilder
    {
        private static readonly Dictionary<string, ItemModdedUnlockInfo> itemUnlockInfos = [];

        // I really hate that I have to do this but I'm afraid the other solution is even worse (iterating through all loot pools each time)
        // Dictionary 1 keys: item ids
        // Dictionary 2 keys: custom loot pool ids
        private static readonly Dictionary<string, Dictionary<string, List<LootItemProbability>>> itemCustomLootPoolCache = [];

        /// <summary>
        /// Creates a new item of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created item. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="id">The string ID of the item.<para>Naming convention depends on which loot pool the item is in. Shop pool: ItemName_SW, treasure pool: ItemName_TW, custom pools/no pool: ItemName_ExtraW</para></param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created item.</returns>
        public static T NewItem<T>(string id, ModProfile profile = null) where T : BaseWearableSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var w = CreateScriptable<T>();
            w.name = profile.GetID(id);
            w.staticModifiers = [];

            return w;
        }

        /// <summary>
        /// Sets the in-game name for the item.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="name">The new display name for the item.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetName<T>(this T w, string name) where T : BaseWearableSO
        {
            w._itemName = name;

            return w;
        }

        /// <summary>
        /// Sets the in-game flavor text for the item.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="flavor">The new flavor text for the item.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetFlavor<T>(this T w, string flavor) where T : BaseWearableSO
        {
            w._flavourText = flavor;

            return w;
        }

        /// <summary>
        /// Sets the in-game description for the item.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="description">The new flavor text for the item.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetDescription<T>(this T w, string description) where T : BaseWearableSO
        {
            w._description = description;

            return w;
        }

        /// <summary>
        /// Sets the in-game sprite for the item.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="spriteName">The name of the image file for the item's sprite.<para />.png extension is optional.</param>
        /// <param name="lockedSpriteName">The name of the image file for the item's locked sprite that appears in the Item Stats menu when the item is locked.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetSprite<T>(this T w, string spriteName, string lockedSpriteName = null, ModProfile profile = null) where T : BaseWearableSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return w;

            w.wearableImage = profile.LoadSprite(spriteName);

            if(!string.IsNullOrEmpty(lockedSpriteName))
                w.GetUnlockInfo().lockedSprite = profile.LoadSprite(lockedSpriteName);

            return w;
        }

        /// <summary>
        /// Sets the in-game sprite for the item.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="sprite">The Sprite object of the item's sprite.</param>
        /// <param name="lockedSprite">The Sprite object of the item's locked sprite that appears in the Item Stats menu when the item is locked.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetSprite<T>(this T w, Sprite sprite, Sprite lockedSprite) where T : BaseWearableSO
        {
            w.wearableImage = sprite;

            if(lockedSprite != null)
                w.GetUnlockInfo().lockedSprite = lockedSprite;

            return w;
        }

        /// <summary>
        /// Sets the item's name, flavor text, description and sprite.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="name">The new display name for the item.</param>
        /// <param name="flavor">The new flavor text for the item.</param>
        /// <param name="description">The new flavor text for the item.</param>
        /// <param name="spriteName">The name of the image file for the item's sprite.<para />.png extension is optional.</param>
        /// <param name="lockedSpriteName">The name of the image file for the item's locked sprite that appears in the Item Stats menu when the item is locked.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetBasicInformation<T>(this T w, string name, string flavor, string description, string spriteName, string lockedSpriteName = null, ModProfile profile = null) where T : BaseWearableSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return w;

            w._itemName = name;
            w._flavourText = flavor;
            w._description = description;
            w.wearableImage = profile.LoadSprite(spriteName);

            if (!string.IsNullOrEmpty(lockedSpriteName))
                w.GetUnlockInfo().lockedSprite = profile.LoadSprite(lockedSpriteName);

            return w;
        }

        /// <summary>
        /// Sets the item's name, flavor text, description and sprite.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="name">The new display name for the item.</param>
        /// <param name="flavor">The new flavor text for the item.</param>
        /// <param name="description">The new flavor text for the item.</param>
        /// <param name="sprite">The Sprite object of the item's sprite.</param>
        /// <param name="lockedSprite">The Sprite object of the item's locked sprite that appears in the Item Stats menu when the item is locked.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetBasicInformation<T>(this T w, string name, string flavor, string description, Sprite sprite, Sprite lockedSprite = null) where T : BaseWearableSO
        {
            w._itemName = name;
            w._flavourText = flavor;
            w._description = description;
            w.wearableImage = sprite;

            if (lockedSprite != null)
                w.GetUnlockInfo().lockedSprite = lockedSprite;

            return w;
        }

        /// <summary>
        /// Sets the item's shop price.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="price">The new shop price for the item.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetPrice<T>(this T w, int price) where T : BaseWearableSO
        {
            w.shopPrice = price;

            return w;
        }

        /// <summary>
        /// Sets the item's character static modifiers. This removes any static modifiers the item had before.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="modifiers">The new static modifiers for the item, as WearableStaticModifierSetterSO objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetStaticModifiers<T>(this T w, params WearableStaticModifierSetterSO[] modifiers) where T : BaseWearableSO
        {
            w.staticModifiers = modifiers ?? [];

            return w;
        }

        /// <summary>
        /// Adds a list of item types to this item.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="itemTypes">The string item types to add to the item.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddItemTypes<T>(this T w, params string[] itemTypes) where T : BaseWearableSO
        {
            w._ItemTypeIDs = [..w._ItemTypeIDs ?? [], ..itemTypes];

            return w;
        }

        /// <summary>
        /// Sets the trigger effects of a MultiCustomTriggerEffectWearable item. This will remove any trigger effects the item had before this.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of MultiCustomTriggerEffectWearable.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="triggerEffects">The new trigger effects for the item, as EffectsAndTrigger objects.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetTriggerEffects<T>(this T w, List<EffectsAndTrigger> triggerEffects) where T : MultiCustomTriggerEffectWearable
        {
            w.triggerEffects = triggerEffects;

            return w;
        }

        /// <summary>
        /// Sets the connection effects of a MultiCustomTriggerEffectWearable item. This will remove any connection effects the item had before this.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of MultiCustomTriggerEffectWearable.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="connectionEffects">The new connection effects for the item, as TriggeredEffect objects.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetConnectionEffects<T>(this T w, List<TriggerEffectInfo> connectionEffects) where T : MultiCustomTriggerEffectWearable
        {
            w.connectionEffects = connectionEffects;

            return w;
        }

        /// <summary>
        /// Sets the disconnection effects of a MultiCustomTriggerEffectWearable item. This will remove any disconnection effects the item had before this.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of MultiCustomTriggerEffectWearable.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="disconnectionEffects">The new disconnection effects for the item, as TriggeredEffect objects.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetDisconnectionEffects<T>(this T w, List<TriggerEffectInfo> disconnectionEffects) where T : MultiCustomTriggerEffectWearable
        {
            w.disconnectionEffects = disconnectionEffects;

            return w;
        }

        /// <summary>
        /// Gets the item's linked unlock info.
        /// <para>This unlock info is automatically created by Pentacle. This method can't get unlock infos that were made through other means.</para>
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <returns>The item's linked unlock info, as an ItemModdedUnlockInfo object.</returns>
        public static ItemModdedUnlockInfo GetUnlockInfo<T>(this T w) where T : BaseWearableSO
        {
            if(itemUnlockInfos.TryGetValue(w.name, out var ret))
                return ret;

            return itemUnlockInfos[w.name] = new(w.name);
        }

        /// <summary>
        /// Sets if the item is locked by default.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="startsLocked">If true, the item will be locked by default. Otherwise, the item wlil be unlocked by default.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T SetStartsLocked<T>(this T w, bool startsLocked) where T : BaseWearableSO
        {
            if(w.startsLocked ==  startsLocked)
                return w;

            w.startsLocked = startsLocked;
            var unlockInfo = w.GetUnlockInfo();

            // Move the item between the locked and unlocked itemstats lists
            foreach (var category in ItemUnlocksDB.ModdedCategories)
            {
                var removeFrom = startsLocked ? category.unlockedItemNames : category.lockedItemNames;

                if (!removeFrom.Remove(unlockInfo))
                    continue;

                if (startsLocked)
                    category.lockedItemNames.Add(unlockInfo);
                else
                    category.unlockedItemNames.Add(unlockInfo);
            }

            // Move the item between the locked and unlocked custom loot pool lists
            if(itemCustomLootPoolCache.TryGetValue(w.name, out var customLootPoolDict))
            {
                foreach(var kvp in customLootPoolDict)
                {
                    var lootPoolId = kvp.Key;
                    var probabilities = kvp.Value;

                    if(!ItemPoolDB.TryGetItemLootListEffect(lootPoolId, out var effect))
                        continue;

                    var removeFrom = startsLocked ? effect._lootableItems : effect._lockedLootableItems;

                    if (removeFrom == null || removeFrom.Count <= 0)
                        continue;

                    var addTo = startsLocked ? (effect._lockedLootableItems ??= []) : (effect._lootableItems ??= []);
                    foreach(var prob in probabilities)
                    {
                        if(!removeFrom.Remove(prob))
                            continue;

                        addTo.Add(prob);
                    }
                }
            }

            return w;
        }

        /// <summary>
        /// Adds a list of effects to a MultiCustomTriggerEffectWearable item's list of trigger, connection and disconnection effects.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of MultiCustomTriggerEffectWearable.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="effects">The effects to add to the effect lists, as EffectsAndTrigger objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddEffectsToAll<T>(this T w, params EffectsAndTrigger[] effects) where T : MultiCustomTriggerEffectWearable
        {
            w.triggerEffects.AddRange(effects);
            w.connectionEffects.AddRange(effects);
            w.disconnectionEffects.AddRange(effects);

            return w;
        }

        /// <summary>
        /// Adds the item to the item database and the treasure item pool.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="addToItemStats">If true, the item will also be added to the (modded) Treasure category of the Item Stats menu.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddToTreasure<T>(this T w, bool addToItemStats = true) where T : BaseWearableSO
        {
            w.AddWithoutItemPools();
            ItemPoolDB.AddItemToTreasurePool(w.name);

            if (addToItemStats)
                w.AddToItemStatsCategory("Treasure", true, "Treasure");

            return w;
        }

        /// <summary>
        /// Adds the item to the item database and the shop item pool.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="addToItemStats">If true, the item will also be added to the (modded) Shop category of the Item Stats menu.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddToShop<T>(this T w, bool addToItemStats = true) where T : BaseWearableSO
        {
            w.AddWithoutItemPools();
            ItemPoolDB.AddItemToShopPool(w.name);

            if (addToItemStats)
                w.AddToItemStatsCategory("Shop", true, "Shop");

            return w;
        }

        /// <summary>
        /// Adds the item to the given category of the Item Stats menu.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="categoryId">The string ID of the category that the item will be added to.</param>
        /// <param name="createIfDoesntExist">If categoryId is not a valid ID for any existing category, this will create one with the ID if true.</param>
        /// <param name="createdDisplayName">The display name of the category, only used if creating the category for the first time.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddToItemStatsCategory<T>(this T w, string categoryId, bool createIfDoesntExist, string createdDisplayName) where T : BaseWearableSO
        {
            var unlockInfo = w.GetUnlockInfo();

            foreach(var category in ItemUnlocksDB.ModdedCategories)
            {
                if (!category.HasSameID(categoryId))
                    continue;

                if (w.startsLocked)
                    category.lockedItemNames.Add(unlockInfo);
                else
                    category.unlockedItemNames.Add(unlockInfo);

                return w;
            }

            if (!createIfDoesntExist)
            {
                DelayedActions.delayedItemStats.Add((categoryId, unlockInfo));
                return w;
            }

            var newCategory = new ModdedItemCategory(categoryId, createdDisplayName);

            if (w.startsLocked)
                newCategory.lockedItemNames.Add(unlockInfo);
            else
                newCategory.unlockedItemNames.Add(unlockInfo);

            ItemUnlocksDB.ModdedCategories.Add(newCategory);
            return w;
        }

        /// <summary>
        /// Adds the item to the item database and a custom loot pool (such as the Fishing Rod or Can of Worms pool).
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="poolId">The ID of the loot pool the item will be added to.</param>
        /// <param name="weight">The item's weight in the loot pool, how common the item should be.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddToCustomItemPool<T>(this T w, string poolId, int weight) where T : BaseWearableSO
        {
            w.AddWithoutItemPools();

            var lootProbability = new LootItemProbability(w.name, weight);

            if (!ItemPoolDB.TryGetItemLootListEffect(poolId, out var effect))
            {
                DelayedActions.delayedItemPools.Add((poolId, lootProbability));
                return w;
            }

            if (w.startsLocked)
                (effect._lockedLootableItems ??= []).Add(lootProbability);
            else
                (effect._lootableItems ??= []).Add(lootProbability);

            // Cache the probability to move it between the locked and unlocked lists if the item gets locked/unlocked after getting added to the pool
            CacheItemLootPool(w.name, poolId, lootProbability);

            return w;
        }

        internal static void CacheItemLootPool(string itemId, string poolId, LootItemProbability probability)
        {
            if (!itemCustomLootPoolCache.TryGetValue(itemId, out var customLootPoolDict))
                itemCustomLootPoolCache[itemId] = customLootPoolDict = [];
            if (!customLootPoolDict.TryGetValue(poolId, out var lootProbabilitiesForPool))
                customLootPoolDict[poolId] = lootProbabilitiesForPool = [];
            lootProbabilitiesForPool.Add(probability);
        }

        /// <summary>
        /// Adds the item to the item database and the fish pool.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <param name="weight">The item's weight in the loot pool, how common the item should be.</param>
        /// <param name="fishingRod">If true, the item will be added to the Fishing Rod's fish pool.</param>
        /// <param name="canOfWorms_WelsCatfish">If true, the item will be added to the Can of Worms' and Wels Catfish's shared fish pool.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddToFishPool<T>(this T w, int weight, bool fishingRod = true, bool canOfWorms_WelsCatfish = true) where T : BaseWearableSO
        {
            if (fishingRod)
                w.AddToCustomItemPool(PoolList_GameIDs.FishingRod.ToString(), weight);
            if (canOfWorms_WelsCatfish)
                w.AddToCustomItemPool(PoolList_GameIDs.CanOfWorms_WelsCatfish.ToString(), weight);

            return w;
        }

        /// <summary>
        /// Adds the item to the item database without adding it to any loot pools.
        /// </summary>
        /// <typeparam name="T">The item's custom type. Must be a subclass of BaseWearableSO.</typeparam>
        /// <param name="w">The object instance of the item.</param>
        /// <returns>The instance of the item for method chaining.</returns>
        public static T AddWithoutItemPools<T>(this T w) where T : BaseWearableSO
        {
            // Check if the item is already added to avoid causing errors
            if (!LoadedWearables.ContainsKey(w.name))
                ItemUtils.JustAddItemSoItCanBeLoaded(w);

            return w;
        }

        /// <summary>
        /// Creates a character static modifier that adds an additional ability to the character.
        /// </summary>
        /// <param name="ab">The ability that the modifier should add, as a CharacterAbility object.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static ExtraAbility_Wearable_SMS ExtraAbilityModifier(CharacterAbility ab)
        {
            var mod = CreateScriptable<ExtraAbility_Wearable_SMS>();
            mod._extraAbility = ab;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that replaces the character's slap (or slap equivalent) with a different ability.
        /// </summary>
        /// <param name="ab">The ability that the modifier should replace slap with, as a CharacterAbility object.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static BasicAbilityChange_Wearable_SMS BasicAbilityModifier(CharacterAbility ab)
        {
            var mod = CreateScriptable<BasicAbilityChange_Wearable_SMS>();
            mod._basicAbility = ab;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that adds modded data to the character.
        /// </summary>
        /// <param name="data">The modded data that the modifier should add, as a ItemModifierDataSetter object.</param>
        /// <param name="id">The string ID of the modded data. If null, defaults to the name of the name of the data's type if null.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static ModdedDataSetter_Wearable_SMS ModdedDataModifier(ItemModifierDataSetter data, string id = null)
        {
            var mod = CreateScriptable<ModdedDataSetter_Wearable_SMS>();
            mod.m_ModdedData = data;
            mod.m_ModdedDataID = id ?? data.GetType().Name;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that adds modded data to the character.
        /// </summary>
        /// <typeparam name="T">The modded data's custom type. Must be a subclass of ItemModifierDataSetter.</typeparam>
        /// <param name="id">The string ID of the modded data. If null, defaults to the name of the name of the data's type if null</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static ModdedDataSetter_Wearable_SMS ModdedDataModifier<T>(string id = null) where T : ItemModifierDataSetter, new()
        {
            var mod = CreateScriptable<ModdedDataSetter_Wearable_SMS>();
            mod.m_ModdedData = new T();
            mod.m_ModdedDataID = id ?? typeof(T).Name;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that changes the character's level.
        /// </summary>
        /// <param name="rankAddition">How much the modifier should increase the level. Negative values will cause the modifier to decrease the level instead.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static RankChange_Wearable_SMS RankChangeModifier(int rankAddition)
        {
            var mod = CreateScriptable<RankChange_Wearable_SMS>();
            mod._rankAdditive = rankAddition;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that marks the character as the "main character", causing them to revive at 1 health after battle if they died (like Nowak).
        /// </summary>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static MainCharacter_Wearable_SMS MainCharacterModifier()
        {
            return CreateScriptable<MainCharacter_Wearable_SMS>();
        }

        /// <summary>
        /// Creates a character static modifier that changes the character's maximum health.
        /// </summary>
        /// <param name="maxHealthChange">How much the modifier should increase the character's maximum health. Negative values will cause the modifier to decrease the maximum health instead.</param>
        /// <param name="changeIsPercentage">If true, maxHealthChange will be treated as a percentage of the character's original health instead of a flat addition.<para>For example, if maxHealthChange is 10, the character's maximum health would be increased by 10%.</para></param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static MaxHealthChange_Wearable_SMS MaxHealthModifier(int maxHealthChange, bool changeIsPercentage = false)
        {
            var mod = CreateScriptable<MaxHealthChange_Wearable_SMS>();
            mod.maxHealthChange = maxHealthChange;
            mod.isChangePercentage = changeIsPercentage;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that changes the character's health color.
        /// </summary>
        /// <param name="healthColor">The pigment color that the modifier should change the character's health color to, as a ManaColorSO object.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static HealthColorChange_Wearable_SMS HealthColorModifier(ManaColorSO healthColor)
        {
            var mod = CreateScriptable<HealthColorChange_Wearable_SMS>();
            mod._healthColor = healthColor;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that adds an additional passive to the character.
        /// </summary>
        /// <param name="passive">The passive that should be added by the modifier, as a BasePassiveAbilitySO object.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static ExtraPassiveAbility_Wearable_SMS ExtraPassiveModifier(BasePassiveAbilitySO passive)
        {
            var mod = CreateScriptable<ExtraPassiveAbility_Wearable_SMS>();
            mod._extraPassiveAbility = passive;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that changes which abilities the character uses.
        /// </summary>
        /// <param name="usesBasicAbility">Determines whether characters with the modifier should use slap (or their slap equivalent) or not.</param>
        /// <param name="usesAllAbilities">Determines whether characters with the modifier should use all abilities or if they should use sets.</param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static AbilitiesUsageChange_Wearable_SMS UsedAbilitiesChangeModifier(bool usesBasicAbility, bool usesAllAbilities)
        {
            var mod = CreateScriptable<AbilitiesUsageChange_Wearable_SMS>();
            mod._usesBasicAbility = usesBasicAbility;
            mod._usesAllAbilities = usesAllAbilities;

            return mod;
        }

        /// <summary>
        /// Creates a character static modifier that multiplies the amount of currency gained after combat.
        /// </summary>
        /// <param name="currencyMultAddition">How much currency multiplier the modifier should add. Negative values will lower the amount of currency gained instead.<para>For example, setting this to 1 will lead to a 100% currency increase (2x coins).</para></param>
        /// <returns>An object instance of the created character static modifier.</returns>
        public static CurrencyMultiplierChange_Wearable_SMS CurrencyMultiplierModifier(int currencyMultAddition)
        {
            var mod = CreateScriptable<CurrencyMultiplierChange_Wearable_SMS>();
            mod._currencyMultiplier = currencyMultAddition;

            return mod;
        }
    }
}
