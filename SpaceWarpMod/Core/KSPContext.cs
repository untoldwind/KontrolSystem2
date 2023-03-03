using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.Core {
    internal class AutopilotHooks {
        private readonly IKSPContext context;
        private readonly List<FlightInputCallback> autopilots = new List<FlightInputCallback>();

        internal AutopilotHooks(IKSPContext context) => this.context = context;

        internal void Add(FlightInputCallback autopilot) {
            if (!autopilots.Contains(autopilot)) autopilots.Add(autopilot);
        }

        internal void Remove(FlightInputCallback autopilot) => autopilots.Remove(autopilot);

        internal bool IsEmpty => autopilots.Count == 0;

        internal void RunAutopilots(FlightCtrlState state, float deltaTime) {
            try {
                ContextHolder.CurrentContext.Value = context;
                foreach (FlightInputCallback autopilot in autopilots)
                    autopilot(ref state, deltaTime);
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }
    }
    public class KSPContext : IKSPContext {
        private readonly KSPConsoleBuffer consoleBuffer;
        private object nextYield;
        private readonly Stopwatch timeStopwatch;
        private readonly long timeoutMillis;
        internal readonly List<IMarker> markers;
        private readonly List<WeakReference<IFixedUpdateObserver>> fixedUpdateObservers;
        private readonly Dictionary<VesselComponent, AutopilotHooks> autopilotHooks;
        private readonly List<BackgroundKSPContext> childContexts;
        
        public KSPContext(KSPConsoleBuffer consoleBuffer) {
            this.consoleBuffer = consoleBuffer;
            markers = new List<IMarker>();
            fixedUpdateObservers = new List<WeakReference<IFixedUpdateObserver>>();
            autopilotHooks = new Dictionary<VesselComponent, AutopilotHooks>();
            nextYield = new WaitForFixedUpdate();
            childContexts = new List<BackgroundKSPContext>();
            timeStopwatch = Stopwatch.StartNew();
            timeoutMillis = 100;
        }

        public bool IsBackground => false;
        public ITO2Logger Logger => LoggerAdapter.Instance;
        
        public void CheckTimeout() {
            long elapsed = timeStopwatch.ElapsedMilliseconds;
            if (elapsed >= timeoutMillis)
                throw new YieldTimeoutException(elapsed);
        }

        public void ResetTimeout() {
            timeStopwatch.Reset();
            timeStopwatch.Start();
        }
        
        public IContext CloneBackground(CancellationTokenSource token) {
            var childContext = new BackgroundKSPContext(consoleBuffer, token);

            childContexts.Add(childContext);

            return childContext;
        }

        public GameMode GameMode => CurrentGameMode;

        public double UniversalTime => GameManager.Instance.Game.SpaceSimulation.UniverseModel.UniversalTime;
        
        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public KSPOrbitModule.IBody FindBody(string name) {
            var body = GameManager.Instance.Game.CelestialBodies.Get(name);

            return body != null ? new BodyWrapper(body) : null;
        }
        
        public object NextYield {
            get {
                object result = nextYield;
                nextYield = new WaitForFixedUpdate();
                return result;
            }
            set => nextYield = value;
        }
        
        public void AddMarker(IMarker marker) => markers.Add(marker);

        public void RemoveMarker(IMarker marker) {
            marker.Visible = false;
            markers.Remove(marker);
        }

        public void ClearMarkers() {
            foreach (IMarker marker in markers) marker.Visible = false;
            markers.Clear();
        }


        public void AddFixedUpdateObserver(IFixedUpdateObserver observer) {
            fixedUpdateObservers.Add(new WeakReference<IFixedUpdateObserver>(observer));
        }
        
        internal void TriggerFixedUpdateObservers() {
            try {
                ContextHolder.CurrentContext.Value = this;
                double deltaTime = Time.fixedDeltaTime;
                for (int i = fixedUpdateObservers.Count - 1; i >= 0; i--) {
                    IFixedUpdateObserver observer;
                    if (fixedUpdateObservers[i].TryGetTarget(out observer))
                        observer.OnFixedUpdate(deltaTime);
                    else
                        fixedUpdateObservers.RemoveAt(i);
                }
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }
        public void HookAutopilot(VesselComponent vessel, FlightInputCallback autopilot) {
            throw new System.NotImplementedException();
        }

        public void UnhookAutopilot(VesselComponent vessel, FlightInputCallback autopilot) {
            throw new System.NotImplementedException();
        }

        public void UnhookAllAutopilots(VesselComponent vessel) {
            throw new System.NotImplementedException();
        }
        
        public void Cleanup() {
            ClearMarkers();
            
            foreach (var childContext in childContexts) {
                childContext.Cleanup();
            }
            
            autopilotHooks.Clear();
            childContexts.Clear();
        }

        internal static GameMode CurrentGameMode {
            get {
                    switch (GameManager.Instance.Game.GlobalGameState.GetState()) {
                    case GameState.KerbalSpaceCenter:  return GameMode.KSC;
                    case GameState.TrackingStation: return GameMode.Tracking;
                    case GameState.VehicleAssemblyBuilder: return GameMode.VAB;
                    case GameState.Launchpad:
                    case GameState.Runway:
                    case GameState.FlightView:
                    case GameState.Map3DView: return GameMode.Flight;
                    default:
                        return GameMode.Unknown;
                    } 
            }
        }
    }
    
    public class BackgroundKSPContext : IContext {
        private readonly KSPConsoleBuffer consoleBuffer;
        private readonly CancellationTokenSource token;
        private readonly List<BackgroundKSPContext> childContexts;

        public BackgroundKSPContext(KSPConsoleBuffer consoleBuffer, CancellationTokenSource token) {
            this.consoleBuffer = consoleBuffer;
            this.token = token;
            childContexts = new List<BackgroundKSPContext>();
        }

        public ITO2Logger Logger => LoggerAdapter.Instance;

        public bool IsBackground => true;

        public void CheckTimeout() => token.Token.ThrowIfCancellationRequested();

        public void ResetTimeout() {
        }

        public IContext CloneBackground(CancellationTokenSource token) {
            var childContext = new BackgroundKSPContext(consoleBuffer, token);

            childContexts.Add(childContext);

            return childContext;
        }

        public void Cleanup() {
            if (token.Token.CanBeCanceled) {
                token.Cancel();
            }

            foreach (var childContext in childContexts) {
                childContext.Cleanup();
            }
        }
    }
}
