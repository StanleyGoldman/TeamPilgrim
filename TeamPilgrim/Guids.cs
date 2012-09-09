// Guids.cs
// MUST match guids.h
using System;

namespace JustAProgrammer.TeamPilgrim
{
    static class GuidList
    {
        public const string guidTeamPilgrimPkgString = "f56b8666-3ef2-46e5-ac26-8aa0efe1f484";
        public const string guidTeamPilgrimCmdSetString = "064ac9aa-5aa9-498a-a5d9-2077be8d9ad6";
        public const string guidToolWindowPersistanceString = "62e88c94-e7e0-4bdc-98ff-1e308fcda7fa";

        public static readonly Guid guidTeamPilgrimCmdSet = new Guid(guidTeamPilgrimCmdSetString);
    };
}