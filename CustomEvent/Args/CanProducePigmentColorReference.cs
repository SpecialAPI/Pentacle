using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class CanProducePigmentColorReference(ManaColorSO pigment, bool canProduce) : IBoolHolder
    {
        public ManaColorSO pigment = pigment;
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
