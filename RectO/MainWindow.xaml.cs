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
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

namespace RectO
{
    public partial class MainWindow : Window
    {
        PrimitiveDrawingDirect drawingDirect;
        private PrimitiveDrawingDirect.Draw oldDraw = null;

        delegate void OnOpenfileNameChanged(object sender, EventArgs e);
        event OnOpenfileNameChanged OpenfileNameChanged;
        List<LayerCanvas> OpenImages = new List<LayerCanvas>();

        DragBorder dragArea = DragBorder.None;
        int dsize = 2;
        int colorDialogBottomIndent = 40;

        ColorDialog colorDialog = new ColorDialog();
        LayerControl layerDialog;

        public MainWindow()
        {
            InitializeComponent();
            drawingDirect = new PrimitiveDrawingDirect(imageCanvas);
            OpenfileNameChanged += Window_OpenfileNameChanged;
            imageCanvas.MouseDown += drawingDirect.DrawOnMouseDown;
            imageCanvas.MouseUp += drawingDirect.DrawOnMouseUp;
            imageCanvas.MouseMove += drawingDirect.DrawOnMouseMove;
            CreateBindings();
        }

        private void CreateBindings()
        {
            KeyBinding Ctrl_Shift_S_binding = new KeyBinding(ApplicationCommands.SaveAs, Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            this.InputBindings.Add(Ctrl_Shift_S_binding);
            Binding foreColorBinding = new Binding();
            foreColorBinding.Source = colorControl;
            foreColorBinding.Path = new PropertyPath("ForeColor");
            foreColorBinding.Mode = BindingMode.TwoWay;
            colorDialog.SelectionColors.SetBinding(ColorControl.ForeColorProperty, foreColorBinding);
            Binding backColorBinding = new Binding();
            backColorBinding.Source = colorControl;
            backColorBinding.Path = new PropertyPath("BackColor");
            backColorBinding.Mode = BindingMode.TwoWay;
            colorDialog.SelectionColors.SetBinding(ColorControl.BackColorProperty, backColorBinding);
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.imageCanvas == null)
                e.CanExecute = false;
            else if (this.imageCanvas.OpenfileName == "" || this.imageCanvas.OpenfileName == null)
                e.CanExecute = false;
            else if (this.imageCanvas.OpenfileName.IndexOf(':') == -1 || this.imageCanvas.lastChangeSaved)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }

        private void IsFileOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.imageCanvas.OpenfileName == "" || this.imageCanvas.OpenfileName == null)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }

        private void CanBack(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.imageCanvas == null)
                e.CanExecute = false;
            else if (this.imageCanvas.dataController != null)
                e.CanExecute = this.imageCanvas.dataController.CanBack();
        }

        private void CanForward(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.imageCanvas == null)
                e.CanExecute = false;
            else if (this.imageCanvas.dataController != null)
                e.CanExecute = this.imageCanvas.dataController.CanForward();
        }

        private void Window_OpenfileNameChanged(object sender, EventArgs e)
        {
            this.Title = "Recto  -  " + this.imageCanvas.OpenfileName.Remove(0, this.imageCanvas.OpenfileName.LastIndexOf('\\') + 1);
            this.Recto1.IsFileOpen = this.imageCanvas.OpenfileName != "";
            string fileRes = this.imageCanvas.OpenfileName.Remove(0, this.imageCanvas.OpenfileName.LastIndexOf('.') + 1);
            if (fileRes == this.imageCanvas.OpenfileName)
                this.imageCanvas.openfileRes = ImageResolutions.Undefined;
            else if (fileRes == "bmp")
                this.imageCanvas.openfileRes = ImageResolutions.BMP;
            else if (fileRes == "gif")
                this.imageCanvas.openfileRes = ImageResolutions.GIF;
            else if (fileRes == "jpg" || fileRes == "jpeg" || fileRes == "jpe" || fileRes == "jfif")
                this.imageCanvas.openfileRes = ImageResolutions.JPG;
            else if (fileRes == "png")
                this.imageCanvas.openfileRes = ImageResolutions.PNG;
            else if (fileRes == "tif" || fileRes == "tiff")
                this.imageCanvas.openfileRes = ImageResolutions.TIF;
            else if (fileRes == "wdp" || fileRes == "wmp" || fileRes == "hdp")
                this.imageCanvas.openfileRes = ImageResolutions.WDP;
            else
                this.imageCanvas.openfileRes = ImageResolutions.Undefined;
        }

        private void MenuItem_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.imageCanvas.lastChangeSaved)
            {
                MessageBoxResult res = MessageBox.Show("Сохранить текущее изображение?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (res == MessageBoxResult.Yes)
                    if (ApplicationCommands.Save.CanExecute(null, this))
                        ApplicationCommands.Save.Execute(null, this);
                    else
                        ApplicationCommands.SaveAs.Execute(null, this);
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }
            Create _CreateWnd = new Create();
            _CreateWnd.Owner = this;
            _CreateWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _CreateWnd.ShowDialog();
            this.OpenfileNameChanged(this, new EventArgs());
            if (LayerControl.IsCreated)
                layerDialog.UpdateLayersControl();
        }

        private void MenuItem_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.imageCanvas.lastChangeSaved)
            {
                MessageBoxResult res = MessageBox.Show("Сохранить текущее изображение?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (res == MessageBoxResult.Yes)
                    if (ApplicationCommands.Save.CanExecute(null, this))
                        ApplicationCommands.Save.Execute(null, this);
                    else
                        ApplicationCommands.SaveAs.Execute(null, this);
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }
            OpenFileDialog _OpenFile_dlg = new OpenFileDialog();
            _OpenFile_dlg.AddExtension = true;
            _OpenFile_dlg.Filter = "Все типы изображений (*.bmp, *.gif, *.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.tif, *.tiff, *.wdp, *wmp, *hdp, *.tga, *.dds)|*.bmp;*.gif;*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.tif;*.tiff;*wdp;*wmp;*hdp;*.tga;*.dds";
            _OpenFile_dlg.Filter += "|BMP (*.bmp)|*.bmp";
            _OpenFile_dlg.Filter += "|GIF (*.gif)|*.gif";
            _OpenFile_dlg.Filter += "|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif";
            _OpenFile_dlg.Filter += "|PNG (*.png)|*.png";
            _OpenFile_dlg.Filter += "|TIFF (*.tif, *.tiff)|*.tif;*.tiff";
            _OpenFile_dlg.Filter += "|WMPhoto (*.wdp, *.wmp, *.hdp)|*.wdp;*.wmp;*.hdp";
            _OpenFile_dlg.Filter += "|TGA (*.tga)|*.tga";
            _OpenFile_dlg.Filter += "|DirectDraw Surface (DDS) (*.dds)|*.dds";
            _OpenFile_dlg.Multiselect = false;
            _OpenFile_dlg.Title="Открыть";
            if (_OpenFile_dlg.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bmp = FileManagementController.OpenImage(_OpenFile_dlg.FileName);
                    this.imageCanvas.Clear();
                    borderCanvas.Width = bmp.Width;
                    borderCanvas.Height = bmp.Height;
                    this.imageCanvas.AddFirstLayer(bmp);
                    this.imageCanvas.OpenfileName = _OpenFile_dlg.FileName;
                    OpenfileNameChanged(this, new EventArgs());
                    imageBorder.Visibility = System.Windows.Visibility.Visible;
                    this.imageCanvas.lastChangeSaved = true;
                    if (LayerControl.IsCreated)
                        layerDialog.UpdateLayersControl();
                }
                catch (FileFormatException excp)
                {
                    MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("Невозможно открыть изображение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.UpdateLayout();
                }
            }
        }

        private void MenuItem_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                FileManagementController.SaveImageRender(imageCanvas, this.imageCanvas.OpenfileName, this.imageCanvas.openfileRes);
                this.imageCanvas.lastChangeSaved = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно сохранить изображение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog _SaveAsFile_dlg = new SaveFileDialog();
            _SaveAsFile_dlg.AddExtension = true;
            _SaveAsFile_dlg.OverwritePrompt = true;
            _SaveAsFile_dlg.FileName = "Новое изображение";
            _SaveAsFile_dlg.Filter += "BMP (*.bmp)|*.bmp";
            _SaveAsFile_dlg.Filter += "|GIF (*.gif)|*.gif";
            _SaveAsFile_dlg.Filter += "|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif";
            _SaveAsFile_dlg.Filter += "|PNG (*.png)|*.png";
            _SaveAsFile_dlg.Filter += "|TIFF (*.tif, *.tiff)|*.tif;*.tiff";
            _SaveAsFile_dlg.Filter += "|WMPhoto (*.wdp, *.wmp, *.hdp)|*.wdp;*.wmp;*.hdp";
            _SaveAsFile_dlg.Filter += "|TGA (*.tga)|*.tga";
            _SaveAsFile_dlg.Filter += "|DirectDraw Surface (DDS) (*.dds)|*.dds";
            _SaveAsFile_dlg.Title = "Сохранить как";
            if (_SaveAsFile_dlg.ShowDialog() == true)
            {
                try
                {
                    FileManagementController.SaveImageRender(imageCanvas, _SaveAsFile_dlg.FileName, this.imageCanvas.openfileRes);
                    this.imageCanvas.OpenfileName = _SaveAsFile_dlg.FileName;
                    OpenfileNameChanged(this, new EventArgs());
                    this.imageCanvas.lastChangeSaved = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Невозможно сохранить изображение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuItem_Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog _Print_dlg = new PrintDialog();
            _Print_dlg.UserPageRangeEnabled = true;
            _Print_dlg.PageRangeSelection = PageRangeSelection.AllPages;
            _Print_dlg.ShowDialog();
        }

        private void MenuItem_Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.imageCanvas.lastChangeSaved)
            {
                MessageBoxResult res = MessageBox.Show("Сохранить изображение перед закрытием?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (res == MessageBoxResult.Yes)
                    if (ApplicationCommands.Save.CanExecute(null, this))
                        ApplicationCommands.Save.Execute(null, this);
                    else
                        ApplicationCommands.SaveAs.Execute(null, this);
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }
            this.imageCanvas.Clear();
            this.imageCanvas.OpenfileName = "";
            if (LayerControl.IsCreated)
                layerDialog.UpdateLayersControl();
            OpenfileNameChanged(this, new EventArgs());
            this.imageCanvas.lastChangeSaved = true;
            imageBorder.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void MenuItem_Filedata_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(this.imageCanvas.OpenfileName, this.imageCanvas.OpenfileName, MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.imageCanvas.OpenfileName != "" && !this.imageCanvas.lastChangeSaved)
            {
                MessageBoxResult res = MessageBox.Show("Сохранить изображение перед закрытием?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation, MessageBoxResult.Yes);
                if (res == MessageBoxResult.Yes)
                    if (ApplicationCommands.Save.CanExecute(null, this))
                        ApplicationCommands.Save.Execute(null, this);
                    else
                        ApplicationCommands.SaveAs.Execute(null, this);
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }
            this.Close();
        }

        private void MenuItem_Back_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.imageCanvas.layers[this.imageCanvas.currentLayerIndex].image.GetVisualCount() != 0)
            {
                this.imageCanvas.layers[this.imageCanvas.currentLayerIndex].image.ClearVisual();
                if (this.drawingDirect.currentDrawing != null)
                    this.drawingDirect.currentDrawing.ResetPoints();
            }
            else
            {
                var layers = new List<LayerInfo>();
                foreach (Layer lay in this.imageCanvas.layers)
                    layers.Add(new LayerInfo(lay.image.image.ImageSource, lay.LayerName));
                List<LayerInfo> img = this.imageCanvas.dataController.GoBack(layers);
                if (img != null)
                {
                    this.imageCanvas.Children.Clear();
                    this.imageCanvas.layers.Clear();
                    foreach (LayerInfo layer in img)
                        this.imageCanvas.AddNewLayer(layer.Image, layer.Name);
                    this.imageCanvas.Width = borderCanvas.Width = img[0].Image.Width;
                    this.imageCanvas.Height = borderCanvas.Height = img[0].Image.Height;
                }
                if (LayerControl.IsCreated)
                    layerDialog.UpdateLayersControl();
            }
        }

        private void MenuItem_Forward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var layers = new List<LayerInfo>();
            foreach (Layer lay in this.imageCanvas.layers)
                layers.Add(new LayerInfo(lay.image.image.ImageSource, lay.LayerName));
            List<LayerInfo> img = this.imageCanvas.dataController.GoForward(layers);
            if (img != null)
            {
                this.imageCanvas.Width = borderCanvas.Width = img[0].Image.Width;
                this.imageCanvas.Height = borderCanvas.Height = img[0].Image.Height;
                this.imageCanvas.Children.Clear();
                this.imageCanvas.layers.Clear();
                foreach (LayerInfo layer in img)
                    this.imageCanvas.AddNewLayer(layer.Image, layer.Name);
            }
            if (LayerControl.IsCreated)
                layerDialog.UpdateLayersControl();
        }
        
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            Point pnt = e.GetPosition((Canvas)sender);
            _TextBlock_Mouse_coord_info1.Text = "X,Y: (" + pnt.X + ',' + pnt.Y + ')';

        }

        private void TopCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.imageCanvas.OpenfileName != "")
            {
                Point pnt = e.GetPosition(sender as Canvas);
                if (pnt.X > 0 && pnt.X < imageBorder.ActualWidth && pnt.Y > imageBorder.ActualHeight - imageBorder.BorderThickness.Bottom - dsize && pnt.Y <= imageBorder.ActualHeight + dsize)
                {
                    oldDraw = drawingDirect.currentDrawing;
                    drawingDirect.currentDrawing = null;
                    if (pnt.X > imageBorder.ActualWidth - 20)
                        dragArea = DragBorder.XY;
                    else
                        dragArea = DragBorder.Y;
                }
                else if (pnt.Y > 0 && pnt.Y < imageBorder.ActualHeight && pnt.X > imageBorder.ActualWidth - imageBorder.BorderThickness.Right - dsize && pnt.X <= imageBorder.ActualWidth + dsize)
                {
                    oldDraw = drawingDirect.currentDrawing;
                    drawingDirect.currentDrawing = null;
                    if (pnt.Y > imageBorder.ActualHeight - 20)
                        dragArea = DragBorder.XY;
                    else
                        dragArea = DragBorder.X;
                }
                e.Handled = true;
            }
        }

        private void TopCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.imageCanvas.OpenfileName != "")
            {
                Canvas canv = sender as Canvas;
                Point pnt = e.GetPosition(canv);
                if (this.dragArea != DragBorder.None)
                {
                    if (this.dragArea != DragBorder.None)
                        this.imageCanvas.layers[this.imageCanvas.currentLayerIndex].image.ClearVisual();
                    switch (this.dragArea)
                    {
                        case DragBorder.X:
                            borderCanvas.Width = pnt.X;
                            imageCanvas.Resize(imageCanvas.Height, pnt.X);
                            break;
                        case DragBorder.Y:
                            borderCanvas.Height = pnt.Y;
                            imageCanvas.Resize(pnt.Y, imageCanvas.Width);
                            break;
                        case DragBorder.XY:
                            borderCanvas.Width = pnt.X;
                            borderCanvas.Height = pnt.Y;
                            imageCanvas.Resize(pnt.Y, pnt.X);
                            break;
                    }
                    this.imageCanvas.lastChangeSaved = false;
                }
                if ((pnt.X > imageBorder.ActualWidth || pnt.Y > imageBorder.ActualHeight) && this.drawingDirect.currentDrawing != null)
                {
                    if (this.drawingDirect.currentDrawing.GetType() == typeof(PrimitiveDrawingDirect.DrawBrush) || this.drawingDirect.currentDrawing.GetType() == typeof(PrimitiveDrawingDirect.DrawPen))
                        this.drawingDirect.currentDrawing.ResetPoints();
                }
                if (pnt.X > 0 && pnt.X < imageBorder.ActualWidth && pnt.Y > imageBorder.ActualHeight - imageBorder.BorderThickness.Bottom - dsize && pnt.Y <= imageBorder.ActualHeight + dsize)
                    if (pnt.X > imageBorder.ActualWidth - 20)
                        Mouse.SetCursor(Cursors.SizeNWSE);
                    else
                        Mouse.SetCursor(Cursors.SizeNS);
                else if (pnt.Y > 0 && pnt.Y < imageBorder.ActualHeight && pnt.X > imageBorder.ActualWidth - imageBorder.BorderThickness.Right - dsize && pnt.X <= imageBorder.ActualWidth + dsize)
                    if (pnt.Y > imageBorder.ActualHeight - 20)
                        Mouse.SetCursor(Cursors.SizeNWSE);
                    else
                        Mouse.SetCursor(Cursors.SizeWE);
            }
        } 

        private void TopCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.imageCanvas.OpenfileName != "")
                dragArea = DragBorder.None;
            if (this.drawingDirect.currentDrawing == null && oldDraw != null)
            {
                drawingDirect.SetDrawing(oldDraw);
                oldDraw = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TopCanvas.MinHeight = this.TopCanvas.ActualHeight;
            this.TopCanvas.MinWidth = this.TopCanvas.ActualWidth;
            this.TopCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.TopCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }

        private void Scroll_Update()
        {
            if (this.imageCanvas.OpenfileName != "")
            {
                Scroll.Visibility = System.Windows.Visibility.Visible;
                if (drawingAreaCell.ActualHeight - (imageBorder.BorderThickness.Top + imageBorder.BorderThickness.Bottom) > imageCanvas.Height + 150)
                {
                    TopCanvas.Height = TopCanvas.ActualHeight;
                    Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
                else
                {
                    TopCanvas.Height = imageCanvas.Height + 150;
                    Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                if (drawingAreaCell.ActualWidth - (imageBorder.BorderThickness.Left + imageBorder.BorderThickness.Right) > imageCanvas.Width + 200)
                {
                    TopCanvas.Width = TopCanvas.ActualWidth;
                    Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
                else
                {
                    TopCanvas.Width = imageCanvas.Width + 200;
                    Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
            }
            else
            {
                Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }

        private void EmpiringColorControl(bool increases)
        {
            try
            {
                ColorControl col = this.colorControl;
                int indexOfColorControl = this.Instrument_ToolBar.Items.IndexOf(col);
                double currentHeight = this.Instrument_ToolBar.PointToScreen(new Point(0, this.Instrument_ToolBar.ActualHeight)).Y;
                for (int i = 0; i < this.Instrument_ToolBar.Items.Count && currentHeight > this.Instrument_ToolBar.PointToScreen(new Point(0, 27)).Y; i++)
                    if (this.Instrument_ToolBar.Items[i].GetType() == typeof(RadioButton))
                    {
                        RadioButton rBut = this.Instrument_ToolBar.Items[i] as RadioButton;
                        double thisButHeight = rBut.PointToScreen(new Point(0, 0)).Y;
                        if (thisButHeight + 2 * rBut.ActualHeight > currentHeight - 14 && thisButHeight + col.ActualHeight < currentHeight - 14)
                        {
                            if (col.Margin.Top != 0)
                                col.Margin = new Thickness(0);
                            if (increases)
                            {
                                RadioButton buff = new RadioButton();
                                buff.Height = buff.Width = 29;
                                this.Instrument_ToolBar.Items[i] = buff;
                                Instrument_ToolBar.Items[indexOfColorControl] = rBut;
                                Instrument_ToolBar.Items[i] = col;
                                if (i == this.Instrument_ToolBar.Items.Count - 1)
                                {
                                    double screenHeight = this.drawingAreaCell.PointToScreen(new Point(0, this.drawingAreaCell.ActualHeight)).Y;
                                    if (screenHeight - currentHeight < 50)
                                        col.Margin = new Thickness(0, (screenHeight - currentHeight) / 2 + 1, 0, 0);
                                    else
                                        col.Margin = new Thickness(0, 25, 0, 0);
                                }
                            }
                            else
                            {
                                Instrument_ToolBar.Items.Remove(col);
                                Instrument_ToolBar.Items[i] = col;
                                Instrument_ToolBar.Items.Add(rBut);
                            }
                            break;
                        }
                    }
                    else if (this.Instrument_ToolBar.Items[i].GetType() == typeof(ColorControl))
                    {
                        if (increases)
                        {
                            if (i == this.Instrument_ToolBar.Items.Count - 1)
                            {
                                double screenHeight = this.drawingAreaCell.PointToScreen(new Point(0, this.drawingAreaCell.ActualHeight)).Y;
                                if (screenHeight - currentHeight < 50)
                                    col.Margin = new Thickness(0, (screenHeight - currentHeight)/2 + 1, 0, 0);
                                else
                                    col.Margin = new Thickness(0, 25, 0, 0);
                            }
                            if (col.PointToScreen(new Point(0, col.ActualHeight)).Y + 31 > currentHeight)
                                break;
                        }
                        else
                        {
                            if (i != 0)
                            {
                                double screenHeight = this.drawingAreaCell.PointToScreen(new Point(0, this.drawingAreaCell.ActualHeight)).Y;
                                if (currentHeight + col.Margin.Top >= screenHeight)
                                    col.Margin = new Thickness(0, screenHeight - currentHeight, 0, 0);
                            }
                            break;
                        }
                    }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.InvalidateVisual();
            }
        }

        private void SetColorDialogPosition()
        {
            double y, x = this.drawingAreaCell.PointToScreen(new Point(0, 0)).X;
            double limitHeight = this.drawingAreaCell.PointToScreen(new Point(0, this.drawingAreaCell.ActualHeight)).Y;
            if (colorDialog.Height + colorDialogBottomIndent > this.drawingAreaCell.ActualHeight)
                y = limitHeight - this.drawingAreaCell.ActualHeight;
            else
                y = limitHeight - (colorDialog.Height + colorDialogBottomIndent);
            Point colorDialogCoord = new Point(x, y);
            colorDialog.Left = colorDialogCoord.X;
            colorDialog.Top = colorDialogCoord.Y;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Scroll_Update();
            SetColorDialogPosition();
            EmpiringColorControl(e.NewSize.Height > e.PreviousSize.Height);
        }

        private void imageCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Scroll_Update();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.imageCanvas.OpenfileName != "" && !this.imageCanvas.lastChangeSaved)
            {
                MessageBoxResult res = MessageBox.Show("Сохранить изображение перед закрытием?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation, MessageBoxResult.Yes);
                if (res == MessageBoxResult.Yes)
                {
                    if (ApplicationCommands.Save.CanExecute(null, this))
                        ApplicationCommands.Save.Execute(null, this);
                    else
                        ApplicationCommands.SaveAs.Execute(null, this);
                    e.Cancel = false;
                }
                else if (res == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else
                    e.Cancel = false;
            }
            else
                e.Cancel = false;
            colorDialog.Close();
        }

        private void ShowColorDialog()
        {
            if (ColorDialog.IsCreated)
            {
                colorDialog.Activate();
            }
            else
            {
                colorDialog.Show();
                SetColorDialogPosition();
            }
        }

        private void colorControl_ForegroundRectClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                colorDialog.CurrentColor = ColorDialog.ColorSide.Fore;
                ShowColorDialog();
            }
        }

        private void colorControl_BackgroundRectClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                colorDialog.CurrentColor = ColorDialog.ColorSide.Back;
                ShowColorDialog();
            }
        }

        private void Test_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Left = 0;
            double returnHeight = this.Height = this.ActualHeight;
            double returnWidth = this.Width = this.ActualWidth;
            this.WindowState = System.Windows.WindowState.Normal;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(25);
            bool decrease = true;
            double factor = (this.Width - this.MinWidth) / (this.Height - this.MinHeight);
            timer.Tick += new EventHandler((object obj, EventArgs ea) =>
            {
                if (decrease)
                {
                    if (this.Height > this.MinHeight)
                        this.Height--;
                    if (this.Width > this.MinWidth)
                        this.Width -= factor;
                    if (this.Height == this.MinHeight && this.Width == this.MinWidth)
                        decrease = !decrease;
                }
                else
                {
                    if (this.Height < returnHeight)
                        this.Height++;
                    if (this.Width < returnWidth)
                        this.Width += factor;
                }
                if (this.Height == returnHeight && this.Width == returnWidth)
                {
                    this.WindowState = System.Windows.WindowState.Maximized;
                    timer.Stop();
                }
            });
            timer.Start();
        }

        private void colorControl_ForeColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (this.imageCanvas != null)
                this.imageCanvas.pen.Brush = new SolidColorBrush(colorControl.ForeColor);
        }

        private void colorControl_BackColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (this.imageCanvas != null)
                this.imageCanvas.brush = new SolidColorBrush(colorControl.BackColor);
        }

        private void LineType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageCanvas != null)
                switch (this.LineType_ComboBox.SelectedIndex)
                {
                    case 0:
                        {
                            this.imageCanvas.pen.DashCap = PenLineCap.Flat;
                            this.imageCanvas.pen.DashStyle = DashStyles.Solid;
                            this.imageCanvas.pen.EndLineCap = PenLineCap.Flat;
                            this.imageCanvas.pen.StartLineCap = PenLineCap.Flat;
                            break;
                        }
                    case 1:
                        {
                            this.imageCanvas.pen.DashCap = PenLineCap.Flat;
                            this.imageCanvas.pen.DashStyle = DashStyles.Dash;
                            this.imageCanvas.pen.EndLineCap = PenLineCap.Flat;
                            this.imageCanvas.pen.StartLineCap = PenLineCap.Flat;
                            break;
                        }
                    case 2:
                        {
                            this.imageCanvas.pen.DashCap = PenLineCap.Round;
                            double[] dashes = { 0, 2 };
                            this.imageCanvas.pen.DashStyle = new DashStyle(dashes, 0);
                            this.imageCanvas.pen.EndLineCap = PenLineCap.Round;
                            this.imageCanvas.pen.StartLineCap = PenLineCap.Round;
                            break;
                        }
                }
        }

        private void Thickness_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageCanvas != null)
                this.imageCanvas.pen.Thickness = Convert.ToInt32((Thickness_ComboBox.SelectedItem as ComboBoxItem).Content);
        }

        private void ExecuteDrawing(PrimitiveDrawingDirect.Draw newDraw)
        {
            drawingDirect.SetDrawing(newDraw);
        }

        private void Line_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawLine());
        }

        private void Ellipse_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawEllipse());
        }

        private void Recto_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawRect());
        }

        private void Pen_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawPen());
        }

        private void Triangle_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawTriangle());
        }

        private void Segment_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawSegment());
        }

        private void Brush_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawBrush());
        }

        private void Text_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawText());
        }

        private void Eraser_Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDrawing(new PrimitiveDrawingDirect.DrawEraser());
        }

        private void ShowLayersDialog()
        {
            if (!LayerControl.IsCreated)
            {
                layerDialog = new LayerControl(imageCanvas);
                layerDialog.Owner = this;
                layerDialog.Show();
            }
            else
            {
                layerDialog.UpdateLayersControl();
                layerDialog.Show();
            }
        }

        private void ShowLayer_Click(object sender, RoutedEventArgs e)
        {
            ShowLayersDialog();
        }

        private void Add_Layer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!LayerControl.IsCreated)
            {
                layerDialog = new LayerControl(imageCanvas);
                layerDialog.Owner = this;
            }
            layerDialog.Add_Layer_Button_Click(this, null);
        }

        private void Remove_Layer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!LayerControl.IsCreated)
            {
                layerDialog = new LayerControl(imageCanvas);
                layerDialog.Owner = this;
            }
            layerDialog.Remove_Layer_Button_Click(this, null);
        }

    }
}
