using System;
using System.Collections.Generic;
using System.Text;

namespace Console_DotNetCore_CaoLiu.Tool
{
    public class MyGroup
    {
        /// <summary>
        /// 获取分组
        /// </summary>
        /// <param name="startint">开始页</param>
        /// <param name="endint">结束页</param>
        /// <param name="taskcount">分组数量</param>
        /// <returns></returns>
        public static List<StartEnd> GetCroup(int startint, int endint, int taskcount = 1)
        {
            List<StartEnd> list = new List<StartEnd>();
            int totalCount = 0;
            int star = startint;
            if (endint == 1 && startint - endint > 0)
            {
                totalCount = startint - endint + 1;
                for (int i = 1; i <= taskcount; i++)
                {
                    int end = startint - totalCount * i / taskcount;
                    list.Add(new StartEnd() { Start = star, End = end });
                    star = end + 1;
                }
            }
            else
            {
                totalCount = endint - startint + 1;
                for (int i = 1; i <= taskcount; i++)
                {
                    int end = startint + totalCount * i / taskcount;
                    list.Add(new StartEnd() { Start = star, End = end });
                    star = end + 1;
                }
            }
            return list;
        }

    }

    public class StartEnd
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
}
