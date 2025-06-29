using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class FieldEffectBuilder
    {
        /// <summary>
        /// Creates a new field effect of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created field effect. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="id_FE">The string database ID of the field effect. Naming convention: FieldEffectName_FE</param>
        /// <param name="fieldId_ID">The string field ID of the field effect used to check if two field effects are the same. Naming convention: FieldEffectName_ID</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T NewFieldEffect<T>(string id_FE, string fieldId_ID, ModProfile profile = null) where T : FieldEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var fe = CreateScriptable<T>();
            fe.name = profile.GetID(id_FE);
            fe._FieldID = profile.GetID(fieldId_ID);

            var seInfo = CreateScriptable<SlotStatusEffectInfoSO>();
            seInfo.name = $"{fe.name}Info";
            seInfo._applied_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._updated_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._removed_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Remove";

            fe._EffectInfo = seInfo;

            return fe;
        }

        /// <summary>
        /// Sets the in-game name for the field effect.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="name">The new display name for the field effect.</param>
        /// <returns>The instance of the field effect, for method chaining.</returns>
        public static T SetName<T>(this T fe, string name) where T : FieldEffect_SO
        {
            fe.EffectInfo._fieldName = name;

            return fe;
        }

        /// <summary>
        /// Sets the in-game description for the field effect.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="description">The new description for the field effect.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetDescription<T>(this T fe, string description) where T : FieldEffect_SO
        {
            fe.EffectInfo._description = description;

            return fe;
        }

        /// <summary>
        /// Sets the in-game sprite for the field effect.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetSprite<T>(this T fe, string spriteName, ModProfile profile = null) where T : FieldEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return fe;

            fe.EffectInfo.icon = profile.LoadSprite(spriteName);

            return fe;
        }

        /// <summary>
        /// Sets the in-game sprite for the field effect.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="sprite">The Sprite object of the field effect's sprite.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetSprite<T>(this T fe, Sprite sprite) where T : FieldEffect_SO
        {
            fe.EffectInfo.icon = sprite;

            return fe;
        }

        /// <summary>
        /// Sets the field effect's name, description and sprite.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="name">The new display name for the field effect.</param>
        /// <param name="description">The new description for the field effect.</param>
        /// <param name="spriteName">The name of the image file in the project files.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetBasicInformation<T>(this T fe, string name, string description, string spriteName, ModProfile profile = null) where T : FieldEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return fe;

            fe.EffectInfo._fieldName = name;
            fe.EffectInfo._description = description;
            fe.EffectInfo.icon = profile.LoadSprite(spriteName);

            return fe;
        }

        /// <summary>
        /// Sets the field effect's name, description and sprite.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="name">The new display name for the field effect.</param>
        /// <param name="description">The new description for the field effect.</param>
        /// <param name="sprite">The Sprite object of the field effect's sprite.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetBasicInformation<T>(this T fe, string name, string description, Sprite sprite) where T : FieldEffect_SO
        {
            fe.EffectInfo._fieldName = name;
            fe.EffectInfo._description = description;
            fe.EffectInfo.icon = sprite;

            return fe;
        }

        /// <summary>
        /// Sets the field effect's applied, updated and removed sounds.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="appliedSound">The name of the sound which plays on the field effect being applied for the first time.</param>
        /// <param name="updatedSound">The name of the sound which plays on the field effect's amount being updated.</param>
        /// <param name="removedSound">The name of the sound which plays on the field effect being removed.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetSounds<T>(this T fe, string appliedSound = null, string updatedSound = null, string removedSound = null) where T : FieldEffect_SO
        {
            if (appliedSound != null)
                fe.EffectInfo._applied_SE_Event = appliedSound;

            if (updatedSound != null)
                fe.EffectInfo._updated_SE_Event = updatedSound;

            if (removedSound != null)
                fe.EffectInfo._removed_SE_Event = removedSound;

            return fe;
        }

        /// <summary>
        /// Adds the field effect to the Status/Field Database.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="addToGlossaryToo">If true, also adds the field effect to the Glossary.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T AddToDatabase<T>(this T fe, bool addToGlossaryToo = true) where T : FieldEffect_SO
        {
            StatusField.AddNewFieldEffect(fe, addToGlossaryToo);
            return fe;
        }

        /// <summary>
        /// Sets the field effect's character and enemy field prefabs.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="characterPath">The character field prefab's name in your profile's asset bundle.</param>
        /// <param name="enemyPath">The enemy field prefab's name in your profile's asset bundle.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetFieldPrefabs<T>(this T fe, string characterPath, string enemyPath, ModProfile profile = null) where T : FieldEffect_SO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return fe;

            var chPrefab = profile.Bundle.LoadAsset<GameObject>(characterPath);
            var chLayout = chPrefab != null ? chPrefab.GetComponent<CharacterFieldEffectLayout>() : null;

            var enPrefab = profile.Bundle.LoadAsset<GameObject>(enemyPath);
            var enLayout = enPrefab != null ? enPrefab.GetComponent<EnemyFieldEffectLayout>() : null;

            return fe.SetFieldPrefabs(chLayout, enLayout);
        }

        /// <summary>
        /// Sets the field effect's character and enemy field prefabs.
        /// </summary>
        /// <typeparam name="T">The field effect's custom type. Must be a subclass of FieldEffect_SO.</typeparam>
        /// <param name="fe">The object instance of the field effect.</param>
        /// <param name="characterPrefab">The character field prefab for the field effect.</param>
        /// <param name="enemyPrefab">The enemy field prefab for the field effect.</param>
        /// <returns>An object instance of the created field effect.</returns>
        public static T SetFieldPrefabs<T>(this T fe, CharacterFieldEffectLayout characterPrefab, EnemyFieldEffectLayout enemyPrefab) where T : FieldEffect_SO
        {
            fe.EffectInfo.m_CharacterLayoutTemplate = characterPrefab;
            fe.EffectInfo.m_EnemyLayoutTemplate = enemyPrefab;

            return fe;
        }
    }
}
