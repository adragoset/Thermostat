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
using Gadgeteer.Modules.DFRobot;
using Thermostat.Core;
using Thermostat.TouchUi;



namespace Thermostat
{

    public partial class Program
    {
        public const string Encryption_Key = "1CdQymMKb42I5Ptf6xSZPFaEjZYiT7C4";

        //private GTM.GHIElectronics.WiFiRS21 wifi;
        private SD2405_Real_Time_Clock realTimeClock;
        //The temperature HUmidity Senesor using sock H3 of hubAp5
        private TemperatureHumidity temperatureHumidity;
        // <summary>The Display module using sockets 15, 16, 17, 6

        private HubAP5 hub;
        private DisplayCP7 display;
        /// <summary>The RelayX1 module using socket H6 of hubAP5.</summary>
        private RelayX1 heat;

        /// <summary>The RelayX1 module using socket H5 of hubAP5.</summary>
        private RelayX1 fan;

        /// <summary>The RelayX1 module using socket H4 of hubAP5.</summary>
        private RelayX1 cool;

        /// <summary>The GasSense module using socket H2 of hubAP5.</summary>
        private GasSense carbonMonoxide;
        private GasSense smoke_combustibles;

        private SensorMeasurements SystemState { get; set; }
        private Settings SystemSettings { get; set; }
        private HvacControl ControlLoop { get; set; }
        private GT.Timer LoggingTimer { get; set; }
        private ThermostatUi Ui { get; set; }
        private Hashtable UiScreens { get; set; }
        private Hashtable Images { get; set; }
        private Hashtable Fonts { get; set; }

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

            //wifi = new GTM.GHIElectronics.WiFiRS21(3);
            this.barometer = new Barometer(10);
            this.display = new GTM.GHIElectronics.DisplayCP7(15, 16, 17, 6);
            this.realTimeClock = new SD2405_Real_Time_Clock(13);
           
            this.hub = new HubAP5(1);
            this.heat = new RelayX1(this.hub.HubSocket6);
            this.cool = new RelayX1(this.hub.HubSocket5);
            this.fan = new RelayX1(this.hub.HubSocket4);
            this.carbonMonoxide = new GasSense(this.hub.HubSocket2);
            this.smoke_combustibles = new GasSense(this.hub.HubSocket1);


            this.temperatureHumidity = new GTM.GHIElectronics.TemperatureHumidity(this.hub.HubSocket3);
            this.SystemSettings = new Settings();
            this.SystemState = new SensorMeasurements(temperatureHumidity, barometer);
            this.ControlLoop = new HvacControl(heat, cool, fan, SystemSettings, SystemState);
            this.UiScreens = new Hashtable();
            this.Images = new Hashtable();
            this.Fonts = new Hashtable();
            this.LoadResources();

            this.Ui = new ThermostatUi(display, UiScreens, Images, Fonts, this.SystemSettings, this.SystemState);
        }

        private void LoadResources()
        {
            //Screens
            this.UiScreens.Add("HomeScreen", Resources.GetString(Resources.StringResources.HomeScreen));
            this.UiScreens.Add("SettingsScreen", Resources.GetString(Resources.StringResources.SettingsScreen));
            this.UiScreens.Add("LogOnScreen", Resources.GetString(Resources.StringResources.LogInScreen));

            //Images
            this.Images.Add("PowerOffEnabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.PowerOffEnabled), Bitmap.BitmapImageType.Gif));
            this.Images.Add("PowerOffDisabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.PowerOffDisabled), Bitmap.BitmapImageType.Gif));
            this.Images.Add("PowerOffDown", new Bitmap(Resources.GetBytes(Resources.BinaryResources.PowerOffDown), Bitmap.BitmapImageType.Gif));

            this.Images.Add("CoolDown", new Bitmap(Resources.GetBytes(Resources.BinaryResources.CoolDown), Bitmap.BitmapImageType.Gif));
            this.Images.Add("CoolDisabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.CoolDisabled), Bitmap.BitmapImageType.Gif));
            this.Images.Add("CoolEnabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.CoolEnabled), Bitmap.BitmapImageType.Gif));

            this.Images.Add("HeatDown", new Bitmap(Resources.GetBytes(Resources.BinaryResources.HeatDown), Bitmap.BitmapImageType.Gif));
            this.Images.Add("HeatDisabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.HeatDisabled), Bitmap.BitmapImageType.Gif));
            this.Images.Add("HeatEnabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.HeatEnabled), Bitmap.BitmapImageType.Gif));

            this.Images.Add("FanDown", new Bitmap(Resources.GetBytes(Resources.BinaryResources.FanDown), Bitmap.BitmapImageType.Gif));
            this.Images.Add("FanEnabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.FanEnabled), Bitmap.BitmapImageType.Gif));
            this.Images.Add("FanDisabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.FanDisabled), Bitmap.BitmapImageType.Gif));

            this.Images.Add("AutoDown", new Bitmap(Resources.GetBytes(Resources.BinaryResources.AutoDown), Bitmap.BitmapImageType.Gif));
            this.Images.Add("AutoEnabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.AutoEnabled), Bitmap.BitmapImageType.Gif));
            this.Images.Add("AutoDisabled", new Bitmap(Resources.GetBytes(Resources.BinaryResources.AutoDisabled), Bitmap.BitmapImageType.Gif));

            this.Images.Add("ArrowKeyUpDown", new Bitmap(Resources.GetBytes(Resources.BinaryResources.ArrowKeyDown), Bitmap.BitmapImageType.Gif));
            this.Images.Add("ArrowKeyUpUp", new Bitmap(Resources.GetBytes(Resources.BinaryResources.ArrowKeyUp), Bitmap.BitmapImageType.Gif));

            var up = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ArrowKeyUp), Bitmap.BitmapImageType.Gif);
            var up1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ArrowKeyUp), Bitmap.BitmapImageType.Gif);
            up1.RotateImage(180, 0, 0, up, 0, 0, up.Width, up.Height, 0);
            var down = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ArrowKeyDown), Bitmap.BitmapImageType.Gif);
            var down1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ArrowKeyDown), Bitmap.BitmapImageType.Gif);
            down1.RotateImage(180, 0, 0, down, 0, 0, down.Width, down.Height, 0);

            this.Images.Add("ArrowKeyDownDown", down1);
            this.Images.Add("ArrowKeyDownUp", up1);

            this.Images.Add("BackGround", new Bitmap(Resources.GetBytes(Resources.BinaryResources.BackGround), Bitmap.BitmapImageType.Gif));

            //fonts 
            this.Fonts.Add("Arial72", Resources.GetFont(Resources.FontResources.Arial72));
        }

        private void timer_Tick(GT.Timer timer)
        {
            Debug.Print("Temperature: " + this.SystemState.PrimaryAirTemperature.FormattedString() + " Relative Humidity: " + this.SystemState.PrimaryAirHumidity.FormattedString());
        }

    }
}
