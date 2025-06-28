using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating in-game achievements.
    /// </summary>
    public static class AchievementBuilder
    {
        /// <summary>
        /// Creates a new modded in-game achievement.
        /// </summary>
        /// <param name="ACH_achievementId">The string ID for the achievement. Naming convention: ACH_AchievementName</param>
        /// <param name="name">The display name of the achievement.</param>
        /// <param name="description">The display description of the achievement.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created achievement.</returns>
        public static ModdedAchievement_t NewAchievement(string ACH_achievementId, string name, string description, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            var ach = new ModdedAchievement_t(profile.GetID(ACH_achievementId), name, description);
            return ach;
        }

        /// <summary>
        /// Sets the unlocked and locked sprites for the achievement icon.
        /// </summary>
        /// <typeparam name="T">The achievement's custom type. Must either be ModdedAchievement_t or a subclass of ModdedAchievement_t.</typeparam>
        /// <param name="ach">The object instance of the achievement.</param>
        /// <param name="unlockedSpriteName">The name of the image file for the unlocked achievement sprite.<para />.png extension is optional.</param>
        /// <param name="overrideLockedSpriteName">The name of the image file for the locked achievement sprite. If null or empty, the achievement will use the game's default locked achievement sprite.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the achievement, for method chaining.</returns>
        public static T SetSprites<T>(this T ach, string unlockedSpriteName, string overrideLockedSpriteName = null, ModProfile profile = null) where T : ModdedAchievement_t
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            ach.m_unlockedSprite = profile.LoadSprite(unlockedSpriteName);
            if(!string.IsNullOrEmpty(overrideLockedSpriteName))
                ach.m_specialLockedSprite = profile.LoadSprite(overrideLockedSpriteName);

            return ach;
        }

        /// <summary>
        /// Sets the unlocked and locked sprites for the achievement icon.
        /// </summary>
        /// <typeparam name="T">The achievement's custom type. Must either be ModdedAchievement_t or a subclass of ModdedAchievement_t.</typeparam>
        /// <param name="ach">The object instance of the achievement.</param>
        /// <param name="unlockedSprite">The Sprite object that contains the unlocked achievement sprite.</param>
        /// <param name="overrideLockedSprite">The Sprite object that contains the optional locked achievement sprite. If null, the achievement will use the game's default locked achievement sprite.</param>
        /// <returns>The instance of the achievement, for method chaining.</returns>
        public static T SetSprites<T>(this T ach, Sprite unlockedSprite, Sprite overrideLockedSprite = null) where T : ModdedAchievement_t
        {
            ach.m_unlockedSprite = unlockedSprite;
            if (overrideLockedSprite != null)
                ach.m_specialLockedSprite = overrideLockedSprite;

            return ach;
        }

        /// <summary>
        /// Adds the achievement to a base-game category in the achievements menu.
        /// </summary>
        /// <typeparam name="T">The achievement's custom type. Must either be ModdedAchievement_t or a subclass of ModdedAchievement_t.</typeparam>
        /// <param name="ach">The object instance of the achievement.</param>
        /// <param name="category">The base-game category which the achievement will be added to.</param>
        /// <returns>The instance of the achievement, for method chaining.</returns>
        public static T AddToBaseCategory<T>(this T ach, AchievementCategoryIDs category) where T : ModdedAchievement_t
        {
            AchievementDB.AddNewAchievement(ach, category.ToString());

            return ach;
        }

        /// <summary>
        /// Adds the achievement to a custom category in the achievements menu. Optional to create the category if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The achievement's custom type. Must either be ModdedAchievement_t or a subclass of ModdedAchievement_t.</typeparam>
        /// <param name="ach">The object instance of the achievement.</param>
        /// <param name="categoryId">The string ID of the custom category.</param>
        /// <param name="createIfDoesntExist">If categoryId is not a valid ID for any existing modded category, this will create one with the ID if true.</param>
        /// <param name="categoryName">The display name of the category, only used if creating the category for the first time.</param>
        /// <returns>The instance of the achievement, for method chaining.</returns>
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
