using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services
{
    public class CheckinNotesCacheWrapper
    {
        private static readonly Lazy<Type> CacheType;
        private static readonly Lazy<ConstructorInfo> Constructor;
        private static readonly Lazy<MethodInfo> GetCheckinNotesWithEnumerablePendingChange;
        private static readonly Lazy<MethodInfo> GetCheckinNotesWithStringArray;

        static CheckinNotesCacheWrapper()
        {
            CacheType = new Lazy<Type>(() => System.Type.GetType("Microsoft.TeamFoundation.VersionControl.Controls.CheckinNotesCache, Microsoft.TeamFoundation.VersionControl.Controls"));
            Constructor = new Lazy<ConstructorInfo>(() => CacheType.Value.GetConstructor(new Type[] { typeof(VersionControlServer) }));
            GetCheckinNotesWithEnumerablePendingChange = new Lazy<MethodInfo>(() => CacheType.Value.GetMethod("GetCheckinNotes", new[] { typeof(IEnumerable<PendingChange>) }, null));
            GetCheckinNotesWithStringArray = new Lazy<MethodInfo>(() => CacheType.Value.GetMethod("GetCheckinNotes", new[] { typeof(string[]) }, null));
        }

        private readonly object _cacheInstance;

        public CheckinNotesCacheWrapper(VersionControlServer versionControlServer)
        {
            _cacheInstance = Constructor.Value.Invoke(new object[] { versionControlServer });
        }

        public CheckinNoteFieldDefinition[] GetCheckinNotes(IEnumerable<PendingChange> pendingChanges)
        {
            return (CheckinNoteFieldDefinition[])GetCheckinNotesWithEnumerablePendingChange.Value.Invoke(_cacheInstance, new object[] { pendingChanges });
        }
        public CheckinNoteFieldDefinition[] GetCheckinNotes(string[] teamProjectPaths)
        {
            return (CheckinNoteFieldDefinition[])GetCheckinNotesWithStringArray.Value.Invoke(_cacheInstance, new object[] { teamProjectPaths });
        }
    }
}
