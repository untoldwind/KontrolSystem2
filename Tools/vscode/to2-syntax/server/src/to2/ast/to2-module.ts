import {
  ModuleItem,
  Node,
  TypeDeclaration,
  ValidationError,
  isTypeDeclaration,
} from ".";
import { InputPosition, InputRange } from "../../parser";
import { ModuleReference } from "../../reference";
import { SemanticToken } from "../../syntax-token";
import { ConstDeclaration, isConstDeclaration } from "./const-declaration";
import { ModuleContext, RootModuleContext } from "./context";
import {
  FunctionDeclaration,
  isFunctionDeclaration,
} from "./function-declaration";
import { FunctionType } from "./function-type";
import { Registry } from "./registry";
import { TO2Type, UNKNOWN_TYPE, resolveTypeRef } from "./to2-type";
import { ReferencedType } from "./to2-type-referenced";

export interface TO2Module {
  name: string;
  description: string;

  findConstant(name: string): TO2Type | undefined;

  allConstants(): [string, TO2Type][];

  findType(name: string): TO2Type | undefined;

  allTypes(): [string, TO2Type][];

  findFunction(name: string): FunctionType | undefined;

  allFunctions(): [string, FunctionType][];
}

export class TO2ModuleNode implements Node, TO2Module {
  private constants: Map<string, ConstDeclaration> = new Map();
  private functions: Map<string, FunctionDeclaration> = new Map();
  private types: Map<string, TypeDeclaration> = new Map();
  public readonly range: InputRange;

  constructor(
    public readonly name: string,
    public readonly description: string,
    public readonly items: ModuleItem[],
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);

    for (const item of items) {
      if (isConstDeclaration(item)) this.constants.set(item.name.value, item);
      if (isFunctionDeclaration(item))
        this.functions.set(item.name.value, item);
      if (isTypeDeclaration(item)) this.types.set(item.name, item);
    }
  }

  public findConstant(name: string): TO2Type | undefined {
    return this.constants.get(name)?.type.value;
  }

  public allConstants(): [string, TO2Type][] {
    return [...this.constants.entries()].map(([name, decl]) => [
      name,
      decl.type.value,
    ]);
  }

  public findType(name: string): TO2Type | undefined {
    return undefined;
  }

  public allTypes(): [string, TO2Type][] {
    return [...this.types.entries()].map(([name, decl]) => [name, decl.type]);
  }

  public findFunction(name: string): FunctionType | undefined {
    return this.functions.get(name)?.functionType;
  }

  public allFunctions(): [string, FunctionType][] {
    return [...this.functions.entries()].map(([name, decl]) => [
      name,
      decl.functionType,
    ]);
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
    const context = new RootModuleContext(registry);
    const errors: ValidationError[] = [];

    for (const item of this.items) {
      errors.push(...item.validateModuleFirstPass(context));
    }

    for (const item of this.items) {
      errors.push(...item.validateModuleSecondPass(context));
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

  allConstants(): [string, TO2Type][] {
    return Object.entries(this.moduleReference.constants).map(
      ([name, constantReference]) => [
        name,
        resolveTypeRef(constantReference.type) ?? UNKNOWN_TYPE,
      ]
    );
  }

  findType(name: string): TO2Type | undefined {
    const aliased = this.moduleReference.typeAliases[name];
    if (aliased) return resolveTypeRef(aliased);
    const typeReference = this.moduleReference.types[name];
    return typeReference
      ? new ReferencedType(typeReference, this.name)
      : undefined;
  }

  allTypes(): [string, TO2Type][] {
    return [
      ...Object.entries(this.moduleReference.typeAliases).map<
        [string, TO2Type]
      >(([name, aliased]) => [name, resolveTypeRef(aliased) ?? UNKNOWN_TYPE]),
      ...Object.entries(this.moduleReference.types).map<[string, TO2Type]>(
        ([name, typeReference]) => [
          name,
          new ReferencedType(typeReference, this.name),
        ]
      ),
    ];
  }

  findFunction(name: string): FunctionType | undefined {
    const functionReference = this.moduleReference.functions[name];
    return functionReference
      ? new FunctionType(
          functionReference.isAsync,
          functionReference.parameters.map((param) => [
            param.name,
            resolveTypeRef(param.type) ?? UNKNOWN_TYPE,
            param.hasDefault,
          ]),
          resolveTypeRef(functionReference.returnType) ?? UNKNOWN_TYPE,
          functionReference.description
        )
      : undefined;
  }

  allFunctions(): [string, FunctionType][] {
    return Object.entries(this.moduleReference.functions).map(
      ([name, functionReference]) => [
        name,
        new FunctionType(
          functionReference.isAsync,
          functionReference.parameters.map((param) => [
            param.name,
            resolveTypeRef(param.type) ?? UNKNOWN_TYPE,
            param.hasDefault,
          ]),
          resolveTypeRef(functionReference.returnType) ?? UNKNOWN_TYPE,
          functionReference.description
        ),
      ]
    );
  }
}
