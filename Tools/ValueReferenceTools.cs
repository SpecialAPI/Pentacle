using Pentacle.CustomEvent.Args;
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

                _                       => null
            };

            return stringHolder != null;
        }
    }
}
