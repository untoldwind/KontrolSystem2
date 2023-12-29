import { DocumentUri, InlayHint } from "vscode-languageserver";
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
import { WithDefinitionRef } from "./definition-ref";

export interface TO2Module {
  name: string;
  description: string;

  findConstant(name: string): WithDefinitionRef<TO2Type> | undefined;

  allConstants(): [string, WithDefinitionRef<TO2Type>][];

  findType(name: string): WithDefinitionRef<TO2Type> | undefined;

  allTypes(): [string, WithDefinitionRef<TO2Type>][];

  findFunction(name: string): WithDefinitionRef<FunctionType> | undefined;

  allFunctions(): [string, WithDefinitionRef<FunctionType>][];
}

export class TO2ModuleNode implements Node, TO2Module {
  private constants: Map<string, ConstDeclaration> = new Map();
  private functions: Map<string, FunctionDeclaration> = new Map();
  private types: Map<string, TypeDeclaration> = new Map();
  public readonly range: InputRange;

  constructor(
    public readonly documentUri: DocumentUri,
    public readonly name: string,
    public readonly description: string,
    public readonly items: ModuleItem[],
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);

    for (const item of items) {
      item.setModuleName(name);
      if (isConstDeclaration(item)) this.constants.set(item.name.value, item);
      if (isFunctionDeclaration(item))
        this.functions.set(item.name.value, item);
      if (isTypeDeclaration(item)) this.types.set(item.name.value, item);
    }
  }

  public findConstant(name: string): WithDefinitionRef<TO2Type> | undefined {
    const decl = this.constants.get(name);

    return decl
      ? {
          definition: { moduleName: this.name, range: decl.name.range },
          value: decl.type.value,
        }
      : undefined;
  }

  public allConstants(): [string, WithDefinitionRef<TO2Type>][] {
    return [...this.constants.entries()].map(([name, decl]) => [
      name,
      {
        definition: { moduleName: this.name, range: decl.name.range },
        value: decl.type.value,
      },
    ]);
  }

  public findType(name: string): WithDefinitionRef<TO2Type> | undefined {
    const decl = this.types.get(name);

    return decl
      ? {
          definition: { moduleName: this.name, range: decl.name.range },
          value: decl.type,
        }
      : undefined;
  }

  public allTypes(): [string, WithDefinitionRef<TO2Type>][] {
    return [...this.types.entries()].map(([name, decl]) => [
      name,
      {
        definition: { moduleName: this.name, range: decl.name.range },
        value: decl.type,
      },
    ]);
  }

  public findFunction(
    name: string,
  ): WithDefinitionRef<FunctionType> | undefined {
    const functionDecl = this.functions.get(name);
    if (functionDecl) {
      return {
        definition: { moduleName: this.name, range: functionDecl.name.range },
        value: functionDecl.functionType,
      };
    }
    const typeDecl = this.types.get(name);
    if (typeDecl && typeDecl?.constructorType) {
      return {
        definition: { moduleName: this.name, range: typeDecl.name.range },
        value: typeDecl.constructorType,
      };
    }

    return undefined;
  }

  public allFunctions(): [string, WithDefinitionRef<FunctionType>][] {
    const result: [string, WithDefinitionRef<FunctionType>][] = [];

    for (const [name, decl] of this.functions.entries()) {
      result.push([
        name,
        {
          definition: { moduleName: this.name, range: decl.name.range },
          value: decl.functionType,
        },
      ]);
    }
    for (const [name, decl] of this.types.entries()) {
      if (!decl.constructorType) continue;
      result.push([
        name,
        {
          definition: { moduleName: this.name, range: decl.name.range },
          value: decl.constructorType,
        },
      ]);
    }
    return result;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this),
    );
  }

  public validate(registry: Registry): ValidationError[] {
    const context = new RootModuleContext(this.name, registry);
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

export function isTO2ModuleNode(module: TO2Module): module is TO2ModuleNode {
  return (module as TO2ModuleNode).documentUri !== undefined;
}

export class ReferencedModule implements TO2Module {
  public readonly name: string;
  public readonly description: string;

  constructor(private readonly moduleReference: ModuleReference) {
    this.name = moduleReference.name;
    this.description = moduleReference.description || "";
  }

  findConstant(name: string): WithDefinitionRef<TO2Type> | undefined {
    const constantReference = this.moduleReference.constants[name];
    const type = constantReference
      ? resolveTypeRef(constantReference.type)
      : undefined;
    return type ? { value: type } : undefined;
  }

  allConstants(): [string, WithDefinitionRef<TO2Type>][] {
    return Object.entries(this.moduleReference.constants).map(
      ([name, constantReference]) => [
        name,
        { value: resolveTypeRef(constantReference.type) ?? UNKNOWN_TYPE },
      ],
    );
  }

  findType(name: string): WithDefinitionRef<TO2Type> | undefined {
    const aliased = this.moduleReference.typeAliases[name];
    if (aliased) {
      const value = resolveTypeRef(aliased);

      return value ? { value } : undefined;
    }

    const typeReference = this.moduleReference.types[name];
    return typeReference
      ? { value: new ReferencedType(typeReference, this.name) }
      : undefined;
  }

  allTypes(): [string, WithDefinitionRef<TO2Type>][] {
    return [
      ...Object.entries(this.moduleReference.typeAliases).map<
        [string, WithDefinitionRef<TO2Type>]
      >(([name, aliased]) => [
        name,
        { value: resolveTypeRef(aliased) ?? UNKNOWN_TYPE },
      ]),
      ...Object.entries(this.moduleReference.types).map<
        [string, WithDefinitionRef<TO2Type>]
      >(([name, typeReference]) => [
        name,
        { value: new ReferencedType(typeReference, this.name) },
      ]),
    ];
  }

  findFunction(name: string): WithDefinitionRef<FunctionType> | undefined {
    const functionReference = this.moduleReference.functions[name];
    return functionReference
      ? {
          value: new FunctionType(
            functionReference.isAsync,
            functionReference.parameters.map((param) => [
              param.name,
              resolveTypeRef(param.type) ?? UNKNOWN_TYPE,
              param.hasDefault,
            ]),
            resolveTypeRef(functionReference.returnType) ?? UNKNOWN_TYPE,
            functionReference.description,
          ),
        }
      : undefined;
  }

  allFunctions(): [string, WithDefinitionRef<FunctionType>][] {
    return Object.entries(this.moduleReference.functions).map(
      ([name, functionReference]) => [
        name,
        {
          value: new FunctionType(
            functionReference.isAsync,
            functionReference.parameters.map((param) => [
              param.name,
              resolveTypeRef(param.type) ?? UNKNOWN_TYPE,
              param.hasDefault,
            ]),
            resolveTypeRef(functionReference.returnType) ?? UNKNOWN_TYPE,
            functionReference.description,
          ),
        },
      ],
    );
  }
}
