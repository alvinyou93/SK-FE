using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace E3.LCM.MBL.BIZ
{
    public class MAPIWebClient : WebClient
    {
        private static string FACE_DETECT_SERVER_PROTOCOL = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_PROTOCOL", @"https");
        private static string FACE_DETECT_SERVER_ADDRESS = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_ADDRESS", @"10.97.12.1");
        private static string FACE_DETECT_SERVER_PORT = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_PORT", @"44337");
        private static string uriBase = string.Format("{0}://{1}:{2}", FACE_DETECT_SERVER_PROTOCOL, FACE_DETECT_SERVER_ADDRESS, FACE_DETECT_SERVER_PORT);
        private static string userid = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_USERID", @"admin");
        private static string password = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_PASSWORD", @"!QAZ2wsxXX");
        private static string router4Login = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_LOGINPATH", @"/1.0/auth/login");
        private static string router4Face = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_EXTRACTFACE", @"/1.0/extract/face");
        private Dictionary<string, string> keyValuePairs;
        public static CookieContainer CookieContainer { get; private set; }

        public MAPIWebClient(CookieContainer container)
        {
            CookieContainer = container;
            //FACE_DETECT_SERVER_PROTOCOL = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_PROTOCOL", @"https");
            //FACE_DETECT_SERVER_ADDRESS = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_ADDRESS", @"10.97.12.1");
            //FACE_DETECT_SERVER_PORT = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_PORT", @"44337");
            //uriBase = string.Format("{0}://{1}:{2}", FACE_DETECT_SERVER_PROTOCOL, FACE_DETECT_SERVER_ADDRESS, FACE_DETECT_SERVER_PORT);
            //userid = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_USERID", @"admin");
            //password = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_PASSWORD", @"!QAZ2wsxXX");
            //router4Login = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_LOGINPATH", @"/1.0/auth/login");
            //router4Face = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_DETECT_SERVER_EXTRACTFACE", @"/1.0/extract/face");
    }
        public MAPIWebClient()
          : this(new CookieContainer())
        { }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }

        /// <summary>
        /// 환경 파일에서 정보를 읽어와서 
        /// </summary>
        /// <returns></returns>
        public string Login()
        {
            string responseFromServer = string.Empty;
            try
            {
                CookieContainer container;
                var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", uriBase, router4Login));

                keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("user_id", userid);
                keyValuePairs.Add("password", password);
                var jsonParams = Newtonsoft.Json.JsonConvert.SerializeObject(keyValuePairs);

                request.Method = "POST";
                request.ContentType = "application/json";

                var buffer = Encoding.UTF8.GetBytes(jsonParams);
                request.ContentLength = buffer.Length;

                var requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                container = request.CookieContainer = new CookieContainer();

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    CookieContainer = container;
                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (IOException ioe)
            {
                E3.Common.Loggers.FileLogger.Error(ioe);
                throw (ioe);
            }
            catch (Exception e)
            {
                E3.Common.Loggers.FileLogger.Error(e);
                responseFromServer = e.Message;
            }
            return responseFromServer;
        }

        public string callWebRequest(string routerPath, string Method, string strJson = "")
        {
            string responseFromServer = string.Empty;
            try
            {
                var ret = Login();
                dynamic v = Newtonsoft.Json.JsonConvert.DeserializeObject(ret);

                var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", uriBase, routerPath));
                request.Method = Method;
                request.ContentType = "application/json";
                request.CookieContainer = CookieContainer;

                if (!string.IsNullOrEmpty(strJson))
                {
                    var buffer = Encoding.UTF8.GetBytes(strJson);
                    request.ContentLength = buffer.Length;

                    var requestStream = request.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Close();
                }

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (IOException ioe)
            {
                E3.Common.Loggers.FileLogger.Error(ioe);
                throw (ioe);
            }
            catch (Exception e)
            {
                E3.Common.Loggers.FileLogger.Error(e);
                throw (e);
                //responseFromServer = e.Message;
            }
            return responseFromServer;
        }

        public string callWebRequest(string routerPath, string Method, Dictionary<string, string> keyValuePairs)
        {
            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                var jsonParams = Newtonsoft.Json.JsonConvert.SerializeObject(keyValuePairs);
                return callWebRequest(routerPath, Method, jsonParams);
            }
            else 
            {
                return callWebRequest(routerPath, Method, "");
            }
        }

        public string callWebRequest(string routerPath, Dictionary<string, string> keyValuePairs)
        {
            return callWebRequest(routerPath, "POST", keyValuePairs);
        }

        public string callWebRequest(Dictionary<string, string> keyValuePairs)
        {
            return callWebRequest(router4Face, "POST", keyValuePairs);
        }

        public string callWebRequest<T>(string routerPath, string Method, T param)
        {
            var jsonParams = Newtonsoft.Json.JsonConvert.SerializeObject(param);
            return callWebRequest(routerPath, Method, jsonParams);
        }
    }
}
