using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// The base class for Hidden Effects. Hidden Effects are similar to passives, but are not displayed.
    /// </summary>
    public abstract class HiddenEffectSO : ScriptableObject, ITriggerEffect<IEffectorChecks>
    {
        /// <summary>
        /// What does this Hidden Effect trigger on?
        /// </summary>
        public List<string> triggers = [];
        /// <summary>
        /// The conditions required for this Hidden effect to trigger.
        /// </summary>
        public List<EffectorConditionSO> conditions = [];

        /// <summary>
        /// Whether this Hidden Effect triggers immediately or as a delayed action.
        /// </summary>
        public abstract bool Immediate { get; }

        /// <summary>
        /// Attaches this Hidden Effect's triggers to the given effector.
        /// </summary>
        /// <param name="caller">The effector to attach this Hidden Effect to.</param>
        public void OnTriggerAttached(IEffectorChecks caller)
        {
            if(triggers != null)
            {
                foreach(var t in triggers)
                {
                    if (string.IsNullOrEmpty(t) || t == TriggerCalls.Count.ToString())
                        continue;

                    CombatManager.Instance.AddObserver(TryTrigger, t, caller);
                }
            }

            CustomOnTriggerAttached(caller);
        }

        /// <summary>
        /// Dettaches this Hidden Effect's triggers from the given effector.
        /// </summary>
        /// <param name="caller">The effector to dettach this Hidden Effect from.</param>
        public void OnTriggerDettached(IEffectorChecks caller)
        {
            if (triggers != null)
            {
                foreach (var t in triggers)
                {
                    if (string.IsNullOrEmpty(t) || t == TriggerCalls.Count.ToString())
                        continue;

                    CombatManager.Instance.RemoveObserver(TryTrigger, t, caller);
                }
            }

            CustomOnTriggerDettached(caller);
        }

        /// <summary>
        /// Connects this Hidden Effect to the given unit.
        /// </summary>
        /// <param name="unit">The unit to connect this Hidden Effect to.</param>
        public virtual void OnConnected(IUnit unit)
        {
        }

        /// <summary>
        /// Disconnects this Hidden Effect from the given unit.
        /// </summary>
        /// <param name="unit">The unit to disconnect this Hidden Effect from.</param>
        public virtual void OnDisconnected(IUnit unit)
        {
        }

        /// <summary>
        /// Attaches this Hidden Effect's custom triggers to the given effector.
        /// </summary>
        /// <param name="caller">The effector to attach this Hidden Effect's custom triggers to.</param>
        public virtual void CustomOnTriggerAttached(IEffectorChecks caller)
        {
        }

        /// <summary>
        /// Dettaches this Hidden Effect's custom triggers from the given effector.
        /// </summary>
        /// <param name="caller">The effector to dettach this Hidden Effect's custom triggers from.</param>
        public virtual void CustomOnTriggerDettached(IEffectorChecks caller)
        {
        }

        /// <summary>
        /// Attempts to trigger this Hidden Effect.
        /// </summary>
        /// <param name="sender">The trigger's sender.</param>
        /// <param name="args">Additional arguments for the trigger.</param>
        public void TryTrigger(object sender, object args)
        {
            if (sender is not IEffectorChecks effector)
                return;

            if(conditions != null)
            {
                foreach(var cond in conditions)
                {
                    if (!cond.MeetCondition(effector, args))
                        return;
                }
            }

            if (Immediate)
                Trigger(sender, args);

            else
                CombatManager.Instance.AddSubAction(new TriggerHiddenEffectAction(this, sender, args));
        }

        /// <summary>
        /// Immediately triggers this Hidden Effect.
        /// </summary>
        /// <param name="sender">The trigger's sender.</param>
        /// <param name="args">Additional arguments for the trigger.</param>
        public virtual void Trigger(object sender, object args)
        {
        }

        /// <summary>
        /// Attempts to trigger this Hidden Effect with a custom trigger index.
        /// </summary>
        /// <param name="sender">The trigger's sender.</param>
        /// <param name="args">Additional arguments for the trigger.</param>
        /// <param name="index">Custom trigger index.</param>
        public void TryCustomTrigger(object sender, object args, int index)
        {
            if (sender is not IEffectorChecks effector)
                return;

            if (conditions != null)
            {
                foreach (var cond in conditions)
                {
                    if (!cond.MeetCondition(effector, args))
                        return;
                }
            }

            if (Immediate)
                CustomTrigger(sender, args, index);

            else
                CombatManager.Instance.AddSubAction(new TriggerHiddenEffectCustomAction(this, sender, args, index));
        }

        /// <summary>
        /// Immediately triggers this Hidden Effect with a custom trigger index.
        /// </summary>
        /// <param name="sender">The trigger's sender.</param>
        /// <param name="args">Additional arguments for the trigger.</param>
        /// <param name="index">Custom trigger index.</param>
        public virtual void CustomTrigger(object sender, object args, int index)
        {
        }
    }

    /// <summary>
    /// An action that triggers the given Hidden Effect with a sender and arguments.
    /// </summary>
    /// <param name="effect">The Hidden Effect to trigger.</param>
    /// <param name="sender">The trigger's sender.</param>
    /// <param name="args">Additional arguments for the trigger.</param>
    public class TriggerHiddenEffectAction(HiddenEffectSO effect, object sender, object args) : CombatAction
    {
        public override IEnumerator Execute(CombatStats stats)
        {
            if(effect == null)
                yield break;

            effect.Trigger(sender, args);
        }
    }

    /// <summary>
    /// An action that triggers the given Hidden Effect with a sender and arguments.
    /// </summary>
    /// <param name="effect">The Hidden Effect to trigger.</param>
    /// <param name="sender">The trigger's sender.</param>
    /// <param name="args">Additional arguments for the trigger.</param>
    /// <param name="index">Custom trigger index.</param>
    public class TriggerHiddenEffectCustomAction(HiddenEffectSO effect, object sender, object args, int index) : CombatAction
    {
        public override IEnumerator Execute(CombatStats stats)
        {
            if (effect == null)
                yield break;

            effect.CustomTrigger(sender, args, index);
        }
    }
}
