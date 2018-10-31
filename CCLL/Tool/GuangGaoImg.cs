using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Tool;

namespace Console_DotNetCore_CaoLiu
{
    public class GuangGaoImg
    {
        /// <summary>
        /// 通过web图片地址获取图片名称
        /// </summary>
        /// <param name="imgurl">图片网络地址</param>
        /// <param name="newPath">新地址</param>
        public static void guanggao_web_to_name(string imgurl,string newPath="f:/gg")
        {
            string newname= Encrypt.getSha1(imgurl) + ".jpg";
            WebClient http = new WebClient();
            http.DownloadFile(imgurl, Path.Combine( "f:/gg" , newname));
            http.Dispose();
        }
    }
}
