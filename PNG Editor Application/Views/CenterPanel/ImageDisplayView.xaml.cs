using PNG_Editor_Application.ViewModels.CenterPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PNG_Editor_Application.Views.CenterPanel
{
    /// <summary>
    /// Interaction logic for ImageDisplayView.xaml
    /// </summary>
    public partial class ImageDisplayView : UserControl
    {
        public ImageDisplayView()
        {
            InitializeComponent();
            DataContext = new ImageDisplayViewModel();
        }
    }
}
