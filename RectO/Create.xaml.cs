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
    /// Логика взаимодействия для Create.xaml
    /// </summary>
    public partial class Create : Window
    {
        public Create()
        {
            InitializeComponent();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (this.Owner as MainWindow).imageCanvas.AddFirstLayer(FileManagementController.CreateImage(Convert.ToInt32(_TextBox_width_dock_1.Text), Convert.ToInt32(_TextBox_height_dock_2.Text)));
                int width = Convert.ToInt32(_TextBox_width_dock_1.Text), height = Convert.ToInt32(_TextBox_height_dock_2.Text);
                (this.Owner as MainWindow).borderCanvas.Width = width;
                (this.Owner as MainWindow).borderCanvas.Height = height;
                (this.Owner as MainWindow).imageBorder.Visibility = System.Windows.Visibility.Visible;
                (this.Owner as MainWindow).imageCanvas.OpenfileName = "Новый документ";
                this.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка при создании файла", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void _TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
                e.Handled = true;
        }
    }
}