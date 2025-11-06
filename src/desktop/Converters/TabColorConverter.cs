using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CajuAjuda.Desktop.Converters
{
    /// <summary>
    /// Conversor para cor de fundo das abas
    /// Aplica affordance visual (Heurística #6: Reconhecimento vs Lembrança)
    /// </summary>
    public class TabColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not int selectedTab || parameter is not string tabIndexStr)
                return Colors.Transparent;

            if (!int.TryParse(tabIndexStr, out int tabIndex))
                return Colors.Transparent;

            // Feedback visual claro: branco para selecionado, cinza claro para não selecionado
            return selectedTab == tabIndex 
                ? Application.Current?.Resources["CardBackground"] as Color ?? Colors.White
                : Application.Current?.Resources["Gray100"] as Color ?? Color.FromArgb("#F5F5F5");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
