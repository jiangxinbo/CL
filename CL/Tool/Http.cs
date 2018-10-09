using CL.Tool;
using Console_DotNetCore_CaoLiu.Bll;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using Tool;

namespace Console_DotNetCore_CaoLiu.Tool
{
    public class Http
    {
        private static HttpClient client;
        //private static CookieContainer cc = new CookieContainer();

        static Http()
        {
            newclient();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void init()
        {
            if (!Directory.Exists(Path.GetDirectoryName(Config.GetGuangGao(""))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.GetGuangGao("")));
            }
            if (!Directory.Exists(Path.GetDirectoryName(Config.GetMakeImgPath(0, Config.TypeId, ""))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.GetMakeImgPath(0, Config.TypeId, "")));
            }
            var s = Path.GetDirectoryName(Config.GetMakeTorrentPath(0, Config.TypeId, ""));
            if (!Directory.Exists(Path.GetDirectoryName(Config.GetMakeTorrentPath(0, Config.TypeId, ""))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.GetMakeTorrentPath(0, Config.TypeId, "")));
            }
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(Config.GetGuangGao(""))))
            {
                Analysis.guanggaoList.Add(Path.GetFileName(file));
            }

            if (Config.Start_numb > 99 || Config.End_numb > 99) login();
            else
            {
                newclient();
            }
        }

        private static void newclient()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 10);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            }
        }

        public static MemoryStream GetFile(string url)
        {
            try
            {

                //Console.WriteLine();
                //Console.WriteLine(url);
                //Console.WriteLine();
                //Console.WriteLine("                                         开始请求文件    " + DateTime.Now);
                Console.Write(",");
                var data = new Http_Client().get(url);
                Console.Write(".");
                //Console.WriteLine();
                //Console.WriteLine("                                         文件请求结束   " + DateTime.Now);
                //Console.WriteLine();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("url:{0} , error:{1}", url,ex.Message);
                L.File.Error(url, ex);
                return null;
            }
        }

        public static void login()
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("pwuser", Config.Uname));
            values.Add(new KeyValuePair<string, string>("pwpwd", Config.Pwd));
            values.Add(new KeyValuePair<string, string>("hideid", "0"));
            values.Add(new KeyValuePair<string, string>("forward", Config.Url + "/"));
            values.Add(new KeyValuePair<string, string>("jumpurl", Config.Url + "/"));

            values.Add(new KeyValuePair<string, string>("step", "2"));
            values.Add(new KeyValuePair<string, string>("cktime", "31536000"));
            var content = new FormUrlEncodedContent(values);
            try
            {
                string loginurl = Config.Url + "/login.php";
                var rsp = client.PostAsync(loginurl, content).Result;
                rsp.Content.Headers.ContentType.CharSet = "gb2312";
                String result = rsp.Content.ReadAsStringAsync().Result;
                if (result.IndexOf("您已經順利登錄") > 0)
                {
                    Console.WriteLine("登录成功");
                    L.File.Warn("登录成功");
                    return;
                }
                else
                {
                    Console.WriteLine("登录失败");
                    L.File.Warn("登录失败"+ result);
                    L.File.Info(result);
                    //Thread.Sleep(5000);
                    //return login();
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("登录错误{0}", ex.Message);
                L.File.Warn("登录错误", ex);
                L.File.Info("登录错误", ex);
                return ;
            }
        }

        /// <summary>
        /// 请求网站内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">默认超时时间60秒</param>
        /// <returns>返回网页数据</returns>
        public static string Postget_String(string url,int postgetcount=1)
        {
            string result = null;
            bool isMainUrl = url.IndexOf(Config.Url) == -1 ? false : true;
            try
            {
                if (isMainUrl)
                {
                    Config.WebTimeSpan.Wait();
                    Console.WriteLine(DateTime.Now+"               进入封印：剩余可进入数量 " + Config.WebTimeSpan.CurrentCount);
                    L.File.WarnFormat("正在请求地址:{0},请求时间{1},请求次数{2}", url, DateTime.Now, postgetcount);
                }
                var response = client.GetAsync(url).Result;
                response.Content.Headers.ContentType.CharSet = "gb2312";
                result = response.Content.ReadAsStringAsync().Result;
                if(isMainUrl)
                {
                    Thread.Sleep(Config.WebSleep+new Random().Next(0,200));
                    Config.WebTimeSpan.Release();
                    Console.WriteLine(DateTime.Now + "                封印解除：剩余可进入数量 " + Config.WebTimeSpan.CurrentCount);
                    L.File.WarnFormat("请求结束 , 地址:{0},结束时间{1},请求次数{2}", url, DateTime.Now, postgetcount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" 请求错误，当前请求次数:" + postgetcount + "   " + url + "   " + e.Message);
                L.File.Error(" 请求错误，当前请求次数:" + postgetcount + "   " + url + "   " + e.Message, e);
                if (isMainUrl)
                {
                    Thread.Sleep(Config.WebSleep + new Random().Next(0, 200));
                    Config.WebTimeSpan.Release();
                    L.File.WarnFormat("请求错误结束 , 地址:{0},结束时间{1},请求次数{2}", url, DateTime.Now, postgetcount);
                }
                if (postgetcount <= 5)
                {
                    Thread.Sleep(Config.WebSleep + new Random().Next(1000, 5000));
                    var webhtml= Postget_String(url,++postgetcount);
                    return webhtml;
                }
                else
                {
                    Console.WriteLine("Postget http请求 超过5次" + url + e.Message);
                    L.File.Error("Postget http请求 超过5次" + url, e);
                    result = null;
                }
            }
            return result;
        }

        public static int getTotalPage(int typeid)
        {
            string url = Config.Url + "/thread0806.php?fid=" + Config.TypeId + "&search=&page=1";
            string htmlstr = Http.Postget_String(url);
            if (htmlstr == null) return 0;
            htmlstr = htmlstr.Replace("\r\n\r\n\t\t↑1\r\n\r\n\t\r\n\r\n\t", "");
            htmlstr = htmlstr.Replace("\r\n\r\n\t\t↑2\r\n\r\n\t\r\n\r\n\t", "");
            htmlstr = htmlstr.Replace("\r\n\r\n\t\t↑3\r\n\r\n\t\r\n\r\n\t", "");
            htmlstr = htmlstr.Replace("\r\n\t\r\n\t", "");
            htmlstr = htmlstr.Replace("\r\n\t", "");
            htmlstr = htmlstr.Replace("\r\n\r\n\t", "");
            htmlstr = htmlstr.Replace("[ <span", "<span");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstr);
            HtmlNode view = doc.GetElementbyId("main");
            if (view == null)
            {
                Console.WriteLine("AnalysisPage() html 没找到 id=main:" + url);
                return 0;
            }
            HtmlNode c_main = view.SelectSingleNode("//a[@class='w70']/input");
            if (c_main == null)
            {
                Console.WriteLine("AnalysisPage() html 没找到 a[@class='w70':" + url);
                return 0;
            }
            var min_max = c_main.Attributes["value"].Value.Split('/');
            return int.Parse(min_max[1]);
        }

    }
}
