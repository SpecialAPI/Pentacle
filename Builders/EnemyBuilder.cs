using Pentacle.Advanced;
using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;
using Pentacle.HiddenPassiveEffects;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating custom enemies.
    /// </summary>
    public static class EnemyBuilder
    {
        /// <summary>
        /// Creates a new enemy using Pentacle's custom AdvancedEnemySO class.
        /// </summary>
        /// <param name="id_EN">The string id of the enemy. Naming convention: EnemyName_EN</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created enemy.</returns>
        public static AdvancedEnemySO NewEnemy(string id_EN, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return NewEnemy<AdvancedEnemySO>(id_EN, profile);
        }

        /// <summary>
        /// Creates a new enemy of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created enemy. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="id_EN">The string id of the enemy. Naming convention: EnemyName_EN</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns></returns>
        public static T NewEnemy<T>(string id_EN, ModProfile profile = null) where T : EnemySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var en = CreateScriptable<T>();

            en.name = profile.GetID(id_EN);

            en.priority = Priority.Normal;
            en.abilitySelector = MiscDB.RarityAbilitySelector;

            en.abilities = [];
            en.passiveAbilities = [];
            en.enterEffects = [];
            en.exitEffects = [];
            en.enemyLoot = new([]);

            en.health = 1;
            en.healthColor = Pigments.Red;

            en.damageSound = string.Empty;
            en.deathSound = string.Empty;

            return en;
        }

        /// <summary>
        /// Sets the in-game name of the enemy.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="name">The new display name of the enemy.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetName<T>(this T en, string name) where T : EnemySO
        {
            en._enemyName = name;

            return en;
        }

        /// <summary>
        /// Sets this enemy's max health and health color.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="health">THe max health for this enemy,</param>
        /// <param name="healthColor">The pigment color of this enemy's health.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetHealth<T>(this T en, int health, ManaColorSO healthColor) where T : EnemySO
        {
            en.health = health;
            en.healthColor = healthColor;

            return en;
        }

        /// <summary>
        /// Sets this enemy's UI, overworld and corpse sprites. Treats the UI sprite as a boss icon and anchors it around its center.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="combatSpriteName">The name of the image file for the combat UI sprite (that shows on the timeline and when the enemy is selected).<para />.png extension is optional.</param>
        /// <param name="overworldSpriteName">The name of the image file for the overworld sprite.<para />.png extension is optional.</param>
        /// <param name="corpseSpriteName">The name of the image file for the corpse sprite (that shows in the overworld after combat).<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetSpritesBoss<T>(this T en, string combatSpriteName, string overworldSpriteName, string corpseSpriteName, ModProfile profile = null) where T : EnemySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return en;

            en.enemySprite = profile.LoadSprite(combatSpriteName);

            if (!string.IsNullOrEmpty(overworldSpriteName))
                en.enemyOverworldSprite = profile.LoadSprite(overworldSpriteName, new(0.5f, 0f));
            else
                en.enemyOverworldSprite = en.enemySprite;

            if (!string.IsNullOrEmpty(corpseSpriteName))
                en.enemyOWCorpseSprite = profile.LoadSprite(corpseSpriteName, new(0.5f, 0f));
            else
                en.enemyOWCorpseSprite = en.enemyOverworldSprite;

            return en;
        }

        /// <summary>
        /// Sets this enemy's UI, overworld and corpse sprites. Treats the UI sprite as a regular enemy icon and anchors it around its bottom middle point.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="combatSpriteName">The name of the image file for the combat UI sprite (that shows on the timeline and when the enemy is selected).<para />.png extension is optional.</param>
        /// <param name="overworldSpriteName">The name of the image file for the overworld sprite.<para />.png extension is optional.</param>
        /// <param name="corpseSpriteName">The name of the image file for the corpse sprite (that shows in the overworld after combat).<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetSprites<T>(this T en, string combatSpriteName, string overworldSpriteName, string corpseSpriteName, ModProfile profile = null) where T : EnemySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return en;

            en.enemySprite = profile.LoadSprite(combatSpriteName, new(0.5f, 0f));

            if (!string.IsNullOrEmpty(overworldSpriteName))
                en.enemyOverworldSprite = profile.LoadSprite(overworldSpriteName, new(0.5f, 0f));
            else
                en.enemyOverworldSprite = en.enemySprite;

            if(!string.IsNullOrEmpty(corpseSpriteName))
                en.enemyOWCorpseSprite = profile.LoadSprite(corpseSpriteName, new(0.5f, 0f));
            else
                en.enemyOWCorpseSprite = en.enemyOverworldSprite;

            return en;
        }

        /// <summary>
        /// Sets this enemy's UI, overworld and corpse sprites.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="combatSprite">The Sprite object of the combat UI sprite (that shows on the timeline and when the enemy is selected).</param>
        /// <param name="overworldSprite">The Sprite object of the overworld sprite.</param>
        /// <param name="corpseSprite">The Sprite object of the corpse sprite (that shows in the overworld after combat).</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetSprites<T>(this T en, Sprite combatSprite, Sprite overworldSprite, Sprite corpseSprite) where T : EnemySO
        {
            en.enemySprite = combatSprite;

            if(overworldSprite != null)
                en.enemyOverworldSprite = overworldSprite;
            else
                en.enemyOverworldSprite = en.enemySprite;

            if(corpseSprite != null)
                en.enemyOWCorpseSprite = corpseSprite;
            else
                en.enemyOWCorpseSprite = en.enemyOverworldSprite;

            return en;
        }

        /// <summary>
        /// Sets the enemy's damage and death sounds.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="damageSound">The name of the sound which plays on the enemy taking damage.</param>
        /// <param name="deathSound">The name of the sound which plays on the enemy dying.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetSounds<T>(this T en, string damageSound, string deathSound) where T : EnemySO
        {
            if (damageSound != null)
                en.damageSound = damageSound;
            if (deathSound != null)
                en.deathSound = deathSound;

            return en;
        }

        /// <summary>
        /// Sets the enemy's combat object prefab.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="prefabName">The prefab's name in your profile's asset bundle.</param>
        /// <param name="gibsName">The name of the gibs object prefab in your profile's asset bundle. Optional, use only if the enemy prefab doesn't already have gibs linked to it.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetEnemyPrefab<T>(this T en, string prefabName, string gibsName = null, ModProfile profile = null) where T : EnemySO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return en;

            var prefab = profile.Bundle.LoadAsset<GameObject>(prefabName);
            var gibs = (ParticleSystem)null;

            if (!string.IsNullOrEmpty(gibsName))
            {
                var gibsObj = profile.Bundle.LoadAsset<GameObject>(gibsName);
                gibs = gibsObj != null ? gibsObj.GetComponent<ParticleSystem>() : null;
            }

            return en.SetEnemyPrefab(prefab, gibs);
        }

        /// <summary>
        /// Sets the enemy's combat object prefab.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="prefab">The combat object prefab for the enemy.</param>
        /// <param name="gibs">The gibs prefab for the enemy. Optional, use only if the enemy prefab doesn't already have gibs linked to it.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetEnemyPrefab<T>(this T en, GameObject prefab, ParticleSystem gibs = null) where T : EnemySO
        {
            if (prefab == null)
                return en;

            var layout = prefab.GetComponent<EnemyInFieldLayout>();
            if (layout == null)
                layout = prefab.AddComponent<EnemyInFieldLayout>();

            var data = prefab.GetComponent<EnemyInFieldLayout_Data>();
            if (data == null)
            {
                data = prefab.AddComponent<EnemyInFieldLayout_Data>();
                data.SetDefaultData();
            }

            if (gibs != null)
                data.m_Gibs = gibs;

            layout.m_Data = data;
            en.enemyTemplate = layout;

            return en;
        }

        /// <summary>
        /// Adds a list of passive effects to the enemy.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="passives">The passive abilities to add to the enemy, as BasePassiveAbilitySO objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T AddPassives<T>(this T en, params BasePassiveAbilitySO[] passives) where T : EnemySO
        {
            en.passiveAbilities ??= [];
            en.passiveAbilities.AddRange(passives);

            return en;
        }

        /// <summary>
        /// Sets the list of abilities for this enemy. This will remove any abilities the enemy had before this.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="abilities">The list of abilities for this enemy, as EnemyAbilityInfo objects.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T SetAbilities<T>(this T en, List<EnemyAbilityInfo> abilities) where T : EnemySO
        {
            en.abilities ??= [];
            en.abilities.Clear();
            en.abilities.AddRange(abilities);

            return en;
        }

        /// <summary>
        /// Adds a list of unit types to this enemy (such as being considered a fish).
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="unitTypes">The string unit types to add to the enemy.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T AddUnitTypes<T>(this T en, params string[] unitTypes) where T : EnemySO
        {
            en.unitTypes ??= [];
            en.unitTypes.AddRange(unitTypes);

            return en;
        }

        /// <summary>
        /// Adds a list of hidden passive effects to the AdvancedEnemySO enemy.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be AdvancedEnemySO or a subclass of AdvancedEnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="hiddenEffects">The hidden passive effects to add to the enemy, as HiddenEffectSO objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T AddHiddenEffects<T>(this T en, params HiddenEffectSO[] hiddenEffects) where T : AdvancedEnemySO
        {
            en.hiddenEffects ??= [];
            en.hiddenEffects.AddRange(hiddenEffects);

            return en;
        }

        /// <summary>
        /// Adds the enemy to the Enemy Database.
        /// </summary>
        /// <typeparam name="T">The enemy's custom type. Must either be EnemySO or a subclass of EnemySO.</typeparam>
        /// <param name="en">The object instance of the enemy.</param>
        /// <param name="addToBronzoPool">If true, this enemy will be added to the list of enemies that Bronzo's Unity can summon.</param>
        /// <param name="addToSepulchrePool">If true, this enemy will be added to the list of enemies that Sepulchre's Chastity can summon.</param>
        /// <param name="addToSmallPool">If true, this enemy will be added to the list of enemies that Scrungie's Regurgitate can summon.</param>
        /// <returns>The instance of the enemy, for method chaining.</returns>
        public static T AddToDatabase<T>(this T en, bool addToBronzoPool = false, bool addToSepulchrePool = false, bool addToSmallPool = false) where T : EnemySO
        {
            EnemyDB.AddNewEnemy(en.name, en);

            if (addToBronzoPool)
                EnemyUtils.AddEnemyToSpawnPool(en, PoolList_GameIDs.Bronzo);
            if (addToSepulchrePool)
                EnemyUtils.AddEnemyToHealthSpawnPool(en, PoolList_GameIDs.Sepulchre);
            if (addToSmallPool)
                EnemyUtils.AddEnemyToSpawnPool(en, PoolList_GameIDs.SmallEnemy);

            return en;
        }
    }
}
