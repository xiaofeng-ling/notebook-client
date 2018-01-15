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

        public Http(string url, string method)
        {
            httpObj = WebRequest.Create(url) as HttpWebRequest;
            httpObj.Method = method;
        }

        public HttpWebResponse Send(IDictionary<string,string> parameters = null, int timeout = 30000)
        {
            httpObj.ContentType = "application/x-www-form-urlencoded";

            httpObj.Timeout = timeout;

            if (httpObj.Method.Equals("POST"))
            {
                using (Stream stream = httpObj.GetRequestStream())
                {
                    byte[] data = Encoding.Default.GetBytes(HttpBuildQuery(parameters));

                    stream.Write(data, 0, data.Length);
                }
            }

            return httpResponse = httpObj.GetResponse() as HttpWebResponse;
        }

        // 从网络请求中获取数据
        public byte[] GetResponseBytes(HttpWebResponse response = null)
        {
            if (response == null)
            {
                response = httpResponse;
             }

            var result = response.GetResponseStream();

            byte[] bytes = new byte[response.ContentLength];

            result.Read(bytes, 0, bytes.Length);

            return bytes;
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
