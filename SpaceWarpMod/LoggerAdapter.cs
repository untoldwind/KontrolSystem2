using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BepInEx.Logging;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {
    public class LoggerAdapter : MonoBehaviour, ITO2Logger {
        private ManualLogSource backend = new ManualLogSource("KontrolSystem");

        private static readonly ConcurrentQueue<LocAction> _executionQueue = new ConcurrentQueue<LocAction>();


        public static bool debugEnabled = true;

        internal ManualLogSource Backend {
            set {
                backend = value;
            }
        }

        public void Update() {
            while (_executionQueue.TryDequeue(out var action)) {
                backend.Log(action.logLevel, action.data);
            }
        }

        public void Debug(string message) {
            if (debugEnabled) _executionQueue.Enqueue(new LocAction {
                logLevel = LogLevel.Debug,
                data = message
            });
        }

        public void Info(string message) => _executionQueue.Enqueue(new LocAction {
            logLevel = LogLevel.Info,
            data = message
        });

        public void Warning(string message) => _executionQueue.Enqueue(new LocAction {
            logLevel = LogLevel.Warning,
            data = message
        });

        public void Error(string message) => _executionQueue.Enqueue(new LocAction {
            logLevel = LogLevel.Error,
            data = message
        });

        public void LogException(Exception exception) => _executionQueue.Enqueue(new LocAction {
            logLevel = LogLevel.Error,
            data = exception
        });

        private struct LocAction {
            public LogLevel logLevel;
            public object data;
        }
    }
}
