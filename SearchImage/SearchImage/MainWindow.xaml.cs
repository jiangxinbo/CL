using SearchImage.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearchImage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<FileInfo> lst = new List<FileInfo>();
            List<P> plist = new List<P>();
            DirectoryInfo fdir = new DirectoryInfo(@"a:/test/");
            FileInfo[] file = fdir.GetFiles();

            if (file.Length != 0 ) //当前目录文件不为空                   
            {
                foreach (FileInfo f in file) //显示当前目录所有文件   
                {
                    lst.Add(f);
                }
            }

            for(int i=0;i<lst.Count;i++)
            {
                for(int j=i+1;j<lst.Count;j++)
                {
                    var url1 = lst[i].FullName;
                    var url2 = lst[j].FullName;
                    var val=xiangdeng(url1, url2);
                    P p = new P();
                    p.info1 = lst[i];
                    p.info2 = lst[j];
                    p.v = val;
                    plist.Add(p);
                    if (val < 0.7) continue;
                    //生成图片显示结果
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(p.info1.FullName, UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    bi.Freeze();

                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = bi;
                    image.Width = 100;
                    image.Height = 100;
                    //lb1.Items.Add(image);

                    Label l = new Label();
                    l.Height = 50;
                    l.Width = 200;
                    l.Content = p.v;
                    l.HorizontalAlignment = HorizontalAlignment.Left;
                    l.VerticalAlignment = VerticalAlignment.Top;


                    BitmapImage bi2 = new BitmapImage();
                    bi2.BeginInit();
                    bi2.UriSource = new Uri(p.info2.FullName, UriKind.RelativeOrAbsolute);
                    bi2.EndInit();
                    bi2.Freeze();

                    System.Windows.Controls.Image image2 = new System.Windows.Controls.Image();
                    image2.Source = bi2;
                    image2.Width = 100;
                    image2.Height = 100;


                    //lb1.Items.Add(image2);

                    StackPanel sp1 = new StackPanel();
                    sp1.Height = 100;
                    sp1.Orientation = Orientation.Horizontal;
                    sp1.Children.Add(l);
                    sp1.Children.Add(image);
                    sp1.Children.Add(image2);

                    lb1.Items.Add(sp1);



                }
            }


           


        }


        private float xiangdeng(string url,string url2)
        {
            Photo1 p = new Photo1();
            var img1=p.Resize(url, @"url.jpg");            
            var intlist1 = p.GetHisogram(img1);
            var img2 = p.Resize(url2, @"url2.jpg");
            var intlist2 = p.GetHisogram(img2);
            return p.GetResult(intlist1, intlist2);
            //tb2.Text +="\r\n"+ p.GetResult(intlist1, intlist2).ToString()+"  - "+url+"  _  "+url2;
        }


    }
}
