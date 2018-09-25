using Console_DotNetCore_CaoLiu.Model;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Console_DotNetCore_CaoLiu.Bll
{
    class ImgRar
    {
        public ImgRar()
        {
            ////以下三个路径都是完整路径，含后缀名
            //var path1 = @"A:\xz\c.jpg";//.jpg  图片路径
            //var path2 = @"A:\xz\cc.zip";//.7z||.rar
            //var path3 = @"A:\xz\a.jpg";//.jpg  生成路径   最后需要改成.rar
            //const int rbuffer = 1024;
            //FileStream fs1 = new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.Read);
            //FileStream fs2 = new FileStream(path3, FileMode.Create, FileAccess.Write, FileShare.None);
            //byte[] farr = new byte[1024];
            //int i = fs1.Read(farr, 0, rbuffer);

            //while (i != 0) //返回0表示读完 
            //{
            //    fs2.Write(farr, 0, rbuffer);
            //    i = fs1.Read(farr, 0, rbuffer);
            //}
            //fs1 = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read);
            //i = fs1.Read(farr, 0, rbuffer);
            //while (i != 0) //返回0表示读完 
            //{
            //    fs2.Write(farr, 0, rbuffer);
            //    i = fs1.Read(farr, 0, rbuffer);
            //}
            //fs1.Close();
            //fs2.Close();
            //Console.WriteLine("over");
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="data2"></param>
        //public static void ToRar(byte[] data,byte[] data2)
        //{
        //    var filename = "a.rar";
        //    ////得到一个压缩文件,流
        //    FileStream zipFile = new FileStream(filename, FileMode.Create);
        //    //创建一个压缩流,写入压缩流中的内容，自动被压缩
        //    ZipOutputStream zos = new ZipOutputStream(zipFile);
        //    //第一步，写入压缩的说明
        //    ZipEntry entry = new ZipEntry("abc.jpg");
        //    entry.Size = data.Length;
        //    //保存
        //    zos.PutNextEntry(entry);
        //    //第二步，写入压缩的文件内容
        //    zos.Write(data, 0, data.Length);
        //    zos.Finish();
        //    zos.Close();
        //    FileStream fs = File.OpenRead(filename);

        //    FileStream f = new FileStream("b.jpg", FileMode.Create);
        //    //f.Write(data2, 0, data2.Length);
        //    byte[] rar = new byte[fs.Length];
        //    fs.Read(rar, 0, rar.Length);
        //    byte[] nrar = new byte[fs.Length + data2.Length];
        //    data2.CopyTo(nrar, 0);
        //    rar.CopyTo(nrar, data2.Length);
        //    f.Write(nrar, 0, nrar.Length);
        //    f.Flush();


        //    var iii = fs.Length / 1024;

        //}

        ///完结版
        public static void ToRar(byte[] data, byte[] data2)
        {
            var filename = "a.rar";
            ////得到一个压缩文件,流
            MemoryStream zipms = new MemoryStream();
            //创建一个压缩流,写入压缩流中的内容，自动被压缩
            ZipOutputStream zos = new ZipOutputStream(zipms);
            //第一步，写入压缩的说明
            ZipEntry entry = new ZipEntry("abc.jpg");
            entry.Size = data.Length;
            //保存
            zos.PutNextEntry(entry);
            //第二步，写入压缩的文件内容
            zos.Write(data, 0, data.Length);
            zos.Finish();
           
            FileStream f = new FileStream("b.jpg", FileMode.Create);
            zipms.Position = 0;
            byte[] rarbyte = new byte[zipms.Length + data2.Length];
            data2.CopyTo(rarbyte, 0);
            zipms.Read(rarbyte, data2.Length, (int)zipms.Length);
            f.Write(rarbyte, 0, rarbyte.Length);
            f.Flush();
            zos.Close();
        }


        public static void ToRar(PageWeb pw, Stream file, Stream img)
        {
            var imgpath = pw.Filepath;
            if (File.Exists(imgpath)) return;
            ////得到一个压缩文件,流
            MemoryStream zipms = new MemoryStream();
            //创建一个压缩流,写入压缩流中的内容，自动被压缩
            ZipOutputStream zos = new ZipOutputStream(zipms);
            //第一步，写入压缩的说明
            ZipEntry entry = new ZipEntry(pw.Title+".torrent");
            entry.Size = file.Length;
            //保存
            zos.PutNextEntry(entry);
            //第二步，写入压缩的文件内容

            file.Position = 0;
            byte[] tempby = new byte[file.Length];
            file.Read(tempby, 0, tempby.Length);
            file.Close();
            zos.Write(tempby, 0, tempby.Length);
            
            zos.Finish();
            byte[] data2 = new byte[img.Length];
            img.Position = 0;
            img.Read(data2, 0, data2.Length);
            img.Close();

            FileStream f = new FileStream(imgpath, FileMode.Create);
            zipms.Position = 0;
            byte[] rarbyte = new byte[zipms.Length + data2.Length];
            data2.CopyTo(rarbyte, 0);
            zipms.Read(rarbyte, data2.Length, (int)zipms.Length);
            f.Write(rarbyte, 0, rarbyte.Length);
            f.Flush();
            zos.Close();
            //Console.Write(" imgRar_完成 ");

        }

    }
}
