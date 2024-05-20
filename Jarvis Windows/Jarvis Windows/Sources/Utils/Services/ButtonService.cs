using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Jarvis_Windows.Sources.Utils.Services;

public class AIActionTemplate
{
    public ObservableCollection<AIButton> FixedAIActionList { get; set; } = new ObservableCollection<AIButton>
    {
        new AIButton
        {
            Icon = "🌐",
            Content = "Translate it",
            Visibility = true,
            Command = null,
            CommandParameter = "Translate it",
            Width = 221,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "✏️",
            Content = "Revise it",
            Visibility = true,
            Command = null,
            CommandParameter = "Revise it",
            Width = 108,
            Margin = new Thickness(0, 0, 10, 10)
        }
    };
    public ObservableCollection<AIButton> DynamicAIActionList { get; set; } = new ObservableCollection<AIButton>
    {
        new AIButton
        {
            Icon = "📌",
            Content = "Extract the main information",
            Visibility = true,
            Command = null,
            CommandParameter = "Extract the main information",
            Width = 230,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "🚀",
            Content = "More",
            Visibility = true,
            Command = null,
            CommandParameter = "More",
            Width = 90,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "👋",
            Content = "Make it constructive",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it constructive",
            Width = 179,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "🖼️",
            Content = "Make it more detailed",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it more detailed",
            Width = 189,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "🎯",
            Content = "Make it persuasive",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it persuasive",
            Width = 171,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "🖋️",
            Content = "Paraphrase it",
            Visibility = false,
            Command = null,
            CommandParameter = "Paraphrase it",
            Width = 137,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "📝",
            Content = "Summarize it",
            Visibility = false,
            Command = null,
            CommandParameter = "Summarize it",
            Width = 135,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "✂️",
            Content = "Simplify it",
            Visibility = false,
            Command = null,
            CommandParameter = "Simplify it",
            Width = 114.695,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "📜",
            Content = "Give a quote",
            Visibility = false,
            Command = null,
            CommandParameter = "Give a quote",
            Width = 133,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "🙂",
            Content = "Give a random name",
            Visibility = false,
            Command = null,
            CommandParameter = "Give a random name",
            Width = 184,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "✉️",
            Content = "Give an asking for help email template",
            Visibility = false,
            Command = null,
            CommandParameter = "Give an asking for help email template",
            Width = 292,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "📧",
            Content = "Give a thank you for help email template",
            Visibility = false,
            Command = null,
            CommandParameter = "Give a thank you for help email template",
            Width = 305,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new AIButton
        {
            Icon = "🔼",
            Content = "Less",
            Visibility = false,
            Command = null,
            CommandParameter = "Less",
            Width = 90,
            Margin = new Thickness(0, 0, 10, 10)
        },
    };
    public ObservableCollection<AIButton> MenuSelectionButtonList { get; set; } = new ObservableCollection<AIButton>
    {
        new AIButton
        {
            Icon = "🌐",
            Visibility = true,
            Command = null,
            PinCommand = null,
            CommandParameter = "Translate it",
            Content = "Translate",
            CornerRadius = "16 0 0 16",
            PinColor = "#6841EA"
        },
        new AIButton
        {
            Icon = "✏️",
            Visibility = true,
            Command = null,
            PinCommand = null,
            CommandParameter = "Revise it",
            Content = "Revise",
            CornerRadius = "0",
            PinColor = "#6841EA"
        },
        new AIButton
        {
            Icon = "📌",
            Visibility = false,
            Command = null,
            PinCommand = null,
            CommandParameter = "Extract the main information",
            Content = "Extract",
            CornerRadius = "0"
        },
        new AIButton
        {
            Icon = "👋",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it constructive",
            Content = "Constructive",
            CornerRadius = "0"
        },
        new AIButton
        {
            Icon = "🖼️",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it more detailed",
            Content = "Detail",
            CornerRadius = "0"
        },
        new AIButton
        {
            Icon = "🎯",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it persuasive",
            Content = "Persuasive",
            CornerRadius = "0"
        },
        new AIButton
        {
            Icon = "🖋️",
            Visibility = false,
            Command = null,
            CommandParameter = "Paraphrase it",
            Content = "Paraphrase",
            CornerRadius = "0"
        },
        new AIButton
        {
            Icon = "📝",
            Visibility = false,
            Command = null,
            CommandParameter = "Summarize it",
            Content = "Summarize",
            CornerRadius = "0"
        },
        new AIButton
        {
            Icon = "✂️",
            Visibility = false,
            Command = null,
            CommandParameter = "Simplify it",
            Content = "Simplify",
            CornerRadius = "0"
        }
    };
}