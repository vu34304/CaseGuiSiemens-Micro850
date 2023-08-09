using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedSolutions.ASCommStd;
using ABLogix = AutomatedSolutions.ASCommStd.AB.Logix;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using NPOI.SS.Formula.Functions;
using S7.Net;
using NPOI.XSSF.Streaming.Values;

namespace DemoCaseGui.Core.Application.Communication
{
    public class Micro850Client
    {
        private readonly ABLogix.Net.Channel myChannel;
        private readonly ABLogix.Device myDevice;
        private readonly ABLogix.Group myGroup;
        private readonly ABLogix.Item myItem;
        private readonly Timer _timer;
        public List<Tag> Tags { get; private set; }
        public List<MqttTag> MqttTags { get; private set; }

        public Micro850Client()
        {
            //Khoi tao PLC
            myChannel = new ABLogix.Net.Channel();           
            myDevice = new ABLogix.Device("192.168.1.50");
            myGroup = new ABLogix.Group(false, 100);
            myItem = new ABLogix.Item();
            myDevice.Model = ABLogix.Model.Micro800;
            myChannel.Devices.Add(myDevice);
            myDevice.Groups.Add(myGroup);
            myGroup.Items.Add(myItem);
            myDevice.TimeoutTransaction = 250;
            myDevice.TimeoutConnect = 2500;
        
           

            _timer = new Timer(300);
            _timer.Elapsed += _timer_Elapsed1;           
            Tags = new()
        {
            
            //TrafficLights
            new("led2", "PLC.Vali_Siemens.led2", null, "_IO_EM_DO_02", DateTime.Now),
            new("led3", "PLC.Vali_Siemens.led3", null, "_IO_EM_DO_03", DateTime.Now),
            new("led4", "PLC.Vali_Siemens.led4", null, "_IO_EM_DO_04", DateTime.Now),
            new("led5", "PLC.Vali_Siemens.led5", null, "_IO_EM_DO_05", DateTime.Now),
            new("led6", "PLC.Vali_Siemens.led6", null, "_IO_EM_DO_06", DateTime.Now),
            new("led7", "PLC.Vali_Siemens.led7", null, "_IO_EM_DO_07", DateTime.Now),

            new("edit_redled", "PLC.Vali_Siemens.led7", null, "_IO_EM_DO_07", DateTime.Now),
            new("edit_yellowled", "PLC.Vali_Siemens.led7", null, "_IO_EM_DO_07", DateTime.Now),
            new("edit_greenled", "PLC.Vali_Siemens.led7", null, "_IO_EM_DO_07", DateTime.Now),

            new("start_trafficlight", "PLC.Vali_Siemens.led7", null, "_IO_EM_DO_07", DateTime.Now),
            new("stop_trafficlight", "PLC.Vali_Siemens.led7", null, "_IO_EM_DO_07", DateTime.Now),

            //Inverter
            new("start", "PLC.Vali_Siemens.led7", null, "HMI_DB.Inverter.Start", DateTime.Now),
            new("stop", "PLC.Vali_Siemens.led7", null, "HMI_DB.Inverter.Stop", DateTime.Now),
            new("setpoint", "PLC.Inverter.setpoint", null, "HMI_DB.Inverter.Speed_SP", DateTime.Now),
            new("speed", "PLC.Inverter.speed", null, "HMI_DB.Inverter.Speed_PV", DateTime.Now),
            new("forward", "PLC.Inverter.setpoint", null, "HMI_DB.Inverter.Fwd", DateTime.Now),
            new("reverse", "PLC.Inverter.speed", null, "HMI_DB.Inverter.Rev", DateTime.Now),
        };
        }

       

        private void _timer_Elapsed1(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var tag in Tags)
            {
                object value = GetData(tag.address);
                if (tag.name is "angleRB3100" or "tempTW2000" or "current_speed_M" or "current_position_M")
                {
                    tag.value = Convert.ToUInt32(value);
                }
                else
                {
                    tag.value = value;
                }
                MqttTags = Tags.Select(e => new MqttTag(
                        e.name,
                        e.value,
                        e.timestamp)).ToList();
            }
        }          
        public object GetData(string TagName)
        {

            myItem.HWTagName = TagName;
            object value;

            myItem.Read();

            StringBuilder sb = new StringBuilder();
            // For atomic types, each Item.Values element represents one atomic value.
            if (!myItem.Values[0].GetType().IsArray)
            {
                for (int i = 0; i < myItem.Elements; i++)
                {
                    sb.Append(myItem.Values[i].ToString() + ",");
                }
            }
            // For structured types (UDT, PDT, and System), each Item.Values element represents an array of bytes
            else
            {
                for (int i = 0; i < myItem.Elements; i++)
                {
                    IList il = myItem.Values[i] as IList;
                    for (int j = 0; j < il.Count; j++)
                        sb.Append(il[j].ToString() + ",");
                }
            }
            // Get rid of trailing comma
            sb.Remove(sb.Length - 1, 1);
            // Show data
            
            string tem = sb.ToString();
            switch (tem)
            {
                case "True":
                    value =true; break;
                case "False":
                    value =false; break;
                default:
                    value = double.Parse(tem); break;
            }  
            return value;
        }        
        public object? GetTagValue(string tagName)
        {
            return Tags.First(x => x.name == tagName).value;
        }

        public string GetTagAddress(string tagName)
        {
            return Tags.First(x => x.name == tagName).address;
        }

        public MqttTag GetTag(string tagName)
        {
            return MqttTags.Find(x => x.name == tagName);

        }

        public  void WritePLC(string TagName, object value)
        {
            myItem.HWTagName = TagName;
            myItem.Write(value);
        }

        public void Connect()
        {
           _timer.Enabled= true;
        }
    }
}
