using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    /// <summary>
    /// Used to modify whether a certain pigment color can be produced or not.
    /// <para>Sent as args by CustomTriggers.CanProducePigmentColor.</para>
    /// </summary>
    /// <param name="pigment"></param>
    /// <param name="canProduce"></param>
    public class CanProducePigmentColorReference(ManaColorSO pigment, bool canProduce) : IBoolHolder
    {
        /// <summary>
        /// The pigment color whose ability to be produced is being modified.
        /// </summary>
        public ManaColorSO pigment = pigment;
        /// <summary>
        /// Whether the pigment color can currently be produced or not. This value can be changed to modify whether that pigment color is producible or not.
        /// </summary>
        public bool canProduce = canProduce;

        bool IBoolHolder.this[int index]
        {
            get => canProduce;
            set => canProduce = value;
        }
        bool IBoolHolder.Value
        {
            get => canProduce;
            set => canProduce = value;
        }
    }
}
