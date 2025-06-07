using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class UnlockBuilder
    {
        public static UnlockableModData NewUnlock(string id, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return new UnlockableModData(profile.GetID(id));
        }

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

        public static T SetQuest<T>(this T u, string questId) where T : UnlockableModData
        {
            u.hasQuestCompletion = true;
            u.questID = questId;

            return u;
        }

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

        public static T SetCustomUnlockPopup<T>(this T u, ModUnlockInfo customPopup) where T : UnlockableModData
        {
            u.modUnlockInfo = customPopup;

            return u;
        }
    }
}
