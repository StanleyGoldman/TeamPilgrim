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

            return GetChangeTypeDescription(pendingChange.ChangeType, pendingChange.ChangeTypeName);
        }

        public static string GetChangeTypeDescription(ChangeType changeType, string changeTypeName)
        {
            var isLock = (changeType & ChangeType.Lock) == ChangeType.Lock;
            var isAdd = (changeType & ChangeType.Add) == ChangeType.Add;
            var isDelete = (changeType & ChangeType.Delete) == ChangeType.Delete;
            var isEdit = (changeType & ChangeType.Edit) == ChangeType.Edit;

            if (isAdd && isLock)
            {
                return "add, lock";
            }

            if (isDelete && isLock)
            {
                return "delete, lock";
            }
            
            if (!isAdd && isLock && isEdit)
            {
                return "lock, edit";
            }

            return changeTypeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
