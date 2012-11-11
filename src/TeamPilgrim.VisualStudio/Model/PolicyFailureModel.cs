namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class PolicyFailureModel : BaseModel
    {
        private bool _override;
        public bool Override
        {
            get
            {
                return _override;
            }
            set
            {
                if (_override == value) return;

                _override = value;

                SendPropertyChanged("SelectedItem");
            }
        }
        
        private string _reason;
        public string Reason
        {
            get
            {
                return _reason;
            }
            set
            {
                if (_reason == value) return;

                _reason = value;

                SendPropertyChanged("Reason");
            }
        }
    }
}
