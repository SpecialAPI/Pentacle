using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public interface IBoolHolder
    {
        bool Value { get; set; }
        bool this[int index] { get; set; }
    }

    public interface IIntHolder
    {
        int Value { get; set; }
        int this[int index] { get; set; }
    }

    public interface IStringHolder
    {
        string Value { get; set; }
        string this[int index] { get; set; }
    }

    public interface IUnitHolder
    {
        IUnit Value { get; set; }
        IUnit this[int index] { get; set; }
    }

    public interface IValueChangeException
    {
        bool DamageDealt { get; }

        void AddModifier(IntValueModifier modifier);
        int GetModifiedValue();
    }
}
