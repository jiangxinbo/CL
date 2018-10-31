using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    //加密类
    public class Encrypt
    {
        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getSha1(string str)
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

        /// <summary>
        /// 获取32位小写md5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getMD5_32(string input)
        {
            //System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            //byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
            //var t2 = BitConverter.ToString(data);
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < data.Length; i++)
            //{
            //    sb.Append(data[i].ToString("X2"));
            //}
            //return sb.ToString();
            byte[] result = Encoding.UTF8.GetBytes(input);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }
        /// <summary>
        /// 获取16位小写md5 建议用来加密密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string getMD5_16(string input)
        {
            return getMD5_32(input).Substring(8, 16);
        }
    }
}
