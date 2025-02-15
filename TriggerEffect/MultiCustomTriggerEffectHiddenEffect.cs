using Pentacle.Advanced;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// A Hidden Effect that can have any amount of effects.
    /// </summary>
    public class MultiCustomTriggerEffectHiddenEffect : HiddenEffectSO
    {
        public override bool Immediate => false;

        /// <summary>
        /// Effects that should be performed on triggers.
        /// </summary>
        public List<EffectsAndTrigger> triggerEffects;
        /// <summary>
        /// Effects that should be performed when this Hidden Effect is connected.
        /// </summary>
        public List<TriggeredEffect> connectionEffects;
        /// <summary>
        /// Effects that should be performed when this Hidden Effect is disconnected.
        /// </summary>
        public List<TriggeredEffect> disconnectionEffects;

        private readonly Dictionary<int, Action<object, object>> effectMethods = [];

        public override void OnConnected(IUnit unit)
        {
            if (connectionEffects == null)
                return;

            for (var i = 0; i < connectionEffects.Count; i++)
            {
                TryPerformHiddenEffectEffect(unit, null, i);
            }
        }

        public override void OnDisconnected(IUnit unit)
        {
            if (disconnectionEffects == null)
                return;

            for (var i = 0; i < disconnectionEffects.Count; i++)
                TryPerformHiddenEffectEffect(unit, null, i + (connectionEffects?.Count ?? 0));
        }

        public override void CustomOnTriggerAttached(IEffectorChecks caller)
        {
            if (triggerEffects == null)
                return;

            for (var i = 0; i < triggerEffects.Count; i++)
            {
                var te = triggerEffects[i];
                var strings = te.TriggerStrings();

                foreach (var str in strings)
                    CombatManager.Instance.AddObserver(GetEffectMethod(i + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)), str, caller);
            }
        }

        public override void CustomOnTriggerDettached(IEffectorChecks caller)
        {
            if (triggerEffects == null)
                return;

            for (var i = 0; i < triggerEffects.Count; i++)
            {
                var te = triggerEffects[i];
                var strings = te.TriggerStrings();

                foreach (var str in strings)
                    CombatManager.Instance.RemoveObserver(GetEffectMethod(i + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)), str, caller);
            }
        }

        public Action<object, object> GetEffectMethod(int i)
        {
            if (effectMethods.TryGetValue(i, out var existing))
                return existing;

            return effectMethods[i] = (sender, args) => TryPerformHiddenEffectEffect(sender, args, i);
        }

        public void TryPerformHiddenEffectEffect(object sender, object args, int index)
        {
            if (index >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IEffectorChecks effector)
                return;

            var te = GetEffectAtIndex(index, out var connect, out var disconnect);

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

            if (te.immediate || connect || disconnect)
                CustomTrigger(sender, args, index);

            else
                CombatManager.Instance.AddSubAction(new TriggerHiddenEffectCustomAction(this, sender, args, index));
        }

        public override void CustomTrigger(object sender, object args, int idx)
        {
            if (idx >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IUnit caster)
                return;

            var te = GetEffectAtIndex(idx, out _, out _);

            if (te == null)
                return;

            te.effect?.DoEffect(caster, args, te, this);
        }

        public TriggeredEffect GetEffectAtIndex(int idx, out bool connection, out bool disconnection)
        {
            connection = true;
            disconnection = false;

            if (connectionEffects != null && idx < connectionEffects.Count)
                return connectionEffects[idx];

            connection = false;
            disconnection = true;

            idx -= connectionEffects?.Count ?? 0;

            if (disconnectionEffects != null && idx < disconnectionEffects.Count)
                return disconnectionEffects[idx];

            disconnection = false;

            idx -= disconnectionEffects?.Count ?? 0;

            if (triggerEffects != null && idx < triggerEffects.Count)
                return triggerEffects[idx];

            return null;
        }
    }
}
