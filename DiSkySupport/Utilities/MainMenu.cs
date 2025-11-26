using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using DiSkySupport.Windows;
using SkEditor.API;
using SkEditor.Controls;
using SkEditor.Views;

namespace DiSkySupport.Utilities;

public static class MainMenu
{
    private static void AddMenuItems()
    {
        MainWindow? mainWindow = SkEditorAPI.Windows.GetMainWindow();
        if (mainWindow == null) return;

        MenuItem? generateToolsItem = GetGenerateToolsMenuItem();
        if (generateToolsItem == null) return;
        
        var generateDiSkyItem = new MenuItem
        {
            Header = "Disky", 
            Name = "GenerateDiskyMenuItem", 
            Icon = Icons.GetIcon(Icons.DiscordIcon)
        };
        
        generateDiSkyItem.Items.Add(new MenuItem
        {
            Header = "Bot Loading", 
            Icon = Icons.GetIcon(Icons.BotIcon),
            Command = new RelayCommand(() => new GenerateBot().ShowDialog(mainWindow))
        });
        generateDiSkyItem.Items.Add( new MenuItem
        {
            Header = "Button", 
            Icon = Icons.GetIcon(Icons.ButtonIcon),
            Command = new RelayCommand(() => new GenerateButton().ShowDialog(mainWindow))
        } );
        generateDiSkyItem.Items.Add(new MenuItem
        {
            Header = "Slash Command (Soon)", 
            Icon = Icons.GetIcon(Icons.SlashCommandIcon)
        } );    
        
        generateToolsItem.Items.Add(generateDiSkyItem);
    }

    private static void RemoveMenuItem()
    {
        MenuItem? generateToolsItem = GetGenerateToolsMenuItem();

        MenuItem? generateDiskyItem = generateToolsItem?.Items.OfType<MenuItem>().FirstOrDefault(i => i.Name == "GenerateDiskyMenuItem");
        
        if (generateDiskyItem == null) return;
        
        generateToolsItem?.Items.Remove(generateDiskyItem);
    }

    public static void RefreshMenuItems()
    {
        RemoveMenuItem();
        AddMenuItems();
    }

    private static MenuItem? GetGenerateToolsMenuItem()
    {
        var mainMenuControl = SkEditorAPI.Windows.GetMainWindow()?.Get<MainMenuControl>("MainMenu");
        var mainMenu = mainMenuControl?.Get<Menu>("MainMenu");
        var toolsMenu = (MenuItem?)mainMenu?.Items[2];
        return (MenuItem?)toolsMenu?.Items[0];
    }
}
