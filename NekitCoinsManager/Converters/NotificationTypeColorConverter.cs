using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Converters;

public class NotificationTypeColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is NotificationType type)
        {
            var color = type switch
            {
                NotificationType.Error => Color.FromRgb(255, 68, 68),    // #FF4444
                NotificationType.Success => Color.FromRgb(120, 177, 89), // #78B159
                NotificationType.Warning => Color.FromRgb(255, 193, 7),  // #FFC107
                NotificationType.Info => Color.FromRgb(51, 51, 51),      // #333333
                _ => Color.FromRgb(51, 51, 51)
            };
            
            return new SolidColorBrush(color);
        }

        return new SolidColorBrush(Color.FromRgb(51, 51, 51));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 