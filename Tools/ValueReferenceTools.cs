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
                BooleanReference br                 => new BooleanReferenceHolder(br),
                BooleanWithTriggerReference bwtr    => new BooleanWithTriggerReferenceHolder(bwtr),
                IBoolHolder bh                      => bh,
                StatusFieldApplication sfa          => new StatusFieldApplicationHolder(sfa),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),
                HealingReceivedValueChangeException hrex    => new HealingReceivedValueChangeExceptionHolder(hrex),

                _                                   => null
            };

            return boolHolder != null;
        }

        public static bool TryGetIntHolder(object args, out IIntHolder intHolder)
        {
            intHolder = args switch
            {
                IntegerReference ir             => new IntegerReferenceHolder(ir),
                IIntHolder ih                   => ih,
                StatusFieldApplication sfa      => new StatusFieldApplicationHolder(sfa),

                _                       => null
            };

            return intHolder != null;
        }

        public static bool TryGetStringHolder(object args, out IStringHolder stringHolder)
        {
            stringHolder = args switch
            {
                StringReference sr              => new StringReferenceHolder(sr),
                IStringHolder sh                => sh,
                StatusFieldApplication sfa      => new StatusFieldApplicationHolder(sfa),
                DamageReceivedValueChangeException drex     => new DamageReceivedValueChangeExceptionHolder(drex),

                _                       => null
            };

            return stringHolder != null;
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
