using Pentacle.Advanced.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    public abstract class AdvancedPassiveAbilitySO : BasePassiveAbilitySO, IMultipleStoredValueHolder
    {
        IEnumerable<UnitStoreData_BasicSO> IMultipleStoredValueHolder.GetDisplayedStoredValues()
        {
            return StoredValues ?? [];
        }

        public List<UnitStoreData_BasicSO> StoredValues = [];
    }
}
