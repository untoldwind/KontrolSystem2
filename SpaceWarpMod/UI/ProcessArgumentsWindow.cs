using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.TO2.AST;
using KSP.Game;
using System;
using System.Linq;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class ProcessArgumentsWindow : ResizableWindow {
        private KontrolSystemProcess process;
        private EntrypointArgumentDescriptor[] argumentDescs;
        private string[] argumentValues;

        public void Attach(KontrolSystemProcess process, Rect parentPosition) {
            var gameMode = GameModeAdapter.GameModeFromState(GameManager.Instance.Game.GlobalGameState.GetState());

            argumentDescs = process.EntrypointArgumentDescriptors(gameMode);
            argumentValues = argumentDescs.Select(arg => arg.DefaultValue.ToString()).ToArray();

            this.process = process;
            windowRect = new Rect(parentPosition.xMin + 20, parentPosition.yMin + 50, 0, 0);

            Open();
        }

        public void Awake() {
            Initialize($"Program Arguments", windowRect, 120, 120, false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            argumentValues = argumentDescs.Zip(argumentValues, Tuple.Create).Select((arg, i) => {
                var (desc, val) = arg;
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{desc.Name}", GUILayout.MinWidth(200));
                var newValueString = GUILayout.TextField(val, GUILayout.MinWidth(100));

                GUILayout.EndHorizontal();

                return newValueString;
            }).ToArray();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start")) {

                object[] arguments = argumentDescs.Zip(argumentValues, Tuple.Create).Select(arg => {
                    var (desc, val) = arg;

                    if (desc.Type == BuiltinType.Int) {
                        long.TryParse(val, out long result);
                        return (object) result;
                    } else if (desc.Type == BuiltinType.Float) {
                        double.TryParse(val, out double result);
                        return result;
                    } else if (desc.Type == BuiltinType.Bool) {
                        bool.TryParse(val, out bool result);
                        return result;
                    } else {
                        LoggerAdapter.Instance.Error($"Unknown Program Argument Type {desc.Type}");
                        return null;
                    }
                }).ToArray();

                Mainframe.Instance.StartProcess(process, GameManager.Instance?.Game?.ViewController?.GetActiveSimVessel(true), arguments);
            }
            if (GUILayout.Button("Close")) {
                Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
