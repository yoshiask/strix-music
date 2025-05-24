using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls;

/// <summary>
/// A panel that sets the width of all columns to the content with the largest width.
/// </summary>
public class UniformMinColumnPanel : Panel
{
    private double _actualColumnWidth;   // Does not include spacing

    /// <summary>
    /// The spacing between columns.
    /// </summary>
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Dependency property for <see cref="Spacing"/>
    /// </summary>
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing), typeof(double), typeof(UniformMinColumnPanel), new PropertyMetadata(0d));

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        Size panelSize = new(availableSize.Width, 0);
        if (Children.Count == 0)
            return panelSize;

        var maxColumnWidth = (availableSize.Width - (Spacing * (Children.Count - 1))) / Children.Count;
        var availableChildSize = new Size(maxColumnWidth, availableSize.Height);

        // Determine minimum width for all columns to be the same width
        _actualColumnWidth = 0.0;
        foreach (var child in Children)
        {
            child.Measure(availableChildSize);

            if (child.DesiredSize.Width > _actualColumnWidth)
                _actualColumnWidth = child.DesiredSize.Width;

            if (child.DesiredSize.Height > panelSize.Height)
                panelSize.Height = child.DesiredSize.Height;
        }

        panelSize.Width = (_actualColumnWidth * Children.Count) + (Spacing * (Children.Count - 1));

        return panelSize;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var columnOffset = _actualColumnWidth + Spacing;

        for (var i = 0; i < Children.Count; i++)
        {
            // Place child
            Point anchorPoint = new(i * columnOffset, 0);

            var child = Children[i];
            var childSize = child.DesiredSize;
            childSize.Width = _actualColumnWidth;

            child.Arrange(new Rect(anchorPoint, childSize));
        }

        return finalSize;
    }
}
