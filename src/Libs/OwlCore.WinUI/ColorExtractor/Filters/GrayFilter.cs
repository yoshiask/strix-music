﻿using OwlCore.WinUI.ColorExtractor.ColorSpaces;
using Windows.UI;

namespace OwlCore.WinUI.ColorExtractor.Filters
{
    public class GrayFilter : IFilter
    {
        public GrayFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        public float Tolerance { get; set; } = .3f;

        public RGBColor Clamp(RGBColor color)
        {
            if (color.GetSaturation() < Tolerance)
            {
                return Clamp(color.ToHsv()).ToRgb();
            }
            return color;
        }

        public HSVColor Clamp(HSVColor color)
        {
            if (color.S < Tolerance)
            {
                color.S = Tolerance;
            }
            return color;
        }

        public bool TakeColor(RGBColor color)
        {
            return color.GetSaturation() > Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return color.S > Tolerance;
        }
    }
}
