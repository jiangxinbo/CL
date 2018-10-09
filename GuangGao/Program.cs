using Console_DotNetCore_CaoLiu.Bll;
using Console_DotNetCore_CaoLiu.Tool;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GuangGao
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("输入广告图片Web地址");

            var imgurl = Console.ReadLine();
            //http://oi67.tinypic.com/5p3k7q.jpg
           
            var ggname = GetSha1(imgurl) + ".jpg";
            Console.WriteLine(ggname);
        }

        public static string GetSha1(string str)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();

            //将mystr转换成byte[]
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);

            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string
            return BitConverter.ToString(dataHashed).Replace("-", "").ToLower();
        }
    }
}
