using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Data
{
    public class SingletonData
    {
        private static readonly Lazy<SingletonData> _Instance = new Lazy<SingletonData>(() => new SingletonData());
        public static SingletonData mInstance
        {
            get
            {
                return _Instance.Value;
            }
        }

        public Model.LoginUserOauth mOauthUserInfo;

        public Common.RingQueue<Model.HTTPPressure> mPressureQueue;
        public Common.RingQueue<Model.HTTPMPURaw> mMPUQueue;
        public Common.RingQueue<Model.HTTPRadarPdat> mPdatQueue;

        public Common.RingQueue<Model.HTTPPressure> mSimulationPressureQueue;
        public Common.RingQueue<Model.HTTPMPURaw> mSimulationMPUQueue;
        public Common.RingQueue<Model.HTTPRadarPdat> mSimulationPdatQueue;

        public Common.RingQueue<Model.HTTPGNSSClock> mGNSSClockQueue;
        public Common.RingQueue<Model.HTTPGNSSMeasurement> mGNSSMeasurementQueue;
        public Common.RingQueue<Model.HTTPSSR> mSSRDictionary;

        public Model.DeviceSensorClass.SensorRdrPdat mPdatRadarSingle;
        public Model.DeviceSensorClass.SensorRdrTdat mTdatRadarSingle;
        public Model.DeviceSensorClass.SensorMPURaw mMPUMaxSingle;
        public Model.DeviceSensorClass.SensorMPURaw mMPUInclineSingle;

        public double mZeroPressureSingle = -1.0;
        public Model.DeviceSensorClass.SensorPressure mPressureSingle;
        public Model.DeviceSensorClass.SensorBattery mBatterySingle;
        public Model.DeviceSensorClass.SensorGPS mSensorGPSSingle;
        public Model.DeviceGPSInfo mCurrentGPSInfo;
        public Model.CustonSetter mCustomSettingInfo;


        public double mPdatDistanceFlag = 1.0;
        public double mPDatAngleFlag = 1.0;
        public double mLocationQueueSize = 7.0;
        public string mLocationLastUpdateTime = "";

        public Common.Constant.BluetoothStatus IsBluetoothConnection = Common.Constant.BluetoothStatus.None;

        public SingletonData()
        {
            mPressureQueue = new Common.RingQueue<Model.HTTPPressure>(10, new System.Collections.Generic.Queue<Model.HTTPPressure>());
            mMPUQueue = new Common.RingQueue<Model.HTTPMPURaw>(10, new System.Collections.Generic.Queue<Model.HTTPMPURaw>());
            mPdatQueue = new Common.RingQueue<Model.HTTPRadarPdat>(10, new Queue<Model.HTTPRadarPdat>());

            mSimulationPressureQueue = new Common.RingQueue<Model.HTTPPressure>(10, new System.Collections.Generic.Queue<Model.HTTPPressure>());
            mSimulationMPUQueue = new Common.RingQueue<Model.HTTPMPURaw>(10, new System.Collections.Generic.Queue<Model.HTTPMPURaw>());
            mSimulationPdatQueue = new Common.RingQueue<Model.HTTPRadarPdat>(10, new Queue<Model.HTTPRadarPdat>());

            mGNSSClockQueue = new Common.RingQueue<Model.HTTPGNSSClock>(10, new Queue<Model.HTTPGNSSClock>());
            mGNSSMeasurementQueue = new Common.RingQueue<Model.HTTPGNSSMeasurement>(100, new Queue<Model.HTTPGNSSMeasurement>());

            mSSRDictionary = new Common.RingQueue<Model.HTTPSSR>(100, new Queue<Model.HTTPSSR>());
        }
    }
}
