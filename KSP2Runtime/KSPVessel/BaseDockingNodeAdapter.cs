using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseDockingNodeAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_DockingNode dataDockingNode;

    protected BaseDockingNodeAdapter(P part, Data_DockingNode dataDockingNode) : base(part) {
        this.dataDockingNode = dataDockingNode;
    }

    [KSField] public bool IsDeployableDockingPort => dataDockingNode.IsDeployableDockingPort;

    [KSField] public Data_DockingNode.DockingState DockingState => dataDockingNode.CurrentState;

    [KSField] public string[] NodeTypes => dataDockingNode.NodeTypes;

}
