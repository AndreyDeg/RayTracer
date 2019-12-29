using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace WindowsFormsRays.Utils
{
    public class MyImage
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private readonly Bitmap bitmap;
        private readonly BitmapData oneBits;
        private readonly IntPtr ptr0;
        private readonly int stride;

        public int[,] zbuffer;

        public struct PixelRGB
        {
            public byte B, G, R;
        }

        public MyImage(int w, int h)
        {
            Width = w;
            Height = h;

            bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            zbuffer = new int[Width, Height];

            oneBits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            ptr0 = oneBits.Scan0;
            stride = oneBits.Stride;

        }

        public Vector this[int x, int y]
        {
            get
            {
                unsafe
                {
                    PixelRGB* pxOne = (PixelRGB*)(ptr0 + y * stride + x * sizeof(PixelRGB));
                    return new Vector(pxOne->R, pxOne->G, pxOne->B);
                }
            }
            set
            {
                unsafe
                {
                    PixelRGB* pxOne = (PixelRGB*)(ptr0 + y * stride + x * sizeof(PixelRGB));
                    pxOne->R = (byte)Math.Min(Math.Max(0, value.x), 255);
                    pxOne->G = (byte)Math.Min(Math.Max(0, value.y), 255);
                    pxOne->B = (byte)Math.Min(Math.Max(0, value.z), 255);
                }
            }
        }

        public Bitmap ToBitmap()
        {
            bitmap.UnlockBits(oneBits);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        static internal Vector[,] ToVectorRGB(Bitmap bitmap)
        {
            var Width = bitmap.Width;
            var Height = bitmap.Height;
            var result = new Vector[Width, Height];

            unsafe
            {
                var oneBits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                byte* ptr0 = (byte*)oneBits.Scan0;
                int stride = oneBits.Stride;

                Parallel.For((long)0, Height, y =>
                {
                    byte* ptr = ptr0 + y * stride;
                    for (int x = 0; x < Width; x++, ptr += sizeof(PixelRGB))
                    {
                        PixelRGB* pxOne = (PixelRGB*)ptr;
                        result[x, y] = new Vector(pxOne->R, pxOne->G, pxOne->B);
                    }
                });

                bitmap.UnlockBits(oneBits);
            }

            return result;
        }

        static internal int[,] ToArrayInt(Bitmap bitmap)
        {
            var result = new int[bitmap.Width, bitmap.Height];

            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var c = bitmap.GetPixel(x, y);
                    result[x, y] = c.R;
                }

            return result;
        }

        public static MyImage operator *(MyImage a, MyImage b)
        {
            var r = new MyImage(a.Width, a.Height);

            for (int y = 0; y < a.Height; y++)
                for (int x = 0; x < a.Width; x++)
                    r[x, y] = a[x, y] * (new Vector(255, 255, 255) - b[x * b.Width / a.Width, y * b.Height / a.Height]) / 255;

            return r;
        }
    }
}
