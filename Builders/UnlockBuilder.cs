using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class that provides tools for creating custom unlocks.
    /// </summary>
    public static class UnlockBuilder
    {
        /// <summary>
        /// Creates a new unlock.
        /// </summary>
        /// <param name="id">The string ID of the unlock.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created unlock.</returns>
        public static UnlockableModData NewUnlock(string id, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return new UnlockableModData(profile.GetID(id));
        }

        /// <summary>
        /// Sets the custom achievement associated with the unlock.
        /// </summary>
        /// <typeparam name="T">The unlock's custom type. Must either be UnlockableModData or a subclass of UnlockableModData.</typeparam>
        /// <param name="u">The object instance of the unlock.</param>
        /// <param name="achievement">The custom achievement that will be associated with the unlock, as a ModdedAchievement_t object.</param>
        /// <returns>The instance of the unlock, for method chaining.</returns>
        public static T SetAchievement<T>(this T u, ModdedAchievement_t achievement) where T : UnlockableModData
        {
            u.hasModdedAchievementUnlock = true;
            u.moddedAchievementID = achievement.m_eAchievementID;

            if (u.hasItemUnlock)
            {
                foreach (var id in u.items)
                {
                    var item = GetWearable(id);

                    if (item == null)
                        continue;

                    item.GetUnlockInfo().achievementID = achievement.m_eAchievementID;
                }
            }

            return u;
        }

        /// <summary>
        /// Sets the items associated with the unlock.
        /// </summary>
        /// <typeparam name="T">The unlock's custom type. Must either be UnlockableModData or a subclass of UnlockableModData.</typeparam>
        /// <param name="u">The object instance of the unlock.</param>
        /// <param name="items">An array containing the IDs of the items that will be associated with the unlock.</param>
        /// <param name="automaticallyLock">If true, the given items will be set as locked by default.</param>
        /// <returns>The instance of the unlock, for method chaining.</returns>
        public static T SetItems<T>(this T u, string[] items, bool automaticallyLock = true) where T : UnlockableModData
        {
            u.hasItemUnlock = true;
            u.items = items;

            if (u.hasModdedAchievementUnlock)
            {
                foreach (var id in items)
                {
                    var item = GetWearable(id);

                    if (item == null)
                        continue;

                    item.GetUnlockInfo().achievementID = u.moddedAchievementID;
                }
            }

            if (automaticallyLock)
            {
                foreach (var id in items)
                {
                    var item = GetWearable(id);

                    if (item == null)
                        continue;

                    item.SetStartsLocked(true);
                }
            }

            return u;
        }

        /// <summary>
        /// Sets the quest associated with the unlock.
        /// </summary>
        /// <typeparam name="T">The unlock's custom type. Must either be UnlockableModData or a subclass of UnlockableModData.</typeparam>
        /// <param name="u">The object instance of the unlock.</param>
        /// <param name="questId">The ID of the quest that will be associated with the unlock.</param>
        /// <returns>The instance of the unlock, for method chaining.</returns>
        public static T SetQuest<T>(this T u, string questId) where T : UnlockableModData
        {
            u.hasQuestCompletion = true;
            u.questID = questId;

            return u;
        }

        /// <summary>
        /// Sets the character associated with the unlock.
        /// </summary>
        /// <typeparam name="T">The unlock's custom type. Must either be UnlockableModData or a subclass of UnlockableModData.</typeparam>
        /// <param name="u">The object instance of the unlock.</param>
        /// <param name="characterId">The ID of the character that will be associated with the unlock.</param>
        /// <param name="automaticallyLock">If true, the given character will be set as locked by default.</param>
        /// <returns>The instance of the unlock, for method chaining.</returns>
        public static T SetCharacter<T>(this T u, string characterId, bool automaticallyLock = true) where T : UnlockableModData
        {
            u.hasCharacterUnlock = true;
            u.character = characterId;

            if (automaticallyLock)
            {
                var ch = GetCharacter(characterId);

                if (ch != null)
                    ch.m_StartsLocked = true;
            }

            return u;
        }

        /// <summary>
        /// Sets custom popup associated with the unlock.
        /// </summary>
        /// <typeparam name="T">The unlock's custom type. Must either be UnlockableModData or a subclass of UnlockableModData.</typeparam>
        /// <param name="u">The object instance of the unlock.</param>
        /// <param name="customPopup">The custom unlock popup that will be associated with the unlock, as a ModUnlockInfo object.</param>
        /// <returns>The instance of the unlock, for method chaining.</returns>
        public static T SetCustomUnlockPopup<T>(this T u, ModUnlockInfo customPopup) where T : UnlockableModData
        {
            u.modUnlockInfo = customPopup;

            return u;
        }
    }
}
