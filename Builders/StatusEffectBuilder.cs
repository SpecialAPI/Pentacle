using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Profiling;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class that provides tools for creating custom status effects.
    /// </summary>
    public static class StatusEffectBuilder
    {
        /// <summary>
        /// Creates a new status effect of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created status effect. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="id_SE">The string database ID of the status effect. Naming convention: StatusEffectName_SE</param>
        /// <param name="statusId_ID">The string status ID of the status effect used to check if two status effects are the same. Naming convention: StatusEffectName_ID</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created status effect.</returns>
        public static T NewStatusEffect<T>(string id_SE, string statusId_ID, ModProfile profile = null) where T : StatusEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var se = CreateScriptable<T>();
            se.name = profile.GetID(id_SE);
            se._StatusID = profile.GetID(statusId_ID);

            var seInfo = CreateScriptable<StatusEffectInfoSO>();
            seInfo.name = $"{se.name}Info";
            seInfo._applied_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._updated_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._removed_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Remove";

            se._EffectInfo = seInfo;

            return se;
        }

        /// <summary>
        /// Sets the in-game name for the status effect.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="name">The new display name for the status effect.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetName<T>(this T se, string name) where T : StatusEffect_SO
        {
            se.EffectInfo._statusName = name;

            return se;
        }

        /// <summary>
        /// Sets the in-game description for the status effect.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="description">The new description for the status effect.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetDescription<T>(this T se, string description) where T : StatusEffect_SO
        {
            se.EffectInfo._description = description;

            return se;
        }

        /// <summary>
        /// Sets the in-game sprite for the status effect.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetSprite<T>(this T se, string spriteName, ModProfile profile = null) where T : StatusEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return se;

            se.EffectInfo.icon = profile.LoadSprite(spriteName);

            return se;
        }

        /// <summary>
        /// Sets the in-game sprite for the status effect.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="sprite">The Sprite object of the status effect's sprite.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetSprite<T>(this T se, Sprite sprite) where T : StatusEffect_SO
        {
            se.EffectInfo.icon = sprite;

            return se;
        }

        /// <summary>
        /// Sets the status effect's name, description and sprite.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="name">The new display name for the status effect.</param>
        /// <param name="description">The new description for the status effect.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetBasicInformation<T>(this T se, string name, string description, string spriteName, ModProfile profile = null) where T : StatusEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return se;

            se.EffectInfo._statusName = name;
            se.EffectInfo._description = description;
            se.EffectInfo.icon = profile.LoadSprite(spriteName);

            return se;
        }

        /// <summary>
        /// Sets the status effect's name, description and sprite.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="name">The new display name for the status effect.</param>
        /// <param name="description">The new description for the status effect.</param>
        /// <param name="sprite">The Sprite object of the status effect's sprite.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetBasicInformation<T>(this T se, string name, string description, Sprite sprite) where T : StatusEffect_SO
        {
            se.EffectInfo._statusName = name;
            se.EffectInfo._description = description;
            se.EffectInfo.icon = sprite;

            return se;
        }

        /// <summary>
        /// Sets the status effect's applied, updated and removed sounds.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="appliedSound">The name of the sound which plays on the status effect being applied for the first time.</param>
        /// <param name="updatedSound">The name of the sound which plays on the status effect's amount being updated.</param>
        /// <param name="removedSound">The name of the sound which plays on the status effect being removed.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T SetSounds<T>(this T se, string appliedSound = null, string updatedSound = null, string removedSound = null) where T : StatusEffect_SO
        {
            if (appliedSound != null)
                se.EffectInfo._applied_SE_Event = appliedSound;

            if (updatedSound != null)
                se.EffectInfo._updated_SE_Event = updatedSound;

            if (removedSound != null)
                se.EffectInfo._removed_SE_Event = removedSound;

            return se;
        }

        /// <summary>
        /// Adds the status effect to the Status/Field Database.
        /// </summary>
        /// <typeparam name="T">The status effect's custom type. Must be a subclass of StatusEffect_SO.</typeparam>
        /// <param name="se">The object instance of the status effect.</param>
        /// <param name="addToGlossaryToo">If true, also adds the status effect to the Glossary.</param>
        /// <returns>The instance of the status effect, for method chaining.</returns>
        public static T AddToDatabase<T>(this T se, bool addToGlossaryToo = true) where T : StatusEffect_SO
        {
            StatusField.AddNewStatusEffect(se, addToGlossaryToo);
            return se;
        }
    }
}
