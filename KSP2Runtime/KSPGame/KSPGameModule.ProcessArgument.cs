using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPGame;

public partial class KSPGameModule {
    [KSClass("ProcessArgument")]
    public class ProcessArgumentAdapter {
        private readonly EntrypointArgumentDescriptor entrypointArgument;

        public ProcessArgumentAdapter(EntrypointArgumentDescriptor entrypointArgument) {
            this.entrypointArgument = entrypointArgument;
        }

        [KSField] public string Name => entrypointArgument.Name;

        [KSField] public string Type => entrypointArgument.Type.Name;

        [KSField] public string DefaultValue => entrypointArgument.DefaultValue.ToString();
    }
}
