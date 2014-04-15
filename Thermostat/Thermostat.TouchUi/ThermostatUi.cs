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

        private Hashtable Screens = new Hashtable();

        private TouchInitialization TouchIni;

        public ThermostatUi(Display_CP7 Display, Hashtable screens, Hashtable images, Hashtable fonts, Settings systemSettings, SensorMeasurements measurements)
        {
            this.Display = Display;
            Glide.FitToScreen = true;
            this.TouchIni = new TouchInitialization(Display);

            InitializeScreens(screens, images, fonts, systemSettings, measurements);
            //set the current window
            Glide.MainWindow = ((IScreen)Screens["HomeScreen"]).Window;
        }

        private void InitializeScreens(Hashtable screens, Hashtable images, Hashtable fonts, Settings systemSettings, SensorMeasurements measurements)
        {
            var homeScreen = new HomeScreen(systemSettings, measurements, images, screens, fonts);
            SetupScreen(homeScreen,"HomeScreen");

            var settingsScreen = new SettingsScreen(systemSettings, measurements, images, screens, fonts);
            SetupScreen(settingsScreen, "SettingsScreen");
            
        }

        private void SetupScreen(IScreen screen, string name)
        {
            screen.StatusUpdated += (a, b) => Handel_Screen_Changed_Event(a, b);
            Screens.Add(name, screen);
        }

        private void Handel_Screen_Changed_Event(object sender, UpdateScreenArgs args) {
            Glide.MainWindow = ((IScreen)Screens[args.Name]).Window;
        }

    }
}
