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
using System.Runtime.CompilerServices;

namespace DemoCaseGui.Core.Application.Communication
{
    public class Micro850Client
    {
        private readonly ABLogix.Net.Channel myChannel;
        private readonly ABLogix.Device myDevice;
        private readonly ABLogix.Group myGroup;
        private readonly ABLogix.Item myItem;
        private Stopwatch _sw;
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



            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed1;

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
            new("confirm_inverter", "PLC.Vali_Micro850.led7", null, "HMI_DB.Inverter.Confirm", DateTime.Now),
        };
        }



        private async void _timer_Elapsed1(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var tag in Tags)
            {
                _sw = new Stopwatch();
                // start transaction timing
                _sw.Start();

                myItem.HWTagName = tag.address;
                myItem.HWTagType = ABLogix.TagType.AUTO;
                await myItem.ReadAsync();
                _sw.Stop();
                string value = "";

                if (!myItem.Values[0].GetType().IsArray)
                {
                    value = string.Join(",", myItem.Values);
                }
                // For structured types (UDT, PDT, and System), each Item.Values element represents an array of bytes
                else
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < myItem.Elements; i++)
                    {
                        System.Collections.IList il = myItem.Values[i] as System.Collections.IList;
                        sb.Append("'" + string.Join(",", il) + "'");
                    }
                    value = sb.ToString();
                }

                tag.value = value;

                //switch (value)
                //{
                //    case "True":
                //        tag.value = true; break;
                //    case "False":
                //        tag.value = false; break;
                //    default:
                //        tag.value =value; break;

                //}

            }
        }
        public object GetData(string TagName)
        {
            //object value = null;
            myItem.HWTagName = TagName;
            
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

            //switch (tem)
            //{
            //    case "True":
            //        value = true; break;
            //    case "False":
            //        value = false; break;
            //    default:
            //        value = tem; break;

            //}

            return tem;


        }
        public object? GetTagValue(string tagName)
        {
            return Tags.First(x => x.name == tagName).value;

        }

        public string GetTagAddress(string tagName)
        {
            return Tags.First(x => x.name == tagName).address;
        }

        //public MqttTag GetTag(string tagName)
        //{
        //    return MqttTags.Find(x => x.name == tagName);

        //}

        public  void WritePLC(string TagName, object value)
        {
            _sw = new Stopwatch();
            // Start stopwatch
            _sw.Start();
            myItem.HWTagName = TagName;
            myItem.HWTagType = ABLogix.TagType.BOOL;
            //var valuesToWrite = value.ToString().Split(new char[] { ',' });
            //// Await async transaction. 
            //await myItem.WriteAsync(valuesToWrite);
           myItem.Write(value);
            _sw.Stop();
        }
        public void WriteNumberPLC(string TagName, object value)
        {
            _sw = new Stopwatch();
            // Start stopwatch
            _sw.Start();
            myItem.HWTagName = TagName;

            //var valuesToWrite = value.ToString().Split(new char[] { ',' });
            //// Await async transaction. 
            //await myItem.WriteAsync(valuesToWrite);
            myItem.Write(value);
            _sw.Stop();
        }

        public void Connect()
        {
            _timer.Enabled = true;
        }

    }
}
