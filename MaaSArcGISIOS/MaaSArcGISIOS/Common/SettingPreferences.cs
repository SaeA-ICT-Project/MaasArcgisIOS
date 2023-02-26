using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class SettingPreferences
    {
        public static void SetPreferences(string pKey, string pValue)
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    Xamarin.Essentials.Preferences.Set(pKey, pValue);
                });
            }
            catch
            {

            }
        }

        public static string GetPreferences(string pKey, string pDefault)
        {
            try
            {
                string result = string.Empty;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    result = Xamarin.Essentials.Preferences.Get(pKey, pDefault);
                });
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static bool CheckPreferences(string pKey)
        {
            try
            {
                bool result = false;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    result = Xamarin.Essentials.Preferences.ContainsKey(pKey);
                });
                return result;
            }
            catch
            {
                return false;
            }
        }

        public static void GetInitJsonSetting()
        {
            try
            {
                if (!CheckPreferences("default_IsCurrentVisible"))
                {
                    SetPreferences("default_IsCurrentVisible", "false");
                }
                if (!CheckPreferences("default_IsSensorUIVisible"))
                {
                    SetPreferences("default_IsSensorUIVisible", "false");
                }
                if (!CheckPreferences("navigate_IsSensorUIVisible"))
                {
                    SetPreferences("navigate_IsSensorUIVisible", "false");
                }
                if (!CheckPreferences("warning_WheelSize"))
                {
                    SetPreferences("warning_WheelSize", "1.0");
                }
                if (!CheckPreferences("warning_IsSuspension"))
                {
                    SetPreferences("warning_IsSuspension", "false");
                }



                if (!CheckPreferences("warning_MPURiskLimit"))
                {
                    SetPreferences("warning_MPURiskLimit", "10.0");
                }
                if (!CheckPreferences("warning_PdatRiskLimit"))
                {
                    SetPreferences("warning_PdatRiskLimit", "5.0");
                }
                if (!CheckPreferences("warning_AngleRiskLimit"))
                {
                    SetPreferences("warning_AngleRiskLimit", "7.0");
                }


                if (!CheckPreferences("warning_BackWarningMeter"))
                {
                    SetPreferences("warning_BackWarningMeter", "5.0");
                }
                if (!CheckPreferences("warning_ComboboxNavigating"))
                {
                    SetPreferences("warning_ComboboxNavigating", "1");
                }
                if (!CheckPreferences("grouping_pdat_distance"))
                {
                    SetPreferences("grouping_pdat_distance", "1.0");
                }
                if (!CheckPreferences("warning_ComboboxNavigating"))
                {
                    SetPreferences("grouping_pdat_angle", "1.0");
                }

                if (!CheckPreferences("warning_LocationSize"))
                {
                    SetPreferences("warning_LocationSize", "7.0");
                }

                if (!CheckPreferences("simulation_speed"))
                {
                    SetPreferences("simulation_speed", "20.0");
                }

                if (!CheckPreferences("location_selectitem"))
                {
                    SetPreferences("location_selectitem", "false");
                }

                if (!CheckPreferences("simulation_savedata"))
                {
                    SetPreferences("simulation_savedata", "true");
                }

                if (!CheckPreferences("ignore_speed"))
                {
                    SetPreferences("ignore_speed", "10.0");
                }
            }
            catch
            {
            }
        }
    }
}
