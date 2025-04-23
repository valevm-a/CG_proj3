using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CG_proj3
{
    // source: https://stackoverflow.com/a/34801225
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        // custom constructor to convert an existing Bitmap into a DirectBitmap
        public DirectBitmap(Bitmap bitmap) : this(bitmap.Width, bitmap.Height)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                    this.SetPixel(x, y, bitmap.GetPixel(x, y));
            }
        }

        // custom constructor to create a DirectBitmap from another DirectBitmap
        public DirectBitmap(DirectBitmap directbitmap) : this(directbitmap.Width, directbitmap.Height)
        {
            for (int y = 0; y < directbitmap.Height; y++)
            {
                for (int x = 0; x < directbitmap.Width; x++)
                    this.SetPixel(x, y, directbitmap.GetPixel(x, y));
            }
        }

        public void Clear(Color color)
        {
            using (Graphics g = Graphics.FromImage(Bitmap))
            {
                g.Clear(color);
            }
        }

        public void SetPixel(int x, int y, Color colour)
        {
            //temp (cuz exception if out the bounds)
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Color.Empty;

            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}