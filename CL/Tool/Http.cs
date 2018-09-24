using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Tool;

namespace Console_DotNetCore_CaoLiu.Tool
{
    public class Http
    {
        private static CookieContainer cc = new CookieContainer();

        public static MemoryStream GetFile(string url)
        {
            try
            {

                Console.WriteLine();
                Console.WriteLine(url);
                Console.WriteLine();
                Console.WriteLine("                                         开始请求文件    " + DateTime.Now);
                var data = new Http_Client().get(url);
                Console.WriteLine();
                Console.WriteLine("                                         文件请求结束   " + DateTime.Now);
                Console.WriteLine();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("url:{0} , error:{1}", url,ex.Message);
                L.File.Error(url, ex);
                return null;
            }
        }

        //public static Stream GetFile(string url)
        //{
        //    try
        //    {
        //        Console.WriteLine();
        //        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!        开始请求图片    " + DateTime.Now);
        //        var data = new Http_Client().get(url);
        //        Console.WriteLine();
        //        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!        图片请求结束   " + DateTime.Now);
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(url);
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }

        //}

        //public static MemoryStream GetFile(string url)
        //{
        //    try
        //    {
        //        Console.WriteLine("********************************   开始请求图片    " + DateTime.Now);
        //        TcpClientHttpRequest tc = new TcpClientHttpRequest();
        //        tc.Action = url;
        //        tc.Method = "get";
        //        Console.WriteLine("-------send() 之前  " + tc.Head);
        //        tc.Send();
        //        Console.WriteLine("-------send() 之后  " + tc.Head);
        //        Console.WriteLine(tc.Head);

        //        Console.WriteLine(tc.Response.Head);
        //        Console.WriteLine("------------------图片请求 Response.GetStream()   " + DateTime.Now);
        //        var data = tc.Response.GetStream();
        //        Console.WriteLine("********************************   图片请求结束   " + DateTime.Now);
        //        MemoryStream ms = new MemoryStream(data);
        //        Console.WriteLine("********************************   MemoryStream(data) 结束 返回ms   " + DateTime.Now);
        //        return ms;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(url);
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }

        //}

        //public static MemoryStream GetFile(string url, int timeout)
        //{
        //    System.GC.Collect();

        //    //设置最大连接数
        //    ServicePointManager.DefaultConnectionLimit = 5000;
        //    HttpWebRequest request = null;
        //    HttpWebResponse response = null;
        //    //请求url以获取数据
        //    try
        //    {
        //        //如果是发送HTTPS请求  
        //        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        //        {
        //            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        //            request = WebRequest.Create(url) as HttpWebRequest;
        //            request.ProtocolVersion = HttpVersion.Version10;
        //        }
        //        else
        //        {
        //            request = (HttpWebRequest)WebRequest.Create(url);
        //        }
        //        request.Timeout = timeout * 1000;
        //        request.ReadWriteTimeout = timeout * 1000;
        //        request.ContinueTimeout = timeout * 1000;
        //        request.Referer = "http://www.viidii.info/?http://www______rmdown______com/link______php?hash=1712f854535ce0184529d410e8f12eddc14d6f9aacd&z";
        //        if (request.CookieContainer == null)
        //        {
        //            request.CookieContainer = cc;
        //        }
        //        request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 50.0.2661.102 Safari / 537.36";
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        request.KeepAlive = true;
        //        request.Method = "get";
        //        //获取服务器返回
        //        response = (HttpWebResponse)request.GetResponse();
        //        MemoryStream ms = new MemoryStream();
        //        Console.Write("  {0}  ", url);
        //        //获取HTTP返回数据
        //        using (Stream myStream = response.GetResponseStream())
        //        {
        //            myStream.CopyTo(ms);
        //            //var length = response.ContentLength;
        //            ////定义一个字节数据
        //            //byte[] btContent = new byte[1024];
        //            //int intSize = 0;
        //            //do
        //            //{
        //            //    if (ms.Length == length)
        //            //    {
        //            //        break;
        //            //    }
        //            //    Console.WriteLine("    1      intSize：{0}  ，ms.Length：{1}  ，total：{2}", intSize, ms.Length, length);
        //            //    intSize = myStream.Read(btContent, 0, btContent.Length);

        //            //    if (intSize > -1)
        //            //    {
        //            //        ms.Write(btContent, 0, intSize);
        //            //    }
        //            //    Console.WriteLine("    2      intSize：{0}  ，ms.Length：{1}  ，total：{2}", intSize, ms.Length, length);
        //            //    if (ms.Length == length)
        //            //    {
        //            //        break;
        //            //    }
        //            //}
        //            //while (intSize > -1);


        //            //关闭流
        //            myStream.Close();
        //            response.Close();
        //            request.Abort();
        //            return ms;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        L.File.Error(url + " [ 下载错误！ ]", e);
        //        //关闭连接和流
        //        if (response != null)
        //        {
        //            response.Close();
        //        }
        //        if (request != null)
        //        {
        //            request.Abort();
        //        }
        //    }
        //    return null;
        //}

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
