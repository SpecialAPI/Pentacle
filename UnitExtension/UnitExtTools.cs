using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.UnitExtension
{
    [HarmonyPatch]
    public static class UnitExtTools
    {
        private static UnitStoreData_BasicSO StoreData_UnitExt;

        internal static void BuildUnitExtData()
        {
            if (StoreData_UnitExt != null)
                return;

            var dat = StoreData_UnitExt = CreateScriptable<UnitStoreData_BasicSO>();

            dat.name = $"{MOD_PREFIX}_UnitExt_SV";
            dat._UnitStoreDataID = $"{MOD_PREFIX}_UnitExt_SV";

            MiscDB.AddNewUnitStoreData(dat.name, dat);
        }

        /// <summary>
        /// Gets the extended variables class for this unit.
        /// </summary>
        /// <param name="unit">The unit to get the extended class for.</param>
        /// <returns>Extended variables class for this unit.</returns>
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
