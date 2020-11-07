using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewFAPIAttributeProperty : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _PropValue = "";
        #endregion
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
