using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Converters;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties
{
    //http://social.msdn.microsoft.com/Forums/en-SG/wpf/thread/edd335ea-e5e1-48e1-91a2-793d613f5cc3
    public static class TrulyObservableListView
    {
        public static readonly DependencyProperty HasBindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached("HasBindableSelectedItems", typeof(bool),
                typeof(System.Windows.Controls.ListView), new PropertyMetadata(false));

        private static readonly DependencyProperty SelectionChangedHandlerProperty =
            DependencyProperty.RegisterAttached("SelectionChangedHandler", typeof(SelectionChangedHandler), typeof(System.Windows.Controls.ListView));

        public static readonly DependencyProperty BindableTrulyObservableSelectedItemsProperty =
            DependencyProperty.Register("BindableTrulyObservableSelectedItems", typeof(IList), typeof(System.Windows.Controls.ListView), new PropertyMetadata(PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var owner = (System.Windows.Controls.ListView)dependencyObject;

            var newValue = (TrulyObservableCollection<INotifyPropertyChanged>) dependencyPropertyChangedEventArgs.NewValue;
            if (newValue != null)
                newValue.CollectionChanged += (sender, notifyCollectionChangedEventArgs) =>
                    {
                        var bindingExpression = owner.GetBindingExpression(BindableTrulyObservableSelectedItemsProperty);
                        if (bindingExpression != null)
                            bindingExpression.UpdateTarget();
                    };
        }

        public static void SetHasBindableSelectedItems(System.Windows.Controls.ListView source, bool value)
        {
            var handler = (SelectionChangedHandler)source.GetValue(SelectionChangedHandlerProperty);

            if (value && handler == null)
            {
                handler = new SelectionChangedHandler(source);
                source.SetValue(SelectionChangedHandlerProperty, handler);
            }
            else if (!value && handler != null)
            {
                source.ClearValue(SelectionChangedHandlerProperty);
            }
        }

        internal class SelectionChangedHandler
        {
            private readonly Binding _binding;

            internal SelectionChangedHandler(System.Windows.Controls.ListView owner)
            {
                _binding = new Binding("SelectedItems")
                {
                    Source = owner,
                    Converter = new ObservableCollectionToTrulyObservableCollectionCoverter()
                };

                owner.SetBinding(BindableTrulyObservableSelectedItemsProperty, _binding);
                owner.SelectionChanged += OwnerSelectionChanged;
            }

            private void OwnerSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var owner = (System.Windows.Controls.ListView)sender;

                BindingOperations.ClearBinding(owner, BindableTrulyObservableSelectedItemsProperty);

                owner.SetBinding(BindableTrulyObservableSelectedItemsProperty, _binding);
            }
        }
    }
}




