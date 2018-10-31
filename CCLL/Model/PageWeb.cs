using Console_DotNetCore_CaoLiu.Bll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Console_DotNetCore_CaoLiu.Model
{
    public class PageWeb
    {
        public PageWeb()
        {
            
        }
        private string id;
        //标题
        private string title;
        //原网址
        private string openurl;
        //json图片地址列表
        private List<string> imgs = new List<string>();
        //下载地址
        private string download;
        //名称列表
        private List<string> names = new List<string>();
        //等级 默认0   50 已经识别
        private int level;
        //创建时间
        private string ctime;
        private long ctimeint;

        //是否番号重复
        private int state_chongfu = 0;
        //是否已浏览 1浏览  0未浏览
        private int state_look = 0;
        private int state_chou = 0;
        //类型,1wuma 2youma 3omei 4dongman 5guochan 6zhongzi 7jiaoliu 8htt 9zaixian 10jishu
        private int typeid = 0;
        private float size = 0;//文件大小 单位GB
        private string fan1Hao4;//番号
        private string filepath;//bt文件地址


        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public String Openurl
        {
            get
            {
                return openurl;
            }

            set
            {
                openurl = value;
            }
        }

        public List<string> Imgs
        {
            get
            {
                return imgs;
            }

            set
            {
                imgs = value;
            }
        }

        public string Download
        {
            get
            {
                return download;
            }

            set
            {
                download = value;
            }
        }







        /// <summary>
        /// 创建时间
        /// </summary>

        public string Ctime
        {
            get
            {
                return ctime;
            }

            set
            {
                ctime = value;
            }
        }


        public List<string> Names
        {
            get
            {
                return names;
            }

            set
            {
                names = value;
            }
        }

        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        /// <summary>
        /// 是否已浏览  0未浏览  1已浏览  
        /// </summary>
        public int StateLook
        {
            get
            {
                return state_look;
            }

            set
            {
                state_look = value;
            }
        }

        /// <summary>
        ///是否是重复番号 1番号重复  0番号不重复
        /// </summary>
        public int StateChongFu
        {
            get { return state_chongfu; }
            set { state_chongfu = value; }
        }

        /// <summary>
        /// </summary>
        public int Typeid
        {
            get
            {
                return typeid;
            }

            set
            {
                typeid = value;
            }
        }

        public float Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public string Fan1Hao4
        {
            get { return fan1Hao4; }
            set { fan1Hao4 = value; }
        }

        /// <summary>
        ///  1表示丑  2表示美   3表示一般
        /// </summary>
        public int State_chou
        {
            get { return state_chou; }
            set { state_chou = value; }
        }

        /// <summary>
        /// bt文件地址
        /// </summary>
        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        


        
        public string TypeStr
        {
            get => Config.TypeStr(typeid);
        }
    }
}
