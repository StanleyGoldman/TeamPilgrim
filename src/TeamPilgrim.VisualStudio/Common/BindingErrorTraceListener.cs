using System.Diagnostics;
using System.Text;
using System.Windows;
using NLog;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    //http://www.switchonthecode.com/tutorials/wpf-snippet-detecting-binding-errors
    public class BindingErrorTraceListener : DefaultTraceListener
    {
        private static BindingErrorTraceListener _listener;

        public static void SetTrace()
        { SetTrace(SourceLevels.Error, TraceOptions.None); }

        public static void SetTrace(SourceLevels level, TraceOptions options)
        {
            if (_listener == null)
            {
                _listener = new BindingErrorTraceListener();
                PresentationTraceSources.DataBindingSource.Listeners.Add(_listener);
            }

            _listener.TraceOutputOptions = options;
            PresentationTraceSources.DataBindingSource.Switch.Level = level;
        }

        public static void CloseTrace()
        {
            if (_listener == null)
            { return; }

            _listener.Flush();
            _listener.Close();
            PresentationTraceSources.DataBindingSource.Listeners.Remove(_listener);
            _listener = null;
        }

        private readonly StringBuilder _message = new StringBuilder();

        private BindingErrorTraceListener()
        { }

        public override void Write(string message)
        { _message.Append(message); }

        public override void WriteLine(string message)
        {
            _message.Append(message);

            var final = _message.ToString();
            _message.Length = 0;

            this.Logger().Trace(final);
            MessageBox.Show(final, "Binding Error", MessageBoxButton.OK,
              MessageBoxImage.Error);
        }
    }
}
