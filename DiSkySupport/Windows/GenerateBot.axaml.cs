using System.Text;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Windowing;
using SkEditor.API;

namespace DiSkySupport.Windows;

public partial class GenerateBot : AppWindow
{
    public GenerateBot()
    {
        this.InitializeComponent();

        this.GenerateButton.Command = new AsyncRelayCommand(this.Generate);
    }

    private async Task Generate()
    {
        if (string.IsNullOrEmpty(this.BotName.Text) || string.IsNullOrEmpty(this.BotToken.Text))
        {
            await SkEditorAPI.Windows.ShowMessage("Error", "You must fill the bot name and token fields.");
            return;
        }

        StringBuilder code = new();

        TextEditor? editor = SkEditorAPI.Files.GetCurrentOpenedFile()?.Editor;

        if (editor == null)
        {
            this.Close();
            return;
        }

        int offset = editor.CaretOffset;

        code.Append($"define new bot named \"{this.BotName.Text}\":");

        // Basic infos
        code.Append($"\n\ttoken: \"{this.BotToken.Text}\"");
        if (!string.IsNullOrEmpty(this.BotCachePolicy.Tag as string))
            code.Append($"\n\tpolicy: {this.BotCachePolicy.Tag.ToString()}");
        if (!string.IsNullOrEmpty(this.BotCacheFlags.Tag as string))
            code.Append($"\n\tcache flags: {this.BotCacheFlags.Tag.ToString()}");
        if (!string.IsNullOrEmpty(this.BotCompression.Tag as string))
            code.Append($"\n\tcompression: {this.BotCompression.Tag.ToString()}");

        // Reloading 
        code.Append($"\n\n\tauto reconnect: {this.AutoReconnect.IsChecked.ToString()?.ToLower()}");
        code.Append($"\n\tforce reload: {this.ForceReload.IsChecked.ToString()?.ToLower()}");

        // Intents
        var intents = new StringBuilder(); // invents will be separated by ', '

        if (this.GatewayIntents.SelectedItems != null)
            foreach (var item in this.GatewayIntents.SelectedItems)
            {
                var tag = ((ListBoxItem)item).Tag?.ToString();

                if (tag is null) continue;

                if (tag is "default")
                {
                    intents = new StringBuilder("default intents");

                    break;
                }

                if (intents.Length > 0) intents.Append(", ");

                intents.Append(tag);
            }

        code.Append($"\n\n\tintents: {intents.ToString()}");

        // Sections
        if (this.GenerateReady.IsChecked ?? true)
            code.Append("\n\n\ton ready:\n\t\t# </> Code to execute when the bot is ready");
        if (this.GenerateGuildReady.IsChecked ?? true)
            code.Append("\n\n\ton guild ready:\n\t\t# </> Code to execute when a guild is ready");
        if (this.GenerateShutdown.IsChecked ?? true)
            code.Append("\n\n\ton shutdown:\n\t\t# </> Code to execute when the bot is shutting down");

        editor.Document.Insert(offset, code.ToString(), AnchorMovementType.AfterInsertion);


        this.Close();
    }
}