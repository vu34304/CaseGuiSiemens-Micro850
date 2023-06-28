﻿using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using MqttClient = DemoCaseGui.Core.Application.Communication.MqttClient;

namespace DemoCaseGui.Core.Application.ViewModels
{
    public class CaseViewModel : BaseViewModel
    {
        private readonly S7Client _s7Client;
        private readonly MqttClient _mqttClient;
        private readonly Timer _timer;
        public bool IsConnected => _s7Client.IsConnected;
        public bool IsMqttConnected => _mqttClient.IsConnected;

        public bool? ledGreen_old, ledRed_old, ledYellow_old, dCMotor_old, statusIF6123_old, statusKT5112_old, statusO5C500_old, statusUGT524_old;
        public float? angleRB3100_old, tempTW2000_old;
        public ushort? countRB3100_old, distanceUGT524_old, setpoint_old, speed_old;
        //light and DC motor
        public bool? LedGreen { get; set; }
        public bool? LedRed { get; set; }
        public bool? LedYellow { get; set; }
        public bool? DCMotor { get; set; }

        //sensor
        public float? RB3100Angle { get; set; }
        public ushort? RB3100Count { get; set; }
        public float? TW2000Temp { get; set; }
        public bool? IF6123Status { get; set; }
        public bool? KT5112Status { get; set; }
        public bool? O5C500Status { get; set; }
        public bool? UGT524Status { get; set; }
        public ushort? UGT524Distance { get; set; }
        public ushort Resolution { get; set; } = 0;

        //inverter
        public bool? Status { get; set; } = false;
        public bool? Direction { get; set; }
        public bool? ButtonStartup { get; set; }
        public bool? ButtonStop { get; set; }
        public bool? MotorForward { get; set; }
        public bool? MotorReverse { get; set; }
        public ushort MotorSetpointWrite { get; set; }
        public ushort? MotorSetpoint { get; set; }
        public ushort? MotorSpeed { get; set; }

        //Siemens Demo Case 
        public bool? SiemensMode { get; set; }
        public bool? SiemensReset { get; set; }
        public bool? SiemensStart { get; set; }
        public bool? SiemensForward { get; set; }
        public bool? SiemensBackward { get; set; }
        public bool? SiemensHome { get; set; }
        public bool? SiemensLed6 { get; set; }
        public bool? SiemensLed7 { get; set; }
        public float SetpointSpeed { get; set; } = 0;
        public float SetpointPosition { get; set; } = 0;
        //
        public bool? Led0 { get; set; }
        public bool? Led1 { get; set; }
        public bool? Led2 { get; set; }
        public bool? Led3 { get; set; }
        public bool? Led4 { get; set; }
        public bool? Led5 { get; set; }
        public bool? Led6 { get; set; }
        public bool? Led7 { get; set; }
        public float? CurrentSpeed { get; set; }
        public float? CurrentPosition { get; set; }

        public ICommand ConnectCommand { get; set; }
        public ICommand ResolutionOKCommand { get; set; }
        public ICommand MotorSetpointOKCommand { get; set; }
        public ICommand SpeedOKCommand { get; set; }
        public ICommand PositionOKCommand { get; set; }


        public CaseViewModel()
        {
            _s7Client = new S7Client();
            _mqttClient = new MqttClient();
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _mqttClient.ApplicationMessageReceived += OnApplicationMessageReceived;
            ConnectCommand = new RelayCommand(Connect);
            ResolutionOKCommand = new RelayCommand(WriteResolution);
            MotorSetpointOKCommand = new RelayCommand(WriteMotorSetpoint);
            SpeedOKCommand = new RelayCommand(WriteSpeed);
            PositionOKCommand = new RelayCommand(WritePosition);

        }

    private async void TimerElapsed(object? sender, EventArgs args)
        {
            //light and DC motor
            if ((bool?)_s7Client.GetTagValue("ledGreen") != ledGreen_old)
            {
                LedGreen = (bool?)_s7Client.GetTagValue("ledGreen");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledGreen", JsonConvert.SerializeObject(_s7Client.GetTag("ledGreen")), true);
            }
            ledGreen_old = (bool?)_s7Client.GetTagValue("ledGreen");

            if ((bool?)_s7Client.GetTagValue("ledRed") != ledRed_old)
            {
                LedRed = (bool?)_s7Client.GetTagValue("ledRed");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledRed", JsonConvert.SerializeObject(_s7Client.GetTag("ledRed")), true);
            }
            ledRed_old = (bool?)_s7Client.GetTagValue("ledRed");

            if ((bool?)_s7Client.GetTagValue("ledYellow") != ledYellow_old)
            {
                LedYellow = (bool?)_s7Client.GetTagValue("ledYellow");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledYellow", JsonConvert.SerializeObject(_s7Client.GetTag("ledYellow")), true);
            }
            ledYellow_old = (bool?)_s7Client.GetTagValue("ledYellow");

            if ((bool?)_s7Client.GetTagValue("DCMotor") != dCMotor_old)
            {
                DCMotor = (bool?)_s7Client.GetTagValue("DCMotor");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/DCMotor", JsonConvert.SerializeObject(_s7Client.GetTag("DCMotor")), true);
            }
            dCMotor_old = (bool?)_s7Client.GetTagValue("DCMotor");

            
            //sensor
            if ((bool?)_s7Client.GetTagValue("statusIF6123") != statusIF6123_old)
            {
                IF6123Status = (bool?)_s7Client.GetTagValue("statusIF6123");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusIF6123", JsonConvert.SerializeObject(_s7Client.GetTag("statusIF6123")), true);
            }
            statusIF6123_old = (bool?)_s7Client.GetTagValue("statusIF6123");

            if ((bool?)_s7Client.GetTagValue("statusKT5112") != statusKT5112_old)
            {
                KT5112Status = (bool?)_s7Client.GetTagValue("statusKT5112");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusKT5112", JsonConvert.SerializeObject(_s7Client.GetTag("statusKT5112")), true);
            }
            statusKT5112_old = (bool?)_s7Client.GetTagValue("statusKT5112");

            if ((bool?)_s7Client.GetTagValue("statusO5C500") != statusO5C500_old)
            {
                O5C500Status = (bool?)_s7Client.GetTagValue("statusO5C500");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusO5C500", JsonConvert.SerializeObject(_s7Client.GetTag("statusO5C500")), true);
            }
            statusO5C500_old = (bool?)_s7Client.GetTagValue("statusO5C500");

            if ((bool?)_s7Client.GetTagValue("statusUGT524") != statusUGT524_old)
            {
                UGT524Status = (bool?)_s7Client.GetTagValue("statusUGT524");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusUGT524", JsonConvert.SerializeObject(_s7Client.GetTag("statusUGT524")), true);
            }
            statusUGT524_old = (bool?)_s7Client.GetTagValue("statusUGT524");

            if ((float?)_s7Client.GetTagValue("angleRB3100") != angleRB3100_old)
            {
                RB3100Angle = (float?)_s7Client.GetTagValue("angleRB3100");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/angleRB3100", JsonConvert.SerializeObject(_s7Client.GetTag("angleRB3100")), true);
            }
            angleRB3100_old = (float?)_s7Client.GetTagValue("angleRB3100");

            if ((ushort?)_s7Client.GetTagValue("countRB3100") != countRB3100_old)
            {
                RB3100Count = (ushort?)_s7Client.GetTagValue("countRB3100");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/countRB3100", JsonConvert.SerializeObject(_s7Client.GetTag("countRB3100")), true);
            }
            countRB3100_old = (ushort?)_s7Client.GetTagValue("countRB3100");

            if ((float?)_s7Client.GetTagValue("tempTW2000") != tempTW2000_old)
            {
                TW2000Temp = (float?)_s7Client.GetTagValue("tempTW2000");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/tempTW2000", JsonConvert.SerializeObject(_s7Client.GetTag("tempTW2000")), true);
            }
            tempTW2000_old = (float?)_s7Client.GetTagValue("tempTW2000");

            if ((ushort?)_s7Client.GetTagValue("distanceUGT524") != distanceUGT524_old)
            {
                if ((ushort?)_s7Client.GetTagValue("distanceUGT524") > 200) UGT524Distance = null;
                else UGT524Distance = (ushort?)_s7Client.GetTagValue("distanceUGT524");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/distanceUGT524", JsonConvert.SerializeObject(_s7Client.GetTag("distanceUGT524")), true);
            }
            distanceUGT524_old = (ushort?)_s7Client.GetTagValue("distanceUGT524");

            //inverter
            ButtonStartup = Status;
            ButtonStop = !Status ;
            MotorForward = Direction;
            MotorReverse = !Direction;
            MotorSetpoint = (ushort?)_s7Client.GetTagValue("setpoint");
            if ((ushort?)_s7Client.GetTagValue("speed") != speed_old)
            {
                MotorSpeed = (ushort?)_s7Client.GetTagValue("speed");
                await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/speed", JsonConvert.SerializeObject(_s7Client.GetTag("speed")), true);
            }
            speed_old = (ushort?)_s7Client.GetTagValue("speed");

            //Siemens Demo Case
            SiemensMode = (bool?)_s7Client.GetTagValue("mode_M");
            SiemensReset = (bool?)_s7Client.GetTagValue("reset_M");
            SiemensStart = (bool?)_s7Client.GetTagValue("start_M");
            SiemensForward = (bool?)_s7Client.GetTagValue("forward_M");
            SiemensBackward = (bool?)_s7Client.GetTagValue("backward_M");
            SiemensHome = (bool?)_s7Client.GetTagValue("home_M");
            SiemensLed6 = (bool?)_s7Client.GetTagValue("temp_led6");
            SiemensLed7 = (bool?)_s7Client.GetTagValue("temp_led7");

            Led0 = (bool?)_s7Client.GetTagValue("led0");
            Led1 = (bool?)_s7Client.GetTagValue("led1");
            Led2 = (bool?)_s7Client.GetTagValue("led2");
            Led3 = (bool?)_s7Client.GetTagValue("led3");
            Led4 = (bool?)_s7Client.GetTagValue("led4");
            Led5 = (bool?)_s7Client.GetTagValue("led5");
            Led6 = (bool?)_s7Client.GetTagValue("led6");
            Led7 = (bool?)_s7Client.GetTagValue("led7");
            CurrentSpeed = (float?)_s7Client.GetTagValue("current_speed_M");
            CurrentPosition = (float?)_s7Client.GetTagValue("current_position_M");

            Status = (bool?)_s7Client.GetTagValue("statusInverter");
            if (Status == false) Direction = null;
            else
            {
                if ((bool?)_s7Client.GetTagValue("directionForward") == true)
                    Direction = true;
                if ((bool?)_s7Client.GetTagValue("directionReverse") == true)
                    Direction = false;
            }

            /////
            //////
            ///
            ///
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/angleRB3100", JsonConvert.SerializeObject(_s7Client.GetTag("angleRB3100")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/countRB3100", JsonConvert.SerializeObject(_s7Client.GetTag("countRB3100")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/tempTW2000", JsonConvert.SerializeObject(_s7Client.GetTag("tempTW2000")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusIF6123", JsonConvert.SerializeObject(_s7Client.GetTag("statusIF6123")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusKT5112", JsonConvert.SerializeObject(_s7Client.GetTag("statusKT5112")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusO5C500", JsonConvert.SerializeObject(_s7Client.GetTag("statusO5C500")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/statusUGT524", JsonConvert.SerializeObject(_s7Client.GetTag("statusUGT524")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/distanceUGT524", JsonConvert.SerializeObject(_s7Client.GetTag("distanceUGT524")), false);
            ///
            ///
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledGreen", JsonConvert.SerializeObject(_s7Client.GetTag("ledGreen")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledRed", JsonConvert.SerializeObject(_s7Client.GetTag("ledRed")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/ledYellow", JsonConvert.SerializeObject(_s7Client.GetTag("ledYellow")), false);
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/DCMotor", JsonConvert.SerializeObject(_s7Client.GetTag("DCMotor")), false);
            /////
            ///////
            //await _mqttClient.Publish("VTSauto/AR_project/Desktop_pub/speed", JsonConvert.SerializeObject(_s7Client.GetTag("speed")), false);
        }


        private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payloadMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var commandMessage = JsonConvert.DeserializeObject<CommandMessage>(payloadMessage);

            switch (commandMessage.name)
            {
                case "controlRed":
                    _s7Client.WritePLC("DB2.DBX16.2", commandMessage.value);
                    break;
                case "controlGreen":
                    _s7Client.WritePLC("DB2.DBX16.0", commandMessage.value);
                    break;
                case "controlYellow":
                    _s7Client.WritePLC("DB2.DBX16.3", commandMessage.value);
                    break;
                case "controlDCMotor":
                    _s7Client.WritePLC("DB2.DBX16.1", commandMessage.value);
                    break;
                //
                case "setpoint":
                    _s7Client.WriteNumberPLC("DB4.DBW8", commandMessage.value);
                    break;
                case "startup":
                    Status = true;
                    _s7Client.WritePLC("DB4.DBX6.0", commandMessage.value);
                    break;
                case "stop":
                    Status = false;
                    Direction = null;
                    _s7Client.WritePLC("DB4.DBX6.1", commandMessage.value);
                    break;
                case "forward":
                    if (Status == true) Direction = true;
                    _s7Client.WritePLC("DB4.DBX6.2", commandMessage.value);
                    break;
                case "reverse":
                    if (Status == true) Direction = false;
                    _s7Client.WritePLC("DB4.DBX6.3", commandMessage.value);
                    break;
                default:
                    _s7Client.WritePLC(_s7Client.GetTagAddress(commandMessage.name), commandMessage.value);
                    break;
            }
        }

        public async void Connect()
        {
            await _s7Client.Connect();
            await _mqttClient.ConnectAsync();
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlRed");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlGreen");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlYellow");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/controlDCMotor");

            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/startup");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/stop");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/forward");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/reverse");
            await _mqttClient.Subscribe("VTSauto/AR_project/Desktop_write/setpoint");
            _timer.Enabled = true;
        }
        public void WriteResolution()
        {
            _s7Client.WriteNumberPLC(_s7Client.GetTagAddress("resolutionRB3100"), Resolution);
        }
        public void WriteMotorSetpoint()
        {
            _s7Client.WriteNumberPLC(_s7Client.GetTagAddress("setpoint"), MotorSetpointWrite);
        }
        public void WriteSpeed()
        {
            _s7Client.WritePLC(_s7Client.GetTagAddress("setpoint_speed_M"), SetpointSpeed);
        }

        public void WritePosition()
        {
            _s7Client.WritePLC(_s7Client.GetTagAddress("setpoint_position_M"), SetpointPosition);
        }
    }
}
