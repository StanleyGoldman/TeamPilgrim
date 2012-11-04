using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class OcClauseBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result;

            if (values.Length == 0)
            {
                result = bool.Parse((string)parameter);
            }
            else
            {
                result = values.Cast<bool>().Any(b => b);
            }

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
