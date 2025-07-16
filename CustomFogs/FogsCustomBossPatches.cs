using Google.Protobuf.Collections;
using Pentacle.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Yarn.Unity;

namespace Pentacle.CustomFogs
{
    [HarmonyPatch]
    internal static class FogsCustomBossPatches
    {
        private const string FOGS_DIALOGUE_PROGRAM = "NPC_Fogs_Dialog";
        private const string FOGS_DIALOGUE_START_NODE = "Fogs.End.Start";
        private const string FOGS_DIALOGUE_FIRSTTALK_NODE = "Fogs.End.FirstTalk";
        private const string FOGS_DIALOGUE_DONE_NODE = "Fogs.End.TransactionDone";

        private const string CUSTOMBOSS_LINEID_FORMAT = "FOGS_CUSTOMBOSS_{0}";
        private const string CUSTOMBOSS_NODE_FORMAT = "Fogs.End.CUSTOMBOSS_{0}_Chosen";

        internal static void AddFogsTranspiler()
        {
            YarnTranspiler.AddTranspiler(FOGS_DIALOGUE_PROGRAM, FOGS_DIALOGUE_START_NODE, AddCustomFinalBossCheck_YarnTranspiler);
            YarnTranspiler.AddTranspiler(FOGS_DIALOGUE_PROGRAM, FOGS_DIALOGUE_FIRSTTALK_NODE, AddFinalBossOptions_YarnTranspiler);
        }

        private static void AddCustomFinalBossCheck_YarnTranspiler(YarnContext ctx)
        {
            var crs = new YarnCursor(ctx);

            if (!crs.TryGotoNext(MoveType.After, x =>
                    x.Opcode == Yarn.Instruction.Types.OpCode.CallFunc &&
                    x.Operands.Count == 1 && x.Operands[0] != null && x.Operands[0].StringValue == "Visited"))
                return;

            var checkCustomBossesVisited = YarnTranspiler.AddYarnOpCode(MOD_GUID, "CustomFogs_CheckCustomBossesVisisted", AddCustomFinalBossCheck_OpCode);
            var op = new RepeatedField<Yarn.Operand>();

            foreach (var bossId in FogsCustomBossManager.CustomFogsBosses.Keys)
                op.Add(new Yarn.Operand(bossId));

            crs.Emit(checkCustomBossesVisited, op);
        }

        private static void AddCustomFinalBossCheck_OpCode(Yarn.VirtualMachine machine, RepeatedField<Yarn.Operand> operands)
        {
            if (InfoHolder == null || !InfoHolder.HasRunData || InfoHolder.Run.InGameData == null)
                return;

            var curr = machine.state.PopValue();

            if (curr.type != Yarn.Value.Type.Bool)
            {
                machine.state.PushValue(curr);
                return;
            }

            var visited = curr.BoolValue;
            var dat = InfoHolder.Run.InGameData;

            foreach (var op in operands)
            {
                var node = string.Format(CUSTOMBOSS_NODE_FORMAT, op.StringValue);

                if (!dat.ContainsVisitedNode(node))
                    continue;

                visited = true;
                break;
            }

            machine.state.PushValue(visited);
        }

        private static void AddFinalBossOptions_YarnTranspiler(YarnContext ctx)
        {
            var crs = new YarnCursor(ctx);

            for (var i = 0; i < 2; i++)
            {
                if (!crs.TryGotoNext(MoveType.After, x => x.Opcode == Yarn.Instruction.Types.OpCode.AddOption))
                    return;
            }

            var addFinalBosses = YarnTranspiler.AddYarnOpCode(MOD_GUID, "CustomFogs_AddFinalBossOptions", AddFinalBossOptions_OpCode);
            var op = new RepeatedField<Yarn.Operand>();

            foreach (var bossId in FogsCustomBossManager.CustomFogsBosses.Keys)
            {
                op.Add(new Yarn.Operand(bossId));

                var node = string.Format(CUSTOMBOSS_NODE_FORMAT, bossId);
                ctx.Program.Nodes.Add(node, BuildCustomFogsBossNode(bossId, node));
            }

            crs.Emit(addFinalBosses, op);
        }

        private static void AddFinalBossOptions_OpCode(Yarn.VirtualMachine machine, RepeatedField<Yarn.Operand> operands)
        {
            foreach (var op in operands)
            {
                machine.state.currentOptions.Add(new(new()
                {
                    ID = string.Format(CUSTOMBOSS_LINEID_FORMAT, op.StringValue),
                    Substitutions = []
                }, string.Format(CUSTOMBOSS_NODE_FORMAT, op.StringValue)));
            }
        }

        private static Yarn.Node BuildCustomFogsBossNode(string bossId, string name)
        {
            return new()
            {
                instructions_ = new()
                {
                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.PushFloat,
                        operands_ = [new Yarn.Operand(0f)]
                    },

                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.CallFunc,
                        operands_ = [new Yarn.Operand(DataUtils.amountToPayFogsFunction)]
                    },

                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.RunCommand,
                        operands_ = [new Yarn.Operand($"{DataUtils.removeCurrencyCommand} {{0}}"), new Yarn.Operand(1f)]
                    },

                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.RunCommand,
                        operands_ = [new Yarn.Operand($"{DataUtils.SwapNextZoneBossCommand} {bossId}"), new Yarn.Operand(0f)]
                    },

                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.RunCommand,
                        operands_ = [new Yarn.Operand(DataUtils.saveProgressCommand), new Yarn.Operand(0f)]
                    },

                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.RunNode,
                        operands_ = [new Yarn.Operand(FOGS_DIALOGUE_DONE_NODE)]
                    },

                    new Yarn.Instruction()
                    {
                        opcode_ = Yarn.Instruction.Types.OpCode.Stop,
                        operands_ = []
                    },
                },
                labels_ = [],
                tags_ = [],
                name_ = name,
            };
        }

        [HarmonyPatch(typeof(DialogueRunner), nameof(DialogueRunner.AddStringTable), typeof(YarnProgram))]
        [HarmonyPostfix]
        private static void AddCustomFinalBossStrings(DialogueRunner __instance, YarnProgram yarnScript)
        {
            if (__instance == null || __instance.strings == null)
                return;

            if (yarnScript == null || yarnScript.name != FOGS_DIALOGUE_PROGRAM)
                return;

            foreach (var kvp in FogsCustomBossManager.CustomFogsBosses)
            {
                var id = string.Format(CUSTOMBOSS_LINEID_FORMAT, kvp.Key);

                __instance.strings[id] = kvp.Value;
            }
        }
    }
}
