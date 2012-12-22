using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class SelectedPendingChangesSummaryEnumIsCheckedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PendingChangesSummaryEnum)value)
            {
                case PendingChangesSummaryEnum.All:
                    return true;

                case PendingChangesSummaryEnum.Some:
                    return null;

                case PendingChangesSummaryEnum.None:
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
