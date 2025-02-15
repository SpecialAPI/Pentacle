using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Effects
{
    public class ThreatenEffect : EffectSO
    {
        public bool directDamage = true;

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

                exitAmount += target.Threaten(amount, caster, targetOffset, directDamage, !directDamage);
            }

            if (exitAmount > 0 && directDamage)
                caster.DidApplyDamage(exitAmount);

            return exitAmount > 0;
        }
    }
}
