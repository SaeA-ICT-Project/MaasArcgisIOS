using System;
using System.BluetoothLe.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace MaaSArcGISIOS.Engine
{
    public class SensorEngine : Common.EngineThread
    {
        private Data.SingletonData mSharedData;

        private Dictionary<System.BluetoothLe.Service, IReadOnlyList<System.BluetoothLe.Characteristic>> mServiceList;
        private Dictionary<Guid, Queue<byte[]>> mCharacteristicList;
        private Guid mDeviceGUID;
        private System.BluetoothLe.Adapter mAdapter;
        private bool bPdatDirection = false;
        private List<byte[]> mLeftPDatBuffer;
        private List<byte[]> mRightPDatBuffer;
        private string[] mCSVData = null;
        private List<Model.DeviceSensorMPUFileClass> mMPUFileList = null;
        private Queue<byte[]> mPDatQueue = null;
        private int LastLowIndex = -1;

        public SensorEngine(System.BluetoothLe.Adapter pAdapter, Guid pDeviceGUID)
        {
            mSharedData = Data.SingletonData.mInstance;
            mLeftPDatBuffer = new List<byte[]>();
            mRightPDatBuffer = new List<byte[]>();
            mPDatQueue = new Queue<byte[]>();
            mDeviceGUID = pDeviceGUID;
            mAdapter = pAdapter;

            mAdapter.DeviceConnectionError += PAdapter_DeviceConnectionError;
            mAdapter.DeviceConnectionLost += PAdapter_DeviceConnectionLost;
            mAdapter.DeviceDisconnected += PAdapter_DeviceDisconnected;
            mAdapter.DeviceConnected += PAdapter_DeviceConnected;
            mAdapter.DeviceDiscovered += PAdapter_DeviceDiscovered;

            mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.None;
            mServiceList = new Dictionary<System.BluetoothLe.Service, IReadOnlyList<System.BluetoothLe.Characteristic>>();
            mCharacteristicList = new Dictionary<Guid, Queue<byte[]>>();

            InitFileDataSetting();

            System.Threading.ThreadPool.QueueUserWorkItem(InnerThreadCallback);
        }

        private void InitFileDataSetting()
        {
            string _destpath = System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "AccelCal.csv");
            System.IO.FileStream _fs = new System.IO.FileStream(_destpath, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(_fs);
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();
            string _line;
            while ((_line = sr.ReadLine()) != null)
            {
                result.Add(_line);
                Console.WriteLine("Debug_Console : {0}",_line);
            }
            sr.Close();
            _fs.Close();
            mCSVData = result.Count() == 0 ? null : result.ToArray();
            if (mCSVData != null)
            {
                if (mCSVData.Length > 0)
                {
                    mMPUFileList = new List<Model.DeviceSensorMPUFileClass>();

                    foreach (var _item in mCSVData)
                    {
                        string[] _splititem = _item.Split(',');
                        if (_splititem.Length == 2)
                        {
                            double outStepValue = -99999.0;
                            double outDisplayValue = -9999.0;

                            if (Double.TryParse(_splititem[0], out outStepValue) && double.TryParse(_splititem[1], out outDisplayValue))
                            {
                                mMPUFileList.Add(new Model.DeviceSensorMPUFileClass()
                                {
                                    StepValue = outStepValue,
                                    DisplayValue = outDisplayValue,
                                });
                            }
                        }
                    }
                    mMPUFileList.Add(new Model.DeviceSensorMPUFileClass()
                    {
                        StepValue = 9999.0,
                        DisplayValue = 30.0,
                    });
                }
            }
        }

        private void InnerThreadCallback(object state)
        {
            while (true)
            {
                try
                {
                    if (mPDatQueue.Count() > 0)
                    {
                        MaaSArcGISIOS.Model.SensorCallBack.CallRdrPdatCharId(mSharedData, mPDatQueue.Dequeue());
                    }
                }
                catch
                {

                }
                System.Threading.Thread.Sleep(10);
            }
        }

        private async void PAdapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Device.Name))
            {
                try
                {
                    mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.Connecting;
                    if (mDeviceGUID == e.Device.Id)
                    {
                        await mAdapter.ConnectToKnownDeviceAsync(e.Device.Id);
                    }
                }
                catch (Exception ex)
                {
                    mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.Disconnect;
                    Console.WriteLine("Errro -> {0}", ex.Message);
                }
            }
        }

        private void SetCharateristicsStop()
        {
            foreach (var itme in mServiceList)
            {
                foreach (var inner in itme.Value)
                {
                    try
                    {
                        inner.StopUpdatesAsync();
                    }
                    catch (Exception ets)
                    {
                        Console.WriteLine("value update error0: {0} , {1}", inner.Uuid, ets.Message);
                    }
                }
            }
        }

        private async void PAdapter_DeviceConnected(object sender, System.BluetoothLe.EventArgs.DeviceEventArgs e)
        {
            await mAdapter.StopScanningForDevicesAsync();
            Console.WriteLine("Bluetooth Connection");
            mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.Connection;
            GetBluetoothLEService(e.Device);
        }

        private async void PAdapter_DeviceDisconnected(object sender, System.BluetoothLe.EventArgs.DeviceEventArgs e)
        {
            Console.WriteLine("Bluetooth DisConnection");
            mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.Disconnect;
            SetCharateristicsStop();
            await System.Threading.Tasks.Task.Delay(5000);
            await mAdapter.StartScanningForDevicesAsync();
        }

        private async void PAdapter_DeviceConnectionLost(object sender, System.BluetoothLe.EventArgs.DeviceErrorEventArgs e)
        {
            Console.WriteLine("Bluetooth LostConnection");
            mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.Disconnect;
            SetCharateristicsStop();
            await System.Threading.Tasks.Task.Delay(5000);
            await mAdapter.StartScanningForDevicesAsync();
        }

        private async void PAdapter_DeviceConnectionError(object sender, System.BluetoothLe.EventArgs.DeviceErrorEventArgs e)
        {
            Console.WriteLine("Bluetooth ErrorConnection");
            mSharedData.IsBluetoothConnection = Common.Constant.BluetoothStatus.Disconnect;
            SetCharateristicsStop();
            await System.Threading.Tasks.Task.Delay(5000);
            await mAdapter.StartScanningForDevicesAsync();
        }

        private async void ConnectBluetoothLE(System.BluetoothLe.Adapter pAdapter)
        {
            await pAdapter.StartScanningForDevicesAsync();
        }

        private async void GetBluetoothLEService(System.BluetoothLe.Device pDevice)
        {
            var services = await pDevice.GetServicesAsync();
            foreach (var service in services)
            {
                if (!mServiceList.ContainsKey(service))
                {
                    mServiceList.Add(service, await service.GetCharacteristicsAsync());
                }
            }
            await System.Threading.Tasks.Task.Delay(1000);
            SetCharateristicEvent();
        }

        private void SetCharateristicEvent()
        {
            foreach (var itme in mServiceList)
            {
                foreach (var inner in itme.Value)
                {
                    try
                    {
                        if (inner.CanUpdate)
                        {
                            if (!mCharacteristicList.ContainsKey(inner.Id))
                            {
                                mCharacteristicList.Add(inner.Id, new Queue<byte[]>());
                            }
                            inner.ValueUpdated += Characteristic_ValueUpdated;
                            inner.StartUpdatesAsync();
                        }
                    }
                    catch (Exception ets)
                    {
                        Console.WriteLine("value update error0: {0} , {1}", inner.Uuid, ets.Message);
                    }
                }
            }
        }

        private void Characteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            try
            {
                if (e.Characteristic.CanUpdate)
                {
                    if (mCharacteristicList.ContainsKey(e.Characteristic.Id))
                    {
                        mCharacteristicList[e.Characteristic.Id].Enqueue(e.Characteristic.Value);
                    }
                }
            }
            catch (Exception ets)
            {

            }
        }

        private void GetQueueData(Guid pGuid)
        {
            if (mCharacteristicList.ContainsKey(pGuid))
            {
                if (mCharacteristicList[pGuid].Count > 0)
                {
                    if (pGuid == Common.Constant.MPURawCharId)
                    {
                        CacaulateSensorMPUData(pGuid, mSharedData, mCharacteristicList[pGuid].Dequeue());
                    }
                    else if (pGuid == Common.Constant.RdrPdatCharId)
                    {
                        CacaulateSensorPDatData(pGuid, mSharedData, mCharacteristicList[pGuid].Dequeue());
                    }
                    else
                    {
                        CacaulateSensorData(pGuid, mSharedData, mCharacteristicList[pGuid].Dequeue());
                    }
                }
            }
        }

        private void CacaulateSensorMPUData(Guid pGuid, Data.SingletonData pShared, byte[] pBuffer)
        {
            try
            {
                var _AccelerData = new Model.DeviceSensorClass.SensorMPURaw();
                _AccelerData.Deserilize(pBuffer);
                if (mMPUFileList != null)
                {
                    var _datestr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    Console.WriteLine("{0} -> X:{1:0.###},Y:{2:0.###},Z:{3:0.###}", _datestr, ((double)_AccelerData.X / 1000.0), ((double)_AccelerData.Y / 1000.0), ((double)_AccelerData.Z / 1000.0));
                    var _where_XArray = (int)Math.Round(mMPUFileList.Where(x => (int)(x.StepValue * 1000) > _AccelerData.X).OrderBy(x => x.StepValue).First().DisplayValue * 10);
                    var _where_YArray = (int)Math.Round(mMPUFileList.Where(x => (int)(x.StepValue * 1000) > _AccelerData.Y).OrderBy(x => x.StepValue).First().DisplayValue * 10);
                    var _where_ZArray = (int)Math.Round(mMPUFileList.Where(x => (int)(x.StepValue * 1000) > _AccelerData.Z).OrderBy(x => x.StepValue).First().DisplayValue * 10);

                    if (mSharedData.mMPUInclineSingle == null)
                    {
                        mSharedData.mMPUMaxSingle = new Model.DeviceSensorClass.SensorMPURaw()
                        {
                            TimeCount = 4000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick,
                            X = _where_XArray,
                            Y = _where_YArray,
                            Z = _where_ZArray,
                        };
                    }
                    else
                    {
                        mSharedData.mMPUMaxSingle.TimeCount = 4000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick;
                        mSharedData.mMPUMaxSingle.X = _where_XArray;
                        mSharedData.mMPUMaxSingle.Y = _where_YArray;
                        mSharedData.mMPUMaxSingle.Z = _where_ZArray;
                    }

                    if (pShared.mCustomSettingInfo != null)
                    {
                        if (_where_XArray > (pShared.mCustomSettingInfo.MPURiskLimit * 10) || (_where_YArray > pShared.mCustomSettingInfo.MPURiskLimit * 10) || _where_ZArray > (pShared.mCustomSettingInfo.MPURiskLimit * 10))
                        {
                            if (pShared.mCurrentGPSInfo != null)
                            {
                                if (pShared.mCurrentGPSInfo.latitude > 1.0 & pShared.mCurrentGPSInfo.longitude > 1.0)
                                {
                                    mSharedData.mMPUQueue.Push(new Model.HTTPMPURaw()
                                    {
                                        gps = new Model.HTTPGPSInfo
                                        {
                                            latitude = pShared.mCurrentGPSInfo.latitude,
                                            longitude = pShared.mCurrentGPSInfo.longitude
                                        },
                                        data = new Model.HTTPInnerMPU
                                        {
                                            x = (float)_where_XArray / 10.0f,
                                            y = (float)_where_YArray / 10.0f,
                                            z = (float)_where_ZArray / 10.0f,
                                            wheelSize = pShared.mCustomSettingInfo == null ? 3.0 : pShared.mCustomSettingInfo.WheelSize,
                                            recordDate = _datestr,
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void CacaulateSensorPDatData(Guid pGuid, Data.SingletonData pShared, byte[] pBuffer)
        {
            try
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length > 0)
                    {
                        if (LastLowIndex >= (int)pBuffer[7])
                        {
                            if (bPdatDirection == false)
                            {
                                if (mRightPDatBuffer.Count > 0)
                                {
                                    byte[] mGetterBuffer = new byte[mRightPDatBuffer.Count * 8];
                                    for (int i = 0; i < mRightPDatBuffer.Count; i++)
                                    {
                                        Buffer.BlockCopy(mRightPDatBuffer[i], 0, mGetterBuffer, i * 8, 8);
                                    }
                                    mPDatQueue.Enqueue(mGetterBuffer);
                                }
                                mLeftPDatBuffer.Clear();
                            }
                            else
                            {
                                if (mLeftPDatBuffer.Count > 0)
                                {
                                    byte[] mGetterBuffer = new byte[mLeftPDatBuffer.Count * 8];
                                    for (int i = 0; i < mLeftPDatBuffer.Count; i++)
                                    {
                                        Buffer.BlockCopy(mLeftPDatBuffer[i], 0, mGetterBuffer, i * 8, 8);
                                    }
                                    mPDatQueue.Enqueue(mGetterBuffer);
                                    mRightPDatBuffer.Clear();
                                }
                            }
                            bPdatDirection = !bPdatDirection;
                        }
                        else
                        {
                            if (bPdatDirection == false)
                            {
                                mRightPDatBuffer.Add(pBuffer);
                            }
                            else
                            {
                                mLeftPDatBuffer.Add(pBuffer);
                            }
                        }
                        LastLowIndex = (int)pBuffer[7];
                    }
                }
            }
            catch
            {

            }
        }

        private void CacaulateSensorData(Guid pGuid, Data.SingletonData pShared, byte[] pBuffer)
        {
            if (Common.Constant.EnvPressCharId == pGuid)
            {
                MaaSArcGISIOS.Model.SensorCallBack.CallEnvPressCharId(pShared, pBuffer);
            }
            else if (Common.Constant.RdrTdatCharId == pGuid)
            {
                MaaSArcGISIOS.Model.SensorCallBack.CallRdrTdatCharId(pShared, pBuffer);
            }
            else if (Common.Constant.BatteryServiceId == pGuid)
            {
                MaaSArcGISIOS.Model.SensorCallBack.CallBatteryLevelId(pShared, pBuffer);
            }
        }

        protected override void begin()
        {
            try
            {
                ConnectBluetoothLE(mAdapter);
            }
            catch (Exception ets)
            {
                Console.WriteLine("console_debug : {0}", ets.Message);
            }
        }

        protected override void end()
        {
        }

        protected override void loop()
        {
            GetQueueData(Common.Constant.EnvPressCharId);
            GetQueueData(Common.Constant.MPURawCharId);
            GetQueueData(Common.Constant.RdrPdatCharId);
            GetQueueData(Common.Constant.RdrTdatCharId);
            GetQueueData(Common.Constant.BatteryServiceId);
        }
    }
}
