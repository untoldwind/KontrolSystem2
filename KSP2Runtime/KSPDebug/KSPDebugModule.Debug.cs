using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
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
                VectorRenderer renderer =
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
            public VectorRenderer AddVector(Func<Position> startProvider, Func<Position> endProvider,
                KSPConsoleModule.RgbaColor color, string label, double width) {
                VectorRenderer renderer =
                    new VectorRenderer(startProvider, endProvider, color, label, width, true);

                renderer.Visible = true;
                KSPContext.CurrentContext.AddMarker(renderer);

                return renderer;
            }
            
            [KSMethod]
            public GroundMarker AddGroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates,
                KSPConsoleModule.RgbaColor color, double rotation) {
                GroundMarker groundMarker = new GroundMarker(geoCoordinates, color, rotation);

                KSPContext.CurrentContext.AddMarker(groundMarker);

                return groundMarker;
            }
            
            [KSMethod(
                Description = "Remove all markers from the game-scene."
            )]
            public void ClearMarkers() {
                KSPContext.CurrentContext.ClearMarkers();
            }
        }
    }
}
