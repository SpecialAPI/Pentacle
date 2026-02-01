using Pentacle.Triggers.Args;
using Pentacle.Triggers.Args.BasegameReferenceHolders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides tools related to the IBoolHolder, IIntHolder, IStringHolder, IUnitHolder and IValueChangeException interfaces.
    /// </summary>
    public static class ValueReferenceTools
    {
        /// <summary>
        /// Tries to convert <paramref name="args"/> to an IBoolHolder:
        /// <list type="bullet">
        /// <item>If <paramref name="args"/> is already a subclass of IBoolHolder, outputs <paramref name="args"/>.</item>
        /// <item>If <paramref name="args"/> is a BooleanReference, outputs an IBoolHolder whose value is connected to the BooleanReference's value.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Damage, whose 2 read-only values get the IntegerReference_Damage's directDamage and ignoresShield respectively.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Heal, whose read-only value gets the IntegerReference_Heal's directHeal.</item>
        /// <item>If <paramref name="args"/> is a BooleanWithTriggerReference, outputs an IBoolHolder whose value is connected to the BooleanWithTriggerReference's value.</item>
        /// <item>If <paramref name="args"/> is a StatusFieldApplication, outputs an IBoolHolder whose primary value is connected to the StatusFieldApplication's canBeApplied and whose secondary read-only value gets the StatusFieldApplication's isStatusPositive.</item>
        /// <item>If <paramref name="args"/> is a DamageReceivedValueChangeException, outputs an IBoolHolder whose 2 read-only values get the DamageReceivedValueChangeException's directDamage and ignoresShield respectively.</item>
        /// <item>If <paramref name="args"/> is a HealingReceivedValueChangeException, outputs an IBoolHolder whose read-only value gets the HealingReceivedValueChangeException's directHealing.</item>
        /// </list>
        /// </summary>
        /// <param name="args">The object to convert to an IBoolHolder.</param>
        /// <param name="boolHolder">If successful, outputs the conversion result. Otherwise, outputs null.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        public static bool TryGetBoolHolder(object args, out IBoolHolder boolHolder)
        {
            boolHolder = args switch
            {
                IBoolHolder bh                              => bh,

                BooleanReference br                         => new BooleanReferenceHolder(br),
                IntegerReference_Damage ird                 => new IntegerReferenceDamageHolder(ird),
                IntegerReference_Heal irh                   => new IntegerReferenceHealHolder(irh),
                BooleanWithTriggerReference bwtr            => new BooleanWithTriggerReferenceHolder(bwtr),
                StatusFieldApplication sfa                  => new StatusFieldApplicationHolder(sfa),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),
                HealingReceivedValueChangeException hrex    => new HealingReceivedValueChangeExceptionHolder(hrex),

                _                                           => null
            };

            return boolHolder != null;
        }

        /// <summary>
        /// Tries to convert <paramref name="args"/> to an IIntHolder:
        /// <list type="bullet">
        /// <item>If <paramref name="args"/> is already a subclass of IIntHolder, outputs <paramref name="args"/>.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Damage, outputs an IIntHolder whose first value is connected to the IntegerReference_Damage's value, and whose second and third values are read-only and get the IntegerReference_Damage's affectedStartSlot and affectedEndSlot respectively.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Heal, outputs an IIntHolder whose value is connected to the IntegerReference_Heal's value.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference, outputs an IIntHolder whose value is connected to the IntegerReference's value.</item>
        /// <item>If <paramref name="args"/> is a StatusFieldApplication, outputs an IIntHolder whose value is connected to the StatusFieldApplication's amount.</item>
        /// </list>
        /// </summary>
        /// <param name="args">The object to convert to an IIntHolder.</param>
        /// <param name="intHolder">If successful, outputs the conversion result. Otherwise, outputs null.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        public static bool TryGetIntHolder(object args, out IIntHolder intHolder)
        {
            intHolder = args switch
            {
                IIntHolder ih                               => ih,
                
                IntegerReference_Damage ird                 => new IntegerReferenceDamageHolder(ird),
                IntegerReference_Heal irh                   => new IntegerReferenceHealHolder(irh),
                IntegerReference ir                         => new IntegerReferenceHolder(ir),

                StatusFieldApplication sfa                  => new StatusFieldApplicationHolder(sfa),

                _                       => null
            };

            return intHolder != null;
        }

        /// <summary>
        /// Tries to convert <paramref name="args"/> to an IStringHolder:
        /// <list type="bullet">
        /// <item>If <paramref name="args"/> is already a subclass of IStringHolder, outputs <paramref name="args"/>.</item>
        /// <item>If <paramref name="args"/> is a StringReference, outputs an IStringHolder whose value is connected to the StringReference's value.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Damage, outputs an IStringHolder whose 2 read-only values get the IntegerReference_Damage's damageTypeID and deathTypeID respectively.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Heal, outputs an IStringHolder whose read-only value gets the IntegerReference_Heal's healTypeID.</item>
        /// <item>If <paramref name="args"/> is a StatusFieldApplication, outputs an IStringHolder whose read-only value gets the StatusFieldApplication's statusID.</item>
        /// <item>If <paramref name="args"/> is a DamageReceivedValueChangeException, outputs an IStringHolder whose read-only value gets the DamageReceivedValueChangeExceptionHolder's damageTypeID.</item>
        /// </list>
        /// </summary>
        /// <param name="args">The object to convert to an IStringHolder.</param>
        /// <param name="stringHolder">If successful, outputs the conversion result. Otherwise, outputs null.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        public static bool TryGetStringHolder(object args, out IStringHolder stringHolder)
        {
            stringHolder = args switch
            {
                IStringHolder sh                            => sh,

                StringReference sr                          => new StringReferenceHolder(sr),
                IntegerReference_Damage ird                 => new IntegerReferenceDamageHolder(ird),
                IntegerReference_Heal irh                   => new IntegerReferenceHealHolder(irh),
                StatusFieldApplication sfa                  => new StatusFieldApplicationHolder(sfa),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),

                _                                           => null
            };

            return stringHolder != null;
        }

        /// <summary>
        /// Tries to convert <paramref name="args"/> to an IUnitHolder:
        /// <list type="bullet">
        /// <item>If <paramref name="args"/> is already a subclass of IUnitHolder, outputs <paramref name="args"/>.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Damage, outputs an IUnitHolder whose 2 read-only values get the IntegerReference_Damage's possibleSourceUnit and damagedUnit respectively.</item>
        /// <item>If <paramref name="args"/> is a IntegerReference_Heal, outputs an IUnitHolder whose 2 read-only values get the IntegerReference_Heal's possibleSourceUnit and healedUnit respectively.</item>
        /// <item>If <paramref name="args"/> is a DamageDealtValueChangeException, outputs an IUnitHolder whose 2 read-only values get the DamageDealtValueChangeException's damagedUnit and casterUnit respectively.</item>
        /// <item>If <paramref name="args"/> is a DamageReceivedValueChangeException, outputs an IUnitHolder whose 2 read-only values get the DamageReceivedValueChangeException's possibleSourceUnit and damagedUnit respectively.</item>
        /// <item>If <paramref name="args"/> is a HealingDealtValueChangeException, outputs an IUnitHolder whose 2 read-only values get the HealingDealtValueChangeException's healingUnit and casterUnit respectively.</item>
        /// <item>If <paramref name="args"/> is a HealingReceivedValueChangeException, outputs an IUnitHolder whose 2 read-only values get the HealingReceivedValueChangeException's possibleSourceUnit and healingUnit respectively.</item>
        /// <item>If <paramref name="args"/> is a IUnit, outputs an IUnitHolder whose read-only value gets that IUnit.</item>
        /// </list>
        /// </summary>
        /// <param name="args">The object to convert to an IUnitHolder.</param>
        /// <param name="unitHolder">If successful, outputs the conversion result. Otherwise, outputs null.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        public static bool TryGetUnitHolder(object args, out IUnitHolder unitHolder)
        {
            unitHolder = args switch
            {
                IUnitHolder uh                              => uh,
                
                IntegerReference_Damage ird                 => new IntegerReferenceDamageHolder(ird),
                IntegerReference_Heal irh                   => new IntegerReferenceHealHolder(irh),
                DamageDealtValueChangeException ddex        => new DamageDealtValueChangeExceptionHolder(ddex),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),
                HealingDealtValueChangeException hdex       => new HealingDealtValueChangeExceptionHolder(hdex),
                HealingReceivedValueChangeException hrex    => new HealingReceivedValueChangeExceptionHolder(hrex),
                IUnit u                                     => new SimpleUnitHolder(u),

                _                                           => null
            };

            return unitHolder != null;
        }

        /// <summary>
        /// Tries to convert <paramref name="args"/> to an IValueChangeException:
        /// <list type="bullet">
        /// <item>If <paramref name="args"/> is already a subclass of IValueChangeException, outputs <paramref name="args"/>.</item>
        /// <item>If <paramref name="args"/> is a basegame value change exception, returns an object whose AddModifier and GetModifiedValue methods are connected to the value change exception's AddModifier and GetModifiedValue methods.
        /// <br>DamageDealt is true for DamageDealtValueChangeException and HealingDealtValueChangeException and false for all other basegame value change exceptions.</br></item>
        /// </list>
        /// </summary>
        /// <param name="args">The object to convert to an IValueChangeException.</param>
        /// <param name="exception">If successful, outputs the conversion result. Otherwise, outputs null.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        public static bool TryGetValueChangeException(object args, out IValueChangeException exception)
        {
            exception = args switch
            {
                IValueChangeException ex                    => ex,

                IntValueChangeException iex                 => new IntValueChangeExceptionHolder(iex),
                DamageDealtValueChangeException ddex        => new DamageDealtValueChangeExceptionHolder(ddex),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),
                HealingDealtValueChangeException hdex       => new HealingDealtValueChangeExceptionHolder(hdex),
                HealingReceivedValueChangeException hrex    => new HealingReceivedValueChangeExceptionHolder(hrex),

                _                                           => null
            };

            return exception != null;
        }
    }
}
