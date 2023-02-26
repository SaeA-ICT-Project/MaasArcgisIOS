using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class CustonSetter
    {
        public double WheelSize { set; get; }
        public bool IsSuspension { set; get; }
        public double MPURiskLimit { set; get; }
        public double PdatRiskLimit { set; get; }
        public double AngleRiskLimit { set; get; }
        public double BackDistanceWarning { set; get; }
        public int NavigatePathIndex { set; get; }
        public double SimulationSpeed { set; get; }
        public bool AndroidGPSProviderSwitch { set; get; }

        public bool SimulationSave { set; get; }

        public double IgnoreSpeed { set; get; }

    }
}
