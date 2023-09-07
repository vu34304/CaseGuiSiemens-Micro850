using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using DemoCaseGui.Core.Application.Models;
using DemoCaseGui.Core.Application.Persistence.Queries;
using DemoCaseGui.Core.Application.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DemoCaseGui.Core.Application.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        private readonly S7Client _s7Client;
        private readonly M850Client _m850Client;
        private readonly CPLogixClient _CPLogixClient;
        private readonly ValiIfmLogRepository valiIfmLogRepository;
        private readonly InverterLogRepository inverterLogRepository;
        private readonly ValiSiemensLogRepository valiSiemensLogRepository;
        private readonly ValiMicroLogRepository valiMicroLogRepository;
        private readonly ValiCompactLogRepository valiCompactLogRepository;
        private readonly IExcelExporter _excelExporter;

        public ObservableCollection<FilterEntry> Entries { get; set; } = new();
        public TimeRangeQuery TimeRange { get; set; } = new();
        private string tagname = "";
        private string tagname1 = "";
        private string tagname2 = "";
        public string Tagname 
        { 
            get { return tagname; } 
            set 
            { 
                tagname = value; 
                var tag = _s7Client.Tags.First(i => i.dbname == tagname);
            }
        }

        public string Tagname1
        {
            get { return tagname1; }
            set
            {
                tagname1 = value;
                var tag = _m850Client.Tags.First(i => i.dbname == tagname1);
            }
        }

        public string Tagname2
        {
            get { return tagname2; }
            set
            {
                tagname2 = value;
                var tag = _CPLogixClient.Tags.First(i => i.dbname == tagname2);
            }
        }

        public ObservableCollection<string> Tagnames { get; set; } = new();

     

        public ICommand FilterCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public FilterViewModel()
        {
            _s7Client = new S7Client();
            _m850Client = new M850Client();
            _CPLogixClient = new CPLogixClient();
            _excelExporter = new ExcelExporter();
            valiIfmLogRepository = new ValiIfmLogRepository();
            inverterLogRepository = new InverterLogRepository();
            valiSiemensLogRepository = new ValiSiemensLogRepository();
            valiMicroLogRepository = new ValiMicroLogRepository();
            valiCompactLogRepository = new ValiCompactLogRepository();
            


            Tagnames = new ObservableCollection<string>(_s7Client.Tags.Select(i => i.dbname).Concat(_m850Client.Tags.Select(i => i.dbname)).Concat(_CPLogixClient.Tags.Select(i => i.dbname)).OrderBy(s => s));

            FilterCommand = new RelayCommand(LoadAsync);
            ExportToExcelCommand = new RelayCommand<string>(ExportToExcel);
        }
        private async void LoadAsync()
        {
            try
            {
                var valiIfmLog = await valiIfmLogRepository.GetListAsync(TimeRange,Tagname);
                var inverterLog = await inverterLogRepository.GetListAsync(TimeRange,Tagname);
                var valiSiemensLog = await valiSiemensLogRepository.GetListAsync(TimeRange, Tagname);
                var valiMicroLog = await valiMicroLogRepository.GetListAsync(TimeRange, Tagname1);
                var valiCompactLog = await valiCompactLogRepository.GetListAsync(TimeRange, Tagname2);

                var entriesvaliIfmLog = valiIfmLog.Select(e => new FilterEntry(
                    e.Name, 
                    e.Timestamp, 
                    e.Value)).ToList();


                var entriesinverterLog = inverterLog.Select(e => new FilterEntry(
                    e.Name,
                    e.Timestamp,
                    e.Value)).ToList();

                var entriesvaliSiemensLog = valiSiemensLog.Select(e => new FilterEntry(
                    e.Name,
                    e.Timestamp,
                    e.Value)).ToList();
                var entriesvaliMicroLog = valiMicroLog.Select(e => new FilterEntry(
                   e.Name,
                   e.Timestamp,
                   e.Value)).ToList();

                var entriesvaliCompactLog = valiCompactLog.Select(e => new FilterEntry(
                  e.Name,
                  e.Timestamp,
                  e.Value)).ToList();

                List<FilterEntry> filters = new();
                foreach (var entry in entriesvaliIfmLog )
                {
                    filters.Add(entry);
                }
                foreach (var entry in entriesinverterLog)
                {
                    filters.Add(entry);
                }
                foreach (var entry in entriesvaliSiemensLog)
                {
                    filters.Add(entry);
                }
                foreach (var entry in entriesvaliMicroLog)
                {
                    filters.Add(entry);
                }
                foreach (var entry in entriesvaliCompactLog)
                {
                    filters.Add(entry);
                }

                Entries = new(filters);
            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }
        
        private void ExportToExcel(string? filePath)
        {
            if (filePath is not null)
            {
                _excelExporter.ExportReport(filePath, Entries);
            }
        }
    }
}
