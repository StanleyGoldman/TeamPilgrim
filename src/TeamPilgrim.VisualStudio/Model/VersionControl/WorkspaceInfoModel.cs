using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class WorkspaceInfoModel : BaseModel
    {
        public WorkspaceInfo WorkspaceInfo { get; private set; }

        public WorkspaceInfoModel(WorkspaceInfo workspaceInfo)
        {
            WorkspaceInfo = workspaceInfo;
        }
    }
}