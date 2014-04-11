using System;
using System.Collections;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using Thermostat.Core;

namespace Thermostat.TouchUi
{
    public class HomeScreen
    {
        private Settings Settings { get; set; }
        private SensorMeasurements Measurements { get; set; }

        public Window Window { get; private set; }
        public Button ButtonIncrementSetTemperatureDown { get; private set; }
        public Button ButtonIncrementSetTemperatureUp { get; private set; }
        public Button ButtonModeOff { get; private set; }
        public Button ButtonFan { get; private set; }
        public Button ButtonHeat { get; private set; }
        public Button ButtonCool { get; private set; }
        public Button ButtonAuto { get; private set; }
        public Button ButtonUnits { get; private set; }
        public Button ButtonSettings { get; private set; }

        public TextBox SetTemperatureValue { get; private set; }
        public TextBox CurrentTemperatureValue { get; private set; }
        public TextBox DateTime { get; private set; }

        public HomeScreen(string xml, Settings settings, SensorMeasurements measurements)
        {
            Settings = settings;
            Measurements = measurements;
            // Load the Window XML string.
            Window = GlideLoader.LoadWindow(xml);

            //Controls
            ButtonIncrementSetTemperatureUp = (Button)Window.GetChildByName("IncrementTemperatureUp");
            ButtonIncrementSetTemperatureDown = (Button)Window.GetChildByName("IncrementTemperatureDown");
            ButtonModeOff = (Button)Window.GetChildByName("Off Mode");
            ButtonFan = (Button)Window.GetChildByName("Fan Mode");
            ButtonHeat = (Button)Window.GetChildByName("Heat Mode");
            ButtonCool = (Button)Window.GetChildByName("AC Mode");
            ButtonAuto = (Button)Window.GetChildByName("Auto Mode");
            ButtonUnits = (Button)Window.GetChildByName("Units");
            ButtonSettings = (Button)Window.GetChildByName("Settings");

            //Info Boxes 
            SetTemperatureValue = (TextBox)Window.GetChildByName("SetTemperature");
            CurrentTemperatureValue = (TextBox)Window.GetChildByName("CurrentTemperatureValue");
            DateTime = (TextBox)Window.GetChildByName("TimeDisplay");

            // Set up event handlers
            //buttons
            ButtonIncrementSetTemperatureUp.TapEvent += new OnTap(IncrementSetTemperatureUp_TapEvent);
            ButtonIncrementSetTemperatureDown.TapEvent += new OnTap(IncrementSetTemperatureDown_TapEvent);
            ButtonModeOff.TapEvent += new OnTap(Mode_Off_TapEvent);
            ButtonFan.TapEvent += new OnTap(Mode_Fan_TapEvent);
            ButtonHeat.TapEvent += new OnTap(Mode_Heat_TapEvent);
            ButtonCool.TapEvent += new OnTap(Mode_Cool_TapEvent);
            ButtonAuto.TapEvent += new OnTap(Mode_Auto_TapEvent);
            ButtonUnits.TapEvent += new OnTap(Mode_Units_TapEvent);
            ButtonSettings.TapEvent += new OnTap(Mode_Settings_TapEvent);

            //Info Boxes
            settings.TargetTemp.TemperatureSettingChanged += (a, b) => Handle_TempSetting_Change(a, b);
            Measurements.PrimaryAirTemperature.TemperatureChanged += (a, b) => Handle_CurrentTemp_Change(a, b);

            // Set up initial values
            this.SetTemperatureValue.Text = this.Settings.TargetTemp.FormattedString();
            this.SetTemperatureValue.Invalidate();

            this.CurrentTemperatureValue.Text = this.Measurements.PrimaryAirTemperature.FormattedString();
            this.CurrentTemperatureValue.Invalidate();

        }

        private void Mode_Settings_TapEvent(object sender)
        {
        }

        private void Mode_Units_TapEvent(object sender)
        {
            if (this.Settings.TemperatureUnits == TemperatureUnits.Farenheit) 
            {
                this.Settings.TemperatureUnits = TemperatureUnits.Celcius;
                this.Measurements.SetTemperatureUnits(TemperatureUnits.Celcius);
                this.ButtonUnits.Text = "K";
                this.ButtonUnits.Invalidate();
            }
            else if (this.Settings.TemperatureUnits == TemperatureUnits.Celcius)
            {
                this.Settings.TemperatureUnits = TemperatureUnits.Kelvin;
                this.Measurements.SetTemperatureUnits(TemperatureUnits.Kelvin);
                this.ButtonUnits.Text = "F";
                this.ButtonUnits.Invalidate();
            }
            else 
            {
                this.Settings.TemperatureUnits = TemperatureUnits.Farenheit;
                this.Measurements.SetTemperatureUnits(TemperatureUnits.Farenheit);
                this.ButtonUnits.Text = "C";
                this.ButtonUnits.Invalidate();
            }
        }

        private void Mode_Auto_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.AUTO;
            this.ClearButtons();
            this.ButtonAuto.Enabled = false;
            this.ButtonAuto.Invalidate();
        }

        private void Mode_Cool_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.HVAC;
            this.ClearButtons();
            this.ButtonCool.Enabled = false;
            this.ButtonCool.Invalidate();
        }

        private void Mode_Heat_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.HEAT;
            this.ClearButtons();
            this.ButtonHeat.Enabled = false;
            this.ButtonHeat.Invalidate();
        }

        private void Mode_Fan_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.FAN;
            this.ClearButtons();
            this.ButtonFan.Enabled = false;
            this.ButtonFan.Invalidate();
        }

        private void Mode_Off_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.OFF;
            this.ClearButtons();
            this.ButtonModeOff.Enabled = false;
            this.ButtonModeOff.Invalidate();
        }

        private void ClearButtons()
        {
            this.ButtonAuto.Enabled = true;
            this.ButtonAuto.Invalidate();
            this.ButtonCool.Enabled = true;
            this.ButtonCool.Invalidate();
            this.ButtonFan.Enabled = true;
            this.ButtonFan.Invalidate();
            this.ButtonHeat.Enabled = true;
            this.ButtonHeat.Invalidate();
            this.ButtonModeOff.Enabled = true;
            this.ButtonModeOff.Invalidate();
        }

        private void Handle_TempSetting_Change(object sender, Thermostat.Core.TemperatureSetting.TemperatureSettingChangedArgs e)
        {
            this.SetTemperatureValue.Text = e.TemperatureString;
            this.SetTemperatureValue.Invalidate();
        }

        private void Handle_CurrentTemp_Change(object sender, Temperature.TemperatureChangedArgs e)
        {
            this.CurrentTemperatureValue.Text = e.TemperatureString;
            this.CurrentTemperatureValue.Invalidate();
        }

        private void IncrementSetTemperatureUp_TapEvent(object sender)
        {
            this.Settings.TargetTemp.IncrementTemperature();
        }

        private void IncrementSetTemperatureDown_TapEvent(object sender)
        {
            this.Settings.TargetTemp.DecrementTemperature();     
        }
    }
}
