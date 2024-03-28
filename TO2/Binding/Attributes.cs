using System;

namespace KontrolSystem.TO2.Binding;

[AttributeUsage(AttributeTargets.Class)]
public class KSModule(string name) : Attribute {
    public string Name { get; } = name;

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class KSClass(string? name = null) : Attribute {
    public string? Name { get; } = name;

    public string? Description { get; set; }

    public Type[]? ScanInterfaces { get; set; }
}

[AttributeUsage(AttributeTargets.Field)]
public class KSConstant(string? name = null) : Attribute {
    public string? Name { get; } = name;

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class KSFunction(string? name = null) : Attribute {
    public string? Name { get; } = name;

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class KSMethod(string? name = null) : Attribute {
    public string? Name { get; } = name;

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class KSField(string? name = null) : Attribute {
    public string? Name { get; } = name;

    public string? Description { get; set; }

    public bool IsAsyncStore { get; set; }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class KSParameter(string description) : Attribute {
    public string Description { get; } = description;
}
