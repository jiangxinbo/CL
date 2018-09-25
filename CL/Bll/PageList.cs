using CL.Tool;
using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Tool;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool;

namespace CL.Bll
{
    public class PageList
    {
        public static int tongji=0;
        /// <summary>
        /// 已处理帖子总数
        /// </summary>
        public static int TotalPost=1;
        /// <summary>
        /// 当前页数
        /// </summary>
        private int currentPage = 0;
        /// <summary>
        /// 当前处理第几个帖子
        /// </summary>
        private int currentPost = 1;

        /// <summary>
        /// 解析页
        /// </summary>
        /// <param name="pageint">当前页数</param>
        public void AnalysisPage(int pageint)
        {
            currentPage = pageint;
            string url = Config.Url + "/thread0806.php?fid=" + Config.TypeId + "&search=&page=" + pageint;
            string htmlstr = Http.Postget_String(url);
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
            bool flag = false;
            currentPost = 0;
            foreach (var item in trlist)
            {
                ++currentPost;
                if (flag)
                {
                    var link = item.SelectSingleNode("td[2]/h3/a");
                    if (link == null) continue;
                    string titleUrl = Config.Url+"/" + link.GetAttributeValue("href", null);
                    string title = link.InnerText;
                    float size = Analysis.GetSize(title);
                    var imgPath = Config.GetMakeImgPath(size, Config.TypeId, title);
                    if (File.Exists(imgPath))
                    {
                        continue;
                    }

                    try
                    {
                        Config.TaskRun.Wait();
                        Interlocked.Increment(ref tongji);
                        Console.WriteLine("正在运行任务数量:当前信号量   {0}:{1}    -- {2}", tongji, Config.TaskRun.CurrentCount,DateTime.Now);
                        Console.WriteLine("  正在处理第{0}页数据中的第{1}个帖子", currentPage, currentPost);
                        Task.Factory.StartNew((param) => {
                            new PagePost().savePage(param as PagePostParam);
                            Config.TaskRun.Release();
                           Console.WriteLine("正在运行任务数量:当前信号量   {0}:{1}    -- {2}", Interlocked.Decrement(ref PageList.tongji),Config.TaskRun.CurrentCount, DateTime.Now);
                        },new PagePostParam() { Title = title, PostUrl = titleUrl, PageCount = currentPage, PostIndex=currentPost });
                    }
                    catch (Exception ex)
                    {
                        Config.TaskRun.Release();
                        Console.WriteLine(ex.Message);
                        L.File.Error("titleUrl", ex);
                        continue;
                    }
                }
                else
                {
                    if (pageint == 1)
                    {
                        if (item.InnerText.IndexOf("普通主題") >= 0)
                        {
                            flag = true;
                        }
                        Console.WriteLine("正在处理第{0}页数据中的第{1}个帖子--[普通主題]", currentPage, currentPost);
                        Console.WriteLine("        第{0}页数据中的第{1}个帖子,处理完毕", currentPage, currentPost);
                    }
                    else
                    {
                        if (item.InnerText.IndexOf("狀態文章作者回復最後發表") >= 0)
                        {
                            flag = true;
                        }
                        Console.WriteLine("正在处理第{0}页数据中的第{1}个帖子--[最後發表]", currentPage, currentPost);
                        Console.WriteLine("        第{0}页数据中的第{1}个帖子,处理完毕", currentPage, currentPost);
                    }
                }
                
            }

        }
    }

    public class PagePostParam
    {
        /// <summary>
        /// 帖子标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 帖子url
        /// </summary>
        public string PostUrl { get; set; }
        /// <summary>
        /// 当前第几页
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 当前第几个帖子
        /// </summary>
        public int PostIndex { get; set; }
    }
}
