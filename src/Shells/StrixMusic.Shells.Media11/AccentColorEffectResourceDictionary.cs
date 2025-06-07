using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using OwlCore.Extensions;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Resources;
using ColorHelperTK = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace StrixMusic.Shells.Media11;

/// <summary>
/// A custom XAML resource dictionary that generates accent color variations.
/// </summary>
public class AccentColorEffectResourceDictionary : ResourceDictionary
{
    private readonly UISettings _uiSettings = new();
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

        var hsvAccentColor = accentColor.ToHsv();

        this["SystemAccentColor_Test"] = ColorHelperTK.FromHsv(hsvAccentColor.H, hsvAccentColor.S * 0.5, hsvAccentColor.V);

        _accentColor = accentColor;
    }
}
