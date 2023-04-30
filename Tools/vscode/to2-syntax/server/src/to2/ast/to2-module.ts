import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition } from "../../parser";
import { ModuleReference } from "../../reference";
import { SemanticToken } from "../../syntax-token";
import { ConstDeclaration, isConstDeclaration } from "./const-declaration";
import { ModuleContext } from "./context";
import {
  FunctionDeclaration,
  isFunctionDeclaration,
} from "./function-declaration";
import { FunctionType } from "./function-type";
import { Registry } from "./registry";
import {
  ReferencedType,
  TO2Type,
  UNKNOWN_TYPE,
  resolveTypeRef,
} from "./to2-type";

export interface TO2Module {
  name: string;
  description: string;

  findConstant(name: string): TO2Type | undefined;

  findType(name: string): TO2Type | undefined;

  findFunction(name: string): FunctionType | undefined;
}

export class TO2ModuleNode implements Node, TO2Module {
  private constants: Map<string, ConstDeclaration> = new Map();
  private functions: Map<string, FunctionDeclaration> = new Map();
  constructor(
    public readonly name: string,
    public readonly description: string,
    public readonly items: ModuleItem[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {
    for (const item of items) {
      if (isConstDeclaration(item)) this.constants.set(item.name.value, item);
      if (isFunctionDeclaration(item))
        this.functions.set(item.name.value, item);
    }
  }

  public findConstant(name: string): TO2Type | undefined {
    return this.constants.get(name)?.type;
  }

  public findType(name: string): TO2Type | undefined {
    return undefined;
  }

  public findFunction(name: string): FunctionType | undefined {
    return this.functions.get(name)?.functionType;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validate(registry: Registry): ValidationError[] {
    const context = new ModuleContext(registry);
    const errors: ValidationError[] = [];

    for (const item of this.items) {
      errors.push(...item.validateModule(context));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const item of this.items) {
      item.collectSemanticTokens(semanticTokens);
    }
  }
}

export class ReferencedModule implements TO2Module {
  public readonly name: string;
  public readonly description: string;

  constructor(private readonly moduleReference: ModuleReference) {
    this.name = moduleReference.name;
    this.description = moduleReference.description || "";
  }

  findConstant(name: string): TO2Type | undefined {
    const constantReference = this.moduleReference.constants[name];
    return constantReference
      ? resolveTypeRef(constantReference.type)
      : undefined;
  }

  findType(name: string): TO2Type | undefined {
    const typeReference = this.moduleReference.types[name];
    return typeReference ? new ReferencedType(typeReference) : undefined;
  }

  findFunction(name: string): FunctionType | undefined {
    const functionReference = this.moduleReference.functions[name];
    return functionReference
      ? new FunctionType(
          functionReference.isAsync,
          functionReference.parameters.map((param) => [
            param.name,
            resolveTypeRef(param.type) ?? UNKNOWN_TYPE,
          ]),
          resolveTypeRef(functionReference.returnType) ?? UNKNOWN_TYPE,
          functionReference.description
        )
      : undefined;
  }
}
