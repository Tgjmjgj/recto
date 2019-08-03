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
    public partial class Recto : UserControl
    {
        public static DependencyProperty IsFileOpenProperty;

        private Brush fillRecto;
        public Brush FillRecto
        {
            get { return fillRecto; }
            set { fillRecto = value; }
        }

        public bool IsFileOpen
        {
            get { return (bool)GetValue(IsFileOpenProperty); }
            set { SetValue(IsFileOpenProperty, value); }
        }

        static Recto()
        {
            IsFileOpenProperty = DependencyProperty.Register("IsFileOpen", typeof(bool), typeof(Recto));
        }

        public Recto()
        {
            InitializeComponent();
        }

        public Recto(Brush entry, Brush final)
        {
            InitializeComponent();
            this.Resources["imgBrush"] = entry;
            this.FillRecto = final;
        }
    }
}
