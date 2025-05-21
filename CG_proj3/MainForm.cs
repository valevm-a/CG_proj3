using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows.Forms;

namespace CG_proj3
{
    public partial class MainForm : Form
    {
        enum Tool { None, Line, Circle, Polygon, Rectangle, Delete, Color, Thickness, Clipping, ColorFill, ImageFill }

        Scene scene;
        DirectBitmap bitmap;
        Tool currentTool = Tool.None;
        bool isDrawing = false;

        Color brushColor = Color.Black;
        int brushThickness = 1;
        bool antiAliasing = false;

        LineShape currentLine;
        private bool isDraggingStart = false;
        private bool isDraggingEnd = false;

        private CircleShape currentCircle;
        private bool isDraggingCircleRadius = false;
        private bool isDraggingCircle = false;
        private Point dragOffset;

        private PolygonShape currentPolygon;
        private bool isCompletePolygon = false;
        private bool isDraggingVertex = false;
        private bool isDraggingLine = false;
        private bool isDraggingPolygon = false;
        int draggingVertexIndex;
        int draggingLineIndex;

        private RectangleShape currentRectangle;
        private bool isDraggingEdge = false;
        private bool isDraggingRectangle = false;

        private bool isClipping = false;
        private PolygonShape clippedPolygon;
        private RectangleShape clippedRectangle;

        private PolygonShape filledPolygon;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            scene = new Scene(pictureBox1.Width, pictureBox1.Height);
            bitmap = scene.Bitmap;
            bitmap.Clear(Color.White);
            pictureBox1.Image = bitmap.Bitmap;
            pictureBox1.Invalidate();
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Line;
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Circle;
        }

        private void polygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Polygon;
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Rectangle;
        }

        private void deleteShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Delete;
        }

        private void changeThicknessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Thickness;
        }

        private void clippingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.Clipping;

            if (isClipping)
                scene.ClippedEdges.Clear();

            isClipping = !isClipping;
            clippingToolStripMenuItem.Checked = isClipping;
            Redraw();
        }

        private void antiAliasingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            antiAliasing = !antiAliasing;
            antiAliasingToolStripMenuItem.Checked = antiAliasing;
            Redraw();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON|*.json";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    scene = Scene.LoadScene(openFileDialog.FileName, pictureBox1.Width, pictureBox1.Height);

                    bitmap = scene.Bitmap;
                    Redraw();
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON|*.json";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    scene.SaveScene(saveFileDialog.FileName);
                }

            }
        }
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scene.Clear();
            Redraw();
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                brushColor = colorDialog1.Color;
            }
            currentTool = Tool.Color;
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            brushThickness = (int)numericUpDown1.Value;
        }

        private void solidColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.ColorFill;
        }

        private void pictureFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentTool = Tool.ImageFill;
        }


            private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentTool == Tool.Line)
            {
                foreach (var line in scene.Lines)
                {
                    if (line.HitTest(e.Location))
                    {
                        if (Math.Abs(e.X - line.Start.X) < 5 && Math.Abs(e.Y - line.Start.Y) < 5)
                        {
                            currentLine = line;
                            isDraggingStart = true;
                            return;
                        }
                        else if (Math.Abs(e.X - line.End.X) < 5 && Math.Abs(e.Y - line.End.Y) < 5)
                        {
                            currentLine = line;
                            isDraggingEnd = true;
                            return;
                        }
                    }
                }

                currentLine = new LineShape()
                {
                    Start = e.Location,
                    End = e.Location,
                    Color = brushColor,
                    Thickness = brushThickness
                };
                scene.AddShape(currentLine);
                isDrawing = true;
            }

            else if (currentTool == Tool.Circle)
            {
                foreach (var circle in scene.Circles)
                {
                    int dx = e.X - circle.Center.X;
                    int dy = e.Y - circle.Center.Y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (Math.Abs(distance - circle.Radius) <= 5)
                    {
                        currentCircle = circle;
                        isDraggingCircleRadius = true;
                        return;
                    }

                    if (distance < circle.Radius - 10)
                    {
                        currentCircle = circle;
                        isDraggingCircle = true;
                        dragOffset = new Point(dx, dy);
                        return;
                    }
                }

                currentCircle = new CircleShape
                {
                    Center = e.Location,
                    Radius = 0,
                    Color = brushColor,
                    Thickness = brushThickness
                };

                scene.AddShape(currentCircle);
                isDrawing = true;
            }

            else if (currentTool == Tool.Polygon)
            {
                if (e.Button == MouseButtons.Right && isDrawing && currentPolygon.Vertices.Count > 2)
                {
                    currentPolygon.LineSegments[^1].End = currentPolygon.Vertices[0];
                    isDrawing = false;
                    isCompletePolygon = true;
                    Redraw();
                    return;
                }

                if (!isDrawing)
                {
                    foreach (var poly in scene.Polygons)
                    {
                        int hitIndex = poly.HitTestVertex(e.Location);
                        if (hitIndex != -1)
                        {
                            currentPolygon = poly;
                            draggingVertexIndex = hitIndex;
                            isDraggingVertex = true;
                            return;
                        }

                        for (int i = 0; i < poly.LineSegments.Count; i++)
                        {
                            if (poly.LineSegments[i].HitTest(e.Location))
                            {
                                currentPolygon = poly;
                                draggingLineIndex = i;
                                isDraggingLine = true;
                                dragOffset = e.Location;
                                return;
                            }
                        }

                        Point centroid = poly.GetCentroid();
                        int dx = e.X - centroid.X;
                        int dy = e.Y - centroid.Y;
                        double distToCentroid = Math.Sqrt(dx * dx + dy * dy);

                        if (distToCentroid < 15)
                        {
                            currentPolygon = poly;
                            isDraggingPolygon = true;
                            dragOffset = new Point(dx, dy);
                            return;
                        }
                    }
                }

                if (!isDrawing)
                {
                    currentPolygon = new PolygonShape
                    {
                        Color = brushColor,
                        Thickness = brushThickness
                    };
                    currentPolygon.Vertices.Clear();
                    isDrawing = true;
                    isCompletePolygon = false;
                    currentPolygon.Vertices.Add(e.Location);
                    currentPolygon.AddLineSegment(e.Location, e.Location);

                    scene.AddShape(currentPolygon);
                }
                else
                {
                    currentPolygon.Vertices.Add(e.Location);
                    currentPolygon.AddLineSegment(currentPolygon.Vertices[^2], currentPolygon.Vertices[^1]);
                }

                Redraw();
            }

            else if (currentTool == Tool.Rectangle)
            {
                if (!isDrawing)
                {
                    foreach (var rect in scene.Rectangles)
                    {
                        int hitIndex = rect.HitTestVertex(e.Location);
                        if (hitIndex != -1)
                        {
                            currentRectangle = rect;
                            draggingVertexIndex = hitIndex;
                            isDraggingVertex = true;
                            return;
                        }

                        foreach (var edge in rect.Edges)
                        {
                            if (edge.HitTest(e.Location))
                            {
                                currentRectangle = rect;
                                isDraggingEdge = true;
                                dragOffset = e.Location;
                                return;
                            }
                        }

                        Point centroid = rect.GetCentroid();
                        int dx = e.X - centroid.X;
                        int dy = e.Y - centroid.Y;
                        double distToCentroid = Math.Sqrt(dx * dx + dy * dy);

                        if (distToCentroid < 30)
                        {
                            currentRectangle = rect;
                            isDraggingRectangle = true;
                            dragOffset = new Point(e.X - currentRectangle.GetCentroid().X, e.Y - currentRectangle.GetCentroid().Y);
                            return;
                        }
                    }
                }

                if (!isDrawing)
                {
                    currentRectangle = new RectangleShape
                    {
                        TopLeft = e.Location,
                        BottomRight = e.Location,
                        Color = brushColor,
                        Thickness = brushThickness
                    };
                    currentRectangle.Update();
                    scene.AddShape(currentRectangle);
                    isDrawing = true;
                }
            }

            else if (currentTool == Tool.Clipping)
            {
                var clippedShape = scene.HitTest(e.Location);
                if (clippedShape is PolygonShape)
                    clippedPolygon = (PolygonShape)clippedShape;

                if (clippedShape is RectangleShape)
                    clippedRectangle = (RectangleShape)clippedShape;

                if (clippedPolygon != null && clippedRectangle != null)
                    scene.ClipPolygonEdges(clippedPolygon, clippedRectangle);

                Redraw();
            }

            else if (currentTool == Tool.ColorFill)
            {
                var filledShape = scene.HitTest(e.Location);

                if (filledShape is PolygonShape)
                    filledPolygon = (PolygonShape)filledShape;

                if (filledPolygon != null)
                {
                    if (colorDialog1.ShowDialog() == DialogResult.OK)
                    {
                        filledPolygon.FillMode = FillType.SolidColor;
                        filledPolygon.FillColor = colorDialog1.Color;
                    }

                    Redraw();
                }
            }

            else if (currentTool == Tool.ImageFill)
            {
                var filledShape = scene.HitTest(e.Location);

                if (filledShape is PolygonShape)
                    filledPolygon = (PolygonShape)filledShape;

                if (filledPolygon != null)
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|" +
                            "JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "PNG Files (*.png)|*.png|" + "All Files (*.*)|*.*";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (Bitmap original = new Bitmap(openFileDialog.FileName))
                            {
                                DirectBitmap bpm = new DirectBitmap(original);
                                filledPolygon.FillImage = bpm;
                            }

                            filledPolygon.FillMode = FillType.Image;
                            filledPolygon.FillImagePath = openFileDialog.FileName;

                            Redraw();
                        }
                    }
                }
            }

            else if (currentTool == Tool.Delete)
            {
                scene.DeleteShapeAt(e.Location);
                Redraw();
            }

            else if (currentTool == Tool.Thickness)
            {
                foreach (var line in scene.Lines)
                {
                    if (line.HitTest(e.Location))
                        line.Thickness = brushThickness;
                }

                foreach (var circle in scene.Circles)
                {
                    if (circle.HitTest(e.Location))
                        circle.Thickness = brushThickness;
                }

                foreach (var poly in scene.Polygons)
                {
                    if (poly.HitTest(e.Location))
                    {
                        poly.Thickness = brushThickness;
                        poly.ChangeThickness();
                    }
                }

                foreach (var rect in scene.Rectangles)
                {
                    if (rect.HitTest(e.Location))
                    {
                        rect.Thickness = brushThickness;
                        rect.ChangeThickness();
                    }
                }

                Redraw();
            }

            else if (currentTool == Tool.Color)
            {
                foreach (var line in scene.Lines)
                {
                    if (line.HitTest(e.Location))
                        line.Color = brushColor;
                }

                foreach (var circle in scene.Circles)
                {
                    if (circle.HitTest(e.Location))
                        circle.Color = brushColor;
                }

                foreach (var poly in scene.Polygons)
                {
                    if (poly.HitTest(e.Location))
                    {
                        poly.Color = brushColor;
                        poly.ChangeColor();
                    }
                }

                foreach (var rect in scene.Rectangles)
                {
                    if (rect.HitTest(e.Location))
                    {
                        rect.Color = brushColor;
                        rect.ChangeColor();
                    }
                }

                Redraw();
            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentTool == Tool.Line && currentLine != null)
            {
                if (isDraggingStart)
                    currentLine.Start = e.Location;

                else if (isDraggingEnd || isDrawing)
                    currentLine.End = e.Location;

                Redraw();
            }

            else if (currentTool == Tool.Circle && currentCircle != null)
            {
                int dx = e.X - currentCircle.Center.X;
                int dy = e.Y - currentCircle.Center.Y;

                if (isDraggingCircleRadius)
                    currentCircle.Radius = (int)Math.Sqrt(dx * dx + dy * dy);

                else if (isDrawing)
                    currentCircle.Radius = (int)Math.Sqrt(dx * dx + dy * dy);

                else if (isDraggingCircle)
                    currentCircle.Center = new Point(e.X - dragOffset.X, e.Y - dragOffset.Y);

                Redraw();
            }

            else if (currentTool == Tool.Polygon && currentPolygon != null)
            {
                if (isDraggingVertex)
                {
                    currentPolygon.Vertices[draggingVertexIndex] = e.Location;

                    int prevIndex = (draggingVertexIndex - 1 + currentPolygon.Vertices.Count) % currentPolygon.Vertices.Count;
                    int nextIndex = draggingVertexIndex;

                    currentPolygon.LineSegments[prevIndex].End = e.Location;
                    currentPolygon.LineSegments[nextIndex].Start = e.Location;
                }

                else if (isDraggingLine)
                {
                    int dx = e.X - dragOffset.X;
                    int dy = e.Y - dragOffset.Y;
                    dragOffset = e.Location;

                    var segment = currentPolygon.LineSegments[draggingLineIndex];
                    Point oldStart = segment.Start;
                    Point oldEnd = segment.End;

                    Point newStart = new Point(oldStart.X + dx, oldStart.Y + dy);
                    Point newEnd = new Point(oldEnd.X + dx, oldEnd.Y + dy);

                    segment.Start = newStart;
                    segment.End = newEnd;

                    int startIndex = currentPolygon.Vertices.FindIndex(p => p == oldStart);
                    if (startIndex != -1)
                        currentPolygon.Vertices[startIndex] = newStart;

                    int endIndex = currentPolygon.Vertices.FindIndex(p => p == oldEnd);
                    if (endIndex != -1)
                        currentPolygon.Vertices[endIndex] = newEnd;

                    int prevIndex = (draggingLineIndex - 1 + currentPolygon.LineSegments.Count) % currentPolygon.LineSegments.Count;
                    if (currentPolygon.LineSegments[prevIndex].End == oldStart)
                        currentPolygon.LineSegments[prevIndex].End = newStart;

                    int nextIndex = (draggingLineIndex + 1) % currentPolygon.LineSegments.Count;
                    if (currentPolygon.LineSegments[nextIndex].Start == oldEnd)
                        currentPolygon.LineSegments[nextIndex].Start = newEnd;

                    currentPolygon.Vertices.Clear();
                    foreach (var line in currentPolygon.LineSegments)
                        currentPolygon.Vertices.Add(line.Start);
                }

                else if (isDraggingPolygon)
                {
                    Point newCenter = e.Location;
                    int offsetX = newCenter.X - currentPolygon.GetCentroid().X;
                    int offsetY = newCenter.Y - currentPolygon.GetCentroid().Y;
                    currentPolygon.Translate(offsetX, offsetY);
                }

                else if (isDrawing)
                {
                    if (currentPolygon.Vertices.Count > 1 && currentPolygon.LineSegments.Count > 0)
                    {
                        currentPolygon.LineSegments.RemoveAt(currentPolygon.LineSegments.Count - 1);
                        currentPolygon.AddLineSegment(currentPolygon.Vertices[^1], e.Location);
                    }

                    else
                        currentPolygon.LineSegments[0].End = e.Location;
                }

                Redraw();
            }

            else if (currentTool == Tool.Rectangle && currentRectangle != null)
            {
                if (isDraggingVertex)
                {
                    currentRectangle.MoveVertex(draggingVertexIndex, e.Location);
                    Redraw();
                }

                else if (isDraggingRectangle)
                {
                    int newCentroidX = e.X - dragOffset.X;
                    int newCentroidY = e.Y - dragOffset.Y;

                    Point centroid = currentRectangle.GetCentroid();

                    int deltaX = newCentroidX - centroid.X;
                    int deltaY = newCentroidY - centroid.Y;

                    currentRectangle.TopLeft = new Point(currentRectangle.TopLeft.X + deltaX, currentRectangle.TopLeft.Y + deltaY);
                    currentRectangle.BottomRight = new Point(currentRectangle.BottomRight.X + deltaX, currentRectangle.BottomRight.Y + deltaY);

                    currentRectangle.Update();
                }

                else if (isDrawing)
                {
                    currentRectangle.BottomRight = e.Location;
                    currentRectangle.Update(); // bugs if top-left, fix
                }
                Redraw();
            }

            if (isClipping && clippedPolygon != null && clippedRectangle != null)
            {
                scene.ClipPolygonEdges(clippedPolygon, clippedRectangle);
                Redraw();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (currentTool == Tool.Line)
            {
                isDrawing = false;
                isDraggingStart = false;
                isDraggingEnd = false;
                currentLine = null;
                Redraw();
            }

            else if (currentTool == Tool.Circle)
            {
                isDrawing = false;
                isDraggingCircleRadius = false;
                isDraggingCircle = false;
                currentCircle = null;
                Redraw();
            }

            else if (currentTool == Tool.Polygon && !isDrawing)
            {
                isDraggingVertex = false;
                isDraggingPolygon = false;
                isDraggingLine = false;
                currentPolygon = null;
                Redraw();
            }

            else if (currentTool == Tool.Rectangle)
            {
                isDraggingVertex = false;
                isDraggingEdge = false;
                isDraggingRectangle = false;
                isDrawing = false;
                currentRectangle = null;
                Redraw();
            }
        }

        private void Redraw()
        {
            bitmap.Clear(Color.White);
            scene.DrawAll(bitmap, antiAliasing);
            pictureBox1.Image = bitmap.Bitmap;
            pictureBox1.Invalidate();
        }
    }
}
