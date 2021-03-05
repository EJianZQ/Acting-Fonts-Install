using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Acting字体安装器
{
    public class FontsData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 最新版下载链接
        /// </summary>
        public string DownLink { get; set; }
        /// <summary>
        /// 购买卡密开关
        /// </summary>
        public int BuyKeysSwitch { get; set; }
        /// <summary>
        /// 购买卡密链接
        /// </summary>
        public string BuyKeysLink { get; set; }
        /// <summary>
        /// 文字教程链接
        /// </summary>
        public string Texttutorial { get; set; }
        /// <summary>
        /// 视频教程链接
        /// </summary>
        public string Videotutorial { get; set; }
    }

    public class UpdateData
    {
        /// <summary>
        /// 解析完毕的数据
        /// </summary>
        public FontsData fontsdata { get; set; }
        /// <summary>
        /// 窗口信息
        /// </summary>
        public IWin32Window win32Window { get; set; }
    }
    class GetData
    {
        public static string Get(string Url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;//SSL证书模式
            //System.GC.Collect();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Proxy = null;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "application/json; charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Timeout = 3000;//3秒即超时
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }

                return retString;
            }
            catch
            {
                return "Error";
            }
        }

        public static string Post(string Url, string Data, string Referer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.Referer = Referer;
            byte[] bytes = Encoding.UTF8.GetBytes(Data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            Stream myResponseStream = request.GetRequestStream();
            myResponseStream.Write(bytes, 0, bytes.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }
            return retString;
        }
    }
}
