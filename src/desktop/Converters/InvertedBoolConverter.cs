// CajuAjuda.Desktop/Converters/InvertedBoolConverter.cs

using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CajuAjuda.Desktop.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        // Adicionado '?' aos tipos de objeto para compatibilidade
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not bool boolValue)
                return false;
            return !boolValue;
        }

        // Adicionado '?' aos tipos de objeto para compatibilidade
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not bool boolValue)
                return false;
            return !boolValue;
        }
    }
}