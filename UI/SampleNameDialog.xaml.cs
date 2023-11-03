using System.Windows;
using System.Windows.Controls;

namespace AI_neural.UI;

public partial class SampleNameDialog
{
    public SampleNameDialog()
    {
        InitializeComponent();
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ConfirmButton.IsEnabled = Text.Text.Length > 0;
    }

    private void OnConfirm(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    public static string? Prompt(string? initialText = null)
    {
        var dialog = new SampleNameDialog();
        if (initialText != null) dialog.Text.Text = initialText;
        else dialog.ConfirmButton.IsEnabled = false;
        return dialog.ShowDialog() == true ? dialog.Text.Text : null;
    }
}