using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public class PromptLibraryViewModel : ViewModelBase
{
    private int _selectedPromptIdx;
    private bool _isShowPromptDetailPopup;
    private bool _isShowReportPopup;
    private bool _isShowSelectReportPopup;
    private bool _isEmptyFeedbackInput;
    private string _feedbackInputMessage;
    private string _reportName;
    private ObservableCollection<ReportButtonModel> _reportButtons;

    public int SelectedPromptIdx
    {
        get { return _selectedPromptIdx; }
        set
        {
            _selectedPromptIdx = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowPromptDetailPopup
    {
        get { return _isShowPromptDetailPopup; }
        set
        {
            _isShowPromptDetailPopup = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowReportPopup
    {
        get { return _isShowReportPopup; }
        set
        {
            _isShowReportPopup = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowSelectReportPopup
    {
        get { return _isShowSelectReportPopup; }
        set
        {
            _isShowSelectReportPopup = value;
            OnPropertyChanged();
        }
    }

    public bool IsEmptyFeedbackInput
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FeedbackInputMessage)) _isEmptyFeedbackInput = true;
            else _isEmptyFeedbackInput = false;
            return _isEmptyFeedbackInput;
        }
        set
        {
            _isEmptyFeedbackInput = value;
            OnPropertyChanged();
        }
    }

    public string FeedbackInputMessage
    {
        get { return _feedbackInputMessage; }
        set
        {
            _feedbackInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyFeedbackInput));
        }
    }
    public string ReportName
    {
        get { return _reportName; }
        set
        {
            _reportName = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand UsePromptCommand { get; set; }
    public RelayCommand ClosePromptDetailCommand { get; set; }
    public RelayCommand OpenReportCommand { get; set; }
    public RelayCommand CloseReportPopupCommand { get; set; }
    public RelayCommand SelectReportCommand { get; set; }
    public RelayCommand SendReportCommand { get; set; }
    public RelayCommand CopyPromptCommand { get; set; }

    public ObservableCollection<ReportButtonModel> ReportButtons
    {
        get { return _reportButtons; }
        set
        {
            _reportButtons = value;
            OnPropertyChanged();
        }
    }

    public PromptLibraryViewModel()
    {
        ClosePromptDetailCommand = new RelayCommand(o => { IsShowPromptDetailPopup = false; }, o => true);
        OpenReportCommand = new RelayCommand(o => { IsShowReportPopup = true; }, o => true);
        CloseReportPopupCommand = new RelayCommand(o => { IsShowReportPopup = false; }, o => true);
        SelectReportCommand = new RelayCommand(o => { IsShowSelectReportPopup = !IsShowSelectReportPopup; }, o => true);
        SendReportCommand = new RelayCommand(ExecuteSendReportCommand, o => true);
        UsePromptCommand = new RelayCommand(ExecuteUsePromptCommand, o => true);
        CopyPromptCommand = new RelayCommand(ExecuteCopyPromptCommand, o => true);

        IsShowPromptDetailPopup = true;
        InitSelectReportList();
    }

    private void InitSelectReportList()
    {
        ReportButtons = new ObservableCollection<ReportButtonModel>();
        string[] reportNameList = [
            "Legal Concerns", "Result in wrong language", "Result on wrong topic/keywords", "Prompt not working as expected", "Spam"
        ];
        
        for (int idx = 0; idx < 5; idx++)
        {
            ReportButtons.Add(new ReportButtonModel
            {
                Idx = idx,
                ReportName = reportNameList[idx],
                Background = (idx != 0) ? "Transparent" : "#E0EFFE",
                SelectReportCommand = new RelayCommand(ExecuteSelectReportCommand, o => true),
            });
        }

        ReportName = ReportButtons[0].ReportName;
    }

    private async void ExecuteUsePromptCommand(object obj)
    {
        IsShowPromptDetailPopup = false;

    }

    private async void ExecuteSendReportCommand(object obj)
    {
        IsShowReportPopup = false;

    }
    
    private async void ExecuteCopyPromptCommand(object obj)
    {


    }

    private async void ExecuteSelectReportCommand(object obj)
    {
        int idx = (int)obj;
        foreach (var reportButton in ReportButtons)
        {
            reportButton.Background = "Transparent";
        }

        ReportName = ReportButtons[idx].ReportName;
        ReportButtons[idx].Background = "#E0EFFE";
        IsShowSelectReportPopup = false;
    }
}
