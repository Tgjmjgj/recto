using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RectO
{
    public partial class ColorControl : UserControl
    {
        public static DependencyProperty ForeColorProperty;
        public static DependencyProperty BackColorProperty;
        public static DependencyProperty ElementSizeProperty;
        public static DependencyProperty ColorRectSizeProperty;
        public static DependencyProperty ReverseAutoSizeProperty;

        public static readonly RoutedEvent ForeColorChangedEvent;
        public static readonly RoutedEvent BackColorChangedEvent;

        public event MouseButtonEventHandler ForegroundRectClick;
        private void OnForegroundRectClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (ForegroundRectClick != null)
                    ForegroundRectClick(sender, e);
            }
        }

        public event MouseButtonEventHandler BackgroundRectClick;
        private void OnBackgroundRectClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (BackgroundRectClick != null)
                    BackgroundRectClick(sender, e);
            }
        }

        public event MouseButtonEventHandler ReverseImageButtonClick;
        private void OnReverseImageButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (ReverseImageButtonClick != null)
                    ReverseImageButtonClick(sender, e);
                Color glass = ForeColor;
                ForeColor = BackColor;
                BackColor = glass;
            }
        }
 
        public event RoutedPropertyChangedEventHandler<Color> ForeColorChanged
        {
            add { AddHandler(ForeColorChangedEvent, value); }
            remove { RemoveHandler(ForeColorChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<Color> BackColorChanged
        {
            add { AddHandler(ForeColorChangedEvent, value); }
            remove { RemoveHandler(ForeColorChangedEvent, value); }
        }

        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        public Color BackColor
        {
            get { return (Color)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        public int ElementSize
        {
            get { return (int)GetValue(ElementSizeProperty); }
            set { SetValue(ElementSizeProperty, value); }
        }
        
        public int ColorRectSize
        {
            get { return (int)GetValue(ColorRectSizeProperty); }
            set { SetValue(ColorRectSizeProperty, value); }
        }

        public int ReverseAutoSize
        {
            get { return (int)GetValue(ReverseAutoSizeProperty); }
            set { SetValue(ReverseAutoSizeProperty, value); }
        }

        private static void OnForeColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Color oldColor = (Color)e.OldValue,
                newColor = (Color)e.NewValue;
            RoutedPropertyChangedEventArgs<Color> arguments = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor);
            arguments.RoutedEvent = ColorControl.ForeColorChangedEvent;
            (sender as ColorControl).RaiseEvent(arguments);
        }

        private static void OnBackColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Color oldColor = (Color)e.OldValue,
                newColor = (Color)e.NewValue;
            RoutedPropertyChangedEventArgs<Color> arguments = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor);
            arguments.RoutedEvent = ColorControl.BackColorChangedEvent;
            (sender as ColorControl).RaiseEvent(arguments);
        }

        private static void OnSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorControl colorControl = sender as ColorControl;
            colorControl.ColorRectSize = Convert.ToInt32(e.NewValue)*3/4;
        }

        private static void OnRectoSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorControl colorControl = sender as ColorControl;
            colorControl.ReverseAutoSize = colorControl.ElementSize - Convert.ToInt32(e.NewValue);
        }

        static ColorControl()
        {
            ForeColorProperty = DependencyProperty.Register("ForeColor", typeof(Color), typeof(ColorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnForeColorChanged)));
            BackColorProperty = DependencyProperty.Register("BackColor", typeof(Color), typeof(ColorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBackColorChanged)));
            ElementSizeProperty = DependencyProperty.Register("ElementSize", typeof(int), typeof(ColorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSizeChanged)));
            ColorRectSizeProperty = DependencyProperty.Register("ColorRectSize", typeof(int), typeof(ColorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRectoSizeChanged)));
            ReverseAutoSizeProperty = DependencyProperty.Register("ReverseAutoSize", typeof(int), typeof(ColorControl));
            ForeColorChangedEvent = EventManager.RegisterRoutedEvent("ForeColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorControl));
            BackColorChangedEvent = EventManager.RegisterRoutedEvent("BackColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorControl));
        }

        public ColorControl()
        {
            InitializeComponent();
        }

    }
}
