using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Effects
{
    /// <summary>
    /// An effect that deals fake damage to all targets equal to entryVariable.
    /// <para>See <see cref="IUnitExtensions.FakeDamage"/> for more information.</para>
    /// </summary>
    public class FakeDamageEffect : EffectSO
    {
        /// <summary>
        /// Determines if the fake damage should be direct or indirect.
        /// </summary>
        public bool directDamage = true;

        /// <inheritdoc/>
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;

            foreach(var t in targets)
            {
                if(t == null || !t.HasUnit)
                    continue;

                var target = t.Unit;
                var amount = entryVariable;

                if (directDamage)
                    amount = caster.WillApplyDamage(amount, target);

                var targetOffset = -1;
                if (areTargetSlots)
                    targetOffset = t.SlotID - target.SlotID;

                exitAmount += target.FakeDamage(amount, caster, targetOffset, directDamage, !directDamage);
            }

            if (exitAmount > 0 && directDamage)
                caster.DidApplyDamage(exitAmount);

            return exitAmount > 0;
        }
    }
}
