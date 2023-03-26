using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.TO2.AST;
using KSP.Game;
using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class ProcessArgumentsWindow : ResizableWindow {
        abstract class ArgumentListElement {
            public EntrypointArgumentDescriptor Descriptor { get; }
            public object Value { get; protected set; }
            public abstract void Draw();

            protected ArgumentListElement(EntrypointArgumentDescriptor descriptor) {
                Descriptor = descriptor;
                Value = descriptor.DefaultValue;
            }
        }

        class NumberListElement : ArgumentListElement {
            private string stringValue;

            public NumberListElement(EntrypointArgumentDescriptor descriptor) : base(descriptor) {
                stringValue = Value.ToString();
            }

            public override void Draw() {
                stringValue = GUILayout.TextField(stringValue, GUILayout.MinWidth(100));

                if (Descriptor.Type == BuiltinType.Int) {
                    if (long.TryParse(stringValue, out var value)) {
                        Value = value;
                    } else {
                        Value = Descriptor.DefaultValue;
                    }
                } else if (Descriptor.Type == BuiltinType.Float) {
                    if (double.TryParse(stringValue, out var value)) {
                        Value = value;
                    } else {
                        Value = Descriptor.DefaultValue;
                    }
                }
            }
        }

        class BoolListElement : ArgumentListElement {
            public BoolListElement(EntrypointArgumentDescriptor descriptor) : base(descriptor) { }

            public override void Draw() {
                Value = GUILayout.Toggle((bool)Value, "", GUILayout.MinWidth(100));
            }
        }

        class StringListElement : ArgumentListElement {
            public StringListElement(EntrypointArgumentDescriptor descriptor) : base(descriptor) { }

            public override void Draw() {
                Value = GUILayout.TextField((string)Value, GUILayout.MinWidth(100));
            }
        }

        private KontrolSystemProcess process;
        private ArgumentListElement[] arguments;

        public void Attach(KontrolSystemProcess process, Rect parentPosition) {
            var gameMode = GameModeAdapter.GameModeFromState(Game.GlobalGameState.GetState());

            var argumentDescs = process.EntrypointArgumentDescriptors(gameMode);
            arguments = argumentDescs.Select<EntrypointArgumentDescriptor, ArgumentListElement>(arg => {
                if (arg.Type == BuiltinType.Int || arg.Type == BuiltinType.Float) {
                    return new NumberListElement(arg);
                } else if (arg.Type == BuiltinType.Bool) {
                    return new BoolListElement(arg);
                } else if (arg.Type == BuiltinType.String) {
                    return new StringListElement(arg);
                } else {
                    throw new Exception($"Invalid Process Argument Type {arg.Type.Name}");
                }
            }
                ).ToArray();

            this.process = process;
            windowRect = new Rect(parentPosition.xMin + 20, parentPosition.yMin + 50, 0, 0);

            Open();
            GUI.BringWindowToFront(objectId);
        }

        public void Awake() {
            Initialize($"Program Arguments", windowRect, 120, 120, false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();

            foreach (var argument in arguments) {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{argument.Descriptor.Name}", GUILayout.MinWidth(150));
                argument.Draw();

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start")) {

                object[] values = arguments.Select(arg => arg.Value).ToArray();
                Mainframe.Instance.StartProcess(process, Game.ViewController.GetActiveSimVessel(true), values);
            }
            if (GUILayout.Button("Close")) {
                Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
