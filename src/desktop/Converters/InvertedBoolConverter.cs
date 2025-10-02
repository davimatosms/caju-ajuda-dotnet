// CajuAjuda.Desktop/Converters/InvertedBoolConverter.cs

using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CajuAjuda.Desktop.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return false;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return false;
            return !boolValue;
        }
    }
}