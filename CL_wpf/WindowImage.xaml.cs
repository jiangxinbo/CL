using Console_DotNetCore_CaoLiu.Bll;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace CL_wpf
{
    /// <summary>
    /// WindowImage.xaml 的交互逻辑
    /// </summary>
    public partial class WindowImage : Window
    {
        MainWindow mw;
        public WindowImage(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();
            Config.pb = pb;

            
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mw.isshowpb.IsChecked = false;
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
