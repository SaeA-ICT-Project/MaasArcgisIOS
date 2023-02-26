using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms;

namespace MaaSArcGISIOS.Engine
{
    public class DeviceEngine : Common.EngineThread
    {
        private Data.SingletonData mSharedData;
        private GeolocationRequest mRequest;
        public DeviceEngine()
        {
            mSharedData = Data.SingletonData.mInstance;


        }

        protected override void begin()
        {
            mRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
        }

        protected override void end()
        {
        }
        protected override void loop()
        {
            try
            {
                if (mSharedData.mCurrentGPSInfo != null && mSharedData.mPressureSingle != null)
                {
                    if (mSharedData.mCurrentGPSInfo.latitude > 1.0 & mSharedData.mCurrentGPSInfo.longitude > 1.0)
                    {
                        mSharedData.mPressureQueue.Push(new MaaSArcGISIOS.Model.HTTPPressure()
                        {
                            gps = new MaaSArcGISIOS.Model.HTTPGPSInfo()
                            {
                                latitude = mSharedData.mCurrentGPSInfo.latitude,
                                longitude = mSharedData.mCurrentGPSInfo.longitude,
                            },
                            data = new MaaSArcGISIOS.Model.HTTPInnerPressure()
                            {
                                altitude = mSharedData.mCurrentGPSInfo.altitude,
                                inclination = mSharedData.mCurrentGPSInfo.angle,
                                pressure = mSharedData.mPressureSingle.Pressure,
                                recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            },
                        });
                    }
                }
            }
            catch
            {

            }
        }
    }
}
