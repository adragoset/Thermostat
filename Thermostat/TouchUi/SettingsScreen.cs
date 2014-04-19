using System;
using Microsoft.SPOT;
using GHI.Glide.Display;
using System.Collections;
using Thermostat.Core;
using GHI.Glide.UI;
using GHI.Glide;

namespace Thermostat.TouchUi
{
    public class SettingsScreen : IScreen
    {
        private Settings SystemSettings;
        private SensorMeasurements Measurements;
        private Hashtable Images;
        private Hashtable Screens;
        private Hashtable Fonts;

        public Window Window { get; private set; }

        public Button ButtonHomeScreen { get; set; }

        public event UpdateScreenEventHandler StatusUpdated;

        public SettingsScreen(Settings systemSettings, SensorMeasurements measurements, Hashtable images, Hashtable screens, Hashtable fonts)
        {
            // TODO: Complete member initialization
            this.SystemSettings = systemSettings;
            this.Measurements = measurements;
            this.Images = images;
            this.Screens = screens;
            this.Fonts = fonts;

            Window = GlideLoader.LoadWindow((String)screens["SettingsScreen"]);
            Window.BackImage = (Bitmap)images["BackGround"];
        }

        public void Open() {
            ButtonHomeScreen = (Button)Window.GetChildByName("Home");
            EnableScreenTouchHandelers();
        }

        public void Close() {
            DisableScreenTouchHandelers();
        }

        private void EnableScreenTouchHandelers() {
            ButtonHomeScreen.TapEvent += new OnTap(HomeScreen_Pressed_Handeler);
        }

        private void DisableScreenTouchHandelers() {
            ButtonHomeScreen.TapEvent -= HomeScreen_Pressed_Handeler;
        }

        private void HomeScreen_Pressed_Handeler(object sender)
        {
            var handler = StatusUpdated;
            if (handler != null)
            {
                handler(sender, new UpdateScreenArgs("HomeScreen", this));
            }
        }
    }
}
