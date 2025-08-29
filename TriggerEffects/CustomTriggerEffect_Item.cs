using BrutalAPI.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects
{
    /// <summary>
    /// Builds an item that can have any amount of effects (<see cref="MultiCustomTriggerEffectWearable"/>).
    /// </summary>
    public class CustomTriggerEffect_Item : BaseItem
    {
        /// <summary>
        /// This item builder's MultiCustomTriggerEffect item object.
        /// </summary>
        public MultiCustomTriggerEffectWearable item;

        /// <inheritdoc/>
        public override BaseWearableSO Item => item;

        /// <summary>
        /// Gets or sets the item's list of effects that should be performed on certain triggers.
        /// </summary>
        public List<EffectsAndTrigger> TriggerEffects
        {
            get => item.triggerEffects;
            set => item.triggerEffects = value;
        }

        /// <summary>
        /// Creates a new MultiCustomTriggerEffectWearable item.
        /// </summary>
        /// <param name="itemId">The string ID of the item.<para>Naming convention depends on which loot pool the item is in. Shop pool: ItemName_SW, treasure pool: ItemName_TW, custom pools/no pool: ItemName_ExtraW</para></param>
        /// <param name="triggerEffects">A list of effects that the item should perform on certain triggers. Defaults to an empty list if null.</param>
        public CustomTriggerEffect_Item(string itemId = "DefaultID_Item", List<EffectsAndTrigger> triggerEffects = null)
        {
            item = CreateScriptable<MultiCustomTriggerEffectWearable>();
            item.triggerEffects = triggerEffects ?? [];

            InitializeItemData(itemId);
        }
    }
}
