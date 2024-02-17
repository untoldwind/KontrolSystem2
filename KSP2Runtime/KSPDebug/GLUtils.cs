using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public static class GLUtils {
    private static Material? _colored;

    public static Material Colored {
        get {
            if (_colored == null) _colored = new Material(Shader.Find("Hidden/Internal-Colored"));

            return _colored;
        }
    }

    public static void GLTriangle(Camera camera, Vector3d worldVertices1, Vector3d worldVertices2,
        Vector3d worldVertices3,
        Color c, Material material) {
        GL.PushMatrix();
        material.SetPass(0);
        GL.LoadPixelMatrix();
        GL.Begin(GL.TRIANGLES);
        GL.Color(c);
        GLVertex(camera, worldVertices1);
        GLVertex(camera, worldVertices2);
        GLVertex(camera, worldVertices3);
        GL.End();
        GL.PopMatrix();
    }

    public static void GLVertex(Camera camera, Vector3d worldPosition) {
        var screenPoint = camera.WorldToScreenPoint(worldPosition);
        GL.Vertex3(screenPoint.x, screenPoint.y, 0);
    }

    public static void GLPixelLine(Camera camera, Vector3d worldPosition1, Vector3d worldPosition2) {
        var screenPoint1 = camera.WorldToScreenPoint(worldPosition1);
        var screenPoint2 = camera.WorldToScreenPoint(worldPosition2);

        if (screenPoint1.z > 0 && screenPoint2.z > 0) {
            GL.Vertex3(screenPoint1.x, screenPoint1.y, 0);
            GL.Vertex3(screenPoint2.x, screenPoint2.y, 0);
        }
    }

    //If dashed = false, draws 0-1-2-3-4-5...
    //If dashed = true, draws 0-1 2-3 4-5...
    public static void DrawPath(Camera camera, Vector3d[] points, Color c,
        Material material, bool dashed) {
        GL.PushMatrix();
        material.SetPass(0);
        GL.LoadPixelMatrix();
        GL.Begin(GL.LINES);
        GL.Color(c);

        Vector3d camPos = camera.transform.position;

        var step = dashed ? 2 : 1;
        for (var i = 0; i < points.Length - 1; i += step) GLPixelLine(camera, points[i], points[i + 1]);

        GL.End();
        GL.PopMatrix();
    }
}
