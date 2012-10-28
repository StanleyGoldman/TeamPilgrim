using System.Windows;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.AttachedProperties
{
    public static class ActivateModel
    {
        public static readonly DependencyProperty ModelProperty = 
            DependencyProperty.RegisterAttached("Model", typeof(BaseModel), typeof(ActivateModel), 
            new PropertyMetadata(OnModelInvalidated));

        public static BaseModel GetModel(DependencyObject sender)
        {
            return (BaseModel)sender.GetValue(ModelProperty);
        }

        public static void SetModel(DependencyObject sender, BaseModel model)
        {
            sender.SetValue(ModelProperty, model);
        }

        private static void OnModelInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)dependencyObject;

            // Add handlers if necessary
            if (e.OldValue == null && e.NewValue != null)
            {
                element.Loaded += OnElementLoaded;
                element.Unloaded += OnElementUnloaded;
            }

            // Or, remove if necessary
            if (e.OldValue != null && e.NewValue == null)
            {
                element.Loaded -= OnElementLoaded;
                element.Unloaded -= OnElementUnloaded;
            }

            // If loaded, deactivate old model and activate new one
            if (element.IsLoaded)
            {
                if (e.OldValue != null)
                {
                    ((BaseModel)e.OldValue).Deactivate();
                }

                if (e.NewValue != null)
                {
                    ((BaseModel)e.NewValue).Activate();
                }
            }
        }

        static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var model = GetModel(element);
            model.Activate();
        }

        static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var model = GetModel(element);
            model.Deactivate();
        }
    }
}
