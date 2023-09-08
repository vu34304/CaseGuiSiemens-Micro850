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
using NPOI.SS.Formula.Functions;
using static NPOI.HSSF.Util.HSSFColor;
using System.ComponentModel;
using HslCommunication.Profinet.XINJE;
using System;
using System.Windows;

namespace DemoCaseGui.Core.Application.ViewModels
{

    public class CaseMicroViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly M850Client _Micro850Client;
        private readonly MqttClient _mqttClient;
        private readonly Timer _timer;
        public bool IsMqttConnected => _mqttClient.IsConnected;
        public ChartValues<float> Value { get; set; }
        //TrafficLight

        public bool? Led2 { get; set; }
        public bool? Led3 { get; set; }
        public bool? Led4 { get; set; }
        public bool? Led5 { get; set; }
        public bool? Led6 { get; set; }
        public bool? Led7 { get; set; }

        public bool? led2_old, led3_old, led4_old, led5_old, led6_old, led7_old;

        public bool? Start_TrafficLights { get; set; }
        public bool? Stop_TrafficLights { get; set; }
        public bool? start_trafficlights_old, stop_trafficlights_old;

        public float? Edit_RedLed { get; set; }
        public float? Edit_YellowLed { get; set; }
        public float? Edit_GreenLed { get; set; }

        public ushort? edit_redled_old, edit_greenled_old, edit_yellowled_old, setpoint_old, time_display_a_old, time_display_b_old;

        //Inverter
        public bool? Start { get; set; }
        public bool? Stop { get; set; }
        public bool? start_old, stop_old, forward_old, reverse_old;

        public float? countRB3100_old, distanceUGT524_old, speed_old;

        public float? MotorSetpointWrite { get; set; }
        public float? MotorSetpoint { get; set; }
        public float? Time_Display_A { get; set; }
        public float? Time_Display_B { get; set; }
        public float? MotorSpeed { get; set; }
        public bool? MotorForward { get; set; }
        public bool? MotorReverse { get; set; }
        public bool? MotorForward1 { get; set; }
        public bool? MotorReverse1 { get; set; }

        public float? Direction { get; set; }
        public bool? ButtonStartup { get; set; }
        public bool? ButtonStop { get; set; }
        public bool? ButtonStartup1 { get; set; }
        public bool? ButtonStop1 { get; set; }

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
            _Micro850Client = new M850Client();
            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed;
            Value = new ChartValues<float> { };

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

            if ((bool?)_Micro850Client.GetTagValue("start_trafficlight") != start_trafficlights_old)
            {
                Start_TrafficLights = (bool?)_Micro850Client.GetTagValue("start_trafficlight");
            }
            start_trafficlights_old = (bool?)_Micro850Client.GetTagValue("start_trafficlight");
            if (Start_TrafficLights is true)
            {
                ButtonStartup1 = true;
                ButtonStop1 = false;
            }

            if ((bool?)_Micro850Client.GetTagValue("stop_trafficlight") != stop_trafficlights_old)
            {
                Stop_TrafficLights = (bool?)_Micro850Client.GetTagValue("stop_trafficlight");
            }
            stop_trafficlights_old = (bool?)_Micro850Client.GetTagValue("stop_trafficlight");
            if (Stop_TrafficLights is true)
            {
                ButtonStartup1 = false;
                ButtonStop1 = true;
            }


            if ((bool?)_Micro850Client.GetTagValue("led2") != led2_old)
            {
                Led2 = (bool?)_Micro850Client.GetTagValue("led2");
            }
            led2_old = (bool?)_Micro850Client.GetTagValue("led2");

            if ((bool?)_Micro850Client.GetTagValue("led3") != led3_old)
            {
                Led3 = (bool?)_Micro850Client.GetTagValue("led3");
            }
            led3_old = (bool?)_Micro850Client.GetTagValue("led3");

            if ((bool?)_Micro850Client.GetTagValue("led4") != led4_old)
            {
                Led4 = (bool?)_Micro850Client.GetTagValue("led4");
            }
            led4_old = (bool?)_Micro850Client.GetTagValue("led4");

            if ((bool?)_Micro850Client.GetTagValue("led5") != led5_old)
            {
                Led5 = (bool?)_Micro850Client.GetTagValue("led5");
            }
            led5_old = (bool?)_Micro850Client.GetTagValue("led5");

            if ((bool?)_Micro850Client.GetTagValue("led6") != led6_old)
            {
                Led6 = (bool?)_Micro850Client.GetTagValue("led6");
            }
            led6_old = (bool?)_Micro850Client.GetTagValue("led6");

            if ((bool?)_Micro850Client.GetTagValue("led7") != led7_old)
            {
                Led7 = (bool?)_Micro850Client.GetTagValue("led7");
            }
            led7_old = (bool?)_Micro850Client.GetTagValue("led7");

            if ((ushort?)_Micro850Client.GetTagValue("edit_redled") != edit_redled_old)
            {
                Edit_RedLed = (ushort?)_Micro850Client.GetTagValue("edit_redled");
            }
            edit_redled_old = (ushort?)_Micro850Client.GetTagValue("edit_redled");

            if ((ushort?)_Micro850Client.GetTagValue("edit_greenled") != edit_greenled_old)
            {
                Edit_GreenLed = (ushort?)_Micro850Client.GetTagValue("edit_greenled");
            }
            edit_greenled_old = (ushort?)_Micro850Client.GetTagValue("edit_greenled");

            if ((ushort?)_Micro850Client.GetTagValue("edit_yellowled") != edit_yellowled_old)
            {
                Edit_YellowLed = (ushort?)_Micro850Client.GetTagValue("edit_yellowled");
            }
            edit_yellowled_old = (ushort?)_Micro850Client.GetTagValue("edit_yellowled");

            if ((ushort?)_Micro850Client.GetTagValue("time_display_a") != time_display_a_old)
            {
                Time_Display_A = (ushort?)_Micro850Client.GetTagValue("time_display_a");
            }
            time_display_a_old = (ushort?)_Micro850Client.GetTagValue("time_display_a");

            if ((ushort?)_Micro850Client.GetTagValue("time_display_b") != time_display_b_old)
            {
                Time_Display_B = (ushort?)_Micro850Client.GetTagValue("time_display_b");
            }
            time_display_b_old = (ushort?)_Micro850Client.GetTagValue("time_display_b");

            //Inverter

            if ((bool?)_Micro850Client.GetTagValue("start_inverter") != start_old)
            {
                Start = (bool?)_Micro850Client.GetTagValue("start_inverter");
            }
            start_old = (bool?)_Micro850Client.GetTagValue("stop_inverter");
            if (Start is true)
            {
                ButtonStartup = true;
                ButtonStop = false;
            }


            if ((bool?)_Micro850Client.GetTagValue("stop_inverter") != stop_old)
            {
                Stop = (bool?)_Micro850Client.GetTagValue("stop_inverter");

            }
            if (Stop is true)
            {
                ButtonStartup = false;
                ButtonStop = true;
            }
            stop_old = (bool?)_Micro850Client.GetTagValue("stop_inverter");

            if ((bool?)_Micro850Client.GetTagValue("forward") != forward_old)
            {
                MotorForward1 = (bool?)_Micro850Client.GetTagValue("forward");
                if (MotorForward1 is true)
                {
                    MotorForward = true;
                    MotorReverse = false;
                }
            }
            forward_old = (bool?)_Micro850Client.GetTagValue("forward");

            if ((bool?)_Micro850Client.GetTagValue("reverse") != reverse_old)
            {
                MotorReverse1 = (bool?)_Micro850Client.GetTagValue("reverse");
                if (MotorReverse1 is true)
                {
                    MotorForward = false;
                    MotorReverse = true;
                }
            }
            reverse_old = (bool?)_Micro850Client.GetTagValue("reverse");

            if ((ushort?)_Micro850Client.GetTagValue("setpoint") != setpoint_old)
            {
                MotorSetpoint = (ushort?)_Micro850Client.GetTagValue("setpoint");
            }
            setpoint_old = (ushort?)_Micro850Client.GetTagValue("setpoint");

            if ((float?)_Micro850Client.GetTagValue("speed") != speed_old)
            {
                MotorSpeed = (float?)_Micro850Client.GetTagValue("speed");
                float   MotorSpeed1 = (float)_Micro850Client.GetTagValue("speed");
                if ( Value.Count() < 15)
                {                
                        Value.Add(MotorSpeed1);
                }
                else Value.RemoveAt(0);

            }
            speed_old = (float?)_Micro850Client.GetTagValue("speed");




            //Led2 = ((string?)_Micro850Client.GetTagValue("led2") == "True") ? true : false;
            ////Led2 = (bool?)_Micro850Client.GetTagValue("led2");
            //Led3 = ((string?)_Micro850Client.GetTagValue("led3") == "True") ? true : false;
            //Led4 = ((string?)_Micro850Client.GetTagValue("led4") == "True") ? true : false;
            //Led5 = ((string?)_Micro850Client.GetTagValue("led5") == "True") ? true : false;
            //Led6 = ((string?)_Micro850Client.GetTagValue("led6") == "True") ? true : false;
            //Led7 = ((string?)_Micro850Client.GetTagValue("led7") == "True") ? true : false;
            //Edit_RedLed = (string?)_Micro850Client.GetTagValue("edit_redled");
            //Edit_YellowLed = (string?)_Micro850Client.GetTagValue("edit_yellowled");
            //Edit_GreenLed = (string?)_Micro850Client.GetTagValue("edit_greenled");

            ////Inverter
            //Start = ((string?)_Micro850Client.GetTagValue("start_inverter") == "True") ? true : false; 
            //Stop = ((string?)_Micro850Client.GetTagValue("stop_inverter") == "True") ? true : false;
            //MotorForward1 = ((string?)_Micro850Client.GetTagValue("forward") == "True") ? true : false;
            //MotorReverse1 = ((string?)_Micro850Client.GetTagValue("reverse") == "True") ? true : false;

            //if (MotorForward1 is true)
            //{
            //    MotorForward = true;
            //    MotorReverse = false;
            //}
            //else if (MotorReverse1 is true)
            //{
            //    MotorForward = false;
            //    MotorReverse = true;
            //}



            //if (Start is true)
            //{
            //    ButtonStartup = true;
            //    ButtonStop = false;
            //}
            //else if (Stop is true)
            //{
            //    ButtonStartup = false;
            //    ButtonStop = true;
            //}

            //MotorSetpoint = (string?)_Micro850Client.GetTagValue("setpoint");


            //if ((string?)_Micro850Client.GetTagValue("speed") != speed_old)
            //{
            //    MotorSpeed = (string?)_Micro850Client.GetTagValue("speed");
            //}
            //speed_old = (string?)_Micro850Client.GetTagValue("speed");
            //string data = (string?)_Micro850Client.GetTagValue("speed");
            //double speed;
            //bool test = double.TryParse(data,out  speed);

            //if (Value.Count() == 0 || Value.Count() < 15)
            //{
            //    if (test)
            //    {
            //        Value.Add(Math.Round(speed,3));
            //    }
            //}
            //else Value.RemoveAt(0);


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
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("edit_redled"), (UInt16)Edit_RedLed);
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("edit_yellowled"), (UInt16)Edit_YellowLed);
            _Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("edit_greenled"), (UInt16)Edit_GreenLed);
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
            //_Micro850Client.WriteNumberPLC(_Micro850Client.GetTagAddress("setpoint"), (float)MotorSetpointWrite);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_inverter"), true);
            Thread.Sleep(1000);
            _Micro850Client.WritePLC(_Micro850Client.GetTagAddress("confirm_inverter"), false);
        }


    }
}
