using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace DemoCaseGui.Core.Application.Communication
{
    public class M850Client
    {
        private readonly AllenBradleyMicroCip plc;
        private readonly Timer _timer;
        public List<Tag> Tags { get; private set; }
        public List<MqttTag> MqttTags { get; private set; }

        public M850Client()
        {
            //Khoi tao PLC
            plc = new AllenBradleyMicroCip("192.168.1.50");
            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed;

            Tags = new()
        {
            
            //TrafficLights
            new("led2", "PLC.Vali_Micro850.led2", null, "_IO_EM_DO_02", DateTime.Now),
            new("led3", "PLC.Vali_Micro850.led3", null, "_IO_EM_DO_03", DateTime.Now),
            new("led4", "PLC.Vali_Micro850.led4", null, "_IO_EM_DO_04", DateTime.Now),
            new("led5", "PLC.Vali_Micro850.led5", null, "_IO_EM_DO_05", DateTime.Now),
            new("led6", "PLC.Vali_Micro850.led6", null, "_IO_EM_DO_06", DateTime.Now),
            new("led7", "PLC.Vali_Micro850.led7", null, "_IO_EM_DO_07", DateTime.Now),

            new("edit_redled", "PLC.Vali_Micro850.redtime", null, "HMI_DB.Traffic_Lights.Red_Time", DateTime.Now),
            new("edit_yellowled", "PLC.Vali_Micro850.yellowtime", null, "HMI_DB.Traffic_Lights.Yellow_Time", DateTime.Now),
            new("edit_greenled", "PLC.Vali_Micro850.greentime", null, "HMI_DB.Traffic_Lights.Green_Time", DateTime.Now),

             new("time_display_a", "PLC.Vali_Micro850.greentime", null, "HMI_DB.Traffic_Lights.Time_Display_A", DateTime.Now),
              new("time_display_b", "PLC.Vali_Micro850.greentime", null, "HMI_DB.Traffic_Lights.Time_Display_B", DateTime.Now),


            new("confirm_trafficlight", "PLC.Vali_Micro850.confirm_trafficlights", null, "HMI_DB.Traffic_Lights.Confirm", DateTime.Now),
            new("start_trafficlight", "PLC.Vali_Micro850.start_trafficlights", null, "HMI_DB.Traffic_Lights.Start", DateTime.Now),
            new("stop_trafficlight", "PLC.Vali_Siemens.stop_trafficlights", null, "HMI_DB.Traffic_Lights.Stop", DateTime.Now),

            //Inverter
            new("start_inverter", "PLC.Vali_Micro850.start_inverter", null, "HMI_DB.Inverter.Start", DateTime.Now),
            new("stop_inverter", "PLC.Vali_Micro850.stop_inverter", null, "HMI_DB.Inverter.Stop", DateTime.Now),
            new("setpoint", "PLC.Vali_Micro850.inverter_setpoint", null, "HMI_DB.Inverter.Speed_SP", DateTime.Now),
            new("speed", "PLC.Vali_Micro850.inverter_speed", null, "HMI_DB.Inverter.Speed_PV", DateTime.Now),
            new("forward", "PLC.Vali_Micro850.inverter_forward", null, "HMI_DB.Inverter.Fwd", DateTime.Now),
            new("reverse", "PLC.Vali_Micro850.inverter_reverse", null, "HMI_DB.Inverter.Rev", DateTime.Now),
            new("confirm_inverter", "PLC.Vali_Micro850.inverter_confirm", null, "HMI_DB.Inverter.Confirm", DateTime.Now),
        };
        }

        private  void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach(var tag in Tags)
            {
                if(tag.name is  "speed") 
                {
                    OperateResult<float> data =  plc.ReadFloat(tag.address);

                    if (data.IsSuccess)
                    {
                        // you get the right value
                        tag.value = data.Content;
                    }
                    else
                    {
                        // failed , but you still can know the failed detail
                       
                    }
                }
                else if (tag.name is "edit_redled" or "edit_yellowled" or "edit_greenled")
                {
                    OperateResult<UInt16> data =  plc.ReadUInt16(tag.address);

                    if (data.IsSuccess)
                    {
                        // you get the right value
                        tag.value = data.Content;
                    }
                    else
                    {
                        // failed , but you still can know the failed detail

                    }
                }
                else if (tag.name is "setpoint" or "time_display_a" or "time_display_b")
                {
                    OperateResult<Byte> data =  plc.ReadByte(tag.address);

                    if (data.IsSuccess)
                    {
                        // you get the right value
                        object value = data.Content;
                        tag.value = Convert.ToUInt16(value);
                    }
                    else
                    {
                        // failed , but you still can know the failed detail

                    }
                }
                else
                {
                    OperateResult<bool>  data =  plc.ReadBool(tag.address);

                    if (data.IsSuccess)
                    {
                        // you get the right value
                        tag.value = data.Content;
                    }
                    else
                    {
                        // failed , but you still can know the failed detail

                    }
                }
                MqttTags = Tags.Select(e => new MqttTag(
                  e.name,
                  e.value,
                  e.timestamp)).ToList();
            }
        }

        public object? GetTagValue(string tagName)
        {
            return Tags.First(x => x.name == tagName).value;

        }

        public string GetTagAddress(string tagName)
        {
            return Tags.First(x => x.name == tagName).address;
        }

        public void WritePLC(string TagName, bool value)
        {
          
            OperateResult write1 =  plc.Write(TagName, value);
          
        }
        public void WriteNumberPLC(string TagName, UInt16 value)
        {
            OperateResult write =  plc.Write(TagName,value);      
        }

        public async void Connect()
        {
            await plc.ConnectServerAsync();
            _timer.Enabled = true;
            
        }
    }
}
