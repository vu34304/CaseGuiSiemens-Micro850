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
        private readonly ValiIfmLogRepository valiIfmLogRepository;
        private readonly InverterLogRepository inverterLogRepository;
        private readonly ValiSiemensLogRepository valiSiemensLogRepository;
        private readonly IExcelExporter _excelExporter;

        public ObservableCollection<FilterEntry> Entries { get; set; } = new();
        public TimeRangeQuery TimeRange { get; set; } = new();
        private string tagname = "";
        public string Tagname 
        { 
            get { return tagname; } 
            set 
            { 
                tagname = value; 
                var tag = _s7Client.Tags.First(i => i.dbname == tagname);
            }
        }
        public ObservableCollection<string> Tagnames { get; set; } = new();

        public ICommand FilterCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public FilterViewModel()
        {
            _s7Client = new S7Client();
            _excelExporter = new ExcelExporter();
            valiIfmLogRepository = new ValiIfmLogRepository();
            inverterLogRepository = new InverterLogRepository();
            valiSiemensLogRepository = new ValiSiemensLogRepository();  

            Tagnames = new ObservableCollection<string>(_s7Client.Tags.Select(i => i.dbname).OrderBy(s => s));
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
