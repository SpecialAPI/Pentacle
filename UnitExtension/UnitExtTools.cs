using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.UnitExtension
{
    /// <summary>
    /// Static class that provides tools related to unit extension.
    /// </summary>
    [HarmonyPatch]
    public static class UnitExtTools
    {
        private static UnitStoreData_BasicSO StoreData_UnitExt;

        internal static void BuildUnitExtData()
        {
            if (StoreData_UnitExt != null)
                return;

            var dat = StoreData_UnitExt = CreateScriptable<UnitStoreData_BasicSO>();

            dat.name = $"{MOD_PREFIX}_UnitExt_USD";
            dat._UnitStoreDataID = $"{MOD_PREFIX}_UnitExt";

            UnitStoreData.AddCustom_Any_UnitStoreDataToPool(dat, dat._UnitStoreDataID);
        }


        /// <summary>
        /// Returns an object containing extended variables for a certain unit.
        /// </summary>
        /// <param name="unit">The unit to whose extended variables will be returned.</param>
        /// <returns>An object containing extended variables for the input unit.</returns>
        public static UnitExt Ext(this IUnit unit)
        {
            if(unit == null || unit.Equals(null))
                return null;

            if (StoreData_UnitExt == null)
                BuildUnitExtData();

            unit.TryGetStoredData(StoreData_UnitExt.name, out var hold, true);

            if (hold == null)
                return null; // WTF?

            return (hold.m_ObjectData ??= new UnitExt(unit)) as UnitExt;
        }
    }
}
