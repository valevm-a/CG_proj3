using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_proj3
{
    public class Scene
    {
        [JsonIgnore]
        public DirectBitmap Bitmap { get; private set; }

        public List<LineShape> Lines { get; set; } = new();
        public List<CircleShape> Circles { get; set; } = new();
        public List<PolygonShape> Polygons { get; set; } = new();
        public List<RectangleShape> Rectangles { get; set; } = new();

        [JsonIgnore]
        public List<LineShape> ClippedEdges { get; set; } = new();

        public Scene(int width, int height)
        {
            Bitmap = new DirectBitmap(width, height);
        }

        // for deserialization
        public Scene()
        {
            Lines = new List<LineShape>();
            Circles = new List<CircleShape>();
            Polygons = new List<PolygonShape>();
            Rectangles = new List<RectangleShape>();
        }

        public void DrawAll(DirectBitmap bmp, bool antiAliasing)
        {
            foreach (var line in Lines)
                line.Draw(bmp, antiAliasing);

            foreach (var circle in Circles)
                circle.Draw(bmp, antiAliasing);

            foreach (var polygon in Polygons)
                polygon.Draw(bmp, antiAliasing);

            foreach (var rectangle in Rectangles)
                rectangle.Draw(bmp, antiAliasing);

            foreach (var edge in ClippedEdges)
                edge.Draw(bmp, antiAliasing);
        }

        public Shape HitTest(Point p)
        {
            foreach (var line in Lines)
                if (line.HitTest(p))
                    return line;

            foreach (var circle in Circles)
                if (circle.HitTest(p))
                    return circle;

            foreach (var poly in Polygons)
                if (poly.HitTest(p))
                    return poly;

            foreach (var rect in Rectangles)
                if (rect.HitTest(p))
                    return rect;

            return null;
        }

        public void DeleteShapeAt(Point p)
        {
            Lines.RemoveAll(l => l.HitTest(p));
            Circles.RemoveAll(c => c.HitTest(p));
            Polygons.RemoveAll(pg => pg.HitTest(p));
            Rectangles.RemoveAll(rg  => rg.HitTest(p));
            ClippedEdges.RemoveAll(ce => ce.HitTest(p));    
        }

        public void Clear()
        {
            Lines.Clear();
            Circles.Clear();
            Polygons.Clear();
            Rectangles.Clear();
            ClippedEdges.Clear();
        }

        public void AddShape(Shape shape)
        {
            if (shape is LineShape line)
                Lines.Add(line);

            else if (shape is CircleShape circle)
                Circles.Add(circle);

            else if (shape is PolygonShape poly)
                Polygons.Add(poly);

            else if (shape is RectangleShape rect)
                Rectangles.Add(rect);
        }

        public void ClipPolygonEdges(PolygonShape poly, RectangleShape rect)
        {
            var clipper = new LiangBarskyClipping();
            var clippedEdges = clipper.ClipPolygonEdges(poly.LineSegments, rect);
            foreach (var edge in clippedEdges)
            {
                edge.Color = Color.Aqua;
                edge.Thickness = poly.Thickness + 1;
            }

            ClippedEdges = clippedEdges;
        }

        public void SaveScene(string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                Converters = { new ColorJsonConverter() }
            };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(path, json);
        }

        public static Scene LoadScene(string path, int width, int height)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new ColorJsonConverter() }
            };

            var json = File.ReadAllText(path);
            var temp = JsonSerializer.Deserialize<Scene>(json, options);

            if (temp == null)
                throw new Exception("Failed to deserialize Scene");

            var scene = new Scene(width, height);

            scene.Lines.AddRange(temp.Lines);
            scene.Circles.AddRange(temp.Circles);
            scene.Polygons.AddRange(temp.Polygons);
            scene.Rectangles.AddRange(temp.Rectangles);

            foreach (var poly in scene.Polygons)
            {
                if (poly.FillMode == FillType.Image && !string.IsNullOrEmpty(poly.FillImagePath) && File.Exists(poly.FillImagePath))
                {
                    using (Bitmap original = new Bitmap(poly.FillImagePath))
                    {
                        DirectBitmap bpm = new DirectBitmap(original);
                        poly.FillImage = bpm;
                    }
                }
            }

            foreach (var rect in scene.Rectangles)
                rect.Update();

            return scene;
        }

    }

    // https://stackoverflow.com/a/69664645
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => ColorTranslator.FromHtml(reader.GetString());

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
            => writer.WriteStringValue("#" + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2").ToLower());
    }

}
