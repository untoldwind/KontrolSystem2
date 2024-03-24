using System;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator;

public static class InstructionSize {
    public static int Get(OpCode op, int labelCount = 0) {
        var baseSize = op.Size;
        var operandSize = op.OperandType switch {
            OperandType.InlineBrTarget => 4,
            OperandType.InlineField => 4,
            OperandType.InlineI => 4,
            OperandType.InlineI8 => 8,
            OperandType.InlineMethod => 4,
            OperandType.InlineNone => 0,
            OperandType.InlineR => 8,
            OperandType.InlineSig => 4,
            OperandType.InlineString => 4,
            OperandType.InlineSwitch => 4 + labelCount * 4,
            OperandType.InlineTok => 4,
            OperandType.InlineType => 4,
            OperandType.InlineVar => 2,
            OperandType.ShortInlineBrTarget => 1,
            OperandType.ShortInlineI => 1,
            OperandType.ShortInlineR => 4,
            OperandType.ShortInlineVar => 1,
            _ => throw new InvalidOperationException($"Unexpected operand type {op.OperandType}"),
        };
        return baseSize + operandSize;
    }

    public static int StackDiff(OpCode opCode, int? varPop, int? varPush) {
        var stackCount = 0;
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
