using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public interface IEngine
    {
        void Start(int pSleep = 1);
        void Stop();
    }
}
