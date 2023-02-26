using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;
using Xamarin.Essentials;

namespace MaaSArcGISIOS.Engine
{
    public class LocationEngine : Common.EngineThread
    {
        private Data.SingletonData mSharedData;
        private GeolocationRequest mRequest;

        public LocationEngine()
        {
            mSharedData = Data.SingletonData.mInstance;
        }

        protected override void begin()
        {
            mRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
            GetInitLocationData();
        }

        protected override void end()
        {
        }
        protected override void loop()
        {
            GetLocationData();
        }

        private void GetInitLocationData()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Geolocation.GetLastKnownLocationAsync();
            });
        }

        private void GetLocationData()
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var pLocation = await Geolocation.GetLocationAsync(mRequest);
                    if (pLocation != null)
                    {
                        var _altitude = Math.Round(Common.LocationRestAPI.GetGoogleMapAlitutde(pLocation), 3);

                        if (_altitude < -999.0)
                        {
                            _altitude = pLocation.Altitude.HasValue ? pLocation.Altitude.Value : 0.0;
                        }

                        if (mSharedData.mCurrentGPSInfo == null)
                        {
                            mSharedData.mLocationLastUpdateTime = String.Format("{0}-> y_dist : {1}m , x_dist : {2}m", DateTime.Now.ToString("HH:mm:ss"), 0, 0);
                            mSharedData.mCurrentGPSInfo = new Model.DeviceGPSInfo
                            {
                                latitude = Math.Round(pLocation.Latitude, 7),
                                longitude = Math.Round(pLocation.Longitude, 7),
                                speed = pLocation.Speed.HasValue ? Math.Round(pLocation.Speed.Value * 3.6, 2) : 0.0,
                                distance = 0.0,
                                altitude = _altitude,
                                angle = -999.0,
                                datetimes = DateTime.Now
                            };
                        }
                        else
                        {
                            var source = new Xamarin.Essentials.Location(mSharedData.mCurrentGPSInfo.latitude, mSharedData.mCurrentGPSInfo.longitude);
                            var target = new Xamarin.Essentials.Location(Math.Round(pLocation.Latitude, 7), Math.Round(pLocation.Longitude, 7));
                            var distance_heigth = Math.Round(Math.Round(_altitude > -999.0 ? _altitude : pLocation.Altitude.Value, 3) - mSharedData.mCurrentGPSInfo.altitude, 3);
                            var distance_width = Math.Round(Xamarin.Essentials.Location.CalculateDistance(source, target, Xamarin.Essentials.DistanceUnits.Kilometers) * 1000.0, 3);
                            var timestpans = DateTime.Now - mSharedData.mCurrentGPSInfo.datetimes;

                            mSharedData.mCurrentGPSInfo.distance = distance_width;
                            mSharedData.mCurrentGPSInfo.speed = pLocation.Speed.HasValue ? Math.Round(pLocation.Speed.Value * 3.6, 2) : 0.0;
                            mSharedData.mCurrentGPSInfo.latitude = Math.Round(pLocation.Latitude, 7);
                            mSharedData.mCurrentGPSInfo.longitude = Math.Round(pLocation.Longitude, 7);
                            mSharedData.mCurrentGPSInfo.altitude = _altitude;
                            mSharedData.mCurrentGPSInfo.datetimes = DateTime.Now;

                            mSharedData.mLocationLastUpdateTime = String.Format("{0}-> y_dist : {1}m , x_dist : {2}m", DateTime.Now.ToString("HH:mm:ss"), distance_heigth, distance_width);

                            if (Math.Abs(distance_heigth) < 0.03 || Math.Abs(distance_width) < 0.1)
                            {
                                mSharedData.mCurrentGPSInfo.angle = 0.0;
                            }
                            else
                            {
                                mSharedData.mCurrentGPSInfo.angle = Math.Atan2(distance_heigth, distance_width) * (180.0 / Math.PI);
                            }
                        }
                    }
                });
            }
            catch
            {

            }
        }
    }
}
