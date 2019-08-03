using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace RectO
{
    static class FileManagementController
    {
        public static BitmapSource CreateImage(int width, int height)
        {
            try
            {
                PixelFormat pf = PixelFormats.Bgr32;
                int Width = Convert.ToInt32(width);
                int Height = Convert.ToInt32(height);
                int rawStride = (Width * pf.BitsPerPixel) / 8;
                byte[] rawImage = new byte[rawStride * Height];
                for (long i = 0; i < rawImage.Length; i++)
                    rawImage[i] = 0xFF;
                BitmapSource bitmap = BitmapSource.Create(width, height,
                    96, 96, pf, null,
                    rawImage, rawStride);
                return bitmap;
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static BitmapImage OpenImage(string path)
        {
            try
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmp.EndInit();
                return bmp;
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static bool SaveImageRender(FrameworkElement visual, string path, ImageResolutions res)
        {
            try
            {
                var bmp = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Default);
                bmp.Render(visual);
                BitmapEncoder encoder;
                switch (res)
                {
                    case ImageResolutions.BMP:
                        encoder = new BmpBitmapEncoder();
                        break;
                    case ImageResolutions.GIF:
                        encoder = new GifBitmapEncoder();
                        break;
                    case ImageResolutions.JPG:
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ImageResolutions.PNG:
                        encoder = new PngBitmapEncoder();
                        break;
                    case ImageResolutions.TIF:
                        encoder = new TiffBitmapEncoder();
                        break;
                    case ImageResolutions.WDP:
                        encoder = new WmpBitmapEncoder();
                        break;
                    default:
                        encoder = new JpegBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (var img = File.Create(path))
                    encoder.Save(img);
                return true;
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

    }
}
