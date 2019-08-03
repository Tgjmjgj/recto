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
using System.Windows.Shapes;

namespace RectO
{
    /// <summary>
    /// Логика взаимодействия для LayerControl.xaml
    /// </summary>
    public partial class LayerControl : Window
    {
        private ImageCanvas image;

        public static bool IsCreated = false;

        public LayerControl()
        {
            LayerControl.IsCreated = true;
            image = null;
            InitializeComponent();
        }

        public LayerControl(ImageCanvas imgCanvas)
        {
            LayerControl.IsCreated = true;
            InitializeComponent();
            this.image = imgCanvas;
            UpdateLayersControl();
        }

        public void Add_Layer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (image != null && image.OpenfileName != "")
            {
                image.AddNewEmptyLayer(image.ActualHeight, image.ActualWidth);
                var newLayerIco = new SmallLayer(image.layers[this.image.layers.Count - 1].LayerName);
                newLayerIco.ChangeVisibleClick += VisibilityControl_Click;
                newLayerIco.UpLayerButtonClick += LayerUp_Click;
                newLayerIco.DownLayerButtonClick += LayerDown_Click;
                newLayerIco.layerSmallImg.Visual = image.layers[this.image.layers.Count - 1];
                this.layersList.Items.Add(newLayerIco);
                this.layersList.SelectedIndex = this.layersList.Items.Count - 1;
            }
        }

        public void Remove_Layer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (image != null && this.layersList.SelectedIndex != -1 && image.OpenfileName != "")
            {
                int index = this.layersList.SelectedIndex;
                try
                {
                    if (image.layers.Count == 1)
                        throw new Exception();
                    this.layersList.Items.RemoveAt(index);
                    this.image.RemoveLayerAt(index);
                    this.layersList.SelectedIndex = this.image.currentLayerIndex;
                }
                catch
                {
                    MessageBox.Show("В изображении должен присутствовать хотя бы один слой!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public void UpdateLayersControl()
        {
            if (image != null)
            {
                this.layersList.Items.Clear();
                for (int i = 0; i < this.image.layers.Count; i++)
                {
                    var newLayerIco = new SmallLayer(this.image.layers[i].LayerName);
                    newLayerIco.ChangeVisibleClick += VisibilityControl_Click;
                    newLayerIco.UpLayerButtonClick += LayerUp_Click;
                    newLayerIco.DownLayerButtonClick += LayerDown_Click;
                    newLayerIco.layerSmallImg.Visual = image.layers[i];
                    this.layersList.Items.Add(newLayerIco);
                    if (image.layers[this.layersList.Items.Count - 1].Visibility == System.Windows.Visibility.Hidden)
                        newLayerIco.OnChangeVisibleClick(newLayerIco.layerState, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
                }
                this.layersList.SelectedIndex = image.currentLayerIndex;
            }
        }


        private void layersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.layersList.SelectedIndex != -1)
            {
                image.currentLayerIndex = this.layersList.SelectedIndex;
                foreach (Layer lay in image.layers)
                    lay.IsSelected = false;
                image.layers[image.currentLayerIndex].IsSelected = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void VisibilityControl_Click(object sender, MouseButtonEventArgs e)
        {
            SmallLayer layer = ((((sender as Rectangle).Parent as DockPanel).Parent as Border).Parent as Grid).Parent as SmallLayer;//-___-
            int layIndex = this.layersList.Items.IndexOf(layer);
            if (layer.LayerVisible)
                image.layers[layIndex].Visibility = System.Windows.Visibility.Visible;
            else
                image.layers[layIndex].Visibility = System.Windows.Visibility.Hidden;
        }

        private void LayerUp_Click(object sender, MouseButtonEventArgs e)
        {
            SmallLayer smLayer = ((((((sender as Rectangle).Parent as Grid).Parent as Grid).Parent as DockPanel).Parent as Border).Parent as Grid).Parent as SmallLayer; //  -________-
            int layIndex = this.layersList.Items.IndexOf(smLayer);
            Layer layer = image.layers[layIndex];
            if (layIndex != 0)
            {
                int swapZ = Convert.ToInt32(layer.GetValue(Panel.ZIndexProperty));
                int oldZ = Convert.ToInt32(image.layers[layIndex - 1].GetValue(Panel.ZIndexProperty));
                image.layers[layIndex].SetValue(Panel.ZIndexProperty, oldZ);
                image.layers[layIndex - 1].SetValue(Panel.ZIndexProperty, swapZ);
                image.layers.Sort((Layer l1, Layer l2) => Convert.ToInt32(l1.GetValue(Panel.ZIndexProperty)) - Convert.ToInt32(l2.GetValue(Panel.ZIndexProperty)));
                image.Children.Clear();
                foreach (Layer lay in image.layers)
                    image.Children.Add(lay);
                UpdateLayersControl();
            }
        }

        private void LayerDown_Click(object sender, MouseButtonEventArgs e)
        {
            SmallLayer smLayer = ((((((sender as Rectangle).Parent as Grid).Parent as Grid).Parent as DockPanel).Parent as Border).Parent as Grid).Parent as SmallLayer; //  -________-
            int layIndex = this.layersList.Items.IndexOf(smLayer);
            Layer layer = image.layers[layIndex];
            if (layIndex != image.layers.Count - 1)
            {
                int swapZ = Convert.ToInt32(layer.GetValue(Panel.ZIndexProperty));
                int oldZ = Convert.ToInt32(image.layers[layIndex + 1].GetValue(Panel.ZIndexProperty));
                image.layers[layIndex].SetValue(Panel.ZIndexProperty, oldZ);
                image.layers[layIndex + 1].SetValue(Panel.ZIndexProperty, swapZ);
                image.layers.Sort((Layer l1, Layer l2) => Convert.ToInt32(l1.GetValue(Panel.ZIndexProperty)) - Convert.ToInt32(l2.GetValue(Panel.ZIndexProperty)));
                image.Children.Clear();
                foreach (Layer lay in image.layers)
                    image.Children.Add(lay);
                UpdateLayersControl();
            }
        }
    }
}
