using Pentacle.Advanced;
using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;

namespace Pentacle.Builders
{
    public static class EnemyBuilder
    {
        public static AdvancedEnemySO NewEnemy(string id_EN, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return NewEnemy<AdvancedEnemySO>(id_EN, profile);
        }

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

        public static T SetName<T>(this T en, string name) where T : EnemySO
        {
            en._enemyName = name;

            return en;
        }

        public static T SetHealth<T>(this T en, int health, ManaColorSO healthColor) where T : EnemySO
        {
            en.health = health;
            en.healthColor = healthColor;

            return en;
        }

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

        public static T SetSounds<T>(this T en, string damageSound, string deathSound) where T : EnemySO
        {
            if (damageSound != null)
                en.damageSound = damageSound;
            if (deathSound != null)
                en.deathSound = deathSound;

            return en;
        }

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

        public static T AddPassives<T>(this T en, params BasePassiveAbilitySO[] passives) where T : EnemySO
        {
            en.passiveAbilities ??= [];
            en.passiveAbilities.AddRange(passives);

            return en;
        }

        public static T SetAbilities<T>(this T en, List<EnemyAbilityInfo> abilities) where T : EnemySO
        {
            en.abilities ??= [];
            en.abilities.Clear();
            en.abilities.AddRange(abilities);

            return en;
        }

        public static T AddHiddenEffects<T>(this T en, params HiddenEffectSO[] hiddenEffects) where T : AdvancedEnemySO
        {
            en.hiddenEffects ??= [];
            en.hiddenEffects.AddRange(hiddenEffects);

            return en;
        }

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
