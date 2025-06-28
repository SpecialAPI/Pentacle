using Pentacle.Internal;
using Pentacle.TriggerEffect;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Profiling;
using UnityEngine.UIElements.UIR;

namespace Pentacle.Builders
{
    public static class ItemBuilder
    {
        private static readonly Dictionary<string, ItemModdedUnlockInfo> itemUnlockInfos = [];

        // I really hate that I have to do this but I'm afraid the other solution is even worse (iterating through all loot pools each time)
        // Dictionary 1 keys: item ids
        // Dictionary 2 keys: custom loot pool ids
        private static readonly Dictionary<string, Dictionary<string, List<LootItemProbability>>> itemCustomLootPoolCache = [];

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

        public static T SetName<T>(this T w, string name) where T : BaseWearableSO
        {
            w._itemName = name;

            return w;
        }

        public static T SetFlavor<T>(this T w, string flavor) where T : BaseWearableSO
        {
            w._flavourText = flavor;

            return w;
        }

        public static T SetDescription<T>(this T w, string description) where T : BaseWearableSO
        {
            w._description = description;

            return w;
        }

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

        public static T SetSprite<T>(this T w, Sprite sprite, Sprite lockedSprite) where T : BaseWearableSO
        {
            w.wearableImage = sprite;

            if(lockedSprite != null)
                w.GetUnlockInfo().lockedSprite = lockedSprite;

            return w;
        }

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

        public static T SetPrice<T>(this T w, int price) where T : BaseWearableSO
        {
            w.shopPrice = price;

            return w;
        }

        public static T SetStaticModifiers<T>(this T w, params WearableStaticModifierSetterSO[] modifiers) where T : BaseWearableSO
        {
            w.staticModifiers = modifiers ?? [];

            return w;
        }

        public static T SetTriggerEffects<T>(this T w, List<EffectsAndTrigger> triggerEffects) where T : MultiCustomTriggerEffectWearable
        {
            w.triggerEffects = triggerEffects;

            return w;
        }

        public static T SetConnectionEffects<T>(this T w, List<TriggeredEffect> connectionEffects) where T : MultiCustomTriggerEffectWearable
        {
            w.connectionEffects = connectionEffects;

            return w;
        }

        public static T SetDisconnectionEffects<T>(this T w, List<TriggeredEffect> disconnectionEffects) where T : MultiCustomTriggerEffectWearable
        {
            w.disconnectionEffects = disconnectionEffects;

            return w;
        }

        public static ItemModdedUnlockInfo GetUnlockInfo<T>(this T w) where T : BaseWearableSO
        {
            if(itemUnlockInfos.TryGetValue(w.name, out var ret))
                return ret;

            return itemUnlockInfos[w.name] = new(w.name);
        }

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

        public static T AddEffectsToAll<T>(this T w, params EffectsAndTrigger[] effects) where T : MultiCustomTriggerEffectWearable
        {
            w.triggerEffects.AddRange(effects);
            w.connectionEffects.AddRange(effects);
            w.disconnectionEffects.AddRange(effects);

            return w;
        }

        public static T AddToTreasure<T>(this T w, bool addToItemStats = true) where T : BaseWearableSO
        {
            w.AddWithoutItemPools();
            ItemPoolDB.AddItemToTreasurePool(w.name);

            if (addToItemStats)
                w.AddToItemStatsCategory("Treasure", true, "Treasure");

            return w;
        }

        public static T AddToShop<T>(this T w, bool addToItemStats = true) where T : BaseWearableSO
        {
            w.AddWithoutItemPools();
            ItemPoolDB.AddItemToShopPool(w.name);

            if (addToItemStats)
                w.AddToItemStatsCategory("Shop", true, "Shop");

            return w;
        }

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

        public static T AddToFishPool<T>(this T w, int weight, bool fishingRod = true, bool canOfWorms_WelsCatfish = true) where T : BaseWearableSO
        {
            if (fishingRod)
                w.AddToCustomItemPool(PoolList_GameIDs.FishingRod.ToString(), weight);
            if (canOfWorms_WelsCatfish)
                w.AddToCustomItemPool(PoolList_GameIDs.CanOfWorms_WelsCatfish.ToString(), weight);

            return w;
        }

        public static T AddWithoutItemPools<T>(this T w) where T : BaseWearableSO
        {
            // Check if the item is already added to avoid causing errors
            if (!LoadedWearables.ContainsKey(w.name))
                ItemUtils.JustAddItemSoItCanBeLoaded(w);

            return w;
        }

        public static ExtraAbility_Wearable_SMS ExtraAbilityModifier(CharacterAbility ab)
        {
            var mod = CreateScriptable<ExtraAbility_Wearable_SMS>();
            mod._extraAbility = ab;

            return mod;
        }

        public static BasicAbilityChange_Wearable_SMS BasicAbilityModifier(CharacterAbility ab)
        {
            var mod = CreateScriptable<BasicAbilityChange_Wearable_SMS>();
            mod._basicAbility = ab;

            return mod;
        }

        public static ModdedDataSetter_Wearable_SMS ModdedDataModifier(ItemModifierDataSetter data, string id = null)
        {
            var mod = CreateScriptable<ModdedDataSetter_Wearable_SMS>();
            mod.m_ModdedData = data;
            mod.m_ModdedDataID = id ?? data.GetType().Name;

            return mod;
        }

        public static ModdedDataSetter_Wearable_SMS ModdedDataModifier<T>(string id = null) where T : ItemModifierDataSetter, new()
        {
            var mod = CreateScriptable<ModdedDataSetter_Wearable_SMS>();
            mod.m_ModdedData = new T();
            mod.m_ModdedDataID = id ?? typeof(T).Name;

            return mod;
        }

        public static RankChange_Wearable_SMS RankChangeModifier(int rankAddition)
        {
            var mod = CreateScriptable<RankChange_Wearable_SMS>();
            mod._rankAdditive = rankAddition;

            return mod;
        }

        public static MainCharacter_Wearable_SMS MainCharacterModifier()
        {
            return CreateScriptable<MainCharacter_Wearable_SMS>();
        }

        public static MaxHealthChange_Wearable_SMS MaxHealthModifier(int maxHealthChange, bool changeIsPercentage = false)
        {
            var mod = CreateScriptable<MaxHealthChange_Wearable_SMS>();
            mod.maxHealthChange = maxHealthChange;
            mod.isChangePercentage = changeIsPercentage;

            return mod;
        }

        public static HealthColorChange_Wearable_SMS HealthColorModifier(ManaColorSO healthColor)
        {
            var mod = CreateScriptable<HealthColorChange_Wearable_SMS>();
            mod._healthColor = healthColor;

            return mod;
        }

        public static ExtraPassiveAbility_Wearable_SMS ExtraPassiveModifier(BasePassiveAbilitySO passive)
        {
            var mod = CreateScriptable<ExtraPassiveAbility_Wearable_SMS>();
            mod._extraPassiveAbility = passive;

            return mod;
        }

        public static AbilitiesUsageChange_Wearable_SMS UsedAbilitiesChangeModifier(bool usesBasicAbility, bool usesAllAbilities)
        {
            var mod = CreateScriptable<AbilitiesUsageChange_Wearable_SMS>();
            mod._usesBasicAbility = usesBasicAbility;
            mod._usesAllAbilities = usesAllAbilities;

            return mod;
        }

        public static CurrencyMultiplierChange_Wearable_SMS CurrencyMultiplierModifier(int currencyMultAddition)
        {
            var mod = CreateScriptable<CurrencyMultiplierChange_Wearable_SMS>();
            mod._currencyMultiplier = currencyMultAddition;

            return mod;
        }
    }
}
