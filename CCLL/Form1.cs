using CL.Bll;
using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCLL
{
    public delegate void dl1st(PictureBox obj);
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            
            textBox5.Text = Path.Combine(Application.StartupPath, "CL"); 
            DataTable ADt = new DataTable();
            DataColumn ADC1 = new DataColumn("name", typeof(string)); 
            DataColumn ADC2 = new DataColumn("id", typeof(int));
            ADt.Columns.Add(ADC1);
            ADt.Columns.Add(ADC2);
            DataRow ADR = ADt.NewRow();
            ADR[0] = "无码";
            ADR[1] = 2;
            ADt.Rows.Add(ADR);

            ADR = ADt.NewRow();
            ADR[0] = "有码";
            ADR[1] = 15;
            ADt.Rows.Add(ADR);

            ADR = ADt.NewRow();
            ADR[0] = "欧美";
            ADR[1] = 4;
            ADt.Rows.Add(ADR);

            ADR = ADt.NewRow();
            ADR[0] = "动漫";
            ADR[1] = 5;
            ADt.Rows.Add(ADR);

            ADR = ADt.NewRow();
            ADR[0] = "国产";
            ADR[1] = 25;
            ADt.Rows.Add(ADR);


            ADR = ADt.NewRow();
            ADR[0] = "中文";
            ADR[1] = 26;
            ADt.Rows.Add(ADR);

            ADR = ADt.NewRow();
            ADR[0] = "交流";
            ADR[1] = 27;
            ADt.Rows.Add(ADR);

            comboBox1.DisplayMember = "name";  //控件显示的列名

            comboBox1.ValueMember = "id";  //控件值的列名

            comboBox1.DataSource = ADt;
            Config.pb = pictureBox1;
            Config.lb = listBox1;
        }
        public dl1st updatebox;
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                var url = textBox1.Text;
                Config.Url = url;
                var info = new Http_Client().get(Config.Url);
                if (info == null)
                {
                    comboBox1.Invoke(new Action(() => {
                        listBox1.Items.Add("网址错误");
                    }));
                    
                }

                comboBox1.Invoke(new Action(()=> {
                    var typeid = comboBox1.SelectedValue;
                    Config.TypeId = (int)(typeid);
                    listBox1.Items.Add("您选择了 "+ comboBox1.Text);
                    listBox1.Refresh();
                }));
               
                

                textBox3.Invoke(new Action(() => {
                    int maxPage = Http.getTotalPage(Config.TypeId);
                    Config.End_numb = maxPage;
                    textBox3.Text = maxPage.ToString();
                }));

                textBox2.Invoke(new Action(() => {
                    var start_str = textBox2.Text.ToString();
                    Config.Start_numb = int.Parse(start_str);
                }));

                textBox4.Invoke(new Action(() => {
                    var task_count_str = textBox4.Text.ToString();
                    Config.Task_count = int.Parse(task_count_str);
                }));

                textBox5.Invoke(new Action(() => {
                    var filepath = textBox5.Text.ToString();
                    Config.Img_path = filepath;
                }));


                comboBox1.Invoke(new Action(() => {
                    listBox1.Items.Clear();
                    listBox1.Items.Add("正在初始化...");
                    Http.init();
                    Thread.Sleep(2000);
                    Config.WebSleep = 1500;
                    listBox1.Items.Clear();
                    listBox1.Items.Add("初始化完成");
                    for (int pageint = Config.Start_numb; pageint <= Config.End_numb; pageint++)
                    {
                        listBox1.Items.Clear();
                        listBox1.Items.Add(string.Format("当前正在处理第{0}页数据", pageint));
                        new PageList().AnalysisPage(pageint);
                    }
                    listBox1.Items.Add("----------------------------------------------------------------");
                    listBox1.Items.Add("----------------------------------------------------------------");
                    listBox1.Items.Add("----------------------全部已完成!-------------------------------");
                    listBox1.Items.Add("----------------------------------------------------------------");
                }));

                
            });
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.textBox5 .Text = path.SelectedPath;
        }
    }
}
