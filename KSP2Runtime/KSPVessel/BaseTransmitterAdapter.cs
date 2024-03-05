using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseTransmitterAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_Transmitter dataTransmitter;
    
    protected BaseTransmitterAdapter(P part, Data_Transmitter dataTransmitter) : base(part) {
        this.dataTransmitter = dataTransmitter;
    }

    [KSField] public double CommunicationRange => dataTransmitter.CommunicationRange;
    
    [KSField] public double DataTransmissionInterval => dataTransmitter.DataTransmissionInterval;

    [KSField] public double DataPacketSize => dataTransmitter.DataPacketSize;

    [KSField] public bool IsTransmitting => dataTransmitter.IsTransmitting.GetValue();

    [KSField] public double ActiveTransmissionSize => dataTransmitter.ActiveTransmissionSize;

    [KSField] public double ActiveTransmissionCompleted => dataTransmitter.ActiveTransmissionCompleted;

    [KSField] public bool HasResourcesToOperate => dataTransmitter.HasResourcesToOperate;
    
    [KSField]
    public KSPVesselModule.ResourceSettingAdapter[] RequiredResources =>
        dataTransmitter.RequiredResources.Select(settings => new KSPVesselModule.ResourceSettingAdapter(settings)).ToArray();

}
