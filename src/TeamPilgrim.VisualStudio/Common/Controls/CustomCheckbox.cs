using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Controls
{
    public class CustomCheckBox : CheckBox
    {
        public static readonly RoutedEvent PreviewCheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomCheckBox));

        protected override void OnToggle()
        {
            var value = IsChecked == null || !IsChecked.Value;

            SetCurrentValue(IsCheckedProperty, value);
        }
    }
}
