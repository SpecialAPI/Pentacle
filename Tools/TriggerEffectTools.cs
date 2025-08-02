using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class TriggerEffectTools
    {
        public static bool TryGetActivatorNameAndSprite(object activator, out string name, out Sprite sprite)
        {
            if (activator is BasePassiveAbilitySO pass)
            {
                name = pass.GetPassiveLocData().text;
                sprite = pass.passiveIcon;

                return true;
            }

            else if (activator is BaseWearableSO item)
            {
                name = item.GetItemLocData().text;
                sprite = item.wearableImage;

                return true;
            }

            name = "";
            sprite = null;

            return false;
        }
    }
}
