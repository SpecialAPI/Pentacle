using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// The base class for Pentacle's hidden passive effect system.
    /// <para>Hidden passive effects can only be added to characters and enemies that use Pentacle's AdvancedCharacterSO and AdvancedEnemySO classes respectively.</para>
    /// <para>It is not possible to add new hidden passive effects to an individual enemy or character in the middle of combat.</para>
    /// </summary>
    public abstract class HiddenEffectSO : ScriptableObject, ITriggerEffect<IEffectorChecks>
    {
        /// <summary>
        /// The list of string trigger calls that trigger this hidden passive effect.
        /// </summary>
        public List<string> triggers = [];
        /// <summary>
        /// The conditions required for this hidden passive effect to trigger.
        /// </summary>
        public List<EffectorConditionSO> conditions = [];

        /// <summary>
        /// Whether this hidden passive effect triggers immediately or as a delayed action.
        /// </summary>
        public abstract bool Immediate { get; }

        /// <summary>
        /// Connects this hidden passive effect's triggers to an effector.
        /// <para>More specifically, this method connects this hidden passive effect's default trigger (TryTrigger) to every trigger call in the triggers list, then calls CustomOnTriggerAttached for custom trigger connection.</para>
        /// <para>This method connects this hidden passive effect's triggers specifically. For triggering on-connection effects, use OnConnected.</para>
        /// </summary>
        /// <param name="caller">The effector to connect this hidden passive effect's triggers to.</param>
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
        /// Disconnects this hidden passive effect's triggers from an effector.
        /// <para>More specifically, this method disconnects this hidden passive effect's default trigger (TryTrigger) from every trigger call in the triggers list, then calls CustomOnTriggerDettached for custom trigger disconnection.</para>
        /// <para>This method disconnects this hidden passive effect's triggers specifically. For triggering on-disconnection effects, use OnDisconnected.</para>
        /// </summary>
        /// <param name="caller">The effector to disconnect this hidden passive effect's triggers from.</param>
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
        /// Connects this hidden passive effect to a unit.
        /// <para>This method is meant to trigger on-connection effects specifically. For connecting this hidden passive effect's triggers, use OnTriggerAttached and CustomOnTriggerAttached instead.</para>
        /// </summary>
        /// <param name="unit">The unit to connect this hidden passive effect to.</param>
        public virtual void OnConnected(IUnit unit)
        {
        }

        /// <summary>
        /// Disconnects this hidden passive effect from a unit.
        /// <para>This method is meant to trigger on-disconnection effects specifically. For disconnecting this hidden passive effect's triggers, use OnTriggerDettached and CustomOnTriggerDettached instead.</para>
        /// </summary>
        /// <param name="unit">The unit to disconnect this hidden passive effect from.</param>
        public virtual void OnDisconnected(IUnit unit)
        {
        }

        /// <summary>
        /// Connects this hidden passive effect's custom triggers (triggers not using the default TryTrigger method) to an effector.
        /// <para>This method is meant to connect this hidden passive effect's custom triggers specifically. For triggering on-connection effects, use OnConnected.</para>
        /// </summary>
        /// <param name="caller">The effector to connect this hidden passive effect's custom triggers to.</param>
        public virtual void CustomOnTriggerAttached(IEffectorChecks caller)
        {
        }

        /// <summary>
        /// Disconnects this hidden passive effect's custom triggers (triggers not using the default TryTrigger method) from an effector.
        /// <para>This method is meant to disconnect this hidden passive effect's custom triggers specifically. For triggering on-disconnection effects, use OnDisconnected.</para>
        /// </summary>
        /// <param name="caller">The effector to connect this hidden passive effect's custom triggers to.</param>
        public virtual void CustomOnTriggerDettached(IEffectorChecks caller)
        {
        }

        /// <summary>
        /// Attempts to trigger this hidden passive effect.
        /// </summary>
        /// <param name="sender">The object that sent the trigger call that triggered this effect.</param>
        /// <param name="args">Additional information from the trigger that can be used by this hidden passive effect.</param>
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
        /// Immediately triggers this hidden passive effect.
        /// <para>This method doesn't check this hiddden passive effect's conditions.</para>
        /// </summary>
        /// <param name="sender">The object that sent the trigger call that triggered this effect.</param>
        /// <param name="args">Additional information from the trigger that can be used by this hidden passive effect.</param>
        public virtual void Trigger(object sender, object args)
        {
        }

        /// <summary>
        /// Attempts to trigger a custom trigger from this hidden passive effect of the given index.
        /// </summary>
        /// <param name="sender">The object that sent the trigger call that triggered this effect.</param>
        /// <param name="args">Additional information from the trigger that can be used by this hidden passive effect.</param>
        /// <param name="index">The index of the custom trigger being triggered.</param>
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
        /// Immediately triggers a custom trigger from this hidden passive effect of the given index.
        /// </summary>
        /// <param name="sender">The object that sent the trigger call that triggered this effect.</param>
        /// <param name="args">Additional information from the trigger that can be used by this hidden passive effect.</param>
        /// <param name="index">The index of the custom trigger being triggered.</param>
        public virtual void CustomTrigger(object sender, object args, int index)
        {
        }
    }

    /// <summary>
    /// A combat action that triggers a hidden passive effect.
    /// </summary>
    /// <param name="effect">The hidden passive effect this action should trigger.</param>
    /// <param name="sender">The object that sent the trigger call that triggered this effect.</param>
    /// <param name="args">Additional information from the trigger that can be used by this hidden passive effect.</param>
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
    /// A combat action that triggers a hidden passive effect's custom trigger.
    /// </summary>
    /// <param name="effect">The hidden passive effect this action should trigger.</param>
    /// <param name="sender">The object that sent the trigger call that triggered this effect.</param>
    /// <param name="args">Additional information from the trigger that can be used by this hidden passive effect.</param>
    /// <param name="index">The index of the custom trigger this action should trigger.</param>
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
