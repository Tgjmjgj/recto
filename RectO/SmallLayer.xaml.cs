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
    /// <summary>
    /// Логика взаимодействия для SmallLayer.xaml
    /// </summary>
    public partial class SmallLayer : UserControl
    {
        public static DependencyProperty LayerVisibleProperty;
        public bool LayerVisible
        {
            get { return (bool)GetValue(LayerVisibleProperty); }
            set { SetValue(LayerVisibleProperty, value); }
        }

        public event MouseButtonEventHandler ChangeVisibleClick;
        public void OnChangeVisibleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.LayerVisible = !this.LayerVisible;
                if (ChangeVisibleClick != null)
                    ChangeVisibleClick(sender, e);
            }
        }

        public event MouseButtonEventHandler UpLayerButtonClick;
        private void OnUpLayerButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            if (UpLayerButtonClick != null)
                    UpLayerButtonClick(sender, e);
        }

        public event MouseButtonEventHandler DownLayerButtonClick;
        private void OnDownLayerButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (DownLayerButtonClick != null)
                    DownLayerButtonClick(sender, e);
        }

        public SmallLayer()
        {
            InitializeComponent();
            this.LayerVisible = true;
        }

        public SmallLayer(string name)
        {
            InitializeComponent();
            this.layerName_Label.Content = name;
            this.LayerVisible = true;
        }

        static SmallLayer()
        {
            LayerVisibleProperty = DependencyProperty.Register("LayerVisible", typeof(bool), typeof(SmallLayer));
        }

    }
}
