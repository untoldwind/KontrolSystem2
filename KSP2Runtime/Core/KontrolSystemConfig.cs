using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime;

public interface KontrolSystemConfig {
    public string Version { get; }

    public string StdLibPath { get; }

    public string LocalLibPath { get; }

    public int RebootTimeout { get; }
    
    public int REPLTimeout { get; }
    
    public int UnitTestTimeout { get; }
    
    public ITO2Logger Logger { get; }

    public OptionalAddons OptionalAddons { get; }
}
