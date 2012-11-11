namespace JustAProgrammer.TeamPilgrim.VisualStudio.Messages
{
    public class ShowPendingChangesTabItemMessage
    {
        public ShowPendingChangesTabItemEnum ShowPendingChangesTabItem { get; set; }
    }

    public enum ShowPendingChangesTabItemEnum
    {
        SourceFiles,
        WorkItems,
        CheckinNotes,
        PolicyWarnings
    }
}
