namespace Thermostat.Core
{
    public class Humidity
    {
        private static object Value_Lock = new object();
        private double Value { get; set; }

        public Humidity()
        {
            SetHumidity(0);
        }

        public double GetHumidity()
        {
            double value = 0;
            lock (Value_Lock)
            {
                value = this.Value;
            }
            return System.Math.Round(value * 100.0) / 100.0;
        }

        public string FormattedString()
        {
            return GetHumidity().ToString("f2") + '%';
        }

        public void SetHumidity(double relativeHumidity)
        {
            lock (Value_Lock)
            {
                this.Value = relativeHumidity;
            }
        }
    }
}
