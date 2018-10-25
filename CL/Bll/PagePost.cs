using CL.Tool;
using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Model;
using Console_DotNetCore_CaoLiu.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Tool;

namespace CL.Bll
{
    public class PagePost
    {
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        public void savePage(PagePostParam ppp)
        {
            string title = ppp.Title;
            string url = ppp.PostUrl;
            int pageint = ppp.PageCount;
            if (title == null) return;
            PageWeb pw = new PageWeb();
            pw.Typeid = Config.TypeId;
            pw.Title = title;
            Analysis.GetFan1Hao4(pw);//设置番号
            Analysis.GetSize(pw);//设置大小
            foreach (var citem in Path.GetInvalidFileNameChars())
            {
                title = title.Replace(citem, '_');
            }
            pw.Title = title;
            var imgpath = Config.GetMakeImgPath(pw.Size, pw.Typeid, pw.Title);
            if (File.Exists(imgpath)) return;
            var torrent = Config.GetMakeTorrentPath(pw.Size, pw.Typeid, pw.Title);
            //if (File.Exists(torrent)) return;
            pw.Filepath = imgpath;
            string openurl = null;
            string web = null;
            try
            {
                Analysis.getPageContext(url, out openurl, out web);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                L.File.Error("savePage()-" + url, ex);
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
            imgStream = Analysis.getImages(pw, web, pageint);
            Analysis.getDownLoadUrl(pw, web);
            if (imgStream != null)
            {//如果有图片

                TorrentDownload_mongo td = new TorrentDownload_mongo(0);
                Stream torrentStream = td.GetTorrentStream(pw, pageint);
                //如果有文件
                if (torrentStream != null)
                {
                    ImgRar.ToRar(pw, torrentStream, imgStream);
                    Console.WriteLine("图种制作完成");
                }
                //如果没有文件
                else
                {
                    //L.File.Debug(string.Format("帖子有图片，【但帖子没有种子】 url:{0}", pw.Openurl));
                    Console.WriteLine("种子读取  失败");
                    imgStream.Close();
                    File.Copy("default.jpg", imgpath);
                }
            }
            //如果没有图片
            else
            {
                TorrentDownload_mongo td = new TorrentDownload_mongo(0);
                Stream torrentStream = td.GetTorrentStream(pw, pageint);
                //如果有文件
                if (torrentStream != null)
                {
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
                    //L.File.Debug(string.Format("种子已下载，【帖子没有图片】 url:{0}", pw.Openurl));
                }
                //如果没有文件
                else
                {
                    //L.File.Debug(string.Format("帖子既没有种子，又没有图片】 url:{0}", pw.Openurl));
                }
                File.Copy("default.jpg", imgpath);
                Console.WriteLine("帖子中没有图片,种子下载完成 {0}", pw.Openurl);
            }

            var totalpost= Interlocked.Increment(ref PageList.TotalPost);
            Console.WriteLine("--第 {0} 页数据中的第[{1}]个帖子处理完毕--,已处理【{2}】个帖子", ppp.PageCount, ppp.PostIndex, totalpost);
        }
    }
}
