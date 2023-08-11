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
        public bool IsMqttConnected => _mqttClient.IsConnected;
        public ChartValues<double> Value { get; set; }

        //TrafficLight
        public bool? Led0 { get; set; }
        public bool? Led1 { get; set; }
        public bool? Led2 { get; set; }
        public bool? Led3 { get; set; }
        public bool? Led4 { get; set; }
        public bool? Led5 { get; set; }
        public bool? Led6 { get; set; }
        public bool? Led7 { get; set; }

        public double? Edit_RedLed { get; set; }
        public double? Edit_YellowLed { get; set; }
        public double? Edit_GreenLed { get; set; }

        //Inverter
        public bool? Start { get; set; }
        public bool? Stop { get; set; }
        public float? CurrentSpeed { get; set; }
        public float? CurrentPosition { get; set; }
        public double? countRB3100_old, distanceUGT524_old, setpoint_old, speed_old;
       
        public double? MotorSetpointWrite { get; set; }
        public double? MotorSetpoint { get; set; }
        public double? MotorSpeed { get; set; }
        public bool? MotorForward { get; set; }
        public bool? MotorReverse { get; set; }
        public bool? MotorForward1 { get; set; }
        public bool? MotorReverse1 { get; set; }

        public bool? Direction { get; set; }
        public bool? ButtonStartup { get; set; }
        public bool? ButtonStop { get; set; }

        //Command
        public ICommand ConnectCommand { get; set; }
        public ICommand MotorSetpointOKCommand { get; set; }
        public ICommand StartTrafficLightsCommand { get; set; }
        public ICommand StopTrafficLightsCommand { get; set; }
        public ICommand StartInverterCommand { get; set; }
        public ICommand StopInverterCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand ReverseCommand { get; set; }

        public CaseMicroViewModel()
        {
            _Micro850Client = new Micro850Client();
            _mqttClient = new MqttClient();
            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed;
            Value = new ChartValues<double> { };

            //Button Command
            ConnectCommand = new RelayCommand(Connect);
            //Traffic Light
            StartTrafficLightsCommand = new RelayCommand(Start_TrafficLight);
            StopTrafficLightsCommand = new RelayCommand(Stop_TrafficLight);
            ConfirmCommand = new RelayCommand(ConfirmTrafficLight);

            //Inverter
            MotorSetpointOKCommand = new RelayCommand(WriteMotorSetpoint);           
            StartInverterCommand = new RelayCommand(Start_Inverter);
            StopInverterCommand = new RelayCommand(Stop_Inverter);
            ForwardCommand = new RelayCommand(Forward_Inverter);
            ReverseCommand = new RelayCommand(Reverse_Inverter);


        }

       

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            //TrafficLight
            Led2 = (bool?)_Micro850Client.GetTagValue("led2");
            Led3 = (bool?)_Micro850Client.GetTagValue("led3");
            Led4 = (bool?)_Micro850Client.GetTagValue("led4");
            Led5 = (bool?)_Micro850Client.GetTagValue("led5");
            Led6 = (bool?)_Micro850Client.GetTagValue("led6");
            Led7 = (bool?)_Micro850Client.GetTagValue("led7");
            Edit_RedLed = (double?)_Micro850Client.GetTagValue("edit_redled");
            Edit_YellowLed = (double?)_Micro850Client.GetTagValue("edit_yellowled");
            Edit_GreenLed = (double?)_Micro850Client.GetTagValue("edit_greenled");
            //Inverter
            Start = (bool?)_Micro850Client.GetTagValue("start_inverter");
            Stop = (bool?)_Micro850Client.GetTagValue("stop_inverter");
            MotorForward1 = (bool?)_Micro850Client.GetTagValue("forward");
            MotorReverse1 = (bool?)_Micro850Client.GetTagValue("reverse");

            if (MotorForward1 is true)
            {
                MotorForward = true;
                MotorReverse = false;
            }
            else if (MotorReverse is true)
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

            MotorSetpoint = (double?)_Micro850Client.GetTagValue("setpoint");
            if ((double?)_Micro850Client.GetTagValue("speed") != speed_old)
            {
                MotorSpeed = (double?)_Micro850Client.GetTagValue("speed");
                Value.Add((double)_Micro850Client.GetTagValue("speed"));
                if (Value.Count() > 10) Value.RemoveAt(0);
            }
            speed_old = (double?)_Micro850Client.GetTagValue("speed");


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

        //TrafficLights
        public void Start_TrafficLight()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("start_trafficlight"),true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("start_trafficlight"), false);
        }

        public void Stop_TrafficLight() 
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("stop_trafficlight"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("stop_trafficlight"), false);
        }

        public void ConfirmTrafficLight()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("edit_redled"), Edit_RedLed);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("edit_yellowled"), Edit_YellowLed);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("edit_greenled"), Edit_YellowLed);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_trafficlight"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_trafficlight"), false);
            //confirm ?
        }

        //Inverter

        public void Start_Inverter()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("start_inverter"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("start_inverter"), false);
        }

        public void Stop_Inverter()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("stop_inverter"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("stop_inverter"), false);
        }

        public void Forward_Inverter()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("forward"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("forward"), false);
        }

        public void Reverse_Inverter()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("reverse"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("reverse"), false);
        }
    }
}
