using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.EffectorConditions
{
    /// <summary>
    /// An effector condition that checks if a string stored in a StringReference args matches any string in this condition's matchStrings list.
    /// </summary>
    public class StringReferenceMatchesAnyEffectorCondition : EffectorConditionSO
    {
        /// <summary>
        /// The list of strings that will be checked. If any string in this list matches the value of the StringReference args, this condition will be met.
        /// </summary>
        public List<string> matchStrings;

        /// <inheritdoc/>
        public override bool MeetCondition(IEffectorChecks effector, object args)
        {
            if (matchStrings == null || matchStrings.Count <= 0)
                return false;

            if(!ValueReferenceTools.TryGetStringHolder(args, out var stringRef))
                return false;

            foreach(var s in matchStrings)
            {
                if (stringRef.Value == s)
                    return true;
            }

            return false;
        }
    }
}
