using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model.VersionControl
{
    public class ShelvesetModel: BaseModel
    {
        public Shelveset Shelveset { get; set; }

        public ShelvesetModel(Shelveset shelveset)
        {
            Shelveset = shelveset;
        }
    }
}
