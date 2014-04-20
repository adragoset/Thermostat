using Microsoft.SPOT;
namespace Thermostat.Core
{
    public enum TemperatureUnits { Farenheit, Celcius, Kelvin }
    public class Temperature
    {
        private static object Temperature_Lock = new object();

        private double temperature;
        private double Value
        {
            get
            {
                lock (Temperature_Lock)
                {
                    return temperature;
                }
            }
            set
            {
                lock (Temperature_Lock)
                {
                    temperature = value;
                }

                OnTemperatureChanged(new TemperatureChangedArgs(temperature, this.FormattedString()));
            }
        }

        private static object Units_Lock = new object();
        private TemperatureUnits Units { get; set; }

        //events 
        //Events
        public delegate void TemperatureChangedDelegate(object sender, TemperatureChangedArgs e);
        public event TemperatureChangedDelegate TemperatureChanged;
        public class TemperatureChangedArgs : EventArgs
        {
            public double Temperature { get; private set; }
            public string TemperatureString { get; set; }

            public TemperatureChangedArgs(double e, string s)
            {
                this.Temperature = e;
                this.TemperatureString = s;
            }
        }

        public Temperature(double temp)
        {
            this.Value = temp;
            SetUnitsCelcius();
        }

        public void setUnitsFarenheit()
        {
            lock (Units_Lock)
            {
                this.Units = TemperatureUnits.Farenheit;
            }
        }

        public void SetUnitsCelcius()
        {
            lock (Units_Lock)
            {
                this.Units = TemperatureUnits.Celcius;
            }
        }

        public void SetUnitsKelvin()
        {
            lock (Units_Lock)
            {
                this.Units = TemperatureUnits.Kelvin;
            }
        }

        private void OnTemperatureChanged(TemperatureChangedArgs e)
        {
            var handler = TemperatureChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public double GetTemperature()
        {

            double convertedValue = 0;
            double value = 0;

            value = this.Value;

            if (this.Units == TemperatureUnits.Celcius)
            {
                convertedValue = value;
            }
            else if (this.Units == TemperatureUnits.Farenheit)
            {
                convertedValue = value * 1.8 + 32;
            }
            else
            {
                convertedValue = value + 273.15;
            }

            return System.Math.Round(convertedValue * 100.0) / 100.0;
        }

        public int GetTemperatureInC()
        {
            return (int)(this.Value);
        }

        public string FormattedString()
        {
            if (this.Units == TemperatureUnits.Celcius)
            {
                return this.GetTemperature().ToString("f2") + " C";
            }
            else if (this.Units == TemperatureUnits.Farenheit)
            {
                return this.GetTemperature().ToString("f2") + " F";
            }
            else
            {
                return this.GetTemperature().ToString("f2") + " K";
            }
        }

        public void SetTemperature(double temperature)
        {
            this.Value = temperature;
        }
    }
}
