using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace VortexTracker.Avalonia.Converters;

public class BoolToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.FromRgb(0, 122, 204)); // #007ACC
        }
        return new SolidColorBrush(Colors.Transparent);
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
