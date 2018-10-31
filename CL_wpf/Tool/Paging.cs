using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// mvc分页
    /// </summary>
    public class Paging
    {
        /// <summary>
        /// 每页显示10条记录
        /// </summary>
        public static readonly int pageCount = 100;
        /// <summary>
        /// 分页标签 显示8页角码（|12345678|第1/14页 共200条记录）
        /// </summary>
        public static readonly int pageNum = 20;


        public static int getZongPage(long zong, int pageCount)
        {
            return Convert.ToInt32(Math.Ceiling(zong / Convert.ToDouble(pageCount)));
        }

        /// <summary>
        /// 生成   |12345678|第1/14页 共200条记录 导航
        /// </summary>
        /// <param name="ViewBag"></param>
        /// <param name="nowPage">需要显示第几页</param>
        /// <param name="pageNum">分页标签 显示8页角码（|12345678|第1/14页 共200条记录）</param>
        public static void setPageNavigation(dynamic ViewBag, int nowPage, int pageNum, long zong)
        {
            int zongPage = getZongPage(zong, pageCount);
            ViewBag.zongPage = zongPage; //总页数
            ViewBag.page = nowPage;//当前页码
            ViewBag.zong = zong;//共有多少条数据
            if (zongPage == 0)
            {
                ViewBag.page = 1;
                ViewBag.first = 1;
                ViewBag.previous = 1;
                ViewBag.next = 1;
                ViewBag.tar = new List<int>() { 1 };
                return;
            }
            ViewBag.first = 1;
            if (nowPage == 1)
            {
                ViewBag.previous = 1;
            }
            else
            {
                ViewBag.previous = nowPage - 1;
            }
            if (nowPage == zongPage)
            {
                ViewBag.next = zongPage;
            }
            else
            {
                ViewBag.next = nowPage + 1;
            }
            var pageStart = 1;
            var pageEnd = 0;
            if (nowPage <= pageNum / 2 + 1)
            {
                pageStart = 1;
                pageEnd = pageNum;
            }
            else if (nowPage > pageNum / 2 + 1)
            {
                pageStart = nowPage - pageNum / 2;
                pageEnd = nowPage + pageNum / 2 - 1;
            }
            var xx = 0;
            if (pageEnd > zongPage)
            {
                xx = pageEnd - zongPage;
                pageEnd = zongPage;
            }
            pageStart -= xx;
            if (pageEnd <= pageNum)
            {
                pageStart = 1;
            }
            List<int> pageTar = new List<int>();
            if (pageEnd - pageStart >= 0)
            {
                for (int i = 0; i <= pageEnd - pageStart; i++)
                {
                    pageTar.Add(pageStart + i);
                }
            }
            ViewBag.tar = pageTar;
        }
    }
}
