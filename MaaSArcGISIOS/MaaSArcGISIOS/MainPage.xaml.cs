using MaaSArcGISIOS.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaaSArcGISIOS
{
    public partial class MainPage : Shell
    {
        private Data.SingletonData mSharedData;

        private Engine.LocationEngine mLocationEngine;
        private Engine.DeviceEngine mDeviceEngine;
        private Engine.SensorEngine mSensorEngine;
        private Engine.HttpEngine mHTTPEngine;

        private ShellView.DefaultPage mDefaultPage;
        private ShellView.MapNaviationPage mMapNaviationPage;
        private ShellView.SSRViewPage mSSRPage;

        private System.BluetoothLe.Device mSensorDevice = null;

        public MainPage()
        {
            InitializeComponent();
            try
            {
                //ssl verification disable
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                Xamarin.Essentials.DeviceDisplay.KeepScreenOn = true;
                mSharedData = Data.SingletonData.mInstance;

                mDefaultPage = new ShellView.DefaultPage();
                mMapNaviationPage = new ShellView.MapNaviationPage();
                mSSRPage = new ShellView.SSRViewPage();

                xSettingShellContent.Content = new ShellView.SettingPage();
                xDefulatShellContent.Content = mDefaultPage;
                xNaviationShellContent.Content = mMapNaviationPage;
                xSSRShellContent.Content = mSSRPage;

                mLocationEngine = new Engine.LocationEngine();
                mDeviceEngine = new Engine.DeviceEngine();
                mHTTPEngine = new Engine.HttpEngine();

                mLocationEngine.Start(5000);
                mDeviceEngine.Start(1000);
                mHTTPEngine.Start(10);

                CreatingTaskSensorEngine(SensorEngineRunningCallback);
            }
            catch (Exception ets)
            {
                Console.WriteLine("Debug_console : {0}", ets.Message);
            }
        }
        private async void SensorEngineRunningCallback(Tuple<System.BluetoothLe.Adapter, Guid> pCallBackResult)
        {
            try
            {
                await pCallBackResult.Item1.StopScanningForDevicesAsync();
                mSensorEngine = new Engine.SensorEngine(pCallBackResult.Item1, pCallBackResult.Item2);
                mSensorEngine.Start(5);
            }
            catch
            {

            }
        }

        private void CreatingTaskSensorEngine(Action<Tuple<System.BluetoothLe.Adapter, Guid>> pResultActionCallBack)
        {
            try
            {
                var ble = System.BluetoothLe.BluetoothLE.Current;
                System.BluetoothLe.Adapter mAdapter = ble.Adapter;
                mAdapter.ScanTimeout = 7 * 1000;
                mAdapter.ScanMode = System.BluetoothLe.ScanMode.LowLatency;
                System.Threading.Tasks.Task.Run(async () =>
                {
                    mAdapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                    await mAdapter.StartScanningForDevicesAsync();
                    if (mSensorDevice != null)
                    {
                        pResultActionCallBack(new Tuple<System.BluetoothLe.Adapter, Guid>(mAdapter, mSensorDevice.Id));
                        mAdapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
                    }
                    else
                    {
                        this.Dispatcher.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Error", "Device를 찾지 못하였습니다. 어플리케이션을 종료해 주시기 바랍니다.", "OK");
                        });
                    }
                });
            }
            catch (Exception ets)
            {
                Console.WriteLine("Debug_console : {0}", ets.Message);
            }
        }

        private void Adapter_DeviceDiscovered(object sender, System.BluetoothLe.EventArgs.DeviceEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Device.Name))
                {
                    if (e.Device.Name.Trim() == Common.Constant.mBluetoothDeviceName)
                    {
                        mSensorDevice = e.Device;
                    }
                }
            }
            catch(Exception ets)
            {
                Console.WriteLine("Debug_console : {0}",ets.Message);
            }
        }
    }
}
