using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;
using NLog;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class SelectPendingChangesCommandConverter : IValueConverter
    {
        private static readonly Logger Logger = TeamPilgrimLogManager.Instance.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Logger.Trace("SelectPendingChangesCommandConverter Convert");

            var observableCollection = value != null ? 
                ((TrulyObservableCollection<INotifyPropertyChanged>)value)
                .Cast<PendingChangeModel>()
                .ToArray() : null;

            var workItemModel =
                observableCollection != null
                && observableCollection.Any()
                && (!observableCollection.Last().IncludeChange);

            return new SelectPendingChangesCommandArgument
                {
                    Collection = observableCollection,
                    Value = workItemModel
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
