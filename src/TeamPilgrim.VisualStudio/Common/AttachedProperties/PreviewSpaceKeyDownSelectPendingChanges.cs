using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.CommandArguments;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.PendingChanges;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties
{
    public class PreviewSpaceKeyDownSelectPendingChanges
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                                                typeof(RelayCommand<SelectPendingChangesCommandArgument>), typeof(PreviewSpaceKeyDownSelectPendingChanges), new UIPropertyMetadata(CommandChanged));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var listView = (ListView)target;
            if (listView != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    listView.PreviewKeyDown += OnPreviewKeyDown;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    listView.PreviewKeyDown -= OnPreviewKeyDown;
                }
            }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space)
                return;

            e.Handled = true;
            var listView = (ListView)sender;
            var command = (RelayCommand<SelectPendingChangesCommandArgument>)listView.GetValue(CommandProperty);

            var selectedItems = listView.SelectedItems.Cast<PendingChangeModel>().ToArray();

            var workItemModel =
                selectedItems.Any()
                && (!selectedItems.Last().IncludeChange);

            var selectPendingChangesCommandArgument = new SelectPendingChangesCommandArgument
                {
                    Collection = selectedItems, Value = workItemModel
                };

            command.Execute(selectPendingChangesCommandArgument);
        }
    }
}