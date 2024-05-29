using ICSharpCode.AvalonEdit;
using System.Windows;
using System;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public static class TextEditorBindingBehavior
{
    public static readonly DependencyProperty BoundTextProperty =
        DependencyProperty.RegisterAttached("BoundText", typeof(string), typeof(TextEditorBindingBehavior), new PropertyMetadata(default(string), OnBoundTextChanged));
    
    public static readonly DependencyProperty BoundLanguageProperty =
        DependencyProperty.RegisterAttached("BoundLanguage", typeof(string), typeof(TextEditorBindingBehavior), new PropertyMetadata(default(string), OnBoundLanguageChanged));

    public static string GetBoundText(DependencyObject obj) => (string)obj.GetValue(BoundTextProperty);
    public static void SetBoundText(DependencyObject obj, string value) => obj.SetValue(BoundTextProperty, value);
    public static string GetBoundLanguage(DependencyObject obj) => (string)obj.GetValue(BoundLanguageProperty);
    public static void SetBoundLanguage(DependencyObject obj, string value) => obj.SetValue(BoundLanguageProperty, value);

    private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextEditor textEditor)
        {
            textEditor.TextChanged -= OnTextEditorTextChanged;
            if (e.NewValue is string newText)
            {
                textEditor.Text = newText;
                textEditor.TextChanged += OnTextEditorTextChanged;
            }
        }
    }

    private static void OnBoundLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextEditor textEditor)
        {
            if (e.NewValue is string newLanguage)
            {
                textEditor.SetCurrentValue(TextEditor.SyntaxHighlightingProperty, HighlightingManager.Instance.GetDefinition(newLanguage));
            }
        }
    }

    private static void OnTextEditorTextChanged(object sender, EventArgs e)
    {
        if (sender is TextEditor textEditor)
        {
            SetBoundText(textEditor, textEditor.Text);
        }
    }
}
