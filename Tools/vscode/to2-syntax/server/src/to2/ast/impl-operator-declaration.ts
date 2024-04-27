import { ModuleItem, Node, ValidationError } from ".";
import {
  InputPosition,
  InputRange,
  UNKNOWN_RANGE,
  WithPosition,
} from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionDeclaration } from "./function-declaration";
import { FunctionType } from "./function-type";
import { LineComment } from "./line-comment";
import { Operator } from "./operator";
import { RecordType, isRecordType } from "./record-type";
import { Registry } from "./registry";
import { ResultType } from "./result-type";
import { TO2Module } from "./to2-module";
import { TO2Type, RealizedType, currentTypeResolver } from "./to2-type";

export class ImplOperatorsDeclaration implements ModuleItem {
  public readonly range: InputRange;

  constructor(
    private readonly implKeyword: WithPosition<"impl">,
    private readonly operatorsKeyword: WithPosition<"operators">,
    public readonly name: WithPosition<string>,
    public readonly operators: (LineComment | FunctionDeclaration)[],
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.operators.reduce(
      (prev, func) => func.reduceNode(combine, prev),
      combine(initialValue, this),
    );
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const structType = this.findStructType(context);

    const implContext = new ImplOperatorsModuleContext(
      context,
      structType ?? new RecordType([]),
    );
    for (const func of this.operators) {
      errors.push(...func.validateModuleFirstPass(implContext));
    }
    errors.push(...implContext.errors);

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const structType = this.findStructType(context);
    if (!structType) {
      errors.push({
        status: "error",
        message: `Undefined struct ${this.name.value}`,
        range: this.name.range,
      });
    }

    const implContext = new ImplOperatorsModuleContext(
      context,
      structType ?? new RecordType([]),
    );
    for (const func of this.operators) {
      errors.push(...func.validateModuleSecondPass(implContext));
    }
    errors.push(...implContext.errors);

    return errors;
  }

  collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.implKeyword.range.semanticToken("keyword"));
    semanticTokens.push(this.operatorsKeyword.range.semanticToken("keyword"));
    semanticTokens.push(this.name.range.semanticToken("struct", "declaration"));

    for (const func of this.operators) {
      func.collectSemanticTokens(semanticTokens);
    }
  }

  public setModuleName(moduleName: string, context: ModuleContext) {
    for (const func of this.operators) {
      func.setModuleName(moduleName, context);
    }
  }

  private findStructType(context: ModuleContext): RecordType | undefined {
    const typeAlias = context.typeAliases
      .get(this.name.value)
      ?.realizedType(context);

    return typeAlias && isRecordType(typeAlias) ? typeAlias : undefined;
  }
}

const unaryOperatorMap: Record<string, Operator> = {
  unary_minus: "-",
};

const binaryOperatorMap: Record<string, Operator> = {
  add: "+",
  sub: "-",
  mul: "*",
  div: "/",
  mod: "%",
};

class ImplOperatorsModuleContext implements ModuleContext {
  public readonly moduleName: string;
  public readonly mappedConstants: Map<string, WithDefinitionRef<TO2Type>> =
    new Map();
  public readonly moduleAliases: Map<string, string[]> = new Map();
  public readonly typeAliases: Map<string, TO2Type> = new Map();
  public readonly registry: Registry;
  public readonly errors: ValidationError[] = [];

  constructor(
    private readonly root: ModuleContext,
    public readonly structType: RecordType,
  ) {
    this.moduleName = root.moduleName;
    this.registry = root.registry;
    this.mappedConstants = root.mappedConstants;
    this.moduleAliases = root.moduleAliases;
    this.typeAliases = root.typeAliases;
  }

  findType(
    namePath: string[],
    typeArguments: RealizedType[],
  ): RealizedType | undefined {
    return this.root.findType(namePath, typeArguments);
  }

  findConstant(namePath: string[]): WithDefinitionRef<TO2Type> | undefined {
    return this.root.findConstant(namePath);
  }

  allConstants(): [string, TO2Type][] {
    return this.root.allConstants();
  }

  findFunction(
    namePath: string[],
    typeHint?: RealizedType,
  ): WithDefinitionRef<FunctionType> | undefined {
    return this.root.findFunction(namePath, typeHint);
  }

  allFunctions(): [string, FunctionType][] {
    return this.root.allFunctions();
  }

  findModule(namePath: string[]): TO2Module | undefined {
    return this.root.findModule(namePath);
  }

  registerLocalFunction(
    name: string,
    func: WithDefinitionRef<FunctionType>,
  ): void {
    const isUnary = name in unaryOperatorMap;
    const isBinary = name in binaryOperatorMap;

    if (!isUnary && !isBinary) {
      this.errors.push({
        status: "error",
        message: `Operator ${name} is not in allowed list of operators [${[...Object.getOwnPropertyNames(unaryOperatorMap), ...Object.getOwnPropertyNames(binaryOperatorMap)].join(", ")}]`,
        range: func.definition?.range ?? UNKNOWN_RANGE,
      });
      return;
    }
    if (isUnary) {
      if (func.value.parameterTypes.length != 1) {
        this.errors.push({
          status: "error",
          message: `Unary operator ${name} must have exactly 1 parameter`,
          range: func.definition?.range ?? UNKNOWN_RANGE,
        });
        return;
      }
      if (
        !func.value.parameterTypes[0][1]
          .realizedType(this)
          .isAssignableFrom(this.structType)
      ) {
        this.errors.push({
          status: "error",
          message: `Parameter of unary operator ${name} has to be ${this.structType.name}`,
          range: func.definition?.range ?? UNKNOWN_RANGE,
        });
        return;
      }

      const ops =
        this.structType.prefixOperators.get(unaryOperatorMap[name]) ?? [];
      this.structType.prefixOperators.set(unaryOperatorMap[name], [
        ...ops,
        {
          otherType: currentTypeResolver().BUILTIN_UNIT,
          resultType: func.value.returnType.realizedType(this),
        },
      ]);
    } else {
      if (func.value.parameterTypes.length != 2) {
        this.errors.push({
          status: "error",
          message: `Unary operator ${name} must have exactly 2 parameters`,
          range: func.definition?.range ?? UNKNOWN_RANGE,
        });
        return;
      }

      const isSuffix = func.value.parameterTypes[0][1]
        .realizedType(this)
        .isAssignableFrom(this.structType);
      const isPrefix = func.value.parameterTypes[1][1]
        .realizedType(this)
        .isAssignableFrom(this.structType);

      if (!isSuffix && !isPrefix) {
        this.errors.push({
          status: "error",
          message: `One of the parameter of binary operator ${name} has to be ${this.structType.name}`,
          range: func.definition?.range ?? UNKNOWN_RANGE,
        });
        return;
      }
      if (isSuffix) {
        const ops =
          this.structType.suffixOperators.get(binaryOperatorMap[name]) ?? [];
        this.structType.suffixOperators.set(binaryOperatorMap[name], [
          ...ops,
          {
            otherType: func.value.parameterTypes[1][1].realizedType(this),
            resultType: func.value.returnType.realizedType(this),
          },
        ]);
      } else {
        const ops =
          this.structType.prefixOperators.get(binaryOperatorMap[name]) ?? [];
        this.structType.prefixOperators.set(binaryOperatorMap[name], [
          ...ops,
          {
            otherType: func.value.parameterTypes[0][1].realizedType(this),
            resultType: func.value.returnType.realizedType(this),
          },
        ]);
      }
    }
  }
}
