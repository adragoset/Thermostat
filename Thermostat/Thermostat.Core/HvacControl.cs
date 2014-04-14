using Gadgeteer.Modules.GHIElectronics;
using GT = Gadgeteer;

namespace Thermostat.Core
{

    public class HvacControl
    {
        private static double Temperature_Differential_DeadZone_Multipler = .50;
        private static int Execution_Period = 5000;
        private static int Millisecond_Conversion_Factor = 60000;

        private bool HeatOn;
        private Relay_X1 Heat;

        private GT.Timer StartDelayTimer;
        private bool CompressorStartDelay;
        private bool CompressorOn;
        private Relay_X1 Cool;
        private GT.Timer FanShutDownDelayTimer { get; set; }

        private bool FanOn;
        private Relay_X1 Fan;
        private Settings SystemSettings;
        private SensorMeasurements SystemState;
        private GT.Timer ControlLoopTimer { get; set; }

        private SystemModeEnum CurrentMode { get; set; }

        public HvacControl(Relay_X1 Heat, Relay_X1 Cool, Relay_X1 Fan, Settings SystemSettings, SensorMeasurements SystemState)
        {
            // TODO: Complete member initialization
            this.Heat = Heat;
            this.HeatOn = false;
            this.Heat.TurnOff();

            this.Cool = Cool;
            this.CompressorStartDelay = false;
            this.CompressorOn = false;
            this.Cool.TurnOff();

            this.Fan = Fan;
            this.FanOn = false;
            this.Fan.TurnOff();

            this.SystemSettings = SystemSettings;
            this.SystemState = SystemState;

            this.CurrentMode = SystemModeEnum.OFF;

            // Create a timer
            ControlLoopTimer = new GT.Timer(Execution_Period);
            ControlLoopTimer.Tick += new GT.Timer.TickEventHandler(Run);
            ControlLoopTimer.Start();
        }

        private void Run(Gadgeteer.Timer timer)
        {
            if (this.CurrentMode != SystemSettings.Mode && SystemSettings.Mode != SystemModeEnum.OFF)
            {
                if (this.CurrentMode == SystemModeEnum.HVAC)
                {
                    this.SwitchCompressorOff();
                }
                else if (this.CurrentMode == SystemModeEnum.HEAT)
                {
                    this.SwitchHeatOff();
                }
            }

            if (SystemSettings.Mode == SystemModeEnum.AUTO)
            {
                RunAutoCycle();
            }
            if (SystemSettings.Mode == SystemModeEnum.HVAC)
            {
                RunHvacCycle();
            }
            else if (SystemSettings.Mode == SystemModeEnum.HEAT)
            {
                RunHeatCycle();
            }
            else if (SystemSettings.Mode == SystemModeEnum.FAN)
            {
                RunFanCycle();
            }
            else if (SystemSettings.Mode == SystemModeEnum.OFF)
            {
                if (this.CompressorOn == true || this.FanOn == true || this.HeatOn == true)
                {
                    SwitchOff();
                }
            }

        }

        private void RunFanCycle()
        {
            this.CurrentMode = SystemModeEnum.FAN;
            this.SwitchFanOn();
        }

        private void RunAutoCycle()
        {
            this.CurrentMode = SystemModeEnum.AUTO;

            if (this.SystemState.PrimaryAirTemperature.GetTemperature() >= this.SystemSettings.AutoMaxTemp)
            {
                this.SwitchFanOn();
                this.SwitchCompressorOn();
            }
            else if (this.SystemState.PrimaryAirTemperature.GetTemperature() <= this.SystemSettings.AutoMinTemp +
                (this.SystemSettings.TemperatureDifferential * Temperature_Differential_DeadZone_Multipler))
            {
                if (this.CompressorOn)
                {
                    this.SwitchCompressorOff();
                    this.SwitchFanOffWithDelay(this.SystemSettings.HvacFanShutDownDelayDuration);
                }
            }

            if (this.SystemState.PrimaryAirTemperature.GetTemperature() <= this.SystemSettings.AutoMinTemp)
            {
                this.SwitchFanOn();
                this.SwitchHeatOn();
            }
            else if (this.SystemState.PrimaryAirTemperature.GetTemperature() <= this.SystemSettings.AutoMaxTemp
                - (this.SystemSettings.TemperatureDifferential * Temperature_Differential_DeadZone_Multipler - this.SystemSettings.HeatAnticipation))
            {
                if (this.HeatOn)
                {
                    this.SwitchHeatOff();
                    this.SwitchFanOffWithDelay(this.SystemSettings.HeatFanShutDownDelayDuration);
                }
            }


        }

        private void RunHeatCycle()
        {
            this.CurrentMode = SystemModeEnum.HEAT;

            if ((this.SystemSettings.TargetTemp.Temperature + this.SystemSettings.DeadZone - this.SystemSettings.HeatAnticipation) <= this.SystemState.PrimaryAirTemperature.GetTemperature() && this.HeatOn == true)
            {
                this.SwitchHeatOff();
                this.SwitchFanOffWithDelay(this.SystemSettings.HeatFanShutDownDelayDuration);
            }
            else if ((this.SystemSettings.TargetTemp.Temperature - this.SystemSettings.DeadZone) >= this.SystemState.PrimaryAirTemperature.GetTemperature() && HeatOn == false)
            {
                this.SwitchFanOn();
                this.SwitchHeatOn();

            }
        }

        private void RunHvacCycle()
        {
            this.CurrentMode = SystemModeEnum.HVAC;

            if ((this.SystemSettings.TargetTemp.Temperature - this.SystemSettings.DeadZone) >= this.SystemState.PrimaryAirTemperature.GetTemperature() && this.CompressorOn != false)
            {
                this.SwitchCompressorOff();
                this.SwitchFanOffWithDelay(this.SystemSettings.HvacFanShutDownDelayDuration);
            }
            else if ((this.SystemSettings.TargetTemp.Temperature + this.SystemSettings.DeadZone) <= this.SystemState.PrimaryAirTemperature.GetTemperature() && this.CompressorOn != true)
            {
                this.SwitchFanOn();
                this.SwitchCompressorOn();
            }
        }

        private void SwitchOff()
        {
            this.CurrentMode = SystemModeEnum.OFF;

            if (this.HeatOn)
            {
                SwitchHeatOff();
            }

            if (this.CompressorOn)
            {
                SwitchCompressorOff();
                SwitchFanOffWithDelay(this.SystemSettings.HvacFanShutDownDelayDuration);
            }

            if (this.FanOn)
            {
                SwitchFanOff();
            }

        }

        private void SwitchFanOffWithDelay(int delay)
        {
            if (FanShutDownDelayTimer == null || FanShutDownDelayTimer.IsRunning == false)
            {
                FanShutDownDelayTimer = new GT.Timer(delay * Millisecond_Conversion_Factor);
                FanShutDownDelayTimer.Behavior = GT.Timer.BehaviorType.RunOnce;
                FanShutDownDelayTimer.Tick += new GT.Timer.TickEventHandler(SwitchFanOffDelegate);
                FanShutDownDelayTimer.Start();
            }
        }

        private void SwitchFanOn()
        {
            if (this.FanOn == false)
            {
                this.FanOn = true;
                this.Fan.TurnOn();
            }
        }

        private void SwitchFanOff()
        {
            if (this.FanOn == true && HeatOn != true && CompressorOn != true
                && this.FanShutDownDelayTimer != null && this.FanShutDownDelayTimer.IsRunning == false)
            {
                this.FanOn = false;
                this.Fan.TurnOff();
            }
            else if (this.FanOn == true && HeatOn != true && CompressorOn != true
                && this.FanShutDownDelayTimer == null)
            {
                this.FanOn = false;
                this.Fan.TurnOff();
            }
        }

        private void SwitchFanOffDelegate(Gadgeteer.Timer timer)
        {
            if (this.FanOn == true && HeatOn != true && CompressorOn != true)
            {
                this.FanOn = false;
                this.Fan.TurnOff();
            }
        }

        private void SwitchHeatOn()
        {
            if (this.HeatOn == false && !this.CompressorOn && this.FanOn == true)
            {
                this.HeatOn = true;
                this.Heat.TurnOn();
            }
        }

        private void SwitchHeatOff()
        {
            if (this.HeatOn == true)
            {
                this.HeatOn = false;
                this.Heat.TurnOff();
            }
        }

        private void SwitchCompressorOff()
        {
            if (this.CompressorOn == true)
            {
                this.CompressorStartDelay = true;
                this.CompressorOn = false;
                this.Cool.TurnOff();

                // Create a timer
                StartDelayTimer = new GT.Timer(SystemSettings.HvacShutDownCycleTime * Millisecond_Conversion_Factor);
                StartDelayTimer.Behavior = GT.Timer.BehaviorType.RunOnce;
                StartDelayTimer.Tick += new GT.Timer.TickEventHandler(CompressorResetStartDelay);
                StartDelayTimer.Start();
            }
        }

        private void SwitchCompressorOn()
        {
            if (!this.CompressorStartDelay  && !this.CompressorOn && !this.HeatOn && this.FanOn == true)
            {
                this.CompressorOn = true;
                this.Cool.TurnOn();
            }
        }

        private void CompressorResetStartDelay(GT.Timer timer)
        {
            this.CompressorStartDelay = false;
        }

    }
}
