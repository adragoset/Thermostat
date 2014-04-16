using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT;
using System;
using GT = Gadgeteer;

namespace Thermostat.Core
{
    public class SensorMeasurements
    {

        public Temperature PrimaryAirTemperature { get; private set; }

        public Humidity PrimaryAirHumidity { get; private set; }

        public SystemModeEnum SystemMode { get; private set; }

        private Gadgeteer.Modules.Seeed.TemperatureHumidity TempHumiditySensor;

        public static object Date_Lock = new object();
        private DateTime dateNow { get; set; }
        public DateTime DateNow
        {
            get
            {
                lock (Date_Lock)
                {
                    return dateNow;
                }
                
            }
            set
            {
                lock (Date_Lock)
                {
                    dateNow = value;
                }
                var handler = ClockChangedEvent;
                if (handler != null)
                {
                    ClockChangedEvent(this.DateNow, new ClockChangedArgs(value));
                }
            }
        }

        private GT.Timer ClockTimer { get; set; }

        public delegate void ClockChangedDelegate(object sender, ClockChangedArgs e);
        public event ClockChangedDelegate ClockChangedEvent;
        public class ClockChangedArgs : EventArgs
        {
            public DateTime Now { get; private set; }

            public ClockChangedArgs(DateTime e)
            {
                this.Now = e;
            }


        }

        public SensorMeasurements(Gadgeteer.Modules.Seeed.TemperatureHumidity tempSensor)
        {
            PrimaryAirTemperature = new Temperature(0);
            PrimaryAirHumidity = new Humidity();
            SetTemperatureUnits(TemperatureUnits.Farenheit);
            TempHumiditySensor = tempSensor;
            TempHumiditySensor.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(TemperatureMeasuermentComplete);
            TempHumiditySensor.StartContinuousMeasurements();

            // Create a timer
            ClockTimer = new GT.Timer(500);
            ClockTimer.Tick += new GT.Timer.TickEventHandler(Clock_Tick);
            ClockTimer.Start();
        }

        private void Clock_Tick(GT.Timer timer)
        {
            this.DateNow = DateTime.Now;
        }

        public void SetTemperatureUnits(TemperatureUnits units)
        {
            if (units == TemperatureUnits.Farenheit)
            {
                this.PrimaryAirTemperature.setUnitsFarenheit();
            }
            else if (units == TemperatureUnits.Celcius)
            {
                this.PrimaryAirTemperature.SetUnitsCelcius();
            }
            else
            {
                this.PrimaryAirTemperature.SetUnitsKelvin();
            }
        }

        private void TemperatureMeasuermentComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            PrimaryAirTemperature.SetTemperature(temperature);
            PrimaryAirHumidity.SetHumidity(relativeHumidity);
        }
    }
}
