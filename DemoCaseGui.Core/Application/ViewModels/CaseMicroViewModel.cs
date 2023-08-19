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
using AutomatedSolutions.ASCommStd.AB.Logix.Data;
using NPOI.SS.Formula.Functions;

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

        public string? led2 { get; set; }

        public bool? Led2 { get; set; }
        public bool? Led3 { get; set; }
        public bool? Led4 { get; set; }
        public bool? Led5 { get; set; }
        public bool? Led6 { get; set; }
        public bool? Led7 { get; set; }

        public bool? Start_TrafficLights { get; set; }
        public bool? Stop_TrafficLights { get; set; }

        public string? Edit_RedLed { get; set; }
        public string? Edit_YellowLed { get; set; }
        public string? Edit_GreenLed { get; set; }


        //Inverter
        public bool? Start { get; set; }
        public bool? Stop { get; set; }

        public string? countRB3100_old, distanceUGT524_old, setpoint_old, speed_old;

        public string? MotorSetpointWrite { get; set; }
        public string? MotorSetpoint { get; set; }
        public string? MotorSpeed { get; set; }
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
        public ICommand ConfirmTrafficLights_Command { get; set; }
        public ICommand ConfirmInverter_Command { get; set; }
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
            ConnectCommand = new RelayCommand(Connect);// Connect PLC
            //Traffic Light
            StartTrafficLightsCommand = new RelayCommand(Start_TrafficLight);
            StopTrafficLightsCommand = new RelayCommand(Stop_TrafficLight);
            ConfirmTrafficLights_Command = new RelayCommand(ConfirmTrafficLight);

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

           
            Led2 = ((string?)_Micro850Client.GetTagValue("led2") == "True") ? true : false;
            //Led2 = (bool?)_Micro850Client.GetTagValue("led2");
            Led3 = ((string?)_Micro850Client.GetTagValue("led3") == "True") ? true : false;
            Led4 = ((string?)_Micro850Client.GetTagValue("led4") == "True") ? true : false;
            Led5 = ((string?)_Micro850Client.GetTagValue("led5") == "True") ? true : false;
            Led6 = ((string?)_Micro850Client.GetTagValue("led6") == "True") ? true : false;
            Led7 = ((string?)_Micro850Client.GetTagValue("led7") == "True") ? true : false;
            Edit_RedLed = (string?)_Micro850Client.GetTagValue("edit_redled");
            Edit_YellowLed = (string?)_Micro850Client.GetTagValue("edit_yellowled");
            Edit_GreenLed = (string?)_Micro850Client.GetTagValue("edit_greenled");

            //Inverter
            Start = ((string?)_Micro850Client.GetTagValue("start_inverter") == "True") ? true : false; 
            Stop = ((string?)_Micro850Client.GetTagValue("stop_inverter") == "True") ? true : false;
            MotorForward1 = ((string?)_Micro850Client.GetTagValue("forward") == "True") ? true : false;
            MotorReverse1 = ((string?)_Micro850Client.GetTagValue("reverse") == "True") ? true : false;

            if (MotorForward1 is true)
            {
                MotorForward = true;
                MotorReverse = false;
            }
            else if (MotorReverse1 is true)
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
            string data = (string?)_Micro850Client.GetTagValue("speed");
            double speed;
            bool test = double.TryParse(data,out  speed);

            if (Value.Count() == 0 || Value.Count() < 15)
            {
                if (test)
                {
                    Value.Add(Math.Round(speed,3));
                }
            }
            else Value.RemoveAt(0);


        }






        public void Connect()
        {
            _Micro850Client.Connect();
            _timer.Enabled = true;
        }

        //TrafficLights
        public void Start_TrafficLight()
        {
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("start_trafficlight"), true);
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
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("edit_redled"), Edit_RedLed);
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("edit_yellowled"), Edit_YellowLed);
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("edit_greenled"), Edit_GreenLed);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_trafficlight"), true);
            Thread.Sleep(500);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_trafficlight"), false);

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

        public void WriteMotorSetpoint()
        {
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("setpoint"), MotorSetpointWrite);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_inverter"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_inverter"), false);
        }


    }
}
