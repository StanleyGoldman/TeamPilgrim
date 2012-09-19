using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JustAProgrammer.TeamPilgrim.VisualStudio.Command;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    //http://blogs.msdn.com/b/dancre/archive/2006/09/15/dm-v-vm-part-7-encapsulating-commands.aspx

    public static class CreateCommandBinding
    {
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.RegisterAttached("Command", typeof(CommandModel), typeof(CreateCommandBinding), new PropertyMetadata(OnCommandInvalidated));

        public static CommandModel GetCommand(DependencyObject sender)
        {
            return (CommandModel)sender.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject sender, CommandModel command)
        {
            sender.SetValue(CommandProperty, command);
        }

        private static void OnCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            var element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();

            // If we're given a command model, set up a binding
            var commandModel = e.NewValue as CommandModel;
            if (commandModel != null)
            {
                element.CommandBindings.Add(new CommandBinding(commandModel.Command, commandModel.OnExecute, commandModel.OnQueryEnabled));
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
