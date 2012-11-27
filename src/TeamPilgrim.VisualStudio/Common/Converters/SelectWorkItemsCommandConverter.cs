using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class SelectWorkItemsCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var observableCollection = value != null ?
                ((TrulyObservableCollection<INotifyPropertyChanged>)value)
                .Cast<WorkItemModel>()
                .ToArray() : null;

            var workItemModel =
                observableCollection != null &&
                observableCollection.Any()
                && (observableCollection
                        .Select(model => !model.IsSelected)
                        .Last());

            return new SelectWorkItemsCommandArgument
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