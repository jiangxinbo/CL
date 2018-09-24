using Console_DotNetCore_CaoLiu.Model;
using Console_DotNetCore_CaoLiu.Tool;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Tool;

namespace Console_DotNetCore_CaoLiu.Bll
{
    public class TorrentDownload_mongo
    {

        private CookieContainer cc = new CookieContainer();
        /// <summary>
        /// 记录数量
        /// </summary>
        int recordCount = 0;
        private string userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        int taskid = 0;

        public TorrentDownload_mongo(int i)
        {
            taskid = i;
        }

        private void printRecordCount(int id)
        {
            int rc = Interlocked.Increment(ref recordCount);
            Console.WriteLine("线程:{0}  已处理:{1}   真实id:{2}", taskid.ToString().PadRight(9), rc.ToString().PadRight(9), id.ToString().PadRight(9));
        }

        private void init(int i)
        {
            taskid = i;
        }

        public Stream GetTorrentStream(PageWeb pw, int pageint)
        {
            if (!File.Exists(pw.Filepath))
            {
                if (pw.Download == null) return null;
                return GetTorrent(pw);
            }
            return null;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        private Stream GetTorrent(PageWeb pw)
        {
            string html = null;
            if (pw.Download.Length == 79)
            {
                html = GetTorrentHtml(pw.Download, pw);
                if (string.IsNullOrWhiteSpace(html)) return null;
            }
            else if (pw.Download.Length == 78)
            {
                html = Repair_Hash_GetTorrentHtml(pw);
                if (string.IsNullOrWhiteSpace(html)) return null;
            }
            else return null;
            if (string.IsNullOrWhiteSpace(html))
            {
                L.File.Info(pw.Id);
                return null;
            }
            string newurl = null;
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode paramRef = doc.DocumentNode.SelectSingleNode("//input[@name='ref']");
                string p_ref = paramRef.GetAttributeValue("Value", null);
                HtmlNode paramReff = doc.DocumentNode.SelectSingleNode("//input[@name='reff']");
                string p_reff = paramReff.GetAttributeValue("Value", null);
                newurl = string.Format("http://www.rmdown.com/download.php?ref={0}&reff={1}", p_ref, p_reff);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTorrent({0}) html解析错误", pw.Download);
                L.File.Error(string.Format("GetTorrent({0}) html解析错误", pw.Download), ex);
                return null;
            }
            return Http.GetFile(newurl);
        }

        //private Stream GetFile(string url,int timeout = 60)
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
        //        request.UserAgent = userAgent;
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        request.KeepAlive = true;
        //        request.Method = "get";
        //        //获取服务器返回
        //        response = (HttpWebResponse)request.GetResponse();
        //        MemoryStream ms = new MemoryStream();
        //        //获取HTTP返回数据
        //        using (Stream myStream = response.GetResponseStream())
        //        {
        //            //定义一个字节数据
        //            byte[] btContent = new byte[1024];
        //            int intSize = 0;
        //            intSize = myStream.Read(btContent, 0, 1024);
        //            while (intSize >= 0)
        //            {
        //                ms.Write(btContent, 0, intSize);
        //                intSize = myStream.Read(btContent, 0, 1024);
        //            }
        //            //关闭流
        //            myStream.Close();
        //            response.Close();
        //            request.Abort();
        //            return ms;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        L.File.Error(url + " [ 下载种子文件错误！ ]", e);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">downloadurl</param>
        /// <param name="pw"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private string GetTorrentHtml(string url, PageWeb pw, int timeout = 60)
        {
            System.GC.Collect();
            string hash = null;
            try
            {
                hash = url.Substring(url.IndexOf("hash="));
                hash = hash.Replace("<", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTorrentHtml() id={0} ,hash解析错误", pw.Id);
                L.File.ErrorFormat(url, ex);
                return null;
            }
            string result = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            //请求url以获取数据
            try
            {
                Console.WriteLine();
                Console.WriteLine("请求种子   " + url);
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 5000;
                //如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                }
                request.Timeout = timeout * 1000;
                request.ReadWriteTimeout = timeout * 1000;
                request.ContinueTimeout = timeout * 1000;
                request.Referer = string.Format("http://www.viidii.info/?http://www______rmdown______com/link______php?{0}&z", hash);
                if (request.CookieContainer == null)
                {
                    request.CookieContainer = cc;
                }
                request.UserAgent = userAgent;
                request.ContentType = "application/x-www-form-urlencoded";
                //request.AllowWriteStreamBuffering = false;
                //启用页面压缩技术
                //request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                request.KeepAlive = true;
                request.Method = "get";
                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();
                //获取HTTP返回数据
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                {
                    result = sr.ReadToEnd().Trim();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetTorrentHtml() id={0}  url:{1}  url长度:{2}", pw.Id.ToString().PadRight(8), url, url.Length);
                L.File.Error(string.Format("GetTorrentHtml() id={0}  url:{1}  url长度:{2}", pw.Id.ToString().PadRight(8), url, url.Length), e);
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            //关闭连接和流
            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }
            return result;
        }

        /// <summary>
        /// 修复hash值不完整
        /// </summary>
        /// <param name="url"></param>
        /// <returns> 返回 GetTorrentHtml()后的html页面</returns>
        private string Repair_Hash_GetTorrentHtml(PageWeb pw)
        {
            string new_url = null;
            for (int i = 0; i <= 9; i++)
            {
                var html = GetTorrentHtml(pw.Download + i, pw);
                if (html != null)
                {
                    new_url = pw.Download + i;
                    return html;
                }
            }
            for (char i = 'a'; i <= 'f'; i++)
            {
                var html = GetTorrentHtml(pw.Download + i.ToString(), pw);
                if (html != null)
                {
                    return html;
                }
            }
            return null;
        }
    }
}
