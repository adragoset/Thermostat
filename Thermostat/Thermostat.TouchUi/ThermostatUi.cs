using System.Threading;
using System.Collections;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using Gadgeteer.Modules.GHIElectronics;
using Thermostat.Core;

namespace Thermostat.TouchUi
{
    public class ThermostatUi
    {
        private Display_CP7 Display;

        private HomeScreen HomeScreen { get; set;}

        private static Button button;
        private TouchInitialization TouchIni;

        public ThermostatUi(Display_CP7 Display, string homeScreen, Settings systemSettings, SensorMeasurements measurements)
        {
            // TODO: Complete member initialization
            this.Display = Display;
            this.HomeScreen = new HomeScreen(homeScreen, systemSettings, measurements);

            // Resize any loaded Window to the LCD's size.
            Glide.FitToScreen = true;


            // Assign the Window to MainWindow; rendering it to the LCD.
            Glide.MainWindow = HomeScreen.Window;

            this.TouchIni = new TouchInitialization(Display);
        }
    }
}
