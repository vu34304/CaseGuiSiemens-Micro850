using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Timer = System.Timers.Timer;


namespace DemoCaseGui.Core.Application.ViewModels
{
    public class CaseCompactLogixViewModel: BaseViewModel
    {
        private readonly CPLogixClient _CPLogixClient;
        private readonly Timer _timer;

        //IO
        public bool? I0_0 { get; set; }
        public bool? I0_1 { get; set; }
        public bool? I0_2 { get; set; }
        public bool? I0_3 { get; set; }

        public bool? Q0_0 { get; set; }
        public bool? Q0_1 { get; set; }
        public bool? Q0_2 { get; set; }
        public bool? Q0_3 { get; set; }

        public bool? i0_0, i0_1, i0_2,i0_3,q0_0,q0_1,q0_2,q0_3;

        //Traffic Lights
        public bool? DO1 { get;set; }
        public bool? DO2 { get; set; }
        public bool? XANH1 { get; set; }
        public bool? XANH2 { get; set; }
        public bool? VANG1 { get; set; }
        public bool? VANG2 { get; set; }

        public bool? do1,do2,xanh1,xanh2,vang1,vang2;

        //AUTO MODE
        public ushort? TIME_DO1_AUTO { get; set; }
        public ushort? TIME_DO2_AUTO { get; set; }
        public ushort? TIME_XANH1_AUTO { get; set; }
        public ushort? TIME_XANH2_AUTO { get; set; }
        public ushort? TIME_VANG1_AUTO { get; set; }
        public ushort? TIME_VANG2_AUTO { get; set; }

        public ushort? time_do1_auto,time_do2_auto,time_xanh1_auto,time_xanh2_auto, time_vang1_auto,time_vang2_auto;    

        //MANUAL MODE
        public ushort? TIME_DO1_MANUAL { get; set; }
        public ushort? TIME_DO2_MANUAL { get; set; }
        public ushort? TIME_XANH1_MANUAL { get; set; }
        public ushort? TIME_XANH2_MANUAL { get; set; }
        public ushort? TIME_VANG1_MANUAL { get; set; }
        public ushort? TIME_VANG2_MANUAL { get; set; }

        public ushort? time_do1_manual, time_do2_manual, time_xanh1_manual, time_xanh2_manual, time_vang1_manual, time_vang2_manual;

        //SENSOR
        public ushort? DEVICE_UGT_524 { get;set; }
        public ushort? DEVICE_KI6000 { get; set; }
        public ushort? DEVICE_O5D_150{ get; set; }
        public ushort? DEVICE_RPV_510 { get; set; }

        public ushort? device_ugt_524, device_ki6000, device_o5d_150, device_rpv_510;

        //Lights IFM
        public bool? DEN_DO_IFM { get; set; }
        public bool? DEN_VANG_IFM { get; set; }
        public bool? DEN_XANH_IFM { get; set; }

        public bool? den_do_ifm, den_vang_ifm, den_xanh_ifm;


        public ICommand ConnectCommand { get; set; }
        public ICommand Start_Auto_Command { get; set; }
        public ICommand Stop_Auto_Command { get; set; }
        public ICommand Start_Manual_Command { get; set; }
        public ICommand Stop_Manual_Command { get; set; }
        public ICommand Start_Inverter_Command { get; set; }
        public ICommand Stop_Inverter_Command { get; set; }

        public CaseCompactLogixViewModel()
        {
            _CPLogixClient = new CPLogixClient();
            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed;

            ConnectCommand = new RelayCommand(Connect);
            Start_Auto_Command = new RelayCommand(Auto_Start);
            Stop_Auto_Command =(new RelayCommand(Auto_Stop));
            Start_Manual_Command =(new RelayCommand(Manual_Start));
            Stop_Manual_Command = new RelayCommand(Manual_Stop);
            Start_Inverter_Command = new RelayCommand(Inverter_Start);
            Stop_Inverter_Command = new RelayCommand (Inverter_Stop);

        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            //IO
            if ((bool?)_CPLogixClient.GetTagValue("q0.0") != q0_0)
            {
                Q0_0 = (bool?)_CPLogixClient.GetTagValue("q0.0");
            }
            q0_0 = (bool?)_CPLogixClient.GetTagValue("i0.0");

            if ((bool?)_CPLogixClient.GetTagValue("q0.1") != q0_1)
            {
                Q0_1 = (bool?)_CPLogixClient.GetTagValue("q0.1");
            }
            q0_1 = (bool?)_CPLogixClient.GetTagValue("q0.1");

            if ((bool?)_CPLogixClient.GetTagValue("q0.2") != q0_2)
            {
                Q0_2 = (bool?)_CPLogixClient.GetTagValue("i0.2");
            }
            q0_2 = (bool?)_CPLogixClient.GetTagValue("q0.2");

            if ((bool?)_CPLogixClient.GetTagValue("q0.3") != q0_3)
            {
                Q0_3 = (bool?)_CPLogixClient.GetTagValue("q0.3");
            }
            q0_3 = (bool?)_CPLogixClient.GetTagValue("q0.3");

            I0_0 = (bool?)_CPLogixClient.GetTagValue("i0.0");
            I0_1 = (bool?)_CPLogixClient.GetTagValue("i0.1");
            I0_2 = (bool?)_CPLogixClient.GetTagValue("i0.2");
            I0_3 = (bool?)_CPLogixClient.GetTagValue("i0.3");

            //TrafficLights
            if ((bool?)_CPLogixClient.GetTagValue("led_do1") != do1)
            {
                DO1 = (bool?)_CPLogixClient.GetTagValue("led_do1");
            }
            do1 = (bool?)_CPLogixClient.GetTagValue("led_do1");

            if ((bool?)_CPLogixClient.GetTagValue("led_do2") != do2)
            {
                DO2 = (bool?)_CPLogixClient.GetTagValue("led_do2");
            }
            do2 = (bool?)_CPLogixClient.GetTagValue("led_do2");

            if ((bool?)_CPLogixClient.GetTagValue("led_xanh1") != xanh1)
            {
                XANH1 = (bool?)_CPLogixClient.GetTagValue("led_xanh1");
            }
            xanh1 = (bool?)_CPLogixClient.GetTagValue("led_xanh1");

            if ((bool?)_CPLogixClient.GetTagValue("led_xanh2") != xanh2)
            {
                XANH2 = (bool?)_CPLogixClient.GetTagValue("led_xanh2");
            }
            xanh2 = (bool?)_CPLogixClient.GetTagValue("led_xanh2");

            if ((bool?)_CPLogixClient.GetTagValue("led_vang1") != vang1)
            {
                VANG1 = (bool?)_CPLogixClient.GetTagValue("led_vang1");
            }
            vang1 = (bool?)_CPLogixClient.GetTagValue("led_vang1");

            if ((bool?)_CPLogixClient.GetTagValue("led_vang2") != vang2)
            {
                VANG2 = (bool?)_CPLogixClient.GetTagValue("led_vang2");
            }
            vang2 = (bool?)_CPLogixClient.GetTagValue("led_vang2");


            //SENSOR
            if ((ushort?)_CPLogixClient.GetTagValue("ugt_524") != device_ugt_524)
            {
                DEVICE_UGT_524 = (ushort?)_CPLogixClient.GetTagValue("ugt_524");
            }
            device_ugt_524 = (ushort?)_CPLogixClient.GetTagValue("ugt_524");

            if ((ushort?)_CPLogixClient.GetTagValue("ki6000") != device_ki6000)
            {
                DEVICE_KI6000 = (ushort?)_CPLogixClient.GetTagValue("ki6000");
            }
            device_ki6000 = (ushort?)_CPLogixClient.GetTagValue("ki6000");

            if ((ushort?)_CPLogixClient.GetTagValue("05d_150") != device_o5d_150)
            {
                DEVICE_O5D_150 = (ushort?)_CPLogixClient.GetTagValue("05d_150");
            }
            device_o5d_150 = (ushort?)_CPLogixClient.GetTagValue("05d_150");

            if ((ushort?)_CPLogixClient.GetTagValue("rpv_510") != device_rpv_510)
            {
                DEVICE_RPV_510 = (ushort?)_CPLogixClient.GetTagValue("rpv_510");
            }
            device_rpv_510 = (ushort?)_CPLogixClient.GetTagValue("rpv_510");

            //Lights IFM
            if ((bool?)_CPLogixClient.GetTagValue("den_do_ifm") != den_do_ifm)
            {
                DEN_DO_IFM = (bool?)_CPLogixClient.GetTagValue("den_do_ifm");
            }
            den_do_ifm = (bool?)_CPLogixClient.GetTagValue("den_do_ifm");

            if ((bool?)_CPLogixClient.GetTagValue("den_xanh_ifm") != den_xanh_ifm)
            {
                DEN_XANH_IFM = (bool?)_CPLogixClient.GetTagValue("den_xanh_ifm");
            }
            den_xanh_ifm = (bool?)_CPLogixClient.GetTagValue("den_xanh_ifm");

            if ((bool?)_CPLogixClient.GetTagValue("den_vang_ifm") != den_vang_ifm)
            {
               DEN_VANG_IFM = (bool?)_CPLogixClient.GetTagValue("den_vang_ifm");
            }
            den_vang_ifm = (bool?)_CPLogixClient.GetTagValue("den_vang_ifm");

        }

        public void Connect()
        {
            _CPLogixClient.Connect();
            _timer.Enabled = true;
        }

        public void Auto_Start()
        {
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("START_AUTO_WEB"), true);
            Thread.Sleep(1000);
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("START_AUTO_WEB"), false);
        }

        public void Auto_Stop()
        {
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("STOP_AUTO_WEB"), true);
            Thread.Sleep(1000);
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("STOP_AUTO_WEB"), false);
        }

        public void Manual_Start()
        {
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("START_MANUAL_WEB"), true);
            Thread.Sleep(1000);
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("START_MANUAL_WEB"), false);
        }

        public void Manual_Stop()
        {
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("STOP_MANUAL_WEB"), true);
            Thread.Sleep(1000);
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("STOP_MANUAL_WEB"), false);
        }


        public void Inverter_Start()
        {
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("START_INVERTER_WEB"), true);
            Thread.Sleep(1000);
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("START_INVERTER_WEB"), false);
        }

        public void Inverter_Stop()
        {
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("STOP_INVERTER_WEB"), true);
            Thread.Sleep(1000);
            _CPLogixClient.WritePLC(_CPLogixClient.GetTagAddress("STOP_INVERTER_WEB"), false);
        }


    }
}
