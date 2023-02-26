using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class HTTPPressure
    {
        public HTTPGPSInfo gps { set; get; }
        public HTTPInnerPressure data { set; get; }
    }

    public class HTTPGPSInfo
    {
        public double latitude { set; get; }

        public double longitude { set; get; }
    }

    public class HTTPInnerPressure
    {
        public double pressure { set; get; }

        public double altitude { set; get; }

        public double inclination { set; get; }
        public string recordDate { set; get; }
    }

    public class HTTPInnerMPU
    {
        public float x { set; get; }

        public float y { set; get; }

        public float z { set; get; }

        public byte shockAbsorber { set; get; }

        public double wheelSize { set; get; }

        public string recordDate { set; get; }
    }

    public class HTTPInnerPDat
    {
        public int objectsCount { set; get; }

        public string recordDate { set; get; }
    }

    public class HTTPGNSSClock
    {
        public string recordDate { set; get; }
        public double biasNanos { set; get; }
        public double biasUncertaintyNanos { set; get; }
        public long fullBiasNanos { set; get; }
        public double driftNanosPerSecond { set; get; }
        public double driftUncertaintyNanosPerSecond { set; get; }
        public long elapsedRealtimeNanos { set; get; }
        public double elapsedRealtimeUncertaintyNanos { set; get; }
        public long timeNanos { set; get; }
        public double timeUncertaintyNanos { set; get; }
    }

    public class HTTPGNSSMeasurement
    {
        public string recordDate { set; get; }
        public int constellationType { set; get; }
        public int svid { set; get; }
        public long receivedSvTimeNanos { set; get; }
        public double pseudorangeRateMetersPerSecond { set; get; }
        public double accumulatedDeltaRangeMeters { set; get; }
        public double carrierFrequencyHz { set; get; }
        public double snrInDb { set; get; }
    }

    public class HTTPSSR
    {
        public string recordDate { set; get; }

        public SignalBiasData sbData;
        public OrbitCorrectionData obcData;
        public IonosphereDelayData lddData;
        public ClockCorrectionData ccdData;
        public TroposphereDelayData tddData;
    }

    public class HTTPMPURaw
    {
        public HTTPGPSInfo gps { set; get; }
        public HTTPInnerMPU data { set; get; }
    }

    public class HTTPRadarPdat
    {
        public HTTPGPSInfo gps { set; get; }
        public HTTPInnerPDat data { set; get; }
    }

    public class HTTPRadarTdat
    {
        public DeviceGPSInfo gps { set; get; }
        public DeviceSensorClass.SensorRdrTdat data { set; get; }
    }

    public class GoogleMapAPILocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class GoogleMapAPIElement
    {
        public double elevation { get; set; }
        public GoogleMapAPILocation location { get; set; }
        public double resolution { get; set; }
    }

    public class GoogleMapAPIResult
    {
        public System.Collections.Generic.List<GoogleMapAPIElement> results { get; set; }
        public string status { get; set; }
    }
}
