using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveCharts;
using Timer = System.Timers.Timer;
using MqttClient = DemoCaseGui.Core.Application.Communication.MqttClient;

namespace DemoCaseGui.Core.Application.ViewModels
{
 
    public class CaseMicroViewModel : BaseViewModel
    {
        private readonly Micro850Client _Micro850Client;
        private readonly MqttClient _mqttClient;
        private readonly Timer _timer;
        public Timer timer;
        public bool IsMqttConnected => _mqttClient.IsConnected;
        public ChartValues<double> Value { get; set; }

        //public bool? ledGreen_old, ledRed_old, ledYellow_old, dCMotor_old, statusIF6123_old, statusKT5112_old, statusO5C500_old, statusUGT524_old;
        //public float? angleRB3100_old, tempTW2000_old;
        //public ushort? countRB3100_old, distanceUGT524_old, setpoint_old, speed_old;
        ////light and DC motor
        //public bool? LedGreen { get; set; }
        //public bool? LedRed { get; set; }
        //public bool? LedYellow { get; set; }
        //public bool? DCMotor { get; set; }

        ////sensor
        //public float? RB3100Angle { get; set; }
        //public ushort? RB3100Count { get; set; }
        //public float? TW2000Temp { get; set; }
        //public bool? IF6123Status { get; set; }
        //public bool? KT5112Status { get; set; }
        //public bool? O5C500Status { get; set; }
        //public bool? UGT524Status { get; set; }
        //public ushort? UGT524Distance { get; set; }
        //public ushort Resolution { get; set; } = 0;

        //inverter
        //public bool? Status { get; set; } = false;
        //public bool? Direction { get; set; }
        //public bool? ButtonStartup { get; set; }
        //public bool? ButtonStop { get; set; }
        //public bool? MotorForward { get; set; }
        //public bool? MotorReverse { get; set; }
        //public ushort MotorSetpointWrite { get; set; }
        //public ushort? MotorSetpoint { get; set; }
        //public ushort? MotorSpeed { get; set; }

        ////Siemens Demo Case 
        //public bool? SiemensMode { get; set; }
        //public bool? SiemensReset { get; set; }
        //public bool? SiemensStart { get; set; }
        //public bool? SiemensForward { get; set; }
        //public bool? SiemensBackward { get; set; }
        //public bool? SiemensHome { get; set; }
        //public bool? SiemensLed6 { get; set; }
        //public bool? SiemensLed7 { get; set; }
        //public float SetpointSpeed { get; set; } = 0;
        //public float SetpointPosition { get; set; } = 0;
        ////
        public bool? Led0 { get; set; }
        public bool? Led1 { get; set; }
        public bool? Led2 { get; set; }
        public bool? Led3 { get; set; }
        public bool? Led4 { get; set; }
        public bool? Led5 { get; set; }
        public bool? Led6 { get; set; }
        public bool? Led7 { get; set; }
        public bool? Start { get; set; }
        public bool? Stop { get; set; }
        public float? CurrentSpeed { get; set; }
        public float? CurrentPosition { get; set; }
        public string? countRB3100_old, distanceUGT524_old, setpoint_old, speed_old;
       
        public string MotorSetpointWrite { get; set; }
        public string? MotorSetpoint { get; set; }
        public string? MotorSpeed { get; set; }
        public bool? MotorForward { get; set; }
        public bool? MotorReverse { get; set; }
        public bool? MotorForward1 { get; set; }
        public bool? MotorReverse1 { get; set; }

        public bool? Direction { get; set; }
        public bool? ButtonStartup { get; set; }
        public bool? ButtonStop { get; set; }

        public ICommand ConnectCommand { get; set; }
        public ICommand ResolutionOKCommand { get; set; }
        public ICommand MotorSetpointOKCommand { get; set; }
        public ICommand SpeedOKCommand { get; set; }
        public ICommand PositionOKCommand { get; set; }


        public CaseMicroViewModel()
        {
            _Micro850Client = new Micro850Client();
            _mqttClient = new MqttClient();
            _timer = new Timer(300);
            _timer.Elapsed += _timer_Elapsed;
            timer =new Timer(500);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            Value = new ChartValues<double> { };
            MotorSetpointOKCommand = new RelayCommand(WriteMotorSetpoint);
            ConnectCommand = new RelayCommand(Connect);
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Random rd = new Random();
            double val = rd.NextDouble() + 10;
            Value.Add(Math.Round(val,3));
            if (Value.Count() > 10)
            {
                Value.RemoveAt(0);
            }
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            

            Led2 = (bool?)_Micro850Client.GetTagValue("led2");
            Led3 = (bool?)_Micro850Client.GetTagValue("led3");
            Led4 = (bool?)_Micro850Client.GetTagValue("led4");
            Led5 = (bool?)_Micro850Client.GetTagValue("led5");
            Led6 = (bool?)_Micro850Client.GetTagValue("led6");
            Led7 = (bool?)_Micro850Client.GetTagValue("led7");  
            Start = (bool?)_Micro850Client.GetTagValue("start");
            Stop = (bool?)_Micro850Client.GetTagValue("stop");
            MotorForward1 = (bool?)_Micro850Client.GetTagValue("forward");
            MotorReverse1 = (bool?)_Micro850Client.GetTagValue("reverse");

            if(MotorForward1 is true)
            {
                MotorForward = true;
                MotorReverse = false;
            }
            else if(MotorReverse is true)
            {
                MotorForward = false;
                MotorReverse = true;
            }



            if (Start is true)
            {
                ButtonStartup = true;
                ButtonStop = false;
            }
            else if (Stop is true)
            {
                ButtonStartup = false;
                ButtonStop = true;
            }

            MotorSetpoint = (string?)_Micro850Client.GetTagValue("setpoint");
            if ((string?)_Micro850Client.GetTagValue("speed") != speed_old)
            {
                MotorSpeed = (string?)_Micro850Client.GetTagValue("speed");

            }
            speed_old = (string?)_Micro850Client.GetTagValue("speed");


        }


        public void WriteMotorSetpoint()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("setpoint"), MotorSetpointWrite);
        }



        public void Connect()
        {
            _Micro850Client.Connect();
            _timer.Enabled = true;
        }


    }
}
