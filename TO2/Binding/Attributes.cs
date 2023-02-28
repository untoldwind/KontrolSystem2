namespace KontrolSystem.TO2.Binding {
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class KSModule : System.Attribute {
        public KSModule(string name) => Name = name;

        public string Name { get; }

        public string Description { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface)]
    public class KSClass : System.Attribute {
        public KSClass(string name = null) => Name = name;

        public string Name { get; }

        public string Description { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class KSConstant : System.Attribute {
        public KSConstant(string name = null) => Name = name;

        public string Name { get; }

        public string Description { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class KSFunction : System.Attribute {
        public KSFunction(string name = null) => Name = name;

        public string Name { get; }

        public string Description { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class KSMethod : System.Attribute {
        public KSMethod(string name = null) => Name = name;

        public string Name { get; }

        public string Description { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
    public class KSField : System.Attribute {
        public KSField(string name = null) => Name = name;

        public string Name { get; }

        public string Description { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class KSParameter : System.Attribute {
        public KSParameter(string description) => Description = description;

        public string Description { get; }
    }
}
