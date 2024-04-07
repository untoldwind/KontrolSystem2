import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { Node, ValidationError } from ".";
import { InputPosition, InputRange } from "../../parser";
import { ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class LookupTypeReference implements Node, TO2Type {
  public readonly name: string;
  public readonly description: string;
  public readonly localName: string;
  public readonly range: InputRange;
  private lookupContext?: ModuleContext;

  constructor(
    public readonly namePath: string[],
    public readonly typeArguments: TO2Type[],
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
    this.name = namePath.join("::");
    this.description = "";
    this.localName = namePath.join("::");
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validate(): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public realizedType(context: ModuleContext): RealizedType {
    const effectiveContext = this.lookupContext ?? context;

    return (
      effectiveContext.findType(
        this.namePath,
        this.typeArguments.map((type) => type.realizedType(effectiveContext)),
      ) ?? UNKNOWN_TYPE
    );
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}

  public setModuleName(moduleName: string, context: ModuleContext): void {
    this.lookupContext = context;
    this.typeArguments.forEach((type) =>
      type.setModuleName?.(moduleName, context),
    );
  }
}
