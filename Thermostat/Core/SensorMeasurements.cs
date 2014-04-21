using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using System;
using GT = Gadgeteer;

namespace Thermostat.Core
{
    public class SensorMeasurements
    {

        public Temperature PrimaryAirTemperature { get; private set; }

        public Humidity PrimaryAirHumidity { get; private set; }

        public Pressure AtmPressure { get; private set; }

        public SystemModeEnum SystemMode { get; private set; }

        private TemperatureHumidity TempHumiditySensor;
        private Barometer BarometerSensor;
        private GasSense CoSensor;
        private GasSense SmokeSensor;

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
                    ClockChangedEvent(this, new ClockChangedArgs(value));
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

        public SensorMeasurements(TemperatureHumidity tempSensor, Barometer barSensor, GasSense coSensor, GasSense smokeSensor)
        {
            PrimaryAirTemperature = new Temperature(0);
            PrimaryAirHumidity = new Humidity();
            AtmPressure = new Pressure();
            SetTemperatureUnits(TemperatureUnits.Farenheit);
            SetupSensorHandlers(tempSensor, barSensor, coSensor, smokeSensor);

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

        private void SetupSensorHandlers(TemperatureHumidity tempSensor, Barometer barSensor, GasSense coSensor, GasSense smokeSensor)
        {
            TempHumiditySensor = tempSensor;
            TempHumiditySensor.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(Temperature_Measuerment_Complete);
            TempHumiditySensor.StartContinuousMeasurements();

            BarometerSensor = barSensor;
            BarometerSensor.ContinuousMeasurementInterval = new TimeSpan(5000);
            BarometerSensor.MeasurementComplete += new Barometer.MeasurementCompleteEventHandler(Barometer_Measurement_Complete);
            BarometerSensor.StartContinuousMeasurements();

            CoSensor = coSensor;
            //CoSensor.
            SmokeSensor = smokeSensor;
        }

        private void Temperature_Measuerment_Complete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            PrimaryAirTemperature.SetTemperature(temperature);
            PrimaryAirHumidity.SetHumidity(relativeHumidity);
        }

        private void Barometer_Measurement_Complete(Barometer sender, Barometer.SensorData sensorData)
        {
            this.AtmPressure.SetAtmPressure(sensorData.Pressure);
        }
    }
}
