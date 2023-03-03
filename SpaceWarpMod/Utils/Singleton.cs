using KSP.Game;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.Utils {
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton&lt;MyClassName&gt; {}
    /// </summary>
    public class Singleton<T> : KerbalMonoBehaviour where T : KerbalMonoBehaviour {
        // Check to see if we're about to be destroyed.
        private static bool _shuttingDown;
        private static readonly object SingletonLock = new object();
        private static T _instance;

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance {
            get {
                if (_shuttingDown) {
                    LoggerAdapter.Instance.Warning("[Singleton] Instance '" + typeof(T) +
                                                   "' already destroyed. Returning null.");
                    return null;
                }

                lock (SingletonLock) {
                    if (_instance == null) {
                        // Search for existing instance.
                        _instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (_instance == null) {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T) + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }


        private void OnApplicationQuit() {
            _shuttingDown = true;
        }


        private void OnDestroy() {
            _shuttingDown = true;
        }
    }
}
