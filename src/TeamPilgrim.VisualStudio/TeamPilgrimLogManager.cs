using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    internal class TeamPilgrimLogManager
    {
        public static readonly LogFactory Instance = new LogFactory(new XmlLoggingConfiguration(GetNLogConfigFilePath()));

        private static string GetNLogConfigFilePath()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            return Path.Combine(Path.GetDirectoryName(thisAssembly.Location), @"NLog.config");
        }
    }
}
