using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace Console_DotNetCore_CaoLiu.Bll
{
    public class Config
    {
        /// <summary>
        /// 同时可运行的任务
        /// </summary>
        public static SemaphoreSlim TaskRun;

        public static Image pb;

        public static ListBox lb;

        /// <summary>
        /// 当前正在运行的任务数量
        /// </summary>
        public static int currentRunTaskCount = 0;

        /// <summary>
        /// 用于避免频繁访问被封ip
        /// </summary>
        public static SemaphoreSlim WebTimeSpan = new SemaphoreSlim(1, 1);

        static Config()
        {
            Url = "https://cl.k6j9.icu";
            TypeId = 2;
            Task_count = 1;
            Start_numb = 1;
            End_numb = 99;
            Img_path = "cl";
            WebSleep = 500;
            Pwd = "eeak47";
            Uname = "jiangxinbo";
        }

        public static string ProcessDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory; //return AppContext.BaseDirectory;
            }
        }

        private static int task_count;
        /// <summary>
        /// 任务数量
        /// </summary>
        public static int Task_count { get { return task_count; }
            set {
                task_count = value;
                TaskRun = new SemaphoreSlim(value, value);
            } }
        /// <summary>
        /// 开始页数
        /// </summary>
        public static int Start_numb { get; set; }

        /// <summary>
        /// 防止被封休眠毫秒数
        /// </summary>
        public static int WebSleep { get; set; }

        /// <summary>
        /// 结束页数
        /// </summary>
        public static int End_numb { get; set; }
        /// <summary>
        /// 网页地址
        /// </summary>
        public static string Url { get; set; }
        /// <summary>
        /// 网页内容类型
        /// </summary>
        public static int TypeId { get; set; }


        public static string Uname { get; set; }

        public static string Pwd { get; set; }

        


        /// <summary>
        /// 图片根目录
        /// </summary>
        public static string Img_path { get; set; }



        public static string TypeStr(int typeid, bool isZh_Cn = true)
        {
            switch (typeid)
            {
                case 2: return isZh_Cn ? "无码" : "wuma";
                case 15: return isZh_Cn ? "有码" : "youma";
                case 4: return isZh_Cn ? "欧美" : "omei";
                case 5: return isZh_Cn ? "动漫" : "dongman";
                case 25: return isZh_Cn ? "国产" : "guochan";
                case 26: return isZh_Cn ? "中文" : "zhongwen";
                case 27: return isZh_Cn ? "交流" : "jiaoliu";
                default: return isZh_Cn ? "默认" : "default";
            }
        }

        /// <summary>
        /// 图种文件存放地址
        /// </summary>
        /// <param name="size"></param>
        /// <param name="typeid"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetMakeImgPath(float size, int typeid, string title)
        {
            string fname = string.Format("{0}G_【{1}】❤{2}〓", size.ToString("#0.00"), TypeStr(typeid), title);
            return Path.Combine(Img_path, string.Format("zhengwen/{0}/{1}.jpg", TypeStr(typeid, false), fname));
        }

        /// <summary>
        /// 如果没有图片只有文件的存放地址
        /// </summary>
        /// <param name="size"></param>
        /// <param name="typeid"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetMakeTorrentPath(float size, int typeid, string title)
        {
            string fname = string.Format("{0}G_【{1}】❤{2}〓", size.ToString("#0.00"), TypeStr(typeid), title);
            return Path.Combine(Img_path, string.Format("zhengwen/{0}_torrent/{1}.torrent", TypeStr(typeid, false), fname));
        }


        /// <summary>
        /// 获取广告地址
        /// </summary>
        /// <param name="urlpath"></param>
        /// <returns></returns>
        public static string GetGuangGao(string urlpath)
        {
            return Path.Combine(Img_path, string.Format("guanggao/{0}", urlpath));
        }



    //public class Config
    //{
    //    public static IConfigurationRoot Value { get; }

    //    static Config()
    //    {
    //        Value = new ConfigurationBuilder() // nuget Microsoft.Extensions.Configuration;
    //            .SetBasePath(ProcessDirectory)  // nuget Microsoft.Extensions.Configuration.FileExtensions;
    //            .AddJsonFile("config.json")// nuget Microsoft.Extensions.Configuration.Json
    //            .Build();
    //    }

    //    public static string ProcessDirectory
    //    {
    //        get
    //        {
    //            return AppDomain.CurrentDomain.BaseDirectory; //return AppContext.BaseDirectory;
    //        }
    //    }

    //    /// <summary>
    //    /// 任务数量
    //    /// </summary>
    //    public static int Task_count { get { return int.Parse(Value["Thread:task_count"]); } }
    //    /// <summary>
    //    /// 开始页数
    //    /// </summary>
    //    public static int Start_numb { get { return int.Parse(Value["Thread:start_numb"]); } }
    //    /// <summary>
    //    /// 结束页数
    //    /// </summary>
    //    public static int End_numb { get { return int.Parse(Value["Thread:end_numb"]); } }
    //    /// <summary>
    //    /// 网页地址
    //    /// </summary>
    //    public static string Url { get { return Value["Http:url"]; } }
    //    /// <summary>
    //    /// 网页内容类型
    //    /// </summary>
    //    public static int TypeId { get { return int.Parse(Value["Http:typeid"]); } }
    //    /// <summary>
    //    /// mongoDB连接字符串
    //    /// </summary>
    //    public static string MongoStr { get { return Value["ConnectionStrings:mongo"]; } }

    //    public static string Uname { get => Value["Http:uname"]; }

    //    public static string Pwd { get => Value["Http:pwd"]; }

    //    /// <summary>
    //    /// 0:只打印页面级信息   1:打印调试信息
    //    /// </summary>
    //    public static int PrintId { get => int.Parse(Value["Thread:print"]); }


    //    /// <summary>
    //    /// 图片根目录
    //    /// </summary>
    //    public static string Img_path { get { return Value["File:img_path"]; } }

    //    //public static string Torrent_path { get { return Value["File:torrent_path"]; } }

    //    /// <summary>
    //    /// 是否启用数据库
    //    /// </summary>
    //    public static bool UseDB { get { return bool.Parse(Value["UseDB"]); } }

    //    public static string TypeStr(int typeid, bool isZh_Cn = true)
    //    {
    //        switch (typeid)
    //        {
    //            case 2: return isZh_Cn ? "无码" : "wuma";
    //            case 15: return isZh_Cn ? "有码" : "youma";
    //            case 4: return isZh_Cn ? "欧美" : "omei";
    //            case 5: return isZh_Cn ? "动漫" : "dongman";
    //            case 25: return isZh_Cn ? "国产" : "guochan";
    //            case 26: return isZh_Cn ? "中文" : "zhongwen";
    //            case 27: return isZh_Cn ? "交流" : "jiaoliu";
    //            default: return isZh_Cn ? "默认" : "default";
    //        }
    //    }

    //    /// <summary>
    //    /// 图种文件存放地址
    //    /// </summary>
    //    /// <param name="size"></param>
    //    /// <param name="typeid"></param>
    //    /// <param name="title"></param>
    //    /// <returns></returns>
    //    public static string GetMakeImgPath(float size, int typeid, string title)
    //    {
    //        string fname = string.Format("{0}G_【{1}】❤{2}〓", size.ToString("#0.00"), TypeStr(typeid), title);
    //        return Path.Combine(Img_path, string.Format("zhengwen/{0}/{1}.jpg", TypeStr(typeid, false), fname));
    //    }

    //    /// <summary>
    //    /// 如果没有图片只有文件的存放地址
    //    /// </summary>
    //    /// <param name="size"></param>
    //    /// <param name="typeid"></param>
    //    /// <param name="title"></param>
    //    /// <returns></returns>
    //    public static string GetMakeTorrentPath(float size, int typeid, string title)
    //    {
    //        string fname = string.Format("{0}G_【{1}】❤{2}〓", size.ToString("#0.00"), TypeStr(typeid), title);
    //        return Path.Combine(Img_path, string.Format("zhengwen/{0}_torrent/{1}.torrent", TypeStr(typeid, false), fname));
    //    }


    //    /// <summary>
    //    /// 获取广告地址
    //    /// </summary>
    //    /// <param name="urlpath"></param>
    //    /// <returns></returns>
    //    public static string GetGuangGao(string urlpath)
    //    {
    //        return Path.Combine(Img_path, string.Format("guanggao/{0}", urlpath));
    //    }



    }
}
