using Pentacle.Advanced;
using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating custom Fools.
    /// </summary>
    public static class CharacterBuilder
    {
        /// <summary>
        /// Creates a basic Character using the game's base character class.
        /// </summary>
        /// <param name="id_CH">The string ID of the character.</param>
        /// <param name="entityId">A second string ID for your character. Probably just best to set it the same as id_CH but, don't ask me why it exists but you need it.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created character.</returns>
        public static AdvancedCharacterSO NewCharacter(string id_CH, string entityId, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            return NewCharacter<AdvancedCharacterSO>(id_CH, entityId, profile);
        }
        /// <summary>
        /// Creates a character defined by a custom class behaviour.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="id_CH">The string ID of the character.</param>
        /// <param name="entityId">A second string ID for your character. Probably just best to set it the same as id_CH but, don't ask me why it exists but you need it.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created character.</returns>
        public static T NewCharacter<T>(string id_CH, string entityId, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

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
        /// <summary>
        /// General method for setting the main traits of a character - name, health colour, and sprites.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <param name="healthColor">The colour (and thus pigment production on direct damage) of the character's health.</param>
        /// <param name="frontSpriteName">The name of the image file for the character front-facing sprite (that shows when you select them in combat).<para />.png extension is optional.</param>
        /// <param name="backSpriteName">The name of the image file for the character back-facing sprite (the default combat sprite).<para />.png extension is optional.</param>
        /// <param name="overworldSpriteName">The name of the image file for the character overworld sprite.<para />.png extension is optional.</param>
        /// <param name="profile"></param>
        /// <returns>The object instance of the character with all the relevant information set.</returns>
        public static T SetBasicInformation<T>(this T ch, string name, ManaColorSO healthColor, string frontSpriteName, string backSpriteName, string overworldSpriteName, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            ch._characterName = name;
            ch.healthColor = healthColor;
            ch.characterSprite = profile.LoadSprite(frontSpriteName);
            ch.characterBackSprite = profile.LoadSprite(backSpriteName);
            ch.characterOWSprite = profile.LoadSprite(overworldSpriteName, new(0.5f, 0f));

            return ch;
        }
        /// <summary>
        /// General method for setting the main traits of a character - name, health colour, and sprites.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <param name="healthColor">The colour (and thus pigment production on direct damage) of the character's health.</param>
        /// <param name="frontSprite">The Sprite object of the character front-facing sprite (that shows when you select them in combat).</param>
        /// <param name="backSprite">The Sprite object of the character back-facing sprite (the default combat sprite).</param>
        /// <param name="overworldSprite">The Sprite object of the character overworld sprite.</param>
        /// <returns>The object instance of the character with all the relevant information set.</returns>
        public static T SetBasicInformation<T>(this T ch, string name, ManaColorSO healthColor, Sprite frontSprite, Sprite backSprite, Sprite overworldSprite) where T : CharacterSO
        {
            ch._characterName = name;
            ch.healthColor = healthColor;
            ch.characterSprite = frontSprite;
            ch.characterBackSprite = backSprite;
            ch.characterOWSprite = overworldSprite;

            return ch;
        }
        /// <summary>
        /// Sets the in-game name of the character.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <returns>The object instance of the character with the new name.</returns>
        public static T SetName<T>(this T ch, string name) where T : CharacterSO
        {
            ch._characterName = name;

            return ch;
        }
        /// <summary>
        /// Sets the health colour of the character.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="color">The health colour of the character.</param>
        /// <returns>The object instance of the character with the new health colour.</returns>
        public static T SetHealthColor<T>(this T ch, ManaColorSO color) where T : CharacterSO
        {
            ch.healthColor = color;

            return ch;
        }
        /// <summary>
        /// Sets all the basic relevant sprites of a character for overworld and in-battle use.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="frontSpriteName">The name of the image file for the character front-facing sprite (that shows when you select them in combat).<para />.png extension is optional.</param>
        /// <param name="backSpriteName">The name of the image file for the character back-facing sprite (the default combat sprite).<para />.png extension is optional.</param>
        /// <param name="overworldSpriteName">The name of the image file for the character overworld sprite.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the character with the new sprites.</returns>
        public static T SetSprites<T>(this T ch, string frontSpriteName, string backSpriteName, string overworldSpriteName, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            ch.characterSprite = profile.LoadSprite(frontSpriteName);
            ch.characterBackSprite = profile.LoadSprite(backSpriteName);
            ch.characterOWSprite = profile.LoadSprite(overworldSpriteName, new(0.5f, 0f));

            return ch;
        }
        /// <summary>
        /// Sets all the basic relevant sprites of a character for overworld and in-battle use.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="frontSprite">The Sprite object of the character front-facing sprite (that shows when you select them in combat).</param>
        /// <param name="backSprite">The Sprite object of the character back-facing sprite (the default combat sprite).</param>
        /// <param name="overworldSprite">The Sprite object of the character overworld sprite.</param>
        /// <returns>The instance of the character with the new sprites.</returns>
        public static T SetSprites<T>(this T ch, Sprite frontSprite, Sprite backSprite, Sprite overworldSprite) where T : CharacterSO
        {
            ch.characterSprite = frontSprite;
            ch.characterBackSprite = backSprite;
            ch.characterOWSprite = overworldSprite;

            return ch;
        }
        /// <summary>
        /// Sets the basic sound events for character related events.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="damageSound">The name of the sound which plays on the character taking damage.</param>
        /// <param name="deathSound">The name of the sound which plays on the character dying.</param>
        /// <param name="dialogueSound">The name of the sound which plays on the character speaking during dialogue.</param>
        /// <returns>The instance of the character with the new sounds.</returns>
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
        /// <summary>
        /// Adds a (list of) passive effects to the character.
        /// </summary>
        /// <typeparam name="T">A custom character type. Must be a type of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="passives">(Infinitely repeatable) The passive abilities of the character, as BasePassiveAbilitySO objects.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Sets whether the character physically moves during overworld transitions or just appears in the right place (like Leviat or Gospel)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ch"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Sets the "basic ability", i.e. slap or slap equivalent of the character.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ch"></param>
        /// <param name="basicAbility"></param>
        /// <returns></returns>
        public static T SetBasicAbility<T>(this T ch, CharacterAbility basicAbility) where T : CharacterSO
        {
            ch.basicCharAbility = basicAbility;

            return ch;
        }
        /// <summary>
        /// Sets whether the character uses all of its abilities (like Longliver) or uses a Slap or Slap equivalent in place of their 1st.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ch"></param>
        /// <param name="usesAllAbilities"></param>
        /// <param name="usesBasicAbility"></param>
        /// <returns></returns>
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
