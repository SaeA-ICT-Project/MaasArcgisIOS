using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    internal class HttpRequestCrawling
    {
        public static void GetZeroPressureValue(double pLatitude, double pLongitude, Action<double> pCallBack)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                /*
                var _ReverseGeoCodeStruct = Common.LocationRestAPI.GetVWorldReverseGeoCoding(pLatitude, pLongitude).response.result;
                if (_ReverseGeoCodeStruct != null)
                {
                    if (_ReverseGeoCodeStruct.Count >= 1)
                    {
                        var _sitename = _ReverseGeoCodeStruct[0].structure.level1;
                    }
                }
                */

                try
                {
                    System.Net.WebRequest myWebRequest;
                    System.Net.WebResponse myWebResponse;

                    myWebRequest = System.Net.WebRequest.Create("https://www.weather.go.kr/w/obs-climate/land/city-obs.do");
                    myWebResponse = myWebRequest.GetResponse();
                    System.IO.Stream streamResponse = myWebResponse.GetResponseStream();
                    System.IO.StreamReader sreader = new System.IO.StreamReader(streamResponse);//reads the data stream
                    var Rstring = sreader.ReadToEnd();//reads it to the end

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(Rstring);

                    var parentbody = doc.DocumentNode.SelectSingleNode("/html/body");
                    var div_containers = parentbody.SelectSingleNode("//div[contains(@class, 'container')]");
                    var section_containers = div_containers.SelectSingleNode("//section[contains(@class, 'page-wrap')]");
                    var div_right_content = section_containers.SelectSingleNode("//div[contains(@class, 'right-content')]");
                    var div_cont_wrap = div_right_content.SelectSingleNode("//div[contains(@class, 'cont-wrap')]");
                    var div_cmp_city_obs = div_right_content.SelectSingleNode("//div[contains(@class, 'cmp-city-obs')]");
                    var div_over_scroll = div_cmp_city_obs.SelectSingleNode("//div[contains(@class, 'over-scroll')]");
                    var weather_table = div_over_scroll.SelectSingleNode("table");
                    var tbody = weather_table.SelectSingleNode("tbody");
                    var trs = tbody.SelectNodes("tr");

                    double minvalue = double.MaxValue;
                    double _parserPressuer = 0.0;
                    double _pressure = -1.0;
                    string _location = string.Empty;

                    foreach (var tr in trs)
                    {
                        var tds = tr.SelectNodes("td");
                        string _sitename = tds[0].InnerText.Trim();

                        if (double.TryParse(tds.Last().InnerText.Trim(), out _parserPressuer) == false)
                        {
                            continue;
                        }
                        else
                        {
                            if (Constant.mWeatherSiteDict.ContainsKey(_sitename))
                            {
                                if (minvalue > (Math.Pow(pLatitude - Constant.mWeatherSiteDict[_sitename].Item1, 2.0) + Math.Pow(pLongitude - Constant.mWeatherSiteDict[_sitename].Item2, 2.0)))
                                {
                                    minvalue = (Math.Pow(pLatitude - Constant.mWeatherSiteDict[_sitename].Item1, 2.0) + Math.Pow(pLongitude - Constant.mWeatherSiteDict[_sitename].Item2, 2.0));
                                    _pressure = _parserPressuer;
                                    _location = _sitename;
                                }
                            }
                        }
                    }

                    Console.WriteLine("debug_console _pressure : {0} , _location : {1}", _pressure, _location);

                    if (_location != string.Empty && _pressure > 0.0)
                    {
                        pCallBack(_pressure);
                    }
                }
                catch (Exception ets)
                {
                    Console.WriteLine("HttpRequestCrawling Error -> {0}", ets.Message);
                }
            });
        }
    }
}
