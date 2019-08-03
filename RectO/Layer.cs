using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectO
{
    public class Layer : Canvas
    {
        public LayerCanvas image;

        private string layerName;
        public string LayerName
        {
            get { return layerName; }
            set { layerName = value; }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public Layer()
        {
            image = new LayerCanvas();
            this.Children.Add(image);
            IsSelected = true;
        }

        public Layer(ImageSource bmp)
        {
            layerName = "Фон";
            image = new LayerCanvas();
            image.image.ImageSource = bmp;
            this.Children.Add(image);
            IsSelected = true;
        }

    }
}
