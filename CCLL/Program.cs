using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCLL
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var f1 = new Form1();
            f1.updatebox = (pictureBox1) => {
                pictureBox1.Image = Image.FromFile(@"A:\a.jpg");
                pictureBox1.Refresh();
                Thread.Sleep(2000);
                pictureBox1.Image = Image.FromFile(@"A:\timg (2).jpg");
                pictureBox1.Refresh();
            };
            Application.Run(f1);
        }
    }
}
