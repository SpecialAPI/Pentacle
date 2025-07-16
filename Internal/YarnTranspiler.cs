using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Internal
{
    [HarmonyPatch]
    internal static class YarnTranspiler
    {
        private static readonly Dictionary<string, Dictionary<string, List<Action<YarnContext>>>> Transpilers = [];
        private static readonly Dictionary<Yarn.Instruction.Types.OpCode, Action<Yarn.VirtualMachine, RepeatedField<Yarn.Operand>>> CustomOpCodes = [];

        public static void AddTranspiler(string program, string node, Action<YarnContext> transpiler)
        {
            if (!Transpilers.TryGetValue(program, out var forProgram))
                Transpilers[program] = forProgram = [];

            if (!forProgram.TryGetValue(node, out var transpilers))
                forProgram[node] = transpilers = [];

            transpilers.Add(transpiler);
        }

        public static Yarn.Instruction.Types.OpCode AddYarnOpCode(string guid, string name, Action<Yarn.VirtualMachine, RepeatedField<Yarn.Operand>> runInstruction)
        {
            var e = EnumExtension.ExtendEnum<Yarn.Instruction.Types.OpCode>(guid, name);
            CustomOpCodes[e] = runInstruction;

            return e;
        }

        [HarmonyPatch(typeof(YarnProgram), nameof(YarnProgram.GetProgram))]
        [HarmonyPostfix]
        private static void HandleTranspilers(YarnProgram __instance, Yarn.Program __result)
        {
            if (__result == null || __instance == null || string.IsNullOrEmpty(__instance.name))
                return;

            if (!Transpilers.TryGetValue(__instance.name, out var forProgram))
                return;

            foreach(var kvp in forProgram)
            {
                var nodeName = kvp.Key;
                var trans = kvp.Value;

                if (!__result.Nodes.TryGetValue(nodeName, out var node))
                    continue;

                var ctx = new YarnContext()
                {
                    Node = node,
                    Program = __result
                };

                foreach(var t in trans)
                    t?.Invoke(ctx);
            }
        }

        [HarmonyPatch(typeof(Yarn.VirtualMachine), nameof(Yarn.VirtualMachine.RunInstruction))]
        [HarmonyPrefix]
        private static bool RunCustomOpCodes(Yarn.VirtualMachine __instance, Yarn.Instruction i)
        {
            if (i == null)
                return true;

            if (!CustomOpCodes.TryGetValue(i.Opcode, out var a))
                return true;

            a?.Invoke(__instance, i.Operands);
            return false;
        }
    }
}
