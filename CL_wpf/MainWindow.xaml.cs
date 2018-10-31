using CL.Bll;
using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Tool;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CL_wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            file_path_text.Text= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CL");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                for(int i=1;i<100000;i++)
                {
                    if (i % 10 == 0)

                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\1.jpeg"));
                        }));

                    if (i % 10 == 1)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\2.png"));
                        }));
                    if (i % 10 == 2)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                        image.Source = new BitmapImage(new Uri(@"F:\3.jpeg"));
                        }));
                    if (i % 10 == 3)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\4.png"));
                        }));
                    if (i % 10 == 4)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\5.jpg"));
            }));
            if (i % 10 == 5)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\6.jpg"));
            }));
            if (i % 10 == 6)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\7.jpg"));
            }));
            if (i % 10 == 7)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\8.jpeg"));
            }));
            if (i % 10 == 8)
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\9.jpg"));
            }));
            if (i % 10 == 9)
                    {
                        image.Dispatcher.Invoke(new Action(() =>
                        {
                            image.Source = new BitmapImage(new Uri(@"F:\0.gif"));

                        }));
                       
                    }
                        
                    

        }


                url_text.Dispatcher.Invoke(new Action(() => {
                    var url = url_text.Text;
                    Config.Url = url;
                }));
               
                var info = new Http_Client().get(Config.Url);
                if (info == null)
                {
                    lb.Items.Add("网址错误");
                }
                var typeid = (combobox_type.SelectedItem as ComboBoxItem).Tag.ToString();
                Config.TypeId = int.Parse(typeid);
                lb.Items.Add("您选择了 " + combobox_type.Text);


                int maxPage = Http.getTotalPage(Config.TypeId);
                Config.End_numb = maxPage;
                end_text.Text = maxPage.ToString();

                var start_str = start_text.Text.ToString();
                Config.Start_numb = int.Parse(start_str);

                var task_count_str = task_count_text.Text.ToString();
                Config.Task_count = int.Parse(task_count_str);

                var filepath = file_path_text.Text.ToString();
                Config.Img_path = filepath;


                lb.Items.Clear();
                lb.Items.Add("正在初始化...");
                Http.init();
                Thread.Sleep(2000);
                Config.WebSleep = 1500;
                lb.Items.Clear();
                lb.Items.Add("初始化完成");
                for (int pageint = Config.Start_numb; pageint <= Config.End_numb; pageint++)
                {
                    lb.Items.Clear();
                    lb.Items.Add(string.Format("当前正在处理第{0}页数据", pageint));
                    new PageList().AnalysisPage(pageint);
                }
                lb.Items.Add("----------------------------------------------------------------");
                lb.Items.Add("----------------------------------------------------------------");
                lb.Items.Add("----------------------全部已完成!-------------------------------");
                lb.Items.Add("----------------------------------------------------------------");

            });
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();  //选择文件夹


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                file_path_text.Text = openFileDialog.SelectedPath;
            }

        }
    }
}
