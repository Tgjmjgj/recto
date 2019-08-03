using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectO
{
    public enum DragBorder { X, Y, XY, None };

    public enum ImageResolutions { BMP, GIF, JPG, PNG, TIF, WDP, Undefined };

    public class LayerInfo
    {
        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public LayerInfo(ImageSource bmp, string n)
        {
            this.Image = bmp;
            this.Name = n;
        }
    }
}
