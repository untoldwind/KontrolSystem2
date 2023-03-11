using KSP.Game;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    public enum GameMode {
        Unknown,
        KSC,
        VAB,
        Tracking,
        Flight,
    }

    public static class GameModeAdapter {
        public static GameMode GameModeFromState(GameState state) {
            switch (GameManager.Instance.Game.GlobalGameState.GetState()) {
            case GameState.KerbalSpaceCenter: return GameMode.KSC;
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
