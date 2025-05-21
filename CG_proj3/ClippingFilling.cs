using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_proj3
{
    public enum FillType
    {
        None,
        SolidColor,
        Image
    }

    public class LiangBarskyClipping
    {
        private bool Clip(float denom, float numer, ref float tE, ref float tL)
        {
            if (denom == 0)
            {
                if (numer > 0)
                    return false; // outside - reject line
                return true;
            }

            float t = numer / denom;
            if (denom > 0)
            {
                if (t > tL) 
                    return false; // no intersection (enter after leave)
                if (t > tE) tE = t;
            }
            else
            {
                if (t < tE)
                    return false; // no intersection (leave before enter)
                if (t < tL) tL = t;
            }
            return true;
        }

        private bool LiangBarsky(LineShape line, RectangleShape clipRect, out LineShape clippedLine)
        {
            clippedLine = new LineShape
            {
                Start = line.Start,
                End = line.End,
                Color = line.Color,
                Thickness = line.Thickness
            };

            float x1 = line.Start.X;
            float y1 = line.Start.Y;
            float x2 = line.End.X;
            float y2 = line.End.Y;

            float dx = x2 - x1;
            float dy = y2 - y1;

            float tE = 0f;
            float tL = 1f;

            float xmin = Math.Min(clipRect.TopLeft.X, clipRect.BottomRight.X);
            float xmax = Math.Max(clipRect.TopLeft.X, clipRect.BottomRight.X);
            float ymin = Math.Min(clipRect.TopLeft.Y, clipRect.BottomRight.Y);
            float ymax = Math.Max(clipRect.TopLeft.Y, clipRect.BottomRight.Y);

            if (!Clip(dx, xmin - x1, ref tE, ref tL)) return false;   // left
            if (!Clip(-dx, x1 - xmax, ref tE, ref tL)) return false;  // right
            if (!Clip(dy, ymin - y1, ref tE, ref tL)) return false;   // top
            if (!Clip(-dy, y1 - ymax, ref tE, ref tL)) return false;  // bottom

            if (tL < 1)
            {
                clippedLine.End = new Point(
                    (int)(x1 + dx * tL),
                    (int)(y1 + dy * tL));
            }

            if (tE > 0)
            {
                clippedLine.Start = new Point(
                    (int)(x1 + dx * tE),
                    (int)(y1 + dy * tE));
            }

            return true;
        }

        public List<LineShape> ClipPolygonEdges(List<LineShape> edges, RectangleShape clipRect)
        {
            var clippedEdges = new List<LineShape>();

            foreach (var edge in edges)
            {
                if (LiangBarsky(edge, clipRect, out var clippedLine))
                    clippedEdges.Add(clippedLine);
            }

            return clippedEdges;
        }

    }

    public static class EdgeTableFilling
    {
        public static void FillPolygon(DirectBitmap bmp, PolygonShape poly)
        {
            if (poly.Vertices.Count < 3) return;

            Dictionary<int, List<EdgeBucket>> ET = EdgeTableFilling.BuildEdgeTable(poly.Vertices);

            List<EdgeBucket> AET = new();
            int y = ET.Keys.Min();

            while (ET.Count > 0 || AET.Count > 0)
            {
                if (ET.ContainsKey(y))
                {
                    AET.AddRange(ET[y]);
                    ET.Remove(y);
                }

                AET.RemoveAll(e => e.YMax == y);

                AET.Sort((a, b) => a.X.CompareTo(b.X));

                for (int i = 0; i < AET.Count; i += 2)
                {
                    int xStart = (int)Math.Round(AET[i].X);
                    int xEnd = (int)Math.Round(AET[i + 1].X);

                    for (int x = xStart; x < xEnd; x++)
                    {
                        if (poly.FillMode == FillType.SolidColor)
                        {
                            bmp.SetPixel(x, y, poly.FillColor);
                        }
                        else if (poly.FillMode == FillType.Image && poly.FillImage != null)
                        {
                            Color imgColor = poly.FillImage.GetPixel(
                                x % poly.FillImage.Width,
                                y % poly.FillImage.Height
                            );
                            bmp.SetPixel(x, y, imgColor);
                        }
                    }
                }

                y++;
                
                foreach (var edge in AET)
                    edge.X += edge.InvSlope;
            }
        }

        private static Dictionary<int, List<EdgeBucket>> BuildEdgeTable(List<Point> vertices)
        {
            var ET = new Dictionary<int, List<EdgeBucket>>();
            int n = vertices.Count;

            for (int i = 0; i < n; i++)
            {
                Point p1 = vertices[i];
                Point p2 = vertices[(i + 1) % n];

                if (p1.Y == p2.Y) continue;

                Point lower = p1.Y < p2.Y ? p1 : p2;
                Point upper = p1.Y < p2.Y ? p2 : p1;

                double invSlope = (double)(upper.X - lower.X) / (upper.Y - lower.Y);

                var edge = new EdgeBucket
                {
                    YMax = upper.Y,
                    X = lower.X,
                    InvSlope = invSlope
                };

                if (!ET.ContainsKey(lower.Y))
                    ET[lower.Y] = new List<EdgeBucket>();

                ET[lower.Y].Add(edge);
            }

            return ET;
        }

        private class EdgeBucket
        {
            public int YMax;
            public double X;
            public double InvSlope;
        }

    }
}
