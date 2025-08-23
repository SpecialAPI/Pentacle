using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides IL-related tools and extensions.
    /// </summary>
    public static class ILTools
    {
        /// <summary>
        /// Tries to move an IL cursor after the next instruction that matches a specific condition a specific number of times.
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="match">The condition that needs to be met for the cursor to move to a specific instruction.</param>
        /// <param name="times">The amount of times the cursor should move.</param>
        /// <returns>True if all the moves were successful, false otherwise.</returns>
        public static bool JumpToNext(this ILCursor crs, Func<Instruction, bool> match, int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                if (!crs.TryGotoNext(MoveType.After, match))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to move an IL cursor before the next instruction that matches a specific condition a specific number of times.
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="match">The condition that needs to be met for the cursor to move to a specific instruction.</param>
        /// <param name="times">The amount of times the cursor should move.</param>
        /// <returns>True if all the moves were successful, false otherwise.</returns>
        public static bool JumpBeforeNext(this ILCursor crs, Func<Instruction, bool> match, int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                if (!crs.TryGotoNext((i == (times - 1)) ? MoveType.Before : MoveType.After, match))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns an enumerable that moves the IL cursor after the next instruction matching a specific condition each iteration. Each iteration returns the instruction that the cursor has moved to.
        /// <para>After there are no more instructions that meet the condition ahead of the cursor, the cursor is moved back to the position where it was before iteration started.</para>
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="match">The condition that needs to be met for the cursor to move to a specific instruction.</param>
        /// <param name="matchFromStart">If true, the cursor will be moved to the start of the instructions list before iteration starts.</param>
        /// <returns>An enumerable that moves the IL cursor each iteration.</returns>
        public static IEnumerable MatchAfter(this ILCursor crs, Func<Instruction, bool> match, bool matchFromStart = true)
        {
            var curr = crs.Next;

            if(matchFromStart)
                crs.Index = 0;

            while (crs.JumpToNext(match))
                yield return crs.Previous;

            crs.Next = curr;
        }

        /// <summary>
        /// Returns an enumerable that moves the IL cursor before the next instruction matching a specific condition each iteration. Each iteration returns the instruction that the cursor has moved to.
        /// <para>After there are no more instructions that meet the condition ahead of the cursor, the cursor is moved back to the position where it was before iteration started.</para>
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="match">The condition that needs to be met for the cursor to move to a specific instruction.</param>
        /// <param name="matchFromStart">If true, the cursor will be moved to the start of the instructions list before iteration starts.</param>
        /// <returns>An enumerable that moves the IL cursor each iteration.</returns>
        public static IEnumerable MatchBefore(this ILCursor crs, Func<Instruction, bool> match, bool matchFromStart = true)
        {
            var curr = crs.Next;

            if(matchFromStart)
                crs.Index = 0;

            while (crs.JumpBeforeNext(match))
            {
                var c = crs.Next;

                yield return crs.Next;
                crs.Goto(c, MoveType.After);
            }

            crs.Next = curr;
        }
        
        /// <summary>
        /// Declares a new local variable in an IL context.
        /// </summary>
        /// <typeparam name="T">The value type for the new local variable.</typeparam>
        /// <param name="ctx">The context to declare the local variable in.</param>
        /// <returns>The definition for the new local variable.</returns>
        public static VariableDefinition DeclareLocal<T>(this ILContext ctx)
        {
            var loc = new VariableDefinition(ctx.Import(typeof(T)));
            ctx.Body.Variables.Add(loc);

            return loc;
        }

        /// <summary>
        /// Declares a new local variable in an IL cursor's IL context.
        /// </summary>
        /// <typeparam name="T">The value type for the new local variable.</typeparam>
        /// <param name="crs">The cursor whose context the local variable will be declared in.</param>
        /// <returns>The definition for the new local variable.</returns>
        public static VariableDefinition DeclareLocal<T>(this ILCursor crs)
        {
            return crs.Context.DeclareLocal<T>();
        }

        /// <summary>
        /// Tries to move an IL cursor after an instruction that pushes a value that will be used as a specific argument by the input instruction.
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="targetInstr">The instruction that the move target needs to push an argument value for.</param>
        /// <param name="argIndex">The index of the argument that the move target needs to push a value for.</param>
        /// <param name="instance">The number of the argument intstruction this needs to move after. This matters if there are multiple instructions that can push the argument value for the input instruction.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryGotoArg(this ILCursor crs, Instruction targetInstr, int argIndex, int instance = 0)
        {
            if (argIndex < 0)
                return false;

            if (instance < 0)
                return false;

            if (targetInstr == null)
                return false;

            var argumentInstrs = targetInstr.GetArgumentInstructions(crs.Context, argIndex);

            if (instance >= argumentInstrs.Count)
                return false;

            crs.Goto(argumentInstrs[instance], MoveType.After);
            return true;
        }

        /// <summary>
        /// Tries to move an IL cursor after an instruction that pushes a value that will be used as a specific argument by the cursor's next instruction.
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="argIndex">The index of the argument that the move target needs to push a value for.</param>
        /// <param name="instance">The number of the argument intstruction this needs to move after. This matters if there are multiple instructions that can push the argument value for the next instruction.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryGotoArg(this ILCursor crs, int argIndex, int instance = 0)
        {
            return crs.TryGotoArg(crs.Next, argIndex, instance);
        }

        /// <summary>
        /// Returns an enumerable that moves the IL cursor after the next instruction that pushes a value that will be used as a specific argument by the input instruction each iteration, starting from the first instruction that does so. Each iteration returns the instruction that the cursor moved to.
        /// <para>After there are no more instructions that meet the condition ahead of the cursor, the cursor is moved back to the position where it was before iteration started.</para>
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="targetInstr">The instruction that the move targets need to push argument values for.</param>
        /// <param name="argIndex">The index of the argument that the move targets need to push values for.</param>
        /// <returns>An enumerable that moves the IL cursor each iteration.</returns>
        public static IEnumerable MatchArg(this ILCursor crs, Instruction targetInstr, int argIndex)
        {
            if (argIndex < 0)
                yield break;

            if (targetInstr == null)
                yield break;

            var curr = crs.Next;
            var argumentInstrs = targetInstr.GetArgumentInstructions(crs.Context, argIndex);

            foreach (var arg in argumentInstrs)
            {
                crs.Goto(arg, MoveType.After);

                yield return null;
            }

            crs.Next = curr;
        }

        /// <summary>
        /// Returns an enumerable that moves the IL cursor after the next instruction that pushes a value that will be used as a specific argument by the cursor's current next instruction each iteration, starting from the first instruction that does so. Each iteration returns the instruction that the cursor moved to.
        /// <para>After there are no more instructions that meet the condition ahead of the cursor, the cursor is moved back to the position where it was before iteration started.</para>
        /// </summary>
        /// <param name="crs">The IL cursor that will be moved.</param>
        /// <param name="argIndex">The index of the argument that the move targets need to push values for.</param>
        /// <returns>An enumerable that moves the IL cursor each iteration.</returns>
        public static IEnumerable MatchArg(this ILCursor crs, int argIndex)
        {
            return crs.MatchArg(crs.Next, argIndex);
        }

        private static List<Instruction> GetArgumentInstructions(this Instruction instruction, ILContext context, int argumentIndex)
        {
            var args = instruction.InputCount();
            var moves = args - argumentIndex - 1;

            if (moves < 0)
                return [];

            var prev = instruction.PossiblePreviousInstructions(context);
            var argInstrs = new List<Instruction>();

            foreach (var i in prev)
                BacktrackToArg(i, context, moves, argInstrs);

            argInstrs.Sort((a, b) => context.IndexOf(a).CompareTo(context.IndexOf(b)));

            return argInstrs;
        }

        private static void BacktrackToArg(Instruction current, ILContext ctx, int remainingMoves, List<Instruction> foundArgs)
        {
            if (remainingMoves <= 0 && current.OutputCount() > 0)
            {
                if (remainingMoves == 0)
                    foundArgs.Add(current);

                return;
            }

            remainingMoves -= current.StackDelta();
            var prev = current.PossiblePreviousInstructions(ctx);

            foreach (var i in prev)
                BacktrackToArg(i, ctx, remainingMoves, foundArgs);
        }

        // TODO: make private
        public static int InputCount(this Instruction instr)
        {
            if (instr == null)
                return 0;

            var op = instr.OpCode;

            if (op.FlowControl == FlowControl.Call)
            {
                var mthd = (IMethodSignature)instr.Operand;
                var ins = 0;

                if (op.Code != Code.Newobj && mthd.HasThis && !mthd.ExplicitThis)
                    ins++; // Input the "self" arg

                if (mthd.HasParameters)
                    ins += mthd.Parameters.Count; // Input all of the parameters

                if (op.Code == Code.Calli)
                    ins++; // No clue for this one

                return ins;
            }

            return op.StackBehaviourPop switch
            {
                StackBehaviour.Pop1 or StackBehaviour.Popi or StackBehaviour.Popref => 1,
                StackBehaviour.Pop1_pop1 or StackBehaviour.Popi_pop1 or StackBehaviour.Popi_popi or StackBehaviour.Popi_popi8 or StackBehaviour.Popi_popr4 or StackBehaviour.Popi_popr8 or StackBehaviour.Popref_pop1 or StackBehaviour.Popref_popi => 2,
                StackBehaviour.Popi_popi_popi or StackBehaviour.Popref_popi_popi or StackBehaviour.Popref_popi_popi8 or StackBehaviour.Popref_popi_popr4 or StackBehaviour.Popref_popi_popr8 or StackBehaviour.Popref_popi_popref => 3,

                _ => 0,
            };
        }

        // TODO: make private
        public static int OutputCount(this Instruction instr)
        {
            if (instr == null)
                return 0;

            var op = instr.OpCode;

            if (op.FlowControl == FlowControl.Call)
            {
                var mthd = (IMethodSignature)instr.Operand;
                var outs = 0;

                if (op.Code == Code.Newobj || mthd.ReturnType.MetadataType != MetadataType.Void)
                    outs++; // Output the return value

                return outs;
            }

            return op.StackBehaviourPush switch
            {
                StackBehaviour.Push1 or StackBehaviour.Pushi or StackBehaviour.Pushi8 or StackBehaviour.Pushr4 or StackBehaviour.Pushr8 or StackBehaviour.Pushref => 1,
                StackBehaviour.Push1_push1 => 2,

                _ => 0,
            };
        }

        // TODO: make private
        public static int StackDelta(this Instruction instr)
        {
            return instr.OutputCount() - instr.InputCount();
        }

        // TODO: make private
        public static List<Instruction> PossiblePreviousInstructions(this Instruction instr, ILContext ctx)
        {
            var l = new List<Instruction>();

            foreach (var i in ctx.Instrs)
            {
                if (Array.IndexOf(i.PossibleNextInstructions(), instr) >= 0)
                    l.Add(i);
            }

            return l;
        }

        // TODO: make private
        public static Instruction[] PossibleNextInstructions(this Instruction instr)
        {
            return instr.OpCode.FlowControl switch
            {
                FlowControl.Next or FlowControl.Call => [instr.Next],
                FlowControl.Branch => instr.GetBranchTarget() is Instruction tr ? [tr] : Array.Empty<Instruction>(),
                FlowControl.Cond_Branch => instr.GetBranchTarget() is Instruction tr ? [instr.Next, tr] : [instr.Next],

                _ => []
            };
        }

        // TODO: make private
        public static Instruction GetBranchTarget(this Instruction branch)
        {
            if (branch.Operand is Instruction tr)
                return tr;

            if (branch.Operand is ILLabel lb)
                return lb.Target;

            return null;
        }

        /// <summary>
        /// A fixed version of Instruction.ToString() that works with ILLabel branch targets.
        /// </summary>
        /// <param name="c">The instruction to convert to a string.</param>
        /// <returns>The input instruction converted to a string.</returns>
        public static string InstructionToString(this Instruction c)
        {
            if (c == null)
                return "Null instruction";

            try
            {
                return c.ToString();
            }
            catch
            {
                try
                {
                    if (c.OpCode.OperandType is OperandType.InlineBrTarget or OperandType.ShortInlineBrTarget && c.Operand is ILLabel l)
                        return $"IL_{c.Offset:x4}: {c.OpCode.Name} IL_{l.Target.Offset:x4}";

                    if (c.OpCode.OperandType is OperandType.InlineSwitch && c.Operand is IEnumerable<ILLabel> en)
                        return $"IL_{c.Offset:x4}: {c.OpCode.Name} {string.Join(", ", en.Select(x => x.Target.Offset.ToString("x4")))}";
                }
                catch { }
            }

            return $"IL_{c.Offset:x4}: {c.OpCode.Name} (This shouldn't be happening)";
        }

        /// <summary>
        /// Gets the value of a compiler-generated IEnumerator's field.
        /// </summary>
        /// <typeparam name="T">The field's value type.</typeparam>
        /// <param name="obj">The enumerator to get the field from.</param>
        /// <param name="name">The name of the field. For fields formatted like <![CDATA[<NAME>__NUMBER]]>, giving just <![CDATA[NAME]]> works.</param>
        /// <returns>The field's value.</returns>
        public static T EnumeratorGetField<T>(this object obj, string name) => (T)obj.GetType().EnumeratorField(name).GetValue(obj);

        /// <summary>
        /// Sets the value of a compiler-generated IEnumerator's field.
        /// </summary>
        /// <param name="obj">The enumerator whose field will be set.</param>
        /// <param name="name">The name of the field. For fields formatted like <![CDATA[<NAME>__NUMBER]]>, giving just <![CDATA[NAME]]> works.</param>
        /// <param name="value">The new value for the field.</param>
        public static void EnumeratorSetField(this object obj, string name, object value) => obj.GetType().EnumeratorField(name).SetValue(obj, value);

        /// <summary>
        /// Gets a FieldInfo from the given method's declaring compiler-generated IEnumerator type.
        /// </summary>
        /// <param name="method">The method whose declaring type the field will be gotten from.</param>
        /// <param name="name">The name of the field. For fields formatted like <![CDATA[<NAME>__NUMBER]]>, giving just <![CDATA[NAME]]> works.</param>
        /// <returns>The field.</returns>
        public static FieldInfo EnumeratorField(this MethodBase method, string name) => method.DeclaringType.EnumeratorField(name);

        /// <summary>
        /// Gets a FieldInfo from the given IEnumerator type.
        /// </summary>
        /// <param name="tp">The IEnumerator type to get the field from..</param>
        /// <param name="name">The name of the field. For fields formatted like <![CDATA[<NAME>__NUMBER]]>, giving just <![CDATA[NAME]]> works.</param>
        /// <returns>The field.</returns>
        public static FieldInfo EnumeratorField(this Type tp, string name) => tp.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First(x => x != null && x.Name != null && (x.Name.Contains($"<{name}>") || x.Name == name));
    }
}
