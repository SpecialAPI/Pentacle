using Pentacle.Advanced;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// A hidden passive effect that can have any amount of trigger-, connection- and disconnection-activated effects, using Pentacle's trigger effect system.
    /// </summary>
    public class MultiCustomTriggerEffectHiddenEffect : HiddenEffectSO, ITriggerEffectHandler
    {
        /// <inheritdoc/>
        public override bool Immediate => false;

        string ITriggerEffectHandler.DisplayedName => string.Empty;
        Sprite ITriggerEffectHandler.Sprite => null;

        /// <summary>
        /// Trigger effects that should be performed on certain triggers.
        /// </summary>
        public List<EffectsAndTrigger> triggerEffects;
        /// <summary>
        /// Trigger effects that should be performed when this hidden passive effect is connected to a unit.
        /// </summary>
        public List<TriggeredEffect> connectionEffects;
        /// <summary>
        /// Trigger effects that should be performed when this hidden passive effect is disconnected from a unit.
        /// </summary>
        public List<TriggeredEffect> disconnectionEffects;

        private readonly Dictionary<int, Action<object, object>> effectMethods = [];

        /// <inheritdoc/>
        public override void OnConnected(IUnit unit)
        {
            if (connectionEffects == null)
                return;

            for (var i = 0; i < connectionEffects.Count; i++)
            {
                TryPerformHiddenEffectEffect(unit, null, i);
            }
        }

        /// <inheritdoc/>
        public override void OnDisconnected(IUnit unit)
        {
            if (disconnectionEffects == null)
                return;

            for (var i = 0; i < disconnectionEffects.Count; i++)
                TryPerformHiddenEffectEffect(unit, null, i + (connectionEffects?.Count ?? 0));
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        private Action<object, object> GetEffectMethod(int i)
        {
            if (effectMethods.TryGetValue(i, out var existing))
                return existing;

            return effectMethods[i] = (sender, args) => TryPerformHiddenEffectEffect(sender, args, i);
        }

        private void TryPerformHiddenEffectEffect(object sender, object args, int index)
        {
            if (index >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IEffectorChecks effector)
                return;

            var te = GetEffectAtIndex(index, out _);

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
                CustomTrigger(sender, args, index);

            else
                CombatManager.Instance.AddSubAction(new TriggerHiddenEffectCustomAction(this, sender, args, index));
        }

        /// <inheritdoc/>
        public override void CustomTrigger(object sender, object args, int idx)
        {
            if (idx >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IUnit caster)
                return;

            var te = GetEffectAtIndex(idx, out var activation);

            if (te == null)
                return;

            te.effect?.DoEffect(caster, args, te, new()
            {
                handler = this,
                activation = activation
            });
        }

        /// <inheritdoc/>
        private TriggeredEffect GetEffectAtIndex(int idx, out TriggerEffectActivation activation)
        {
            activation = TriggerEffectActivation.Connection;

            if (connectionEffects != null && idx < connectionEffects.Count)
                return connectionEffects[idx];

            activation = TriggerEffectActivation.Disconnection;
            idx -= connectionEffects?.Count ?? 0;

            if (disconnectionEffects != null && idx < disconnectionEffects.Count)
                return disconnectionEffects[idx];

            activation = TriggerEffectActivation.Trigger;
            idx -= disconnectionEffects?.Count ?? 0;

            if (triggerEffects != null && idx < triggerEffects.Count)
                return triggerEffects[idx];

            return null;
        }

        bool ITriggerEffectHandler.TryGetPopupUIAction(int unitId, bool isUnitCharacter, bool consumed, out CombatAction action)
        {
            action = null;
            return false;
        }
    }
}
