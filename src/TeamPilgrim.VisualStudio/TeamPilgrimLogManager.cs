using System.IO;
using System.Reflection;
using System.Windows;
using NLog;
using NLog.Config;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    internal class TeamPilgrimLogManager
    {
        public static readonly LogFactory Instance;

        static TeamPilgrimLogManager()
        {
            var designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            
            Instance = designTime 
                ? new LogFactory() 
                : new LogFactory(new XmlLoggingConfiguration(GetNLogConfigFilePath()));
        }

        private static string GetNLogConfigFilePath()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            return Path.Combine(Path.GetDirectoryName(thisAssembly.Location), @"NLog.config");
        }
    }
}
