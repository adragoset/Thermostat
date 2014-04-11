using Gadgeteer.Modules.Seeed;

namespace Thermostat.Core
{
    public class SensorMeasurements
    {

        public Temperature PrimaryAirTemperature { get; private set; }

        public Humidity PrimaryAirHumidity { get; private set; }

        public SystemModeEnum SystemMode { get; private set; }

        private Gadgeteer.Modules.Seeed.TemperatureHumidity TempHumiditySensor;

        public SensorMeasurements(Gadgeteer.Modules.Seeed.TemperatureHumidity tempSensor)
        {
            PrimaryAirTemperature = new Temperature(0);
            PrimaryAirHumidity = new Humidity();
            SetTemperatureUnits(TemperatureUnits.Farenheit);
            TempHumiditySensor = tempSensor;
            TempHumiditySensor.MeasurementComplete += new TemperatureHumidity.MeasurementCompleteEventHandler(TemperatureMeasuermentComplete);
            TempHumiditySensor.StartContinuousMeasurements();
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
