using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectO
{
    public class ImageCanvas : Canvas
    {
        public ImageDataManagementController dataController;

        public static Cursor penCursor = new Cursor(Application.GetResourceStream(new Uri("/Image/Cursors/Handwriting.ani", UriKind.Relative)).Stream);

        public List<Layer> layers;
        public int currentLayerIndex;

        public Pen pen = new Pen(new SolidColorBrush(Colors.Black), 1);
        public Brush brush = new SolidColorBrush(Colors.Transparent);

        public ImageResolutions openfileRes;
        private string openfileName;
        public string OpenfileName
        {
            get { return openfileName; }
            set { openfileName = value; }
        }

        public bool lastChangeSaved;

        private int layerNum;
        public int LayerNum
        {
            get { return layerNum; }
            set { layerNum = value; }
        }

        public ImageCanvas()
        {
            layers = new List<Layer>();
            this.Background = Brushes.Transparent;
            dataController = new ImageDataManagementController(50);
            OpenfileName = "";
            openfileRes = ImageResolutions.Undefined;
            this.LayerNum = 1;
            lastChangeSaved = true;
            Focusable = true;
            Cursor = ImageCanvas.penCursor;
        }

        public void AddFirstLayer(ImageSource bmp)
        {
            if (bmp != null)
            {
                Layer newLayer = new Layer(bmp);
                newLayer.LayerName = "Фон";
                layers.Add(newLayer);
                this.Children.Add(newLayer);
                newLayer.SetValue(ZIndexProperty, 0);
                currentLayerIndex = layers.IndexOf(newLayer);
                this.Resize(bmp.Height, bmp.Width);
            }
        }

        public void AddNewLayer(ImageSource bmp)
        {
            if (bmp != null)
            {
                Layer newLayer = new Layer(bmp);
                newLayer.LayerName = "Слой " + this.LayerNum;
                newLayer.SetValue(ZIndexProperty, this.LayerNum);
                this.LayerNum++;
                layers.Add(newLayer);
                this.Children.Add(newLayer);
                newLayer.SetValue(ZIndexProperty, 0);
                currentLayerIndex = layers.IndexOf(newLayer);
                this.Resize(bmp.Height, bmp.Width);
            }
        }

        public void AddNewLayer(ImageSource bmp, string name)
        {
            if (bmp != null)
            {
                foreach (Layer layer in this.layers)
                    layer.IsSelected = false;
                Layer newLayer = new Layer(bmp);
                newLayer.LayerName = name;
                layers.Add(newLayer);
                this.Children.Add(newLayer);
                newLayer.SetValue(ZIndexProperty, 0);
                currentLayerIndex = layers.IndexOf(newLayer);
                this.Resize(bmp.Height, bmp.Width);
            }
        }

        public void AddNewEmptyLayer(double height, double width)
        {
            foreach (Layer layer in this.layers)
                layer.IsSelected = false;
            Layer newLayer = new Layer();
            newLayer.LayerName = "Слой " + this.LayerNum;
            newLayer.SetValue(ZIndexProperty, this.LayerNum);
            this.LayerNum++;
            layers.Add(newLayer);
            this.Children.Add(newLayer);
            currentLayerIndex = layers.IndexOf(newLayer);
            this.Resize(height, width);
        }

        public void RemoveLayerAt(int number)
        {
            layers.RemoveAt(number);
            this.Children.RemoveAt(number);
            this.currentLayerIndex = (number == 0 ? 0 : number - 1);
        }

        public void Clear()
        {
            this.currentLayerIndex = -1;
            this.LayerNum = 1;
            this.layers.Clear();
            this.Children.Clear();
        }

        public void Resize(double height, double width)
        {
            this.Height = height;
            this.Width = width;
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].Width = layers[i].image.Width = width;
                layers[i].Height = layers[i].image.Height = height;
            }
        }

    }
}
