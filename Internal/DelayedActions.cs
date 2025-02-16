using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Internal
{
    internal static class DelayedActions
    {
        public static List<(string bossId, string entityId, UnlockableModData unlock)> delayedFinalBossUnlocks = [];
        public static List<(string category, string achievement)> delayedAchievements = [];

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
        }
    }
}
