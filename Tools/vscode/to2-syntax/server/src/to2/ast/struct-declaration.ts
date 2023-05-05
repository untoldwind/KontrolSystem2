import { Position } from "vscode-languageserver-textdocument";
import {
  Expression,
  ModuleItem,
  Node,
  TypeDeclaration,
  ValidationError,
} from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { FunctionParameter } from "./function-declaration";
import { LineComment, isLineComment } from "./line-comment";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { RecordType } from "./record-type";
import { FunctionType } from "./function-type";

export class StructField implements Node {
  public readonly range: InputRange;

  constructor(
    public readonly name: string,
    public readonly type: WithPosition<TO2Type>,
    public readonly description: string,
    public readonly initializer: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.initializer.reduceNode(combine, combine(initialValue, this));
  }
  public validate(): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}
}

export class StructDeclaration implements Node, TypeDeclaration {
  public readonly isTypeDecl: true = true;

  public readonly range: InputRange;
  public readonly name: string;
  public readonly type: TO2Type;

  constructor(
    public readonly pubKeyword: WithPosition<"pub"> | undefined,
    public readonly structKeyword: WithPosition<"struct">,
    public readonly structName: WithPosition<string>,
    public readonly description: string,
    public readonly constructorParameters: FunctionParameter[],
    public readonly fields: (LineComment | StructField)[],
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);
    this.name = this.structName.value;
    this.type = new RecordType(
      this.fields.flatMap((field) => {
        if (isLineComment(field)) return [];
        return [[field.name, field.type]];
      })
    );
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.fields.reduce(
      (prev, field) => field.reduceNode(combine, prev),
      this.constructorParameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this)
      )
    );
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.typeAliases.has(this.name)) {
      errors.push({
        status: "error",
        message: `Duplicate type name ${this.name}`,
        range: this.structName.range,
      });
    } else {
      context.typeAliases.set(this.name, this.type.realizedType(context));
    }
    if (context.mappedFunctions.has(this.name)) {
      errors.push({
        status: "error",
        message: `Duplicate function name ${this.name}`,
        range: this.structName.range,
      });
    } else {
      context.mappedFunctions.set(
        this.structName.value,
        new FunctionType(
          false,
          this.constructorParameters.map((param) => [
            param.name.value,
            param.type?.value ?? UNKNOWN_TYPE,
            param.defaultValue !== undefined,
          ]),
          this.type
        )
      );
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    if (this.pubKeyword) {
      semanticTokens.push(this.pubKeyword.range.semanticToken("keyword"));
    }
    semanticTokens.push(this.structKeyword.range.semanticToken("keyword"));
    semanticTokens.push(
      this.structName.range.semanticToken("struct", "declaration")
    );
  }
}
