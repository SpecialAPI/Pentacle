using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class AbilityUsageReferenceHolder(AbilityUsageReference abRef) : IUnitHolder
    {
        public AbilityUsageReference abRef = abRef;

        IUnit IUnitHolder.this[int index]
        {
            get
            {
                if(!CombatManager.Instance._stats.TryGetUnit(abRef.m_UnitID, abRef.m_IsUnitCharacter, out var unit))
                    return null;

                return unit;
            }
            set => PentacleLogger.LogWarning($"AbilityUsageReferenceHolder's Unit value is read-only.");
        }

        IUnit IUnitHolder.Value
        {
            get
            {
                if (!CombatManager.Instance._stats.TryGetUnit(abRef.m_UnitID, abRef.m_IsUnitCharacter, out var unit))
                    return null;

                return unit;
            }
            set => PentacleLogger.LogWarning($"AbilityUsageReferenceHolder's Unit value is read-only.");
        }
    }
}
