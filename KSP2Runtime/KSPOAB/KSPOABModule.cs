using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

[KSModule("ksp::oab",
    Description = "Collection of types and functions to get information about the current object/vessel assembly."
)]
public partial class KSPOABModule {
    [KSFunction(
        Description = "Try to get the currently active vessel. Will result in an error if there is none."
    )]
    public static Result<ObjectAssemblyBuilderAdapter> ActiveObjectAssemblyBuilder() =>
        ObjectAssemblyBuilderAdapter.NullSafe(KSPContext.CurrentContext.Game.OAB.Current)
            .OkOr("No active object assembly");
}
