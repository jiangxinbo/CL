using CL.Bll;
using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Model;
using Console_DotNetCore_CaoLiu.Tool;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool;

namespace CL
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Config.TypeId = 2;
            //Config.Task_count = 20;
            Config.WebSleep = 100;

            //Config.Start_numb = 1;
            //Config.End_numb = 500;

            //查询每种类型最大页数

            

            setOption();


            Console.WriteLine("正在初始化...");
            Http.init();

            for (int pageint = Config.Start_numb; pageint <= Config.End_numb; pageint++)
            {
                new PageList().AnalysisPage(pageint);
            }
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("----------------------全部已完成!-------------------------------");
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine();
            Console.Read();
        }

        static void setOption()
        {
            #region 
            string timestr = "早上";
            Dictionary<string, string> cd = new Dictionary<string, string>() { { "2", "无码" }, { "15", "有码" }, { "4", "欧美" },
                { "5", "动漫" }, { "25", "国产" },{ "26", "中文" },{ "27", "交流" },};
            if (DateTime.Now.Hour >= 5 & DateTime.Now.Hour <= 9)
                timestr = "早上";
            else if (DateTime.Now.Hour >= 10 && DateTime.Now.Hour <= 11)
                timestr = "上午";
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 13)
                timestr = "中午";
            else if (DateTime.Now.Hour >= 14 && DateTime.Now.Hour <= 18)
                timestr = "下午";
            else timestr = "晚上";
            #endregion
            var info = new Http_Client().get(Config.Url);
            if (info == null)
            {
                Console.WriteLine("请输入最新地址(在浏览器拷贝地址后，在这句话上面点击鼠标右键就能粘贴地址), 按[ 回车 ]键 确定");
                Console.WriteLine(@"主意地址只要/前面的 例如 https://cl.u5k0.icu/wer/8wer?sfsf=2  则应该输入 https://cl.u5k0.icu");
                var urlindex = Console.ReadLine();
                Config.Url = urlindex;
            }
            Console.WriteLine("{0}同学,{1}好：  现在时间：{2}", "**", timestr, DateTime.Now);
            
            Console.WriteLine("2=无码  15=有码  4=欧美  5=动漫  25=国产 26=中文  27=交流 请输入您喜的数字,按[ 回车 ]键 确定");
            var typeid = Console.ReadLine();
            Config.TypeId = int.Parse(typeid);
            Console.WriteLine("您选择的类型是 {0}  ， 最大页数 {1}", cd[typeid],Http.getTotalPage(Config.TypeId));
            Console.WriteLine();

            Console.WriteLine("开始页数");
            var start_page = Console.ReadLine();
            Config.Start_numb = int.Parse(start_page);
            Console.WriteLine("结束页数");
            var end_page = Console.ReadLine();
            Config.End_numb = int.Parse(end_page);
            Console.WriteLine("任务数量");
            var task_count = Console.ReadLine();
            Config.Task_count = int.Parse(task_count);

            Console.WriteLine("请输入图片生成地址 (想要图片放到哪个文件夹，就把那个文件夹拖到这句话上面) 按[ 回车 ]键 确定");
            var filepath = Console.ReadLine();
            Config.Img_path = filepath;




        }



    }
}
