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
using System.Windows;

namespace notebook
{

    public delegate void callbackDelegate(string responseText);

    /// <summary>
    /// 网络请求类，封装了一些方法
    /// </summary>
    public class Http
    {

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">方法，默认GET</param>
        /// <param name="parameters">字典对象的参数</param>
        /// <param name="timeout">超时时间，毫秒</param>
        /// <returns>接收到的数据</returns>
        public static String Send(string url, string method = "GET", IDictionary<string,string> parameters = null)
        {

            string responseText = "";

            try
            {
                string tempParam = "";
                if (null != parameters)
                    tempParam = Http.HttpBuildQuery(parameters);

                if ("GET" == method)
                    url += "?" + tempParam;

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = method;

                if ("POST" == method)
                {
                    byte[] data = Encoding.Default.GetBytes(tempParam);
                    request.ContentLength = data.Length;
                    request.ContentType = "application/x-www-form-urlencoded";

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }
                }

                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                responseText = new StreamReader(responseStream).ReadToEnd();
            }
            catch(WebException e)
            {
                Stream responseStream = e.Response.GetResponseStream();
                responseText = new StreamReader(responseStream).ReadToEnd();
            }
            catch (Exception)
            {
                MessageBox.Show("网络错误，请重试！");
            }

            return responseText;
        }

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> SendAsync(string url, string method = "GET", IDictionary<string, string> parameters = null, callbackDelegate testDelegate = null)
        {
            string responseText = "";

            await Task.Run(async () =>
            {
                responseText = Send(url, method, parameters);

                testDelegate?.Invoke(responseText);
            });

            return responseText;
        }

        /// <summary>
        /// 创建http查询
        //  例如 name=123&email=234@1.com
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>string</returns>
        public static string HttpBuildQuery(IDictionary<string, string> parameters, bool needEncode = false)
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
