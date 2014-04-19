using Microsoft.SPOT;
using System;
namespace Thermostat.Core
{
    public class Settings
    {
        public TemperatureSetting TargetTemp;

        private static object TempUnits_Lock = new object();
        private TemperatureUnits tempUnits;
        public TemperatureUnits TemperatureUnits
        {
            get
            {
                lock (TempUnits_Lock)
                {
                    return tempUnits;
                }
            }
            set
            {
                lock (TempUnits_Lock)
                {
                    tempUnits = value;
                }


                this.TargetTemp.Units = value;

            }
        }

        private static object HvacSHutdownCycleTime_Lock = new object();
        private int hvacShutdownCycleTime;
        public int HvacShutDownCycleTime
        {
            get
            {
                lock (HvacSHutdownCycleTime_Lock)
                {
                    return hvacShutdownCycleTime;
                }
            }
            set
            {
                lock (HvacSHutdownCycleTime_Lock)
                {
                    hvacShutdownCycleTime = value;
                }
            }
        }

        private static object HvacFanShutDownDelay_Lock = new object();
        private int HvacfanShutDownDelayDutration;
        public int HvacFanShutDownDelayDuration
        {
            get
            {
                lock (HvacFanShutDownDelay_Lock)
                {
                    return HvacfanShutDownDelayDutration;
                }
            }

            set
            {
                lock (HvacFanShutDownDelay_Lock)
                {
                    this.HvacfanShutDownDelayDutration = value;
                }
            }
        }

        private static object HeatFanShutDownDelay_Lock = new object();
        private int heatFanShutDownDelayDutration;
        public int HeatFanShutDownDelayDuration
        {
            get
            {
                lock (HeatFanShutDownDelay_Lock)
                {
                    return this.heatFanShutDownDelayDutration;
                }
            }

            set
            {
                lock (HeatFanShutDownDelay_Lock)
                {
                    this.heatFanShutDownDelayDutration = value;
                }
            }
        }



        private static object Mode_Lock = new object();
        private SystemModeEnum mode;
        public SystemModeEnum Mode
        {
            get
            {
                lock (Mode_Lock)
                {
                    return mode;
                }
            }
            set
            {
                lock (Mode_Lock)
                {
                    mode = value;
                }
            }
        }

        private static object DeadZone_Lock = new object();
        private int deadZone;
        public int DeadZone
        {
            get
            {
                lock (DeadZone_Lock)
                {
                    return deadZone;
                }
            }
            set
            {
                lock (DeadZone_Lock)
                {
                    deadZone = value;
                }
            }
        }

        private static object HeatAnticipator_Lock = new object();
        private double heatAnticipation;
        public double HeatAnticipation
        {
            get
            {
                lock (HeatAnticipator_Lock)
                {
                    return heatAnticipation;
                }
            }
            set
            {
                lock (HeatAnticipator_Lock)
                {
                    if (value > 1 || value < 0)
                    {
                        throw new ArgumentException("The value must be between 1 and 0");
                    }

                    this.heatAnticipation = value;
                }
            }
        }

        private static object TemperatureDifferential_Lock = new object();
        private int temperatureDifferential;
        public int TemperatureDifferential
        {
            get
            {
                lock (TemperatureDifferential_Lock)
                {
                    return temperatureDifferential;
                }
            }
            set
            {
                lock (TemperatureDifferential_Lock)
                {
                    if (value < 2)
                    {
                        throw new ArgumentException("Value must be greater than 4");
                    }

                    this.temperatureDifferential = value;
                }
            }
        }

        public int AutoMaxTemp
        {
            get
            {
                return temperatureDifferential + TargetTemp.Temperature;
            }
        }

        public int AutoMinTemp
        {
            get
            {
                return (int)TargetTemp.Temperature - temperatureDifferential;
            }
        }

        public bool GuiLoggedIn { get; set; }

        public Settings()
        {
            TargetTemp = new TemperatureSetting();
            this.Mode = SystemModeEnum.OFF;
            this.HvacShutDownCycleTime = 10;
            this.TemperatureUnits = TemperatureUnits.Farenheit;
            this.HvacFanShutDownDelayDuration = 1;
            this.HeatFanShutDownDelayDuration = 10;
            this.DeadZone = 2;
            this.HeatAnticipation = .2;
            this.TemperatureDifferential = 4;
        }
    }
}
