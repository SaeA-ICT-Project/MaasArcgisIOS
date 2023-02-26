using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public interface IOAuth2Provider
    {
        string GetDefaultURL();
        void GetProviderKey(string pResultURL, Action<MaaSArcGISIOS.Common.Constant.OauthProvdierEnum, string> pResultCallBack);
        void SetUnlinkOAuth2();
    }

    public class AccessTokenClass
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
    }
    public class ArcgisAccessTokenClass
    {
        public string access_token { get; set; }
        public long expires_in { get; set; }
    }

    public class GoogleUserOauth
    {
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }

    public class KakaoUserOauth
    {
        public string id { get; set; }
        public string connected_at { get; set; }
    }

    public class NaverUserOutputData
    {
        public string resultcode { get; set; }
        public string message { get; set; }
        public NaverUserOutputResponseData response { get; set; }
    }

    public class NaverUserOutputResponseData
    {
        public string id { get; set; }
    }

    public class LoginUserOauth
    {
        public string LoginProvider { get; set; }
        public string Providerkey { get; set; }
    }
}
