using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Console_DotNetCore_CaoLiu.Tool
{
    public class Http_Client2
    {
        /// <summary>
        /// 获取或设置请求与回应的超时时间,默认3秒.
        /// </summary>
        public int TimeOut
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置请求cookie
        /// </summary>
        public List<string> Cookies
        {
            get;
            set;
        }

        /// <summary>
        /// 获取请求返回的 HTTP 头部内容
        /// </summary>
        public HttpHeader HttpHeaders
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取或设置错误信息分隔符
        /// </summary>
        private string ErrorMessageSeparate;



        public Http_Client2()
        {
            this.TimeOut = 5;
            this.Cookies = new List<string>();
            this.ErrorMessageSeparate = ";;";
            this.HttpHeaders = new HttpHeader();
        }



        /// <summary>
        /// get或post方式请求一个 http 或 https 地址.使用 Socket 方式
        /// </summary>
        /// <param name="url">请求绝对地址</param>
        /// <param name="referer">请求来源地址,可为空</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <returns>返回图像</returns>
        public Image GetImageUseSocket(string url, string referer, string postData = null)
        {
            Image result = null;
            MemoryStream ms = this.GetSocketResult(url, referer, postData);

            try
            {
                if (ms != null)
                {
                    result = Image.FromStream(ms);
                }
            }
            catch (Exception e)
            {
                string ss = e.Message;
            }

            return result;
        }

        /// <summary>
        /// get或post方式请求一个 http 或 https 地址.使用 Socket 方式
        /// </summary>
        /// <param name="url">请求绝对地址</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <returns>返回 html 内容,如果发生异常将返回上次http状态码及异常信息</returns>
        public string GetHtmlUseSocket(string url, string postData = null)
        {
            return this.GetHtmlUseSocket(url, null, postData);
        }

        /// <summary>
        /// get或post方式请求一个 http 或 https 地址.使用 Socket 方式
        /// </summary>
        /// <param name="url">请求绝对地址</param>
        /// <param name="referer">请求来源地址,可为空</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <returns>返回 html 内容,如果发生异常将返回上次http状态码及异常信息</returns>
        public string GetHtmlUseSocket(string url, string referer, string postData = null)
        {
            string result = string.Empty;

            try
            {
                MemoryStream ms = this.GetSocketResult(url, referer, postData);

                if (ms != null)
                {
                    result = Encoding.GetEncoding(string.IsNullOrWhiteSpace(this.HttpHeaders.Charset) ? "UTF-8" : this.HttpHeaders.Charset).GetString(ms.ToArray());
                }
            }
            catch (SocketException se)
            {
                result = this.HttpHeaders.ResponseStatusCode + this.ErrorMessageSeparate + se.ErrorCode.ToString() + this.ErrorMessageSeparate + se.SocketErrorCode.ToString("G") + this.ErrorMessageSeparate + se.Message;
            }
            catch (Exception e)
            {
                result = this.HttpHeaders.ResponseStatusCode + this.ErrorMessageSeparate + e.Message;
            }

            return result;
        }

        /// <summary>
        /// get或post方式请求一个 http 或 https 地址.
        /// </summary>
        /// <param name="url">请求绝对地址</param>
        /// <param name="referer">请求来源地址,可为空</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <returns>返回的已解压的数据内容</returns>
        public MemoryStream GetSocketResult(string url, string referer, string postData)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new UriFormatException("'Url' cannot be empty.");
            }

            MemoryStream result = null;
            Uri uri = new Uri(url);

            if (uri.Scheme == "http")
            {
                result = this.GetHttpResult(uri, referer, postData);
            }
            else if (uri.Scheme == "https")
            {
                result = this.GetSslResult(uri, referer, postData);
            }
            else
            {
                throw new ArgumentException("url must start with HTTP or HTTPS.", "url");
            }

            if (!string.IsNullOrWhiteSpace(this.HttpHeaders.Location))
            {
                result = GetSocketResult(this.HttpHeaders.Location, uri.AbsoluteUri, null);
            }
            else
            {
                result = unGzip(result);
            }

            return result;
        }

        /// <summary>
        /// get或post方式请求一个 http 地址.
        /// </summary>
        /// <param name="uri">请求绝对地址</param>
        /// <param name="referer">请求来源地址,可为空</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <param name="headText">输出包含头部内容的StringBuilder</param>
        /// <returns>返回未解压的数据流</returns>
        private MemoryStream GetHttpResult(Uri uri, string referer, string postData)
        {
            MemoryStream result = new MemoryStream(10240);
            Socket HttpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            HttpSocket.SendTimeout = this.TimeOut * 1000;
            HttpSocket.ReceiveTimeout = this.TimeOut * 1000;

            try
            {
                byte[] send = GetSendHeaders(uri, referer, postData);
                HttpSocket.Connect(uri.Host, uri.Port);

                if (HttpSocket.Connected)
                {
                    HttpSocket.Send(send, SocketFlags.None);
                    this.ProcessData(HttpSocket, ref result);
                }

                result.Flush();
            }
            finally
            {
                HttpSocket.Shutdown(SocketShutdown.Both);
                HttpSocket.Close();
            }

            result.Seek(0, SeekOrigin.Begin);

            return result;
        }

        /// <summary>
        /// get或post方式请求一个 https 地址.
        /// </summary>
        /// <param name="uri">请求绝对地址</param>
        /// <param name="referer">请求来源地址,可为空</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <param name="headText">输出包含头部内容的StringBuilder</param>
        /// <returns>返回未解压的数据流</returns>
        private MemoryStream GetSslResult(Uri uri, string referer, string postData)
        {
            MemoryStream result = new MemoryStream(10240);
            StringBuilder sb = new StringBuilder(1024);

            byte[] send = GetSendHeaders(uri, referer, postData);
            TcpClient client = new TcpClient(uri.Host, uri.Port);

            try
            {
                SslStream sslStream = new SslStream(client.GetStream(), true
                    , new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors)
                       => {
                           return sslPolicyErrors == SslPolicyErrors.None;
                       }
                        ), null);
                sslStream.ReadTimeout = this.TimeOut * 1000;
                sslStream.WriteTimeout = this.TimeOut * 1000;

                X509Store store = new X509Store(StoreName.My);

                sslStream.AuthenticateAsClient(uri.Host, store.Certificates, System.Security.Authentication.SslProtocols.Default, false);

                if (sslStream.IsAuthenticated)
                {
                    sslStream.Write(send, 0, send.Length);
                    sslStream.Flush();

                    this.ProcessData(sslStream, ref result);
                }

                result.Flush();
            }
            finally
            {
                client.Close();
            }

            result.Seek(0, SeekOrigin.Begin);

            return result;
        }

        /// <summary>
        /// 返回请求的头部内容
        /// </summary>
        /// <param name="uri">请求绝对地址</param>
        /// <param name="referer">请求来源地址,可为空</param>
        /// <param name="postData">post请求参数. 设置空值为get方式请求</param>
        /// <returns>请求头部数据</returns>
        private byte[] GetSendHeaders(Uri uri, string referer, string postData)
        {
            string sendString = @"{0} {1} HTTP/1.1
Accept: text/html, application/xhtml+xml, */*
Referer: {2}
Accept-Language: zh-CN
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)
Accept-Encoding: gzip, deflate
Host: {3}
Connection: Keep-Alive
Cache-Control: no-cache
";

            sendString = string.Format(sendString, string.IsNullOrWhiteSpace(postData) ? "GET" : "POST", uri.PathAndQuery
                , string.IsNullOrWhiteSpace(referer) ? uri.AbsoluteUri : referer, uri.Host);

            if (this.Cookies != null && this.Cookies.Count > 0)
            {
                sendString += string.Format("Cookie: {0}\r\n", string.Join("; ", this.Cookies.ToArray()));
            }

            if (string.IsNullOrWhiteSpace(postData))
            {
                sendString += "\r\n";
            }
            else
            {
                int dlength = Encoding.UTF8.GetBytes(postData).Length;

                sendString += string.Format(@"Content-Type: application/x-www-form-urlencoded
Content-Length: {0}

{1}
", postData.Length, postData);
            }

            return Encoding.UTF8.GetBytes(sendString);
            ;
        }

        /// <summary>
        /// 设置此类的字段
        /// </summary>
        /// <param name="headText">头部文本</param>
        private void SetThisHeaders(string headText)
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
        /// 解压数据流
        /// </summary>
        /// <param name="data">数据流, 压缩或未压缩的.</param>
        /// <returns>返回解压缩的数据流</returns>
        private MemoryStream unGzip(MemoryStream data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.", "data");
            }

            data.Seek(0, SeekOrigin.Begin);
            MemoryStream result = data;

            if (this.HttpHeaders.IsGzip)
            {
                GZipStream gs = new GZipStream(data, CompressionMode.Decompress);
                result = new MemoryStream(1024);

                try
                {
                    byte[] buffer = new byte[1024];
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
            }

            return result;
        }


        /// <summary>
        /// 处理请求返回的数据.
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">数据源实例</param>
        /// <param name="body">保存数据的流</param>
        private void ProcessData<T>(T reader, ref MemoryStream body)
        {
            byte[] data = new byte[10240];
            int bodyStart = -1;//数据部分起始位置
            int readLength = 0;

            bodyStart = GetHeaders(reader, ref data, ref readLength);

            if (bodyStart >= 0)
            {
                if (this.HttpHeaders.IsChunk)
                {
                    GetChunkData(reader, ref data, ref bodyStart, ref readLength, ref body);
                }
                else
                {
                    GetBodyData(reader, ref data, bodyStart, readLength, ref body);
                }
            }
        }

        /// <summary>
        /// 取得返回的http头部内容,并设置相关属性.
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">数据源实例</param>
        /// <param name="data">待处理的数据</param>
        /// <param name="readLength">读取的长度</param>
        /// <returns>数据内容的起始位置,返回-1表示未读完头部内容</returns>
        private int GetHeaders<T>(T reader, ref byte[] data, ref int readLength)
        {
            int result = -1;
            StringBuilder sb = new StringBuilder(1024);

            do
            {
                readLength = this.ReadData(reader, ref data);

                if (result < 0)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        char c = (char)data[i];
                        sb.Append(c);

                        if (c == '\n' && string.Concat(sb[sb.Length - 4], sb[sb.Length - 3], sb[sb.Length - 2], sb[sb.Length - 1]).Contains("\r\n\r\n"))
                        {
                            result = i + 1;
                            this.SetThisHeaders(sb.ToString());
                            break;
                        }
                    }
                }

                if (result >= 0)
                {
                    break;
                }
            }
            while (readLength > 0);

            return result;
        }

        /// <summary>
        /// 取得未分块数据的内容
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="reader">数据源实例</param>
        /// <param name="data">已读取未处理的字节数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="readLength">读取的长度</param>
        /// <param name="body">保存块数据的流</param>
        private void GetBodyData<T>(T reader, ref byte[] data, int startIndex, int readLength, ref MemoryStream body)
        {
            int contentTotal = 0;

            if (startIndex < data.Length)
            {
                int count = readLength - startIndex;
                body.Write(data, startIndex, count);
                contentTotal += count;
            }

            int tlength = 0;

            do
            {
                tlength = this.ReadData(reader, ref data);
                contentTotal += tlength;
                body.Write(data, 0, tlength);

                if (this.HttpHeaders.ContentLength > 0 && contentTotal >= this.HttpHeaders.ContentLength)
                {
                    break;
                }
            }
            while (tlength > 0);

            if (body.Length != 290576)
            {
                Console.WriteLine();
            }
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
        private void GetChunkData<T>(T reader, ref byte[] data, ref int startIndex, ref int readLength, ref MemoryStream body)
        {
            int chunkSize = -1;//每个数据块的长度,用于分块数据.当长度为0时,说明读到数据末尾.

            while (true)
            {
                chunkSize = this.GetChunkHead(reader, ref data, ref startIndex, ref readLength);
                this.GetChunkBody(reader, ref data, ref startIndex, ref readLength, ref body, chunkSize);

                if (chunkSize <= 0)
                {
                    break;
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
        private int GetChunkHead<T>(T reader, ref byte[] data, ref int startIndex, ref int readLength)
        {
            int chunkSize = -1;
            List<char> tChars = new List<char>();//用于临时存储块长度字符

            if (startIndex >= data.Length || startIndex >= readLength)
            {
                readLength = this.ReadData(reader, ref data);
                startIndex = 0;
            }

            do
            {
                for (int i = startIndex; i < readLength; i++)
                {
                    char c = (char)data[i];
                    Console.Write( "   ({0})   ", data[i]);
                    if (c == '\n')
                    {
                        try
                        {
                            chunkSize = Convert.ToInt32(new string(tChars.ToArray()).TrimEnd('\r'), 16);
                            startIndex = i + 1;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Maybe exists 'chunk-ext' field.", e);
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
                readLength = this.ReadData(reader, ref data);
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
        private void GetChunkBody<T>(T reader, ref byte[] data, ref int startIndex, ref int readLength, ref MemoryStream body, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                return;
            }

            int chunkReadLength = 0;//每个数据块已读取长度

            if (startIndex >= data.Length || startIndex >= readLength)
            {
                readLength = this.ReadData(reader, ref data);
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
                readLength = this.ReadData(reader, ref data);
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
        private int ReadData<T>(T reader, ref byte[] data)
        {
            int result = -1;

            if (reader is Socket)
            {
                result = (reader as Socket).Receive(data, SocketFlags.None);
            }
            else if (reader is SslStream)
            {
                result = (reader as SslStream).Read(data, 0, data.Length);
            }

            return result;
        }
    }

    public class HttpHeader
    {
        /// <summary>
        /// 获取请求回应状态码
        /// </summary>
        public string ResponseStatusCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取跳转url
        /// </summary>
        public string Location
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取是否由Gzip压缩
        /// </summary>
        public bool IsGzip
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取返回的文档类型
        /// </summary>
        public string ContentType
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取内容使用的字符集
        /// </summary>
        public string Charset
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取内容长度
        /// </summary>
        public long ContentLength
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取是否分块传输
        /// </summary>
        public bool IsChunk
        {
            get;
            internal set;
        }








    }
}
