using System.Net;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using DiSkySupport.Windows;
using FluentAvalonia.UI.Controls;
using SkEditor.API;
using SkEditor.Utilities;

namespace DiSkySupport;

public class DiSkyAddon : IAddon
{
    public async void OnEnable()
    {

        try
        {
            // Bot generator
            var generateDiSky = new MenuItem { Header = "DiSky", Icon = Icons.GetIcon(Icons.DiscordIcon) };
            var generateBotLoading = new MenuItem { Header = "Bot Loading", Icon = Icons.GetIcon(Icons.BotIcon) };
            var generateButton = new MenuItem { Header = "Button", Icon = Icons.GetIcon(Icons.ButtonIcon) };
            var generateSlashCommand = new MenuItem { Header = "Slash Command (Soon)", Icon = Icons.GetIcon(Icons.SlashCommandIcon) };
            
            generateDiSky.Items.Add(generateBotLoading);
            generateDiSky.Items.Add(generateButton);
            generateDiSky.Items.Add(generateSlashCommand);

            // Try getting the right item ...
            var menu = ApiVault.Get().GetMenu();
            var tools = menu.Items[2] as MenuItem;
            var generate = tools?.Items[0] as MenuItem;
            
            generate?.Items.Add(generateDiSky);
            generateBotLoading.Command = new RelayCommand(() => new GenerateBot().ShowDialog(ApiVault.Get().GetMainWindow()));
            generateButton.Command = new RelayCommand(() => new GenerateButton().ShowDialog(ApiVault.Get().GetMainWindow()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ApiVault.Get().ShowMessage("Loading Error", "DiSky addon failed to load correctly, surely because the code base of SkEditor change! Wait for an update of the addon to come out.");
        }
    }

    public string Name => "DiSky";
    public string Version => "1.0.0";
    
}