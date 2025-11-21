using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CajuAjuda.Desktop.Converters
{
    /// <summary>
    /// Conversor semântico para cores de prioridade
    /// Segue as heurísticas de Nielsen: Visibilidade do status e consistência
    /// </summary>
    public class PriorityColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not string prioridade)
                return Application.Current?.Resources["Gray500"] as Color ?? Colors.Gray;

            // Usa recursos globais para garantir consistência (Heurística #4)
            return prioridade.ToUpper() switch
            {
                "BAIXA" => Application.Current?.Resources["PrioridadeBaixa"] as Color ?? Color.FromArgb("#10B981"),
                "MEDIA" or "MÉDIA" => Application.Current?.Resources["PrioridadeMedia"] as Color ?? Color.FromArgb("#FFC107"),
                "ALTA" => Application.Current?.Resources["PrioridadeAlta"] as Color ?? Color.FromArgb("#FD7E14"),
                "URGENTE" or "CRÍTICA" or "CRITICA" => Application.Current?.Resources["PrioridadeUrgente"] as Color ?? Color.FromArgb("#DC3545"),
                _ => Application.Current?.Resources["Gray500"] as Color ?? Colors.Gray
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
