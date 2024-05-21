using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public class CustomRichTextBox : RichTextBox
{
    public static readonly DependencyProperty TextContentProperty =
        DependencyProperty.Register("TextContent", typeof(string), typeof(CustomRichTextBox),
            new PropertyMetadata(string.Empty, OnTextContentChanged));

    public string TextContent
    {
        get { return (string)GetValue(TextContentProperty); }
        set { SetValue(TextContentProperty, value); }
    }

    private static void OnTextContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomRichTextBox richTextBox)
        {
            richTextBox.UpdateText();
        }
    }

    private void UpdateText()
    {
        this.Document.Blocks.Clear();

        if (string.IsNullOrEmpty(TextContent))
        {
            return;
        }

        var paragraph = new Paragraph();
        string text = TextContent;
        int lastPos = 0;

        // Regex to match `...`, ```...```, **...**, #..., and Step <number>: ...
        var matches = Regex.Matches(text, @"(```[^```]+```|`[^`]+`|\*\*[^*]+\*\*|^#.*|^Step\s+\d+:\s+.*(\n*\s*)?)", RegexOptions.Multiline);
        foreach (Match match in matches)
        {
            if (match.Index > lastPos)
            {
                paragraph.Inlines.Add(new Run(text.Substring(lastPos, match.Index - lastPos)));
            }

            string matchValue = match.Value;
            if (matchValue.StartsWith("`") || matchValue.StartsWith("`"))
            {
                var innerText = matchValue.Trim('`').Trim();
                while (innerText.StartsWith("\n"))
                {
                    innerText = innerText.Substring(1).Trim(' ');
                }
                while (innerText.EndsWith("\n"))
                {
                    innerText = innerText.Substring(0, innerText.Length - 1).Trim(' ');
                }

                var specialText = new TextBlock(new Run(innerText))
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var borderSpecialText = new Border
                {
                    Background = new BrushConverter().ConvertFromString("#E5E7EB") as Brush,
                    CornerRadius = new CornerRadius(4),
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(4, 0, 4, 0),
                    Margin = new Thickness(0, 1, 0, -4),
                    Child = specialText
                };

                paragraph.Inlines.Add(new InlineUIContainer(borderSpecialText));
            }
            else if (matchValue.StartsWith("**"))
            {
                var boldText = matchValue.Trim('*');
                paragraph.Inlines.Add(new Bold(new Run(boldText)));
            }
            else if (matchValue.StartsWith("#") || Regex.IsMatch(matchValue, @"^Step\s+\d+:\s+.*"))
            {
                var innerText = matchValue.Trim('#').Trim(' ');
                paragraph.Inlines.Add(new Bold(new Run(innerText)));
            }

            lastPos = match.Index + match.Length;
        }

        if (lastPos < text.Length)
        {
            paragraph.Inlines.Add(new Run(text.Substring(lastPos).Trim(' ')));
        }

        this.Document.Blocks.Add(paragraph);
        this.Document.SetCurrentValue(FlowDocument.MaxPageWidthProperty, (double)406);
    }
}