using Pentacle.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements.UIR;

namespace Pentacle.Internal
{
    internal static class DelayedActions
    {
        public static List<(string bossId, string entityId, UnlockableModData unlock)> delayedFinalBossUnlocks = [];
        public static List<(string category, string achievement)> delayedAchievements = [];
        public static List<(string category, ItemModdedUnlockInfo itemStats)> delayedItemStats = [];
        public static List<(string lootPoolId, LootItemProbability probability)> delayedItemPools = [];

        public static void ProcessPostAwakeActions()
        {
            for(var i = 0; i < delayedFinalBossUnlocks.Count; i++)
            {
                var (bossId, entityId, unlock) = delayedFinalBossUnlocks[i];

                if (!UnlockablesDB.TryGetFinalBossUnlockCheck(bossId, out var check))
                    continue;

                check.AddUnlockData(entityId, unlock);

                delayedFinalBossUnlocks.RemoveAt(i);
                i--;
            }

            for(var i = 0; i < delayedAchievements.Count; i++)
            {
                var (categoryId, achievement) = delayedAchievements[i];
                var achDb = AchievementDB;

                foreach(var category in achDb.ModdedAchievementCategories)
                {
                    if(!category.HasSameID(categoryId))
                        continue;

                    category.achievementNames.Add(achievement);

                    delayedAchievements.RemoveAt(i);
                    i--;
                    break;
                }
            }

            for(var i = 0; i < delayedItemStats.Count; i++)
            {
                var (categoryId, itemStats) = delayedItemStats[i];
                var w = GetWearable(itemStats.itemID);

                if(w == null)
                    continue;

                foreach(var category in ItemUnlocksDB.ModdedCategories)
                {
                    if(!category.HasSameID(categoryId))
                        continue;

                    if (w.startsLocked)
                        category.lockedItemNames.Add(itemStats);
                    else
                        category.unlockedItemNames.Add(itemStats);

                    delayedItemStats.RemoveAt(i);
                    i--;
                    break;
                }
            }

            for(var i = 0; i < delayedItemPools.Count; i++)
            {
                var (lootPoolId, probability) = delayedItemPools[i];
                var w = GetWearable(probability.itemName);

                if (w == null)
                    continue;

                if (!ItemPoolDB.TryGetItemLootListEffect(lootPoolId, out var effect))
                    continue;

                if (w.startsLocked)
                    effect._lockedLootableItems.Add(probability);
                else
                    effect._lootableItems.Add(probability);
                ItemBuilder.CacheItemLootPool(w.name, lootPoolId, probability);

                delayedFinalBossUnlocks.RemoveAt(i);
                i--;
            }
        }
    }
}
