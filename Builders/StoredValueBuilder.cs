using Pentacle.Advanced;
using Pentacle.Tools;
using System;
using UnityEngine.Profiling;

namespace Pentacle.Builders
{
    public static class StoredValueBuilder
    {
        public const string StoredValueInsert_PlusMinusPlusZero = "{0:+#;-#;+0}";
        public const string StoredValueInsert_PlusMinusMinusZero = "{0:+#;-#;-0}";
        public const string StoredValueInsert_PlusMinusZero = "{0:+#;-#;0}";

        public static Color StoredValueColor_Positive = new(0f, 0.5961f, 0.8667f);
        public static Color StoredValueColor_Negative = new(0.8667f, 0f, 0.2157f);
        public static Color StoredValueColor_Rare = new(0.7725f, 0.2667f, 0.8196f);

        public static T NewStoredValue<T>(string id_USD, string storedValueId, ModProfile profile = null) where T : UnitStoreData_BasicSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());

            var sv = CreateScriptable<T>();

            sv.name = profile.GetID(id_USD);
            sv._UnitStoreDataID = profile.GetID(storedValueId);

            UnitStoreData.AddCustom_Any_UnitStoreDataToPool(sv, sv._UnitStoreDataID);
            return sv;
        }

        public static T SetFormat<T>(this T sv, string format) where T : AdvancedStoredValueIntInfo
        {
            sv.staticString = format;

            return sv;
        }

        public static T SetDynamicString<T>(this T sv, Func<UnitStoreDataHolder, string> dynamicString) where T : AdvancedStoredValueIntInfo
        {
            sv.dynamicString = dynamicString;

            return sv;
        }

        public static T SetColor<T>(this T sv, Color color) where T : AdvancedStoredValueIntInfo
        {
            sv.color = color;

            return sv;
        }

        public static T SetDynamicColor<T>(this T sv, Func<UnitStoreDataHolder, Color> dynamicColor) where T : AdvancedStoredValueIntInfo
        {
            sv.dynamicColor = dynamicColor;

            return sv;
        }

        public static T SetDisplayCondition<T>(this T sv, IntCondition condition) where T : AdvancedStoredValueIntInfo
        {
            sv.displayCondition = condition;

            return sv;
        }

        public static T SetCustomDisplayCondition<T>(this T sv, Func<UnitStoreDataHolder, bool> customCondition) where T : AdvancedStoredValueIntInfo
        {
            sv.customDisplayCondition = customCondition;

            return sv;
        }
    }
}
