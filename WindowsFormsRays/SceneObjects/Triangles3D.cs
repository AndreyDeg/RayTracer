using System;
using System.Collections.Generic;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Triangles3D : IObject3D
    {
        public Vector boxMin, boxMax;
        public List<Triangle3D> triangles;

        public Triangles3D(List<Triangle3D> triangles)
        {
            this.triangles = triangles;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;
            foreach (var triangle in triangles)
            {
                minX = Math.Min(minX, triangle.boxMin.x);
                maxX = Math.Max(maxX, triangle.boxMax.x);
                minY = Math.Min(minY, triangle.boxMin.y);
                maxY = Math.Max(maxY, triangle.boxMax.y);
                minZ = Math.Min(minZ, triangle.boxMin.z);
                maxZ = Math.Max(maxZ, triangle.boxMax.z);
            }

            boxMin = new Vector(minX, minY, minZ);
            boxMax = new Vector(maxX, maxY, maxZ);
        }

        public IMaterial Material { get; set; }

        public float GetDistance(Vector position)
        {
            float boxDist = Utils3D.BoxTest(position, boxMin, boxMax);
            if (boxDist >= 1)
                return boxDist;

            float dist = float.MaxValue;
            foreach (var triangle in triangles)
                dist = Math.Min(dist, triangle.GetDistance(position));

            return dist;
        }
    }
}
