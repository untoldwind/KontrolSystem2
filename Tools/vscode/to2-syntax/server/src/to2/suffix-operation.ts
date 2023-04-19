import { Operator } from "./ast/operator";

export interface SuffixOperation {

}

export class OperatorSuffix implements SuffixOperation {
    constructor(public readonly op: Operator) {
    }
}