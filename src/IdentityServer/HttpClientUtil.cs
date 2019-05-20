using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IdentityServer
{
    public class HttpClientUtil
    {

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            //httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// 发起post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostResponse<T>(string url, T postData) where T : class, new()
        {
            string result = "0";
            var format = new IsoDateTimeConverter();
            format.DateTimeFormat = "yyyyMMddHHmmssSSS";
            var dataJson = JsonConvert.SerializeObject(postData, Newtonsoft.Json.Formatting.Indented, format);
            var content = new StringContent(dataJson, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                //Task<string> t = response.Content.ReadAsStringAsync();
                //string s = t.Result;
                //T ss = JsonConvert.DeserializeObject<T>(s);
                result = "1";
            }
            return result;
        }

        /// <summary>
        /// 发起post请求主键返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns>主键</returns>
        public static string PostResponseKey<T>(string url, T postData) where T : class, new()
        {
            string ret = "0";
            var format = new IsoDateTimeConverter();
            format.DateTimeFormat = "yyyyMMddHHmmssSSS";
            var dataJson = JsonConvert.SerializeObject(postData, Newtonsoft.Json.Formatting.Indented, format);
            var content = new StringContent(dataJson, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                ret = t.Result;
            }
            return ret;
        }


        /// <summary>
        /// 发起post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static T PostResponse<T>(string url, string postData) where T : class, new()
        {
            //if (postData != null)
            //{
            //    if (url.StartsWith("https"))
            //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            //    HttpClient httpClient = new HttpClient();
            //    var format = new IsoDateTimeConverter();
            //    format.DateTimeFormat = "yyyyMMddHHmmssSSS";
            //    var dataJson = JsonConvert.SerializeObject(postData, Newtonsoft.Json.Formatting.Indented, format);
            //    var content = new StringContent(dataJson, Encoding.UTF8, "application/json");
            //    HttpResponseMessage response = httpClient.PutAsync(url, content).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        result = "1";
            //    }
            //}

            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();

            T result = default(T);

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }
            return result;
        }

        /// <summary>
        /// V3接口全部为Xml形式，故有此方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T PostXmlResponse<T>(string url, string xmlString) where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(xmlString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();

            T result = default(T);

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = XmlDeserialize<T>(s);
            }
            return result;
        }

        /// <summary>
        /// 反序列化Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xmlString) where T : class, new()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(xmlString))
                {
                    return (T)ser.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("XmlDeserialize发生异常：xmlString:" + xmlString + "异常信息：" + ex.Message);
            }

        }


        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetResponse<T>(string url) where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(3000);
            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }
            return result;
        }

        /// <summary>
        /// Get请求返回List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns>List<T></returns>
        public static List<T> GetResponseList<T>(string url) where T : class, new()
        {
            if (url.StartsWith("https"))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            List<T> result = default(List<T>);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;
                if (s != null && !s.StartsWith("["))
                {
                    s = "[" + s + "]";
                }
                result = JsonConvert.DeserializeObject<List<T>>(s);
            }
            return result;
        }


        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string DeleteResponse(string url)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return "1";
            }
            return "0";
        }

        /// <summary>
        /// 发起put请求 List格式数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">put数据</param>
        /// <returns></returns>
        public static string PutListDataResponse<T>(string url, List<T> postData) where T : class, new()
        {
            string result = "0";
            if (postData != null && postData.Count > 0)
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                HttpClient httpClient = new HttpClient();
                var format = new IsoDateTimeConverter();
                format.DateTimeFormat = "yyyyMMddHHmmssSSS";
                var dataJson = JsonConvert.SerializeObject(postData[0], Newtonsoft.Json.Formatting.Indented, format);
                var content = new StringContent(dataJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = "1";
                }
            }
            else
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.PutAsync(url, null).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = "1";
                }
            }

            return result;
        }

        /// <summary>
        /// 发起put请求 (修改时候用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">put数据</param>
        /// <returns></returns>
        public static string PutDataResponse<T>(string url, T postData) where T : class, new()
        {
            string result = "0";
            if (postData != null)
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                HttpClient httpClient = new HttpClient();
                var format = new IsoDateTimeConverter();
                format.DateTimeFormat = "yyyyMMddHHmmssSSS";
                var dataJson = JsonConvert.SerializeObject(postData, Newtonsoft.Json.Formatting.Indented, format);
                var content = new StringContent(dataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = "1";
                }
            }
            else
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.PutAsync(url, null).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = "1";
                }
            }
            return result;
        }
    }
}
