namespace KontrolSystem.TO2.AST;

public enum IndexSpecType {
    Single
}

public class IndexSpec(Expression index) {
    public readonly IndexSpecType indexType = IndexSpecType.Single;
    public readonly Expression start = index;

    public IVariableContainer? VariableContainer {
        set => start.VariableContainer = value;
    }

    public override string ToString() => $"{indexType}";
}
