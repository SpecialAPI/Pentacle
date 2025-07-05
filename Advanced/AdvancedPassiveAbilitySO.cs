using Pentacle.Advanced.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// An extended version of the BasePassiveAbilitySO class.
    /// </summary>
    public abstract class AdvancedPassiveAbilitySO : BasePassiveAbilitySO, IMultipleStoredValueHolder
    {
        IEnumerable<UnitStoreData_BasicSO> IMultipleStoredValueHolder.GetDisplayedStoredValues()
        {
            return StoredValues ?? [];
        }

        /// <summary>
        /// A list of stored values displayed in this passive's combat tooltip. These are displayed in addition to BasePassiveAbilitySO.specialStoredData
        /// </summary>
        public List<UnitStoreData_BasicSO> StoredValues = [];
    }
}
