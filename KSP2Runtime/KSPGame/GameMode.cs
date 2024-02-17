using KSP.Game;

namespace KontrolSystem.KSP.Runtime.KSPGame;

public enum KSPGameMode {
    Unknown,
    KSC,
    VAB,
    Tracking,
    Flight
}

public static class GameModeAdapter {
    public static KSPGameMode GameModeFromState(GameState state) {
        switch (state) {
        case GameState.KerbalSpaceCenter: return KSPGameMode.KSC;
        case GameState.TrackingStation: return KSPGameMode.Tracking;
        case GameState.VehicleAssemblyBuilder: return KSPGameMode.VAB;
        case GameState.Launchpad:
        case GameState.Runway:
        case GameState.FlightView:
        case GameState.Map3DView: return KSPGameMode.Flight;
        default:
            return KSPGameMode.Unknown;
        }
    }
}
