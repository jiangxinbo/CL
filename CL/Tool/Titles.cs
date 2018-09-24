using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tool
{
    public class Titles
    {
        /// <summary>
        /// 存放所有番号
        /// </summary>
        public static List<string> fanhao_List = new List<string>();
        /// <summary>
        /// 存放所有网页url用来检测是否有重复项
        /// </summary>
        public static List<string> ls = new List<string>();

        /// <summary>
        /// 从标题中过滤出文件大小  **.**G
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
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
                        L.File.Error("--------------------", ee);
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
        private static String ToDBC(String input)
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
        /// 获取番号
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetFan1Hao4(string title)
        {
            string fh = null;
            string pattern = @"([A-Za-z0-9_]+\s)*([A-Za-z0-9_]+(_|-)){1,2}[a-zA-Z]*[0-9]+";
            foreach (Match match in Regex.Matches(title, pattern))
            {
                return match.Value;
            }
            if (fh == null)
            {
                pattern = @"[A-Za-z0-9_]+-[A-Za-z0-9_]+\s\d+";
                foreach (Match match in Regex.Matches(title, pattern))
                {
                    return match.Value;
                }
            }
            if (fh == null)
            {
                pattern = @"([A-Za-z0-9_]+\s+)+[a-zA-Z]+[0-9]+";
                foreach (Match match in Regex.Matches(title, pattern))
                {
                    return fh = match.Value;
                }
            }
            return "";
        }

        /// <summary>
        /// 此番号是否已经存在
        /// </summary>
        /// <param name="fh"></param>
        public static bool isHasFanHao(int typeid, string fanhao)
        {
            if (Titles.fanhao_List.Contains(typeid + fanhao))
            {
                return true;
            }
            return false;
        }

    }
}
