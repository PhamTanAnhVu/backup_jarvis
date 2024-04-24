using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class TextMenuViewModel : ViewModelBase
{
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _uIElementDetector;
    private SendEventGA4 _sendEventGA4;

    private int _languageSelectedIndex;
    private string _remainingAPIUsage;
    private string _textMenuAPI;
    private bool _isSpinningJarvisIconTextMenu;
    private double _textMenuAPIscrollBarHeight;

    private ObservableCollection<AIButton> _textMenuButtons;
    public List<Language> TextMenuLanguages { get; set; }
    public RelayCommand TextMenuAICommand { get; set; }
    public RelayCommand ShowTextMenuOperationsCommand { get; set; }
    public RelayCommand HideTextMenuAPICommand { get; set; }
    public RelayCommand LanguageComboBoxCommand { get; set; }
    
    public PopupDictionaryService PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set
        {
            _popupDictionaryService = value;
            OnPropertyChanged();
        }
    }

    public UIElementDetector UIElementDetector
    {
        get { return _uIElementDetector; }
        set
        {
            _uIElementDetector = value;
            OnPropertyChanged();
        }
    }

    public SendEventGA4 SendEventGA4
    {
        get { return _sendEventGA4; }
        set
        {
            _sendEventGA4 = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<AIButton> TextMenuButtons
    {
        get { return _textMenuButtons; }
        set
        {
            _textMenuButtons = value;
            OnPropertyChanged();
        }
    }

    public string RemainingAPIUsage
    {
        get { return _remainingAPIUsage; }
        set
        {
            _remainingAPIUsage = value;
            OnPropertyChanged();
        }
    }

    public int LanguageSelectedIndex
    {
        get { return _languageSelectedIndex; }
        set
        {
            if (_languageSelectedIndex != value)
            {
                _languageSelectedIndex = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsSpinningJarvisIconTextMenu
    {
        get { return _isSpinningJarvisIconTextMenu; }
        set
        {
            _isSpinningJarvisIconTextMenu = value;
            OnPropertyChanged();
        }
    }

    public string TextMenuAPI
    {
        get { return _textMenuAPI; }
        set
        {
            _textMenuAPI = value;
            OnPropertyChanged();
        }
    }

    public double TextMenuAPIscrollBarHeight
    {
        get { return _textMenuAPIscrollBarHeight; }
        set
        {
            _textMenuAPIscrollBarHeight = value;
            OnPropertyChanged();
        }
    }

    public TextMenuViewModel(PopupDictionaryService popupDictionaryService, UIElementDetector uIElementDetector, SendEventGA4 sendEventGA4)
    {
        PopupDictionaryService = popupDictionaryService;
        UIElementDetector = uIElementDetector;
        SendEventGA4 = sendEventGA4;

        TextMenuAICommand = new RelayCommand(ExecuteTextMenuAICommand, o => true);
        ShowTextMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
        HideTextMenuAPICommand = new RelayCommand(ExecuteHideTextMenuAPICommand, o => true);
        LanguageComboBoxCommand = new RelayCommand(OnLanguageSelectionChanged, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        TextMenuLanguages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        LanguageSelectedIndex = 14;

        InitializeButtons();
    }

    public async void OnLanguageSelectionChanged(object sender)
    {
        TextMenuAICommand.Execute("Translate it");
    }

    public async void ExecuteShowMenuOperationsCommand(object obj)
    {
        bool _menuShowStatus = PopupDictionaryService.IsShowMenuOperations;

        PopupDictionaryService.ShowMenuOperations(!_menuShowStatus);
        PopupDictionaryService.ShowJarvisAction(false);

        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        TextMenuButtons = aIActionTemplate.MenuSelectionButtonList;

        if (_menuShowStatus == false)
        {
            await Task.Run(async () =>
            {
                // Some processing before the await (if needed)
                await Task.Delay(0); // This allows the method to yield to the caller

                await SendEventGA4.SendEvent("open_input_actions");
            });
        }
    }

    public async void ExecuteHideTextMenuAPICommand(object obj)
    {
        PopupDictionaryService.ShowTextMenuAPIOperations(false);

        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        TextMenuButtons = aIActionTemplate.MenuSelectionButtonList;
    }

    private void InitializeButtons()
    {
        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        TextMenuButtons = aIActionTemplate.MenuSelectionButtonList;
    }

    private void OnLanguageSelectionChanged(object sender, EventArgs e)
    {
        TextMenuAICommand.Execute("Translate it");
    }

    public async void ExecuteTextMenuAICommand(object obj)
    {
        string _actionType = (string) obj;
        string _aiAction = "custom";
        string _targetLanguage = TextMenuLanguages[LanguageSelectedIndex].Value;

        try
        {
            TextMenuAPI = "";
            TextMenuAPIscrollBarHeight = 88;
            IsSpinningJarvisIconTextMenu = true;
            PopupDictionaryService.IsShowMenuSelectionResponse = true;

            //var textFromElement = "Jarvis AI Assistant, your all-in-one solution that harnesses the formidable capabilities of ChatGPT, which provides large and wide knowledge, GPT 4 for cutting-edge language understanding, Claude AI for advanced innovations, Llama 2 for next-level text generation, Bard for creative content creation, Bing Chat for seamless communication, Meta AI for deep learning potential, Chat GPT for conversational prowess, Chatting GPT for natural dialogue, GPT Chat for interactive communication, GPT4 for state-of-the-art language processing, and the transformative power of Ajax AI (also known as AjaxAI), OpenAI and latest AI model Gemini.";
            var textFromElement = UIElementDetector.GetSelectedText();

            var textFromAPI = "";

            if (_actionType == "Translate it")
            {
                TextMenuButtons[0].Visibility = TextMenuButtons[1].Visibility = false;
                textFromAPI = await JarvisApi.Instance.TranslateHandler(textFromElement, _targetLanguage);
                _aiAction = "translate";
            }

            else if (_actionType == "Summarize it")
            {
                TextMenuButtons[0].Visibility = TextMenuButtons[2].Visibility = false;
                TextMenuButtons[1].HorizontalAlignment = "Right";
                TextMenuButtons[1].SeparateLineWidth = 0;

                textFromAPI = await JarvisApi.Instance.ReviseHandler(textFromElement);
            }

            else
            {
                TextMenuButtons[1].Visibility = TextMenuButtons[2].Visibility = false;
                TextMenuButtons[0].HorizontalAlignment = "Right";
                TextMenuButtons[0].SeparateLineWidth = 0;

                textFromAPI = await JarvisApi.Instance.AIHandler(textFromElement, _actionType);

            }

            if (textFromAPI == null)
            {
                Debug.WriteLine($"?????? {ErrorConstant.translateError}");
                return;
            }

            //TODO: Test
            UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError);

            TextMenuAPI = textFromAPI;
        }
        catch { }
        finally
        {
            IsSpinningJarvisIconTextMenu = false; // Stop spinning animation
            var eventParams = new Dictionary<string, object>
            {
                { "ai_action", _aiAction }
            };

            if (_aiAction == "translate")
                eventParams.Add("ai_action_translate_to", _targetLanguage);
            else if (_aiAction == "custom")
                eventParams.Add("ai_action_custom", _actionType);

            await SendEventGA4.SendEvent("do_ai_action", eventParams);
        }
    }

    public int GetMenuSelectionActionWidth()
    {
        int visibleButtons = 0;
        foreach (var button in TextMenuButtons)
        {
            if (button.Visibility)
            {
                visibleButtons++;
            }
        }

        return visibleButtons * 32 + 64;
    }

    public int GetMenuSelectionActionHeight()
    {
        return 30;
    }
}
