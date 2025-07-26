using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provdes integer-related tools.
    /// </summary>
    public static class IntTools
    {
        /// <summary>
        /// Checks if an integer meets a certain condition.
        /// </summary>
        /// <param name="i">The integer to check.</param>
        /// <param name="condition">The condition that the integer must meet.</param>
        /// <returns>True if the condition is met, false otherwise.</returns>
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

        /// <summary>
        /// Compares two integers.
        /// </summary>
        /// <param name="a">The first integer.</param>
        /// <param name="b">The second integer.</param>
        /// <param name="comparison">Determines how the two integers should be compared.</param>
        /// <returns>The result of the comparison.</returns>
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

        /// <summary>
        /// Does an operation on two integers.
        /// </summary>
        /// <param name="a">The first integer.</param>
        /// <param name="b">The second integer.</param>
        /// <param name="operation">Determines which operation should be done on the integers.</param>
        /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Types of integer operations that can be used for IntTools.DoOperation.
    /// </summary>
    public enum IntOperation
    {
        /// <summary>
        /// Returns the second integer.
        /// <para>If the first integer is set to the result of this operation, this is equivalent to a = b.</para>
        /// </summary>
        Set,

        /// <summary>
        /// Returns the result of adding the second integer to the first integer.
        /// <para>If the first integer is set to the result of this operation, this is equivalent to a += b.</para>
        /// </summary>
        Add,
        /// <summary>
        /// Returns the result of subtracting the second integer from the first integer.
        /// <para>If the first integer is set to the result of this operation, this is equivalent to a -= b.</para>
        /// </summary>
        Subtract,

        /// <summary>
        /// Returns the result of multiplying the first integer by the second integer.
        /// <para>If the first integer is set to the result of this operation, this is equivalent to a *= b.</para>
        /// </summary>
        Multiply,
        /// <summary>
        /// Returns the result of dividing the first integer by the second integer. This result is rounded down for positive numbers and rounded up for negative numbers.
        /// <para>If the first integer is set to the result of this operation, this is equivalent to a /= b.</para>
        /// </summary>
        Divide
    }

    // Why is this here?
    public enum BoolOperation
    {
        Set,

        Or,
        And,
        Xor
    }

    /// <summary>
    /// Types of integer conditions that can be used for IntTools.MeetsIntCondition.
    /// </summary>
    public enum IntCondition
    {
        /// <summary>
        /// Always returns true.
        /// </summary>
        None,

        /// <summary>
        /// Returns true if the integer is positive. Returns false if the integer is negative or zero.
        /// </summary>
        Positive,
        /// <summary>
        /// Returns true if the integer is negative. Returns false if the integer is positive or zero.
        /// </summary>
        Negative,

        /// <summary>
        /// Returns true if the integer isn't zero.
        /// </summary>
        NonZero
    }

    /// <summary>
    /// Types of integer comparison types that can be used for IntTools.CompareInts.
    /// </summary>
    public enum IntComparison
    {
        /// <summary>
        /// Returns true if the two integers are equal.
        /// </summary>
        Equal,
        /// <summary>
        /// Returns true if the two integers are not equal.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Returns true if the first integer is greater than the second integer.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Returns true if the first integer is greater than or equal to the second integer.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Returns true if the first integer is less than the second number.
        /// </summary>
        LessThan,
        /// <summary>
        /// Returns true if the first integer is less than or equal to the second integer.
        /// </summary>
        LessThanOrEqual,
    }
}
