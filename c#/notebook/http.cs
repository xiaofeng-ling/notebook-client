using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace notebook
{
    // 网络请求类，封装了一些方法
    public class Http
    {
        // http请求对象
        private HttpWebRequest httpObj = null;
        private HttpWebResponse httpResponse = null;
        private byte[] httpResult = new byte[0];

        public HttpWebResponse HttpResponse { get { return httpResponse; } }

        public Http(string url, string method)
        {
            List<string> methods = new List<string> { "POST", "GET" };

            if (!methods.Exists(value => value.Equals(method)))
                throw new Exception("不支持的方法:" + method);


            httpObj = WebRequest.CreateHttp(url);
            httpObj.Method = method;
        }

        public Http Send(IDictionary<string,string> parameters = null, int timeout = 30000)
        {
            httpObj.ContentType = "application/x-www-form-urlencoded";
            httpObj.Headers = new WebHeaderCollection();

            httpObj.Timeout = timeout;

            if (httpObj.Method.Equals("POST") && parameters != null)
            {
                using (Stream stream = httpObj.GetRequestStream())
                {
                    byte[] data = Encoding.Default.GetBytes(HttpBuildQuery(parameters));

                    stream.Write(data, 0, data.Length);
                }
            }

            httpResponse = httpObj.GetResponse() as HttpWebResponse;

            return this;
        }

        // 从网络请求中获取数据
        public byte[] GetResponseBytes()
        {
            if (httpResponse == null)
                throw new Exception("没有返回响应！");

            Stream result = httpResponse.GetResponseStream();
            MemoryStream memoryStream = new MemoryStream(1024);

            if (httpResult.Length > 0)
                return httpResult;

            byte[] buffer = new byte[1*1024];          // 缓存区1KB

            // 从网络流读取数据到内存流
            while (true)
            {
                int count = result.Read(buffer, 0, buffer.Length);

                if (count <= 0)
                    break;

                memoryStream.Write(buffer, 0, count);
            }

            return httpResult = memoryStream.ToArray();
        }

        // 将网络请求转化为字符串
        public string GetResponseString(string encode = "UTF-8")
        {
            byte[] result = GetResponseBytes();

            Encoding encoding = Encoding.GetEncoding(encode);
            return encoding.GetString(result);
        }
        
        // 创建http查询
        // 例如 name=123&email=234@1.com
        private string HttpBuildQuery(IDictionary<string, string> parameters)
        {
            StringBuilder buffer = new StringBuilder();
            bool firstFlag = true;

            foreach (string key in parameters.Keys)
            {
                if (firstFlag)
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    firstFlag = false;
                }
                else
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
            }

            return WebUtility.UrlEncode(buffer.ToString());
        }
    }
}
