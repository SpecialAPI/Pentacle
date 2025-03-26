using Pentacle.Tools;
using Pentacle.TriggerEffect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pentacle.Builders
{
    public static class PassiveBuilder
    {
        public static T NewPassive<T>(string id_PA, string passiveId, ModProfile profile = null) where T : BasePassiveAbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            var pa = CreateScriptable<T>();
            pa.name = profile.GetID(id_PA);
            pa.m_PassiveID = passiveId;

            pa._characterDescription = "This passive is not meant for party members.";
            pa._enemyDescription = "This passive is not meant for enemies.";

            return pa;
        }

        public static T SetName<T>(this T pa, string name) where T : BasePassiveAbilitySO
        {
            pa._passiveName = name;

            return pa;
        }

        public static T SetSprite<T>(this T pa, string spriteName, ModProfile profile = null) where T : BasePassiveAbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            pa.passiveIcon = profile.LoadSprite(spriteName);

            return pa;
        }

        public static T SetSprite<T>(this T pa, Sprite sprite) where T : BasePassiveAbilitySO
        {
            pa.passiveIcon = sprite;

            return pa;
        }

        public static T SetBasicInformation<T>(this T pa, string name, string spriteName, ModProfile profile = null) where T : BasePassiveAbilitySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            pa._passiveName = name;
            pa.passiveIcon = profile.LoadSprite(spriteName);

            return pa;
        }

        public static T SetBasicInformation<T>(this T pa, string name, Sprite sprite) where T : BasePassiveAbilitySO
        {
            pa._passiveName = name;
            pa.passiveIcon = sprite;

            return pa;
        }

        public static T SetCharacterDescription<T>(this T pa, string description) where T : BasePassiveAbilitySO
        {
            pa._characterDescription = description;

            return pa;
        }

        public static T SetEnemyDescription<T>(this T pa, string description) where T : BasePassiveAbilitySO
        {
            pa._enemyDescription = description;

            return pa;
        }

        public static T AutoSetDescriptions<T>(this T pa, string descriptionTemplate) where T : BasePassiveAbilitySO
        {
            pa._characterDescription = DoPassiveDescriptionAutoInserts(descriptionTemplate, false);
            pa._enemyDescription = DoPassiveDescriptionAutoInserts(descriptionTemplate, true);

            return pa;
        }

        public static string DoPassiveDescriptionAutoInserts(string description, bool enemy)
        {
            for (int i = 0; i < CharacterDescriptionInserts.Length; i++)
            {
                var idx = i;

                if (enemy)
                    idx = i % 2 == 0 ? i + 1 : i - 1;

                if (idx >= CharacterDescriptionInserts.Length)
                    idx = i;

                var orig = (DescriptionInsert)i;
                var rep = CharacterDescriptionInserts[idx];

                description = description.ReplaceAndKeepCase(orig.ToString(), rep);
            }

            return description;
        }

        public static T SetStoredValue<T>(this T pa, UnitStoreData_BasicSO storeData) where T : BasePassiveAbilitySO
        {
            pa.specialStoredData = storeData;

            return pa;
        }

        public static T AddToGlossary<T>(this T pa, string glossaryDescription) where T : BasePassiveAbilitySO
        {
            Glossary.CreateAndAddCustom_PassiveToGlossary(pa._passiveName, glossaryDescription, pa.passiveIcon);

            return pa;
        }

        public static T AddToDatabase<T>(this T pa) where T : BasePassiveAbilitySO
        {
            Passives.AddCustomPassiveToPool(pa.name, pa._passiveName, pa);

            return pa;
        }

        public static T SetTriggerEffects<T>(this T pa, List<EffectsAndTrigger> triggerEffects) where T : MultiCustomTriggerEffectPassive
        {
            pa.triggerEffects = triggerEffects;

            return pa;
        }

        public static T SetConnectionEffects<T>(this T pa, List<TriggeredEffect> connectionEffects) where T : MultiCustomTriggerEffectPassive
        {
            pa.connectionEffects = connectionEffects;

            return pa;
        }

        public static T SetDisconnectionEffects<T>(this T pa, List<TriggeredEffect> disconnectionEffects) where T : MultiCustomTriggerEffectPassive
        {
            pa.disconnectionEffects = disconnectionEffects;

            return pa;
        }

        public static T AddEffectsToAll<T>(this T pa, params EffectsAndTrigger[] effects) where T : MultiCustomTriggerEffectPassive
        {
            pa.triggerEffects.AddRange(effects);
            pa.connectionEffects.AddRange(effects);
            pa.disconnectionEffects.AddRange(effects);

            return pa;
        }

        public static readonly string[] CharacterDescriptionInserts =
        [
            "party member",
            "enemy",

            "party members",
            "enemies",
        ];

        public enum DescriptionInsert
        {
            ally,
            opponent,

            allies,
            opponents
        }
    }
}
