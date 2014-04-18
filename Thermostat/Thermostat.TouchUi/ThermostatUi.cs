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
        private DisplayCP7 Display;

        private Hashtable Screens = new Hashtable();

        private TouchInitialization TouchIni;

        private Settings Settings;

        public ThermostatUi(DisplayCP7 Display, Hashtable screens, Hashtable images, Hashtable fonts, Settings systemSettings, SensorMeasurements measurements)
        {
            this.Display = Display;
            this.Settings = systemSettings;
            this.Settings.GuiLoggedIn = true;
            Glide.FitToScreen = true;
            this.TouchIni = new TouchInitialization(Display);
            InitializeScreens(screens, images, fonts, systemSettings, measurements);
            //set the current window
            Glide.MainWindow = ((IScreen)Screens["HomeScreen"]).Window;
        }

        private void InitializeScreens(Hashtable screens, Hashtable images, Hashtable fonts, Settings systemSettings, SensorMeasurements measurements)
        {
            var homeScreen = new HomeScreen(systemSettings, measurements, images, screens, fonts);
            SetupScreen(homeScreen, "HomeScreen");

            var settingsScreen = new SettingsScreen(systemSettings, measurements, images, screens, fonts);
            SetupScreen(settingsScreen, "SettingsScreen");

        }

        private void SetupScreen(IScreen screen, string name)
        {
            screen.StatusUpdated += (a, b) => Handel_Screen_Changed_Event(a, b);
            Screens.Add(name, screen);
        }

        private void Handel_Screen_Changed_Event(object sender, UpdateScreenArgs args)
        {
            var newWindow = ((IScreen)Screens[args.Name]);
            var oldWindow = ((IScreen)args.Window);

            oldWindow.Close();

            if (args.Name == "HomeScreen" || this.Settings.GuiLoggedIn)
            {
                Glide.MainWindow = newWindow.Window;
            }
            else
            {
                Glide.MainWindow = ((IScreen)Screens["LogIn"]).Window;
            }

            newWindow.Open();

            Glide.MainWindow.Invalidate();
        }

    }
}
