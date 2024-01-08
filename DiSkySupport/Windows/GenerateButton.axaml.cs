using System.Text;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.Input;
using DiSkySupport.Generator;
using SkEditor.API;

namespace DiSkySupport.Windows;

public partial class GenerateButton : Window
{
    public GenerateButton()
    {
        InitializeComponent();
        
        ButtonName.TextChanged += (_, _) => UpdatePreview();
        ButtonStyle.SelectionChanged += (_, _) => UpdatePreview();
        ButtonStyle.SelectionChanged += (_, _) => UpdateIdBox();
        Disabled.IsCheckedChanged += (_, _) => UpdatePreview();
        
        GenerateButtonCommand.Command = new RelayCommand(GenerateCode);
        
        UpdatePreview();
    }

    public void UpdatePreview()
    {
        var disabled = Disabled.IsChecked ?? false;
        PreviewImage.Opacity = disabled ? 0.5 : 1;
        PreviewImage.Source = ButtonGenerator.GenerateButton((ButtonStyle?.SelectedValue as ComboBoxItem).Tag.ToString(), 
            ButtonName.Text);
    }
    
    public void UpdateIdBox()
    {
        var style = (ButtonStyle?.SelectedValue as ComboBoxItem)?.Tag?.ToString()?.ToLower();
        if (style == null) 
            return;
        ButtonIdText.Text = style == "link" ? "Button URL" : "Button ID";
    }

    public void GenerateCode()
    {
        if (string.IsNullOrEmpty(ButtonEmoji.Text) && string.IsNullOrEmpty(ButtonName.Text))
        {
            ApiVault.Get().ShowMessage("Error", "You must fill the button name or emoji field.");
            return;
        }
        
        var builder = new StringBuilder();
        
        TextEditor editor = ApiVault.Get().GetTextEditor();
        int offset = editor.CaretOffset;

        builder.Append("new ");
        
        if (Disabled.IsChecked ?? false)
            builder.Append("disabled ");
        
        var style = (ButtonStyle?.SelectedValue as ComboBoxItem).Tag.ToString().ToLower();
        builder.Append(style + " ");
        
        builder.Append("button with ");
        builder.Append(style == "link" ? "url " : "id ");
        builder.Append($"\"{ButtonId.Text}\" ");

        if (!string.IsNullOrEmpty(ButtonName.Text))
            builder.Append($"named \"{ButtonName.Text}\" ");
        if (!string.IsNullOrEmpty(ButtonEmoji.Text))
            builder.Append($"with reaction \"{ButtonEmoji.Text}\"");
        
        editor.Document.Insert(offset, builder.ToString(), AnchorMovementType.AfterInsertion);
        Close();
    }
}