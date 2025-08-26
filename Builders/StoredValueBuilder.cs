using Pentacle.Advanced;
using Pentacle.Tools;
using System;
using UnityEngine.Profiling;

namespace Pentacle.Builders
{
    /// <summary>
    /// Static class which provides tools for creating stored values.
    /// </summary>
    public static class StoredValueBuilder
    {
        /// <summary>
        /// String that can be used in a stored value's display format in order to insert its current value as part of the display, including the value's sign. E.g. $"The value is currently {StoredValueInsert_PlusMinusPlusZero}".<para>If the value is 0, it will be inserted as +0.</para>
        /// </summary>
        public const string StoredValueInsert_PlusMinusPlusZero = "{0:+#;-#;+0}";
        /// <summary>
        /// String that can be used in a stored value's display format in order to insert its current value as part of the display, including the value's sign. E.g. $"The value is currently {StoredValueInsert_PlusMinusMinusZero}".<para>If the value is 0, it will be inserted as -0.</para>
        /// </summary>
        public const string StoredValueInsert_PlusMinusMinusZero = "{0:+#;-#;-0}";
        /// <summary>
        /// String that can be used in a stored value's display format in order to insert its current value as part of the display, including the value's sign. E.g. $"The value is currently {StoredValueInsert_PlusMinusZero}".<para>If the value is 0, it will be inserted as 0.</para>
        /// </summary>
        public const string StoredValueInsert_PlusMinusZero = "{0:+#;-#;0}";

        /// <summary>
        /// The basegame color for "positive" stored values such as infestation and Arnold's damage and healing buffs.
        /// </summary>
        public static Color StoredValueColor_Positive = new(0f, 0.5961f, 0.8667f);
        /// <summary>
        /// The basegame color for "negative" stored values such as Burnout's damage decrease and the count for Blood Breathing Bomb.
        /// </summary>
        public static Color StoredValueColor_Negative = new(0.8667f, 0f, 0.2157f);
        /// <summary>
        /// The basegame color for "rare" stored values such as parasite health.
        /// </summary>
        public static Color StoredValueColor_Rare = new(0.7725f, 0.2667f, 0.8196f);

        /// <summary>
        /// Creates a new stored value info of the given custom class.
        /// </summary>
        /// <typeparam name="T">The custom type for the created stored value info. Must either be UnitStoreData_BasicSO or a subclass of UnitStoreData_BasicSO.</typeparam>
        /// <param name="id_USD">The string id of the stored value info. Naming convention: StoredValueName_USD</param>
        /// <param name="storedValueId">The database id of the stored value info.</param>
        /// <param name="profile">Your mod profile.</param>
        /// <returns>An object instance of the created stored value info</returns>
        public static T NewStoredValue<T>(string id_USD, string storedValueId, ModProfile profile = null) where T : UnitStoreData_BasicSO
        {
            profile ??= ProfileManager.GetProfile(Assembly.GetCallingAssembly());
            if (!ProfileManager.EnsureProfileExists(profile))
                return null;

            var sv = CreateScriptable<T>();

            sv.name = profile.GetID(id_USD);
            sv._UnitStoreDataID = profile.GetID(storedValueId);

            UnitStoreData.AddCustom_Any_UnitStoreDataToPool(sv, sv._UnitStoreDataID);
            return sv;
        }

        /// <summary>
        /// Sets the display format for the AdvancedStoredValueIntInfo stored value info.
        /// </summary>
        /// <typeparam name="T">The stored value info's custom type. Must either be AdvancedStoredValueIntInfo or a subclass of AdvancedStoredValueIntInfo.</typeparam>
        /// <param name="sv">The object instance of the stored value info.</param>
        /// <param name="format">The new display format for the stored value info.<para>{0} is replaced by the stored value's current value. You can insert the values for StoredValueInsert_PlusMinusPlusZero, StoredValueInsert_PlusMinusMinusZero or StoredValueInsert_PlusMinusZero instead to change how the value gets displayed.</para></param>
        /// <returns>The instance of the stored value info, for method chaining.</returns>
        public static T SetFormat<T>(this T sv, string format) where T : AdvancedStoredValueIntInfo
        {
            sv.staticString = format;

            return sv;
        }

        /// <summary>
        /// Sets the dynamic display for the AdvancedStoredValueIntInfo stored value info.
        /// </summary>
        /// <typeparam name="T">The stored value info's custom type. Must either be AdvancedStoredValueIntInfo or a subclass of AdvancedStoredValueIntInfo.</typeparam>
        /// <param name="sv">The object instance of the stored value info.</param>
        /// <param name="dynamicString">A delegate that returns the stored value's display based on its stored data.</param>
        /// <returns>The instance of the stored value info, for method chaining.</returns>
        public static T SetDynamicString<T>(this T sv, Func<UnitStoreDataHolder, string> dynamicString) where T : AdvancedStoredValueIntInfo
        {
            sv.dynamicString = dynamicString;

            return sv;
        }

        /// <summary>
        /// Sets the display color for the AdvancedStoredValueIntInfo stored value info.
        /// </summary>
        /// <typeparam name="T">The stored value info's custom type. Must either be AdvancedStoredValueIntInfo or a subclass of AdvancedStoredValueIntInfo.</typeparam>
        /// <param name="sv">The object instance of the stored value info.</param>
        /// <param name="color">The new display color for the stored value info.</param>
        /// <returns>The instance of the stored value info, for method chaining.</returns>
        public static T SetColor<T>(this T sv, Color color) where T : AdvancedStoredValueIntInfo
        {
            sv.color = color;

            return sv;
        }

        /// <summary>
        /// Sets the dynamic display color for the AdvancedStoredValueIntInfo stored value info.
        /// </summary>
        /// <typeparam name="T">The stored value info's custom type. Must either be AdvancedStoredValueIntInfo or a subclass of AdvancedStoredValueIntInfo.</typeparam>
        /// <param name="sv">The object instance of the stored value info.</param>
        /// <param name="dynamicColor">A delegate that returns the stored value's display color based on its stored data.</param>
        /// <returns>The instance of the stored value info, for method chaining.</returns>
        public static T SetDynamicColor<T>(this T sv, Func<UnitStoreDataHolder, Color> dynamicColor) where T : AdvancedStoredValueIntInfo
        {
            sv.dynamicColor = dynamicColor;

            return sv;
        }

        /// <summary>
        /// Sets a basic display condition for the stored value info.
        /// </summary>
        /// <typeparam name="T">The stored value info's custom type. Must either be AdvancedStoredValueIntInfo or a subclass of AdvancedStoredValueIntInfo.</typeparam>
        /// <param name="sv">The object instance of the stored value info.</param>
        /// <param name="condition">The new basic display condition type for the stored value info.</param>
        /// <returns>The instance of the stored value info, for method chaining.</returns>
        public static T SetDisplayCondition<T>(this T sv, IntCondition condition) where T : AdvancedStoredValueIntInfo
        {
            sv.displayCondition = condition;

            return sv;
        }

        /// <summary>
        /// Sets a custom display condition for the stored value info.
        /// </summary>
        /// <typeparam name="T">The stored value info's custom type. Must either be AdvancedStoredValueIntInfo or a subclass of AdvancedStoredValueIntInfo.</typeparam>
        /// <param name="sv">The object instance of the stored value info.</param>
        /// <param name="customCondition">A delegate that determines if the stored value should be displayed based on its stored data.<para>If this delegate returns true, the stored value will be displayed. Otherwise, the stored value will not be displayed.</para></param>
        /// <returns>The instance of the stored value info, for method chaining.</returns>
        public static T SetCustomDisplayCondition<T>(this T sv, Func<UnitStoreDataHolder, bool> customCondition) where T : AdvancedStoredValueIntInfo
        {
            sv.customDisplayCondition = customCondition;

            return sv;
        }
    }
}
