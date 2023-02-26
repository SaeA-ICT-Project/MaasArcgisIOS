using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class RestSharpProvider
    {
        /*
        public static async void HttpPostRequest(Action<string> pCallBack)
        {
            try
            {
                var _Client = new RestSharp.RestClient("https://www.arcgis.com/sharing/rest/oauth2/token");
                _Client.Timeout = 10 * 1000;
                var _Requset = new RestSharp.RestRequest(RestSharp.Method.POST);
                _Requset.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                _Requset.AddParameter("f", "json");
                _Requset.AddParameter("client_id", "EVlfcQyPNTFDMTQm");
                _Requset.AddParameter("client_secret", "94bfc6f8a7f24ca98c66e899495b40ed");
                _Requset.AddParameter("grant_type", "client_credentials");
                var response = await _Client.ExecuteAsync(_Requset);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    pCallBack(Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ArcgisAccessTokenClass>(response.Content).access_token);
                }
                else
                {
                    pCallBack(string.Empty);
                }
            }
            catch
            {
                pCallBack(string.Empty);
            }
        }
        */


        public static async void HttpPostRequest(string pHeader, string pBody, Action<string> pCallBack, int pTimeoutSecond = 10)
        {
            try
            {
                var _Client = new RestSharp.RestClient(pHeader);
                _Client.Timeout = pTimeoutSecond * 1000;
                var _Requset = new RestSharp.RestRequest(RestSharp.Method.POST);
                _Requset.AddHeader("Content-Type", "application/json");
                _Requset.AddParameter("application/json", pBody, RestSharp.ParameterType.RequestBody);
                var response = await _Client.ExecuteAsync(_Requset);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    pCallBack(response.Content);
                }
                else
                {
                    pCallBack(String.Format("{0},{1},{2}", pBody, response.StatusCode, response.Content));
                }
            }
            catch (Exception ets)
            {
                pCallBack(ets.Message);
            }
        }

        public static Tuple<string, System.Net.HttpStatusCode> HttpGetRequest(string pHeader, string pBody, int pTimeoutSecond = 10)
        {
            try
            {
                var _Client = new RestSharp.RestClient(pHeader);
                _Client.Timeout = pTimeoutSecond * 1000;
                var _Requset = new RestSharp.RestRequest(RestSharp.Method.GET);
                _Requset.AddHeader("Content-Type", "application/json");
                _Requset.AddParameter("application/json", pBody, RestSharp.ParameterType.RequestBody);
                RestSharp.IRestResponse response = _Client.Execute(_Requset);

                if (response.IsSuccessful)
                {
                    return new Tuple<string, System.Net.HttpStatusCode>(response.Content, response.StatusCode);
                }
                else
                {
                    return new Tuple<string, System.Net.HttpStatusCode>(pHeader + pBody, System.Net.HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return new Tuple<string, System.Net.HttpStatusCode>(pHeader + pBody, System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
