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
    public class SelectedPendingChangesSummaryEnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((WorkspaceServiceModel.SelectedPendingChangesSummaryEnum)value)
            {
                case WorkspaceServiceModel.SelectedPendingChangesSummaryEnum.All:
                    return true;

                case WorkspaceServiceModel.SelectedPendingChangesSummaryEnum.Some:
                    return null;

                case WorkspaceServiceModel.SelectedPendingChangesSummaryEnum.None:
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
