using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaaSArcGISIOS.Data;

namespace MaaSArcGISIOS.Model
{
    public class SensorCallBack
    {
        internal static void CallConServiceId(Data.SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallConServiceId");
        }

        internal static void CallDefaultDeviceName(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("DeviceName : {0}", System.Text.Encoding.Default.GetString(pBuffer));
        }

        internal static void CallDefaultAppearance(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("Appearance : {0}", System.Text.Encoding.Default.GetString(pBuffer));
        }

        internal static void CallDefaultPeripheral(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("Peripheral : {0}", System.Text.Encoding.Default.GetString(pBuffer));
        }

        internal static void CallDefaultCentralAddressResolution(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CentralAddressResolution : {0}", System.Text.Encoding.Default.GetString(pBuffer));
        }

        internal static void CallConDeviceNameCharId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("Config DeviceName : {0}", System.Text.Encoding.ASCII.GetString(pBuffer));
        }

        internal static void CallConAdvParamCharId(SingletonData pShared, byte[] pBuffer)
        {
        }

        internal static void CallConConnParamCharId(SingletonData pShared, byte[] pBuffer)
        {
        }

        internal static void CallConFWVersionCharId(SingletonData pShared, byte[] pBuffer)
        {
        }

        internal static void CallConMTUCharId(SingletonData pShared, byte[] pBuffer)
        {
        }

        internal static void CallEnvServiceId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallEnvServiceId length : {0}", pBuffer.Length);
        }

        internal static void CallEnvPressCharId(SingletonData pShared, byte[] pBuffer)
        {
            //LPS22HB 16Page 참고. ( H | L | XL ) / 4096.0
            try
            {
                var _PressureData = new Model.DeviceSensorClass.SensorPressure() { TimeCount = 4000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick };
                _PressureData.Deserilize(pBuffer);
                if (_PressureData.bDeserilize)
                {
                    if (pShared.mPressureSingle == null)
                    {
                        pShared.mPressureSingle = _PressureData;
                    }
                    else
                    {
                        pShared.mPressureSingle.Pressure = _PressureData.Pressure;
                        pShared.mPressureSingle.TimeCount = 4000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick;
                    }
                }
            }
            catch
            {

            }
        }

        internal static void CallEnvTempCharId(SingletonData pShared, byte[] pBuffer)
        {
            //LPS22HB 46Page 추가 참고없음. (H | L)
            var _TemperData = new Model.DeviceSensorClass.SensorTemper() { TimeCount = 10000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick };
            if (_TemperData.bDeserilize)
            {
                _TemperData.Deserilize(pBuffer);
            }
            //pShared.mTemperatureQueue.Push(_Temper);
        }

        internal static void CallEnvContCharId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("ReadCallEnvContCharId length : {0}", pBuffer.Length);
        }

        internal static void CallMPUServiceId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallMPUServiceId length : {0}", pBuffer.Length);
        }


        internal static void CallMPUContCharId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallMPUContCharId length : {0}", pBuffer.Length);
        }

        internal static void CallRadarServiceId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallRadarServiceId length : {0}", pBuffer.Length);
        }

        internal static void CallRdrPdatCharId(SingletonData pShared, byte[] pBuffer)
        {
            double _PDatDist = pShared.mPdatDistanceFlag;

            try
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length > 0)
                    {
                        var _RadarData = new Model.DeviceSensorClass.SensorRdrPdat(pBuffer.Length / 8) { TimeCount = 10000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick };
                        _RadarData.Deserilize(pBuffer);

                        var _DataPoint = new System.Collections.Generic.HashSet<DataPoint>();
                        var RadarMap = new System.Collections.Generic.Dictionary<int, Model.DeviceSensorClass.SensorRadar>();
                        RadarMap.Clear();
                        foreach (var _item in _RadarData.RadarInfo)
                        {
                            int _AngleInt = (int)(Math.Round(_item.Angle * 1000));
                            if (RadarMap.ContainsKey(_AngleInt))
                            {
                                if (RadarMap[_AngleInt].Distance > _item.Distance)
                                {
                                    RadarMap[_AngleInt] = _item;
                                }
                            }
                            else
                            {
                                RadarMap.Add(_AngleInt, _item);
                            }
                        }

                        int _count = 0;
                        foreach (var _item in RadarMap)
                        {
                            if (Math.Abs(_item.Value.Distance) < 20.0)
                            {
                                _DataPoint.Add(new DataPoint(_count, Math.Round(_item.Value.Distance, 3), Math.Round(_item.Value.Angle, 3), Math.Round(_item.Value.Speed, 3)));
                                _count++;
                            }
                        }

                        var metric = new DataPointDissimilarityMetric();
                        var linkage = new Aglomera.Linkage.AverageLinkage<DataPoint>(metric);
                        var algorithm = new Aglomera.AgglomerativeClusteringAlgorithm<DataPoint>(linkage);
                        var clusteringResult = algorithm.GetClustering(_DataPoint);

                        var caculateResult = clusteringResult.Where(values => values.Dissimilarity <= _PDatDist).OrderBy(key => key.Dissimilarity).Last();
                        System.Collections.Generic.List<Model.DeviceSensorClass.SensorRadar> ResultDataList = new System.Collections.Generic.List<Model.DeviceSensorClass.SensorRadar>();
                        foreach (var _item in caculateResult)
                        {
                            _count = 0;
                            double _x = 0.0;
                            double _y = 0.0;
                            double _speed = 0.0;
                            foreach (var _inner in _item)
                            {
                                _x = _x + _inner.mXValue;
                                _y = _y + _inner.mYValue;
                                _speed = _speed + _inner.mETCValue;
                                _count++;
                            }
                            _x = _x / (double)_count;
                            _y = _y / (double)_count;
                            _speed = _speed / (double)_count;

                            double _distance = Math.Sqrt(Math.Pow(_x, 2) + Math.Pow(_y, 2));
                            double _angle = Math.Asin(_x / _distance) * (180.0 / Math.PI);

                            ResultDataList.Add(new DeviceSensorClass.SensorRadar()
                            {
                                Angle = Math.Round(_angle, 3),
                                Distance = Math.Round(_distance, 3),
                                Speed = Math.Round(_speed, 3),
                            });
                        }

                        if (_RadarData.bDeserilize)
                        {
                            pShared.mPdatRadarSingle = new DeviceSensorClass.SensorRdrPdat(ResultDataList.Count())
                            {
                                TimeCount = 2000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick,
                            };

                            for (int i = 0; i < ResultDataList.Count(); i++)
                            {
                                pShared.mPdatRadarSingle.RadarInfo[i] = ResultDataList[i];
                            }

                            if (pShared.mCurrentGPSInfo != null)
                            {
                                if (pShared.mCurrentGPSInfo.latitude > 1.0 & pShared.mCurrentGPSInfo.longitude > 1.0)
                                {
                                    pShared.mPdatQueue.Push(new HTTPRadarPdat()
                                    {
                                        gps = new HTTPGPSInfo
                                        {
                                            latitude = pShared.mCurrentGPSInfo.latitude,
                                            longitude = pShared.mCurrentGPSInfo.longitude,
                                        },
                                        data = new HTTPInnerPDat
                                        {
                                            objectsCount = ResultDataList.Count(),
                                            recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
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

        internal static void CallRdrTdatCharId(SingletonData pShared, byte[] pBuffer)
        {
            try
            {
                var _RadarData = new Model.DeviceSensorClass.SensorRdrTdat() { TimeCount = 10000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick };
                _RadarData.Deserilize(pBuffer);
                if (_RadarData.bDeserilize)
                {
                    if (pShared.mTdatRadarSingle == null)
                    {
                        pShared.mTdatRadarSingle = _RadarData;
                    }
                    else
                    {
                        pShared.mTdatRadarSingle.RadarInfo = _RadarData.RadarInfo;
                        pShared.mTdatRadarSingle.TimeCount = 10000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick;
                    }
                }
            }
            catch
            {

            }
        }

        internal static void CallRdrContCharId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("ReadCallRdrContCharId length : {0}", pBuffer.Length);
        }

        internal static void CallGPSServiceId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallGPSServiceId length : {0}", pBuffer.Length);
        }

        internal static void CallGPSRMCCharId(SingletonData pShared, byte[] pBuffer)
        {
        }

        internal static void CallGPSContCharId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("ReadCallGPSContCharId length : {0}", pBuffer.Length);
        }

        internal static void CallBatteryServiceId(SingletonData pShared, byte[] pBuffer)
        {
            Console.WriteLine("CallBatteryServiceId length : {0}", pBuffer.Length);
        }

        internal static void CallBatteryLevelId(SingletonData pShared, byte[] pBuffer)
        {
            try
            {
                var _BatteryData = new Model.DeviceSensorClass.SensorBattery() { TimeCount = 10000 / MaaSArcGISIOS.Common.Constant.mDefualtPageTick };
                _BatteryData.Deserilize(pBuffer);
                if (_BatteryData.bDeserilize)
                {
                    if (pShared.mBatterySingle == null)
                    {
                        pShared.mBatterySingle = _BatteryData;
                    }
                    else
                    {
                        pShared.mBatterySingle.Energy = _BatteryData.Energy;
                    }
                }
            }
            catch
            {

            }
        }
    }
}
