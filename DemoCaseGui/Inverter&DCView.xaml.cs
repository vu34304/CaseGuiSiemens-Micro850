using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Events;
using LiveCharts.Wpf;

namespace DemoCaseGui
{
    /// <summary>
    /// Interaction logic for Inverter_DCView.xaml
    /// </summary>
    public partial class Inverter_DCView : UserControl
    {
        public Inverter_DCView()
        {
            InitializeComponent();
            DataContext = new Core.Application.ViewModels.CaseMicroViewModel();
        }
        
    }
}
