using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OwlCore.Extensions;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Resources;

namespace StrixMusic.Shells.Media11;

/// <summary>
/// A custom XAML resource dictionary that generates accent color variations.
/// </summary>
public class AccentColorEffectResourceDictionary : ResourceDictionary
{
    private readonly UISettings _uiSettings = new();
    private ColorSpaceConverter _colorConverter = new();
    private Color _accentColor;

    /// <inheritdoc/>
    public AccentColorEffectResourceDictionary()
    {
        var accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
        UpdateSource(accentColor);

        // Listen for theme changes
        _uiSettings.ColorValuesChanged += ColorValuesChanged;
    }

    private async void ColorValuesChanged(UISettings sender, object args)
    {
        var accentColor = sender.GetColorValue(UIColorType.Accent);

        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        {
            UpdateSource(accentColor);
        });
    }

    private void UpdateSource(Color accentColor)
    {
        if (accentColor == _accentColor)
            return;

        Debug.WriteLine($"New accent color {accentColor}, recomputing resources...");

        var rgbBase = new Rgb(accentColor.R / 255f, accentColor.G / 255f, accentColor.B / 255f);
        //var rgbBase = new Rgb(36 / 255f, 71 / 255f, 178 / 255f);
        var lchBase = _colorConverter.ToCieLch(rgbBase);

        this["SystemAccentColor"] = ConvertToColor(rgbBase);
        this["SystemAccentColor_Test"] = ConvertToColor(rgbBase);

        var lchHighlightTarget = _colorConverter.ToCieLch(new Rgb(87 / 255f, 198 / 255f, 239 / 255f));
        this["SystemAccentColor_HighlightT"] = ConvertToColor(lchHighlightTarget);

        var lchHighlight = new CieLch(
            LogIncrease(lchBase.L, 1.5f, 100),
            lchBase.C / 1.75f,
            HueOffset(lchBase.H, -55)
        );
        this["SystemAccentColor_Highlight"] = ConvertToColor(lchHighlight);

        var lchSubtleBackground = new CieLch(
            Application.Current.RequestedTheme is ApplicationTheme.Light
                ? 95f : 15f,
            15f,
            lchBase.H
        );
        this["SystemAccentColor_SubtleBackground"] = ConvertToColor(lchSubtleBackground);

        _accentColor = accentColor;
    }

    private Color ConvertToColor(CieLch lch) => ConvertToColor(_colorConverter.ToRgb(lch));

    private static Color ConvertToColor(Rgb rgb) => Color.FromArgb(255, (byte)(rgb.R * 255), (byte)(rgb.G * 255), (byte)(rgb.B * 255));

    private static float LogIncrease(float x, float a, float m)
    {
        var head = m - x;
        return x + (head / a);
    }

    private static float HueOffset(float hue, float offset)
    {
        var h2 = hue + offset;
        return h2 switch
        {
            > 360 => h2 - 360,
            < 0 => h2 + 360,
            _ => h2
        };
    }
}
