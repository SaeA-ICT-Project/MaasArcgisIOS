using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Engine
{
    public class HttpEngine : Common.EngineThread
    {
        private Data.SingletonData mSharedData;
        private Common.MSSQLProvider mDBProvider;
        private string mOuathHdaer = "107827137342103490727/Google"; //test
        private string mPressureHttpAPI = "/new/api/rawdata/environ/pressure/";
        private string mMPUHttpAPI = "/new/api/rawdata/mpu/raw/";
        private string mPdatHttpAPI = "/new/api/rawdata/radar/pdat/";
        private string mSimulationPressureHttpAPI = "/new/api/rawdata/simulation/pressure/";
        private string mSimulationMpuHttpAPI = "/new/api/rawdata/simulation/mpu/";
        private string mSimulationPdatHttpAPI = "/new/api/rawdata/simulation/pdat/";
        private string mGNSSClockHttpAPI = "/new/api/rawdata/gnss/clock";
        private string mGNSSMeasureHttpAPI = "/new/api/rawdata/gnss/measurement";
        private string mSSRHttpAPI = "/new/api/rawdata/ssr";

        public HttpEngine()
        {
            mSharedData = Data.SingletonData.mInstance;
        }

        protected override void begin()
        {
            mDBProvider = new Common.MSSQLProvider();
            if (mSharedData.mOauthUserInfo != null)
            {
                mOuathHdaer = string.Format("{0}/{1}", mSharedData.mOauthUserInfo.Providerkey, mSharedData.mOauthUserInfo.LoginProvider);
            }
        }

        protected override void end()
        {
            mDBProvider.CloseMSSQL();
        }

        protected override void loop()
        {
            try
            {
                if (this.mSharedData.mPressureQueue != null)
                {
                    if (!this.mSharedData.mPressureQueue.IsEmpty())
                    {

                        var item = this.mSharedData.mPressureQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.EnvironPressure([user],[type],[RecordDate],[latitude],[longitude],[Pressure],[Altitude],[Inclination]) " +
                            "VALUES({0},'{1}','{2}',{3},{4},{5},{6},{7})",
                            mSharedData.mOauthUserInfo.Providerkey,
                            mSharedData.mOauthUserInfo.LoginProvider,
                           item.data.recordDate,
                           item.gps.latitude,
                           item.gps.longitude,
                           item.data.pressure,
                           item.data.altitude,
                           item.data.inclination);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);

                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}{2}", Common.Constant.mSignUpApiWebPage, mPressureHttpAPI, mOuathHdaer)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mPressureQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mMPUQueue != null)
                {
                    if (!this.mSharedData.mMPUQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mMPUQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.MpuRaw([user],[type],[RecordDate],[latitude],[longitude],[acc_x],[acc_y],[acc_z],[WheelSize]) " +
                            "VALUES({0},'{1}','{2}',{3},{4},{5},{6},{7},{8})",
                            mSharedData.mOauthUserInfo.Providerkey,
                            mSharedData.mOauthUserInfo.LoginProvider,
                           item.data.recordDate,
                           item.gps.latitude,
                           item.gps.longitude,
                           item.data.x,
                           item.data.y,
                           item.data.z,
                           item.data.wheelSize);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);
                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}{2}", Common.Constant.mSignUpApiWebPage, mMPUHttpAPI, mOuathHdaer)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mMPUQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mPdatQueue != null)
                {
                    if (!this.mSharedData.mPdatQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mPdatQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.RadarPdat([user],[type],[RecordDate],[latitude],[longitude],[ObjectsCount]) " +
                            "VALUES({0},'{1}','{2}',{3},{4},{5})",
                            mSharedData.mOauthUserInfo.Providerkey,
                            mSharedData.mOauthUserInfo.LoginProvider,
                           item.data.recordDate,
                           item.gps.latitude,
                           item.gps.longitude,
                           item.data.objectsCount);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);

                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}{2}", Common.Constant.mSignUpApiWebPage, mPdatHttpAPI, mOuathHdaer)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mPdatQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }


            try
            {
                if (this.mSharedData.mSimulationPressureQueue != null)
                {
                    if (!this.mSharedData.mSimulationPressureQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mSimulationPressureQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SEnvironPressure([user],[type],[RecordDate],[latitude],[longitude],[Pressure],[Altitude],[Inclination]) " +
                            "VALUES({0},'{1}','{2}',{3},{4},{5},{6},{7})",
                            mSharedData.mOauthUserInfo.Providerkey,
                            mSharedData.mOauthUserInfo.LoginProvider,
                           item.data.recordDate,
                           item.gps.latitude,
                           item.gps.longitude,
                           item.data.pressure,
                           item.data.altitude,
                           item.data.inclination);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);
                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}{2}", Common.Constant.mSignUpApiWebPage, mSimulationPressureHttpAPI, mOuathHdaer)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mSimulationPressureQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mSimulationMPUQueue != null)
                {
                    if (!this.mSharedData.mSimulationMPUQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mSimulationMPUQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SMpuRaw([user],[type],[RecordDate],[latitude],[longitude],[acc_x],[acc_y],[acc_z],[WheelSize]) " +
                            "VALUES({0},'{1}','{2}',{3},{4},{5},{6},{7},{8})",
                            mSharedData.mOauthUserInfo.Providerkey,
                            mSharedData.mOauthUserInfo.LoginProvider,
                           item.data.recordDate,
                           item.gps.latitude,
                           item.gps.longitude,
                           item.data.x,
                           item.data.y,
                           item.data.z,
                           item.data.wheelSize);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);

                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}{2}", Common.Constant.mSignUpApiWebPage, mSimulationMpuHttpAPI, mOuathHdaer)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mSimulationMPUQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mSimulationPdatQueue != null)
                {
                    if (!this.mSharedData.mSimulationPdatQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mSimulationPdatQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SRadarPdat([user],[type],[RecordDate],[latitude],[longitude],[ObjectsCount]) " +
                            "VALUES({0},'{1}','{2}',{3},{4},{5})",
                            mSharedData.mOauthUserInfo.Providerkey,
                            mSharedData.mOauthUserInfo.LoginProvider,
                           item.data.recordDate,
                           item.gps.latitude,
                           item.gps.longitude,
                           item.data.objectsCount);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);
                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}{2}", Common.Constant.mSignUpApiWebPage, mSimulationPdatHttpAPI, mOuathHdaer)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mSimulationPdatQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mGNSSClockQueue != null)
                {
                    if (!this.mSharedData.mGNSSClockQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mGNSSClockQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.GNSSClock([RecordDate],[BiasNanos],[BiasUncertaintyNanos],[FullBiasNanos],[DriftNanosPerSecond],[DriftUncertaintyNanosPerSecond],[ElapsedRealtimeNanos],[ElapsedRealtimeUncertaintyNanos],[TimeNanos],[TimeUncertaintyNanos]) " +
                            "VALUES('{0}',{1},{2},{3},{4},{5},{6},{7},{8},{9})",
                           item.recordDate,
                           item.biasNanos,
                           item.biasUncertaintyNanos,
                           item.fullBiasNanos,
                           item.driftNanosPerSecond,
                           item.driftUncertaintyNanosPerSecond,
                           item.elapsedRealtimeNanos,
                           item.elapsedRealtimeUncertaintyNanos,
                           item.timeNanos,
                           item.timeUncertaintyNanos);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);
                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}", Common.Constant.mSignUpApiWebPage, mGNSSClockHttpAPI)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mGNSSClockQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mGNSSMeasurementQueue != null)
                {
                    if (!this.mSharedData.mGNSSMeasurementQueue.IsEmpty())
                    {
                        var item = this.mSharedData.mGNSSMeasurementQueue.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        strbuilder.AppendFormat("INSERT INTO MaaS.dbo.GNSSMeasurment([RecordDate],[ConstellationType],[Svid],[ReceivedSvTimeNanos],[PseudorangeRateMetersPerSecond],[AccumulatedDeltaRangeMeters],[CarrierFrequencyHz],[SnrInDb]) " +
                            "VALUES('{0}',{1},{2},{3},{4},{5},{6},{7})",
                           item.recordDate,
                           item.constellationType,
                           item.svid,
                           item.receivedSvTimeNanos,
                           item.pseudorangeRateMetersPerSecond,
                           item.accumulatedDeltaRangeMeters,
                           item.carrierFrequencyHz,
                           item.snrInDb);

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);
                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}", Common.Constant.mSignUpApiWebPage, mGNSSMeasureHttpAPI)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mGNSSMeasurementQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (this.mSharedData.mSSRDictionary != null)
                {
                    if (!this.mSharedData.mSSRDictionary.IsEmpty())
                    {
                        var item = this.mSharedData.mSSRDictionary.Pop();
                        System.Text.StringBuilder strbuilder = new System.Text.StringBuilder();

                        if (item.sbData != null)
                        {
                            strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SSR([RecordDate],[gs],[sbd_prn],[sbd_signalIndex],[codeBias],[phaseBias]) " +
                                "VALUES('{0}',{1},{2},{3},{4},{5})",
                               item.recordDate,
                               item.sbData.gs,
                               item.sbData.prn,
                               item.sbData.signalIndex,
                               item.sbData.codeBias,
                               item.sbData.phaseBias);
                        }
                        if (item.obcData != null)
                        {
                            strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SSR([RecordDate],[gs],[ord_prn],[ord_signalIndex],[radial],[alongTrack],[crossTrack]) " +
                                "VALUES('{0}',{1},{2},{3},{4},{5},{6})",
                               item.recordDate,
                               item.obcData.gs,
                               item.obcData.prn,
                               item.obcData.iod,
                               item.obcData.radial,
                               item.obcData.alongTrack,
                               item.obcData.crossTrack);
                        }
                        if (item.lddData != null)
                        {
                            strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SSR([RecordDate],[gs],[ldd_prn],[ldd_latitude],[ldd_longitude],[stec]) " +
                                "VALUES('{0}',{1},{2},{3},{4},{5})",
                               item.recordDate,
                               item.lddData.gs,
                               item.lddData.prn,
                               item.lddData.latitude,
                               item.lddData.longitude,
                               item.lddData.stec);
                        }
                        if (item.ccdData != null)
                        {
                            strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SSR([RecordDate],[gs],[ccd_prn],[deltaClockC0],[deltaClockC1],[deltaClockC2]) " +
                                "VALUES('{0}',{1},{2},{3},{4},{5})",
                               item.recordDate,
                               item.ccdData.gs,
                               item.ccdData.prn,
                               item.ccdData.deltaClockC0,
                               item.ccdData.deltaClockC1,
                               item.ccdData.deltaClockC2);
                        }
                        if (item.tddData != null)
                        {
                            strbuilder.AppendFormat("INSERT INTO MaaS.dbo.SSR([RecordDate],[gs],[tdd_latitude],[tdd_longitude],[ztd],[zwd]) " +
                                "VALUES('{0}',{1},{2},{3},{4},{5})",
                               item.recordDate,
                               item.tddData.gs,
                               item.tddData.latitude,
                               item.tddData.longitude,
                               item.tddData.ztd,
                               item.tddData.zwd);
                        }

                        mDBProvider.InsertMSSQLData(strbuilder.ToString(), SensorHttpRequsetCallBack);
                        /*
                        Common.RestSharpProvider.HttpPostRequest(
                            string.Format("{0}{1}", Common.Constant.mSignUpApiWebPage, mGNSSMeasureHttpAPI)
                            , Common.JsonParser.GetHTTPJsonSerializer(this.mSharedData.mGNSSMeasurementQueue.Pop())
                            , SensorHttpRequsetCallBack);
                        */
                    }
                }
            }
            catch
            {

            }
        }

        private void SensorHttpRequsetCallBack(string pCallBack)
        {
            Console.WriteLine(pCallBack);
        }
    }
}
