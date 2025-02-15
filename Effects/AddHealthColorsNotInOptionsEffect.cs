using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Effects
{
    public class AddHealthColorsNotInOptionsEffect : EffectSO
    {
        public List<ManaColorSO> healthColors;

        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;

            if(healthColors == null || healthColors.Count <= 0)
                return false;

            foreach(var t in targets)
            {
                if(t == null || !t.HasUnit)
                    continue;

                var ext = t.Unit.Ext();

                foreach(var c in healthColors)
                {
                    if(ext.HealthColors.Exists(x => x.pigmentID == c.pigmentID))
                        continue;

                    ext.AddHealthColor(c);
                    exitAmount++;
                }
            }

            return exitAmount > 0;
        }
    }
}
