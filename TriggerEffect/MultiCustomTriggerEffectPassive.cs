using Pentacle.Advanced;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// A passive that can have any amount of effects.
    /// </summary>
    public class MultiCustomTriggerEffectPassive : AdvancedPassiveAbilitySO
    {
        public override bool IsPassiveImmediate => false;
        public override bool DoesPassiveTrigger => true;

        /// <summary>
        /// Effects that should be performed on triggers.
        /// </summary>
        public List<EffectsAndTrigger> triggerEffects;
        /// <summary>
        /// Effects that should be performed when this passive is connected.
        /// </summary>
        public List<TriggeredEffect> connectionEffects;
        /// <summary>
        /// Effects that should be performed when this passive is disconnected.
        /// </summary>
        public List<TriggeredEffect> disconnectionEffects;

        private readonly Dictionary<int, Action<object, object>> effectMethods = [];

        public MultiCustomTriggerEffectPassive()
        {
            _triggerOn = [];
        }

        public override void OnPassiveConnected(IUnit unit)
        {
            if (connectionEffects == null)
                return;

            for (var i = 0; i < connectionEffects.Count; i++)
            {
                TryPerformItemEffect(unit, null, i);
            }
        }

        public override void OnPassiveDisconnected(IUnit unit)
        {
            if (disconnectionEffects == null)
                return;

            for (var i = 0; i < disconnectionEffects.Count; i++)
            {
                TryPerformItemEffect(unit, null, i + (connectionEffects?.Count ?? 0));
            }
        }

        public override void CustomOnTriggerAttached(IPassiveEffector caller)
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

        public override void CustomOnTriggerDettached(IPassiveEffector caller)
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
            if (index >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IPassiveEffector effector || !effector.CanPassiveTrigger(m_PassiveID))
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
                FinalizeCustomTriggerPassive(sender, args, index);

            else
                CombatManager.Instance.AddSubAction(new PerformPassiveCustomAction(this, sender, args, index));
        }

        public override void FinalizeCustomTriggerPassive(object sender, object args, int idx)
        {
            if (idx >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IPassiveEffector effector || sender is not IUnit caster)
                return;

            var te = GetEffectAtIndex(idx, out var activation);

            if (te == null)
                return;

            if (te.doesPopup && (te.effect == null || !te.effect.ManuallyHandlePopup))
                CombatManager.Instance.AddUIAction(GetPopupUIAction(effector.ID, effector.IsUnitCharacter, false));

            te.effect.DoEffect(caster, args, te, new()
            {
                activator = this,
                getPopupUIAction = GetPopupUIAction,
                activation = activation
            });
        }

        public CombatAction GetPopupUIAction(int id, bool isUnitCharacter, bool consumed)
        {
            return new ShowPassiveInformationUIAction(id, isUnitCharacter, GetPassiveLocData().text, passiveIcon);
        }

        public TriggeredEffect GetEffectAtIndex(int idx, out TriggerEffectActivation activation)
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

        public override void TriggerPassive(object sender, object args)
        {
        }
    }
}
