using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

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

        private Hashtable Images { get; set; }
        private Hashtable Screens { get; set; }

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

        public TextBlock SetTemperatureValue { get; private set; }
        public TextBlock CurrentTemperatureValue { get; private set; }
        public TextBlock DateTime { get; private set; }
        public TextBlock MaxTemp { get; private set; }
        public TextBlock MinTemp { get; private set; }
        public TextBlock Humidity { get; private set; }

        public HomeScreen(Settings settings, SensorMeasurements measurements, Hashtable images, Hashtable screens, Hashtable fonts)
        {
            Settings = settings;
            Measurements = measurements;
            Images = images;
            Screens = screens;

            // Load the Window XML string.
            Window = GlideLoader.LoadWindow((String)screens["ControlScreen"]);
            Window.BackImage = (Bitmap)images["BackGround"];

            //Info Boxes 
            SetTemperatureValue = (TextBlock)Window.GetChildByName("SetTemperature");
            SetTemperatureValue.Font = (Font)fonts["Arial72"];
            settings.TargetTemp.TemperatureSettingChanged += (a, b) => Handle_TempSetting_Change(a, b);
            CurrentTemperatureValue = (TextBlock)Window.GetChildByName("CurrentTemperatureValue");
            CurrentTemperatureValue.Font = (Font)fonts["Arial72"];
            Measurements.PrimaryAirTemperature.TemperatureChanged += (a, b) => Handle_CurrentTemp_Change(a, b);
            DateTime = (TextBlock)Window.GetChildByName("TimeDisplay");
            Measurements.ClockChangedEvent += new Thermostat.Core.SensorMeasurements.ClockChangedDelegate(Handle_Clock_Changed);
            MaxTemp = (TextBlock)Window.GetChildByName("MaxTemp");
            MinTemp = (TextBlock)Window.GetChildByName("MinTemp");
            Humidity = (TextBlock)Window.GetChildByName("Humidity");

            SetUpButtons();

            // Set up initial values
            SetInitialMeasurementValues();

        }

        private void Handle_Clock_Changed(object sender, SensorMeasurements.ClockChangedArgs e)
        {
            DateTime.Text = e.Now.ToString();
            DateTime.Invalidate();
        }

        private void SetInitialMeasurementValues()
        {
            this.SetTemperatureValue.Text = this.Settings.TargetTemp.FormattedString();
            this.SetTemperatureValue.Invalidate();

            this.CurrentTemperatureValue.Text = this.Measurements.PrimaryAirTemperature.FormattedString();
            this.CurrentTemperatureValue.Invalidate();

            UpdateMinMaxTemp();

            SetButtonState(ButtonModeOff, "PowerOffEnabled", false);

            Humidity.Text = Measurements.PrimaryAirHumidity.FormattedString();
            Humidity.Invalidate();
        }

        private void SetUpButtons()
        {
            ButtonIncrementSetTemperatureUp = (Button)Window.GetChildByName("IncrementTemperatureUp");
            ButtonIncrementSetTemperatureUp.ButtonUp = (Bitmap)Images["ArrowKeyUpUp"];
            ButtonIncrementSetTemperatureUp.ButtonDown = (Bitmap)Images["ArrowKeyUpDown"];

            ButtonIncrementSetTemperatureDown = (Button)Window.GetChildByName("IncrementTemperatureDown");
            ButtonIncrementSetTemperatureDown.ButtonUp = (Bitmap)Images["ArrowKeyDownUp"];
            ButtonIncrementSetTemperatureDown.ButtonDown = (Bitmap)Images["ArrowKeyDownDown"];

            ButtonModeOff = SetUpButton("Off Mode", "PowerOff");

            ButtonFan = SetUpButton("Fan Mode", "Fan");
            ButtonHeat = SetUpButton("Heat Mode", "Heat");
            ButtonCool = SetUpButton("AC Mode", "Cool");
            ButtonAuto = SetUpButton("Auto Mode", "Auto");

            ButtonUnits = (Button)Window.GetChildByName("Units");
            ButtonSettings = (Button)Window.GetChildByName("Settings");

            ButtonIncrementSetTemperatureUp.TapEvent += new OnTap(IncrementSetTemperatureUp_TapEvent);
            ButtonIncrementSetTemperatureDown.TapEvent += new OnTap(IncrementSetTemperatureDown_TapEvent);
            ButtonModeOff.TapEvent += new OnTap(Mode_Off_TapEvent);
            ButtonFan.TapEvent += new OnTap(Mode_Fan_TapEvent);
            ButtonHeat.TapEvent += new OnTap(Mode_Heat_TapEvent);
            ButtonCool.TapEvent += new OnTap(Mode_Cool_TapEvent);
            ButtonAuto.TapEvent += new OnTap(Mode_Auto_TapEvent);
            ButtonUnits.TapEvent += new OnTap(Mode_Units_TapEvent);
            ButtonSettings.TapEvent += new OnTap(Mode_Settings_TapEvent);
        }

        private Button SetUpButton(string ButtonName, string imagePrefix) {
            var button = (Button)Window.GetChildByName(ButtonName);
            button.ButtonUp = (Bitmap)Images[imagePrefix + "Disabled"];
            button.ButtonDown = (Bitmap)Images[imagePrefix + "Down"];
            return button;
        }

        private void ClearButtons()
        {
            SetButtonState(this.ButtonAuto, "AutoDisabled", true);
            SetButtonState(this.ButtonCool, "CoolDisabled", true);
            SetButtonState(this.ButtonFan, "FanDisabled", true);
            SetButtonState(this.ButtonHeat, "HeatDisabled", true);
            SetButtonState(this.ButtonModeOff, "PowerOffDisabled", true);
        }

        private void SetButtonState(Button button, String buttonImage, bool enabled)
        {
            button.ButtonUp = (Bitmap)Images[buttonImage];
            button.Enabled = enabled;
            if (enabled == true)
            {
                button.Alpha = 255;
            }
            else
            {
                button.Alpha = 510;
            }
            button.Invalidate();
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
            SetButtonState(this.ButtonAuto, "AutoEnabled", false);
        }

        private void Mode_Cool_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.HVAC;
            this.ClearButtons();
            SetButtonState(this.ButtonCool, "CoolEnabled", false);
        }

        private void Mode_Heat_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.HEAT;
            this.ClearButtons();
            SetButtonState(this.ButtonHeat, "HeatEnabled", false);
        }

        private void Mode_Fan_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.FAN;
            this.ClearButtons();
            SetButtonState(this.ButtonFan, "FanEnabled", false);
        }

        private void Mode_Off_TapEvent(object sender)
        {
            this.Settings.Mode = SystemModeEnum.OFF;
            this.ClearButtons();
            SetButtonState(this.ButtonModeOff, "PowerOffEnabled", false);
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

            Humidity.Text = Measurements.PrimaryAirHumidity.FormattedString();
            Humidity.Invalidate();

            UpdateMinMaxTemp();
        }

        private void UpdateMinMaxTemp()
        {
            var temp = this.Settings.TargetTemp.Temperature;
            var deadZone = this.Settings.DeadZone;

            if (this.Settings.Mode == SystemModeEnum.AUTO)
            {
                this.MinTemp.Text = "Min: " + this.Settings.AutoMinTemp.ToString();
                this.MaxTemp.Text = "Max: " + this.Settings.AutoMaxTemp.ToString();
                this.MinTemp.Invalidate();
                this.MaxTemp.Invalidate();
            }
            else
            {
                this.MinTemp.Text = "Min: " + (temp - deadZone).ToString();
                this.MaxTemp.Text = "Max: " + (temp + deadZone).ToString();
                this.MinTemp.Invalidate();
                this.MaxTemp.Invalidate();
            }
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
