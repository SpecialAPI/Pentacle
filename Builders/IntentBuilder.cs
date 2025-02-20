using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pentacle.Builders
{
    public static class IntentBuilder
    {
        public static readonly Color IntentColor_StatusRemove = new(0.3529f, 0.3529f, 0.3529f);
        public static readonly Color IntentColor_DamageToEnemy = new(1f, 0f, 0f);
        public static readonly Color IntentColor_DamageToCharcter = new(0.8471f, 0.4549f, 0.898f);

        public static string AddIntent(string id, string spriteName, Color? color = null, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            return AddIntent(id, profile.LoadSprite(spriteName), color, profile);
        }

        public static string AddIntent(string id, Sprite sprite, Color? color = null, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            color ??= Color.white;
            var intentId = profile.GetID(id);
            Intents.AddCustom_Basic_IntentToPool(intentId, new()
            {
                id = intentId,
                _sprite = sprite,
                _color = color.GetValueOrDefault()
            });

            return intentId;
        }

        public static void AddStatusEffectIntents(string baseId, StatusEffect_SO status, out string applyIntent, out string removeIntent, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            applyIntent = AddIntent(baseId, status.EffectInfo.icon, Color.white, profile);
            removeIntent = AddIntent($"Rem_{baseId}", status.EffectInfo.icon, IntentColor_StatusRemove, profile);
        }

        public static void AddFieldEffectIntents(string baseId, FieldEffect_SO field, out string applyIntent, out string removeIntent, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            applyIntent = AddIntent(baseId, field.EffectInfo.icon, Color.white, profile);
            removeIntent = AddIntent($"Rem_{baseId}", field.EffectInfo.icon, IntentColor_StatusRemove, profile);
        }

        public static void AddPassiveIntents(string baseId, BasePassiveAbilitySO passive, out string addPassiveIntent, out string removePassiveIntent, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            addPassiveIntent = AddIntent(baseId, passive.passiveIcon, Color.white, profile);
            removePassiveIntent = AddIntent($"Rem_{baseId}", passive.passiveIcon, IntentColor_StatusRemove, profile);
        }

        public static void AddGenericAddRemoveIntents(string baseId, string spriteName, out string addPassiveIntent, out string removePassiveIntent, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            var sprite = profile.LoadSprite(spriteName);
            addPassiveIntent = AddIntent(baseId, sprite, Color.white, profile);
            removePassiveIntent = AddIntent($"Rem_{baseId}", sprite, IntentColor_StatusRemove, profile);
        }

        public static void AddGenericAddRemoveIntents(string baseId, Sprite sprite, out string addPassiveIntent, out string removePassiveIntent, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            addPassiveIntent = AddIntent(baseId, sprite, Color.white, profile);
            removePassiveIntent = AddIntent($"Rem_{baseId}", sprite, IntentColor_StatusRemove, profile);
        }

        public static string AddDamageIntent(string id, string characterSpriteName, string enemySpriteName, Color? characterColor = null, Color? enemyColor = null, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            return AddDamageIntent(id, profile.LoadSprite(characterSpriteName), profile.LoadSprite(enemySpriteName), characterColor, enemyColor, profile);
        }

        public static string AddDamageIntent(string id, Sprite characterSprite, Sprite enemySprite, Color? characterColor = null, Color? enemyColor = null, ModProfile profile = null)
        {
            characterColor ??= IntentColor_DamageToCharcter;
            enemyColor ??= IntentColor_DamageToEnemy;
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            var intentId = profile.GetID(id);
            Intents.AddCustom_Damage_IntentToPool(intentId, new()
            {
                id = intentId,
                
                _color = characterColor.GetValueOrDefault(),
                _enemyColor = enemyColor.GetValueOrDefault(),

                _sprite = characterSprite,
                _enemySprite = enemySprite
            });

            return intentId;
        }
    }
}
