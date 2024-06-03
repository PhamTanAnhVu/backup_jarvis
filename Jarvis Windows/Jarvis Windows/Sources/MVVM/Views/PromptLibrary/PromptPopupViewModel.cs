using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Jarvis_Windows.Sources.MVVM.Views.PromptLibrary;

public class PromptPopupViewModel : ViewModelBase
{
    #region Fields
    private int _selectedPromptIdx;
    private bool _isShowPromptDetailPopup;
    private bool _isShowReportPopup;
    private bool _isShowSelectReportPopup;
    private bool _isShowEditPromptPopup;
    private bool _isEmptyFeedbackInput;
    private bool _isEmptyPromptNameInput;
    private bool _isEmptyPromptDescriptionInput;
    private bool _isEmptyPromptContentInput;
    private bool _isPrivatePrompt;
    private bool _isPublicPrompt;
    private bool _isShowLanguagePopup;
    private bool _isShowCategoryPopup;
    private string _feedbackInputMessage;
    private string _reportName;
    private string _languageName;
    private string _categoryName;
    private string _publicPromptLanguage;
    private string _publicPromptCategory;
    private string _promptNameInputMessage;
    private string _promptDescriptionInputMessage;
    private string _promptContentInputMessage;
    private string _promptHeader;
    private ObservableCollection<PromptPopupItem> _reportButtons;
    private ObservableCollection<PromptPopupItem> _languageButtons;
    private ObservableCollection<PromptPopupItem> _categoryButtons;
    private ObservableCollection<PromptBracketItem> _bracketItems;
    #endregion

    #region Properties
    public int SelectedPromptIdx
    {
        get => _selectedPromptIdx;
        set
        {
            _selectedPromptIdx = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowPromptDetailPopup
    {
        get => _isShowPromptDetailPopup;
        set
        {
            _isShowPromptDetailPopup = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowReportPopup
    {
        get => _isShowReportPopup;
        set
        {
            _isShowReportPopup = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowSelectReportPopup
    {
        get => _isShowSelectReportPopup;
        set
        {
            _isShowSelectReportPopup = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowEditPromptPopup
    {
        get => _isShowEditPromptPopup;
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

    public bool IsPrivatePrompt
    {
        get => _isPrivatePrompt;
        set
        {
            _isPrivatePrompt = value;
            IsPublicPrompt = !_isPrivatePrompt;
            OnPropertyChanged();
        }
    }
    public bool IsPublicPrompt
    {
        get => _isPublicPrompt;
        set
        {
            _isPublicPrompt = value; 
            OnPropertyChanged();
        }
    }
    public bool IsShowLanguagePopup
    {
        get => _isShowLanguagePopup;
        set
        {
            _isShowLanguagePopup = value; 
            OnPropertyChanged();
        }
    }
    public bool IsShowCategoryPopup
    {
        get => _isShowCategoryPopup;
        set
        {
            _isShowCategoryPopup = value; 
            OnPropertyChanged();
        }
    }

    public string FeedbackInputMessage
    {
        get => _feedbackInputMessage;
        set
        {
            _feedbackInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyFeedbackInput));
        }
    }
    public string PromptNameInputMessage
    {
        get => _promptNameInputMessage;
        set
        {
            _promptNameInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyPromptNameInput));
        }
    }
    public string PromptDescriptionInputMessage
    {
        get => _promptDescriptionInputMessage;
        set
        {
            _promptDescriptionInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyPromptDescriptionInput));
        }
    }

    public string PromptContentInputMessage
    {
        get => _promptContentInputMessage;
        set
        {
            _promptContentInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyPromptContentInput));
        }
    }


    public string ReportName
    {
        get => _reportName;
        set
        {
            _reportName = value;
            OnPropertyChanged();
        }
    }
    public string LanguageName
    {
        get => _languageName;
        set
        {
            _languageName = value;
            OnPropertyChanged();
        }
    }
    public string CategoryName
    {
        get => _categoryName;
        set
        {
            _categoryName = value;
            OnPropertyChanged();
        }
    }

    public string PublicPromptLanguage
    {
        get => _publicPromptLanguage;
        set
        {
            _publicPromptLanguage = value;
            OnPropertyChanged();
        }
    }

    public string PublicPromptCategory
    {
        get => _publicPromptCategory;
        set
        {
            _publicPromptCategory = value;
            OnPropertyChanged();
        }
    }
    public string PromptHeader
    {
        get => _promptHeader;
        set
        {
            _promptHeader = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PromptPopupItem> ReportButtons
    {
        get => _reportButtons;
        set
        {
            _reportButtons = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<PromptPopupItem> LanguageButtons
    {
        get => _languageButtons;
        set
        {
            _languageButtons = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<PromptPopupItem> CategoryButtons
    {
        get => _categoryButtons;
        set
        {
            _categoryButtons = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PromptBracketItem> BracketItems
    {
        get => _bracketItems;
        set
        {
            _bracketItems = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands
    public RelayCommand UsePromptCommand { get; set; }
    public RelayCommand ClosePopupCommand { get; set; }
    public RelayCommand OpenReportCommand { get; set; }
    public RelayCommand SendReportCommand { get; set; }
    public RelayCommand CopyPromptCommand { get; set; }
    public RelayCommand SaveEditPromptCommand { get; set; }
    public RelayCommand SelectPromptTypeCommand { get; set; }
    public RelayCommand ShowSelectionPopupCommand { get; set; }
    #endregion

    public PromptPopupViewModel()
    {
        ClosePopupCommand = new RelayCommand(ExecuteClosePopupCommand, o => true);
        OpenReportCommand = new RelayCommand(o => { IsShowReportPopup = true; }, o => true);
        SaveEditPromptCommand = new RelayCommand(ExecuteSaveEditPromptCommand, o => true);
        SendReportCommand = new RelayCommand(ExecuteSendReportCommand, o => true);
        UsePromptCommand = new RelayCommand(ExecuteUsePromptCommand, o => true);
        CopyPromptCommand = new RelayCommand(ExecuteCopyPromptCommand, o => true);
        SelectPromptTypeCommand = new RelayCommand(ExecuteSelectPromptTypeCommand, o => true);
        ShowSelectionPopupCommand = new RelayCommand(ExecuteShowSelectionPopupCommand, o => true);

        PromptHeader = "New Prompt";
        
        IsShowEditPromptPopup = true;
        IsPrivatePrompt = true;
        ReportButtons = InitButtonPopup("report_list");
        LanguageButtons = InitButtonPopup("language_list");
        CategoryButtons = InitButtonPopup("category_list");

        List<string> category = new List<string> { "Industry", "Competitors", "Target Market", "Price" };
        OnUsePrompt(category);
    }

    private List<string> ReadFromJson(string fileName)
    {
        string relativePath = Path.Combine("Appsettings", "Configs", $"{fileName}.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = File.ReadAllText(fullPath);

        return JsonSerializer.Deserialize<List<String>>(jsonContent);
    }

    private ObservableCollection<PromptPopupItem> InitButtonPopup(string fileName)
    {
        ObservableCollection<PromptPopupItem> Buttons = new ObservableCollection<PromptPopupItem>();
        List<string> buttons = ReadFromJson(fileName);
        
        for (int idx = 0; idx < buttons.Count; idx++)
        {
            Buttons.Add(new PromptPopupItem
            {
                Idx = idx,
                Name = buttons[idx],
                Background = (idx != 0) ? "Transparent" : "#E0EFFE",
                SelectCommand = new RelayCommand(obj => ExecuteSelectCommand(obj, fileName), o => true),
        });
        }

        if (fileName == "report_list") { ReportName = Buttons[0].Name; }
        if (fileName == "language_list") { LanguageName = Buttons[14].Name; }
        if (fileName == "category_list") { CategoryName = Buttons[0].Name; }

        return Buttons;
    }
    private async void ExecuteShowSelectionPopupCommand(object obj)
    {
        string type = (string)obj;
        if (type == "Language") { IsShowLanguagePopup = !IsShowLanguagePopup; }
        if (type == "Category") { IsShowCategoryPopup = !IsShowCategoryPopup; }
        if (type == "Report") { IsShowReportPopup = !IsShowReportPopup; }
    }

    private async void ExecuteClosePopupCommand(object obj)
    {
        string type = (string)obj;
        if (type == "Edit") { IsShowEditPromptPopup = false; }
        if (type == "PromptDetail") { IsShowPromptDetailPopup = false; }
        if (type == "Report") { IsShowReportPopup = false; }
    }

    private async void ExecuteSaveEditPromptCommand(object obj)
    {
        IsShowEditPromptPopup = false;
        PromptLibraryEventTrigger.PublishSaveEditChanged(true, EventArgs.Empty);
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

    private async void ExecuteSelectCommand(object obj, string type)
    {
        int idx = (int)obj;
        if (type == "report_list")
        {
            foreach (var button in ReportButtons) { button.Background = "Transparent"; }

            ReportName = ReportButtons[idx].Name;
            ReportButtons[idx].Background = "#E0EFFE";
            IsShowSelectReportPopup = false;
        }

        else if (type == "language_list")
        {
            foreach (var button in LanguageButtons) { button.Background = "Transparent"; }

            LanguageName = LanguageButtons[idx].Name;
            LanguageButtons[idx].Background = "#E0EFFE";
            IsShowLanguagePopup = false;
        }

        else if (type == "category_list")
        {
            foreach (var button in CategoryButtons) { button.Background = "Transparent"; }

            CategoryName = CategoryButtons[idx].Name;
            CategoryButtons[idx].Background = "#E0EFFE";
            IsShowCategoryPopup = false;
        }
    }

    private async void ExecuteSelectPromptTypeCommand(object obj)
    {
        string type = (string)obj;
        IsPrivatePrompt = (type == "Private") ? true : false;
    }

    public void OnUsePrompt(List<string> category)
    {
        BracketItems = new ObservableCollection<PromptBracketItem>();
        double minHeight = (category.Count > 1) ? 48 : 80;
        double maxHeight = minHeight + 40;

        for (int idx = 0; idx < category.Count; idx++)
        {
            var item = category[idx];
            BracketItems.Add(new PromptBracketItem
            {
                Margin = (idx < category.Count - 1) ? $"0 0 0 {12}" : "0",
                PreText = item,
                InputText = "",
                MinHeight = minHeight,
                MaxHeight = maxHeight,
            });
        }
    }
}
