import { Input, Parser, ParserFailure, ParserSuccess } from ".";

export function tag(tag : string) : Parser<string> {
    return (input : Input) => {
        if (input.available() < tag.length) return new ParserFailure(`Expected ${tag}`);
        const content = input.take(tag.length);
        if(content !== tag) return new ParserFailure(`Expected ${tag}`);
        return new ParserSuccess(input.advance(tag.length), content);
    }
}