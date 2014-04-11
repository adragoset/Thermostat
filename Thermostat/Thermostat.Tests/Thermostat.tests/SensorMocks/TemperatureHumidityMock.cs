using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.Seeed;

namespace Thermostat.Tests.SensorMocks
{
    public class TemperatureHumidityMock: TemperatureHumidity
    {
        public TemperatureHumidityMock(int value): base(value) {
            
        }

        public void StartContinuousMeasurement() {
 
        }
    }
}
