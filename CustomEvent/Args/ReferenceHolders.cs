using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    /// <summary>
    /// An interface for objects that hold one or more boolean values.
    /// </summary>
    public interface IBoolHolder
    {
        /// <summary>
        /// Gets or sets this boolean holder's primary value.
        /// </summary>
        bool Value { get; set; }
        /// <summary>
        /// Gets or sets a value from this boolean holder.
        /// </summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The current value with that index.</returns>
        bool this[int index] { get; set; }
    }

    /// <summary>
    /// An interface for objects that hold one or more integer values.
    /// </summary>
    public interface IIntHolder
    {
        /// <summary>
        /// Gets or sets this integer holder's primary value.
        /// </summary>
        int Value { get; set; }
        /// <summary>
        /// Gets or sets a value from this integer holder.
        /// </summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The current value with that index.</returns>
        int this[int index] { get; set; }
    }

    /// <summary>
    /// An interface for objects that hold one or more string values.
    /// </summary>
    public interface IStringHolder
    {
        /// <summary>
        /// Gets or sets this string holder's primary value.
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Gets or sets a value from this string holder.
        /// </summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The current value with that index.</returns>
        string this[int index] { get; set; }
    }

    /// <summary>
    /// An interface for objects that hold one or more IUnit values.
    /// </summary>
    public interface IUnitHolder
    {
        /// <summary>
        /// Gets or sets this unit holder's primary value.
        /// </summary>
        IUnit Value { get; set; }
        /// <summary>
        /// Gets or sets a value from this unit holder.
        /// </summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The current value with that index.</returns>
        IUnit this[int index] { get; set; }
    }

    /// <summary>
    /// An interface for "value change exceptions" - objects used to modify certain values, such as dealt/received damage or healing.
    /// </summary>
    public interface IValueChangeException
    {
        /// <summary>
        /// Determines if this value change exception counts as "damage dealt" or not. This is used by some int value modifiers to determine their order.
        /// <para>This should be true for value change exceptions related to dealing damage, healing, etc. and false for other value change exceptions.</para>
        /// </summary>
        bool DamageDealt { get; }

        /// <summary>
        /// Adds an int value modifier to this value change exception.
        /// </summary>
        /// <param name="modifier">The int value modifier to add.</param>
        void AddModifier(IntValueModifier modifier);
        /// <summary>
        /// Gets this value change change exception's modified value by applying all added int value modifiers to its initial value.
        /// </summary>
        /// <returns>The modified value of this value change exception.</returns>
        int GetModifiedValue();
    }
}
