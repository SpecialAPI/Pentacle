using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Internal
{
    internal class YarnCursor
    {
        private int _index;

        public YarnContext Context { get; }

        public Yarn.Instruction Next
        {
            get => Index < Instructions.Count && Index >= 0 ? Instructions[Index] : null;
            set => Goto(value);
        }

        public Yarn.Instruction Previous
        {
            get => Index - 1 < Instructions.Count && Index >= 0 ? Instructions[Index - 1] : null;
            set => Goto(value, MoveType.After);
        }

        public int Index
        {
            get => _index;
            set => Goto(value);
        }

        public RepeatedField<Yarn.Instruction> Instructions => Context.Instructions;

        public YarnCursor(YarnContext ctx)
        {
            Context = ctx;
            Index = 0;
        }

        public void Emit(Yarn.Instruction.Types.OpCode opcode, RepeatedField<Yarn.Operand> operands)
        {
            var instr = new Yarn.Instruction()
            {
                opcode_ = opcode,
                operands_ = operands
            };

            Emit(instr);
        }

        public void Emit(Yarn.Instruction instr)
        {
            Instructions.Insert(Index, instr);
            
            foreach(var kvp in Context.Labels.ToList())
            {
                if (kvp.Value < Index)
                    continue;

                Context.Labels[kvp.Key] = kvp.Value + 1;
            }

            Index++;
        }

        public bool TryGotoNext(MoveType moveType, Predicate<Yarn.Instruction> predicate)
        {
            for(var i = _index; i < Instructions.Count; i++)
            {
                var here = Instructions[i];

                if(!predicate.Invoke(here))
                    continue;

                Goto(i, moveType);
                return true;
            }

            return false;
        }

        public void Goto(Yarn.Instruction value, MoveType moveType = MoveType.Before)
        {
            Goto(Instructions.IndexOf(value), moveType);
        }

        public void Goto(int index, MoveType moveType = MoveType.Before)
        {
            _index = index;

            if(moveType == MoveType.After)
                _index++;
        }
    }
}
