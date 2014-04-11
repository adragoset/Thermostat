using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;
using Thermostat.Core;
using Thermostat.TouchUi;



namespace Thermostat
{
    public partial class Program
    {
        private SensorMeasurements SystemState { get; set; }
        private Settings SystemSettings { get; set; }
        private HvacControl ControlLoop { get; set; }
        private GT.Timer LoggingTimer { get; set; }
        private ThermostatUi Ui { get; set; }
        private Hashtable UiScreens { get; set; }

        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            this.SystemSettings = new Settings();
            this.SystemState = new SensorMeasurements(temperatureHumidity);
            this.ControlLoop = new HvacControl(Heat, Cool, Fan, SystemSettings, SystemState);
            this.UiScreens = new Hashtable();
            this.LoadResources();

            this.Ui = new ThermostatUi(Display, Resources.GetString(Resources.StringResources.ControlScreen), this.SystemSettings, this.SystemState);

            // Create a timer
            LoggingTimer = new GT.Timer(15000);
            LoggingTimer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            LoggingTimer.Start();
        }

        private void LoadResources()
        {
            this.UiScreens.Add(Resources.StringResources.ControlScreen.ToString(), Resources.GetString(Resources.StringResources.ControlScreen));
        }

        private void timer_Tick(GT.Timer timer)
        {
            
            Debug.Print("Temperature: " + this.SystemState.PrimaryAirTemperature.FormattedString() + " Relative Humidity: " + this.SystemState.PrimaryAirHumidity.FormattedString());
        }


    }
}
