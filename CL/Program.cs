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
            Config.Task_count = 20;
            Config.WebSleep = 100;

            
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

        //static void setOption()
        //{
        //    Config.End_numb = 99;
        //    Config.Img_path = @"/图种文件夹";
        //    Config.Start_numb = 1;
        //    Config.Task_count = 1;
        //    Config.TypeId = 2;
        //    Config.UseDB = false;
        //    Config.Url = @"https://cl.u5k0.icu";
        //    #region 
        //    string timestr = "早上";
        //    Dictionary<string, string> cd = new Dictionary<string, string>() { { "2", "无码" }, { "15", "有码" }, { "4", "欧美" },
        //        { "5", "动漫" }, { "25", "国产" },{ "26", "中文" },{ "27", "交流" },};
        //    if (DateTime.Now.Hour >= 5 & DateTime.Now.Hour <= 9)
        //        timestr = "早上";
        //    else if (DateTime.Now.Hour >= 10 && DateTime.Now.Hour <= 11)
        //        timestr = "上午";
        //    else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 13)
        //        timestr = "中午";
        //    else if (DateTime.Now.Hour >= 14 && DateTime.Now.Hour <= 18)
        //        timestr = "下午";
        //    else timestr = "晚上";
        //    #endregion
        //    Console.WriteLine("{0}同学,{1}好：  现在时间：{2}","帅彤",timestr,DateTime.Now);
        //    Console.WriteLine("请输入您喜的数字,按[ 回车 ]键 确定");
        //    Console.WriteLine("2=无码  15=有码  4=欧美  5=动漫  25=国产 26=中文  27=交流");
        //    var typeid= Console.ReadLine();
        //    Config.TypeId = int.Parse(typeid);
        //    Console.Clear();
        //    Console.WriteLine("您选择的类型是 {0} ", cd[typeid]);
        //    Console.WriteLine();
        //    Console.WriteLine("请输入图片生成地址 (想要图片放到哪个文件夹，就把那个文件夹拖到这句话上面)");
        //    Console.WriteLine("按[ 回车 ]键 确定");
        //    var filepath= Console.ReadLine();
        //    Config.Img_path = filepath;
        //    Console.Clear();
        //    var info = new Http_Client().get(Config.Url);
        //    if(info==null)
        //    {
        //        Console.WriteLine("最后一步,请输入最新地址(在浏览器拷贝地址后，在这句话上面点击鼠标右键就能粘贴地址), 按[ 回车 ]键 确定");
        //        Console.WriteLine(@"主意地址只要/前面的 例如 https://cl.u5k0.icu/wer/8wer?sfsf=2  则应该输入 https://cl.u5k0.icu");
        //        var urlindex = Console.ReadLine();
        //        Config.Url = urlindex;
        //    }

            
            

        //}


        
    }
}
