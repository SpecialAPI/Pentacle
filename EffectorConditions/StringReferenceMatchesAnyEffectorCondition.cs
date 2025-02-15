using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.EffectorConditions
{
    public class StringReferenceMatchesAnyEffectorCondition : EffectorConditionSO
    {
        public List<string> matchStrings;

        public override bool MeetCondition(IEffectorChecks effector, object args)
        {
            if (matchStrings == null || matchStrings.Count <= 0)
                return false;

            if(!args.TryGetStringReference(out var stringRef))
                return false;

            foreach(var s in matchStrings)
            {
                if (stringRef.value == s)
                    return true;
            }

            return false;
        }
    }
}
