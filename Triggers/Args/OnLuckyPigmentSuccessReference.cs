using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Provides information about how much lucky pigment was produced on a successful lucky pigment roll.
    /// <para>Sent as args by CustomTriggers.OnLuckyPigmentSuccess.</para>
    /// </summary>
    /// <param name="luckyPigmentAmount">The amount of lucky pigment that was produced.</param>
    public class OnLuckyPigmentSuccessReference(int luckyPigmentAmount) : IIntHolder
    {
        /// <summary>
        /// The amount of lucky pigment that was produced.
        /// </summary>
        public readonly int luckyPigmentAmount = luckyPigmentAmount;

        int IIntHolder.this[int index]
        {
            get => luckyPigmentAmount;
            set => PentacleLogger.LogWarning($"OnLuckyPigmentSuccessReference's int value is read-only.");
        }
        int IIntHolder.Value
        {
            get => luckyPigmentAmount;
            set => PentacleLogger.LogWarning($"OnLuckyPigmentSuccessReference's int value is read-only.");
        }
    }
}
