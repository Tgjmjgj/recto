using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace RectO
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow _mainWnd;

        App()
        {
            WindowPre _preWnd = new WindowPre();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2000);
            timer.Tick += new EventHandler(delegate(object o, EventArgs e)
                {
                    _mainWnd = new MainWindow();
                    this.MainWindow = _mainWnd;
                    _preWnd.Close();
                    timer.Stop();
                });
            timer.Start();
        }

    }
}
