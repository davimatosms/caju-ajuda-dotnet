using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CajuAjuda.Desktop.Converters
{
    public class TabColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not int selectedTab || parameter is not string tabIndexStr)
                return Colors.Transparent;

            if (!int.TryParse(tabIndexStr, out int tabIndex))
                return Colors.Transparent;

            // Retorna branco se for a aba selecionada, caso contrário transparente
            return selectedTab == tabIndex ? Colors.White : Color.FromArgb("#F5F5F5");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
