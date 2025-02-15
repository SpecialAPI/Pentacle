using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomFogs
{
    internal class YarnContext
    {
        public Yarn.Program Program;
        public Yarn.Node Node;

        public RepeatedField<Yarn.Instruction> Instructions => Node.Instructions;
        public MapField<string, int> Labels => Node.Labels;
    }
}
