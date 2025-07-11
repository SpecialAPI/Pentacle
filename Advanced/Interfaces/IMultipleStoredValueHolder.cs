using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.Interfaces
{
    /// <summary>
    /// An interface that allows abilities and passives to display multiple stored values at once.
    /// </summary>
    public interface IMultipleStoredValueHolder
    {
        /// <summary>
        /// Returns a list of stored values that should be displayed by this ability or passive. These stored values will be displayed in addition to the single stored value abilities and passives can normally display.
        /// </summary>
        /// <returns></returns>
        IEnumerable<UnitStoreData_BasicSO> GetDisplayedStoredValues();
    }
}
