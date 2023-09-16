using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace DemoCaseGui.Core.Application.Communication
{
    public class CPLogixClient
    {
        private readonly AllenBradleyConnectedCipNet plc;
        private readonly Timer _timer;
        public List<Tag> Tags { get; private set; }
        public List<MqttTag> MqttTags { get; private set; }

        public CPLogixClient()
        {
            plc = new AllenBradleyConnectedCipNet("192.168.1.101");
            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed;
            Tags = new()
        {
            //IO
            new("i0.0", "PLC.Vali_CPLogix.i0.0", null, "_IO_EM_DO_00", DateTime.Now),
            new("i0.1", "PLC.Vali_CPLogix.i0.1", null, "_IO_EM_DO_01", DateTime.Now),
            new("i0.2", "PLC.Vali_CPLogix.i0.2", null, "_IO_EM_DO_02", DateTime.Now),
            new("i0.3", "PLC.Vali_CPLogix.i0.3", null, "_IO_EM_DO_03", DateTime.Now),

            new("q0.0", "PLC.Vali_CPLogix.q0.0", null, "_IO_EM_DI_00", DateTime.Now),
            new("q0.1", "PLC.Vali_CPLogix.q0.1", null, "_IO_EM_DI_01", DateTime.Now),
            new("q0.2", "PLC.Vali_CPLogix.q0.2", null, "_IO_EM_DI_02", DateTime.Now),
            new("q0.3", "PLC.Vali_CPLogix.q0.3", null, "_IO_EM_DI_03", DateTime.Now),


            //TrafficLights
            new("led_do1", "PLC.Vali_CPLogix.TrafficLights.led_do1", null, "DO1", DateTime.Now),
            new("led_do2", "PLC.Vali_CPLogix.TrafficLights.led_do2", null, "DO2", DateTime.Now),
            new("led_xanh1", "PLC.Vali_CPLogix.TrafficLights.led_xanh1", null, "XANH1", DateTime.Now),
            new("led_xanh2", "PLC.Vali_CPLogix.TrafficLights.led_xanh2", null, "XANH2", DateTime.Now),
            new("led_vang1", "PLC.Vali_CPLogix.TrafficLights.led_vang1", null, "VANG1", DateTime.Now),
            new("led_vang2", "PLC.Vali_CPLogix.TrafficLights.led_vang2", null, "VANG2", DateTime.Now),

            //new("set_do1", "PLC.Vali_Micro850.led7", null, "SET_D1", DateTime.Now),
            //new("set_xanh1", "PLC.Vali_Micro850.led7", null, "SET_X1", DateTime.Now),
            //new("set_vang1", "PLC.Vali_Micro850.led7", null, "SET_V1", DateTime.Now),

            //AUTO MODE
            new("time_do1_auto", "PLC.Vali_CPLogix.TrafficLights.time_do1_auto", null, "D1_HIEN_AUTO", DateTime.Now),
            new("time_do2_auto", "PLC.Vali_CPLogix.TrafficLights.time_do2_auto", null, "D2_HIEN_AUTO", DateTime.Now),
            new("time_xanh1_auto", "PLC.Vali_CPLogix.TrafficLights.time_xanh1_auto", null, "X1_HIEN_AUTO", DateTime.Now),
            new("time_xanh2_auto", "PLC.Vali_CPLogix.TrafficLights.time_xanh2_auto", null, "X2_HIEN_AUTO", DateTime.Now),
            new("time_vang1_auto", "PLC.Vali_CPLogix.TrafficLights.time_vang1_auto", null, "V1_HIEN_AUTO", DateTime.Now),
            new("time_vang2_auto", "PLC.Vali_CPLogix.TrafficLights.time_vang2_auto", null, "V2_HIEN_AUTO", DateTime.Now),

            new("start_auto", "PLC.Vali_CPLogix.TrafficLights.start_auto", null, "START_AUTO_WEB", DateTime.Now),
            new("start_manual", "PLC.Vali_CPLogix.TrafficLights.start_manual", null, "START_MANUAL_WEB", DateTime.Now),
            new("stop_auto", "PLC.Vali_CPLogix.TrafficLights.stop_auto", null, "STOP_AUTO_WEB", DateTime.Now),
            new("stop_manual", "PLC.Vali_CPLogix.TrafficLights.stop_manual", null, "STOP_MANUAL_WEB", DateTime.Now),

            //MANUAL MODE
            new("time_do1_manual", "PLC.Vali_CPLogix.TrafficLights.time_do1_manual", null, "D1_HIEN", DateTime.Now),
            new("time_do2_manual", "PLC.Vali_CPLogix.TrafficLights.time_do2_manual", null, "D2_HIEN", DateTime.Now),
            new("time_xanh1_manual", "PLC.Vali_CPLogix.TrafficLights.time_xanh1_manual", null, "X1_HIEN", DateTime.Now),
            new("time_xanh2_manual", "PLC.Vali_CPLogix.TrafficLights.time_xanh2_manual", null, "X2_HIEN", DateTime.Now),
            new("time_vang1_manual", "PLC.Vali_CPLogix.TrafficLights.time_vang1_manual", null, "V1_HIEN", DateTime.Now),
            new("time_vang2_manual", "PLC.Vali_CPLogix.TrafficLights.time_vang2_manual", null, "V2_HIEN", DateTime.Now),

            //SENSOR
            new("ugt_524", "PLC.Vali_CPLogix.Sensor.ugt_524_device", null, "UGT_524_ALARM.PV_DEVICE", DateTime.Now),
            new("ki6000", "PLC.Vali_CPLogix.Sensor.ki6000_device", null, "KI6000_ALARM.PV_DEVICE", DateTime.Now),
            new("05d_150", "PLC.Vali_CPLogix.Sensor.o5d_150_device", null, "O5D_150_ALARM.PV_DEVICE", DateTime.Now),
            new("rpv_510", "PLC.Vali_CPLogix.Sensor.rpv_510_device", null, "RPV_510_ALARM.PV_DEVICE", DateTime.Now),

            //INVERTER
            new("setpoint", "PLC.Vali_CPLogix.Inverter.setpoint", null, "BIEUDOHMI", DateTime.Now),
             new("start_inverter", "PLC.Vali_CPLogix.TrafficLights.start_auto", null, "START_INVERTER_WEB", DateTime.Now),
            new("stop_inverter", "PLC.Vali_CPLogix.TrafficLights.start_manual", null, "STOP_INVERTER_WEB", DateTime.Now),

            //Lights IFM
            new("den_do_ifm", "PLC.Vali_Micro850.yellowtime", null, "IFM_AL1322:O1.DATA[23].9", DateTime.Now),
            new("den_xanh_ifm", "PLC.Vali_Micro850.yellowtime", null, "IFM_AL1322:O1.DATA[23].1", DateTime.Now),
            new("den_vang_ifm", "PLC.Vali_Micro850.yellowtime", null, "IFM_AL1322:O1.DATA[24].9", DateTime.Now),

        };
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var tag in Tags)
            {

                if (tag.name is "ugt_524" or "ki6000" or "05d_150" or "rpv_510" or "time_vang2_manual"
                    or "time_vang1_manual" or "time_xanh2_manual" or "time_xanh1_manual" or "time_do2_manual"
                    or "time_do1_manual" or "time_vang2_auto" or "time_vang1_auto" or "time_xanh2_auto"
                    or "time_xanh1_auto" or "time_do2_auto" or "time_do1_auto")
                {
                    OperateResult<UInt16> data = plc.ReadUInt16(tag.address);

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

                else
                {
                    OperateResult<bool> data = plc.ReadBool(tag.address);

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

            OperateResult write1 = plc.Write(TagName, value);

        }
        public void WriteNumberPLC(string TagName, UInt16 value)
        {
            OperateResult write = plc.Write(TagName, value);
        }

        public async void Connect()
        {
            await plc.ConnectServerAsync();
            _timer.Enabled = true;

        }
    }
}
