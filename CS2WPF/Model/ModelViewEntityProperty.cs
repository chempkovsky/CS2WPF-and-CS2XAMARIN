using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewEntityProperty : ModelViewKeyProperty
    {
        protected ObservableCollection<ModelViewAttribute> _Attributes;
        protected ObservableCollection<ModelViewFAPIAttribute> _FAPIAttributes;
        public ObservableCollection<ModelViewAttribute> Attributes
        {
            get
            {
                return _Attributes;
            }
            set
            {
                if (_Attributes != value)
                {
                    _Attributes = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ModelViewFAPIAttribute> FAPIAttributes
        {
            get
            {
                return _FAPIAttributes;
            }
            set
            {
                if (_FAPIAttributes != value)
                {
                    _FAPIAttributes = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
