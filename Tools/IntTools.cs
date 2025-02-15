using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class IntTools
    {
        public static bool MeetsIntCondition(int i, IntCondition condition)
        {
            return condition switch
            {
                IntCondition.Positive => i > 0,
                IntCondition.Negative => i < 0,

                IntCondition.NonZero => i != 0,

                _ => true
            };
        }

        public static bool CompareInts(int a, int b, IntComparison comparison)
        {
            return comparison switch
            {
                IntComparison.Equal => a == b,
                IntComparison.NotEqual => a != b,

                IntComparison.GreaterThan => a > b,
                IntComparison.GreaterThanOrEqual => a >= b,

                IntComparison.LessThan => a < b,
                IntComparison.LessThanOrEqual => a <= b,

                _ => true
            };
        }

        public static int DoOperation(int a, int b, IntOperation operation)
        {
            return operation switch
            {
                IntOperation.Add => a + b,
                IntOperation.Subtract => a - b,

                IntOperation.Multiply => a * b,
                IntOperation.Divide => a / b,

                _ => b
            };
        }
    }

    public enum IntOperation
    {
        Set,

        Add,
        Subtract,

        Multiply,
        Divide
    }

    public enum BoolOperation
    {
        Set,

        Or,
        And,
        Xor
    }

    public enum IntCondition
    {
        None,

        Positive,
        Negative,

        NonZero
    }

    public enum IntComparison
    {
        Equal,
        NotEqual,

        GreaterThan,
        GreaterThanOrEqual,

        LessThan,
        LessThanOrEqual,
    }
}
