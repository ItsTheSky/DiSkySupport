using System.Reflection;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DiSkySupport.Utilities;
using FluentAvalonia.UI.Controls;
using SkEditor.API;

namespace DiSkySupport;

public class DiSkyAddon : IAddon
{
    private ImageIconSource? _iconSource;
    
    public string Name => "DiSky";
    
    public string Identifier => "DiSkySupport";
    
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)!;
    
    public string? Description => "Adds some features to SkEditor related to the DiSky addon.";
    
    public Version GetMinimalSkEditorVersion() => new(2, 9, 3);
    
    public IconSource GetAddonIcon()
    {
        if (this._iconSource != null) return this._iconSource;

        Stream stream = AssetLoader.Open(new Uri("avares://DiSkySupport/Assets/DiSky.ico"));
        return this._iconSource =
            new ImageIconSource { Source = new Bitmap(stream) };
    }
    
    public async void OnEnable()
    {
        try
        {
            MainMenu.RefreshMenuItems();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await SkEditorAPI.Windows.ShowMessage("Loading Error", "DiSky addon failed to load correctly, surely because the code base of SkEditor change! Wait for an update of the addon to come out.");
        }
    }
    
}