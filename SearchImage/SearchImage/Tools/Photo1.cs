using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SearchImage.Tools
{
    public class Photo1
    {
        Image img;
        public Bitmap Resize(string imageFile, string newImageFile)
        {
            img = Image.FromFile(imageFile);
            Bitmap imgOutput = new Bitmap(img, 256, 256);
            imgOutput.Save(newImageFile, ImageFormat.Jpeg);
            imgOutput.Dispose();
            return (Bitmap)Image.FromFile(newImageFile);
        }


        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            img.Dispose();
            return histogram;
        }

        //计算相减后的绝对值
        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (result == 0)
                result = 1;
            return abs / result;
        }

        //最终计算结果
        public float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
            {
                return 0;
            }
            else
            {
                float result = 0;
                int j = firstNum.Length;
                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                    //Console.WriteLine(i + "----" + result);
                }
                return result / j;
            }
        }
    }
}
