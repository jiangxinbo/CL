using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Tool
{
    public class ImageMerge
    {
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="streams">图片流</param>
        /// <param name="minWidth">图片最小宽度</param>
        /// <returns></returns>
        public static  MemoryStream Merge(List<Stream> streams,int minWidth=1500)
        {
            List<Image> imglist = new List<Image>();
            int maxWidth = minWidth;
            foreach (var streamitem in streams)
            {
                Image img = Image.FromStream(streamitem);
                maxWidth = maxWidth < img.Width ? img.Width : maxWidth;
                imglist.Add(img);
            }
            int drawX = 0;//绘图高度
            int drawY = 0;//绘图宽度
            int rowheight = 0; //当前行高
            foreach (var item in imglist)
            {
                if (drawX + item.Width <= maxWidth)
                {
                    drawX += item.Width;
                    if (item.Height > rowheight)
                        rowheight = item.Height;
                }
                else
                {
                    drawY = rowheight;
                    rowheight += item.Height;
                    drawX = item.Width;
                }
            }
            Bitmap b = new Bitmap(maxWidth, rowheight);
            var g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            drawX = 0;//绘图高度
            drawY = 0;//绘图宽度
            rowheight = 0; //当前最大行高
            foreach (var item in imglist)
            {
                if (drawX + item.Width <= maxWidth)
                {
                    //g.DrawImage(item, drawX, drawY, item.Width, item.Height);
                    g.DrawImageUnscaled(item, drawX, drawY, item.Width, item.Height);
                    drawX += item.Width;
                    if (item.Height > rowheight)
                        rowheight = item.Height;
                }
                else
                {
                    drawY = rowheight;
                    rowheight += item.Height;
                    drawX = 0;
                    g.DrawImage(item, drawX, drawY, item.Width, item.Height);
                    drawX += item.Width;
                }
            }
            var myImageCodecInfo = GetEncoderInfo("image/jpeg");
            // 基于GUID创建一个Encoder对象
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream stream = new MemoryStream();
            b.Save(stream, myImageCodecInfo, myEncoderParameters);
            b.Dispose();
            g.Dispose();
            return stream;
        }

        public static MemoryStream Merge(List<Image> imglist, int minWidth = 1500)
        {
            int maxWidth = minWidth;
            foreach (var img in imglist)
            {
                maxWidth = maxWidth < img.Width ? img.Width : maxWidth;
            }
            int drawX = 0;//绘图高度
            int drawY = 0;//绘图宽度
            int rowheight = 0; //当前行高
            foreach (var item in imglist)
            {
                if (drawX + item.Width <= maxWidth)
                {
                    drawX += item.Width;
                    if (item.Height > rowheight)
                        rowheight = item.Height;
                }
                else
                {
                    drawY = rowheight;
                    rowheight += item.Height;
                    drawX = item.Width;
                }
            }
            Bitmap b = new Bitmap(maxWidth, rowheight);
            var g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            drawX = 0;//绘图高度
            drawY = 0;//绘图宽度
            rowheight = 0; //当前最大行高
            foreach (var item in imglist)
            {
                if (drawX + item.Width <= maxWidth)
                {
                    g.DrawImage(item, drawX, drawY, item.Width, item.Height);
                    //g.DrawImageUnscaled(item, drawX, drawY, item.Width, item.Height);
                    drawX += item.Width;
                    if (item.Height > rowheight)
                        rowheight = item.Height;
                }
                else
                {
                    drawY = rowheight;
                    rowheight += item.Height;
                    drawX = 0;
                    g.DrawImage(item, drawX, drawY, item.Width, item.Height);
                    drawX += item.Width;
                }
            }
            var myImageCodecInfo = GetEncoderInfo("image/jpeg");
            // 基于GUID创建一个Encoder对象
            //用于Quality参数类别。
            var myEncoder = Encoder.Quality;

            //创建一个EncoderParameters对象。
            // EncoderParameters对象有一个EncoderParameter数组
            //对象 在这种情况下，只有一个
            //数组中的EncoderParameter对象。
            var myEncoderParameters = new EncoderParameters(1);
            //将位图保存为质量级别为100的JPEG文件。
            var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream stream = new MemoryStream();
            b.Save(stream, myImageCodecInfo, myEncoderParameters);
            b.Dispose();
            g.Dispose();
            return stream;
        }

        /// <summary>
        /// 合并后保存图片
        /// </summary>
        /// <param name="imglist">图片列表</param>
        /// <param name="fileName">完整文件名</param>
        /// <param name="minWidth">最小宽度</param>
        public static void MergeToSaveFile(List<Image> imglist,string fileName, int minWidth = 1500)
        {
            Image img = Image.FromStream(Merge(imglist, minWidth));
            img.Save(fileName);
            //img.Dispose();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        
    }
}
