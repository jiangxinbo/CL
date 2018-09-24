using Console_DotNetCore_CaoLiu.Model;
using Console_DotNetCore_CaoLiu.Tool;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tool;

//CaptchaGen.NetCore

namespace Console_DotNetCore_CaoLiu.Bll
{
    public class HtmlHelp_mongo
    {
        private List<string> guanggaoList = new List<string>();
        int postgetcount = 0;// Postget() 请求错误次数
        
        /// <summary>
        /// 域名 ss.wedid.us
        /// </summary>
        string domainName = null;

        int recordPageCount;//已经处理的页数
        int recordItemCount;//已经处理帖子
        
        string indexUrl;

        HttpClient client;
        public int taskid;
        /// <summary>
        /// 当前第几页
        /// </summary>
        private int currentPage;
        private int currentTotalRow;
        /// <summary>
        /// 当前处理第几条数据
        /// </summary>
        private int currentRow;
        /// <summary>
        /// 当前帖子总图片数量
        /// </summary>
        private int currentTotalImg;
        /// <summary>
        /// 当天处理第几个图片
        /// </summary>
        private int currentImgCount;

        public HtmlHelp_mongo()
        {
            domainName = Config.Url;
            indexUrl = domainName + "/";
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
                guanggaoList.Add(Path.GetFileName(file));
            }
        }

        public HttpClient login()
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("pwuser", Config.Uname));
            values.Add(new KeyValuePair<string, string>("pwpwd", Config.Pwd));
            values.Add(new KeyValuePair<string, string>("hideid", "0"));
            values.Add(new KeyValuePair<string, string>("forward", indexUrl));
            values.Add(new KeyValuePair<string, string>("jumpurl", indexUrl));

            values.Add(new KeyValuePair<string, string>("step", "2"));
            values.Add(new KeyValuePair<string, string>("cktime", "31536000"));
            var content = new FormUrlEncodedContent(values);
            try
            {
                string loginurl = domainName + "/login.php";
                var rsp = client.PostAsync(loginurl, content).Result;
                rsp.Content.Headers.ContentType.CharSet = "gb2312";
                String result = rsp.Content.ReadAsStringAsync().Result;
                if (result.IndexOf("您已經順利登錄") > 0)
                {
                    Console.WriteLine(taskid+" 登录成功");
                    return client;
                }
                else
                {
                    L.File.Info(result);
                    //Thread.Sleep(5000);
                    //return login();
                }
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine("登录错误{0}", ex.Message);
                L.File.Info("登录错误", ex);
                Thread.Sleep(5000);
                return login();
            }
        }

        public void Init(int start_count, int end_count,int taskid)
        {
            this.taskid = taskid;
            Console.WriteLine("起始页数:{0},结束页数:{1},文件地址:{2}", start_count, end_count, indexUrl);
            recordPageCount = 0;
            if (Math.Abs(start_count - end_count) > 99) client = login();
            else
            {
                client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            }

            if (start_count <= end_count)
            {
                for (int pageint = start_count; pageint <= end_count; pageint++)
                {
                    AnalysisPage(pageint);
                    Interlocked.Increment(ref recordPageCount);
                }
            }
            else
            {
                for (int pageint = start_count; pageint >= end_count; pageint--)
                {
                    AnalysisPage(pageint);
                    Interlocked.Increment(ref recordPageCount);
                }
            }

        }

        /// <summary>
        /// 解析页
        /// </summary>
        /// <param name="pageint">当前页数</param>
        public void AnalysisPage(int pageint)
        {
            currentPage = pageint;
            string url = indexUrl + "thread0806.php?fid=" + Config.TypeId + "&search=&page=" + pageint;
            Console.WriteLine(taskid+" 正在请求:{0}", url);
            string htmlstr = Postget(url);
            if (htmlstr == null) return;
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
                return;
            }
            HtmlNode tbody = null;
            if (Config.TypeId == 2)
            {
                HtmlNode c_main = view.SelectSingleNode("div[3]/table");
                if (c_main == null)
                {
                    Console.WriteLine("AnalysisPage() html 没找到 div[3]/table:" + url);
                    return;
                }
                string table = c_main.OuterHtml;
                HtmlDocument newdoc = new HtmlDocument();
                newdoc.LoadHtml(table);
                tbody = newdoc.DocumentNode.SelectSingleNode("table/tbody");
            }
            else if (Config.TypeId == 15 || Config.TypeId == 25 || Config.TypeId == 26 || Config.TypeId == 27 || Config.TypeId == 4 || Config.TypeId == 5)
            {
                tbody = view.SelectSingleNode("//table[@id='ajaxtable']/tbody");
            }
            else
            {
                tbody = view.SelectSingleNode("//table[@id='ajaxtable']/tbody");
            }
            if (tbody == null)
            {
                Console.WriteLine("AnalysisPage() html 没找到 table/tbody:" + url);
                return;
            }
            var trlist = tbody.SelectNodes("tr");
            if (trlist == null || trlist.Count == 0)
            {
                Console.WriteLine("AnalysisPage() html 没有找到帖子行数 或者行数为0:" + url);
                return;
            }
            Console.WriteLine(taskid+" 第{0}页,共有{1}条数据", pageint, trlist.Count);
            bool flag = false;
            currentTotalRow = trlist.Count;
            currentRow = 0;
            foreach (var item in trlist)
            {
                Console.WriteLine();
                Console.WriteLine("将处理第{0}页 {1}条 - 第{2}条   ", currentPage, currentTotalRow, ++currentRow);
                if (flag)
                {
                    var link = item.SelectSingleNode("td[2]/h3/a");
                    if (link == null) continue;
                    string titleUrl = indexUrl + link.GetAttributeValue("href", null);
                    string title = link.InnerText;
                    bool hasTitle = false;
                    float size = GetSize(title);
                    var imgPath= Config.GetMakeImgPath(size, Config.TypeId, title);
                    if(File.Exists(imgPath))
                    {
                        Interlocked.Increment(ref recordItemCount);
                        Console.WriteLine("第{0}页 {1}条 - 第{2}条 数据处理完毕      ", currentPage,currentTotalRow, currentRow);
                        continue;
                    }

                    
                   
                    try
                    {
                        savePage(title, titleUrl, pageint);
                        Console.WriteLine("第{0}页 {1}条 - 第{2}条 数据处理完毕      ", currentPage, currentTotalRow, currentRow);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("第{0}页 {1}条 - 第{2}条 数据处理 【失败】      ", currentPage, currentTotalRow, currentRow);
                        Console.WriteLine(ex.Message);
                        L.File.Error("titleUrl", ex);
                        Interlocked.Increment(ref recordItemCount);
                        continue;
                    }
                    
                }
                if (pageint == 1)
                {
                    if (item.InnerText.IndexOf("普通主題") >= 0)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (item.InnerText.IndexOf("狀態文章作者回復最後發表") >= 0)
                    {
                        flag = true;
                    }
                }
                Console.WriteLine("第{0}页 {1}条 - 第{2}条 数据处理完毕      ", currentPage, currentTotalRow, currentRow);
            }

        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        private void savePage(string title, string url, int pageint)
        {
            if (title == null) return;
            PageWeb pw = new PageWeb();
            pw.Typeid = Config.TypeId;
            pw.Title = title;
            GetFan1Hao4(pw);//设置番号
            GetSize(pw);//设置大小
            foreach (var citem in Path.GetInvalidFileNameChars())
            {
                title = title.Replace(citem, '_');
            }
            pw.Title = title;
            
            
            var imgpath = Config.GetMakeImgPath(pw.Size, pw.Typeid, pw.Title);
            if (File.Exists(imgpath))
            {
                if (Config.PrintId == 1) Console.Write(" img_jump ");
                return;
            }
            var torrent = Config.GetMakeTorrentPath(pw.Size, pw.Typeid, pw.Title);
            if (File.Exists(torrent))
            {
                if (Config.PrintId == 1) Console.Write(" torrent_jump ");
                return;
            }
            pw.Filepath = imgpath;
            string openurl = null;
            string web = null;
            try
            {
                getPageContext(url, out openurl, out web);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                L.File.Error("savePage()-"+ url, ex);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(web))
            {
                Thread.Sleep(1000 * 1);
                var new_client = login();
                savePage(title, url, pageint);
                return;
            }
            List<string> namelist = new List<string>();
            foreach (string name in CLConfig.has)
            {
                if (title.ToLower().IndexOf(name.ToLower()) != -1) pw.Names.Add(name);
            }
            if (pw.Names.Count > 0) pw.Level = 50;
            pw.Openurl = openurl;
            pw.StateLook = 0;
            pw.Ctime = Time.getTime();
            Stream imgStream = null;
            imgStream = getImages(pw, web,pageint);
            getDownLoadUrl(pw, web);
            if (imgStream != null)
            {//如果有图片
                
                TorrentDownload_mongo td = new TorrentDownload_mongo(0);
                Stream torrentStream = td.GetTorrentStream(pw, pageint);
                if (torrentStream != null)
                {//如果有文件
                    Console.WriteLine("种子读取  成功->图种制作  开始");
                    ImgRar.ToRar(pw, torrentStream, imgStream);
                    Console.WriteLine("              ->图种制作  完成");
                }
                else
                {
                    L.File.Warn(string.Format("帖子有图片，【但帖子没有种子】 url:{0}", pw.Openurl));
                    Console.WriteLine("种子读取  失败");
                    imgStream.Close();
                    File.Copy("default.jpg", imgpath);
                }
            }
            else
            {//如果没有图片
                TorrentDownload_mongo td = new TorrentDownload_mongo(0);
                Stream torrentStream = td.GetTorrentStream(pw, pageint);
                Console.WriteLine("td.GetTorrentStream  完成");
                if (torrentStream != null)
                {//如果有文件
                    FileStream fs = new FileStream(torrent, FileMode.Create);
                    byte[] by = new byte[1024 * 10];
                    torrentStream.Position = 0;
                    int size = torrentStream.Read(by, 0, by.Length);
                    while (size > 0)
                    {
                        fs.Write(by, 0, size);
                        size = torrentStream.Read(by, 0, by.Length);
                    }
                    fs.Close();
                    torrentStream.Close();
                    L.File.Debug(string.Format("种子已下载，【帖子没有图片】 url:{0}", pw.Openurl));
                }
                else
                {
                    L.File.Warn(string.Format("帖子既没有种子，又没有图片】 url:{0}", pw.Openurl));
                }
                File.Copy("default.jpg", imgpath);
                Console.Write("图片列表为 空 ,种子下载完成 ");
            }
            Console.Clear();
            Console.WriteLine("第{0}页 共{1}条 - 第【{2}】条 数据处理完毕,本次运行已处理[ {3} ]个帖子", currentPage, currentTotalRow, currentRow, Interlocked.Increment(ref recordItemCount));
        }

        private void getPageContext(string itemValue, out string openurl, out string web)
        {
            web = Postget(itemValue);
            openurl = itemValue;
            if (web == null) return;
            string tishi = "正在轉入主題, 请稍后";
            int tiaozhuan = web.IndexOf(tishi);
            if (tiaozhuan > 0)
            {
                web = web.Substring(tiaozhuan + 15 + tishi.Length);
                int ruguoj = web.LastIndexOf("如果");
                string itemurl = indexUrl + web.Substring(0, ruguoj - 2);
                openurl = itemurl;
                web = Postget(itemurl);
            }
        }

        private Stream getImages(PageWeb pw, string webStr,int pageint=0)
        {
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(webStr);
            string info;
            HtmlNode view = doc.GetElementbyId("main");
            if (view == null)
            {
                info = webStr;
            }
            else
            {
                HtmlNode c_main = view.SelectSingleNode("//div[@class='tpc_content do_not_catch']");

                if (c_main != null)
                {
                    info = c_main.OuterHtml;
                }
                else
                {
                    info = view.OuterHtml;
                }
            }
            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(info);
            if (Config.PrintId == 1) Console.Write("---- img分析图片标签 -----");
            // 取得匹配项列表 
            List<Stream> streamsList = new List<Stream>();
            currentTotalImg = matches.Count;
            currentImgCount = 0;
            foreach (Match match in matches)
            {
                
                Console.WriteLine("第{0}页 {1} - {2}条  ->   {3} - 第 {4} 个图片开始检查      ", pageint,currentTotalRow,currentRow,currentTotalImg ,++currentImgCount);
                var imgurl = match.Groups["imgUrl"].Value;
                if (imgurl.ToLower().IndexOf(".gif") == -1)
                {
                    if (guanggaoList.IndexOf(Encrypt.getSha1(imgurl) + ".jpg") > -1)
                    {
                        Console.WriteLine(" 此图片是广告 已跳过 ");
                        continue;
                    }
                    Console.WriteLine(pw.Openurl);
                    if(imgurl== "http://img599.net/images/2018/09/20/001.th.jpg")
                    {
                        Console.WriteLine();
                    }
                    var ms=Http.GetFile(imgurl);
                    if(ms==null)
                    {
                        continue;
                    }
                    try
                    {
                        Console.WriteLine("Http.GetFile 结束");
                        Image img = Image.FromStream(ms);
                        Console.WriteLine("Image.FromStream() 结束");
                        if (img.Width / img.Height < 3)
                        {
                            pw.Imgs.Add(imgurl);
                            MemoryStream truems = new MemoryStream();
                            img.Save(truems, ImageFormat.Jpeg);
                            streamsList.Add(truems);
                        }
                        else
                        {
                            var ggname = Encrypt.getSha1(imgurl) + ".jpg";
                            var endimgfile = Config.GetGuangGao(ggname);
                            img.Save(endimgfile, ImageFormat.Jpeg);
                            guanggaoList.Add(ggname);
                        }
                        ms.Close();
                    }
                    catch (Exception ex)
                    {
                        L.File.Error(imgurl, ex);
                        ms.Close();
                    }
                }
                Console.Clear();
            }
            if (streamsList.Count > 0)
            {
                Console.WriteLine("{0} - 所有图片下载完毕   图片合成 开始", DateTime.Now);
                var result= ImageMerge.Merge(streamsList, 0);
                Console.WriteLine("{0} - 所有图片下载完毕   图片合成 完毕", DateTime.Now);
                return result;
            }
            Console.WriteLine("读取后可用图片为 0", DateTime.Now);
            return null;
        }

        private void getDownLoadUrl(PageWeb pw, string html)
        {
            string basestr = "rmdown.com/link.php?hash=";
            HtmlDocument newdoc = new HtmlDocument();
            newdoc.LoadHtml(html);
            string webStr = "";
            var tbody = newdoc.DocumentNode.SelectSingleNode("//div[@class='t t2'][1]/div[@class='tpc_content']");
            if (tbody != null) webStr = tbody.OuterHtml;
            else webStr = html;
            int indexCount = webStr.IndexOf(basestr);
            if (indexCount < 0)
            {
                string basestr2 = "rmdown______com/link______php?hash=";
                indexCount = webStr.IndexOf(basestr2);
                if (indexCount < 0)
                {
                    return;
                }
                webStr = "http://www." + basestr + webStr.Substring(indexCount + basestr2.Length, 43);
            }
            else
            {
                webStr = "http://www." + webStr.Substring(indexCount, 43 + basestr.Length);
            }
            pw.Download = webStr;
            int dayue = pw.Download.IndexOf("<");
            if (dayue > 0) pw.Download = pw.Download.Substring(0, dayue);
        }

        /// <summary>
        /// 设置番号
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private void GetFan1Hao4(PageWeb pw)
        {
            string fh = null;
            string pattern = @"([A-Za-z0-9_]+\s)*([A-Za-z0-9_]+(_|-)){1,2}[a-zA-Z]*[0-9]+";
            try
            {
                foreach (Match match in Regex.Matches(pw.Title, pattern))
                {
                    pw.Fan1Hao4 = match.Value;
                    return;
                }
            }
            catch (Exception ex)
            {
                L.File.Error("GetFan1Hao4()", ex);
                Console.WriteLine(ex.Message);
            }
            if (fh == null)
            {
                pattern = @"[A-Za-z0-9_]+-[A-Za-z0-9_]+\s\d+";
                foreach (Match match in Regex.Matches(pw.Title, pattern))
                {
                    pw.Fan1Hao4 = match.Value;
                    return;
                }
            }
            if (fh == null)
            {
                pattern = @"([A-Za-z0-9_]+\s+)+[a-zA-Z]+[0-9]+";
                foreach (Match match in Regex.Matches(pw.Title, pattern))
                {
                    pw.Fan1Hao4 = match.Value;
                    return;
                }
            }
            pw.Fan1Hao4 = "";
            return;
        }

        /// <summary>
        /// 从标题中过滤出文件大小  **.**G
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private void GetSize(PageWeb pw)
        {
            pw.Size = GetSize(pw.Title);
        }

        private float GetSize(string title)
        {
            string pattern = @"\[.*\/\s*(\d\.*\d*)(\s*\w{1,2})\s?\]";
            foreach (Match match in Regex.Matches(title, pattern))
            {
                float num = 0;
                try
                {
                    num = Convert.ToSingle(match.Groups[1].Value);
                }
                catch (Exception ex)
                {
                    try
                    {
                        num = Convert.ToSingle(ToDBC(match.Groups[1].Value));
                    }
                    catch (Exception ee)
                    {
                        L.File.Error("GetSize()--", ee);
                        Console.WriteLine(ex.Message);
                    }
                }
                var dw = match.Groups[2].Value.ToLower();
                if (dw == "m" || dw == "mb")
                {
                    num = num / 1024;
                }
                else if (dw == "g" || dw == "gb")
                {
                }
                else
                {
                    num = 0;
                }
                return num;
            }
            return 0;
        }

        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">默认超时时间60秒</param>
        /// <returns>返回网页数据</returns>
        public string Postget(string url)
        {
            string result = null;
            try
            {
                Console.Write("Postget 1");
                var response = client.GetAsync(url).Result;
                response.Content.Headers.ContentType.CharSet = "gb2312";
                result = response.Content.ReadAsStringAsync().Result;
                Console.Write("Postget 2");
                postgetcount = 0;
            }
            catch (Exception e)
            {
                postgetcount++;
                if (postgetcount <= 5)
                {
                    Console.WriteLine(taskid+" 请求次数:" + postgetcount + "   " + url + "   " + e.Message);
                    L.File.Error(taskid + " 请求次数:" + postgetcount + "   " + url + "   " + e.Message, e);
                    Thread.Sleep(1000 * 1);
                    return Postget(url);
                }
                else
                {
                    Console.WriteLine("Postget http请求 超过5次" + url + e.Message);
                    L.File.Error("Postget http请求 超过5次" + url, e);
                }
            }
            return result;
        }

    }
}
