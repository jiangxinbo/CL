using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Tool;

namespace Console_DotNetCore_CaoLiu.Tool
{
    /// <summary>
    /// 自己写的版本
    /// </summary>
    public class Http_Client
    {
        private int timeout = 10;
        private Socket _socket;
        private SslStream ssl = null;
        private bool isHttps = false;
        private int headLengh = 0;
        private HttpHeader HttpHeaders;
        private List<string> Cookies;
        public Http_Client()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public MemoryStream get(string url, string datainfo = null, string method = "get", string charset = "UTF-8", string referer = null)
        {
            try
            {
                return init(url, datainfo, method, charset, referer);
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
                L.File.Error(url, ex);
            }
            finally
            {
                _socket.Close();
            }
            return null;

        }

        private MemoryStream init(string url, string datainfo = null, string method = "get", string charset = "UTF-8", string referer = null)
        {
            MemoryStream stream = null;
            #region
            var coding = Encoding.GetEncoding(charset);
            byte[] dataByte = null;
            if (datainfo != null)
                dataByte = coding.GetBytes(datainfo);
            Uri uri = new Uri(url);
            _socket.Connect(uri.Host, uri.Port);
            MemoryStream ms = new MemoryStream();
            if (referer == null) referer = url;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0} {1} HTTP/1.1\r\n", method.ToUpper(), uri.PathAndQuery));
            sb.Append(string.Format("Host: {0}\r\n", uri.Authority));
            sb.Append("Accept: */*\r\n");
            sb.Append("Content-Type:application/x-www-form-urlencoded\r\n");
            //sb.Append("Connection: keep - alive\r\n");
            //sb.Append("User - Agent: Mozilla / 5.0(Windows NT 10.0; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.132 Safari / 537.36\r\n");
            //sb.Append(string.Format("Accept: image / webp,image / apng,image/*,*/*; q = 0.8\r\n"));
            //sb.Append(string.Format("Referer: {0}\r\n", referer));
            //sb.Append(string.Format("Accept - Encoding: gzip, deflate, br\r\n"));
            //if (datainfo != null)
            //    sb.Append(string.Format("Content-Length:{0}\r\n", dataByte.Length));
            //sb.Append(string.Format("Accept - Language: zh - CN,zh; q = 0.9,en; q = 0.8\r\n"));
            sb.Append(string.Format("\r\n"));
            var sendinfo = sb.ToString();
            _socket.ReceiveTimeout = 1000 * timeout;
            _socket.SendTimeout = 1000 * timeout;

            isHttps = string.Compare(uri.Scheme, "https", false) == 0;
            if (isHttps)
            {
                ssl = new SslStream(new NetworkStream(_socket), false, delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                    return sslPolicyErrors == SslPolicyErrors.None;
                }, null);
                ssl.ReadTimeout = 1000 * timeout;
                ssl.WriteTimeout = 1000 * timeout;
                ssl.AuthenticateAsClient(uri.Host);
                ssl.Write(coding.GetBytes(sendinfo));
                if (datainfo != null)
                    ssl.Write(dataByte);
                //ssl.Flush();
            }
            else
            {
                var result_int = _socket.Send(coding.GetBytes(sendinfo));
                if (datainfo != null)
                    result_int = _socket.Send(dataByte);
            }
            #endregion
            byte[] filedata= getHeader();
            int status_code = int.Parse(HttpHeaders.ResponseStatusCode);
            if(status_code!=200)
            {
                L.File.InfoFormat("状态码：【{0}】 , [ url ]：{1}", status_code, url);
            }
            switch (status_code)
            {
                case 403:return stream;
                default:break;
            }
            //Console.WriteLine("                                               {0}",status_code);
            if(status_code>=500)
            {
                Console.WriteLine("                服务器返回状态码：{0}  ---  {1}", status_code,url);
                return stream;
            }

            if(HttpHeaders.IsChunk)
            {
                byte[] data = new byte[102400];
                int bodyStart = 0;//数据部分起始位置
                int readLength = filedata.Length;
                stream = new MemoryStream();
                Array.Copy(filedata, 0, data, 0, filedata.Length);
                GetChunkData(ref data, ref bodyStart, ref readLength, ref stream);
            }
            else
            {
                stream = defaultReceive(filedata);
            }
            if (!string.IsNullOrWhiteSpace(this.HttpHeaders.Location))
            {
                stream = new Http_Client().get(this.HttpHeaders.Location, referer= uri.AbsoluteUri);
            }
            if (HttpHeaders.IsGzip)
            {
                stream=unGzip(stream);
            }
            stream.Flush();
            return stream;
        }

        /// <summary>
        /// 获取头文件
        /// </summary>
        /// <returns></returns>
        private byte[] getHeader()
        {
            MemoryStream header_ms = new MemoryStream();
            MemoryStream file = new MemoryStream();
            StringBuilder sb = new StringBuilder();
            int rece = 0;
            do
            {
                byte[] data = new byte[102400];

                var d1 = _socket.Connected;

                if (isHttps)
                {
                    rece = ssl.Read(data, 0, data.Length);
                }
                else
                {
                    rece = _socket.Receive(data, 0, data.Length, SocketFlags.None);
                }
                if (headLengh == 0)
                {   //读取头信息
                    header_ms.Write(data, 0, rece);
                    if (header_ms.Length > rece)
                    {
                        data = new byte[header_ms.Length];
                        header_ms.Position = 0;
                        header_ms.Read(data, 0, data.Length);
                    }
                    for (int i = 0; i < header_ms.Length; i++)
                    {
                        if (headLengh > 0)
                            break;
                        char c = (char)data[i];
                        sb.Append(c);
                        if (c == '\n')
                        {
                            var endstr = string.Concat(sb[sb.Length - 4], sb[sb.Length - 3], sb[sb.Length - 2], sb[sb.Length - 1]);
                            if (endstr.Contains("\r\n\r\n"))
                            {
                                headLengh = i + 1;
                                SetHeaders(sb.ToString());
                                byte[] filedata = new byte[Convert.ToInt32(header_ms.Length) - headLengh];
                                header_ms.Position = headLengh;
                                header_ms.Read(filedata, 0, filedata.Length);
                                header_ms.Close();
                                header_ms.Dispose();
                                return filedata;
                            }
                        }
                    }
                }
                
               
            }
            while (rece > 0);

            return null;
        }

        //接收
        private MemoryStream defaultReceive(byte[] filedata)
        {
            int rece = 0;
            MemoryStream file = new MemoryStream();
            StringBuilder sb = new StringBuilder();
            if (filedata != null)
                file.Write(filedata, 0, filedata.Length);
            if (file.Length >= HttpHeaders.ContentLength) return file;
            do
            {
                byte[] data = new byte[102400];
                if (isHttps)
                {
                    rece = ssl.Read(data, 0, data.Length);
                }
                else
                {
                    rece = _socket.Receive(data, 0, data.Length, SocketFlags.None);
                }
                file.Write(data, 0, rece);
                if(file.Length>= HttpHeaders.ContentLength)
                {
                    return file;
                }
            }
            while (rece > 0);
            return file;
        }





        //解压
        private MemoryStream unGzip(MemoryStream data)
        {
            data.Seek(0, SeekOrigin.Begin);
            MemoryStream result = new MemoryStream();
            if (!HttpHeaders.IsGzip) return data;
            GZipStream gs = new GZipStream(data, CompressionMode.Decompress);
            try
            {
                byte[] buffer = new byte[1024*10];
                int length = -1;
                do
                {
                    length = gs.Read(buffer, 0, buffer.Length);
                    result.Write(buffer, 0, length);
                }
                while (length != 0);
                gs.Flush();
                result.Flush();
            }
            finally
            {
                gs.Close();
            }
            return result;
        }

        private void SetHeaders(string headText)
        {
            if (string.IsNullOrWhiteSpace(headText))
            {
                throw new ArgumentNullException("'WithHeadersText' cannot be empty.");
            }

            //Match m = Regex.Match( withHeadersText,@".*(?=\r\n\r\n)",  RegexOptions.Singleline | RegexOptions.IgnoreCase );

            //if ( m == null || string.IsNullOrWhiteSpace( m.Value ) )
            //{
            //    throw new HttpParseException( "'SetThisHeaders' method has bug." );
            //}

            string[] headers = headText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (headers == null || headers.Length == 0)
            {
                throw new ArgumentException("'WithHeadersText' param format error.");
            }
            this.HttpHeaders = new HttpHeader();
            foreach (string head in headers)
            {
                if (head.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
                {
                    string[] ts = head.Split(' ');
                    if (ts.Length > 1)
                    {
                        this.HttpHeaders.ResponseStatusCode = ts[1];
                    }
                }
                else if (head.StartsWith("Set-Cookie:", StringComparison.OrdinalIgnoreCase))
                {
                    this.Cookies = this.Cookies ?? new List<string>();
                    string tCookie = head.Substring(11, head.IndexOf(";") < 0 ? head.Length - 11 : head.IndexOf(";") - 10).Trim();

                    if (!this.Cookies.Exists(f => f.Split('=')[0] == tCookie.Split('=')[0]))
                    {
                        this.Cookies.Add(tCookie);
                    }
                }
                else if (head.StartsWith("Location:", StringComparison.OrdinalIgnoreCase))
                {
                    this.HttpHeaders.Location = head.Substring(9).Trim();
                }
                else if (head.StartsWith("Content-Encoding:", StringComparison.OrdinalIgnoreCase))
                {
                    if (head.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        this.HttpHeaders.IsGzip = true;
                    }
                }
                else if (head.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] types = head.Substring(13).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string t in types)
                    {
                        if (t.IndexOf("charset=", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            this.HttpHeaders.Charset = t.Trim().Substring(8);
                        }
                        else if (t.IndexOf('/') >= 0)
                        {
                            this.HttpHeaders.ContentType = t.Trim();
                        }
                    }
                }
                else if (head.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                {
                    this.HttpHeaders.ContentLength = long.Parse(head.Substring(15).Trim());
                }
                else if (head.StartsWith("Transfer-Encoding:", StringComparison.OrdinalIgnoreCase) && head.EndsWith("chunked", StringComparison.OrdinalIgnoreCase))
                {
                    this.HttpHeaders.IsChunk = true;
                }
            }
        }



        /// <summary>
        /// 取得分块数据的数据长度
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">Socket实例</param>
        /// <param name="data">已读取未处理的字节数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="readLength">读取的长度</param>
        /// <returns>块长度,返回0表示已到末尾.</returns>
        private int GetChunkHead(ref byte[] data, ref int startIndex, ref int readLength)
        {
            int chunkSize = -1;
            List<char> tChars = new List<char>();//用于临时存储块长度字符

            if (startIndex >= data.Length || startIndex >= readLength)
            {
                readLength = this.ReadData(ref data);
                startIndex = 0;
            }

            do
            {
                for (int i = startIndex; i < readLength; i++)
                {
                    char c = (char)data[i];

                    if (c == '\n')
                    {
                        try
                        {
                            chunkSize = Convert.ToInt32(new string(tChars.ToArray()).TrimEnd('\r'), 16);
                            startIndex = i + 1;
                        }
                        catch (Exception e)
                        {
                            L.File.Error("Maybe exists 'chunk-ext' field.", e);
                        }
                        break;
                    }
                    tChars.Add(c);
                }
                if (chunkSize >= 0)
                {
                    break;
                }
                startIndex = 0;
                readLength = this.ReadData(ref data);
            }
            while (readLength > 0);
            return chunkSize;
        }

        /// <summary>
        /// 取得分块传回的数据内容
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">Socket实例</param>
        /// <param name="data">已读取未处理的字节数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="readLength">读取的长度</param>
        /// <param name="body">保存块数据的流</param>
        /// <param name="chunkSize">块长度</param>
        private void GetChunkBody(ref byte[] data, ref int startIndex, ref int readLength, ref MemoryStream body, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                return;
            }

            int chunkReadLength = 0;//每个数据块已读取长度

            if (startIndex >= data.Length || startIndex >= readLength)
            {
                readLength = this.ReadData(ref data);
                startIndex = 0;
            }

            do
            {
                int owing = chunkSize - chunkReadLength;
                int count = Math.Min(readLength - startIndex, owing);

                body.Write(data, startIndex, count);
                chunkReadLength += count;

                if (owing <= count)
                {
                    startIndex += count + 2;
                    break;
                }

                startIndex = 0;
                readLength = this.ReadData(ref data);
            }
            while (readLength > 0);
        }



        /// <summary>
        /// 从数据源读取数据
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">数据源</param>
        /// <param name="data">用于存储读取的数据</param>
        /// <returns>读取的数据长度,无数据为-1</returns>
        private int ReadData(ref byte[] data)
        {
            int result = -1;

            if (isHttps)
            {
                result = ssl.Read(data, 0, data.Length);
            }
            else
            {
                result = _socket.Receive(data, 0, data.Length, SocketFlags.None);
            }

            return result;
        }



        /// <summary>
        /// 取得分块数据
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">Socket实例</param>
        /// <param name="data">已读取未处理的字节数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="readLength">读取的长度</param>
        /// <param name="body">保存块数据的流</param>
        private void GetChunkData(ref byte[] data, ref int startIndex, ref int readLength, ref MemoryStream body)
        {
            int chunkSize = -1;//每个数据块的长度,用于分块数据.当长度为0时,说明读到数据末尾.

            while (true)
            {
                chunkSize = this.GetChunkHead(ref data, ref startIndex, ref readLength);
                this.GetChunkBody(ref data, ref startIndex, ref readLength, ref body, chunkSize);
                if (chunkSize <= 0)
                {
                    break;
                }
            }
        }

    }
}
