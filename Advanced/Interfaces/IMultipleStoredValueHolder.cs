using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.Interfaces
{
    public interface IMultipleStoredValueHolder
    {
        IEnumerable<UnitStoreData_BasicSO> GetDisplayedStoredValues();
    }
}
