using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewAttributeProperty : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _PropName = "";
        protected string _PropValue = "";
        #endregion
        public string PropName
        {
            get
            {
                return _PropName;
            }
            set
            {
                if (_PropName != value)
                {
                    _PropName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PropValue
        {
            get
            {
                return _PropValue;
            }
            set
            {
                if (_PropValue != value)
                {
                    _PropValue = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
