using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CajuAjuda.Desktop.Converters
{
    /// <summary>
    /// Conversor semântico para cores de status de chamado
    /// Garante feedback visual claro (Heurística #1: Visibilidade do Status)
    /// </summary>
    public class StatusColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is not string status)
                return Application.Current?.Resources["Gray500"] as Color ?? Colors.Gray;

            return status.ToUpper() switch
            {
                "ABERTO" => Application.Current?.Resources["StatusAberto"] as Color ?? Color.FromArgb("#1976D2"),
                "EM_ANDAMENTO" or "EMANDAMENTO" or "EM ANDAMENTO" => Application.Current?.Resources["StatusAndamento"] as Color ?? Color.FromArgb("#FF8F00"),
                "AGUARDANDO_CLIENTE" or "AGUARDANDO" => Application.Current?.Resources["StatusAguardando"] as Color ?? Color.FromArgb("#FFC107"),
                "RESOLVIDO" => Application.Current?.Resources["StatusResolvido"] as Color ?? Color.FromArgb("#10B981"),
                "FECHADO" => Application.Current?.Resources["StatusFechado"] as Color ?? Color.FromArgb("#757575"),
                "CANCELADO" => Application.Current?.Resources["StatusCancelado"] as Color ?? Color.FromArgb("#DC3545"),
                _ => Application.Current?.Resources["Gray500"] as Color ?? Colors.Gray
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}