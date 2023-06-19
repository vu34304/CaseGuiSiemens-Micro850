using CommunityToolkit.Mvvm.Input;
using DemoCaseGui.Core.Application.Communication;
using System.Windows.Input;
namespace DemoCaseGui.Core.Application.ViewModels;
public class MainViewModel : BaseViewModel
{
    public CaseViewModel CaseViewModel { get; set; }
    public FilterViewModel FilterViewModel { get; set; }
    public MainViewModel(CaseViewModel caseViewModel, FilterViewModel filterViewModel)
    {
        CaseViewModel = caseViewModel;
        FilterViewModel = filterViewModel;
    }
}
