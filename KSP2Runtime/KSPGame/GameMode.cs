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
        return state switch {
            GameState.KerbalSpaceCenter => KSPGameMode.KSC,
            GameState.TrackingStation => KSPGameMode.Tracking,
            GameState.VehicleAssemblyBuilder => KSPGameMode.VAB,
            GameState.Launchpad or GameState.Runway or GameState.FlightView or GameState.Map3DView => KSPGameMode.Flight,
            _ => KSPGameMode.Unknown,
        };
    }
}
