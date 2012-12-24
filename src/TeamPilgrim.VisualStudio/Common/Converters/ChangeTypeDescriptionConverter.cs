using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class ChangeTypeDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pendingChange = (PendingChange)value;

            var changeList = new List<string> { };

            if (pendingChange.IsAdd)
                changeList.Add("add");
            
            if (pendingChange.IsLock)
                changeList.Add("lock");

            if (pendingChange.IsEdit && !pendingChange.IsAdd)
                changeList.Add("edit");

            return (changeList.Count == 0) ? pendingChange.ChangeTypeName : string.Join(", ", changeList);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
