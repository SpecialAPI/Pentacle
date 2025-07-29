using Pentacle.CustomEvent.Args;
using Pentacle.CustomEvent.Args.BasegameReferenceHolders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class ValueReferenceTools
    {
        public static bool TryGetBoolHolder(object args, out IBoolHolder boolHolder)
        {
            boolHolder = args switch
            {
                IBoolHolder bh                              => bh,
                BooleanReference br                         => new BooleanReferenceHolder(br),
                BooleanWithTriggerReference bwtr            => new BooleanWithTriggerReferenceHolder(bwtr),
                StatusFieldApplication sfa                  => new StatusFieldApplicationHolder(sfa),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),
                HealingReceivedValueChangeException hrex    => new HealingReceivedValueChangeExceptionHolder(hrex),

                _                                           => null
            };

            return boolHolder != null;
        }

        public static bool TryGetIntHolder(object args, out IIntHolder intHolder)
        {
            intHolder = args switch
            {
                IIntHolder ih                               => ih,
                IntegerReference ir                         => new IntegerReferenceHolder(ir),
                StatusFieldApplication sfa                  => new StatusFieldApplicationHolder(sfa),

                _                       => null
            };

            return intHolder != null;
        }

        public static bool TryGetStringHolder(object args, out IStringHolder stringHolder)
        {
            stringHolder = args switch
            {
                IStringHolder sh                            => sh,
                StringReference sr                          => new StringReferenceHolder(sr),
                StatusFieldApplication sfa                  => new StatusFieldApplicationHolder(sfa),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),

                _                                           => null
            };

            return stringHolder != null;
        }

        public static bool TryGetUnitHolder(object args, out IUnitHolder unitHolder)
        {
            unitHolder = args switch
            {
                IUnitHolder uh                              => uh,
                DamageDealtValueChangeException ddex        => new DamageDealtValueChangeExceptionHolder(ddex),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),
                HealingDealtValueChangeException hdex       => new HealingDealtValueChangeExceptionHolder(hdex),
                HealingReceivedValueChangeException hrex    => new HealingReceivedValueChangeExceptionHolder(hrex),

                _                                           => null
            };

            return unitHolder != null;
        }

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
