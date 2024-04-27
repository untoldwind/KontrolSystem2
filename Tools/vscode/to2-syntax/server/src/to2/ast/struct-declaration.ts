import { Expression, Node, TypeDeclaration, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE, currentTypeResolver } from "./to2-type";
import { FunctionParameter } from "./function-declaration";
import { LineComment, isLineComment } from "./line-comment";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext, FunctionContext, ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { RecordType } from "./record-type";
import { FunctionType } from "./function-type";

export class StructField implements Node {
  public readonly range: InputRange;

  constructor(
    public readonly name: WithPosition<string>,
    public readonly type: WithPosition<TO2Type>,
    public readonly description: string,
    public readonly initializer: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.initializer.reduceNode(combine, combine(initialValue, this));
  }

  public validate(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(
      ...this.initializer.validateBlock(
        context,
        this.type.value.realizedType(context.module),
      ),
    );

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}
}

export class StructDeclaration implements Node, TypeDeclaration {
  public readonly isTypeDecl: true = true;
  public readonly range: InputRange;
  public readonly name: WithPosition<string>;
  public type: TO2Type;
  public readonly constructorType: FunctionType;

  constructor(
    public readonly pubKeyword: WithPosition<"pub"> | undefined,
    public readonly structKeyword: WithPosition<"struct">,
    public readonly structName: WithPosition<string>,
    public readonly description: string,
    public readonly constructorParameters: FunctionParameter[],
    public readonly fields: (LineComment | StructField)[],
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
    this.name = this.structName;
    this.type = new RecordType(
      this.fields.flatMap((field) => {
        if (isLineComment(field)) return [];
        return [[field.name, field.type.value]];
      }),
      undefined,
      undefined,
      undefined,
      this.name.value,
    );
    this.constructorType = new FunctionType(
      false,
      this.constructorParameters.map((param) => [
        param.name.value,
        param.type?.value ?? UNKNOWN_TYPE,
        param.defaultValue !== undefined,
      ]),
      this.type,
    );
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.fields.reduce(
      (prev, field) => field.reduceNode(combine, prev),
      this.constructorParameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this),
      ),
    );
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.typeAliases.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate type name ${this.name}`,
        range: this.structName.range,
      });
    } else {
      context.typeAliases.set(this.name.value, this.type.realizedType(context));
    }
    if (context.findFunction([this.name.value])) {
      errors.push({
        status: "error",
        message: `Duplicate function name ${this.name}`,
        range: this.structName.range,
      });
    } else {
      context.registerLocalFunction(this.structName.value, {
        definition: {
          moduleName: context.moduleName,
          range: this.structName.range,
        },
        value: this.constructorType,
      });
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const blockContext = new FunctionContext(
      context,
      currentTypeResolver().BUILTIN_UNIT,
    );

    for (const parameter of this.constructorParameters) {
      errors.push(...parameter.validateBlock(blockContext));
      blockContext.localVariables.set(parameter.name.value, {
        definition: {
          moduleName: context.moduleName,
          range: parameter.name.range,
        },
        value: parameter.resultType(blockContext),
      });
    }
    for (const field of this.fields) {
      if (isLineComment(field)) continue;
      errors.push(...field.validate(blockContext));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    if (this.pubKeyword) {
      semanticTokens.push(this.pubKeyword.range.semanticToken("keyword"));
    }
    semanticTokens.push(this.structKeyword.range.semanticToken("keyword"));
    semanticTokens.push(
      this.structName.range.semanticToken("struct", "declaration"),
    );
  }

  public setModuleName(moduleName: string, context: ModuleContext) {
    this.type.setModuleName?.(moduleName, context);
  }
}
