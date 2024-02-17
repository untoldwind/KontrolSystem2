using System;

namespace KontrolSystem.TO2.Binding;

[AttributeUsage(AttributeTargets.Class)]
public class KSModule : Attribute {
    public KSModule(string name) {
        Name = name;
    }

    public string Name { get; }

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class KSClass : Attribute {
    public KSClass(string? name = null) {
        Name = name;
    }

    public string? Name { get; }

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Field)]
public class KSConstant : Attribute {
    public KSConstant(string? name = null) {
        Name = name;
    }

    public string? Name { get; }

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class KSFunction : Attribute {
    public KSFunction(string? name = null) {
        Name = name;
    }

    public string? Name { get; }

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class KSMethod : Attribute {
    public KSMethod(string? name = null) {
        Name = name;
    }

    public string? Name { get; }

    public string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class KSField : Attribute {
    public KSField(string? name = null) {
        Name = name;
    }

    public string? Name { get; }

    public string? Description { get; set; }

    public bool IsAsyncStore { get; set; }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class KSParameter : Attribute {
    public KSParameter(string description) {
        Description = description;
    }

    public string Description { get; }
}
