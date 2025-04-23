using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CG_proj3
{
    public abstract class Shape
    {
        public Color Color { get; set; }
        public int Thickness { get; set; }

        public abstract void Draw(DirectBitmap bmp, bool antiAliasing);
        public abstract bool HitTest(Point p);
        public abstract void Move(Point delta);
    }

    public class LineShape : Shape
    {
        public Point Start, End;

        public override void Draw(DirectBitmap bmp, bool antiAliasing)
        {
            if (!antiAliasing)
                DrawDDA(bmp);
            else
                DrawWuLine(bmp, Start.X, Start.Y, End.X, End.Y, Color);
        }

        private void DrawDDA(DirectBitmap bmp)
        {
            int dx = End.X - Start.X;
            int dy = End.Y - Start.Y;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xInc = dx / (float)steps;
            float yInc = dy / (float)steps;

            float x = Start.X;
            float y = Start.Y;

            for (int i = 0; i <= steps; i++)
            {
                if (Thickness > 1)
                {
                    var brush = BrushManager.CreateCircularBrush(Thickness / 2);
                    BrushManager.StampBrush(bmp, (int)Math.Round(x), (int)Math.Round(y), brush, Color);
                }

                else
                    bmp.SetPixel((int)Math.Round(x), (int)Math.Round(y), Color);

                x += xInc;
                y += yInc;
            }
        }

        private void DrawWuLine(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color lineColor)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = dx == 0 ? 0 : dy / dx;

            float y = y0;

            for (int x = x0; x <= x1; x++)
            {
                int yInt = (int)Math.Floor(y);
                float yFrac = y - yInt;

                Color c1 = XiaolinWuHelper.Blend(lineColor, steep ? bmp.GetPixel(yInt, x) : bmp.GetPixel(x, yInt), 1 - yFrac);
                Color c2 = XiaolinWuHelper.Blend(lineColor, steep ? bmp.GetPixel(yInt + 1, x) : bmp.GetPixel(x, yInt + 1), yFrac);

                if (steep)
                {
                    bmp.SetPixel(yInt, x, c1);
                    bmp.SetPixel(yInt + 1, x, c2);
                }
                else
                {
                    bmp.SetPixel(x, yInt, c1);
                    bmp.SetPixel(x, yInt + 1, c2);
                }

                y += gradient;
            }
        }


        public override bool HitTest(Point p)
        {
            float A = p.X - Start.X;
            float B = p.Y - Start.Y;
            float C = End.X - Start.X;
            float D = End.Y - Start.Y;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = (len_sq != 0) ? dot / len_sq : -1;

            float xx, yy;

            if (param < 0)
            {
                xx = Start.X;
                yy = Start.Y;
            }
            else if (param > 1)
            {
                xx = End.X;
                yy = End.Y;
            }
            else
            {
                xx = Start.X + param * C;
                yy = Start.Y + param * D;
            }

            float dx = p.X - xx;
            float dy = p.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy) <= Thickness + 3; // tolerance
        }

        public override void Move(Point delta)
        {
            Start = new Point(Start.X + delta.X, Start.Y + delta.Y);
            End = new Point(End.X + delta.X, End.Y + delta.Y);
        }
    }

    public class CircleShape : Shape
    {
        public Point Center;
        public int Radius;

        public override void Draw(DirectBitmap bmp, bool antiAliasing)
        {
            if (!antiAliasing)
                DrawMidpointCircle(bmp);

            else
                DrawWuCircle(bmp);
        }

        private void DrawMidpointCircle(DirectBitmap bmp)
        {
            int x = 0;
            int y = Radius;
            int d = 1 - Radius;

            DrawSymmetricPoints(bmp, x, y, Color);

            while (y > x)
            {
                if (d < 0)
                    d += 2 * x + 3;

                else
                {
                    d += 2 * (x - y) + 5;
                    y--;
                }
                x++;
                DrawSymmetricPoints(bmp, x, y, Color);
            }
        }

        private void DrawSymmetricPoints(DirectBitmap bmp, int x, int y, Color color)
        {
            var points = new (int dx, int dy)[]
            {
            ( x,  y), ( y,  x),
            (-x,  y), (-y,  x),
            (-x, -y), (-y, -x),
            ( x, -y), ( y, -x),
            };

            foreach (var (dx, dy) in points)
            {
                int px = Center.X + dx;
                int py = Center.Y + dy;

                if (Thickness > 1)
                {
                    var brush = BrushManager.CreateCircularBrush(Thickness / 2);
                    BrushManager.StampBrush(bmp, px, py, brush, color);
                }
                else
                {
                    bmp.SetPixel(px, py, Color);
                }
            }
        }

        // https://landkey.net/d/L/J/RF/WUCircle/Intro.txt.legacy.htm
        private void DrawWuCircle(DirectBitmap bmp)
        {
            int x = Radius;
            int y = 0;

            DrawSymmetricPoints(bmp, x, y, Color);

            float d = 0f;

            while (x > y)
            {
                y++;
                double idealX = Math.Sqrt(Radius * Radius - y * y);
                float dc = (float)(Math.Ceiling(idealX) - idealX);

                if (dc < d)
                    x--;

                Color c1 = XiaolinWuHelper.Blend(Color, bmp.GetPixel(Center.X + x, Center.Y + y), 1 - dc);
                Color c2 = XiaolinWuHelper.Blend(Color, bmp.GetPixel(Center.X + x - 1, Center.Y + y), dc);

                DrawSymmetricPoints(bmp, x, y, c1);
                DrawSymmetricPoints(bmp, x - 1, y, c2);

                d = dc;
            }
        }

        public override bool HitTest(Point p)
        {
            int dx = p.X - Center.X;
            int dy = p.Y - Center.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return Math.Abs(distance - Radius) <= Thickness + 3;
        }

        public override void Move(Point delta)
        {
            Center = new Point(Center.X + delta.X, Center.Y + delta.Y);
        }
    }

    public class PolygonShape : Shape
    {
        public List<LineShape> LineSegments = new List<LineShape>();
        public List<Point> Vertices = new List<Point>();

        public override void Draw(DirectBitmap bmp, bool antiAliasing)
        {
            foreach (var line in LineSegments)
            {
                line.Draw(bmp, antiAliasing);
            }
        }

        public void AddLineSegment(Point start, Point end)
        {
            var line = new LineShape
            {
                Start = start,
                End = end,
                Color = Color,
                Thickness = Thickness
            };
            LineSegments.Add(line);
        }

        public override bool HitTest(Point p)
        {
            foreach (var line in LineSegments)
            {
                if (line.HitTest(p))
                    return true;
            }
            return false;
        }

        public int HitTestVertex(Point location, int tolerance = 5)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Math.Abs(location.X - Vertices[i].X) < tolerance && Math.Abs(location.Y - Vertices[i].Y) < tolerance)
                    return i;
            }
            return -1;
        }


        public Point GetCentroid()
        {
            int xSum = 0, ySum = 0;
            foreach (var pt in Vertices)
            {
                xSum += pt.X;
                ySum += pt.Y;
            }
            return new Point(xSum / Vertices.Count, ySum / Vertices.Count);
        }

        public void Translate(int dx, int dy)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Point oldPoint = Vertices[i];
                Point newPoint = new Point(oldPoint.X + dx, oldPoint.Y + dy);
                Vertices[i] = newPoint;

                foreach (var line in LineSegments)
                {
                    if (line.Start == oldPoint)
                        line.Start = newPoint;
                    if (line.End == oldPoint)
                        line.End = newPoint;
                }
            }
        }


        public void ChangeThickness()
        {
            foreach (var line in LineSegments)
                line.Thickness = Thickness;
        }

        public void ChangeColor()
        {
            foreach (var line in LineSegments)
                line.Color = Color;
        }

        public override void Move(Point delta)
        {
            // 
        }
    }

    public static class XiaolinWuHelper
    {
        public static Color Blend(Color lineColor, Color backgroundColor, float alpha)
        {
            int Clamp(int value) => Math.Min(255, Math.Max(0, value));

            int r = Clamp((int)(lineColor.R * alpha + backgroundColor.R * (1 - alpha)));
            int g = Clamp((int)(lineColor.G * alpha + backgroundColor.G * (1 - alpha)));
            int b = Clamp((int)(lineColor.B * alpha + backgroundColor.B * (1 - alpha)));

            return Color.FromArgb(r, g, b);
        }
    }

    public static class BrushManager
    {
        public static bool[,] CreateCircularBrush(int radius)
        {
            int size = 2 * radius + 1;
            bool[,] brush = new bool[size, size];
            int cx = radius, cy = radius;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        brush[x, y] = true;
                    }
                }
            }

            return brush;
        }

        public static void StampBrush(DirectBitmap bmp, int centerX, int centerY, bool[,] brush, Color color)
        {
            int size = brush.GetLength(0);
            int radius = size / 2;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (brush[x, y])
                    {
                        int px = centerX + (x - radius);
                        int py = centerY + (y - radius);

                        if (px >= 0 && px < bmp.Width && py >= 0 && py < bmp.Height)
                        {
                            bmp.SetPixel(px, py, color);
                        }
                    }
                }
            }
        }
    }
}
