using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewFAPIAttribute : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _AttrName = "";
        protected ObservableCollection<ModelViewFAPIAttributeProperty> _VaueProperties;
        #endregion
        public string AttrName
        {
            get
            {
                return _AttrName;
            }
            set
            {
                if (_AttrName != value)
                {
                    _AttrName = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ModelViewFAPIAttributeProperty> VaueProperties
        {
            get
            {
                return _VaueProperties;
            }
            set
            {
                if (_VaueProperties != value)
                {
                    _VaueProperties = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
