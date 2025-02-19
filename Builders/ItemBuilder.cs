using System;
using System.Collections.Generic;
using System.Text;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Pentacle.Builders
{
    public static class ItemBuilder
    {
        public static T NewItem<T>(string id, Assembly callingAssembly = null) where T : BaseWearableSO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            if (!ProfileManager.TryGetProfile(callingAssembly, out var profile))
                return null;

            var w = CreateScriptable<T>();
            w.name = profile.GetID(id);
            w.staticModifiers = [];

            return w;
        }

        public static T SetName<T>(this T w, string name) where T : BaseWearableSO
        {
            w._itemName = name;

            return w;
        }

        public static T SetFlavor<T>(this T w, string flavor) where T : BaseWearableSO
        {
            w._flavourText = flavor;

            return w;
        }

        public static T SetDescription<T>(this T w, string description) where T : BaseWearableSO
        {
            w._description = description;

            return w;
        }

        public static T SetSprite<T>(this T w, string spriteName, Assembly callingAssembly = null) where T : BaseWearableSO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();
            w.wearableImage = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return w;
        }

        public static T SetSprite<T>(this T w, Sprite sprite) where T : BaseWearableSO
        {
            w.wearableImage = sprite;

            return w;
        }

        public static T SetBasicInformation<T>(this T w, string name, string flavor, string description, string spriteName, Assembly callingAssembly = null) where T : BaseWearableSO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();

            w._itemName = name;
            w._flavourText = flavor;
            w._description = description;
            w.wearableImage = ResourceLoader.LoadSprite(spriteName, assembly: callingAssembly);

            return w;
        }

        public static T SetBasicInformation<T>(this T w, string name, string flavor, string description, Sprite sprite) where T : BaseWearableSO
        {
            w._itemName = name;
            w._flavourText = flavor;
            w._description = description;
            w.wearableImage = sprite;

            return w;
        }

        public static T SetPrice<T>(this T w, int price) where T : BaseWearableSO
        {
            w.shopPrice = price;

            return w;
        }

        public static T SetStaticModifiers<T>(this T w, params WearableStaticModifierSetterSO[] modifiers) where T : BaseWearableSO
        {
            w.staticModifiers = modifiers ?? [];

            return w;
        }

        public static T AddToTreasure<T>(this T w, Sprite lockedSprite = null, string achievementId = "") where T : BaseWearableSO
        {
            ItemUtils.AddItemToTreasureStatsCategoryAndGamePool(w, new(w.name, lockedSprite, achievementId));

            return w;
        }

        public static T AddToTreasure<T>(this T w, string lockedSpriteName, string achievementId = "", Assembly callingAssembly = null) where T : BaseWearableSO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();
            ItemUtils.AddItemToTreasureStatsCategoryAndGamePool(w, new(w.name, ResourceLoader.LoadSprite(lockedSpriteName, assembly: callingAssembly), achievementId));

            return w;
        }

        public static T AddToShop<T>(this T w, Sprite lockedSprite = null, string achievementId = "") where T : BaseWearableSO
        {
            ItemUtils.AddItemToShopStatsCategoryAndGamePool(w, new(w.name, lockedSprite, achievementId));

            return w;
        }

        public static T AddToShop<T>(this T w, string lockedSpriteName, string achievementId = "", Assembly callingAssembly = null) where T : BaseWearableSO
        {
            callingAssembly ??= Assembly.GetCallingAssembly();
            ItemUtils.AddItemToShopStatsCategoryAndGamePool(w, new(w.name, ResourceLoader.LoadSprite(lockedSpriteName, assembly: callingAssembly), achievementId));

            return w;
        }

        public static ExtraAbility_Wearable_SMS ExtraAbilityModifier(CharacterAbility ab)
        {
            var mod = CreateScriptable<ExtraAbility_Wearable_SMS>();
            mod._extraAbility = ab;

            return mod;
        }

        public static BasicAbilityChange_Wearable_SMS BasicAbilityModifier(CharacterAbility ab)
        {
            var mod = CreateScriptable<BasicAbilityChange_Wearable_SMS>();
            mod._basicAbility = ab;

            return mod;
        }

        public static ModdedDataSetter_Wearable_SMS ModdedDataModifier(ItemModifierDataSetter data, string id = null)
        {
            var mod = CreateScriptable<ModdedDataSetter_Wearable_SMS>();
            mod.m_ModdedData = data;
            mod.m_ModdedDataID = id ?? data.GetType().Name;

            return mod;
        }

        public static ModdedDataSetter_Wearable_SMS ModdedDataModifier<T>(string id = null) where T : ItemModifierDataSetter, new()
        {
            var mod = CreateScriptable<ModdedDataSetter_Wearable_SMS>();
            mod.m_ModdedData = new T();
            mod.m_ModdedDataID = id ?? typeof(T).Name;

            return mod;
        }

        public static RankChange_Wearable_SMS RankChangeModifier(int rankAddition)
        {
            var mod = CreateScriptable<RankChange_Wearable_SMS>();
            mod._rankAdditive = rankAddition;

            return mod;
        }

        public static MainCharacter_Wearable_SMS MainCharacterModifier()
        {
            return CreateScriptable<MainCharacter_Wearable_SMS>();
        }

        public static MaxHealthChange_Wearable_SMS MaxHealthModifier(int maxHealthChange, bool changeIsPercentage = false)
        {
            var mod = CreateScriptable<MaxHealthChange_Wearable_SMS>();
            mod.maxHealthChange = maxHealthChange;
            mod.isChangePercentage = changeIsPercentage;

            return mod;
        }

        public static HealthColorChange_Wearable_SMS HealthColorModifier(ManaColorSO healthColor)
        {
            var mod = CreateScriptable<HealthColorChange_Wearable_SMS>();
            mod._healthColor = healthColor;

            return mod;
        }

        public static ExtraPassiveAbility_Wearable_SMS ExtraPassiveModifier(BasePassiveAbilitySO passive)
        {
            var mod = CreateScriptable<ExtraPassiveAbility_Wearable_SMS>();
            mod._extraPassiveAbility = passive;

            return mod;
        }

        public static AbilitiesUsageChange_Wearable_SMS UsedAbilitiesChangeModifier(bool usesBasicAbility, bool usesAllAbilities)
        {
            var mod = CreateScriptable<AbilitiesUsageChange_Wearable_SMS>();
            mod._usesBasicAbility = usesBasicAbility;
            mod._usesAllAbilities = usesAllAbilities;

            return mod;
        }

        public static CurrencyMultiplierChange_Wearable_SMS CurrencyMultiplierModifier(int currencyMultAddition)
        {
            var mod = CreateScriptable<CurrencyMultiplierChange_Wearable_SMS>();
            mod._currencyMultiplier = currencyMultAddition;

            return mod;
        }
    }
}
