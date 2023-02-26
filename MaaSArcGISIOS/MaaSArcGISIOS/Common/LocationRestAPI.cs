using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Xamarin.Forms.PlatformConfiguration;

namespace MaaSArcGISIOS.Common
{
    public class LocationRestAPI
    {
        private static readonly string mVworldURL =
            "http://api.vworld.kr/req/search?service=search&request=search&version=2.0&crs=EPSG:4326";
        private static readonly string mVworldGeoCodeURL =
            "http://api.vworld.kr/req/address?service=address&request=getAddress&version=2.0&crs=epsg:4326";

        //3개월마다 갱신됨. 추후 회사명 구독하고 영구적으로 받도록 해야함.

        private static readonly string mVworldAPIKey = Constant.mVworldAPIKey;

        private static readonly string mBaroEMapURL = "https://map.ngii.go.kr/openapi/search.json?";
        //제한 없음. 다만 상용되는지는 확인 필요.(개인용은 무제한)
        private static readonly string mBaroEMapAPIKey = "A6482EDB327B8D118018BDC7C0933EC4";
        private static readonly string mBaroEMapReferneceURL = "http://localhost:7070";

        private static readonly string mJusoURL = "https://www.juso.go.kr/addrlink/addrEngApi.do?resultType=json";
        //기간 90일 (2022.04.16 ~ 2022.07.15). 추후 회사명 구독하고 영구적으로 받도록 해야함.
        private static readonly string mJusoAPIKey = "devU01TX0FVVEgyMDIyMDQxNjExMzI0MTExMjQ2OTY=";
        /// <summary>
        /// Vworld에서 장소 요청시 들어오는 각종 데이터들
        /// </summary>
        /// <returns>Json 파싱된 장소값들</returns>
        public static Model.VworldPlacePoi GeVWorldAddressList(string pAddress, Xamarin.Essentials.Location pLocation, int pPageIndex = 1, int pPageSize = 10)
        {
            try
            {
                StringBuilder mHttpURLBuilder = new StringBuilder();
                mHttpURLBuilder.Clear();
                mHttpURLBuilder.AppendFormat("{0}&key={1}&size={2}&page={3}&query={4}&type=place&format=json&bbox={5}"
                    , mVworldURL
                    , mVworldAPIKey
                    , pPageSize
                    , pPageIndex
                    , pAddress
                    , string.Format("{0},{1},{2},{3}", pLocation.Longitude - 1.0, pLocation.Latitude - 1.0, pLocation.Longitude + 1.0, pLocation.Latitude + 1.0));
                HttpClient _Client = new HttpClient();
                string _JsonString = _Client.GetStringAsync(mHttpURLBuilder.ToString()).GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<Model.VworldPlacePoi>(_JsonString);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Vworld에서 장소 요청시 들어오는 각종 데이터들
        /// </summary>
        /// <returns>Json 파싱된 장소값들</returns>
        public static Model.VworldPlacePoi GeVWorldAddressList(string pAddress, int pPageIndex = 1, int pPageSize = 10)
        {
            try
            {
                StringBuilder mHttpURLBuilder = new StringBuilder();
                mHttpURLBuilder.Clear();
                mHttpURLBuilder.AppendFormat("{0}&key={1}&size={2}&page={3}&query={4}&type=place&format=json"
                    , mVworldURL
                    , mVworldAPIKey
                    , pPageSize
                    , pPageIndex
                    , pAddress);
                HttpClient _Client = new HttpClient();
                string _JsonString = _Client.GetStringAsync(mHttpURLBuilder.ToString()).GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<Model.VworldPlacePoi>(_JsonString);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 개발자 도로명주소 API에서 들어온 영문주소를 실제 지도에서 사용하도록 수정.
        /// </summary>
        /// <param name="pHangulAddress">한글 주소 입력</param>
        /// <returns>영문 주소 뽑아줌. error 가 올경우 실패한 거임.</returns>
        public static string ConvertEnglishAddress(string pHangulAddress)
        {
            try
            {
                StringBuilder mHttpURLBuilder = new StringBuilder();
                mHttpURLBuilder.Clear();
                mHttpURLBuilder.AppendFormat("{0}&confmKey={1}&keyword={2}", mJusoURL, mJusoAPIKey, pHangulAddress);

                HttpClient _Client = new HttpClient();
                string _JsonString = _Client.GetStringAsync(mHttpURLBuilder.ToString()).GetAwaiter().GetResult();
                var _JsonData = JsonConvert.DeserializeObject<Model.DevJusoParser>(_JsonString);
                if (_JsonData.results.common.errorCode == "0")
                {
                    return _JsonData.results.juso[0].jibunAddr;
                }
                else
                {
                    return "error";
                }
            }
            catch
            {
                return "error";
            }
        }

        public static double GetGoogleMapAlitutde(Xamarin.Essentials.Location pLocation)
        {
            try
            {
                string _header = string.Format("https://maps.googleapis.com/maps/api/elevation/json?locations={0},{1}&key={2}", pLocation.Latitude, pLocation.Longitude, Common.Constant.mGoogleMapAltitudeAPIKey);
                var _result = Common.RestSharpProvider.HttpGetRequest(_header, "");
                if (_result.Item2 == System.Net.HttpStatusCode.OK)
                {
                    var _convertResult = JsonConvert.DeserializeObject<Model.GoogleMapAPIResult>(_result.Item1);

                    if (_convertResult.status == "OK")
                    {
                        if (_convertResult.results.Count > 0)
                        {
                            return _convertResult.results[0].elevation;
                        }
                        else
                        {
                            return -9999.0;
                        }
                    }
                    else
                    {
                        return -9998.0;
                    }
                }
                else
                {
                    return -9997.0;
                }
            }
            catch
            {
                return -9996.0;
            }
        }
    }
}
