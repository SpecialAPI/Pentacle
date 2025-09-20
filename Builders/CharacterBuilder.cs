using Pentacle.Advanced;
using Pentacle.HiddenPassiveEffects;
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
        /// Creates a new character using Pentacle's custom AdvancedCharacterSO class.
        /// </summary>
        /// <param name="id_CH">The string ID of the character. Naming convention: CharacterName_CH</param>
        /// <param name="entityId">The character's "entity ID". Entity IDs are used by the game to make sure the same character doesn't appear multiple times in the same run.<para>Entity IDs are also used for final boss unlocks.</para></param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created character.</returns>
        public static AdvancedCharacterSO NewCharacter(string id_CH, string entityId, ModProfile profile = null)
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            return NewCharacter<AdvancedCharacterSO>(id_CH, entityId, profile);
        }

        /// <summary>
        /// Creates a new character of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created character. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="id_CH">The string ID of the character. Naming convention: CharacterName_CH</param>
        /// <param name="entityId">The character's "entity ID". Entity IDs are used by the game to make sure the same character doesn't appear multiple times in the same run.<para>Entity IDs are also used for final boss unlocks.</para></param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created character.</returns>
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

        /// <summary>
        /// General method for setting the main traits of a character - name, health colour, and sprites.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <param name="healthColor">The pigment colour of the character's health.</param>
        /// <param name="frontSpriteName">The name of the image file for the character front-facing sprite (that shows when you select them in combat).<para />.png extension is optional.</param>
        /// <param name="backSpriteName">The name of the image file for the character back-facing sprite (the default combat sprite).<para />.png extension is optional.</param>
        /// <param name="overworldSpriteName">The name of the image file for the character overworld sprite.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
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

        /// <summary>
        /// General method for setting the main traits of a character - name, health colour, and sprites.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <param name="healthColor">The pigment colour of the character's health.</param>
        /// <param name="frontSprite">The Sprite object of the character front-facing sprite (that shows when you select them in combat).</param>
        /// <param name="backSprite">The Sprite object of the character back-facing sprite (the default combat sprite).</param>
        /// <param name="overworldSprite">The Sprite object of the character overworld sprite.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
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
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T SetName<T>(this T ch, string name) where T : CharacterSO
        {
            ch._characterName = name;

            return ch;
        }

        /// <summary>
        /// Sets the health colour of the character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="color">The pigment colour of the character's health.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T SetHealthColor<T>(this T ch, ManaColorSO color) where T : CharacterSO
        {
            ch.healthColor = color;

            return ch;
        }

        /// <summary>
        /// Sets all the basic relevant sprites of a character for overworld and in-battle use.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="frontSpriteName">The name of the image file for the character front-facing sprite (that shows when you select them in combat).<para />.png extension is optional.</param>
        /// <param name="backSpriteName">The name of the image file for the character back-facing sprite (the default combat sprite).<para />.png extension is optional.</param>
        /// <param name="overworldSpriteName">The name of the image file for the character overworld sprite.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
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

        /// <summary>
        /// Sets all the basic relevant sprites of a character for overworld and in-battle use.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="frontSprite">The Sprite object of the character front-facing sprite (that shows when you select them in combat).</param>
        /// <param name="backSprite">The Sprite object of the character back-facing sprite (the default combat sprite).</param>
        /// <param name="overworldSprite">The Sprite object of the character overworld sprite.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
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
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="damageSound">The name of the sound which plays on the character taking damage.</param>
        /// <param name="deathSound">The name of the sound which plays on the character dying.</param>
        /// <param name="dialogueSound">The name of the sound which plays on the character fleeing combat.<para>For all basegame characters this sound matches their dialogue sound, but this property doesn't determine the character's voice during dialogue.</para></param>
        /// <returns>The instance of the character, for method chaining.</returns>
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
        /// Adds a list of passive effects to the character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="passives">The passive abilities to add to the character, as BasePassiveAbilitySO objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T AddPassives<T>(this T ch, params BasePassiveAbilitySO[] passives) where T : CharacterSO
        {
            ch.passiveAbilities ??= [];
            ch.passiveAbilities.AddRange(passives);

            return ch;
        }

        /// <summary>
        /// Adds a list of CharacterRankedData objects as level data to the character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="rankedData">The level data to add, as CharacterRankedData objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T AddRankedData<T>(this T ch, params CharacterRankedData[] rankedData) where T : CharacterSO
        {
            ch.rankedData ??= [];
            ch.rankedData.AddRange(rankedData);

            return ch;
        }

        /// <summary>
        /// Sets whether the character physically moves during overworld transitions or just appears in the right place (like Leviat or Gospel)
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="moves">Whether the character should move during overworld transitions or not.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T SetMovesOnOverworld<T>(this T ch, bool moves) where T : CharacterSO
        {
            ch.movesOnOverworld = moves;

            return ch;
        }

        /// <summary>
        /// Sets up and adds level data to the character. This will remove all level data the character previously had.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="ranks">How many levels the character should have.</param>
        /// <param name="rankSetup">A delegate that sets up one level of the character.<para>The first argument is the 0-indexed level number, the second argumet is the 1-indexed level number. The delgate needs to return the CharacterRankedData object for the level given to it.</para></param>
        /// <returns>The instance of the character, for method chaining.</returns>
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

        /// <summary>
        /// Returns a value from the given array with an index equal to the current character level. Only works in RankedDataSetup.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <param name="rankValues">The array of values to choose from.</param>
        /// <returns>A value from the array for the current character level.</returns>
        public static T RankedValue<T>(params T[] rankValues)
        {
            return rankValues[Mathf.Clamp(_rank, 0, rankValues.Length - 1)];
        }

        /// <summary>
        /// Returns a value from the given array with an index equal to the given character level.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <param name="rank">The character level to choose a value for.</param>
        /// <param name="rankValues">The array of values to choose from.</param>
        /// <returns>A value from the array for the given character level.</returns>
        public static T RankedValueManual<T>(int rank, params T[] rankValues)
        {
            return rankValues[Mathf.Clamp(rank, 0, rankValues.Length - 1)];
        }

        /// <summary>
        /// Sets the "basic ability", i.e. slap or slap equivalent of the character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="basicAbility">The ability this character will use as a slap equivlaent.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T SetBasicAbility<T>(this T ch, CharacterAbility basicAbility) where T : CharacterSO
        {
            ch.basicCharAbility = basicAbility;

            return ch;
        }

        /// <summary>
        /// Sets whether the character uses all of its abilities (like Longliver) or has sets. Also sets whether the character uses slap (or a slap equivalent ability) or not.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="usesAllAbilities">If true, this character will use all abilities at once. Otherwise, this character will use sets.</param>
        /// <param name="usesBasicAbility">If true, this character will use slap (or a slap equivalent ability). Otherwise, this character will only use the abilities in their level data.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T SetAbilityUsage<T>(this T ch, bool usesAllAbilities, bool usesBasicAbility) where T : CharacterSO
        {
            ch.usesAllAbilities = usesAllAbilities;
            ch.usesBasicAbility = usesBasicAbility;

            return ch;
        }

        /// <summary>
        /// Adds a list of unit types to this character (such as being considered a fish).
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="unitTypes">The string unit types to add to the character.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T AddUnitTypes<T>(this T ch, params string[] unitTypes) where T : CharacterSO
        {
            ch.unitTypes ??= [];
            ch.unitTypes.AddRange(unitTypes);

            return ch;
        }

        /// <summary>
        /// Adds a list of hidden passive effects to the AvancedCharacterSO character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be AdvancedCharacterSO or a subclass of AdvancedCharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="hiddenEffects">The hidden passive effects to add to the character, as HiddenEffectSO objects.<para>Can either be given as an array or as infinitely repeatable arguments.</para></param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T AddHiddenEffects<T>(this T ch, params HiddenPassiveEffectSO[] hiddenEffects) where T : AdvancedCharacterSO
        {
            ch.hiddenEffects ??= [];
            ch.hiddenEffects.AddRange(hiddenEffects);

            return ch;
        }

        /// <summary>
        /// Adds unlock data for a final boss to this character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="bossId">The BossID of the final boss that the unlock is for.</param>
        /// <param name="unlock">The unlock data as a UnlockableModData object.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
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

        /// <summary>
        /// Adds the character to the Character Database.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="appearsInShops">Determines whether the character should appear in fool shops.</param>
        /// <param name="locked">Determines whether the character is locked by default.</param>
        /// <returns>The instance of the character, for method chaining.</returns>
        public static T AddToDatabase<T>(this T ch, bool appearsInShops = true, bool locked = false) where T : CharacterSO
        {
            ch.m_StartsLocked = locked;
            if(!appearsInShops)
                MiscDB.AddOmittedFoolToZones(ch.name);

            CharacterDB.AddNewCharacter(ch.name, ch);

            return ch;
        }

        /// <summary>
        /// Generates menu character data for the character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="unlockedSpriteName">The name of the image file for the unlocked menu sprite.<para />.png extension is optional.</param>
        /// <param name="lockedSpriteName">The name of the image file for the locked menu sprite.<para />.png extension is optional.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>The generated SelectableCharacterData object.</returns>
        public static SelectableCharacterData GenerateMenuCharacter<T>(this T ch, string unlockedSpriteName, string lockedSpriteName = null, ModProfile profile = null) where T : CharacterSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var unlockedSprite = profile.LoadSprite(unlockedSpriteName);
            var lockedSprite = string.IsNullOrEmpty(lockedSpriteName) ? null : profile.LoadSprite(lockedSpriteName);

            return new(ch.name, unlockedSprite, lockedSprite != null ? lockedSprite : unlockedSprite);
        }

        /// <summary>
        /// Generates menu character data for the character.
        /// </summary>
        /// <typeparam name="T">The character's custom type. Must either be CharacterSO or a subclass of CharacterSO.</typeparam>
        /// <param name="ch">The object instance of the character.</param>
        /// <param name="unlockedSprite">The Sprite object of the unlocked menu sprite.</param>
        /// <param name="lockedSprite">The Sprite object of the locked menu sprite.</param>
        /// <returns>The generated SelectableCharacterData object.</returns>
        public static SelectableCharacterData GenerateMenuCharacter<T>(this T ch, Sprite unlockedSprite, Sprite lockedSprite = null) where T : CharacterSO
        {
            return new(ch.name, unlockedSprite, lockedSprite != null ? lockedSprite : unlockedSprite);
        }

        /// <summary>
        /// Adds the menu character data to the menu character database.
        /// </summary>
        /// <typeparam name="T">The menu data's type. Must either be SelectableCharacterData or a subclass of SelectableCharacterData.</typeparam>
        /// <param name="selCh">The object instance of the menu character data.</param>
        /// <returns>The instance of the menu character data, for method chaining.</returns>
        public static T AddToDatabase<T>(this T selCh) where T : SelectableCharacterData
        {
            CharacterDB.SelectableCharacters.Add(selCh);

            return selCh;
        }

        /// <summary>
        /// Makes all sets of the character be considered as offense for selection bias purposes.
        /// </summary>
        /// <typeparam name="T">The menu data's type. Must either be SelectableCharacterData or a subclass of SelectableCharacterData.</typeparam>
        /// <param name="selCh">The object instance of the menu character data.</param>
        /// <returns>The instance of the menu character data, for method chaining.</returns>
        public static T SetAsFullDPS<T>(this T selCh) where T : SelectableCharacterData
        {
            CharacterDB._dpsCharacters.Add(new(selCh.CharacterName), new([]));

            return selCh;
        }

        /// <summary>
        /// Makes all sets of the character be considered as support for selection bias purposes.
        /// </summary>
        /// <typeparam name="T">The menu data's type. Must either be SelectableCharacterData or a subclass of SelectableCharacterData.</typeparam>
        /// <param name="selCh">The object instance of the menu character data.</param>
        /// <returns>The instance of the menu character data, for method chaining.</returns>
        public static T SetAsFullSupport<T>(this T selCh) where T : SelectableCharacterData
        {
            CharacterDB._supportCharacters.Add(new(selCh.CharacterName), new([]));

            return selCh;
        }

        private static int _rank = -1;
    }
}
