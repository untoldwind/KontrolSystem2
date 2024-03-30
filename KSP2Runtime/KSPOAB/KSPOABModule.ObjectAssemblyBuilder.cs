using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.OAB;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyBuilder", Description = "Represents the current object assembly builder.")]
    public class ObjectAssemblyBuilderAdapter(ObjectAssemblyBuilder objectAssemblyBuilder) {
        [KSField(Description = "Get the current main assembly if there is one.")]
        public Option<ObjectAssemblyAdapter> MainAssembly {
            get {
                var mainAssembly = objectAssemblyBuilder.Stats.MainAssembly;

                return mainAssembly != null ? new Option<ObjectAssemblyAdapter>(new ObjectAssemblyAdapter(mainAssembly)) : new Option<ObjectAssemblyAdapter>();
            }
        }

        [KSField(Description = "Get all object assemblies (i.e. all parts that are not fully connected)")]
        public ObjectAssemblyAdapter[] Assemblies => objectAssemblyBuilder.ActivePartTracker.partAssemblies
            .Select(assembly => new ObjectAssemblyAdapter(assembly)).ToArray();

        public static Option<ObjectAssemblyBuilderAdapter> NullSafe(ObjectAssemblyBuilder? objectAssemblyBuilder) =>
            objectAssemblyBuilder != null
                ? new Option<ObjectAssemblyBuilderAdapter>(new ObjectAssemblyBuilderAdapter(objectAssemblyBuilder))
                : new Option<ObjectAssemblyBuilderAdapter>();
    }
}
