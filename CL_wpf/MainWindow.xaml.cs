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
        WindowImage wi;
        public MainWindow()
        {
            wi = new WindowImage(this);
            InitializeComponent();
            file_path_text.Text= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CL");
            if (isshowpb.IsChecked.GetValueOrDefault())
            {
                Config.isShowPB = true;
            }
            else
            {
                Config.isShowPB = false;
            }
            Config.pb = wi.pb;
            Config.lb = lb;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btn1.IsEnabled = false;
            file_btn.IsEnabled = false;
            file_path_text.IsEnabled = false;
            combobox_type.IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                isshowpb.Dispatcher.Invoke(new Action(() =>
                {
                    bool isshow = isshowpb.IsChecked.GetValueOrDefault();
                }));
                url_text.Dispatcher.Invoke(new Action(() =>
                {
                    var url = url_text.Text;
                    Config.Url = url;
                }));

                var info = new Http_Client().get(Config.Url);
                if (info == null)
                {
                    lb.Dispatcher.Invoke(new Action(() =>
                    {
                        lb.Items.Add("网址错误");
                    }));
                }
                combobox_type.Dispatcher.Invoke(new Action(() =>
                {
                    var typeid = (combobox_type.SelectedItem as ComboBoxItem).Tag.ToString();
                    Config.TypeId = int.Parse(typeid);
                    lb.Items.Add("您选择了 " + combobox_type.Text);
                }));

                int maxPage = Http.getTotalPage(Config.TypeId);
                Config.End_numb = maxPage;
                end_text.Dispatcher.Invoke(new Action(() =>
                {
                    end_text.Text = maxPage.ToString();
                    var start_str = start_text.Text.ToString();
                    Config.Start_numb = int.Parse(start_str);
                    var task_count_str = task_count_text.Text.ToString();
                    Config.Task_count = int.Parse(task_count_str);
                    var filepath = file_path_text.Text.ToString();
                    Config.Img_path = filepath;
                    lb.Items.Clear();
                    lb.Items.Add("正在初始化...");
                }));

                Http.init();
                Thread.Sleep(1000);
                Config.WebSleep = 1500;
                lb.Dispatcher.Invoke(new Action(() =>
                {
                    lb.Items.Clear();
                    lb.Items.Add("初始化完成");
                }));
                for (int pageint = Config.Start_numb; pageint <= Config.End_numb; pageint++)
                {
                    page_l.Dispatcher.Invoke(() =>
                    {
                        page_l.Content = string.Format("当前正在处理第{0}页数据", pageint);
                    });
                    new PageList().AnalysisPage(pageint);
                }
                lb.Dispatcher.Invoke(new Action(() =>
                {
                    lb.Items.Add("----------------------------------------------------------------");
                    lb.Items.Add("----------------------------------------------------------------");
                    lb.Items.Add("----------------------全部已完成!-------------------------------");
                    lb.Items.Add("----------------------------------------------------------------");
                }));
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

        private void isshowpb_Checked(object sender, RoutedEventArgs e)
        {
            Config.isShowPB = true;
            wi.Show();
        }

        private void isshowpb_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.isShowPB = false;
            wi.Hide();
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Clipboard.SetText(e.AddedItems[0].ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wi.Close();
            
        }
    }
}
