using System;
using KontrolSystem.TO2.Generator;
using Xunit;

namespace KontrolSystem.TO2.Test.AST {
    public class TestBlockContext : SyncBlockContext {
        public TestBlockContext() : base(new TestModuleContext(), new TestILEmitter()) {
        }

        public void AssertCommands(params string[] commands) {
            if (IL is TestILEmitter testIl) {
                int i;
                for (i = 0; i < commands.Length; i++) {
                    Assert.Equal(commands[i], i < testIl.commands.Count ? testIl.commands[i].ToString() : "");
                }

                for (; i < testIl.commands.Count; i++) {
                    Assert.Equal("", testIl.commands[i].ToString());
                }
            } else {
                throw new Xunit.Sdk.XunitException("Not using test emitter");
            }
        }

        public string ILCommands {
            get {
                if (IL is TestILEmitter testIl) {
                    return String.Join("\n", testIl.commands);
                } else {
                    throw new Xunit.Sdk.XunitException("Not using test emitter");
                }
            }
        }
    }
}
