using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class SelectedPendingChangesSummaryEnumIsCheckedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((CollectionSelectionSummaryEnum)value)
            {
                case CollectionSelectionSummaryEnum.All:
                    return true;

                case CollectionSelectionSummaryEnum.Some:
                    return null;

                case CollectionSelectionSummaryEnum.None:
                    return false;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
