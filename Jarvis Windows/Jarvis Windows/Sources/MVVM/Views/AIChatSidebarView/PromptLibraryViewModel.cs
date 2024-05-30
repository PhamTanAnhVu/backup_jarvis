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
    private bool _isShowEditPromptPopup;
    private bool _isEmptyFeedbackInput;
    private bool _isEmptyPromptNameInput;
    private bool _isEmptyPromptDescriptionInput;
    private bool _isEmptyPromptContentInput;
    private string _feedbackInputMessage;
    private string _reportName;
    private string _publicPromptLanguage;
    private string _publicPromptCategory;
    private string _promptNameInputMessage;
    private string _promptDescriptionInputMessage;
    private string _promptContentInputMessage;

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
    public bool IsShowEditPromptPopup
    {
        get { return _isShowEditPromptPopup; }
        set
        {
            _isShowEditPromptPopup = value;
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

    public bool IsEmptyPromptNameInput
    {
        get
        {
            if (string.IsNullOrWhiteSpace(PromptNameInputMessage)) _isEmptyPromptNameInput = true;
            else _isEmptyPromptNameInput = false;
            return _isEmptyPromptNameInput;
        }
        set
        {
            _isEmptyPromptNameInput = value;
            OnPropertyChanged();
        }
    }

    public bool IsEmptyPromptDescriptionInput
    {
        get
        {
            if (string.IsNullOrWhiteSpace(PromptDescriptionInputMessage)) _isEmptyPromptDescriptionInput = true;
            else _isEmptyPromptDescriptionInput = false;
            return _isEmptyPromptDescriptionInput;
        }
        set
        {
            _isEmptyPromptDescriptionInput = value;
            OnPropertyChanged();
        }
    }

    public bool IsEmptyPromptContentInput
    {
        get
        {
            if (string.IsNullOrWhiteSpace(PromptContentInputMessage)) _isEmptyPromptContentInput = true;
            else _isEmptyPromptContentInput = false;
            return _isEmptyPromptContentInput;
        }
        set
        {
            _isEmptyPromptContentInput = value;
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
    public string PromptNameInputMessage
    {
        get { return _promptNameInputMessage; }
        set
        {
            _promptNameInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyPromptNameInput));
        }
    }
    public string PromptDescriptionInputMessage
    {
        get { return _promptDescriptionInputMessage; }
        set
        {
            _promptDescriptionInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyPromptDescriptionInput));
        }
    }

    public string PromptContentInputMessage
    {
        get { return _promptContentInputMessage; }
        set
        {
            _promptContentInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyPromptContentInput));
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

    public string PublicPromptLanguage
    {
        get { return _publicPromptLanguage; }
        set
        {
            _publicPromptLanguage = value;
            OnPropertyChanged();
        }
    }

    public string PublicPromptCategory
    {
        get { return _publicPromptCategory; }
        set
        {
            _publicPromptCategory = value;
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
    public RelayCommand CloseEditPromptPopupCommand { get; set; }
    public RelayCommand SaveEditPromptCommand { get; set; }

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
        CloseEditPromptPopupCommand = new RelayCommand(o => { IsShowEditPromptPopup = false; }, o => true);
        SaveEditPromptCommand = new RelayCommand(ExecuteSaveEditPromptCommand, o => true);
        SelectReportCommand = new RelayCommand(o => { IsShowSelectReportPopup = !IsShowSelectReportPopup; }, o => true);
        SendReportCommand = new RelayCommand(ExecuteSendReportCommand, o => true);
        UsePromptCommand = new RelayCommand(ExecuteUsePromptCommand, o => true);
        CopyPromptCommand = new RelayCommand(ExecuteCopyPromptCommand, o => true);

        IsShowPromptDetailPopup = true;
        IsShowEditPromptPopup = true;
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

    private async void ExecuteSaveEditPromptCommand(object obj)
    {
        IsShowEditPromptPopup = false;

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
