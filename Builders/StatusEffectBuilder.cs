using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class StatusEffectBuilder
    {
        public static T NewStatusEffect<T>(string id_SE, string statusId_ID, Assembly callingAssembly = null) where T : StatusEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if (!ProfileManager.TryGetProfile(callingAssembly, out var profile))
                return null;

            var se = CreateScriptable<T>();
            se.name = $"{profile.Prefix}_{id_SE}";
            se._StatusID = $"{profile.Prefix}_{statusId_ID}";

            var seInfo = CreateScriptable<StatusEffectInfoSO>();
            seInfo.name = $"{se.name}Info";
            seInfo._applied_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._updated_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Update";
            seInfo._removed_SE_Event = "event:/UI/Combat/Status/UI_CBT_STS_Remove";

            se._EffectInfo = seInfo;

            return se;
        }

        public static T SetName<T>(this T se, string name) where T : StatusEffect_SO
        {
            se.EffectInfo._statusName = name;

            return se;
        }

        public static T SetDescription<T>(this T se, string description) where T : StatusEffect_SO
        {
            se.EffectInfo._description = description;

            return se;
        }

        public static T SetSprite<T>(this T se, string spriteName, Assembly callingAssembly = null) where T : StatusEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();
            se.EffectInfo.icon = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return se;
        }

        public static T SetSprite<T>(this T se, Sprite sprite) where T : StatusEffect_SO
        {
            se.EffectInfo.icon = sprite;

            return se;
        }

        public static T SetBasicInformation<T>(this T se, string name, string description, string spriteName, Assembly callingAssembly = null) where T : StatusEffect_SO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            se.EffectInfo._statusName = name;
            se.EffectInfo._description = description;
            se.EffectInfo.icon = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return se;
        }

        public static T SetBasicInformation<T>(this T se, string name, string description, Sprite sprite) where T : StatusEffect_SO
        {
            se.EffectInfo._statusName = name;
            se.EffectInfo._description = description;
            se.EffectInfo.icon = sprite;

            return se;
        }

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

        public static T AddToDatabase<T>(this T se, bool addToGlossaryToo = true) where T : StatusEffect_SO
        {
            StatusField.AddNewStatusEffect(se, addToGlossaryToo);
            return se;
        }
    }
}
