using Pentacle.Advanced;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects
{
    /// <summary>
    /// A passive that can have any amount of trigger-, connection- and disconnection-activated effects, using Pentacle's trigger effect system.
    /// </summary>
    public class MultiCustomTriggerEffectPassive : AdvancedPassiveAbilitySO, ITriggerEffectHandler
    {
        /// <inheritdoc/>
        public override bool IsPassiveImmediate => false;
        /// <inheritdoc/>
        public override bool DoesPassiveTrigger => true;

        string ITriggerEffectHandler.DisplayedName => GetPassiveLocData().text;
        Sprite ITriggerEffectHandler.Sprite => passiveIcon;

        /// <summary>
        /// Trigger effects that should be performed on certain triggers.
        /// </summary>
        public List<EffectsAndTrigger> triggerEffects;
        /// <summary>
        /// Trigger effects that should be performed when this passive is connected to a unit.
        /// </summary>
        public List<TriggeredEffect> connectionEffects;
        /// <summary>
        /// Trigger effects that should be performed when this passive is disconnected from a unit.
        /// </summary>
        public List<TriggeredEffect> disconnectionEffects;

        private readonly Dictionary<int, Action<object, object>> effectMethods = [];

        /// <inheritdoc/>
        public MultiCustomTriggerEffectPassive()
        {
            _triggerOn = [];
        }

        /// <inheritdoc/>
        public override void OnPassiveConnected(IUnit unit)
        {
            if (connectionEffects == null)
                return;

            for (var i = 0; i < connectionEffects.Count; i++)
            {
                TryPerformItemEffect(unit, null, i);
            }
        }

        /// <inheritdoc/>
        public override void OnPassiveDisconnected(IUnit unit)
        {
            if (disconnectionEffects == null)
                return;

            for (var i = 0; i < disconnectionEffects.Count; i++)
            {
                TryPerformItemEffect(unit, null, i + (connectionEffects?.Count ?? 0));
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        private Action<object, object> GetEffectMethod(int i)
        {
            if (effectMethods.TryGetValue(i, out var existing))
                return existing;

            return effectMethods[i] = (sender, args) => TryPerformItemEffect(sender, args, i);
        }

        private void TryPerformItemEffect(object sender, object args, int index)
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

        /// <inheritdoc/>
        public override void FinalizeCustomTriggerPassive(object sender, object args, int idx)
        {
            if (idx >= ((triggerEffects?.Count ?? 0) + (connectionEffects?.Count ?? 0) + (disconnectionEffects?.Count ?? 0)) || sender is not IPassiveEffector effector || sender is not IUnit caster)
                return;

            var te = GetEffectAtIndex(idx, out var activation);

            if (te == null)
                return;

            if (te.doesPopup && (te.effect == null || !te.effect.ManuallyHandlePopup))
                CombatManager.Instance.AddUIAction(GetPopupUIAction(effector.ID, effector.IsUnitCharacter, false));

            te.effect?.DoEffect(caster, args, te, new()
            {
                handler = this,
                activation = activation
            });
        }

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

        /// <inheritdoc/>
        public override void TriggerPassive(object sender, object args)
        {
        }

        private CombatAction GetPopupUIAction(int id, bool isUnitCharacter, bool consumed)
        {
            return new ShowPassiveInformationUIAction(id, isUnitCharacter, GetPassiveLocData().text, passiveIcon);
        }

        bool ITriggerEffectHandler.TryGetPopupUIAction(int unitId, bool isUnitCharacter, bool consumed, out CombatAction action)
        {
            action = GetPopupUIAction(unitId, isUnitCharacter, consumed);
            return true;
        }
    }
}
