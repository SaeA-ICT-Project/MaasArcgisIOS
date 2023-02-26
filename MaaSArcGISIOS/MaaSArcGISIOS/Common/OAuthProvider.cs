using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class OAuth2GoogleProvider : Model.IOAuth2Provider
    {
        private string mAccess_Token = null;
        private const string mClientID = "204175858581-0dsn1sv4khjkfjn0r75t3qh1tlq2a2fb.apps.googleusercontent.com";
        //private const string mClientID = "137927749008-n9tlhbo33lr4okq98lhse0bnl5qr3u6a.apps.googleusercontent.com";
        private const string mClientSecret = "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";
        //private const string mClientSecret = "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";
        private const string mRedirectURL = "https://maas.saeaict.com:8087/signin-google";
        private const string mScope = "openid profile email";
        private const string mFlowName = "GeneralOAuthFlow";
        private const string mOauthCheckURL = "https://accounts.google.com/o/oauth2/token";

        public OAuth2GoogleProvider()
        {
        }

        public string GetDefaultURL()
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            strBuilder.AppendFormat("https://accounts.google.com/o/oauth2/v2/auth/oauthchooseaccount?response_type=code");
            strBuilder.AppendFormat("&client_id={0}", mClientID);
            strBuilder.AppendFormat("&redirect_uri={0}", mRedirectURL);
            strBuilder.AppendFormat("&scope={0}", mScope);
            strBuilder.AppendFormat("&flowName={0}", mFlowName);

            return strBuilder.ToString();
        }

        public void GetProviderKey(string pResultURL, Action<Constant.OauthProvdierEnum, string> pResultCallBack)
        {
            var words = pResultURL.Split('=');
            var codes = words[1].Split('&')[0];

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            System.Net.HttpWebRequest webReuqest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(mOauthCheckURL);
            webReuqest.Method = "POST";

            strBuilder.AppendFormat("code={0}", codes);
            strBuilder.AppendFormat("&client_id={0}", mClientID);
            strBuilder.AppendFormat("&client_secret={0}", mClientSecret);
            strBuilder.AppendFormat("&redirect_uri={0}", mRedirectURL);
            strBuilder.AppendFormat("&grant_type=authorization_code");

            byte[] byteArrays = System.Text.Encoding.UTF8.GetBytes(strBuilder.ToString());
            webReuqest.ContentType = "application/x-www-form-urlencoded";
            webReuqest.ContentLength = byteArrays.Length;
            System.IO.Stream postStream = webReuqest.GetRequestStream();
            postStream.Write(byteArrays, 0, byteArrays.Length);
            postStream.Close();

            System.Net.WebResponse response = webReuqest.GetResponse();
            postStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();

            Console.WriteLine(responseFromServer);
            Model.AccessTokenClass serStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.AccessTokenClass>(responseFromServer);

            System.Threading.Tasks.Task.Run(() =>
            {
                if (serStatus != null)
                {
                    string accessToken = string.Empty;
                    accessToken = serStatus.access_token;
                    this.mAccess_Token = serStatus.access_token;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        pResultCallBack(Constant.OauthProvdierEnum.Google, GetRequestProviderKey(accessToken));
                    }
                    pResultCallBack(Constant.OauthProvdierEnum.Google, null);
                }
                pResultCallBack(Constant.OauthProvdierEnum.Google, null);
            });
        }
        private string GetRequestProviderKey(string pAccessToken)
        {
            System.Net.Http.HttpClient _Client = new System.Net.Http.HttpClient();
            var urlProfile = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + pAccessToken;

            _Client.CancelPendingRequests();
            System.Net.Http.HttpResponseMessage outputs = _Client.GetAsync(urlProfile).GetAwaiter().GetResult();

            if (outputs.IsSuccessStatusCode)
            {
                string outputData = outputs.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(outputData);
                Model.GoogleUserOauth serStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.GoogleUserOauth>(outputData);
                return serStatus.id;
            }
            else
            {
                return null;
            }
        }

        public void SetUnlinkOAuth2()
        {
        }
    }

    public class OAuth2KakaoProvider : Model.IOAuth2Provider
    {
        private string mAccess_Token = null;
        private const string mClientID = "14b5389f9c60eae82a7721e30c4a0fc1";
        private const string mClientSecret = "yaDqkqyk3h1X3DNnKzgZqNvgIwoaYau7";
        private const string mRedirectURL = "https://maas.saeaict.com:8087/signin-kakao";
        private const string mScope = "";
        private const string mOauthCheckURL = "https://kauth.kakao.com/oauth/token";
        private string mLoginCode = string.Empty;

        public OAuth2KakaoProvider()
        {
        }

        public string GetDefaultURL()
        {
            return "https://kauth.kakao.com/oauth/authorize?response_type=code&client_id=14b5389f9c60eae82a7721e30c4a0fc1&redirect_uri=https://maas.saeaict.com:8087/signin-kakao";
        }

        public void GetProviderKey(string pResultURL, Action<Constant.OauthProvdierEnum, string> pResultCallBack)
        {
            var words = pResultURL.Split('=');
            var codes = words[1].Split('&')[0];

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            System.Net.HttpWebRequest webReuqest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(mOauthCheckURL);
            webReuqest.Method = "POST";

            strBuilder.AppendFormat("code={0}", codes);
            strBuilder.AppendFormat("&client_id={0}", mClientID);
            strBuilder.AppendFormat("&client_secret={0}", mClientSecret);
            strBuilder.AppendFormat("&redirect_uri={0}", mRedirectURL);
            strBuilder.AppendFormat("&grant_type=authorization_code");

            byte[] byteArrays = System.Text.Encoding.UTF8.GetBytes(strBuilder.ToString());
            webReuqest.ContentType = "application/x-www-form-urlencoded";
            webReuqest.ContentLength = byteArrays.Length;
            System.IO.Stream postStream = webReuqest.GetRequestStream();
            postStream.Write(byteArrays, 0, byteArrays.Length);
            postStream.Close();

            System.Net.WebResponse response = webReuqest.GetResponse();
            postStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            Model.AccessTokenClass serStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.AccessTokenClass>(responseFromServer);

            System.Threading.Tasks.Task.Run(() =>
            {
                if (serStatus != null)
                {
                    string accessToken = string.Empty;
                    accessToken = serStatus.access_token;
                    this.mAccess_Token = serStatus.access_token;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        pResultCallBack(Constant.OauthProvdierEnum.Kakao, GetRequestProviderKey(accessToken));
                    }
                    pResultCallBack(Constant.OauthProvdierEnum.Kakao, null);
                }
                pResultCallBack(Constant.OauthProvdierEnum.Kakao, null);
            });
        }

        private string GetRequestProviderKey(string pAccessToken)
        {
            System.Net.HttpWebRequest webReuqest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://kapi.kakao.com/v2/user/me");
            webReuqest.Headers["Authorization"] = "Bearer " + pAccessToken;

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webReuqest.GetResponse();
            string responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseString);
            Model.KakaoUserOauth _OutputData = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.KakaoUserOauth>(responseString);
            return _OutputData.id;
        }

        public void SetUnlinkOAuth2()
        {
        }
    }

    public class OAuth2NaverProvider : Model.IOAuth2Provider
    {
        private string mAccess_Token = null;
        private const string mClientID = "R3EjxdSjCLBbQjekh3bV";
        private const string mClientSecret = "5zLUy9KZ0l";
        private const string mRedirectURL = "https://maas.saeaict.com:8087/signin-naver";
        private const string mScope = "";
        private const string mOauthCheckURL = "https://nid.naver.com/oauth2.0/token";
        public string GetDefaultURL()
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            strBuilder.AppendFormat("https://nid.naver.com/oauth2.0/authorize?");
            strBuilder.AppendFormat("client_id={0}", mClientID);
            strBuilder.AppendFormat("&scope={0}", mScope);
            strBuilder.AppendFormat("&response_type=code");
            strBuilder.AppendFormat("&redirect_uri={0}", mRedirectURL);

            return strBuilder.ToString();
        }

        public void GetProviderKey(string pResultURL, Action<Constant.OauthProvdierEnum, string> pResultCallBack)
        {
            var words = pResultURL.Split('=');
            var codes = words[1].Split('&')[0];

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            System.Net.HttpWebRequest webReuqest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(mOauthCheckURL);
            webReuqest.Method = "POST";

            webReuqest.Method = "POST";

            strBuilder.AppendFormat("code={0}", codes);
            strBuilder.AppendFormat("&client_id={0}", mClientID);
            strBuilder.AppendFormat("&client_secret={0}", mClientSecret);
            strBuilder.AppendFormat("&redirect_uri={0}", mRedirectURL);
            strBuilder.AppendFormat("&grant_type=authorization_code");

            byte[] byteArrays = System.Text.Encoding.UTF8.GetBytes(strBuilder.ToString());
            webReuqest.ContentType = "application/x-www-form-urlencoded";
            webReuqest.ContentLength = byteArrays.Length;
            System.IO.Stream postStream = webReuqest.GetRequestStream();
            postStream.Write(byteArrays, 0, byteArrays.Length);
            postStream.Close();

            System.Net.WebResponse response = webReuqest.GetResponse();
            postStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            Model.AccessTokenClass serStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.AccessTokenClass>(responseFromServer);

            System.Threading.Tasks.Task.Run(() =>
            {
                if (serStatus != null)
                {
                    string accessToken = string.Empty;
                    accessToken = serStatus.access_token;
                    this.mAccess_Token = serStatus.access_token;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        pResultCallBack(Constant.OauthProvdierEnum.Naver, GetRequestProviderKey(accessToken));
                    }
                    pResultCallBack(Constant.OauthProvdierEnum.Naver, null);
                }
                pResultCallBack(Constant.OauthProvdierEnum.Naver, null);
            });
        }

        public string GetRequestProviderKey(string pAccessToken)
        {
            System.Net.HttpWebRequest webReuqest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://openapi.naver.com/v1/nid/me");
            webReuqest.Headers["Authorization"] = "Bearer " + pAccessToken;

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webReuqest.GetResponse();
            string responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
            Model.NaverUserOutputData _OutputData = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.NaverUserOutputData>(responseString);
            return _OutputData.response.id;
        }
        public void SetUnlinkOAuth2()
        {

        }
    }
}
