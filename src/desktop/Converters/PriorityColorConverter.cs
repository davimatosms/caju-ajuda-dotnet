using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CajuAjuda.Desktop.Converters
{
    public class PriorityColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not string prioridade)
                return Colors.Gray;

            return prioridade.ToUpper() switch
            {
                "BAIXA" => Color.FromArgb("#28a745"),      // Verde
                "MEDIA" or "MÉDIA" => Color.FromArgb("#ffc107"), // Amarelo
                "ALTA" => Color.FromArgb("#fd7e14"),       // Laranja
                "URGENTE" or "CRÍTICA" => Color.FromArgb("#dc3545"), // Vermelho
                _ => Colors.Gray
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
