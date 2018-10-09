using System;
using System.IO;

namespace UnRar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("输入压缩目录");
            var imgpath = Console.ReadLine();
            foreach(var filename in Directory.GetFiles(imgpath))
            {
                var s = Path.GetFullPath(filename)+Path.GetFileNameWithoutExtension(filename) + ".rar";
                File.Copy(filename, s);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
