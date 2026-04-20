using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Linq;
using System.Collections;

namespace Net_Ease_Cloud_Form_API
{
    /// <summary>
    /// Http帮助类
    /// </summary>
    public class HttpHelper
    {
        private static Random rand = new Random((int)DateTime.Now.ToFileTimeUtc());
        private static string userAgent = string.Empty;
        private static IWebProxy proxy = null;

        private CookieContainer cc;

        /// <summary>
        /// 返回响应的Header
        /// </summary>
        public WebHeaderCollection response_header = null;
        private List<string> agentList = new List<string>();
        private List<IWebProxy> proxyList = new List<IWebProxy>();

        public CookieContainer CC
        {
            get
            {
                return cc;
            }
            set
            {
                this.cc = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpHelper()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            userAgent = UserAgent();
            proxy = Proxy();
            this.cc = new CookieContainer();
        }

        /// <summary>
        /// 通过cookie初始化
        /// </summary>
        /// <param name="cc"></param>
        public HttpHelper(CookieContainer cc)
        {
            userAgent = UserAgent();
            proxy = Proxy();
            this.cc = cc;
        }

        /// <summary>
        /// 使用post方式访问目标网页，返回stream二进制流  
        /// </summary>
        /// <param name="targetURL">请求地址</param>
        /// <param name="formData">POST内容</param>
        /// <param name="contentType">默认 application/x-www-form-urlencoded</param>
        /// <param name="referer"></param>
        /// <param name="allowAutoRedirect">是否自动处理302重定向处理</param>
        /// <param name="encoding2">请求数据 默认 ASCII</param>
        /// <param name="headers">header</param>
        /// <returns></returns>
        public Stream PostAndGetStream(string targetURL, string formData, string contentType, string referer, bool allowAutoRedirect, Encoding encoding2 = null, Dictionary<string, string> headers = null)
        {
            byte[] data = null;
            if (encoding2 != null)
            {
                data = encoding2.GetBytes(formData);
            }
            else
            {
                //数据编码
                ASCIIEncoding encoding = new ASCIIEncoding();
                //UTF8Encoding encoding = new UTF8Encoding();
                data = encoding.GetBytes(formData);
            }

            //请求目标网页
            HttpWebRequest request = null;
            if (targetURL.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                request = WebRequest.Create(targetURL) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.Expect100Continue = true;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(targetURL);
            }

            request.CookieContainer = cc;
            request.Method = "POST";    //使用post方式发送数据
            if (string.IsNullOrEmpty(contentType))
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            else
            {
                request.ContentType = contentType;
            }

            request.KeepAlive = false;
            request.Referer = referer;
            request.AllowAutoRedirect = allowAutoRedirect;
            request.ContentLength = data.Length;
            //WebProxy proxy = new WebProxy("127.0.0.1", 8888);
            //request.Proxy = proxy;

            request.UserAgent = userAgent;
            //request.Proxy = proxy ;
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    if (item.Key.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Accept = item.Value;
                    }
                    else if (item.Key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))
                    {
                        request.UserAgent = item.Value;
                    }
                    else if (item.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Host = item.Value;
                    }
                    else
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }
            }

            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response_header = response.Headers;
            cc.Add(response.Cookies);

            Stream stream = response.GetResponseStream();
            return stream;
        }

        ///<summary>
        /// 使用post方式访问目标网页，返回字节数组
        ///</summary>
        public byte[] PostAndGetByte(string targetURL, string formData, string contentType, string referer, bool allowAutoRedirect)
        {
            Stream stream = PostAndGetStream(targetURL, formData, contentType, referer, allowAutoRedirect);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        ///<summary>
        /// 使用post方式访问目标网页，返回图片
        ///</summary>
        public System.Drawing.Image PostAndGetBitmap(string targetURL, string formData, string contentType, string referer, bool allowAutoRedirect)
        {
            Stream stream = PostAndGetStream(targetURL, formData, contentType, referer, allowAutoRedirect);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            return image;
        }

        ///<summary>
        /// 使用post方式访问目标网页，返回文件
        ///</summary>
        public void PostAndGetFile(string targetURL, string formData, string contentType, string referer, bool allowAutoRedirect, string fileName)
        {
            byte[] bytes = PostAndGetByte(targetURL, formData, contentType, referer, allowAutoRedirect);
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes);
                    bw.Close();
                }

                fs.Close();
            }
        }
        /// <summary>
        /// 下载线上文件到本地
        /// </summary>
        /// <param name="URL">下载文件链接</param>
        /// <param name="filename">保存到本地的地址</param>
        /// <returns></returns>
        public void PostAndGetFile(string URL, string formData, string contentType, string referer, string filename)
        {
            //数据编码
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = null;
            data = encoding.GetBytes(formData);
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            Stream st = null;
            Stream so = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            req = (HttpWebRequest)WebRequest.Create(URL);//请求网络资源
            req.UserAgent = userAgent;
            //req.Proxy = proxy;
            req.ContentType = contentType;
            req.Referer = referer;
            req.Timeout = 60 * 60 * 1000;//一个小时的等待时间
            req.Method = "POST";    //使用post方式发送数据
            req.CookieContainer = cc;
            List<Cookie> cookies = GetAllCookies(cc);
            CC = cc;
            Stream newStream = req.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            rep = (HttpWebResponse)req.GetResponse();//返回Internet资源的响应
            long totalBytes = rep.ContentLength;//获取请求返回内容的长度
            st = rep.GetResponseStream();//读取服务器的响应资源，以IO流的形式进行读写
            cc.Add(rep.Cookies);
            string cs = ReadStream(st, Encoding.UTF8);//测试用
            so = new FileStream(filename, FileMode.Create);
            long totalDownloadedByte = 0;
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, (int)by.Length);
            while (osize > 0)
            {
                totalDownloadedByte = osize + totalDownloadedByte;
                so.Write(by, 0, osize);
                osize = st.Read(by, 0, (int)by.Length);//读取当前字节流的总长度
            }
            so.Flush();
            so.Close();
        }

        #region post带参数的请求
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// Post是从服务器上传送数据
        /// </summary>
        /// <param name="url">请求Url地址</param>
        /// <param name="dic">拼接Url的字段</param>
        /// <returns></returns>
        public string Post(string url, Dictionary<string, string> dic)
        {
            #region 【网上查的】
            //eg：http://IP:端口/AAA/BBB/CCC//发送对应参数
            #region 创建Web访问对象
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.AutomaticDecompression = DecompressionMethods.GZip;
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            List<Cookie> cookies = GetAllCookies(cc);
            req.CookieContainer = cc;
            #endregion

            #region Post添加并拼接参数，形成对应Url地址
            StringBuilder builder = new StringBuilder();
            int i = 0;
            if (dic.Count > 0)
            {
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            #endregion

            #region 发送请求
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            #region 通过Web访问对象获取响应内容
            string result = "";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
            #endregion
            #endregion
        }
        #endregion


        ///<summary>
        /// 使用post方式访问目标网页，返回html页面，参数：vcode=1&note_id=2&file_id=3
        ///</summary>
        public string PostAndGetHtml(string targetURL, string formData, string contentType, string referer, bool allowAutoRedirect, Encoding encoding, Encoding encoding2 = null, Dictionary<string, string> headers = null)
        {
            Stream stream = PostAndGetStream(targetURL, formData, contentType, referer, allowAutoRedirect, encoding2, headers);

            return ReadStream(stream, encoding);
        }

        ///<summary>
        /// 使用get方式访问目标网页，返回stream二进制流
        ///</summary>
        public Stream GetAndGetStream(string targetURL, string contentType, string referer, bool allowAutoRedirect, bool IsHttpOnly, Dictionary<string, string> headers = null)
        {
            //请求目标网页
            HttpWebRequest request = null;
            if (targetURL.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;//设置这个安全协议必须在创建请求之前！
                request = WebRequest.Create(targetURL) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.Expect100Continue = true;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(targetURL);
            }
            request.CookieContainer = cc;
            CC = cc;
            request.Method = "GET";    //使用get方式发送数据
            request.ContentType = contentType;
            request.Referer = referer;
            request.AllowAutoRedirect = allowAutoRedirect;
            request.UserAgent = userAgent;
            //request.Proxy = proxy;
            request.Accept = "*/*";
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    if (item.Key.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Accept = item.Value;
                    }
                    else if (item.Key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))
                    {
                        userAgent = item.Value;
                    }
                    else if (item.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Host = item.Value;
                    }
                    else
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }
            }
            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (IsHttpOnly)
            {
                if (response.Headers.ToString().Contains("Set-Cookie"))
                {
                    SetCC(response.Headers.GetValues("Set-Cookie").ToList(), response.ResponseUri.Host);//保存该异步的cookie
                }
            }
            else
            {
                cc.Add(response.Cookies);
            }
            response_header = response.Headers;

            Stream stream = response.GetResponseStream();
            return stream;
        }
        /// <summary>
        /// 获取剩下的Cookie C#无法获取全部的cookie值，在谷歌浏览器中。能看到。C#无法获取到全部Cookie
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public List<string> GetHttpCookies(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            var response = (HttpWebResponse)request.GetResponse();
            var resultAsync = request.GetResponseAsync();
            return resultAsync.Result.Headers.GetValues("Set-Cookie").ToList();
        }
        /// <summary>
        /// 将List集合中的数据添加至Cookie
        /// </summary>
        /// <param name="cookies">cookie集合</param>
        /// <param name="domain">域</param>
        public void SetCC(List<string> cookies, string domain)
        {
            foreach (string cookie in cookies)
            {
                string[] strs = cookie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                //JSESSIONID=916AD4B7E25448F2B9D5A899AA9E3B03; Path=/;HttpOnly
                //netcut_session=anchab6ooh0o67ddjph1aqlj1nrc8auo; expires=Mon, 12-Sep-2022 02:37:44 GMT; Max-Age=7200; path=/; HttpOnly
                foreach (string str in strs)
                {
                    string[] ss = str.Split(new char[] { '=' }, 2);
                    if (ss.Length == 2)
                    {
                        CC.Add(new System.Net.Cookie()
                        {
                            Name = ss[0].Trim(),
                            Value = ss[1].Trim().Replace(",", "%2C"),
                            Domain = domain,//域
                            Expires = DateTime.Now.AddDays(30)
                        });
                    }
                }
            }
        }
        ///<summary>
        /// 使用get方式访问目标网页，返回字节数组
        ///</summary>
        public byte[] GetAndGetByte(string targetURL, string contentType, string referer, bool allowAutoRedirect, bool IsHttpOnly)
        {
            Stream stream = GetAndGetStream(targetURL, contentType, referer, allowAutoRedirect, IsHttpOnly);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        ///<summary>
        /// 使用get方式访问目标网页，返回图片
        ///</summary>
        public System.Drawing.Image GetAndGetBitmap(string targetURL, string contentType, string referer, bool allowAutoRedirect, bool IsHttpOnly)
        {
            Stream stream = GetAndGetStream(targetURL, contentType, referer, true, IsHttpOnly);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            return image;
        }

        ///<summary>
        /// 使用get方式访问目标网页，返回文件
        ///</summary>
        public void GetAndGetFile(string targetURL, string contentType, string referer, bool allowAutoRedirect, string fileName, bool IsHttpOnly)
        {
            byte[] bytes = GetAndGetByte(targetURL, contentType, referer, allowAutoRedirect, IsHttpOnly);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// 下载线上文件到本地
        /// </summary>
        /// <param name="URL">下载文件链接</param>
        /// <param name="filename">保存到本地的地址</param>
        /// <returns></returns>
        public void GetAndGetFile(string URL, string contentType, string referer, string filename)
        {
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            Stream st = null;
            Stream so = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            req = (HttpWebRequest)WebRequest.Create(URL);//请求网络资源
            req.UserAgent = userAgent;
            //req.Proxy = proxy;
            req.ContentType = contentType;
            req.Referer = referer;
            req.Timeout = 60 * 60 * 1000;//一个小时的等待时间
            rep = (HttpWebResponse)req.GetResponse();//返回Internet资源的响应
            long totalBytes = rep.ContentLength;//获取请求返回内容的长度
            st = rep.GetResponseStream();//读取服务器的响应资源，以IO流的形式进行读写
            so = new FileStream(filename, FileMode.Create);
            long totalDownloadedByte = 0;
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, (int)by.Length);
            while (osize > 0)
            {
                totalDownloadedByte = osize + totalDownloadedByte;
                so.Write(by, 0, osize);
                osize = st.Read(by, 0, (int)by.Length);//读取当前字节流的总长度
            }
            so.Flush();
            so.Close();
        }
        /// <summary>
        /// 异步下载线上文件到本地
        /// </summary>
        /// <param name="URL">下载文件链接</param>
        /// <param name="filename">保存到本地的地址</param>
        /// <returns></returns>
        public void GetAndGetFileAsync(string URL, string contentType, string referer, string filename)
        {
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            Stream st = null;
            Stream so = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            req = (HttpWebRequest)WebRequest.Create(URL);//请求网络资源
            req.UserAgent = userAgent;
            //req.Proxy = proxy;
            req.ContentType = contentType;
            req.Referer = referer;
            req.Timeout = 60 * 60 * 1000;//一个小时的等待时间
            rep = (HttpWebResponse)req.GetResponseAsync().Result;//异步返回Internet资源的响应

            SetCC(rep.Headers.GetValues("Set-Cookie").ToList(), rep.ResponseUri.Host);//保存该异步的cookie

            long totalBytes = rep.ContentLength;//获取请求返回内容的长度
            st = rep.GetResponseStream();//读取服务器的响应资源，以IO流的形式进行读写
            so = new FileStream(filename, FileMode.Create);
            long totalDownloadedByte = 0;
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, (int)by.Length);
            while (osize > 0)
            {
                totalDownloadedByte = osize + totalDownloadedByte;
                so.Write(by, 0, osize);
                osize = st.Read(by, 0, (int)by.Length);//读取当前字节流的总长度
            }
            so.Flush();
            so.Close();
        }
        ///<summary>
        /// 使用get方式访问目标网页，返回html页面
        ///</summary>
        public string GetAndGetHtml(string targetURL, string contentType, string referer, bool allowAutoRedirect, Encoding encoding, bool IsHttpOnly, Dictionary<string, string> headers = null)
        {
            Stream stream = GetAndGetStream(targetURL, contentType, referer, allowAutoRedirect, IsHttpOnly, headers);

            return ReadStream(stream, encoding);
        }

        /// <summary>
        /// Post表单文件上传
        /// </summary>
        /// <param name="url">上传地址</param>
        /// <param name="file">文件路径</param>
        /// <param name="uploadName">上传文件的File控件名称</param>
        /// <param name="param">kevvalue参数值</param>
        public string PostUpload(string url, string file, string uploadName, NameValueCollection param)
        {
            string boundary = DateTime.Now.Ticks.ToString("x");
            HttpWebRequest uploadRequest = (HttpWebRequest)WebRequest.Create(url);//url为上传的地址
            uploadRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            uploadRequest.Method = "POST";
            uploadRequest.Accept = "*/*";
            uploadRequest.KeepAlive = true;
            uploadRequest.Headers.Add("Accept-Language", "zh-cn");
            uploadRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            uploadRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            uploadRequest.CookieContainer = cc;
            HttpWebResponse reponse;

            //创建一个内存流
            Stream memStream = new MemoryStream();

            //确定上传的文件路径
            if (!String.IsNullOrEmpty(file))
            {
                boundary = "--" + boundary;

                //添加上传文件参数格式边界
                string paramFormat = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}\r\n";

                //写上参数
                foreach (string key in param.Keys)
                {
                    string formitem = string.Format(paramFormat, key, param[key]);

                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);

                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }

                //添加上传文件数据格式边界
                string dataFormat = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n";

                string header = string.Format(dataFormat, uploadName, Path.GetFileName(file));

                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                //获取文件内容
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                //将文件内容写进内存流
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();

                //添加文件结束边界
                byte[] boundarybytes = System.Text.Encoding.UTF8.GetBytes("\r\n\n" + boundary + "\r\nContent-Disposition: form-data; name=\"Upload\"\r\n\nSubmit Query\r\n" + boundary + "--");

                memStream.Write(boundarybytes, 0, boundarybytes.Length);

                //设置请求长度
                uploadRequest.ContentLength = memStream.Length;

                //获取请求写入流
                Stream requestStream = uploadRequest.GetRequestStream();

                //将内存流数据读取位置归零
                memStream.Position = 0;

                byte[] tempBuffer = new byte[memStream.Length];

                memStream.Read(tempBuffer, 0, tempBuffer.Length);

                memStream.Close();

                //将内存流中的buffer写入到请求写入流
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);

                requestStream.Close();
            }

            //获取到上传请求的响应
            reponse = (HttpWebResponse)uploadRequest.GetResponse();
            cc.Add(reponse.Cookies);

            return new StreamReader(reponse.GetResponseStream()).ReadToEnd();
        }

        ///<summary>
        /// 使用get方式访问目标网页，返回stream二进制流
        ///</summary>
        public Stream HeadAndGetStream(string targetURL, string contentType, string referer, bool allowAutoRedirect)
        {
            //请求目标网页
            HttpWebRequest request = null;
            if (targetURL.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(targetURL) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(targetURL);
            }

            request.CookieContainer = cc;
            CC = cc;
            request.Method = "HEAD";    //使用get方式发送数据
            request.ContentType = contentType;
            request.Referer = referer;
            request.AllowAutoRedirect = allowAutoRedirect;
            request.UserAgent = userAgent;
            //request.Proxy = proxy;

            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            cc.Add(response.Cookies);
            response_header = response.Headers;

            Stream stream = response.GetResponseStream();
            return stream;
        }

        /// <summary>
        /// head请求
        /// </summary>
        /// <param name="targetURL"></param>
        /// <param name="contentType"></param>
        /// <param name="referer"></param>
        /// <param name="allowAutoRedirect"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string HeadAndGetHtml(string targetURL, string contentType, string referer, bool allowAutoRedirect, Encoding encoding)
        {
            Stream stream = HeadAndGetStream(targetURL, contentType, referer, allowAutoRedirect);

            return ReadStream(stream, encoding);
        }

        /// <summary>
        /// 带gzip解密的流转字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string ReadStream(Stream stream, Encoding encoding)
        {
            //StreamReader myStreamReader = new StreamReader(stream, encoding);
            //string retString = myStreamReader.ReadToEnd();//读取响应的json
            //return retString;
            MemoryStream ms = StreamToMemoryStream(stream);
            byte[] b = ms.ToArray();
            string s = encoding.GetString(b, 0, b.Length);
            return s;
            BinaryReader br = new BinaryReader(stream);
            byte[] read = br.ReadBytes((int)stream.Length);
            ms.Read(read, 0, read.Length);
            if (read.Length >= 4)
            {
                // 1f 8b 08 00
                if (read[0] == 0x1f && read[1] == 0x8b && read[2] == 0x08 && read[3] == 0x0)
                {
                    // gzip压缩
                    try
                    {
                        using (GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (var resultStream = new MemoryStream())
                            {
                                gzip.CopyTo(resultStream);

                                return encoding.GetString(resultStream.ToArray());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return encoding.GetString(read);
                    }
                }
            }

            return encoding.GetString(read);
        }
        public MemoryStream StreamToMemoryStream(Stream instream)
        {
            MemoryStream outstream = new MemoryStream();
            const int bufferLen = 4096;
            byte[] buffer = new byte[1024 * 1024 * 5];
            int count = 0;
            while ((count = instream.Read(buffer, 0, bufferLen)) > 0)//Read和Write会偏移所以offset不需要变
            {
                outstream.Write(buffer, 0, count);
            }
            return outstream;
        }

        /// <summary>
        /// 检查类型结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        private string UserAgent()
        {
            // list.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)");
            // list.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)");
            // agentList.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
            // agentList.Add("ChinaMobile/7.1.0 (iPhone; iOS 9.2.1; Scale/2.00)");
            // list.Add("ChinaUnicom4.x/1000 CFNetwork/758.2.8 Darwin/15.0.0");
            //agentList.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");
            agentList.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36");
            agentList.Add("Mozilla / 5.0(Linux; Android 4.1.1; Nexus 7 Build / JRO03D) AppleWebKit / 535.19(KHTML, like Gecko) Chrome / 18.0.1025.166 Safari / 535.19");
            agentList.Add("Mozilla / 5.0(Linux; U; Android 4.0.4; en - gb; GT - I9300 Build / IMM76D) AppleWebKit / 534.30(KHTML, like Gecko) Version / 4.0 Mobile Safari/ 534.30");
            agentList.Add("Mozilla / 5.0(Linux; U; Android 2.2; en - gb; GT - P1000 Build / FROYO) AppleWebKit / 533.1(KHTML, like Gecko) Version / 4.0 Mobile Safari/ 533.1");
            agentList.Add("Mozilla / 5.0(Windows NT 6.2; WOW64; rv: 21.0) Gecko / 20100101 Firefox / 21.0");
            agentList.Add("Mozilla/5.0 (Android; Mobile; rv:14.0) Gecko/14.0 Firefox/14.0");
            agentList.Add("Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36");
            agentList.Add("Mozilla / 5.0(Linux; Android 4.0.4; Galaxy Nexus Build / IMM76B) AppleWebKit / 535.19(KHTML, like Gecko) Chrome / 18.0.1025.133 Mobile Safari/ 535.19");
            agentList.Add("Mozilla/5.0 (iPad; CPU OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3");
            agentList.Add("Mozilla/5.0 (iPod; U; CPU like Mac OS X; en) AppleWebKit/420.1 (KHTML, like Gecko) Version/3.0 Mobile/3A101a Safari/419.3");

            return agentList[rand.Next(0, agentList.Count)];
        }
        private IWebProxy Proxy()
        {
            proxyList.Add(new WebProxy("47.101.44.122", 80));
            proxyList.Add(new WebProxy("116.63.93.172", 8081));
            proxyList.Add(new WebProxy("42.180.208.43", 8070));
            proxyList.Add(new WebProxy("60.255.151.82", 80));
            proxyList.Add(new WebProxy("218.2.214.107", 80));
            proxyList.Add(new WebProxy("116.117.134.135", 9999));
            proxyList.Add(new WebProxy("202.108.22.5", 80));
            proxyList.Add(new WebProxy("45.64.22.24", 80));
            proxyList.Add(new WebProxy("180.97.34.35", 80));
            proxyList.Add(new WebProxy("112.80.248.73", 80));
            proxyList.Add(new WebProxy("116.117.134.135", 8828));
            proxyList.Add(new WebProxy("47.92.234.75", 80));
            proxyList.Add(new WebProxy("203.74.120.79", 3128));
            proxyList.Add(new WebProxy("60.255.151.81", 80));
            proxyList.Add(new WebProxy("39.106.223.134", 80));
            proxyList.Add(new WebProxy("222.74.202.229", 80));
            proxyList.Add(new WebProxy("58.240.52.114", 80));
            //return proxyList[rand.Next(0, proxyList.Count)];
            return null;
        }

        /// <summary>
        /// 设置ua，默认
        /// </summary>
        /// <param name="ua"></param>
        public void SetUA(string ua)
        {
            userAgent = ua;
        }

        /// <summary>
        /// 填充cookie
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="cookie"></param>
        public void FillCookie(string domain, string cookie)
        {
            var array = cookie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array)
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    string[] itemArray = item.Trim().Split(new char[] { '=' }, 2);
                    if (itemArray.Length == 2)
                    {
                        CC.Add(new Cookie()
                        {
                            Name = itemArray[0],
                            Value = itemArray[1].Replace(",", "%2C"),
                            Domain = domain,
                            Expires = DateTime.Now.AddDays(30)
                        });
                    }
                }
            }
        }
        public List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            StringBuilder sb = new StringBuilder();
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        lstCookies.Add(c);
                        sb.AppendLine(c.Domain + ":" + c.Name + "____" + c.Value + "\r\n");
                    }
            }
            return lstCookies;
        }

        /// <summary>
        /// 获取重定向后的URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetWeb302URL(string url)
        {
            HttpWebRequest h = (HttpWebRequest)HttpWebRequest.Create(url);
            h.AllowAutoRedirect = false;
            HttpWebResponse r = (HttpWebResponse)h.GetResponse();
            //判断是否重定向  Ambiguous 300  Found 302  Moved 301 
            if (r.StatusCode == HttpStatusCode.Ambiguous || r.StatusCode == HttpStatusCode.Found || r.StatusCode == HttpStatusCode.Moved)
            {
                string newUrl = r.Headers["Location"];//获取重定向的网址
                if (!string.IsNullOrEmpty(newUrl))
                {
                    //此处做你的处理

                    r.Close();
                    //获取源码
                    return newUrl;
                }
                else
                {
                    return url;
                }
            }
            else
            {
                r.Close();
                return url;
            }

        }
    }
}
