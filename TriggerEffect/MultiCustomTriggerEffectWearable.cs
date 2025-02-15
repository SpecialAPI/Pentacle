using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// An item that can have any amount of effects.
    /// </summary>
    public class MultiCustomTriggerEffectWearable : BaseWearableSO
    {
        /// <summary>
        /// This item's effects.
        /// </summary>
        public List<EffectsAndTrigger> triggerEffects;
        /// <summary>
        /// Effects that should be performed when this item is connected.
        /// </summary>
        public List<TriggeredEffect> connectionEffects;
        /// <summary>
        /// Effects that should be performed when this item is disconnected.
        /// </summary>
        public List<TriggeredEffect> disconnectionEffects;

        public override bool IsItemImmediate => false;
        public override bool DoesItemTrigger => false;

        private readonly Dictionary<int, Action<object, object>> effectMethods = [];

        public override void OnTriggerAttachedAction(IWearableEffector caller)
        {
            if (connectionEffects == null)
                return;

            for (var i = 0; i < connectionEffects.Count; i++)
            {
                TryPerformItemEffect(caller, null, i);
            }
        }

        public override void OnTriggerDettachedAction(IWearableEffector caller)
        {
            if (disconnectionEffects == null)
                return;

            for (var i = 0; i < disconnectionEffects.Count; i++)
            {
                TryPerformItemEffect(caller, null, i + (connectionEffects?.Count ?? 0));
            }
        }

        public override void CustomOnTriggerAttached(IWearableEffector caller)
        {
            if (triggerEffects == null)
                return;

            for (var i = 0; i < triggerEffects.Count; i++)
            {
                var te = triggerEffects[i];
                var strings = te.TriggerStrings();

                foreach (var str in strings)
                {
                    CombatManager.Instance.AddObserver(GetEffectMethod(i + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)), str, caller);
                }
            }
        }

        public override void CustomOnTriggerDettached(IWearableEffector caller)
        {
            if (triggerEffects == null)
                return;

            for (var i = 0; i < triggerEffects.Count; i++)
            {
                var te = triggerEffects[i];
                var strings = te.TriggerStrings();

                foreach (var str in strings)
                {
                    CombatManager.Instance.RemoveObserver(GetEffectMethod(i + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)), str, caller);
                }
            }
        }

        public Action<object, object> GetEffectMethod(int i)
        {
            if (effectMethods.TryGetValue(i, out var existing))
                return existing;

            return effectMethods[i] = (sender, args) => TryPerformItemEffect(sender, args, i);
        }

        public void TryPerformItemEffect(object sender, object args, int index)
        {
            if (index >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IWearableEffector effector || !effector.CanWearableTrigger)
                return;

            var te = GetEffectAtIndex(index);

            if (te == null)
                return;

            if (te.conditions != null)
            {
                foreach (var cond in te.conditions)
                {
                    if (!cond.MeetCondition(effector, args))
                        return;
                }
            }

            if (te.immediate)
                FinalizeCustomTriggerItem(sender, args, index);

            else
                CombatManager.Instance.AddSubAction(new PerformItemCustomAction(this, sender, args, index));
        }

        public override void FinalizeCustomTriggerItem(object sender, object args, int idx)
        {
            if (idx >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IWearableEffector effector || sender is not IUnit caster || effector.IsWearableConsumed)
                return;

            var te = GetEffectAtIndex(idx);

            if (te == null)
                return;

            var consumed = te.getsConsumed;

            if (consumed)
                effector.ConsumeWearable();

            if (te.doesPopup)
                CombatManager.Instance.AddUIAction(new ShowItemInformationUIAction(effector.ID, GetItemLocData().text, consumed, wearableImage));

            te.effect?.DoEffect(caster, args, te, this);
        }

        public TriggeredEffect GetEffectAtIndex(int idx)
        {
            if (connectionEffects != null && idx < connectionEffects.Count)
                return connectionEffects[idx];

            idx -= connectionEffects?.Count ?? 0;

            if (disconnectionEffects != null && idx < disconnectionEffects.Count)
                return disconnectionEffects[idx];

            idx -= disconnectionEffects?.Count ?? 0;

            if (triggerEffects != null && idx < triggerEffects.Count)
                return triggerEffects[idx];

            return null;
        }
    }
}
