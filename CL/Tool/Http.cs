using CL.Tool;
using Console_DotNetCore_CaoLiu.Bll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            if (!Directory.Exists(Path.GetDirectoryName(Config.GetGuangGao(""))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.GetGuangGao("")));
            }
            if (!Directory.Exists(Path.GetDirectoryName(Config.GetMakeImgPath(0, Config.TypeId, ""))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.GetMakeImgPath(0, Config.TypeId, "")));
            }
            if (!Directory.Exists(Path.GetDirectoryName(Config.GetMakeTorrentPath(0, Config.TypeId, ""))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Config.GetMakeTorrentPath(0, Config.TypeId, "")));
            }
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(Config.GetGuangGao(""))))
            {
                Analysis.guanggaoList.Add(Path.GetFileName(file));
            }

            if (Config.Start_numb > 99 || Config.End_numb>99)  login();
            else
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
                var data = new Http_Client().get(url);
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
                    return;
                }
                else
                {
                    L.File.Info(result);
                    //Thread.Sleep(5000);
                    //return login();
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("登录错误{0}", ex.Message);
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
            try
            {
                bool isMainUrl = url.IndexOf(Config.Url) == -1 ? false : true;
                if (isMainUrl)
                {
                    Config.WebTimeSpan.Wait();
                }
                var response = client.GetAsync(url).Result;
                response.Content.Headers.ContentType.CharSet = "gb2312";
                result = response.Content.ReadAsStringAsync().Result;
                postgetcount = 0;
                if(isMainUrl)
                {
                    Thread.Sleep(Config.WebSleep);
                    Config.WebTimeSpan.Release();
                }
            }
            catch (Exception e)
            {
                postgetcount++;
                if (postgetcount <= 5)
                {
                    Console.WriteLine(" 请求次数:" + postgetcount + "   " + url + "   " + e.Message);
                    L.File.Error(" 请求次数:" + postgetcount + "   " + url + "   " + e.Message, e);
                    Thread.Sleep(1000 * 1);
                    return Postget_String(url,++postgetcount);
                }
                else
                {
                    Config.WebTimeSpan.Release();
                    Console.WriteLine("Postget http请求 超过5次" + url + e.Message);
                    L.File.Error("Postget http请求 超过5次" + url, e);
                }
            }
            return result;
        }

    }
}
