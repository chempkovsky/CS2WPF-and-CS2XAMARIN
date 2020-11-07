using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model.Serializable.UI
{
    public class ModelViewPropertyOfVwNotified: NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _OriginalPropertyName;
        protected string _TypeFullName;
        protected bool _IsNullable;
        protected bool _IsRequired;
        protected bool _IsRequiredInView;
        protected string _UnderlyingTypeName;
        protected bool _IsSelected;
        protected string _ForeignKeyName;
        protected string _ForeignKeyNameChain;
        protected string _ViewPropertyName;
        protected string _JsonPropertyName;
        protected bool _IsUsedByfilter;
        protected bool _IsUsedBySorting;
        protected ObservableCollection<ModelViewAttribute> _Attributes;
        protected ObservableCollection<ModelViewFAPIAttribute> _FAPIAttributes;
        #endregion


        public string OriginalPropertyName
        {
            get
            {
                return _OriginalPropertyName;
            }
            set
            {
                if (_OriginalPropertyName != value)
                {
                    _OriginalPropertyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypeFullName
        {
            get
            {
                return _TypeFullName;
            }
            set
            {
                if (_TypeFullName != value)
                {
                    _TypeFullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNullable
        {
            get
            {
                return _IsNullable;
            }
            set
            {
                if (_IsNullable != value)
                {
                    _IsNullable = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsRequired
        {
            get
            {
                return _IsRequired;
            }
            set
            {
                if (_IsRequired != value)
                {
                    _IsRequired = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRequiredInView
        {
            get
            {
                return _IsRequiredInView;
            }
            set
            {
                if (_IsRequiredInView != value)
                {
                    _IsRequiredInView = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UnderlyingTypeName
        {
            get
            {
                return _UnderlyingTypeName;
            }
            set
            {
                if (_UnderlyingTypeName != value)
                {
                    _UnderlyingTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ForeignKeyName
        {
            get
            {
                return _ForeignKeyName;
            }
            set
            {
                if (_ForeignKeyName != value)
                {
                    _ForeignKeyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ForeignKeyNameChain
        {
            get
            {
                return _ForeignKeyNameChain;
            }
            set
            {
                if (_ForeignKeyNameChain != value)
                {
                    _ForeignKeyNameChain = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ViewPropertyName
        {
            get
            {
                return _ViewPropertyName;
            }
            set
            {
                if (_ViewPropertyName != value)
                {
                    _ViewPropertyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string JsonPropertyName
        {
            get
            {
                return _JsonPropertyName;
            }
            set
            {
                if (_JsonPropertyName != value)
                {
                    _JsonPropertyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsUsedByfilter
        {
            get
            {
                return _IsUsedByfilter;
            }
            set
            {
                if (_IsUsedByfilter != value)
                {
                    _IsUsedByfilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsUsedBySorting
        {
            get
            {
                return _IsUsedBySorting;
            }
            set
            {
                if (_IsUsedBySorting != value)
                {
                    _IsUsedBySorting = value;
                    OnPropertyChanged();
                }
            }
        }

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
