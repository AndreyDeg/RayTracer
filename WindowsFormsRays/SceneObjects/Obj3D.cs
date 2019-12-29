using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using WindowsFormsRays.Materials;
using WindowsFormsRays.Utils;
using System.Linq;

namespace WindowsFormsRays.SceneObjects
{
    public class Obj3D : IObject3D
    {
        public Vector boxMin, boxMax;
        public IMaterial Material { get; set; }

        private List<Triangles3D> Triangles = new List<Triangles3D>();

        public float GetDistance(Vector position)
        {
			float boxDist = Utils3D.BoxTest(position, boxMin, boxMax);
			if (boxDist >= 1)
				return boxDist;

            float dist = float.MaxValue;
            foreach (var triangle in Triangles)
                dist = Math.Min(dist, triangle.GetDistance(position));

            return dist;
        }

        public struct Face
        {
            public int N, tex, nrm;

            public Face(string[] prms, int n)
            {
                N = int.Parse(prms[1 + n * 3]) - 1;
                tex = int.Parse(prms[2 + n * 3]) - 1;
                nrm = int.Parse(prms[3 + n * 3]) - 1;
            }
        }

        public List<Vector> verts = new List<Vector>();
        public List<Vector> tex = new List<Vector>();
        public List<Vector> nrm = new List<Vector>();
        public List<Face[]> faces = new List<Face[]>();

        public float minX = float.MaxValue;
        public float maxX = float.MinValue;
        public float minY = float.MaxValue;
        public float maxY = float.MinValue;
        public float minZ = float.MaxValue;
        public float maxZ = float.MinValue;

        public float sizeX { get { return maxX - minX; } }
        public float sizeY { get { return maxY - minY; } }
        public float sizeZ { get { return maxZ - minZ; } }

        public int textureW, textureH;
        public Vector[,] texture;

        public int normalW, normalH;
        public Vector[,] normal;

        public int specW, specH;
        public int[,] spec;

        private Vector ToVector(string x, string y, string z)
        {
            return new Vector(
                float.Parse(x, CultureInfo.InvariantCulture),
                float.Parse(y, CultureInfo.InvariantCulture),
                float.Parse(z, CultureInfo.InvariantCulture));
        }

        public Obj3D(string filename, string textureFile = null, string normalFile = null, string specFile = null)
        {
            var file = File.OpenText(filename);
            while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                if (line == null) break;
                line = line.Replace("  ", " ");
                var prms = line.Split(' ', '/');
                switch (prms[0])
                {
                    case "v": verts.Add(ToVector(prms[1], prms[2], prms[3])*5+new Vector(0,5,0)); break;
                    case "vt": tex.Add(ToVector(prms[1], prms[2], prms[3])); break;
                    case "vn": nrm.Add(ToVector(prms[1], prms[2], prms[3])); break;
                    case "f": faces.Add(new[] { new Face(prms, 0), new Face(prms, 1), new Face(prms, 2) }); break;
                }
            }

            foreach (var vert in verts)
            {
                minX = Math.Min(minX, vert.x);
                maxX = Math.Max(maxX, vert.x);
                minY = Math.Min(minY, vert.y);
                maxY = Math.Max(maxY, vert.y);
                minZ = Math.Min(minZ, vert.z);
                maxZ = Math.Max(maxZ, vert.z);
            }

            if (textureFile != null)
            {
                var ftexture = new Bitmap(textureFile);
                ftexture.RotateFlip(RotateFlipType.RotateNoneFlipY);
                texture = MyImage.ToVectorRGB(ftexture);
                textureW = texture.GetLength(0);
                textureH = texture.GetLength(1);
            }

            if (normalFile != null)
            {
                var fnormal = new Bitmap(normalFile);
                fnormal.RotateFlip(RotateFlipType.RotateNoneFlipY);
                normal = MyImage.ToVectorRGB(fnormal);
                normalW = normal.GetLength(0);
                normalH = normal.GetLength(1);

                for (int x = 0; x < normalW; x++)
                    for (int y = 0; y < normalH; y++)
                        normal[x, y] = normal[x, y] / 127.5f - 1f;
            }

            if (specFile != null)
            {
                var fspec = new Bitmap(specFile);
                fspec.RotateFlip(RotateFlipType.RotateNoneFlipY);
                spec = MyImage.ToArrayInt(fspec);
                specW = spec.GetLength(0);
                specH = spec.GetLength(1);
            }

            CreateKTree();
        }

        private void CreateKTree()
        {
            boxMin = new Vector(minX, minY, minZ);
            boxMax = new Vector(maxX, maxY, maxZ);

            var trs = new List<Triangle3D>();
            foreach (var face in faces)
                trs.Add(new Triangle3D(verts[face[0].N], verts[face[1].N], verts[face[2].N]));

            IEnumerable<List<Triangle3D>> res = new List<List<Triangle3D>> { trs };
            res = CreateKTreeX(res);
            res = CreateKTreeY(res);
            res = CreateKTreeZ(res);
            res = CreateKTreeX(res);
            res = CreateKTreeY(res);
            res = CreateKTreeZ(res);

            Triangles.AddRange(res.Select(x => new Triangles3D(x)));
        }

        private IEnumerable<List<Triangle3D>> CreateKTreeX(IEnumerable<List<Triangle3D>> trss)
        {
            foreach (var trs in trss)
            {
                trs.Sort((a, b) => Math.Sign(a.boxMin.x - b.boxMin.x));
                yield return trs.GetRange(0, trs.Count / 2);
                yield return trs.GetRange(trs.Count / 2, trs.Count - trs.Count / 2);
            }
        }
        private IEnumerable<List<Triangle3D>> CreateKTreeY(IEnumerable<List<Triangle3D>> trss)
        {
            foreach (var trs in trss)
            {
                trs.Sort((a, b) => Math.Sign(a.boxMin.y - b.boxMin.y));
                yield return trs.GetRange(0, trs.Count / 2);
                yield return trs.GetRange(trs.Count / 2, trs.Count - trs.Count / 2);
            }
        }
        private IEnumerable<List<Triangle3D>> CreateKTreeZ(IEnumerable<List<Triangle3D>> trss)
        {
            foreach (var trs in trss)
            {
                trs.Sort((a, b) => Math.Sign(a.boxMin.z - b.boxMin.z));
                yield return trs.GetRange(0, trs.Count / 2);
                yield return trs.GetRange(trs.Count / 2, trs.Count - trs.Count / 2);
            }
        }

        public Vector GetTexture(Vector t)
        {
            return GetTexture(t.x, t.y);
        }

        public Vector GetTexture(float x, float y)
        {
            return texture[(int)(x * textureW) % textureW, (int)(y * textureH) % textureH];
        }

        public Vector GetNormal(Vector t)
        {
            return GetNormal(t.x, t.y);
        }

        public Vector GetNormal(float x, float y)
        {
            return normal[(int)(x * normalW) % normalW, (int)(y * normalH) % normalH];
        }

        public float GetSpecular(Vector t)
        {
            return GetSpecular(t.x, t.y);
        }

        public float GetSpecular(float x, float y)
        {
            return spec[(int)(x * specW) % specW, (int)(y * specH) % specH];
        }
    }
}
