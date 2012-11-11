using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Model.WorkItemQuery;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Model
{
    public class LicenseModel : BaseModel
    {
        private string _licesnseKey;
        public string LicesnseKey
        {
            get
            {
                return _licesnseKey;
            }
            set
            {
                if (_licesnseKey == value) return;

                _licesnseKey = value;

                SendPropertyChanged("LicesnseKey");
            }
        }

        private DateTime _expirationDate;
        public DateTime ExpirationDate
        {
            get
            {
                return _expirationDate;
            }
            set
            {
                if (_expirationDate == value) return;

                _expirationDate = value;

                SendPropertyChanged("ExpirationDate");
            }
        }
    }
}
