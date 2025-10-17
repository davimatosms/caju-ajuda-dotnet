using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CajuAjuda.Desktop.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not int selectedTab || parameter is not string tabIndexStr)
                return false;

            if (!int.TryParse(tabIndexStr, out int tabIndex))
                return false;

            return selectedTab == tabIndex;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
