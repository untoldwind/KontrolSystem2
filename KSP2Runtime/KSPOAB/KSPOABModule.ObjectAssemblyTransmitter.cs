using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyTransmitter")]
    public class ObjectAssemblyTransmitterAdapter : BaseTransmitterAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart> {
        public ObjectAssemblyTransmitterAdapter(ObjectAssemblyPartAdapter part, Data_Transmitter dataTransmitter) : base(part, dataTransmitter) {
        }
    }
}
