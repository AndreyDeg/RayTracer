using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Triangle3D : IObject3D
    {
        public Vector a;
        public Vector b;
        public Vector c;
        public Vector boxMin, boxMax;

        public Triangle3D(Vector a, Vector b, Vector c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            boxMin = new Vector(Math.Min(Math.Min(a.x, b.x), c.x), Math.Min(Math.Min(a.y, b.y), c.y), Math.Min(Math.Min(a.z, b.z), c.z));
            boxMax = new Vector(Math.Max(Math.Max(a.x, b.x), c.x), Math.Max(Math.Max(a.y, b.y), c.y), Math.Max(Math.Max(a.z, b.z), c.z));
        }

        public IMaterial Material { get; set; }

        public float GetDistance(Vector position)
        {
            return Utils3D.udTriangle(position, a, b, c);
        }
    }
}
