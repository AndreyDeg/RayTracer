using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Box3D : IObject3D
    {
        public Vector Min, Max;
        public IMaterial Material { get; set; }

        public float GetDistance(Vector position, Vector direction)
        {
            return Utils3D.BoxTest(position, Min, Max);
        }
    }
}
