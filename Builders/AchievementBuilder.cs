using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class AchievementBuilder
    {
        public static ModdedAchievement_t NewAchievement(string ACH_achievementId, string name, string description, Assembly callingAssembly = null)
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if(!ProfileManager.TryGetProfile(callingAssembly, out var profile))
            {
                return null;
            }

            var ach = new ModdedAchievement_t(profile.GetID(ACH_achievementId), name, description);
            return ach;
        }

        public static T SetSprites<T>(this T ach, string unlockedSpriteName, string overrideLockedSpriteName = null, Assembly callingAssembly = null) where T : ModdedAchievement_t
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            ach.m_unlockedSprite = ResourceLoader.LoadSprite(unlockedSpriteName, assembly: callingAssembly);
            if(!string.IsNullOrEmpty(overrideLockedSpriteName))
                ach.m_specialLockedSprite = ResourceLoader.LoadSprite(overrideLockedSpriteName, assembly: callingAssembly);

            return ach;
        }

        public static T SetSprites<T>(this T ach, Sprite unlockedSprite, Sprite overrideLockedSprite = null) where T : ModdedAchievement_t
        {
            ach.m_unlockedSprite = unlockedSprite;
            if (overrideLockedSprite != null)
                ach.m_specialLockedSprite = overrideLockedSprite;

            return ach;
        }

        public static T AddToBaseCategory<T>(this T ach, AchievementCategoryIDs category) where T : ModdedAchievement_t
        {
            AchievementDB.AddNewAchievement(ach, category.ToString());

            return ach;
        }

        public static T AddToCustomCategory<T>(this T ach, string categoryId, bool createIfDoesntExist = false, string categoryName = "") where T : ModdedAchievement_t
        {
            var achDb = AchievementDB;

            if (!achDb._steamAchievements.TryAddModdedAchievement(ach))
            {
                Debug.LogError($"Achievement with id {ach.m_eAchievementID} already exists.");
                return ach;
            }

            foreach(var category in achDb.ModdedAchievementCategories)
            {
                if (!category.HasSameID(categoryId))
                    continue;

                category.achievementNames.Add(ach.m_eAchievementID);
                return ach;
            }

            DelayedActions.delayedAchievements.Add((categoryId, ach.m_eAchievementID));
            return ach;
        }
    }
}
