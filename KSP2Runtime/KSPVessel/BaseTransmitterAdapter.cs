﻿using System.Linq;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseTransmitterAdapter<P, T>(P part, Data_Transmitter dataTransmitter)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Transmitter dataTransmitter = dataTransmitter;

    [KSField] public double CommunicationRange => dataTransmitter.CommunicationRange;

    [KSField] public double DataTransmissionInterval => dataTransmitter.DataTransmissionInterval;

    [KSField] public double DataPacketSize => dataTransmitter.DataPacketSize;

    [KSField] public bool IsTransmitting => dataTransmitter.IsTransmitting.GetValue();

    [KSField] public double ActiveTransmissionSize => dataTransmitter.ActiveTransmissionSize;

    [KSField] public double ActiveTransmissionCompleted => dataTransmitter.ActiveTransmissionCompleted;

    [KSField] public bool HasResourcesToOperate => dataTransmitter.HasResourcesToOperate;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter[] RequiredResources =>
        dataTransmitter.RequiredResources.Select(settings => new KSPResourceModule.ResourceSettingAdapter(settings)).ToArray();

}
