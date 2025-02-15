using Pentacle.CustomEvent.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class ValueReferenceTools
    {
        public static bool TryGetBoolReference(this object args, out BooleanReference boolRef)
        {
            boolRef = null;

            if(args is BooleanReference br)
            {
                boolRef = br;
                return true;
            }

            if(args is IBoolReferenceHolder brh)
            {
                boolRef = brh.BoolReference;
                return true;
            }

            return false;
        }

        public static bool TryGetIntReference(this object args, out IntegerReference intRef)
        {
            intRef = null;

            if (args is IntegerReference ir)
            {
                intRef = ir;
                return true;
            }

            if (args is IIntReferenceHolder irh)
            {
                intRef = irh.IntReference;
                return true;
            }

            return false;
        }

        public static bool TryGetStringReference(this object args, out StringReference stringRef)
        {
            stringRef = null;

            if (args is StringReference sr)
            {
                stringRef = sr;
                return true;
            }

            if (args is IStringReferenceHolder srh)
            {
                stringRef = srh.StringReference;
                return true;
            }

            return false;
        }
    }
}
