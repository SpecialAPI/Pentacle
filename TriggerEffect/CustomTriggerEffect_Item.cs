using BrutalAPI.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// An item that can have any amount of effects.
    /// </summary>
    public class CustomTriggerEffect_Item : BaseItem
    {
        /// <summary>
        /// This item builder's item object.
        /// </summary>
        public MultiCustomTriggerEffectWearable item;

        public override BaseWearableSO Item => item;

        /// <summary>
        /// Gets or sets this item's effects.
        /// </summary>
        public List<EffectsAndTrigger> TriggerEffects
        {
            get => item.triggerEffects;
            set => item.triggerEffects = value;
        }

        /// <summary>
        /// Creates a new MultiCustomTriggerEffectWearable item with itemId as its id and optionally gives it effects.
        /// </summary>
        /// <param name="itemId">The item's internal id.</param>
        /// <param name="triggerEffects">A list of this item's effects</param>
        public CustomTriggerEffect_Item(string itemId = "DefaultID_Item", List<EffectsAndTrigger> triggerEffects = null)
        {
            item = CreateScriptable<MultiCustomTriggerEffectWearable>();
            item.triggerEffects = triggerEffects ?? [];

            InitializeItemData(itemId);
        }
    }
}
