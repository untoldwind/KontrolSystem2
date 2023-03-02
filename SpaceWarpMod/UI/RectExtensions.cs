using System;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public static class RectExtensions {
        public static Rect ClampToRectAngle(Rect pos, Rect target) {
            float topSeparation = Math.Abs(target.y - pos.y);
            float bottomSeparation = Math.Abs(target.yMax - pos.yMax);
            float leftSeparation = Math.Abs(target.x - pos.x);
            float rightSeparation = Math.Abs(target.xMax - pos.xMax);

            if (topSeparation <= bottomSeparation) {
                pos.y = target.y;
            } else {
                pos.y = target.yMax - pos.height;
            }

            if (leftSeparation <= rightSeparation) {
                pos.x = target.x;
            } else {
                pos.x = target.xMax - pos.width;
            }

            return pos;
        }
    }
}
