using MaaSArcGISIOS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaaSArcGISIOS.ShellView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapNaviationPage : ContentPage
    {
        private Xamarin.Essentials.Location mLastLocation = null;
        private Xamarin.Essentials.GeolocationRequest mRequest = null;
        private Data.SingletonData mSharedData;
        private Common.EsriMapComponent mEsriMap;

        private Model.VworldPlacePoi.Point mStartPoint = null;
        private Model.VworldPlacePoi.Point mEndPoint = null;
        private NavigataionMapEngine mEngine;
        private bool mCheckBoxValue = false;
        private bool mSimulationValue = false;
        private Common.MSSQLProvider mDBProvider;
        private Model.DeviceGPSInfo SimualationLocation = null;
        private Random mRand;
        private List<Xamarin.Essentials.Location> mMPULocationList;
        private List<Xamarin.Essentials.Location> mAngleLocationList;
        private List<Xamarin.Essentials.Location> mRadarLocationList;
        public MapNaviationPage()
        {
            InitializeComponent();
            mSharedData = Data.SingletonData.mInstance;
            mEsriMap = new Common.EsriMapComponent(
                CatchStringCallBack,
                RouteStringCurrentSiteCallBack,
                RouteStringCurrentDirectionManeuverCallBack,
                RouteStringFutureDirectionManeuverCallBack,
                RouteLongSearchTimeCallBack,
                RouteDisplayLocationCallBack,
                RouteFutureLocationCallBack,
                true);
            mEsriMap.CreateMap();
            xEsriMapLayout.Children.Add(mEsriMap.GetMapView());
            mEngine = new NavigataionMapEngine(this);
            mEngine.Start(1000);
            mMPULocationList = new List<Xamarin.Essentials.Location>();
            mAngleLocationList = new List<Xamarin.Essentials.Location>();
            mRadarLocationList = new List<Xamarin.Essentials.Location>();
            mDBProvider = new Common.MSSQLProvider();
            mRand = new Random();
        }

        private void RouteLongSearchTimeCallBack(long pCallback, int pType)
        {
            try
            {
                this.Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    if (pCallback < 0)
                    {
                        await DisplayAlert("오류", "서버연결에 장애가 발생하였습니다.", "OK");
                    }
                    else
                    {
                        if (pType == 0)
                        {
                            xSearchButton0.Text = String.Format("최적({0}.{1}초)", pCallback / 1000, (pCallback / 100) % 10);
                        }
                        else if (pType == 1)
                        {
                            xSearchButton1.Text = String.Format("안전({0}.{1}초)", pCallback / 1000, (pCallback / 100) % 10);
                        }
                        else
                        {
                            xSearchButton2.Text = String.Format("편한({0}.{1}초)", pCallback / 1000, (pCallback / 100) % 10);
                        }
                    }
                });
            }
            catch
            {

            }
        }

        private void RouteStringFutureDirectionManeuverCallBack(string pCallBack)
        {
            try
            {
                this.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    xFutureManeuverinfo.Text = pCallBack;
                });
            }
            catch
            {

            }
        }
        private void RouteStringCurrentDirectionManeuverCallBack(string pCallBack)
        {
            try
            {
                this.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    xCurrentManeuverInfo.Text = pCallBack;
                });
            }
            catch
            {

            }
        }
        private void RouteStringCurrentSiteCallBack(string pCallBack)
        {
            try
            {
                this.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    xSiteInfo.Text = pCallBack;
                });
            }
            catch
            {

            }
        }
        private void CatchStringCallBack(string pCallBack)
        {
            this.Dispatcher.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Route Error", pCallBack, "OK");
            });
        }

        private void RouteDisplayLocationCallBack(Xamarin.Essentials.Location obj)
        {
            try
            {
                if (mSimulationValue)
                {
                    var _datetimestr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    var _altitude = Math.Round(Common.LocationRestAPI.GetGoogleMapAlitutde(obj), 3);

                    if (SimualationLocation == null)
                    {
                        SimualationLocation = new DeviceGPSInfo()
                        {
                            latitude = obj.Latitude,
                            longitude = obj.Longitude,
                            altitude = _altitude > -999.0 ? _altitude : 0.0,
                            angle = 0.0,
                        };
                    }
                    else
                    {
                        var source = new Xamarin.Essentials.Location(SimualationLocation.latitude, SimualationLocation.longitude);
                        var target = new Xamarin.Essentials.Location(Math.Round(obj.Latitude, 7), Math.Round(obj.Longitude, 7));
                        var distance_heigth = Math.Round(Math.Round(_altitude > -999.0 ? _altitude : SimualationLocation.altitude, 3) - SimualationLocation.altitude, 3);
                        var distance_width = Math.Round(Xamarin.Essentials.Location.CalculateDistance(source, target, Xamarin.Essentials.DistanceUnits.Kilometers) * 1000.0, 3);

                        SimualationLocation.latitude = obj.Latitude;
                        SimualationLocation.longitude = obj.Longitude;
                        SimualationLocation.altitude = _altitude;

                        if (Math.Abs(distance_heigth) < 0.03 || Math.Abs(distance_width) < 0.1)
                        {
                            SimualationLocation.angle = 0.0;
                        }
                        else
                        {
                            SimualationLocation.angle = Math.Round(Math.Atan2(distance_heigth, distance_width) * (180.0 / Math.PI), 2);
                        }
                    }

                    int _randint = mRand.Next(200);

                    if (mSharedData.mCustomSettingInfo != null)
                    {
                        if (mSharedData.mCustomSettingInfo.SimulationSave == true)
                        {
                            mSharedData.mSimulationPressureQueue.Push(new HTTPPressure()
                            {
                                gps = new HTTPGPSInfo
                                {
                                    latitude = obj.Latitude,
                                    longitude = obj.Longitude,
                                },
                                data = new HTTPInnerPressure
                                {
                                    altitude = _altitude,
                                    inclination = SimualationLocation == null ? 0.0 : SimualationLocation.angle,
                                    recordDate = _datetimestr,
                                }
                            });

                            mSharedData.mSimulationMPUQueue.Push(new HTTPMPURaw()
                            {
                                gps = new HTTPGPSInfo
                                {
                                    latitude = obj.Latitude,
                                    longitude = obj.Longitude,
                                },
                                data = new HTTPInnerMPU
                                {
                                    recordDate = _datetimestr,
                                    wheelSize = 3.0,
                                    x = Common.Constant.mMPURandomArray[_randint * 37 % 100],
                                    y = Common.Constant.mMPURandomArray[_randint * 17 % 100],
                                    z = Common.Constant.mMPURandomArray[_randint * 19 % 100],
                                }
                            });

                            mSharedData.mSimulationPdatQueue.Push(new HTTPRadarPdat()
                            {
                                gps = new HTTPGPSInfo
                                {
                                    latitude = obj.Latitude,
                                    longitude = obj.Longitude,
                                },
                                data = new HTTPInnerPDat
                                {
                                    recordDate = _datetimestr,
                                    objectsCount = Common.Constant.mPdatRandomArray[_randint % 100]
                                }
                            });
                        }
                    }
                    else
                    {
                        mSharedData.mSimulationPressureQueue.Push(new HTTPPressure()
                        {
                            gps = new HTTPGPSInfo
                            {
                                latitude = obj.Latitude,
                                longitude = obj.Longitude,
                            },
                            data = new HTTPInnerPressure
                            {
                                altitude = _altitude,
                                inclination = SimualationLocation == null ? 0.0 : SimualationLocation.angle,
                                recordDate = _datetimestr,
                            }
                        });

                        mSharedData.mSimulationMPUQueue.Push(new HTTPMPURaw()
                        {
                            gps = new HTTPGPSInfo
                            {
                                latitude = obj.Latitude,
                                longitude = obj.Longitude,
                            },
                            data = new HTTPInnerMPU
                            {
                                recordDate = _datetimestr,
                                wheelSize = 3.0,
                                x = Common.Constant.mMPURandomArray[_randint * 37 % 100],
                                y = Common.Constant.mMPURandomArray[_randint * 17 % 100],
                                z = Common.Constant.mMPURandomArray[_randint * 19 % 100],
                            }
                        });

                        mSharedData.mSimulationPdatQueue.Push(new HTTPRadarPdat()
                        {
                            gps = new HTTPGPSInfo
                            {
                                latitude = obj.Latitude,
                                longitude = obj.Longitude,
                            },
                            data = new HTTPInnerPDat
                            {
                                recordDate = _datetimestr,
                                objectsCount = Common.Constant.mPdatRandomArray[_randint % 100]
                            }
                        });
                    }
                }
            }
            catch
            {

            }
        }

        private void RouteFutureLocationCallBack(Xamarin.Essentials.Location obj, Xamarin.Essentials.Location pSource)
        {
            try
            {
                var _absLatDiff = Math.Abs(pSource.Latitude - obj.Latitude) / 2.0;
                var _absLonDiff = Math.Abs(pSource.Longitude - obj.Longitude) / 2.0;
                var _maxDiff = _absLatDiff > _absLonDiff ? _absLatDiff : _absLonDiff;

                var _searchminLat = ((pSource.Latitude + obj.Latitude) / 2.0) - _maxDiff;
                var _searchmaxLat = ((pSource.Latitude + obj.Latitude) / 2.0) + _maxDiff;

                var _searchminLon = ((pSource.Longitude + obj.Longitude) / 2.0) - _maxDiff;
                var _searchmaxLon = ((pSource.Longitude + obj.Longitude) / 2.0) + _maxDiff;

                var _MPUWarninglist = mMPULocationList.Where(x => ((x.Latitude >= _searchminLat - 0.0001) && (x.Latitude <= _searchmaxLat + 0.00001)) && ((x.Longitude >= _searchminLon - 0.0001) && (x.Longitude <= _searchmaxLon + 0.0001)));
                var _AngleWarninglist = mAngleLocationList.Where(x => ((x.Latitude >= _searchminLat - 0.0001) && (x.Latitude <= _searchmaxLat + 0.0001)) && ((x.Longitude >= _searchminLon - 0.0001) && (x.Longitude <= _searchmaxLon + 0.0001)));
                var _RadarWarninglist = mRadarLocationList.Where(x => ((x.Latitude >= _searchminLat - 0.0001) && (x.Latitude <= _searchmaxLat + 0.0001)) && ((x.Longitude >= _searchminLon - 0.0001) && (x.Longitude <= _searchmaxLon + 0.0001)));

                if (_MPUWarninglist == null)
                {
                    this.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        xAreaMPUInfo.Text = string.Format("요철 : 전방0.0m");
                    });
                }
                else
                {
                    if (_MPUWarninglist.Count() >= 1)
                    {
                        double minMPUDistance = 9999.0;
                        foreach (var item in _MPUWarninglist)
                        {
                            var caculateDistance = Xamarin.Essentials.Location.CalculateDistance(item, pSource, Xamarin.Essentials.DistanceUnits.Kilometers);
                            if (minMPUDistance > caculateDistance)
                            {
                                minMPUDistance = caculateDistance;
                            }
                        }
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            xAreaMPUInfo.Text = string.Format("요철 : 전방 {0:0.0}m", Math.Round(minMPUDistance * 1000.0, 1));
                        });
                    }
                    else
                    {
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            xAreaMPUInfo.Text = string.Format("요철 : 전방 0.0m");
                        });
                    }
                }

                if (_AngleWarninglist == null)
                {
                    this.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        xAreaAngleInfo.Text = string.Format("급경사 : 전방 0.0m");
                    });
                }
                else
                {
                    if (_AngleWarninglist.Count() >= 1)
                    {
                        double minAngleDistance = 9999.0;
                        foreach (var item in _AngleWarninglist)
                        {
                            var caculateDistance = Xamarin.Essentials.Location.CalculateDistance(item, pSource, Xamarin.Essentials.DistanceUnits.Kilometers);

                            if (minAngleDistance > caculateDistance)
                            {
                                minAngleDistance = caculateDistance;
                            }
                        }
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            xAreaAngleInfo.Text = string.Format("급경사 : 전방 {0:0.0}m", Math.Round(minAngleDistance * 1000.0, 1));
                        });
                    }
                    else
                    {
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            xAreaAngleInfo.Text = string.Format("급경사 : 전방 0.0m");
                        });
                    }
                }

                if (_RadarWarninglist == null)
                {
                    this.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        xAreaRadarInfo.Text = string.Format("혼잡도 : 전방 0.0m");
                    });
                }
                else
                {
                    if (_RadarWarninglist.Count() >= 1)
                    {
                        double minAngleDistance = 9999.0;
                        foreach (var item in _RadarWarninglist)
                        {
                            var caculateDistance = Xamarin.Essentials.Location.CalculateDistance(item, pSource, Xamarin.Essentials.DistanceUnits.Kilometers);

                            if (minAngleDistance > caculateDistance)
                            {
                                minAngleDistance = caculateDistance;
                            }
                        }
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            xAreaRadarInfo.Text = string.Format("혼잡도 : 전방 {0:0.0}m", Math.Round(minAngleDistance * 1000.0, 1));
                        });
                    }
                    else
                    {
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            xAreaRadarInfo.Text = string.Format("혼잡도 : 전방 0.0m");
                        });
                    }
                }
            }
            catch (Exception ets)
            {
                Console.WriteLine(ets.Message);
            }
        }

        protected override void OnAppearing()
        {
            mEsriMap.InitAppearing();
            if (mLastLocation != null)
            {
                mEsriMap.SetMapViewPointAsync(mLastLocation);
            }
        }

        protected override void OnDisappearing()
        {
            mEsriMap.InitDisAppearing();
        }
        private async void SourcePathButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (xStartLocationPath.Text == String.Empty)
                {
                    mStartPoint = null;
                    await DisplayAlert("Error", "Input Address", "Cancel");
                    return;
                }
                string _inputtext = xStartLocationPath.Text.Trim();
                await Navigation.PushAsync(new FindListViewPage(_inputtext, GetStartPoiItem));
            }
            catch (Exception ets)
            {
                mStartPoint = null;
                await DisplayAlert("Error", ets.Message, "Cancel");
            }
        }

        private void GetStartPoiItem(VworldPlacePoi.PoiItem pTarget)
        {
            try
            {
                if (pTarget != null)
                {
                    xStartLocationPath.Text = pTarget.title;
                    mStartPoint = pTarget.point;
                }
                else
                {
                    mStartPoint = null;
                }
            }
            catch
            {

            }
        }

        private async void DestinationPathButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (xEndLocationPath.Text == String.Empty)
                {
                    mEndPoint = null;
                    await DisplayAlert("Error", "Input Address", "Cancel");
                    return;
                }
                string _inputtext = xEndLocationPath.Text.Trim();
                await Navigation.PushAsync(new FindListViewPage(_inputtext, GetEndPoiItem));
            }
            catch (Exception ets)
            {
                mEndPoint = null;
                await DisplayAlert("Error", ets.Message, "Cancel");
            }
        }

        private void GetEndPoiItem(VworldPlacePoi.PoiItem pTarget)
        {
            try
            {
                if (pTarget != null)
                {
                    xEndLocationPath.Text = pTarget.title;
                    mEndPoint = pTarget.point;
                }
                else
                {
                    mEndPoint = null;
                }
            }
            catch
            {

            }
        }

        private async void RoutedButton_Clicked0(object sender, EventArgs e)
        {
            Button target = sender as Button;

            try
            {
                if (mCheckBoxValue == false)
                {
                    if (mStartPoint == null || mEndPoint == null)
                    {
                        await DisplayAlert("Error", "Don't Set Path", "Cancel");
                        return;
                    }
                    else
                    {
                        var endpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x));
                        var startpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mStartPoint.y), Convert.ToDouble(mStartPoint.x));
                        var startlocate = new Xamarin.Essentials.Location(startpoint.Y, startpoint.X);
                        var endlocate = new Xamarin.Essentials.Location(endpoint.Y, endpoint.X);

                        SetMemberValueMPUAngleList(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate);
                        mEsriMap.RoutedAddress(mSimulationValue,
                            0,
                            endpoint,
                            startpoint,
                            mSharedData.mCustomSettingInfo.SimulationSpeed);
                    }
                }
                else
                {
                    if (mEndPoint == null)
                    {
                        await DisplayAlert("Error", "Don't Set Path", "Cancel");
                        return;
                    }
                    else
                    {
                        if (mSharedData.mCurrentGPSInfo != null)
                        {
                            var startlocate = new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude);
                            var endpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x));
                            var startpoint = mEsriMap.CreateMapPoint(new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude));
                            var endlocate = new Xamarin.Essentials.Location(endpoint.Y, endpoint.X);

                            SetMemberValueMPUAngleList(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate);
                            mEsriMap.RoutedAddress(mSimulationValue,
                                0,
                                endpoint,
                                startpoint,
                                mSharedData.mCustomSettingInfo.SimulationSpeed);
                        }
                        else
                        {
                            await DisplayAlert("Error", "Do not Set Currnet Position", "Cancel");
                        }
                    }
                }
            }
            catch (Exception ets)
            {

            }
        }
        private async void RoutedButton_Clicked1(object sender, EventArgs e)
        {
            Button target = sender as Button;

            try
            {
                if (mCheckBoxValue == false)
                {
                    if (mStartPoint == null || mEndPoint == null)
                    {
                        await DisplayAlert("Error", "Don't Set Path", "Cancel");
                        return;
                    }
                    else
                    {
                        var endpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x));
                        var startpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mStartPoint.y), Convert.ToDouble(mStartPoint.x));
                        var startlocate = new Xamarin.Essentials.Location(startpoint.Y, startpoint.X);
                        var endlocate = new Xamarin.Essentials.Location(endpoint.Y, endpoint.X);

                        var mputhreshold = mSharedData.mCustomSettingInfo == null ? 10.0 : mSharedData.mCustomSettingInfo.MPURiskLimit;
                        var avoidArray = mDBProvider.SelectMPU(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate, mputhreshold).ToArray();

                        SetMemberValueMPUAngleList(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate);
                        mEsriMap.RoutedAddress(mSimulationValue,
                            1,
                            endpoint,
                            startpoint,
                            mSharedData.mCustomSettingInfo.SimulationSpeed,
                            avoidArray.Length < 1 ? null : avoidArray);
                    }
                }
                else
                {
                    if (mEndPoint == null)
                    {
                        await DisplayAlert("Error", "Don't Set Path", "Cancel");
                        return;
                    }
                    else
                    {
                        if (mSharedData.mCurrentGPSInfo != null)
                        {
                            var startlocate = new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude);
                            var endpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x));
                            var startpoint = mEsriMap.CreateMapPoint(new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude));
                            var endlocate = new Xamarin.Essentials.Location(endpoint.Y, endpoint.X);

                            var mputhreshold = mSharedData.mCustomSettingInfo == null ? 10.0 : mSharedData.mCustomSettingInfo.MPURiskLimit;
                            var avoidArray = mDBProvider.SelectMPU(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate, mputhreshold).ToArray();

                            SetMemberValueMPUAngleList(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate);
                            mEsriMap.RoutedAddress(mSimulationValue,
                                1,
                                endpoint,
                                startpoint,
                                mSharedData.mCustomSettingInfo.SimulationSpeed,
                                avoidArray.Length < 1 ? null : avoidArray);
                        }
                        else
                        {
                            await DisplayAlert("Error", "Do not Set Currnet Position", "Cancel");
                        }
                    }
                }
            }
            catch
            {

            }
        }
        private async void RoutedButton_Clicked2(object sender, EventArgs e)
        {
            Button target = sender as Button;

            try
            {
                if (mCheckBoxValue == false)
                {
                    if (mStartPoint == null || mEndPoint == null)
                    {
                        await DisplayAlert("Error", "Don't Set Path", "Cancel");
                        return;
                    }
                    else
                    {
                        var endpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x));
                        var startpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mStartPoint.y), Convert.ToDouble(mStartPoint.x));
                        var startlocate = new Xamarin.Essentials.Location(startpoint.Y, startpoint.X);
                        var endlocate = new Xamarin.Essentials.Location(endpoint.Y, endpoint.X);

                        var anglethreshold = mSharedData.mCustomSettingInfo == null ? 7.0 : mSharedData.mCustomSettingInfo.AngleRiskLimit;
                        var avoidArray = mDBProvider.SelectAngle(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate, anglethreshold).ToArray();

                        SetMemberValueMPUAngleList(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate);
                        mEsriMap.RoutedAddress(mSimulationValue,
                            2,
                            endpoint,
                            startpoint,
                            mSharedData.mCustomSettingInfo.SimulationSpeed,
                            avoidArray.Length < 1 ? null : avoidArray);

                    }
                }
                else
                {
                    if (mEndPoint == null)
                    {
                        await DisplayAlert("Error", "Don't Set Path", "Cancel");
                        return;
                    }
                    else
                    {
                        if (mSharedData.mCurrentGPSInfo != null)
                        {
                            var startlocate = new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude);
                            var endpoint = mEsriMap.ConvertoWGS84(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x));
                            var startpoint = mEsriMap.CreateMapPoint(new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude));
                            var endlocate = new Xamarin.Essentials.Location(endpoint.Y, endpoint.X);

                            var anglethreshold = mSharedData.mCustomSettingInfo == null ? 7.0 : mSharedData.mCustomSettingInfo.AngleRiskLimit;
                            var avoidArray = mDBProvider.SelectAngle(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate, anglethreshold).ToArray();

                            SetMemberValueMPUAngleList(mSimulationValue, mSharedData.mOauthUserInfo, startlocate, endlocate);
                            mEsriMap.RoutedAddress(mSimulationValue,
                                2,
                                endpoint,
                                startpoint,
                                mSharedData.mCustomSettingInfo.SimulationSpeed,
                                avoidArray.Length < 1 ? null : avoidArray);
                        }
                        else
                        {
                            await DisplayAlert("Error", "Do not Set Currnet Position", "Cancel");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void SetMemberValueMPUAngleList(bool pSimulator, LoginUserOauth pOauth, Xamarin.Essentials.Location pStartLocate, Xamarin.Essentials.Location pEndLocate)
        {
            mMPULocationList.Clear();
            foreach (var item in mDBProvider.SelectMPUFrontWarning(pSimulator, pOauth, pStartLocate, pEndLocate, mSharedData.mCustomSettingInfo.MPURiskLimit).ToArray())
            {
                mMPULocationList.Add(item);
            }
            mAngleLocationList.Clear();
            foreach (var item in mDBProvider.SelectAngleFrontWarning(pSimulator, pOauth, pStartLocate, pEndLocate, mSharedData.mCustomSettingInfo.AngleRiskLimit).ToArray())
            {
                mAngleLocationList.Add(item);
            }
            mRadarLocationList.Clear();
            foreach (var item in mDBProvider.SelectRadarFrontWarning(pSimulator, pOauth, pStartLocate, pEndLocate, mSharedData.mCustomSettingInfo.PdatRiskLimit).ToArray())
            {
                mRadarLocationList.Add(item);
            }
        }

        private void xCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox target = sender as CheckBox;

            mCheckBoxValue = target.IsChecked;

            if (target.IsChecked)
            {
                xStartGrid.IsVisible = false;
            }
            else
            {
                xStartGrid.IsVisible = true;
            }
        }

        private void xSimulationableCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox target = sender as CheckBox;
            mSimulationValue = target.IsChecked;
        }


        private class NavigataionMapEngine : Common.EngineThread
        {
            private MapNaviationPage mParent;
            public NavigataionMapEngine(MapNaviationPage pParentPage)
            {
                mParent = pParentPage;
            }
            protected override void begin()
            {

            }

            protected override void end()
            {
            }

            protected override void loop()
            {

                mParent.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (this.mParent.mSharedData.mCurrentGPSInfo != null)
                        {
                            if (this.mParent.mLastLocation == null)
                            {
                                this.mParent.mLastLocation = new Xamarin.Essentials.Location(this.mParent.mSharedData.mCurrentGPSInfo.latitude, this.mParent.mSharedData.mCurrentGPSInfo.longitude);
                                if (this.mParent.mEsriMap.IsInitMapScale() == false)
                                {
                                    this.mParent.mEsriMap.SetMapViewPointAsync(this.mParent.mLastLocation);
                                }
                            }
                            else
                            {
                                this.mParent.mLastLocation.Latitude = this.mParent.mSharedData.mCurrentGPSInfo.latitude;
                                this.mParent.mLastLocation.Longitude = this.mParent.mSharedData.mCurrentGPSInfo.longitude;
                            }


                            if (mParent.mEsriMap.GetRoutedProgress())
                            {
                                if (mParent.mSharedData.mCustomSettingInfo != null)
                                {
                                    mParent.xSpeedLabel.Text = string.Format("속도 : {0:0.0}km/h", mParent.mSharedData.mCustomSettingInfo.SimulationSpeed);
                                }
                                else
                                {
                                    mParent.xSpeedLabel.Text = string.Format("속도 : 20.0km/h");
                                }
                            }
                            else
                            {
                                mParent.xSpeedLabel.Text = string.Format("속도 : {0:0.0}km/h", this.mParent.mSharedData.mCurrentGPSInfo.speed);
                            }

                            if (mParent.mEsriMap.GetRoutedProgress())
                            {
                                if (mParent.SimualationLocation != null)
                                {
                                    mParent.xAngleLabel.Text = string.Format("기울기 : {0:0.0}°", this.mParent.SimualationLocation.angle);
                                }
                                else
                                {
                                    mParent.xAngleLabel.Text = string.Format("기울기 : 0.0°");
                                }
                            }
                            else
                            {
                                if (this.mParent.mSharedData.mCurrentGPSInfo.angle > -400.0)
                                {
                                    mParent.xAngleLabel.Text = string.Format("기울기 : {0:0.0}°", this.mParent.mSharedData.mCurrentGPSInfo.angle);
                                }
                                else
                                {
                                    mParent.xAngleLabel.Text = string.Format("기울기 : None");
                                }
                            }
                        }

                        if (this.mParent.mSharedData.mBatterySingle != null)
                        {
                            mParent.xBatteryLabel.Text = string.Format("배터리 : {0}%", this.mParent.mSharedData.mBatterySingle.Energy);
                        }
                        else
                        {
                            mParent.xBatteryLabel.Text = string.Format("배터리 : None");
                        }

                        if (this.mParent.mLastLocation != null)
                        {
                            if (mParent.mEsriMap.GetRoutedProgress())
                            {
                                mParent.mEsriMap.SetCurrentMapPoint(this.mParent.mLastLocation, false);
                            }
                            else
                            {
                                mParent.mEsriMap.SetCurrentMapPoint(this.mParent.mLastLocation, true);
                            }
                        }
                    }
                    catch
                    {

                    }
                });
            }
        }
    }
}