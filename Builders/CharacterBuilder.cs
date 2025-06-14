﻿using Pentacle.Advanced;
using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    public static class CharacterBuilder
    {
        public static AdvancedCharacterSO NewCharacter(string id_CH, string entityId, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return NewCharacter<AdvancedCharacterSO>(id_CH, entityId, profile);
        }

        public static T NewCharacter<T>(string id_CH, string entityId, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var ch = CreateScriptable<T>();
            ch.name = profile.GetID(id_CH);
            ch.entityID = profile.GetID(entityId);

            ch.healthColor = Pigments.Purple;
            ch.passiveAbilities = [];
            ch.basicCharAbility = AbilityDB.SlapAbility;

            ch.damageSound = string.Empty;
            ch.deathSound = string.Empty;
            ch.dxSound = string.Empty;

            ch.m_StartsLocked = false;

            return ch;
        }

        public static T SetBasicInformation<T>(this T ch, string name, ManaColorSO healthColor, string frontSpriteName, string backSpriteName, string overworldSpriteName, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return ch;

            ch._characterName = name;
            ch.healthColor = healthColor;
            ch.characterSprite = profile.LoadSprite(frontSpriteName);
            ch.characterBackSprite = profile.LoadSprite(backSpriteName);
            ch.characterOWSprite = profile.LoadSprite(overworldSpriteName, new(0.5f, 0f));

            return ch;
        }

        public static T SetBasicInformation<T>(this T ch, string name, ManaColorSO healthColor, Sprite frontSprite, Sprite backSprite, Sprite overworldSprite) where T : CharacterSO
        {
            ch._characterName = name;
            ch.healthColor = healthColor;
            ch.characterSprite = frontSprite;
            ch.characterBackSprite = backSprite;
            ch.characterOWSprite = overworldSprite;

            return ch;
        }

        public static T SetName<T>(this T ch, string name) where T : CharacterSO
        {
            ch._characterName = name;

            return ch;
        }

        public static T SetHealthColor<T>(this T ch, ManaColorSO color) where T : CharacterSO
        {
            ch.healthColor = color;

            return ch;
        }

        public static T SetSprites<T>(this T ch, string frontSpriteName, string backSpriteName, string overworldSpriteName, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return ch;

            ch.characterSprite = profile.LoadSprite(frontSpriteName);
            ch.characterBackSprite = profile.LoadSprite(backSpriteName);
            ch.characterOWSprite = profile.LoadSprite(overworldSpriteName, new(0.5f, 0f));

            return ch;
        }

        public static T SetSprites<T>(this T ch, Sprite frontSprite, Sprite backSprite, Sprite overworldSprite) where T : CharacterSO
        {
            ch.characterSprite = frontSprite;
            ch.characterBackSprite = backSprite;
            ch.characterOWSprite = overworldSprite;

            return ch;
        }

        public static T SetSounds<T>(this T ch, string damageSound, string deathSound, string dialogueSound) where T : CharacterSO
        {
            if(damageSound != null)
                ch.damageSound = damageSound;
            if(deathSound != null)
                ch.deathSound = deathSound;
            if(dialogueSound != null)
                ch.dxSound = dialogueSound;

            return ch;
        }

        public static T AddPassives<T>(this T ch, params BasePassiveAbilitySO[] passives) where T : CharacterSO
        {
            ch.passiveAbilities ??= [];
            ch.passiveAbilities.AddRange(passives);

            return ch;
        }

        public static T AddRankedData<T>(this T ch, params CharacterRankedData[] rankedData) where T : CharacterSO
        {
            ch.rankedData ??= [];
            ch.rankedData.AddRange(rankedData);

            return ch;
        }

        public static T SetMovesOnOverworld<T>(this T ch, bool moves) where T : CharacterSO
        {
            ch.movesOnOverworld = moves;

            return ch;
        }

        public static T RankedDataSetup<T>(this T ch, int ranks, Func<int, int, CharacterRankedData> rankSetup) where T : CharacterSO
        {
            ch.rankedData ??= [];
            ch.rankedData.Clear();

            for(var i = 0; i < ranks; i++)
            {
                _rank = i;
                ch.rankedData.Add(rankSetup(i, i + 1));

                _rank = -1;
            }

            return ch;
        }

        public static T RankedValue<T>(params T[] rankValues)
        {
            return rankValues[Mathf.Clamp(_rank, 0, rankValues.Length - 1)];
        }

        public static T RankedValueManual<T>(int rank, params T[] rankValues)
        {
            return rankValues[Mathf.Clamp(rank, 0, rankValues.Length - 1)];
        }

        public static T SetBasicAbility<T>(this T ch, CharacterAbility basicAbility) where T : CharacterSO
        {
            ch.basicCharAbility = basicAbility;

            return ch;
        }

        public static T SetAbilityUsage<T>(this T ch, bool usesAllAbilities, bool usesBasicAbility) where T : CharacterSO
        {
            ch.usesAllAbilities = usesAllAbilities;
            ch.usesBasicAbility = usesBasicAbility;

            return ch;
        }

        public static T AddUnitTypes<T>(this T ch, params string[] unitTypes) where T : CharacterSO
        {
            ch.unitTypes ??= [];
            ch.unitTypes.AddRange(unitTypes);

            return ch;
        }

        public static T AddHiddenEffects<T>(this T ch, params HiddenEffectSO[] hiddenEffects) where T : AdvancedCharacterSO
        {
            ch.hiddenEffects ??= [];
            ch.hiddenEffects.AddRange(hiddenEffects);

            return ch;
        }

        public static T AddFinalBossUnlock<T>(this T ch, string bossId, UnlockableModData unlock) where T : CharacterSO
        {
            if (unlock == null)
                return ch;

            if(unlock.hasModdedAchievementUnlock)
                ch.m_BossAchData.Add(new(bossId, unlock.moddedAchievementID));

            if (UnlockablesDB.TryGetFinalBossUnlockCheck(bossId, out var check))
                check.AddUnlockData(ch.entityID, unlock);
            else
                DelayedActions.delayedFinalBossUnlocks.Add((bossId, ch.entityID, unlock));

            return ch;
        }

        public static T AddToDatabase<T>(this T ch, bool appearsInShops = true, bool locked = false) where T : CharacterSO
        {
            ch.m_StartsLocked = locked;
            if(!appearsInShops)
                MiscDB.AddOmittedFoolToZones(ch.name);

            CharacterDB.AddNewCharacter(ch.name, ch);

            return ch;
        }

        public static SelectableCharacterData GenerateMenuCharacter<T>(this T ch, string unlockedSpriteName, string lockedSpriteName = null, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var unlockedSprite = profile.LoadSprite(unlockedSpriteName);
            var lockedSprite = string.IsNullOrEmpty(lockedSpriteName) ? null : profile.LoadSprite(lockedSpriteName);

            return new(ch.name, unlockedSprite, lockedSprite != null ? lockedSprite : unlockedSprite);
        }

        public static SelectableCharacterData GenerateMenuCharacter<T>(this T ch, Sprite unlockedSprite, Sprite lockedSprite = null) where T : CharacterSO
        {
            return new(ch.name, unlockedSprite, lockedSprite != null ? lockedSprite : unlockedSprite);
        }

        public static T AddToDatabase<T>(this T selCh) where T : SelectableCharacterData
        {
            CharacterDB.SelectableCharacters.Add(selCh);

            return selCh;
        }

        public static T SetAsFullDPS<T>(this T selCh) where T : SelectableCharacterData
        {
            CharacterDB._dpsCharacters.Add(new(selCh.CharacterName), new([]));

            return selCh;
        }

        public static T SetAsFullSupport<T>(this T selCh) where T : SelectableCharacterData
        {
            CharacterDB._supportCharacters.Add(new(selCh.CharacterName), new([]));

            return selCh;
        }

        private static int _rank = -1;
    }
}
