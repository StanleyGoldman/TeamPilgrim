using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters
{
    public class WorkItemQueryFolderModelImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileName = "folder_Closed_16xLG.png";

            var workItemQueryFolderModel = (WorkItemQueryFolderModel)value;
            if (workItemQueryFolderModel.QueryFolderType.HasValue)
            {
                switch (workItemQueryFolderModel.QueryFolderType.Value)
                {
                    case QueryFolderTypeEnum.MyQueries:
                        fileName = "user_16xLG.png";
                        break;

                    case QueryFolderTypeEnum.TeamQueries:
                        fileName = "Team_16xLG.png";
                        break;
                }
            }

            var uriSource = new Uri(string.Format(@"/TeamPilgrim.VisualStudio;component/Resources\{0}", fileName), UriKind.Relative);
            return new BitmapImage(uriSource);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}