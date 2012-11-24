using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class BooleanToAlternateFontWeightConverter : IValueConverter
    {
        public FontWeight FalseWeight { get; set; }

        public FontWeight TrueWeight { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueWeight : FalseWeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
