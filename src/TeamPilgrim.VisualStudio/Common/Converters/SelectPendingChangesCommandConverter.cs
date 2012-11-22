using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class SelectPendingChangesCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var observableCollection = value != null ? 
                ((TrulyObservableCollection<INotifyPropertyChanged>)value)
                .Cast<PendingChangeModel>()
                .ToArray() : null;

            var workItemModel =
                observableCollection != null &&
                observableCollection.Any()
                && (observableCollection
                        .Select(model => !model.IncludeChange)
                        .Last());

            return new WorkspaceServiceModel.SelectPendingChangesCommandArgument
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
