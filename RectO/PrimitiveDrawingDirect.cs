using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace RectO
{
    class PrimitiveDrawingDirect
    {
        public Draw currentDrawing;

        public ImageCanvas CanvasWithImage;

        public void SetDrawing(Draw drawing)
        {
            drawing.CanvasWithImage = this.CanvasWithImage;
            if (currentDrawing != null)
                if (currentDrawing.GetType() == typeof(DrawText))
                    (currentDrawing as DrawText).ResetPoints();
            currentDrawing = drawing;
        }

        public PrimitiveDrawingDirect(ImageCanvas canv)
        {
            this.CanvasWithImage = canv;
            currentDrawing = null;
        }

        public void DrawOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (currentDrawing != null)
                    currentDrawing.draw_MouseDown(sender, e);
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DrawOnMouseMove(object sender, MouseEventArgs e)
        {
            if (currentDrawing != null)
                currentDrawing.draw_MouseMove(sender, e);
        }

        public void DrawOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentDrawing != null)
                currentDrawing.draw_MouseUp(sender, e);
        }

        public abstract class Draw
        {
            public ImageCanvas CanvasWithImage;

            public abstract void ResetPoints();
            public abstract void draw_MouseDown(object sender, MouseButtonEventArgs e);
            public abstract void draw_MouseMove(object sender, MouseEventArgs e);
            public abstract void draw_MouseUp(object sender, MouseButtonEventArgs e);

            protected void Rendering()
            {
                int index = -1;
                var layers = new List<LayerInfo>();
                foreach (Layer lay in this.CanvasWithImage.layers)
                {
                    layers.Add(new LayerInfo(lay.image.image.ImageSource, lay.LayerName));
                    if (lay.Visibility == Visibility.Hidden)
                        index = this.CanvasWithImage.layers.IndexOf(lay);
                }
                this.CanvasWithImage.dataController.Next(layers);
                for (int i = 0; i < this.CanvasWithImage.layers.Count; i++)
                {
                    if (i == index)
                        this.CanvasWithImage.layers[i].Visibility = Visibility.Visible;
                    var bmp = new RenderTargetBitmap((int)this.CanvasWithImage.ActualWidth, (int)this.CanvasWithImage.ActualHeight, 96, 96, PixelFormats.Default);
                    bmp.Render(this.CanvasWithImage.layers[i]);
                    this.CanvasWithImage.layers[i].image.image.ImageSource = bmp;
                    this.CanvasWithImage.layers[i].image.SaveVisual();
                    this.CanvasWithImage.layers[i].image.ClearVisual();
                    if (i == index)
                        this.CanvasWithImage.layers[i].Visibility = Visibility.Hidden;
                }
                this.CanvasWithImage.lastChangeSaved = false;
            }
        }

        public class DrawLine : Draw
        {
            Point beginPoint;
            Point endPoint;
            bool move = false;

            public override void ResetPoints()
            {
                beginPoint = endPoint = default(Point);
                move = false;
            }

            public override void  draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    beginPoint = e.GetPosition(this.CanvasWithImage);
                    move = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    move = false;
                }
            }
            
            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && move)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext context = visual.RenderOpen();
                    endPoint = e.GetPosition(this.CanvasWithImage);
                    context.DrawLine(this.CanvasWithImage.pen, beginPoint, endPoint);
                    context.Close();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Rendering();
                        move = false;
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawRect : Draw
        {
            Point beginPoint;
            Point endPoint;
            bool move = false;

            public override void ResetPoints()
            {
                beginPoint = endPoint = default(Point);
                move = false;
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    beginPoint = e.GetPosition(this.CanvasWithImage);
                    move = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    move = false;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && move)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext context = visual.RenderOpen();
                    endPoint = e.GetPosition(this.CanvasWithImage);
                    context.DrawRectangle(this.CanvasWithImage.brush, this.CanvasWithImage.pen, new Rect(beginPoint, endPoint));
                    context.Close();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Rendering();
                        move = false;
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawEllipse : Draw
        {
            Point beginPoint;
            Point endPoint;
            bool move = false;

            public override void ResetPoints()
            {
                beginPoint = endPoint = default(Point);
                move = false;
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    beginPoint = e.GetPosition(this.CanvasWithImage);
                    move = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    move = false;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && move)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext context = visual.RenderOpen();
                    endPoint = e.GetPosition(this.CanvasWithImage);
                    double radiusX, radiusY;
                    if (Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Toggled)
                        radiusX = radiusY = Math.Sqrt(Math.Pow(beginPoint.X - endPoint.X, 2) + Math.Pow(beginPoint.Y - endPoint.Y, 2));
                    else
                    {
                        radiusX = beginPoint.X - endPoint.X;
                        radiusY = beginPoint.Y - endPoint.Y;
                    }
                    context.DrawEllipse(this.CanvasWithImage.brush, this.CanvasWithImage.pen, beginPoint, radiusX, radiusY);
                    context.Close();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Rendering();
                        move = false;
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawPen : Draw
        {
            Point beginPoint;
            Point endPoint;
            bool move = false;

            public override void ResetPoints()
            {
                beginPoint = endPoint = default(Point);
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    beginPoint = endPoint = e.GetPosition(this.CanvasWithImage);
                    move = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                    move = false;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && move)
                {
                    if (beginPoint == default(Point) || endPoint == default(Point))
                        beginPoint = endPoint = e.GetPosition(this.CanvasWithImage);
                    else
                        endPoint = e.GetPosition(this.CanvasWithImage);
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext context = visual.RenderOpen();
                    Pen penWithoutThickness = new Pen(this.CanvasWithImage.pen.Brush, 1);
                    context.DrawLine(penWithoutThickness, beginPoint, endPoint);
                    beginPoint = endPoint;
                    context.Close();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Rendering();
                        move = false;
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawTriangle : Draw
        {
            Point firstPoint;
            Point secondPoint;
            Point thirdPoint;
            bool move = false;
            bool drawingFirstLine = true;

            public override void ResetPoints()
            {
                firstPoint = secondPoint = thirdPoint = default(Point);
                move = false;
                drawingFirstLine = true;
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (drawingFirstLine)
                    {
                        firstPoint = secondPoint = thirdPoint = e.GetPosition(this.CanvasWithImage);
                        move = true;
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                    move = false;
                    drawingFirstLine = true;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (move)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext cont = visual.RenderOpen();
                    if (e.LeftButton == MouseButtonState.Pressed && drawingFirstLine)
                    {
                        secondPoint = e.GetPosition(this.CanvasWithImage);
                        cont.DrawLine(this.CanvasWithImage.pen, firstPoint, secondPoint);
                    }
                    else
                    {
                        thirdPoint = e.GetPosition(this.CanvasWithImage);
                        cont.DrawLine(this.CanvasWithImage.pen, secondPoint, thirdPoint);
                        cont.DrawLine(this.CanvasWithImage.pen, thirdPoint, firstPoint);
                    }
                    cont.Close();
                    CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (drawingFirstLine)
                        {
                            this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.SaveVisual();
                            drawingFirstLine = false;
                        }
                        else
                        {
                            this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                            DrawingVisual currentVisual = new DrawingVisual();
                            DrawingContext context = currentVisual.RenderOpen();
                            PathGeometry geom = new PathGeometry();
                            PathFigure triangle = new PathFigure();
                            LineSegment line1 = new LineSegment(secondPoint, true);
                            LineSegment line2 = new LineSegment(thirdPoint, true);
                            triangle.IsClosed = true;
                            triangle.IsFilled = true;
                            triangle.StartPoint = firstPoint;
                            triangle.Segments.Add(line1);
                            triangle.Segments.Add(line2);
                            geom.Figures.Add(triangle);
                            context.DrawGeometry(this.CanvasWithImage.brush, this.CanvasWithImage.pen, geom);
                            context.Close();
                            this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(currentVisual);
                            Rendering();
                            move = false;
                            drawingFirstLine = true;
                        }
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                    {
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                        move = false;
                        drawingFirstLine = true;
                    }
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawSegment : Draw
        {
            Point firstPoint;
            Point secondPoint;
            Point thirdPoint;
            bool move = false;
            bool drawingFirstLine = true;

            public override void ResetPoints()
            {
                firstPoint = secondPoint = thirdPoint = default(Point);
                move = false;
                drawingFirstLine = true;
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (drawingFirstLine)
                    {
                        firstPoint = secondPoint = thirdPoint = e.GetPosition(this.CanvasWithImage);
                        move = true;
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                    move = false;
                    drawingFirstLine = true;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (move)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext cont = visual.RenderOpen();
                    if (e.LeftButton == MouseButtonState.Pressed && drawingFirstLine)
                    {
                        secondPoint = e.GetPosition(this.CanvasWithImage);
                        cont.DrawLine(this.CanvasWithImage.pen, firstPoint, secondPoint);
                    }
                    else
                    {
                        thirdPoint = e.GetPosition(this.CanvasWithImage);
                        PathGeometry geom = new PathGeometry();
                        PathFigure segment = new PathFigure();
                        double radius = Math.Sqrt(Math.Pow(secondPoint.X - firstPoint.X, 2) + Math.Pow(secondPoint.Y - firstPoint.Y, 2));
                        double x0 = firstPoint.X,
                                y0 = firstPoint.Y,
                                x1 = thirdPoint.X,
                                y1 = thirdPoint.Y;
                        double k = (x1 - x0) / (y1 - y0);
                        int modY = thirdPoint.Y > firstPoint.Y ? 1 : -1;
                        double y = modY * Math.Sqrt(Math.Pow(radius, 2) / (Math.Pow(k, 2) + 1)) + y0;
                        double x = k * (y - y0) + x0;
                        Point pointOnArc = new Point(x, y);
                        thirdPoint = pointOnArc;
                        bool isLargeArc = true;
                        if (secondPoint.X > firstPoint.X && secondPoint.Y < firstPoint.Y)
                            if (thirdPoint.X < secondPoint.X && thirdPoint.Y < firstPoint.Y + (firstPoint.Y - secondPoint.Y))
                                isLargeArc = false;
                            else
                                isLargeArc = true;
                        else if (secondPoint.X < firstPoint.X && secondPoint.Y < firstPoint.Y)
                            if (thirdPoint.X < firstPoint.X + (firstPoint.X - secondPoint.X) && thirdPoint.Y > secondPoint.Y)
                                isLargeArc = false;
                            else
                                isLargeArc = true;
                        else if (secondPoint.X < firstPoint.X && secondPoint.Y > firstPoint.Y)
                            if (thirdPoint.X > secondPoint.X && thirdPoint.Y > firstPoint.Y + (firstPoint.Y - secondPoint.Y))
                                isLargeArc = false;
                            else
                                isLargeArc = true;
                        else if (secondPoint.X > firstPoint.X && secondPoint.Y > firstPoint.Y)
                            if (thirdPoint.X > firstPoint.X + (firstPoint.X - secondPoint.X) && thirdPoint.Y < secondPoint.Y)
                                isLargeArc = false;
                            else
                                isLargeArc = true;
                        ArcSegment arc = new ArcSegment(thirdPoint, new Size(radius, radius), 0, !isLargeArc, SweepDirection.Clockwise, true);
                        LineSegment line = new LineSegment(firstPoint, true);
                        segment.StartPoint = secondPoint;
                        segment.IsFilled = false;
                        segment.Segments.Add(arc);
                        segment.Segments.Add(line);
                        geom.Figures.Add(segment);
                        cont.DrawGeometry(this.CanvasWithImage.brush, this.CanvasWithImage.pen, geom);
                    }
                    cont.Close();
                    CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                    CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (drawingFirstLine)
                        {
                            this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.SaveVisual();
                            drawingFirstLine = false;
                        }
                        else
                        {
                            this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                            DrawingVisual currentVisual = new DrawingVisual();
                            DrawingContext context = currentVisual.RenderOpen();
                            PathGeometry geom = new PathGeometry();
                            PathFigure segment = new PathFigure();
                            double radius = Math.Sqrt(Math.Pow(secondPoint.X - firstPoint.X, 2) + Math.Pow(secondPoint.Y - firstPoint.Y, 2));
                            bool isLargeArc = true;
                            if (secondPoint.X > firstPoint.X && secondPoint.Y < firstPoint.Y)
                                if (thirdPoint.X < secondPoint.X && thirdPoint.Y < firstPoint.Y + (firstPoint.Y - secondPoint.Y))
                                    isLargeArc = false;
                                else
                                    isLargeArc = true;
                            else if (secondPoint.X < firstPoint.X && secondPoint.Y < firstPoint.Y)
                                if (thirdPoint.X < firstPoint.X + (firstPoint.X - secondPoint.X) && thirdPoint.Y > secondPoint.Y)
                                    isLargeArc = false;
                                else
                                    isLargeArc = true;
                            else if (secondPoint.X < firstPoint.X && secondPoint.Y > firstPoint.Y)
                                if (thirdPoint.X > secondPoint.X && thirdPoint.Y > firstPoint.Y + (firstPoint.Y - secondPoint.Y))
                                    isLargeArc = false;
                                else
                                    isLargeArc = true;
                            else if (secondPoint.X > firstPoint.X && secondPoint.Y > firstPoint.Y)
                                if (thirdPoint.X > firstPoint.X + (firstPoint.X - secondPoint.X) && thirdPoint.Y < secondPoint.Y)
                                    isLargeArc = false;
                                else
                                    isLargeArc = true;
                            LineSegment line = new LineSegment(secondPoint, true);
                            ArcSegment arc = new ArcSegment(thirdPoint, new Size(radius, radius), 0, !isLargeArc, SweepDirection.Clockwise, true);
                            segment.StartPoint = firstPoint;
                            segment.IsClosed = true;
                            segment.IsFilled = true;
                            segment.Segments.Add(line);
                            segment.Segments.Add(arc);
                            geom.Figures.Add(segment);
                            context.DrawGeometry(this.CanvasWithImage.brush, this.CanvasWithImage.pen, geom);
                            context.DrawGeometry(this.CanvasWithImage.brush, this.CanvasWithImage.pen, geom);
                            context.Close();
                            this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(currentVisual);
                            Rendering();
                            move = false;
                            drawingFirstLine = true;
                        }
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                    {
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                        move = false;
                        drawingFirstLine = true;
                    }
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawBrush : Draw
        {
            Point beginPoint;
            Point mediumPoint;
            Point mediumMediumPoint;
            Point endPoint;
            bool move = false;
            Pen brushStrokePen;

            public override void ResetPoints()
            {
                beginPoint = mediumPoint = mediumMediumPoint = endPoint = default(Point);
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    brushStrokePen = new Pen(this.CanvasWithImage.pen.Brush, this.CanvasWithImage.pen.Thickness);
                    brushStrokePen.EndLineCap = PenLineCap.Round;
                    brushStrokePen.StartLineCap = PenLineCap.Round;
                    brushStrokePen.LineJoin = PenLineJoin.Round;
                    beginPoint = mediumPoint = mediumMediumPoint = endPoint = e.GetPosition(this.CanvasWithImage);
                    move = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                    move = false;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && move)
                {
                    if (beginPoint == default(Point) || mediumPoint == default(Point) || mediumMediumPoint == default(Point) || endPoint == default(Point))
                        beginPoint = mediumPoint = mediumMediumPoint = endPoint = e.GetPosition(this.CanvasWithImage);
                    else
                    {
                        beginPoint = mediumPoint;
                        mediumPoint = mediumMediumPoint;
                        mediumMediumPoint = endPoint;
                        endPoint = e.GetPosition(this.CanvasWithImage);
                    }
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext context = visual.RenderOpen();
                    PathGeometry geom = new PathGeometry();
                    PathFigure pathTria = new PathFigure();
                    LineSegment line1 = new LineSegment(mediumPoint, true);
                    LineSegment line2 = new LineSegment(mediumMediumPoint, true);
                    LineSegment line3 = new LineSegment(endPoint, true);
                    pathTria.Segments.Add(line1);
                    pathTria.Segments.Add(line2);
                    pathTria.Segments.Add(line3);
                    pathTria.StartPoint = beginPoint;
                    pathTria.IsClosed = true;
                    pathTria.IsFilled = true;
                    geom.Figures.Add(pathTria);
                    context.DrawGeometry(this.brushStrokePen.Brush, this.brushStrokePen, geom);
                    context.Close();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Rendering();
                        move = false;
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public class DrawText : Draw
        {
            Point textPoint;
            TextBox textBox;

            public override void ResetPoints()
            {
                textPoint = default(Point);
                this.textBox_LostFocus(textBox, new RoutedEventArgs());
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {

            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left && textBox == null)
                    {
                        textPoint = e.GetPosition(this.CanvasWithImage);
                        textBox = new TextBox();
                        textBox.Name = "textBox";
                        textBox.MinWidth = 20;
                        textBox.FontSize = 14;
                        textBox.IsEnabled = true;
                        textBox.AcceptsReturn = true;
                        textBox.AcceptsTab = true;
                        textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                        textBox.VerticalAlignment = VerticalAlignment.Stretch;
                        textBox.Visibility = Visibility.Visible;
                        textBox.Background = this.CanvasWithImage.brush;
                        textBox.Foreground = this.CanvasWithImage.pen.Brush;
                        textBox.BorderThickness = new Thickness(0.5, 0.5, 1, 1);
                        textBox.Margin = new Thickness(textPoint.X, textPoint.Y - 7, 0, 0);
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].Children.Add(textBox);
                        textBox.Focus();
                        textBox.LostFocus += new RoutedEventHandler(textBox_LostFocus);
                    }
                    else if (textBox != null)
                        this.ResetPoints();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            private void textBox_LostFocus(object sender, RoutedEventArgs e)
            {
                if (textBox != null)
                {
                    textBox.BorderThickness = new Thickness(0);
                    textBox.SelectionBrush = Brushes.Transparent;
                    textBox.SelectAll();
                    this.CanvasWithImage.UpdateLayout();
                    Rendering();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].Children.Remove(textBox);
                    this.textBox = null;
                }
            }
        }

        public class DrawEraser : Draw
        {
            Point beginPoint;
            Point endPoint;
            bool move = false;

            public override void ResetPoints()
            {
                beginPoint = endPoint = default(Point);
            }

            public override void draw_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    beginPoint = endPoint = e.GetPosition(this.CanvasWithImage);
                    move = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.ClearVisual();
                    move = false;
                }
            }

            public override void draw_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && move)
                {
                    if (beginPoint == default(Point) || endPoint == default(Point))
                        beginPoint = endPoint = e.GetPosition(this.CanvasWithImage);
                    else
                        endPoint = e.GetPosition(this.CanvasWithImage);
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext context = visual.RenderOpen();
                    Pen penEraser = new Pen(Brushes.White, this.CanvasWithImage.pen.Thickness > 5 ? this.CanvasWithImage.pen.Thickness : 5);
                    penEraser.StartLineCap = penEraser.EndLineCap = PenLineCap.Round;
                    context.DrawLine(penEraser, beginPoint, endPoint);
                    beginPoint = endPoint;
                    context.Close();
                    this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.AddVisual(visual);
                }
            }

            public override void draw_MouseUp(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Rendering();
                        move = false;
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                        this.CanvasWithImage.layers[this.CanvasWithImage.currentLayerIndex].image.DeleteVisual();
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
