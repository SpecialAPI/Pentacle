using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating custom ability intents.
    /// </summary>
    public static class IntentBuilder
    {
        /// <summary>
        /// The color of intents for removing status effects and field effects.
        /// </summary>
        public static readonly Color IntentColor_StatusRemove = new(0.3529f, 0.3529f, 0.3529f);
        /// <summary>
        /// The color of damage intents targeting enemies.
        /// </summary>
        public static readonly Color IntentColor_DamageToEnemy = new(1f, 0f, 0f);
        /// <summary>
        /// The color of damage intents targeting characters.
        /// </summary>
        public static readonly Color IntentColor_DamageToCharcter = new(0.8471f, 0.4549f, 0.898f);

        /// <summary>
        /// Adds a new intent to the game.
        /// </summary>
        /// <param name="id">The string ID of the intent.</param>
        /// <param name="spriteName">The name of the image file for the intent's sprite.<para />.png extension is optional.</param>
        /// <param name="color">The intent sprite's tint. Optional, defaults to white (no tint).</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The created intent's ID.</returns>
        public static string AddIntent(string id, string spriteName, Color? color = null, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return string.Empty;

            return AddIntent(id, profile.LoadSprite(spriteName), color, profile);
        }

        /// <summary>
        /// Adds a new intent to the game.
        /// </summary>
        /// <param name="id">The string ID of the intent.</param>
        /// <param name="sprite">The Sprite object of the intent's sprite.</param>
        /// <param name="color">The intent sprite's tint. Optional, defaults to white (no tint).</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The created intent's ID.</returns>
        public static string AddIntent(string id, Sprite sprite, Color? color = null, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return string.Empty;

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

        /// <summary>
        /// Adds intents for applying and removing a status effect to the game.
        /// </summary>
        /// <param name="baseId">Base string ID for the intents. Naming convention: Status_StatusEffectName</param>
        /// <param name="status">The status effect that the intents are for.</param>
        /// <param name="applyIntent">The created "apply status effect" intent's ID.</param>
        /// <param name="removeIntent">The created "remove status effect" intent's ID.</param>
        /// <param name="profile">Your mod profile.</param>
        public static void AddStatusEffectIntents(string baseId, StatusEffect_SO status, out string applyIntent, out string removeIntent, ModProfile profile = null)
        {
            applyIntent = string.Empty;
            removeIntent = string.Empty;

            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return;

            applyIntent = AddIntent(baseId, status.EffectInfo.icon, Color.white, profile);
            removeIntent = AddIntent($"Rem_{baseId}", status.EffectInfo.icon, IntentColor_StatusRemove, profile);
        }

        /// <summary>
        /// Adds intents for applying and removing a field effect to the game.
        /// </summary>
        /// <param name="baseId">Base string ID for the intents. Naming convention: Field_FieldEffectName</param>
        /// <param name="field">The field effect that the intents are for.</param>
        /// <param name="applyIntent">The created "apply field effect" intent's ID.</param>
        /// <param name="removeIntent">The created "remove field effect" intent's ID.</param>
        /// <param name="profile">Your mod profile.</param>
        public static void AddFieldEffectIntents(string baseId, FieldEffect_SO field, out string applyIntent, out string removeIntent, ModProfile profile = null)
        {
            applyIntent = string.Empty;
            removeIntent = string.Empty;

            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return;

            applyIntent = AddIntent(baseId, field.EffectInfo.icon, Color.white, profile);
            removeIntent = AddIntent($"Rem_{baseId}", field.EffectInfo.icon, IntentColor_StatusRemove, profile);
        }

        /// <summary>
        /// Adds intents for adding and removing a passive to the game.
        /// </summary>
        /// <param name="baseId">Base string ID for the intents. Naming convention: PA_PassiveName</param>
        /// <param name="passive">The passive that the intents are for.</param>
        /// <param name="addPassiveIntent">The created "add passive" intent's ID.</param>
        /// <param name="removePassiveIntent">The created "remove passive" intent's ID.</param>
        /// <param name="profile">Your mod profile.</param>
        public static void AddPassiveIntents(string baseId, BasePassiveAbilitySO passive, out string addPassiveIntent, out string removePassiveIntent, ModProfile profile = null)
        {
            addPassiveIntent = string.Empty;
            removePassiveIntent = string.Empty;

            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return;

            addPassiveIntent = AddIntent(baseId, passive.passiveIcon, Color.white, profile);
            removePassiveIntent = AddIntent($"Rem_{baseId}", passive.passiveIcon, IntentColor_StatusRemove, profile);
        }

        /// <summary>
        /// Adds a pair of intents for "adding" and "removing" something using the given sprite.
        /// </summary>
        /// <param name="baseId">Base string ID for the intents.</param>
        /// <param name="spriteName">The name of the image file for the intents.<para />.png extension is optional.</param>
        /// <param name="addIntent">The created "add" intent's ID.</param>
        /// <param name="removeIntent">The created "remove passive" intent's ID.</param>
        /// <param name="profile">Your mod profile.</param>
        public static void AddGenericAddRemoveIntents(string baseId, string spriteName, out string addIntent, out string removeIntent, ModProfile profile = null)
        {
            addIntent = string.Empty;
            removeIntent = string.Empty;

            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return;

            var sprite = profile.LoadSprite(spriteName);
            addIntent = AddIntent(baseId, sprite, Color.white, profile);
            removeIntent = AddIntent($"Rem_{baseId}", sprite, IntentColor_StatusRemove, profile);
        }

        /// <summary>
        /// Adds a pair of intents for "adding" and "removing" something using the given sprite.
        /// </summary>
        /// <param name="baseId">Base string ID for the intents.</param>
        /// <param name="sprite">The Sprite object of the intents.</param>
        /// <param name="addIntent">The created "add" intent's ID.</param>
        /// <param name="removeIntent">The created "remove passive" intent's ID.</param>
        /// <param name="profile">Your mod profile.</param>
        public static void AddGenericAddRemoveIntents(string baseId, Sprite sprite, out string addIntent, out string removeIntent, ModProfile profile = null)
        {
            addIntent = string.Empty;
            removeIntent = string.Empty;

            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return;

            addIntent = AddIntent(baseId, sprite, Color.white, profile);
            removeIntent = AddIntent($"Rem_{baseId}", sprite, IntentColor_StatusRemove, profile);
        }

        /// <summary>
        /// Adds an intent for dealing damage to the game.
        /// </summary>
        /// <param name="id">The string ID of the intent.</param>
        /// <param name="characterSpriteName">The name of the image file for the intent's sprite when targeting characters.<para />.png extension is optional.</param>
        /// <param name="enemySpriteName">The name of the image file for the intent's sprite when targeting enemies.<para />.png extension is optional.</param>
        /// <param name="characterColor">The intent's tint when targeting characters. Optional, defaults to the basegame "damage to characters" intent tint.</param>
        /// <param name="enemyColor">The intent's tint when targeting enemies. Optional, defaults to the basegame "damage to enemies" intent tint.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The created intent's ID.</returns>
        public static string AddDamageIntent(string id, string characterSpriteName, string enemySpriteName, Color? characterColor = null, Color? enemyColor = null, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return string.Empty;

            return AddDamageIntent(id, profile.LoadSprite(characterSpriteName), profile.LoadSprite(enemySpriteName), characterColor, enemyColor, profile);
        }

        /// <summary>
        /// Adds an intent for dealing damage to the game.
        /// </summary>
        /// <param name="id">The string ID of the intent.</param>
        /// <param name="characterSprite">The Sprite object of the intent's sprite when targeting characters.</param>
        /// <param name="enemySprite">The Sprite object of the intent's sprite when targeting enemies.</param>
        /// <param name="characterColor">The intent's tint when targeting characters. Optional, defaults to the basegame "damage to characters" intent tint.</param>
        /// <param name="enemyColor">The intent's tint when targeting enemies. Optional, defaults to the basegame "damage to enemies" intent tint.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The created intent's ID.</returns>
        public static string AddDamageIntent(string id, Sprite characterSprite, Sprite enemySprite, Color? characterColor = null, Color? enemyColor = null, ModProfile profile = null)
        {
            characterColor ??= IntentColor_DamageToCharcter;
            enemyColor ??= IntentColor_DamageToEnemy;
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return string.Empty;

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
