using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    public interface ITriggerEffectHandler
    {
        string DisplayedName { get; }
        Sprite Sprite { get; }

        bool TryGetPopupUIAction(int unitId, bool isUnitCharacter, bool consumed, out CombatAction action);
    }
}
