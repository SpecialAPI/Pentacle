using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    public class AdvancedStoredValueIntInfo : UnitStoreData_BasicSO
    {
        public string staticString = string.Empty;
        public Func<UnitStoreDataHolder, string> dynamicString;

        public Color color = Color.white;
        public Func<UnitStoreDataHolder, Color> dynamicColor;

        public IntCondition displayCondition = IntCondition.Positive;
        public Func<UnitStoreDataHolder, bool> customDisplayCondition;

        public override bool TryGetUnitStoreDataToolTip(UnitStoreDataHolder holder, out string result)
        {
            var display = MeetsCondition(holder);

            result = display ? FormatStoredValue(holder) : string.Empty;

            if (string.IsNullOrWhiteSpace(result))
                display = false;

            return display;
        }

        public virtual string FormatStoredValue(UnitStoreDataHolder holder)
        {
            var str = GetString(holder);

            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return str.Colorize(GetColor(holder));
        }

        public virtual string GetString(UnitStoreDataHolder holder)
        {
            return dynamicString?.Invoke(holder) ?? string.Format(staticString, holder.m_MainData);
        }

        public virtual Color GetColor(UnitStoreDataHolder holder)
        {
            return dynamicColor?.Invoke(holder) ?? color;
        }

        public virtual bool MeetsCondition(UnitStoreDataHolder holder)
        {
            return customDisplayCondition?.Invoke(holder) ?? IntTools.MeetsIntCondition(holder.m_MainData, displayCondition);
        }
    }
}
