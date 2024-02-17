using KontrolSystem.TO2.Generator;
using Xunit;
using Xunit.Sdk;

namespace KontrolSystem.TO2.Test.AST;

public class TestBlockContext : SyncBlockContext {
    public TestBlockContext() : base(new TestModuleContext(), new TestILEmitter()) {
    }

    public string ILCommands {
        get {
            if (IL is TestILEmitter testIl)
                return string.Join("\n", testIl.commands);
            throw new XunitException("Not using test emitter");
        }
    }

    public void AssertCommands(params string[] commands) {
        if (IL is TestILEmitter testIl) {
            int i;
            for (i = 0; i < commands.Length; i++)
                Assert.Equal(commands[i], i < testIl.commands.Count ? testIl.commands[i].ToString() : "");

            for (; i < testIl.commands.Count; i++) Assert.Equal("", testIl.commands[i].ToString());
        } else {
            throw new XunitException("Not using test emitter");
        }
    }
}
