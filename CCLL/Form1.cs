using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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



        }
        public dl1st updatebox;
        private void button1_Click(object sender, EventArgs e)
        {
            var url = textBox1.Text;
            var info = new Http_Client().get(Config.Url);
            if (info == null)
            {
                listBox1.Items.Add("网址错误");
            }
 
            
            var typeid = comboBox1.SelectedValue;
            Config.TypeId = (int)(typeid);
            //int maxPage = Http.getTotalPage(Config.TypeId);
            //Console.WriteLine("您选择的类型是 {0}  ， 最大页数 {1}", cd[typeid], maxPage);
            //Console.WriteLine();

            //Console.WriteLine("开始页数 1");
            //var start_page = 1; //Console.ReadLine();
            //Config.Start_numb = 1;
            ////Config.Start_numb = int.Parse(start_page);
            //Console.WriteLine("结束页数 " + maxPage);
            //var end_page = maxPage;// Console.ReadLine();
            //// Config.End_numb = int.Parse(end_page);
            //Config.End_numb = maxPage;
            //Console.WriteLine("任务数量");
            //var task_count = Console.ReadLine();
            //Config.Task_count = int.Parse(task_count);

            //Console.WriteLine("请输入图片生成地址 (想要图片放到哪个文件夹，就把那个文件夹拖到这句话上面) 按[ 回车 ]键 确定");
            //var filepath = Console.ReadLine();
            //Config.Img_path = filepath;



        }
    }
}
