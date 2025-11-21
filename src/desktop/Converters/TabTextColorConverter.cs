using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CajuAjuda.Desktop.Converters
{
    public class TabTextColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not int selectedTab || parameter is not string tabIndexStr)
                return Colors.Gray;

            if (!int.TryParse(tabIndexStr, out int tabIndex))
                return Colors.Gray;

            // Retorna Primary color se for a aba selecionada, caso contrário cinza
            return selectedTab == tabIndex
                ? Application.Current?.Resources["Primary"] as Color ?? Colors.Blue
                : Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
