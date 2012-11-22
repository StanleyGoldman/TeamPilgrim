using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class ObservableCollectionToTrulyObservableCollectionCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var collection = ((ObservableCollection<object>) value).Cast<INotifyPropertyChanged>().ToList();
            return new TrulyObservableCollection<INotifyPropertyChanged>(collection);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
