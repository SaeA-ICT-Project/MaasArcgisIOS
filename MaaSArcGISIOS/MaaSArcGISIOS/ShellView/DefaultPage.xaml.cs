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
    public partial class DefaultPage : ContentPage
    {
        private Xamarin.Essentials.Location mLastLocation = null;
        private Common.EsriMapComponent mEsriMap;
        private DefaultMapEngine mMapEngine;
        private bool bAutoFlag = false;
        public DefaultPage()
        {
            InitializeComponent();
            mEsriMap = new Common.EsriMapComponent(
                CatchStringCallBack,
                RouteStringCurrentSiteCallBack,
                RouteStringCurrentDirectionManeuverCallBack,
                RouteStringFutureDirectionManeuverCallBack,
                RouteLongSearchTimeCallBack,
                RouteDisplayLocationCallBack,
                RouteFutureLocationCallBack,
                false);
            mEsriMap.CreateMap();
            xEsriMapLayout.Children.Add(mEsriMap.GetMapView());
            mMapEngine = new DefaultMapEngine(this);
            mMapEngine.Start(MaaSArcGISIOS.Common.Constant.mDefualtPageTick);
        }

        private void RouteFutureLocationCallBack(Xamarin.Essentials.Location obj, Xamarin.Essentials.Location pSource)
        {

        }

        private void RouteDisplayLocationCallBack(Xamarin.Essentials.Location obj)
        {

        }

        private void RouteLongSearchTimeCallBack(long obj, int ttss)
        {
        }

        private void AutoCenterSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            bAutoFlag = e.Value;
            //mEsriMap.SetAutoCenterControl(bAutoFlag);
        }

        private void RouteStringFutureDirectionManeuverCallBack(string pCallBack)
        {

        }
        private void RouteStringCurrentDirectionManeuverCallBack(string pCallBack)
        {

        }
        private void RouteStringCurrentSiteCallBack(string pCallBack)
        {

        }
        private void CatchStringCallBack(string pCallBack)
        {

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

        private class DefaultMapEngine : Common.EngineThread
        {
            private Data.SingletonData mSharedData;
            private DefaultPage mParent;
            private int count = 0;

            public DefaultMapEngine(DefaultPage pParentPage)
            {
                mParent = pParentPage;
                mSharedData = Data.SingletonData.mInstance;
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
                        switch (this.mSharedData.IsBluetoothConnection)
                        {
                            case Common.Constant.BluetoothStatus.Connecting:
                                mParent.xGridPanel.BackgroundColor = System.Drawing.Color.Green;
                                break;
                            case Common.Constant.BluetoothStatus.Connection:
                                mParent.xGridPanel.BackgroundColor = System.Drawing.Color.DodgerBlue;
                                break;
                            case Common.Constant.BluetoothStatus.None:
                                mParent.xGridPanel.BackgroundColor = System.Drawing.Color.DimGray;
                                break;
                            default:
                                mParent.xGridPanel.BackgroundColor = System.Drawing.Color.OrangeRed;
                                break;
                        }
                        mParent.xLastUpdateTimeLabel.Text = this.mSharedData.mLocationLastUpdateTime == null ? "" : this.mSharedData.mLocationLastUpdateTime;

                        if (this.mSharedData.mCurrentGPSInfo != null)
                        {
                            if (this.mParent.mLastLocation == null)
                            {
                                this.mParent.mLastLocation = new Xamarin.Essentials.Location(this.mSharedData.mCurrentGPSInfo.latitude, this.mSharedData.mCurrentGPSInfo.longitude);
                                if (this.mParent.mEsriMap.IsInitMapScale() == false)
                                {
                                    this.mParent.mEsriMap.SetMapViewPointAsync(this.mParent.mLastLocation);
                                }
                            }
                            else
                            {
                                this.mParent.mLastLocation.Latitude = this.mSharedData.mCurrentGPSInfo.latitude;
                                this.mParent.mLastLocation.Longitude = this.mSharedData.mCurrentGPSInfo.longitude;
                            }



                            mParent.xLocationInfoLabel.Text = string.Format("{0:0.000000}°, {1:0.000000}°, {2:0.0##}m", this.mSharedData.mCurrentGPSInfo.latitude, this.mSharedData.mCurrentGPSInfo.longitude, this.mSharedData.mCurrentGPSInfo.altitude < -9999.0 ? 0.0 : this.mSharedData.mCurrentGPSInfo.altitude);
                            mParent.xSpeedLabel.Text = string.Format("속도 : {0:0.0}km/h", this.mSharedData.mCurrentGPSInfo.speed);

                            if (this.mSharedData.mCurrentGPSInfo.angle > -400.0)
                            {
                                mParent.xAngleLabel.Text = string.Format("기울기 : {0:0.0}°", this.mSharedData.mCurrentGPSInfo.angle);
                            }
                            else
                            {
                                mParent.xAngleLabel.Text = string.Format("기울기 : None");
                            }
                        }

                        if (mParent.bAutoFlag)
                        {
                            if (count == 100)
                            {
                                if (this.mParent.mLastLocation != null)
                                {
                                    mParent.mEsriMap.SetMapViewCenterAsync(this.mParent.mLastLocation);
                                }
                                count = 0;
                            }
                            count++;
                        }
                        else
                        {
                            count = 0;
                        }


                        if (this.mSharedData.mPdatRadarSingle != null)
                        {
                            try
                            {
                                mParent.xBehindSensorStackLayout.Children.Clear();
                                mParent.xBehindSensorCountLabel.Text = String.Format("후방 감지 : {0}", this.mSharedData.mPdatRadarSingle.RadarInfo.Length);
                                for (int i = 0; i < this.mSharedData.mPdatRadarSingle.RadarInfo.Length; i++)
                                {
                                    mParent.xBehindSensorStackLayout.Children.Add(new Label
                                    {
                                        Text = string.Format("거리 : {0:0.0}m , 속도 : {1:0.0}km/h , 각도 : {2}{3:0.0}˚",
                                        this.mSharedData.mPdatRadarSingle.RadarInfo[i].Distance,
                                        this.mSharedData.mPdatRadarSingle.RadarInfo[i].Speed,
                                        this.mSharedData.mPdatRadarSingle.RadarInfo[i].Angle >= 0.0 ? "L" : "R",
                                        Math.Abs(this.mSharedData.mPdatRadarSingle.RadarInfo[i].Angle)),
                                        HorizontalOptions = LayoutOptions.Start,
                                        TextColor = Color.White,
                                    });
                                }
                                if (this.mSharedData.mPdatRadarSingle.TimeCount > 0)
                                {
                                    this.mSharedData.mPdatRadarSingle.TimeCount--;
                                }
                                else
                                {
                                    this.mSharedData.mPdatRadarSingle = null;
                                }
                            }
                            catch
                            {

                            }


                            try
                            {
                                if (this.mSharedData.mPdatRadarSingle.RadarInfo.Length > 0)
                                {
                                    var minDistance = this.mSharedData.mPdatRadarSingle.RadarInfo.Min(x => x.Distance);

                                    if (mSharedData.mCustomSettingInfo != null)
                                    {
                                        mParent.xBackDistanceProgress.Progress = minDistance > mSharedData.mCustomSettingInfo.BackDistanceWarning ? 0.0 : ((mSharedData.mCustomSettingInfo.BackDistanceWarning - minDistance) / (mSharedData.mCustomSettingInfo.BackDistanceWarning));
                                    }
                                }
                                else
                                {
                                    mParent.xBackDistanceProgress.Progress = 0.0;
                                }
                            }
                            catch
                            {
                                mParent.xBackDistanceProgress.Progress = 0.0;
                            }
                        }
                        else
                        {
                            mParent.xBehindSensorStackLayout.Children.Clear();
                            mParent.xBehindSensorCountLabel.Text = String.Format("후방 감지 : 0개");
                        }

                        if (this.mSharedData.mBatterySingle != null)
                        {
                            mParent.xBatteryLabel.Text = string.Format("배터리 : {0}%", this.mSharedData.mBatterySingle.Energy);
                        }
                        else
                        {
                            mParent.xBatteryLabel.Text = string.Format("배터리 : None");
                        }

                        if (this.mSharedData.mPressureSingle != null)
                        {
                            mParent.xLocationAngleLabel.Text = string.Format("{0:0.0##}Pa", this.mSharedData.mPressureSingle.Pressure);
                            if (this.mSharedData.mPressureSingle.TimeCount > 0)
                            {
                                this.mSharedData.mPressureSingle.TimeCount--;
                            }
                            else
                            {
                                this.mSharedData.mPressureSingle = null;
                            }
                        }
                        else
                        {
                            mParent.xLocationAngleLabel.Text = string.Format("None");
                        }

                        if (this.mSharedData.mMPUMaxSingle != null)
                        {
                            //m/s²
                            this.mParent.xXAxisMPULabel.Text = String.Format("{0}.{1}", this.mSharedData.mMPUMaxSingle.X / 10, this.mSharedData.mMPUMaxSingle.X % 10);
                            this.mParent.xYAxisMPULabel.Text = String.Format("{0}.{1}", this.mSharedData.mMPUMaxSingle.Y / 10, this.mSharedData.mMPUMaxSingle.Y % 10);
                            this.mParent.xZAxisMPULabel.Text = String.Format("{0}.{1}", this.mSharedData.mMPUMaxSingle.Z / 10, this.mSharedData.mMPUMaxSingle.Z % 10);

                            if (this.mSharedData.mMPUMaxSingle.TimeCount > 0)
                            {
                                this.mSharedData.mMPUMaxSingle.TimeCount--;
                            }
                            else
                            {
                                this.mSharedData.mMPUMaxSingle = null;
                            }
                        }
                        else
                        {
                            this.mParent.xXAxisMPULabel.Text = string.Format("None");
                            this.mParent.xYAxisMPULabel.Text = string.Format("None");
                            this.mParent.xZAxisMPULabel.Text = string.Format("None");
                        }

                        if (this.mSharedData.mMPUInclineSingle != null)
                        {
                            this.mParent.xXAxisDeviationMPULabel.Text = String.Format("{0}{1}.{2}m/s²", this.mSharedData.mMPUInclineSingle.X < 0 ? "-" : "", Math.Abs(this.mSharedData.mMPUInclineSingle.X) / 100, Math.Abs(this.mSharedData.mMPUInclineSingle.X) % 100);

                            this.mParent.xYAxisDeviationMPULabel.Text = String.Format("{0}{1}.{2}m/s²", this.mSharedData.mMPUInclineSingle.Y < 0 ? "-" : "", Math.Abs(this.mSharedData.mMPUInclineSingle.Y) / 100, Math.Abs(this.mSharedData.mMPUInclineSingle.Y) % 100);

                            this.mParent.xZAxisDeviationMPULabel.Text = String.Format("{0}{1}.{2}m/s²", this.mSharedData.mMPUInclineSingle.Z < 0 ? "-" : "", Math.Abs(this.mSharedData.mMPUInclineSingle.Z) / 100, Math.Abs(this.mSharedData.mMPUInclineSingle.Z) % 100);

                            if (this.mSharedData.mMPUInclineSingle.TimeCount > 0)
                            {
                                this.mSharedData.mMPUInclineSingle.TimeCount--;
                            }
                            else
                            {
                                this.mSharedData.mMPUInclineSingle = null;
                            }
                        }
                        else
                        {
                            this.mParent.xXAxisDeviationMPULabel.Text = string.Format("None");
                            this.mParent.xYAxisDeviationMPULabel.Text = string.Format("None");
                            this.mParent.xZAxisDeviationMPULabel.Text = string.Format("None");
                        }

                        if (this.mParent.mLastLocation != null)
                        {
                            if (mParent.mEsriMap.GetRoutedProgress())
                            {

                            }
                            else
                            {
                                mParent.mEsriMap.SetCurrentMapPoint(this.mParent.mLastLocation, true);
                            }
                        }
                    }
                    catch (Exception ets)
                    {

                    }
                });
            }
        }
    }
}