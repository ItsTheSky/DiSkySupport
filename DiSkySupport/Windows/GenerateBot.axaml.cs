using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
        InitializeComponent();
        
        GenerateButton.Command = new RelayCommand(Generate);
    }

    public void Generate()
    {
        if (string.IsNullOrEmpty(BotName.Text) || string.IsNullOrEmpty(BotToken.Text))
        {
            ApiVault.Get().ShowMessage("Error", "You must fill the bot name and token fields.");
            return;
        }
        
        StringBuilder code = new();

        TextEditor editor = ApiVault.Get().GetTextEditor();
        int offset = editor.CaretOffset;
        
        code.Append($"define new bot named \"{BotName.Text}\":");
        
        // Basic infos
        code.Append($"\n\ttoken: \"{BotToken.Text}\"");
        if (!string.IsNullOrEmpty(BotCachePolicy.Tag as string)) code.Append($"\n\tpolicy: {BotCachePolicy.Tag.ToString()}");
        if (!string.IsNullOrEmpty(BotCacheFlags.Tag as string)) code.Append($"\n\tcache flags: {BotCacheFlags.Tag.ToString()}");
        if (!string.IsNullOrEmpty(BotCompression.Tag as string)) code.Append($"\n\tcompression: {BotCompression.Tag.ToString()}");
        
        // Reloading 
        code.Append($"\n\n\tauto reconnect: {AutoReconnect.IsChecked.ToString()?.ToLower()}");
        code.Append($"\n\tforce reload: {ForceReload.IsChecked.ToString()?.ToLower()}");
        
        // Intents
        var intents = new StringBuilder(); // invents will be separated by ', '
        if (GatewayIntents.SelectedItems != null)
            foreach (var item in GatewayIntents.SelectedItems)
            {
                var tag = ((ListBoxItem)item).Tag?.ToString();
                if (tag is null)
                    continue;

                if (tag is "default")
                {
                    intents = new StringBuilder("default intents");
                    break;
                }

                if (intents.Length > 0)
                    intents.Append(", ");

                intents.Append(tag);
            }

        code.Append($"\n\n\tintents: {intents.ToString()}");
        
        // Sections
        if (GenerateReady.IsChecked ?? true)
            code.Append("\n\n\ton ready:\n\t\t# </> Code to execute when the bot is ready");
        if (GenerateGuildReady.IsChecked ?? true)
            code.Append("\n\n\ton guild ready:\n\t\t# </> Code to execute when a guild is ready");
        if (GenerateShutdown.IsChecked ?? true)
            code.Append("\n\n\ton shutdown:\n\t\t# </> Code to execute when the bot is shutting down");
        
        editor.Document.Insert(offset, code.ToString(), AnchorMovementType.AfterInsertion);
        Close();
    }
}