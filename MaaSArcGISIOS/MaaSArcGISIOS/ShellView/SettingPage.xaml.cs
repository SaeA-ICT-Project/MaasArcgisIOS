using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaaSArcGISIOS.ShellView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private Data.SingletonData mSharedData;

        public SettingPage()
        {
            InitializeComponent();

            mSharedData = Data.SingletonData.mInstance;

            this.Appearing += SettingPage_Appearing;
            try
            {
                InitSettingValue();
            }
            catch (Exception ets)
            {
                Console.WriteLine(ets.Message);
            }
        }

        private void InitSettingValue()
        {
            var wheelsize = Common.SettingPreferences.GetPreferences("warning_WheelSize", "1.0");
            var isSuspension = Common.SettingPreferences.GetPreferences("warning_IsSuspension", "false");

            var mpulimit = Common.SettingPreferences.GetPreferences("warning_MPURiskLimit", "10.0");
            var pdatlimit = Common.SettingPreferences.GetPreferences("warning_PdatRiskLimit", "5.0");
            var anglelimit = Common.SettingPreferences.GetPreferences("warning_AngleRiskLimit", "7.0");

            var backmeter = Common.SettingPreferences.GetPreferences("warning_BackWarningMeter", "5.0");
            var pdatdist = Common.SettingPreferences.GetPreferences("grouping_pdat_distance", "1.0");
            var comboboxindex = Common.SettingPreferences.GetPreferences("warning_ComboboxNavigating", "1");
            var simulatorspeed = Common.SettingPreferences.GetPreferences("simulation_speed", "20.0");

            var simulationsave = Common.SettingPreferences.GetPreferences("simulation_savedata", "true");
            var ignoreSpeed = Common.SettingPreferences.GetPreferences("ignore_speed", "10.0");

            this.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                mSharedData.mCustomSettingInfo = new Model.CustonSetter()
                {
                    WheelSize = Convert.ToDouble(wheelsize),
                    IsSuspension = Convert.ToBoolean(isSuspension),
                    MPURiskLimit = Convert.ToDouble(mpulimit),
                    PdatRiskLimit = Convert.ToDouble(pdatlimit),
                    AngleRiskLimit = Convert.ToDouble(anglelimit),
                    NavigatePathIndex = Convert.ToInt32(comboboxindex),
                    BackDistanceWarning = Convert.ToDouble(backmeter),
                    SimulationSpeed = Convert.ToDouble(simulatorspeed),
                    SimulationSave = Convert.ToBoolean(simulationsave),
                    IgnoreSpeed = Convert.ToDouble(ignoreSpeed),
                };

                xWheelLabel.Text = wheelsize;
                xWheelStepper.Value = Convert.ToDouble(wheelsize);
                xIsSuspensionSwitch.On = Convert.ToBoolean(isSuspension);
                xMPURiskLimitLabel.Text = mpulimit;
                xMPURiskLimitStepper.Value = Convert.ToDouble(mpulimit);
                xPdatRiskLimitLabel.Text = pdatlimit;
                xPdatRiskLimitStepper.Value = Convert.ToDouble(pdatlimit);
                xAngleRiskLimitLabel.Text = anglelimit;
                xAngleRiskLimitStepper.Value = Convert.ToDouble(anglelimit);
                xBackWarningMeterLabel.Text = backmeter;
                xBackWarningMeterStepper.Value = Convert.ToDouble(backmeter);
                xPdatDistanceLabel.Text = pdatdist;
                xPdatDistanceStepper.Value = Convert.ToDouble(pdatdist);
                mSharedData.mPdatDistanceFlag = Convert.ToDouble(pdatdist);
                xSimulationSpeedStepper.Value = Convert.ToDouble(simulatorspeed);
                xSimulationSpeedLabel.Text = simulatorspeed;
                xIsSimulationSaveSwitch.On = Convert.ToBoolean(simulationsave);
                xIgnoreSppedStepper.Value = Convert.ToDouble(ignoreSpeed);
                xIgnoreSppedLabel.Text = ignoreSpeed;

                switch (Convert.ToInt32(comboboxindex))
                {
                    case 1:
                        xNavigateRadioButton2.IsChecked = true;
                        break;
                    case 2:
                        xNavigateRadioButton3.IsChecked = true;
                        break;
                    default:
                        xNavigateRadioButton1.IsChecked = true;
                        break;
                }
                xIsSuspensionSwitch.OnChanged += XIsSuspensionSwitch_OnChanged;
            });

        }

        private void XIsSuspensionSwitch_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                Common.SettingPreferences.SetPreferences("warning_IsSuspension", e.Value.ToString());
            }
            catch
            {

            }
        }

        private void SettingPage_Appearing(object sender, EventArgs e)
        {
            MaaSArcGISIOS.Common.Constant.mDefaultPageStatus = Common.Constant.ShellCurrentStatus.Defualt;
            MaaSArcGISIOS.Common.Constant.mNavigatePageStatus = Common.Constant.ShellCurrentStatus.Defualt;
        }

        private void xWheelStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.WheelSize = e.NewValue;

                //xWheelLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("warning_WheelSize", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xDefaultCrrentSwitch_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.IsSuspension = e.Value;

                Common.SettingPreferences.SetPreferences("default_IsSensorUIVisible", e.Value.ToString());
            }
            catch
            {

            }
        }

        private void xDefaultSensorSwitch_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                Common.SettingPreferences.SetPreferences("default_IsCurrentVisible", e.Value.ToString());
            }
            catch
            {

            }
        }

        private void xNavigationSensorSwitch_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                Common.SettingPreferences.SetPreferences("navigate_IsSensorUIVisible", e.Value.ToString());
            }
            catch
            {

            }
        }

        private void xBackWarningMeterStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.BackDistanceWarning = e.NewValue;

                xBackWarningMeterLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("warning_BackWarningMeter", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xPdatDistanceStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                mSharedData.mPdatDistanceFlag = e.NewValue;

                xPdatDistanceLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("grouping_pdat_distance", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xPdatAngleStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                Common.SettingPreferences.SetPreferences("grouping_pdat_angle", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xLocationQueueStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                mSharedData.mLocationQueueSize = e.NewValue;
                Common.SettingPreferences.SetPreferences("warning_LocationSize", e.NewValue.ToString());
            }
            catch
            {

            }
        }


        private void xNavigateRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                RadioButton target = sender as RadioButton;

                switch (target.Content.ToString())
                {
                    case "최적":
                        if (mSharedData.mCustomSettingInfo != null)
                            mSharedData.mCustomSettingInfo.NavigatePathIndex = 0;

                        Common.SettingPreferences.SetPreferences("warning_ComboboxNavigating", "0");
                        break;
                    case "안전":
                        if (mSharedData.mCustomSettingInfo != null)
                            mSharedData.mCustomSettingInfo.NavigatePathIndex = 1;

                        Common.SettingPreferences.SetPreferences("warning_ComboboxNavigating", "1");
                        break;
                    default:
                        if (mSharedData.mCustomSettingInfo != null)
                            mSharedData.mCustomSettingInfo.NavigatePathIndex = 2;

                        Common.SettingPreferences.SetPreferences("warning_ComboboxNavigating", "2");
                        break;
                }
            }
            catch
            {

            }
        }

        private void xSimulationSpeedStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.SimulationSpeed = e.NewValue;

                xSimulationSpeedLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("simulation_speed", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xMPURiskLimitStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.MPURiskLimit = e.NewValue;

                xMPURiskLimitLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("warning_MPURiskLimit", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xPdatRiskLimitStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.PdatRiskLimit = e.NewValue;

                xPdatRiskLimitLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("warning_PdatRiskLimit", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xAngleRiskLimitStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.AngleRiskLimit = e.NewValue;

                xAngleRiskLimitLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("warning_AngleRiskLimit", e.NewValue.ToString());
            }
            catch
            {

            }
        }

        private void xIsSimulationSaveSwitch_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.SimulationSave = e.Value;

                Common.SettingPreferences.SetPreferences("simulation_savedata", e.Value.ToString());
            }
            catch
            {

            }
        }

        private void xIgnoreSppedStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                if (mSharedData.mCustomSettingInfo != null)
                    mSharedData.mCustomSettingInfo.IgnoreSpeed = e.NewValue;

                xIgnoreSppedLabel.Text = e.NewValue.ToString();
                Common.SettingPreferences.SetPreferences("ignore_speed", e.NewValue.ToString());
            }
            catch
            {

            }
        }
    }
}