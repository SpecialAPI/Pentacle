using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class FieldEffectBuilder
    {
        public static T NewFieldEffect<T>(string id_FE, string fieldId_ID, Assembly callingAssembly = null) where T : FieldEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if (!ProfileManager.TryGetProfile(callingAssembly, out var profile))
                return null;

            var fe = CreateScriptable<T>();
            fe.name = $"{profile.Prefix}_{id_FE}";
            fe._FieldID = $"{profile.Prefix}_{fieldId_ID}";

            var seInfo = CreateScriptable<SlotStatusEffectInfoSO>();
            seInfo.name = $"{fe.name}Info";
            seInfo._applied_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._updated_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._removed_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Remove";

            fe._EffectInfo = seInfo;

            return fe;
        }

        public static T SetName<T>(this T fe, string name) where T : FieldEffect_SO
        {
            fe.EffectInfo._fieldName = name;

            return fe;
        }

        public static T SetDescription<T>(this T fe, string description) where T : FieldEffect_SO
        {
            fe.EffectInfo._description = description;

            return fe;
        }

        public static T SetSprite<T>(this T fe, string spriteName, Assembly callingAssembly = null) where T : FieldEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();
            fe.EffectInfo.icon = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return fe;
        }

        public static T SetSprite<T>(this T fe, Sprite sprite) where T : FieldEffect_SO
        {
            fe.EffectInfo.icon = sprite;

            return fe;
        }

        public static T SetBasicInformation<T>(this T fe, string name, string description, string spriteName, Assembly callingAssembly = null) where T : FieldEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            fe.EffectInfo._fieldName = name;
            fe.EffectInfo._description = description;
            fe.EffectInfo.icon = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return fe;
        }

        public static T SetBasicInformation<T>(this T fe, string name, string description, Sprite sprite) where T : FieldEffect_SO
        {
            fe.EffectInfo._fieldName = name;
            fe.EffectInfo._description = description;
            fe.EffectInfo.icon = sprite;

            return fe;
        }

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

        public static T AddToDatabase<T>(this T fe, bool addToGlossaryToo = true) where T : FieldEffect_SO
        {
            StatusField.AddNewFieldEffect(fe, addToGlossaryToo);
            return fe;
        }

        public static T SetFieldPrefabs<T>(this T fe, string characterPath, string enemyPath, Assembly callingAssembly = null) where T : FieldEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if (!ProfileManager.TryGetProfile(callingAssembly, out var profile))
                return fe;

            var chPrefab = profile.Bundle.LoadAsset<GameObject>(characterPath);
            var chLayout = chPrefab != null ? chPrefab.GetComponent<CharacterFieldEffectLayout>() : null;

            var enPrefab = profile.Bundle.LoadAsset<GameObject>(enemyPath);
            var enLayout = enPrefab != null ? enPrefab.GetComponent<EnemyFieldEffectLayout>() : null;

            return fe.SetFieldPrefabs(chLayout, enLayout);
        }

        public static T SetFieldPrefabs<T>(this T fe, CharacterFieldEffectLayout characterPrefab, EnemyFieldEffectLayout enemyPrefab) where T : FieldEffect_SO
        {
            fe.EffectInfo.m_CharacterLayoutTemplate = characterPrefab;
            fe.EffectInfo.m_EnemyLayoutTemplate = enemyPrefab;

            return fe;
        }
    }
}
