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
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RectO
{
    /// <summary>
    /// Логика взаимодействия для ColorDialog.xaml
    /// </summary>
    public partial class ColorDialog : Window
    {
        public enum ColorSide { Fore, Back };
        private ColorSide currentColor;
        public ColorSide CurrentColor
        {
            get { return currentColor; }
            set { currentColor = value; }
        }

        private bool InputColor = false;

        public static bool IsCreated = false;

        public static DependencyProperty RedProperty;
        public static DependencyProperty GreenProperty;
        public static DependencyProperty BlueProperty;
        public static DependencyProperty AlphaProperty;
        public static DependencyProperty SelectedColorProperty;

        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }

        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        public byte Alpha
        {
            get { return (byte)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        static ColorDialog()
        {
            SelectedColorProperty = DependencyProperty.Register("SelectedProperty", typeof(Color), typeof(ColorDialog), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedColorChanged)));
            RedProperty = DependencyProperty.Register("Red", typeof(byte), typeof(ColorDialog), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorComponentChanged)));
            GreenProperty = DependencyProperty.Register("Green", typeof(byte), typeof(ColorDialog), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorComponentChanged)));
            BlueProperty = DependencyProperty.Register("Blue", typeof(byte), typeof(ColorDialog), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorComponentChanged)));
            AlphaProperty = DependencyProperty.Register("Alpha", typeof(byte), typeof(ColorDialog), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorComponentChanged)));
       }

        public ColorDialog()
        {
            InitializeComponent();
            this.SelectionColors.BackColor = Colors.White;
            this.SelectionColors.ForeColor = Colors.Transparent;
        }

        private void SelectionColors_ForegroundRectClick(object sender, MouseButtonEventArgs e)
        {
            this.InputColor = false;
            this.CurrentColor = ColorSide.Fore;
            this.SelectedColor = this.SelectionColors.ForeColor;
        }

        private void SelectionColors_BackgroundRectClick(object sender, MouseButtonEventArgs e)
        {
            this.InputColor = false;
            this.CurrentColor = ColorSide.Back;
            this.SelectedColor = this.SelectionColors.BackColor;
        }

        private void colorDialog_Deactivated(object sender, EventArgs e)
        {
            ColorDialog.IsCreated = false;
            this.Hide();
        }

        private void colorDialog_Activated(object sender, EventArgs e)
        {
            this.InputColor = false;
            ColorDialog.IsCreated = true;
            this.Visibility = System.Windows.Visibility.Visible;
            switch (CurrentColor)
            {
                case ColorSide.Back:
                    SelectedColor = SelectionColors.BackColor;
                    break;
                case ColorSide.Fore:
                    SelectedColor = SelectionColors.ForeColor;
                    break;
            }
        }

        private void colorDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ColorDialog.IsCreated = true;
        }

        private void colorDialog_Closed(object sender, EventArgs e)
        {
            ColorDialog.IsCreated = false;
        }

        private void OnRectangleClick(object sender, MouseButtonEventArgs e)
        {
            this.InputColor = false;
            this.SelectedColor = ((sender as Rectangle).Fill as SolidColorBrush).Color;
        }

        private static void OnColorComponentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorDialog colorDialog = sender as ColorDialog;
            Color modifiableColor = colorDialog.SelectedColor;
            if (e.Property == RedProperty)
                modifiableColor.R = (byte)e.NewValue;
            else if (e.Property == GreenProperty)
                modifiableColor.G = (byte)e.NewValue;
            else if (e.Property == BlueProperty)
                modifiableColor.B = (byte)e.NewValue;
            else if (e.Property == AlphaProperty)
                modifiableColor.A = (byte)e.NewValue;
            colorDialog.SelectedColor = modifiableColor;
            if (!colorDialog.InputColor)
            {
                string colorString = colorDialog.SelectedColor.ToString(System.Globalization.CultureInfo.CurrentUICulture).Remove(0, 1);
                colorDialog.NumberColorPresentation.Text = colorString;
            }
        }
        
        private static void OnSelectedColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorDialog colorDialog = sender as ColorDialog;
            Color newColor = (Color)e.NewValue;
            colorDialog.Red = newColor.R;
            colorDialog.Green = newColor.G;
            colorDialog.Blue = newColor.B;
            colorDialog.Alpha = newColor.A;
            switch (colorDialog.CurrentColor)
            {
                case ColorSide.Back :
                    colorDialog.SelectionColors.BackColor = newColor;
                    break;
                case ColorSide.Fore :
                    colorDialog.SelectionColors.ForeColor = newColor;
                    break;
            }
            (colorDialog.Resources["PreviewColorBrush"] as SolidColorBrush).Color = newColor;
        }

        private void SelectionColors_ForeColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (this.CurrentColor == ColorSide.Fore)
                this.SelectedColor = (Color)e.NewValue;
        }

        private void SelectionColors_BackColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (this.CurrentColor == ColorSide.Back)
                this.SelectedColor = (Color)e.NewValue;
        }

        private void NumberColorPresentation_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.F) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right)
                e.Handled = false;
            else
                e.Handled = true;
        }

        public string CreateFullColorString(string partColor)
        {
            string color = "";
            for (int i = 0; i < 8 - partColor.Length; i++)
                color += "F";
            color += partColor;
            return color;
        }

        private void NumberColorPresentation_KeyUp(object sender, KeyEventArgs e)
        {
            this.InputColor = true;
            string color = CreateFullColorString(NumberColorPresentation.Text);
            this.Alpha = Convert.ToByte(color.Substring(0, 2), 16);
            this.Red = Convert.ToByte(color.Substring(2, 2), 16);
            this.Green = Convert.ToByte(color.Substring(4, 2), 16);
            this.Blue = Convert.ToByte(color.Substring(6, 2), 16);
        }

        private void slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.InputColor = false;
        }

    }
}
