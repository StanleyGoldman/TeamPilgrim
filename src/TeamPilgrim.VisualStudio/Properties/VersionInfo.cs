// <autogenerated>
// This file is auto-generated via HgVersionFile.
// </autogenerated>

using System;

// ReSharper disable CheckNamespace
namespace JustAProgrammer.TeamPilgrim.VisualStudio
// ReSharper restore CheckNamespace
{
    public static class VersionInfo
    {
	
#if DEBUG
		public const bool IsDebug = true;
#else // RELEASE
		public const bool IsDebug = false;
#endif

        /// <summary>
        /// The major version number for this assembly.
        /// </summary>
        public const string Major = "0";

        /// <summary>
        /// The minor version number for this assembly.
        /// </summary>
        public const string Minor = "2";

        /// <summary>
        /// The patch version number for this assembly.
        /// </summary>
        public const string Patch = "3";

        /// <summary>
        /// The changeset for this assembly.
        /// </summary>
        public const string Changeset = "";

        /// <summary>
        /// The shortened changeset for this assembly.
        /// </summary>
        public const string ChangesetShort = "";

        /// <summary>
        /// A boolean value indicating if we are runnning a dirty build. Returns <c>true</c> if the folder contains uncommitted changes; otherwise, <c>false</c>.
        /// </summary>
        public const bool IsDirtyBuild = false;

        /// <summary>
        /// A string value indicating if we are running a debug build. Returns <c>+debug</c> if debug build; otherwise, empty.
        /// </summary>
#if DEBUG
        public const string DebugBuild = "+debug";
#else // RELEASE
        public const string DebugBuild = "";
#endif

        /// <summary>
        /// A string value indicating if we are runnning a dirty build. Returns <c>+dirty</c> if the folder contains uncommitted changes; otherwise, empty.
        /// </summary>
#if false
        public const string DirtyBuild = "+dirty";
#else // false
        public const string DirtyBuild = "";
#endif

        /// <summary>
        /// The pre-release string of this assembly.
        /// </summary>
        public const string PreRelease = "-alpha"; // -alpha, -beta, -rc

        /// <summary>
        /// The build version string of this assembly.
        /// </summary>
        public const string BuildVersion = "+build.";

        /// <summary>
        /// The full version for this assembly.
        /// </summary>
        public const string Full = Major + "." + Minor + "." + Patch;

        /// <summary>
        /// The full informational version for this assembly using Semantic Versioning.
        /// </summary>
        /// <see href="http://semver.org/" />
        public const string FullInformational = Full + PreRelease + BuildVersion;
    }
}