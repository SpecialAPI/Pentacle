using Pentacle.Advanced.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// An extended version of the AbilitySO class.
    /// </summary>
    public class AdvancedAbilitySO : AbilitySO, IMultipleStoredValueHolder
    {
        IEnumerable<UnitStoreData_BasicSO> IMultipleStoredValueHolder.GetDisplayedStoredValues()
        {
            return StoredValues ?? [];
        }

        /// <summary>
        /// A list of stored values displayed in this ability's combat tooltip. These are displayed in addition to AbilitySO.specialStoredData
        /// </summary>
        public List<UnitStoreData_BasicSO> StoredValues = [];
    }
}
