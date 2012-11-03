using System;
using System.Reflection;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Business.Services.VisualStudio.WorkItem
{
    public class DocumentServiceWrapper
    {
        private readonly DocumentService _documentService;
        private readonly Type _documentServiceType;
        private readonly Lazy<MethodInfo> _getDefaultParentMethod;

        public DocumentServiceWrapper(DocumentService documentService)
        {
            _documentService = documentService;
            _documentServiceType = documentService.GetType();

            _getDefaultParentMethod = new Lazy<MethodInfo>(() => _documentServiceType.GetMethod("GetDefaultParent", BindingFlags.Static | BindingFlags.NonPublic));
        }

        public IQueryDocument GetQuery(TfsTeamProjectCollection tfsTeamProjectCollection, string queryId, object lockToken)
        {
            return _documentService.GetQuery(tfsTeamProjectCollection, queryId, lockToken);
        }

        public IResultsDocument GetLinkResults(IQueryDocument queryDocument, object lockToken)
        {
            return _documentService.GetLinkResults(queryDocument, lockToken);
        }

        public IResultsDocument CreateLinkResults(IQueryDocument queryDocument, object lockToken)
        {
            return _documentService.CreateLinkResults(queryDocument, lockToken);
        }

        public void ShowResults(IResultsDocument resultsDocument)
        {
            _documentService.ShowResults(resultsDocument);
        }

        public void ShowQuery(IQueryDocument queryDocument)
        {
            _documentService.ShowQuery(queryDocument);
        }

        public QueryFolder GetDefaultParent(Project project, bool isPublicQuery)
        {
            return (QueryFolder) _getDefaultParentMethod.Value.Invoke(null, new object[] {project, isPublicQuery});
        }
    }
}