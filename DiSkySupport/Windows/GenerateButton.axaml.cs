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
        this.InitializeComponent();

        this.ButtonName.TextChanged += (_, _) => this.UpdatePreview();
        this.ButtonStyle.SelectionChanged += (_, _) => this.UpdatePreview();
        this.ButtonStyle.SelectionChanged += (_, _) => this.UpdateIdBox();
        this.Disabled.IsCheckedChanged += (_, _) => this.UpdatePreview();

        this.GenerateButtonCommand.Command = new AsyncRelayCommand(this.GenerateCode);

        this.UpdatePreview();
    }

    private void UpdatePreview()
    {
        var disabled = this.Disabled.IsChecked ?? false;
        this.PreviewImage.Opacity = disabled ? 0.5 : 1;
        
        var selectedStyle = (this.ButtonStyle.SelectedValue as ComboBoxItem)?.Tag?.ToString()?.ToLower();
        
        if (selectedStyle == null) 
            return;
        
        this.PreviewImage.Source = ButtonGenerator.GenerateButton(selectedStyle, this.ButtonName.Text ?? "");
    }

    private void UpdateIdBox()
    {
        var style = (this.ButtonStyle?.SelectedValue as ComboBoxItem)?.Tag?.ToString()?.ToLower();
        
        if (style == null) 
            return;
        
        this.ButtonIdText.Text = style == "link" ? "Button URL" : "Button ID";
    }

    private async Task GenerateCode()
    {
        if (string.IsNullOrEmpty(this.ButtonEmoji.Text) && string.IsNullOrEmpty(this.ButtonName.Text))
        {
            await SkEditorAPI.Windows.ShowMessage("Error", "You must fill the button name or emoji field.");
            return;
        }
        
        var builder = new StringBuilder();
        
        TextEditor? editor = SkEditorAPI.Files.GetCurrentOpenedFile()?.Editor;
        
        if (editor == null)
        {
            this.Close();
            
            return;
        }
        
        int offset = editor.CaretOffset;

        builder.Append("new ");
        
        if (this.Disabled.IsChecked ?? false)
            builder.Append("disabled ");
        
        var style = (this.ButtonStyle.SelectedValue as ComboBoxItem)?.Tag?.ToString()?.ToLower();
        
        if (style == null) 
            return;
        
        builder.Append(style + " ");
        
        builder.Append("button with ");
        builder.Append(style == "link" ? "url " : "id ");
        builder.Append($"\"{this.ButtonId.Text}\" ");

        if (!string.IsNullOrEmpty(this.ButtonName.Text))
            builder.Append($"named \"{this.ButtonName.Text}\" ");
        if (!string.IsNullOrEmpty(this.ButtonEmoji.Text))
            builder.Append($"with reaction \"{this.ButtonEmoji.Text}\"");
        
        editor.Document.Insert(offset, builder.ToString(), AnchorMovementType.AfterInsertion);
        this.Close();
    }
}