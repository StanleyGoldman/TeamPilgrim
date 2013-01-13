using System.Reflection;
using JustAProgrammer.TeamPilgrim.VisualStudio;

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
