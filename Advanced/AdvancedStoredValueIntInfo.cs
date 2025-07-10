using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// A stored value class that has many tools for customizing how an integer stored value is displayed.
    /// </summary>
    public class AdvancedStoredValueIntInfo : UnitStoreData_BasicSO
    {
        /// <summary>
        /// The stored value's static format. Used in string.Format with the integer value being the first object.
        /// </summary>
        public string staticString = string.Empty;
        /// <summary>
        /// The stored value's dynamic string function. If this function isn't null, the string it returns will be used as the stored value's displayed string.
        /// <para>If it is null, staticString is used instead.</para>
        /// </summary>
        public Func<UnitStoreDataHolder, string> dynamicString;

        /// <summary>
        /// The static color used for this stored value's display.
        /// </summary>
        public Color color = Color.white;
        /// <summary>
        /// The stored value's dynamic color function. If this function isn't null, the color it returns will be used as the stored value's display color.
        /// <para>If it is null, color (the static color field) is used instead.</para>
        /// </summary>
        public Func<UnitStoreDataHolder, Color> dynamicColor;

        /// <summary>
        /// The basic integer condition for this stored value being displayed.
        /// </summary>
        public IntCondition displayCondition = IntCondition.Positive;
        /// <summary>
        /// The custom display condition function for this stored value being displayed. If this function isn't null, the stored value will be displayed when it returns true. This overrides displayCondition.
        /// <para>If it is null, displayCondition is used instead.</para>
        /// </summary>
        public Func<UnitStoreDataHolder, bool> customDisplayCondition;

        /// <summary>
        /// Determines how this stored value should be displayed.
        /// </summary>
        /// <param name="holder">The holder for this stored value's current value.</param>
        /// <param name="result">Determines what string should be displayed for this stored value.</param>
        /// <returns>True if this stored value should be displayed, false otherwise.</returns>
        public override bool TryGetUnitStoreDataToolTip(UnitStoreDataHolder holder, out string result)
        {
            var display = MeetsCondition(holder);

            result = display ? FormatStoredValue(holder) : string.Empty;

            if (string.IsNullOrWhiteSpace(result))
                display = false;

            return display;
        }

        /// <summary>
        /// Gets this stored value's display string with color formatting.
        /// </summary>
        /// <param name="holder">The holder for this stored value's current value.</param>
        /// <returns>The string display for this stored value with color formatting.</returns>
        public virtual string FormatStoredValue(UnitStoreDataHolder holder)
        {
            var str = GetString(holder);

            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return str.Colorize(GetColor(holder));
        }

        /// <summary>
        /// Gets this stored value's display string. This method doesn't apply color formatting to the display, FormatStoredValue() is used for that instead.
        /// </summary>
        /// <param name="holder">The holder for this stored value's current value.</param>
        /// <returns>The string display for this stored value without color formatting.</returns>
        public virtual string GetString(UnitStoreDataHolder holder)
        {
            return dynamicString?.Invoke(holder) ?? string.Format(staticString, holder.m_MainData);
        }

        /// <summary>
        /// Gets the display color for this stored value from its value.
        /// </summary>
        /// <param name="holder">The holder for this stored value's current value.</param>
        /// <returns>The display color for this stored value.</returns>
        public virtual Color GetColor(UnitStoreDataHolder holder)
        {
            return dynamicColor?.Invoke(holder) ?? color;
        }

        /// <summary>
        /// Checks if this stored value's display condition is met.
        /// </summary>
        /// <param name="holder">The holder for this stored value's current value.</param>
        /// <returns>True if this stored value should be displayed, false otherwise.</returns>
        public virtual bool MeetsCondition(UnitStoreDataHolder holder)
        {
            return customDisplayCondition?.Invoke(holder) ?? IntTools.MeetsIntCondition(holder.m_MainData, displayCondition);
        }
    }
}
