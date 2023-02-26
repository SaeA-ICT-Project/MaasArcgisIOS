using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class JsonSetting
    {
        public class DefaultView
        {
            public bool IsCurrentVisible { set; get; }

            public bool IsSensorUIVisible { set; get; }
        }

        public class NaviationView
        {
            public bool IsSensorUIVisible { set; get; }
        }

        public class WarningSetting
        {
            public double WheelSize { set; get; }
            public bool IsSuspension { set; get; }
            public double HighRiskLimit { set; get; }
            public double MiddleRiskLimit { set; get; }
            public double LowRiskLimit { set; get; }
            public double BackWarningMeter { set; get; }
            public int ComboboxNavigating { set; get; }
        }
    }
}
