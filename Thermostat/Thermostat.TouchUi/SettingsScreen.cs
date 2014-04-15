using System;
using Microsoft.SPOT;
using GHI.Glide.Display;
using System.Collections;
using Thermostat.Core;

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

        public SettingsScreen(Settings systemSettings, SensorMeasurements measurements, Hashtable images, Hashtable screens, Hashtable fonts)
        {
            // TODO: Complete member initialization
            this.SystemSettings = systemSettings;
            this.Measurements = measurements;
            this.Images = images;
            this.Screens = screens;
            this.Fonts = fonts;
        }




        public event UpdateScreenEventHandler StatusUpdated;
    }
}
