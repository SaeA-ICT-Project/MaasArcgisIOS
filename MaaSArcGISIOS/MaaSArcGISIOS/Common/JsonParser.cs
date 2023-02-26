using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class JsonParser
    {
        public static string GetHTTPJsonSerializer(Model.HTTPPressure pRequestBody)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(pRequestBody);
        }

        public static string GetHTTPJsonSerializer(Model.HTTPMPURaw pRequestBody)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(pRequestBody);
        }

        public static string GetHTTPJsonSerializer(Model.HTTPRadarPdat pRequestBody)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(pRequestBody);
        }

        public static string GetHTTPJsonSerializer(Model.HTTPRadarTdat pRequestBody)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(pRequestBody);
        }

        public static string GetHTTPJsonSerializer(Model.HTTPGNSSClock pRequestBody)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(pRequestBody);
        }

        public static string GetHTTPJsonSerializer(Model.HTTPGNSSMeasurement pRequestBody)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(pRequestBody);
        }
    }
}
