using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Model;
using Console_DotNetCore_CaoLiu.Tool;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Tool;

namespace CL.Tool
{
    public static class Analysis
    {

        public static List<string> guanggaoList = new List<string>();

        /// <summary>
        /// 设置番号
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static void GetFan1Hao4(PageWeb pw)
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
        public static void GetSize(PageWeb pw)
        {
            pw.Size = GetSize(pw.Title);
        }

        public static float GetSize(string title)
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

        /// <summary>
        /// 全角空格为12288，半角空格为32，其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String ToDBC(String input)
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

        public static void getDownLoadUrl(PageWeb pw, string html)
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

        public static Stream getImages(PageWeb pw, string webStr, int pageint = 0)
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
            // 取得匹配项列表 
            List<Stream> streamsList = new List<Stream>();
            foreach (Match match in matches)
            {
                var imgurl = match.Groups["imgUrl"].Value;
                if (imgurl.ToLower().IndexOf(".gif") == -1)
                {
                    if (guanggaoList.IndexOf(Encrypt.getSha1(imgurl) + ".jpg") > -1)
                    {
                        //Console.WriteLine(" 此图片是广告 已跳过 ");
                        continue;
                    }
                    //Console.WriteLine("图片请求 {0} ，{1}",imgurl,DateTime.Now);
                    var ms = Http.GetFile(imgurl);
                    if (ms == null)
                    {
                        continue;
                    }
                    try
                    {
                        Image img = Image.FromStream(ms);
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
                //Console.Clear();
            }
            if (streamsList.Count > 0)
            {
                MemoryStream result = null;
                try
                {
                    result = ImageMerge.Merge(streamsList, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("合并图片错误：" + pw.Openurl);
                    L.File.Error("合并图片错误："+pw.Openurl, ex);
                }
                
                //Console.WriteLine("{0} - 图片合成 完毕", DateTime.Now);
                return result;
            }
            //Console.WriteLine("读取后可用图片为 0", DateTime.Now);
            return null;
        }

        public static void getPageContext(string itemValue, out string openurl, out string web)
        {
            web = Http.Postget_String(itemValue);
            openurl = itemValue;
            if (web == null) return;
            string tishi = "正在轉入主題, 请稍后";
            int tiaozhuan = web.IndexOf(tishi);
            if (tiaozhuan > 0)
            {
                web = web.Substring(tiaozhuan + 15 + tishi.Length);
                int ruguoj = web.LastIndexOf("如果");
                string itemurl = Config.Url + "/" + web.Substring(0, ruguoj - 2);
                openurl = itemurl;
                web = Http.Postget_String(itemurl);
            }
        }

    }
}
