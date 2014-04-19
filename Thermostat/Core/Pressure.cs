using System;
using Microsoft.SPOT;

namespace Thermostat.Core
{
    public class Pressure
    {
        private static object Value_Lock = new object();
        private double value { get; set; }

        public delegate void PressureChangedDelegate(object sender, PressureChangedArgs e);
        public event PressureChangedDelegate PressureChanged;
        public class PressureChangedArgs : EventArgs
        {
            public double Pressure { get; private set; }
            public string PressureString { get; set; }

            public PressureChangedArgs(double e, string s)
            {
                this.Pressure = e;
                this.PressureString = s;
            }


        }

        public Pressure()
        {
            this.value = 0;
        }

        public double GetAtmPressure()
        {
            double value = 0;
            lock (Value_Lock)
            {
                value = this.value * .001;
            }
            return System.Math.Round(value * 100.0) / 100.0;
        }

        public string FormattedString()
        {
            return GetAtmPressure().ToString("f2") + " BAR";
        }

        private void OnPressureChanged(PressureChangedArgs e)
        {
            var handler = PressureChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void SetAtmPressure(double pressure)
        {
            lock (Value_Lock)
            {
                this.value = pressure;
            }
            OnPressureChanged(new PressureChangedArgs(pressure, this.FormattedString()));
        }
    }
}
