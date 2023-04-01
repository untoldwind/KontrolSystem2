﻿using System;
using System.Collections.Generic;
using System.Text;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class BoundEnumType : RealizedType {
        private readonly string modulePrefix;
        public readonly string localName;
        private readonly string description;
        public readonly Type enumType;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        
        public BoundEnumType(string modulePrefix, string localName, string description, Type enumType) {
            this.modulePrefix = modulePrefix;
            this.localName = localName;
            this.description = description;
            this.enumType = enumType;
            
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory>() {
                { "to_string", new BoundMethodInvokeFactory("String representation of the number", 
                    true, () => BuiltinType.String, () => new List<RealizedParameter>(), false,
                    enumType, enumType.GetMethod("ToString", Type.EmptyTypes )) }
            };
        }

        public override string Name {
            get {
                StringBuilder builder = new StringBuilder();

                if (modulePrefix != null) {
                    builder.Append(modulePrefix);
                    builder.Append("::");
                }

                builder.Append(localName);

                return builder.ToString();
            }
        }

        public override string Description => description;

        public override string LocalName => localName;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => enumType;
    }
}
