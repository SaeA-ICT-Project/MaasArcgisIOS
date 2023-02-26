using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class DeviceSensorClass
    {
        public class SensorMPURaw
        {
            public int X { set; get; }
            public int Y { set; get; }
            public int Z { set; get; }

            public bool bDeserilize = false;
            public int TimeCount { get; set; }

            public void Deserilize(byte[] pBuffer)
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length == 12)
                    {
                        this.X = (int)Math.Round(System.BitConverter.ToSingle(pBuffer, 0) * 1000.0f);
                        this.Y = (int)Math.Round(System.BitConverter.ToSingle(pBuffer, 4) * 1000.0f);
                        this.Z = (int)Math.Round(System.BitConverter.ToSingle(pBuffer, 8) * 1000.0f);
                        bDeserilize = true;
                        return;
                    }
                }
                bDeserilize = false;
            }
        }

        public class SensorPressure
        {
            public float Pressure { set; get; }
            public int TimeCount { get; set; }

            public bool bDeserilize = false;
            public void Deserilize(byte[] pBuffer)
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length == 4)
                    {
                        this.Pressure = BitConverter.ToSingle(pBuffer, 0);
                        bDeserilize = true;
                        return;
                    }
                }
                bDeserilize = false;
                //this.Pressure = Convert.ToSingle((pBuffer[0] << 16) | (pBuffer[1] << 8) | (pBuffer[2])) / 4096.0f;
                //this.Pressure = Convert.ToSingle((pBuffer[0]) | (pBuffer[1] << 8) | (pBuffer[2]) << 16 | pBuffer[3] << 24);
            }
        }

        public class SensorTemper
        {
            public float Temperature { set; get; }
            public int TimeCount { get; set; }

            public bool bDeserilize = false;
            public void Deserilize(byte[] pBuffer)
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length == 2)
                    {
                        this.Temperature = Convert.ToSingle((pBuffer[0] << 8) | pBuffer[1]) / 100.0f;
                        bDeserilize = true;
                        return;
                    }
                }
                bDeserilize = false;
            }
        }

        public class SensorRadar
        {
            public double Distance { set; get; }
            public double Speed { set; get; }
            public double Angle { set; get; }
            public int Meagnitude { set; get; }

            public SensorRadar()
            {

            }

            public void Deserilize(byte[] pBuffer, int pIndex)
            {
                this.Distance = Convert.ToDouble((float)((UInt16)(pBuffer[0 + pIndex] << 8 | pBuffer[1 + pIndex]))) * 0.01;
                this.Speed = Convert.ToDouble((float)((Int16)(pBuffer[2 + pIndex] << 8 | pBuffer[3 + pIndex]))) * 0.01;
                this.Angle = Convert.ToDouble((float)((Int16)(pBuffer[4 + pIndex] << 8 | pBuffer[5 + pIndex]))) * 0.01;
                this.Meagnitude = (pBuffer[7 + pIndex] << 8 | pBuffer[6 + pIndex]);
            }
        }

        public class SensorRdrPdat
        {
            public int RadarSize { get; set; }
            public SensorRadar[] RadarInfo { set; get; }
            public int TimeCount { get; set; }

            public bool bDeserilize = false;
            public SensorRdrPdat(int pSize)
            {
                RadarSize = pSize;
                this.RadarInfo = new SensorRadar[RadarSize];
            }

            public void Deserilize(byte[] pBuffer)
            {
                try
                {
                    if (pBuffer != null)
                    {
                        for (int i = 0; i < this.RadarSize; i++)
                        {
                            this.RadarInfo[i] = new SensorRadar();
                            this.RadarInfo[i].Deserilize(pBuffer, i * 8);
                        }
                    }
                }
                catch
                {
                    bDeserilize = false;
                }
                finally
                {
                    bDeserilize = true;
                }
            }
        }

        public class SensroRdrPdatResult
        {
            public int RadarSize { get; set; }
            public int TimeCount { get; set; }
        }
        public class SensorRdrTdat
        {
            public SensorRadar RadarInfo { set; get; }
            public int TimeCount { get; set; }

            public bool bDeserilize = false;
            public SensorRdrTdat()
            {
                this.RadarInfo = new SensorRadar();
            }

            public void Deserilize(byte[] pBuffer)
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length == 8)
                    {
                        RadarInfo.Deserilize(pBuffer, 0);
                        bDeserilize = true;
                        return;
                    }
                }
                bDeserilize = false;
            }
        }

        public class SensorGPS
        {
            public double latitude { set; get; }
            public double longitude { set; get; }
            public double altitude { set; get; }
            public double speed { set; get; }

            public void Deserilize(byte[] pBuffer)
            {
                var latV = pBuffer[0] << 24 | pBuffer[1] << 16 | pBuffer[2] << 8 | pBuffer[3];
                var latS = pBuffer[4] << 24 | pBuffer[5] << 16 | pBuffer[6] << 8 | pBuffer[7];
                var lat = (float)(latV / latS);
                var degWholeLat = (float)((int)lat / 100);
                var degDecLat = (lat - degWholeLat * 100) / 60.0;

                var logV = pBuffer[8] << 24 | pBuffer[9] << 16 | pBuffer[10] << 8 | pBuffer[11];
                var logS = pBuffer[12] << 24 | pBuffer[13] << 16 | pBuffer[14] << 8 | pBuffer[15];
                var lon = (float)(logV / logS);
                var degWholeLon = (float)((int)(lon / 100));
                var degDecLon = (lon - degWholeLon * 100) / 60.0;

                var altV = pBuffer[16] << 24 | pBuffer[17] << 16 | pBuffer[18] << 8 | pBuffer[19];
                var altS = pBuffer[20] << 24 | pBuffer[21] << 16 | pBuffer[22] << 8 | pBuffer[23];

                var spdV = pBuffer[24] << 24 | pBuffer[25] << 16 | pBuffer[26] << 8 | pBuffer[27];
                var spdS = pBuffer[28] << 24 | pBuffer[29] << 16 | pBuffer[30] << 8 | pBuffer[31];

                this.latitude = degWholeLat + degDecLat;
                this.longitude = degWholeLon + degDecLon;
                this.altitude = (altV / altS);
                this.speed = (spdV / spdS);
            }
        }

        public class SensorBattery
        {
            public int Energy { set; get; }
            public int TimeCount { get; set; }

            public bool bDeserilize = false;

            public void Deserilize(byte[] pBuffer)
            {
                if (pBuffer != null)
                {
                    if (pBuffer.Length == 2)
                    {
                        this.Energy = Convert.ToInt32(pBuffer[1]);
                        bDeserilize = true;
                        return;
                    }
                }
                bDeserilize = false;
            }
        }
    }
    public class DeviceGPSInfo
    {
        public double latitude { set; get; }
        public double longitude { set; get; }
        public double altitude { set; get; }
        public double distance { set; get; }
        public double angle { set; get; }
        public double speed { set; get; }
        public DateTime datetimes { set; get; }
    }

    public class DeviceSensorMPUFileClass
    {
        public double StepValue { set; get; }
        public double DisplayValue { set; get; }
    }
}
