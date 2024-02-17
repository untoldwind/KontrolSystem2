using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public partial class KSPDebugModule {
    [KSClass("Debug", Description = "Collection of debug helper")]
    public class Debug {
        [KSMethod(Description =
            @"Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
                  The line may have a `label` at its mid-point.
                 "
        )]
        public VectorRenderer AddLine(Func<Position> startProvider, Func<Position> endProvider,
            KSPConsoleModule.RgbaColor color,
            string label, double width) {
            var renderer =
                new VectorRenderer(startProvider, endProvider, color, label, width, false);

            renderer.Visible = true;
            KSPContext.CurrentContext.AddMarker(renderer);

            return renderer;
        }

        [KSMethod(Description =
            @"Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
                  The vector may have a `label` at its mid-point.
                 "
        )]
        public VectorRenderer AddVector(Func<Position> startProvider, Func<Vector> vectorProvider,
            KSPConsoleModule.RgbaColor color, string label, double width) {
            var renderer =
                new VectorRenderer(startProvider, vectorProvider, color, label, width, true);

            renderer.Visible = true;
            KSPContext.CurrentContext.AddMarker(renderer);

            return renderer;
        }

        [KSMethod]
        public GroundMarker AddGroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates,
            KSPConsoleModule.RgbaColor color, double rotation) {
            var groundMarker = new GroundMarker(geoCoordinates, color, rotation);

            KSPContext.CurrentContext.AddMarker(groundMarker);

            return groundMarker;
        }

        [KSMethod]
        public BillboardRenderer AddBillboard(Func<Position> positionProvider, Func<string> textProvider,
            KSPConsoleModule.RgbaColor color, long fontSize) {
            var marker = new BillboardRenderer(positionProvider, textProvider, color, fontSize);
            marker.Visible = true;

            KSPContext.CurrentContext.AddMarker(marker);

            return marker;
        }

        [KSMethod]
        public PathRenderer AddPath(Position[] path, KSPConsoleModule.RgbaColor color, double width) {
            var pathMarker = new PathRenderer(path, color, width);
            pathMarker.Visible = true;

            KSPContext.CurrentContext.AddMarker(pathMarker);

            return pathMarker;
        }

        [KSMethod(
            Description = "Remove all markers from the game-scene."
        )]
        public void ClearMarkers() {
            KSPContext.CurrentContext.ClearMarkers();
        }
    }
}
