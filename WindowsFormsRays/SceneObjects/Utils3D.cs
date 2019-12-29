using System;

namespace WindowsFormsRays.SceneObjects
{
    public static class Utils3D
    {
        public static float Pow8(float d)
        {
            var d2 = d * d;
            var d4 = d2 * d2;
            return d4 * d4;
        }

        public static float min(float l, float r) { return l < r ? l : r; }

        // Rectangle CSG equation. Returns minimum signed distance from 
        // space carved by lowerLeft vertex and opposite rectangle
        // vertex upperRight.
        public static float BoxTest(Vector position, Vector lowerLeft, Vector upperRight)
        {
            lowerLeft = position - lowerLeft;
            upperRight = upperRight - position;
            return -min(
              min(
                min(lowerLeft.x, upperRight.x),
                min(lowerLeft.y, upperRight.y)
              ),
              min(lowerLeft.z, upperRight.z));
        }

        public static float dot(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        public static Vector cross(Vector v1, Vector v2)
        {
            return new Vector(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }
        public static float clamp(float v, float min, float max)
        {
            return v < min ? min : v > max ? max : v;
        }
        public static Vector clamp(Vector v, float min, float max)
        {
            return new Vector(clamp(v.x, min, max), clamp(v.y, min, max), clamp(v.z, min, max));
        }
        public static float dot2(Vector v) { return dot(v, v); }
        public static float udTriangle(Vector p, Vector a, Vector b, Vector c)
        {
            Vector ba = b - a, pa = p - a;
            Vector cb = c - b, pb = p - b;
            Vector ac = a - c, pc = p - c;
            var nor = cross(ba, ac);

            return (float)Math.Sqrt(
              (Math.Sign(dot(cross(ba, nor), pa)) +
               Math.Sign(dot(cross(cb, nor), pb)) +
               Math.Sign(dot(cross(ac, nor), pc)) < 2.0)
               ?
               Math.Min(Math.Min(
               dot2(ba * clamp(dot(ba, pa) / dot2(ba), 0.0f, 1.0f) - pa),
               dot2(cb * clamp(dot(cb, pb) / dot2(cb), 0.0f, 1.0f) - pb)),
               dot2(ac * clamp(dot(ac, pc) / dot2(ac), 0.0f, 1.0f) - pc))
               :
               dot(nor, pa) * dot(nor, pa) / dot2(nor));
        }
    }
}
