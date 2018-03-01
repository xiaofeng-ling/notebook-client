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
using Newtonsoft.Json;

namespace notebook
{

    /// <summary>
    /// 网络请求类，封装了一些方法
    /// </summary>
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

            httpObj.Headers = new WebHeaderCollection();
            httpObj.ContentType = "application/x-www-form-urlencoded";
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

        /// <summary>
        /// 从网络请求中获取数据
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetResponseBytes()
        {
            if (httpResponse == null)
                throw new Exception("没有返回响应！");

            MemoryStream memoryStream = new MemoryStream();

            if (httpResult.Length > 0)
                return httpResult;

            byte[] buffer = new byte[1*1024];          // 缓存区1KB

            using (Stream result = httpResponse.GetResponseStream())
            {
                // 从网络流读取数据到内存流
                while (true)
                {
                    int count = result.Read(buffer, 0, buffer.Length);

                    if (count <= 0)
                        break;

                    memoryStream.Write(buffer, 0, count);
                }
            }

            return httpResult = memoryStream.ToArray();
        }

        /// <summary>
        /// 将网络请求转化为字符串
        /// </summary>
        /// <param name="encode"></param>
        /// <returns>string</returns>
        public string GetResponseString(string encode = "UTF-8")
        {
            byte[] result = GetResponseBytes();

            Encoding encoding = Encoding.GetEncoding(encode);
            return encoding.GetString(result);
        }

        /// <summary>
        /// 获取json解码后的对象
        /// </summary>
        /// <example>
        /// this.GetResponseJsonObject<new {cn=0}>();
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="encode"></param>
        /// <returns>泛型对象</returns>
        public T GetResponseJsonObject<T>(string encode = "UTF-8")
        {
            string result = GetResponseString(encode);

            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// 创建http查询
        //  例如 name=123&email=234@1.com
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>string</returns>
        private string HttpBuildQuery(IDictionary<string, string> parameters, bool needEncode = false)
        {
            StringBuilder buffer = new StringBuilder();
            string format = "{0}={1}";

            foreach (string key in parameters.Keys)
            {
                string value;

                if (needEncode)
                    value = WebUtility.UrlEncode(parameters[key]);
                else
                    value = parameters[key];


                buffer.AppendFormat(format, key, value);

                format = "&{0}={1}";
            }

            return buffer.ToString();
        }
    }
}
