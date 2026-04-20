using Pic_API_Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Net_Ease_Cloud_Form_API
{
    class Common
    {
        /// <summary>
        /// Http的Get请求
        /// </summary>
        /// <param name="Url">请求的URL网址路径</param>
        /// <param name="urlStr">参数(/login/cellphone?phone=15573313867)</param>
        /// <returns></returns>
        public static string HttpGet(string Url, string urlStr, string cookies)
        {
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + urlStr);
            request.Method = "GET";
            //request.ContentType = "text/html;charset=UTF-8";
            request.KeepAlive = true;
            request.AllowAutoRedirect = false;
            if (cookies != null)//Cookies不为空时（登陆后）添加到请求头
            {
                request.Headers.Add("Cookie:" + cookies);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //if (Cookies == null)//第一次登陆时将Cookies赋值
            //{
            //    Cookies = response.GetResponseHeader("Set-Cookie");
            //}
            Stream myResponseStream = response.GetResponseStream();
            SaveFileWithType(response.ContentType, "", myResponseStream);//根据类型下载文件
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();//读取响应的json
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        /// <summary>
        /// 用于保存图片流
        /// </summary>
        /// <param name="Url">路径</param>
        /// <param name="FirstName">文件前缀名称</param>
        public static void HttpGet(string Url, string FirstName = "", PictureBox pictureBox2 = null)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            SaveImgFromStream(myResponseStream, FirstName, pictureBox2);
        }
        public static void HttpGet(KeyValuePair<string, string> item)
        {
            try
            {
                ServicePointManager.DefaultConnectionLimit = 512;//最大并发数位512
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(item.Value);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 60 * 60 * 1000;//设置超时前等待时间为一个小时
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                SaveFileWithType(response.ContentType, item.Key, stream);//根据类型下载文件

                //特别要注意的地方 - request和response是同时使用的，如果Request.Abort的话，那么Response读取值会报错，所以request取消时要在response完成之后
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"名称：{item.Key}，URL：{item.Value}，异常：{ex.Message}");
            }
        }
        /// <summary>
        /// 获取存放的路径
        /// </summary>
        /// <param name="folder">要存放的文件夹</param>
        /// <param name="filename">文件名称.jpg</param>
        /// <returns></returns>
        public static string GetPath(string folder, string filename = null)
        {
            string oldpath = Environment.CurrentDirectory;
            string newpath = oldpath.Substring(0, oldpath.IndexOf("bin")) + folder;
            if (filename != null)
            {
                newpath += "\\" + filename;
            }
            return newpath;
        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int GetTimeStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 返回時間戳參數（不包含ID）
        /// </summary>
        /// <returns></returns>
        public static string GetWithTimeStamp(DateTime time)
        {
            string postdate = string.Format("&timestamp={0}", GetTimeStamp(time));
            return postdate;
        }
        /// <summary>
        /// 返回时间戳参数(包含ID)
        /// </summary>
        /// <param name="id">账号ID</param>
        /// <returns></returns>
        public static string GetWithTimeStamp(string id)
        {
            string postdate = string.Format("/simi/playlist?id={0}&timestamp={1}", id, GetTimeStamp(DateTime.Now));
            return postdate;
        }
        /// <summary>
        /// 返回时间戳参数（包含ID以及自定义时间戳）
        /// </summary>
        /// <param name="id">账号ID</param>
        /// <param name="dateTime">要添加的时间</param>
        /// <returns></returns>
        public static string GetWithTimeStamp(string id, DateTime dateTime)
        {
            string postdate = string.Format("/simi/playlist?id={0}&timestamp={1}", id, GetTimeStamp(dateTime));
            return postdate;
        }
        /// <summary>
        /// 根据类型下载文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        private static bool SaveFileWithType(string _type, string filename, Stream stream)
        {
            string type = _type.Contains("audio") ? "audio" : _type.Contains("image") ? "image" : "";
            switch (type)
            {
                case "image"://如果内容类型是图片就保存到Image文件夹
                    SaveImgFromStream(stream);
                    return false;
                case "audio":
                    Stream filestream = new FileStream(GetPath("Music", filename.Replace(" ", "").Replace(":", "-").Replace("?", "").Replace("/", "").Replace("\\", "").Replace("*", "") + ".mp3"), FileMode.Create);
                    byte[] by = new byte[102400];
                    int osize = stream.Read(by, 0, (int)by.Length);
                    while (osize > 0)
                    {
                        filestream.Write(by, 0, osize);
                        osize = stream.Read(by, 0, (int)by.Length);
                    }
                    stream.Close();
                    filestream.Close();
                    return true;
                default:
                    return false;
            }
        }
        private static void SaveImgFromStream(Stream stream, string FirstName = "", PictureBox pictureBox2 = null)
        {
            Image image = Image.FromStream(stream);
            string path = GetPath("Image", FirstName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg");
            image.Save(path);
            if (pictureBox2 != null)
            {
                pictureBox2.ImageLocation = path;
            }
        }
        public static List<PicUrl_Init> PicUrlInit()
        {
            //1.json 2.流
            List<PicUrl_Init> picUrls = new List<PicUrl_Init>();
            picUrls.Add(new PicUrl_Init() { Name = "樱花", Url = Const.Sakura2_WebUrl });
            //picUrls.Add(new PicUrl_Init() { Name = "晓晴博客", Url = Const.TouBie_WebUrl });//流：No
            picUrls.Add(new PicUrl_Init() { Name = "搏天api", Url = Const.BoTian2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "樱道", Url = Const.R100862_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "小歪", Url = Const.IXiaoWai2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "保罗", Url = Const.BaoLuo2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "墨天逸", Url = Const.MTY2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "EEE.DOG", Url = Const.EEEDOG2_WebUrl });
            //picUrls.Add(new PicUrl_Init() { Name = "动漫星空", Url = Const.AnimeXK2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "岁月小筑", Url = Const.XJH2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "东方Project", Url = Const.Paulzzh2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "韩小韩", Url = Const.VVHan2_WebUrl });
            picUrls.Add(new PicUrl_Init() { Name = "三秋（不稳定）", Url = Const.Ghser2_WebUrl });
            return picUrls;
        }
    }
}
