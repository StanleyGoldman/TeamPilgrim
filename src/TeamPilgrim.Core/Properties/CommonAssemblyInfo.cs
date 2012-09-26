using System.Reflection;
using Bootstrap.Common.Core;

[assembly: AssemblyProduct("TeamPilgrim")]

[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else // RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion(VersionInfo.Full)]
[assembly: AssemblyFileVersion(VersionInfo.Full)]
[assembly: AssemblyInformationalVersion(VersionInfo.FullInformational)]
