using System;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator {
    public static class InstructionSize {
        public static int Get(OpCode op, int labelCount = 0) {
            var baseSize = op.Size;
            int operandSize;

            switch (op.OperandType) {
            case OperandType.InlineBrTarget:
                operandSize = 4;
                break;
            case OperandType.InlineField:
                operandSize = 4;
                break;
            case OperandType.InlineI:
                operandSize = 4;
                break;
            case OperandType.InlineI8:
                operandSize = 8;
                break;
            case OperandType.InlineMethod:
                operandSize = 4;
                break;
            case OperandType.InlineNone:
                operandSize = 0;
                break;
            case OperandType.InlineR:
                operandSize = 8;
                break;
            case OperandType.InlineSig:
                operandSize = 4;
                break;
            case OperandType.InlineString:
                operandSize = 4;
                break;
            case OperandType.InlineSwitch:
                operandSize = 4 + labelCount * 4;
                break;
            case OperandType.InlineTok:
                operandSize = 4;
                break;
            case OperandType.InlineType:
                operandSize = 4;
                break;
            case OperandType.InlineVar:
                operandSize = 2;
                break;
            case OperandType.ShortInlineBrTarget:
                operandSize = 1;
                break;
            case OperandType.ShortInlineI:
                operandSize = 1;
                break;
            case OperandType.ShortInlineR:
                operandSize = 4;
                break;
            case OperandType.ShortInlineVar:
                operandSize = 1;
                break;
            default:
                throw new InvalidOperationException($"Unexpected operand type {op.OperandType}");
            }

            return baseSize + operandSize;
        }

        public static int StackDiff(OpCode opCode, int? varPop, int? varPush) {
            int stackCount = 0;
            switch (opCode.StackBehaviourPop) {
            case StackBehaviour.Pop0:
                break;
            case StackBehaviour.Pop1:
            case StackBehaviour.Popi:
            case StackBehaviour.Popref:
                stackCount--;
                break;
            case StackBehaviour.Pop1_pop1:
            case StackBehaviour.Popref_pop1:
            case StackBehaviour.Popref_popi:
                stackCount -= 2;
                break;
            case StackBehaviour.Popref_popi_popi:
            case StackBehaviour.Popref_popi_pop1:
            case StackBehaviour.Popref_popi_popi8:
            case StackBehaviour.Popref_popi_popr8:
                stackCount -= 3;
                break;
            case StackBehaviour.Varpop:
                if (!varPop.HasValue)
                    throw new CodeGenerationException($"Got stack behaviour Varpop without count for {opCode}");
                stackCount -= varPop.Value;
                break;
            default:
                throw new CodeGenerationException(
                    $"Invalid pop stack behaviour for {opCode}: {opCode.StackBehaviourPop}");
            }

            switch (opCode.StackBehaviourPush) {
            case StackBehaviour.Push0:
                break;
            case StackBehaviour.Push1:
            case StackBehaviour.Pushi:
            case StackBehaviour.Pushi8:
            case StackBehaviour.Pushr8:
            case StackBehaviour.Pushref:
                stackCount++;
                break;
            case StackBehaviour.Push1_push1:
                stackCount += 2;
                break;
            case StackBehaviour.Varpush:
                if (!varPush.HasValue)
                    throw new CodeGenerationException($"Got stack behaviour Varpush without count for {opCode}");
                stackCount += varPush.Value;
                break;
            default:
                throw new CodeGenerationException(
                    $"Invalid push stack behaviour for {opCode}: {opCode.StackBehaviourPush}");
            }

            return stackCount;
        }
    }
}
