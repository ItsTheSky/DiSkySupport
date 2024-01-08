using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace DiSkySupport.Generator;

public static class ButtonGenerator
{
    private static readonly Dictionary<string, Bitmap> CachedImages = new();
    private static Bitmap? _cachedLinkIcon;
    
    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Link
    }

    public static IImage GenerateButton(string tag, string name)
    {
        var style = GetStyleFromTag(tag);
        var pathName = style == ButtonStyle.Link ? "secondary" : style.ToString().ToLower();
        var ninePatch = CachedImages.TryGetValue(pathName, out var image) ? image : new Bitmap(AssetLoader.Open(new Uri($"avares://DiSkySupport/Assets/Buttons/{pathName}.png")));
        
        var link = style == ButtonStyle.Link;
        if (link && _cachedLinkIcon == null)
            _cachedLinkIcon = new Bitmap(AssetLoader.Open(new Uri($"avares://DiSkySupport/Assets/Buttons/link-icon.png")));
        
        var font = new FontFamily("Whitney Medium");
        var textBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        var text = new FormattedText(name,
            CultureInfo.CurrentCulture,
            FlowDirection.RightToLeft, new Typeface(font),
            35, textBrush);
        
        var nameWidth = (int) text.Width;
        var horizontalPadding = 24;
        var totalMiddleWidth = nameWidth + horizontalPadding * 2 + (link ? 24 + 30 : 0);
        
        var ninePatchHeight = ninePatch.PixelSize.Height; // should always be 64
        
        var newImage = new RenderTargetBitmap(new PixelSize(4 + 4 + totalMiddleWidth, ninePatchHeight));
        using var context = newImage.CreateDrawingContext();
        
        // Draw left side
        context.DrawImage(ninePatch, new Rect(0, 0, 5, ninePatchHeight));
        // Draw middle
        context.DrawImage(ninePatch, new Rect(4, 0, 1, ninePatchHeight), new Rect(4, 0, totalMiddleWidth, ninePatchHeight));
        // Draw right side
        context.DrawImage(ninePatch, new Rect(5, 0, 4, ninePatchHeight), new Rect(4 + totalMiddleWidth, 0, 4, ninePatchHeight));
        // Draw link icon
        if (link)
            context.DrawImage(_cachedLinkIcon, new Rect(4 + horizontalPadding + nameWidth + 24, (ninePatchHeight - 30) / 2, 30, 30));
        
        // Draw text6
        context.DrawText(text, new Point(4 + horizontalPadding, 
            (ninePatchHeight - text.Height) / 2));

        return newImage;
    }
    
    public static ButtonStyle GetStyleFromTag(string tag)
    {
        return tag switch
        {
            "primary" => ButtonStyle.Primary,
            "secondary" => ButtonStyle.Secondary,
            "success" => ButtonStyle.Success,
            "danger" => ButtonStyle.Danger,
            "link" => ButtonStyle.Link,
            _ => ButtonStyle.Primary
        };
    }
    
}