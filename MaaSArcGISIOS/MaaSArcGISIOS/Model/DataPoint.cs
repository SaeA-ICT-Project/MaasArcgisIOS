using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class DataPoint : IComparable<DataPoint>
    {
        public int mIndex { set; get; }
        public double mXValue { set; get; }
        public double mYValue { set; get; }

        public double mETCValue { set; get; }

        public DataPoint(int pIndex, double pDistance, double pAngle, double pSpeed)
        {
            mIndex = pIndex;
            mXValue = Math.Round(Math.Sin(pAngle * (Math.PI / 180.0)) * pDistance, 5);
            mYValue = Math.Round(Math.Cos(pAngle * (Math.PI / 180.0)) * pDistance, 5);

            Console.WriteLine("debug_console -> dist : {0:0.000}m , Angle : {1:0.000} , X : {2:0.000} , Y : {3:0.000}", pDistance, pAngle, mXValue, mYValue);

            mETCValue = pSpeed;
        }

        public int CompareTo(DataPoint other)
        {
            if (this.mXValue > other.mXValue) return 1;
            else if (this.mXValue.Equals(other.mXValue))
            {
                if (this.mYValue > other.mYValue) return 1;
                else if (this.mYValue.Equals(other.mYValue)) return 0;
                else return -1;
            }
            else return -1;
        }
    }

    internal class DataPointDissimilarityMetric : Aglomera.IDissimilarityMetric<DataPoint>
    {
        public double Calculate(DataPoint instance1, DataPoint instance2)
        {
            return Math.Sqrt(Math.Pow(instance1.mXValue - instance2.mXValue, 2) + Math.Pow(instance1.mYValue - instance2.mYValue, 2));
        }
    }
}
