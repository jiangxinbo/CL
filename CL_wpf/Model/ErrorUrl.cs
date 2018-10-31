using System;
using System.Collections.Generic;
using System.Text;

namespace Console_DotNetCore_CaoLiu.Model
{
    public class ErrorUrl
    {
        private int id;
        private string title;
        private string downloadUrl;
        private string openUrl;
        private int size;

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string DownloadUrl { get => downloadUrl; set => downloadUrl = value; }
        public string OpenUrl { get => openUrl; set => openUrl = value; }
        public int Size { get => size; set => size = value; }
    }
}
