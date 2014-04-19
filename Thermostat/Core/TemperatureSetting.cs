using System;
using Microsoft.SPOT;

namespace Thermostat.Core
{
    public class TemperatureSetting
    {
        private static object Temperature_Lock = new object();
        private int temperature;
        public int Temperature
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
                    this.temperature = value;
                }

                OnTemperatureSettingChanged(new TemperatureSettingChangedArgs(value, this.FormattedString()));
            }
        }

        private static object Units_Lock = new object();
        private TemperatureUnits units;
        public TemperatureUnits Units
        {
            get
            {
                lock (Units_Lock)
                {
                    return units;
                }
            }
            set
            {
                TemperatureUnits oldUnits;
                lock (Units_Lock)
                {
                    oldUnits = units;
                    this.units = value;
                }

                if (oldUnits == TemperatureUnits.Celcius && value == TemperatureUnits.Farenheit)
                {
                    this.Temperature = (int)(this.Temperature * 1.8 + 32.0);
                }
                else if (oldUnits == TemperatureUnits.Farenheit && value == TemperatureUnits.Celcius)
                {
                    this.Temperature = (int)(((this.Temperature - 32) * 5) / 9);
                }
                else if (oldUnits == TemperatureUnits.Celcius && value == TemperatureUnits.Kelvin)
                {
                    this.Temperature = (int)(this.Temperature + 273.15);
                }
                else if (oldUnits == TemperatureUnits.Farenheit && value == TemperatureUnits.Kelvin)
                {
                    this.Temperature = (int)(((this.Temperature - 32) / 1.8) + 273.15);
                }
                else if (oldUnits == TemperatureUnits.Kelvin && value == TemperatureUnits.Celcius)
                {
                    this.Temperature = (int)(this.Temperature - 273.15);
                }
                else if (oldUnits == TemperatureUnits.Kelvin && value == TemperatureUnits.Farenheit)
                {
                    this.Temperature = (int)(((this.Temperature - 273.15) * 1.8) + 32);
                }
            }
        }

        public delegate void TemperatureSettingChangedDelegate(object sender, TemperatureSettingChangedArgs e);
        public event TemperatureSettingChangedDelegate TemperatureSettingChanged;
        public class TemperatureSettingChangedArgs : EventArgs
        {
            public double Temperature { get; private set; }
            public string TemperatureString { get; set; }

            public TemperatureSettingChangedArgs(double e, string s)
            {
                this.Temperature = e;
                this.TemperatureString = s;
            }


        }

        private void OnTemperatureSettingChanged(TemperatureSettingChangedArgs e)
        {
            var handler = TemperatureSettingChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public TemperatureSetting()
        {
            this.temperature = 30;
            this.units = TemperatureUnits.Celcius;
        }

        public void IncrementTemperature()
        {
            int newTemp = this.Temperature + 1;
            if (this.Units == TemperatureUnits.Celcius)
            {
                if (newTemp <= 40 && newTemp >= 20)
                {
                    this.Temperature = newTemp;
                }
            }
            else if (this.units == TemperatureUnits.Farenheit)
            {
                if (newTemp <= 90 && newTemp >= 60)
                {
                    this.Temperature = newTemp;
                }
            }
            else
            {
                this.temperature = newTemp;
            }
        }

        public void DecrementTemperature()
        {
            int newTemp = this.Temperature - 1;
            if (this.Units == TemperatureUnits.Celcius)
            {
                if (newTemp <= 40 && newTemp >= 20)
                {
                    this.Temperature = newTemp;
                }
            }
            else if (this.units == TemperatureUnits.Farenheit)
            {
                if (newTemp <= 90 && newTemp >= 60)
                {
                    this.Temperature = newTemp;
                }
            }
            else
            {
                this.temperature = newTemp;
            }
        }

        public string FormattedString()
        {
            if (this.Units == TemperatureUnits.Celcius)
            {
                return this.Temperature + " C";
            }
            else if (this.Units == TemperatureUnits.Farenheit)
            {
                return this.Temperature + " F";
            }
            else
            {
                return this.Temperature + " K";
            }
        }
    }
}
