using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewProperty : ModelViewBaseProperty
    {
        #region Fileds
        protected bool _IsSelected;
        protected string _ForeignKeyName;
        protected string _ForeignKeyNameChain;
        protected string _EditableViewPropertyName;
        protected bool _IsRequiredInView;
        protected string _EditableJsonPropertyName;
        protected ObservableCollection<ModelViewAttribute> _Attributes;
        protected ObservableCollection<ModelViewFAPIAttribute> _FAPIAttributes;
        #endregion
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected == value) return;
                _IsSelected = value;
                OnPropertyChanged();
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
                if (_ForeignKeyName == value) return;
                _ForeignKeyName = value;
                OnPropertyChanged();
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
                if (_ForeignKeyNameChain == value) return;
                _ForeignKeyNameChain = value;
                OnPropertyChanged();
            }
        }
        public string EditableViewPropertyName
        {
            get
            {
                return _EditableViewPropertyName;
            }
            set
            {
                if (_EditableViewPropertyName == value) return;
                _EditableViewPropertyName = value;
                OnPropertyChanged();
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
                if (_IsRequiredInView == value) return;
                _IsRequiredInView = value;
                OnPropertyChanged();
            }
        }
        public string EditableJsonPropertyName
        {
            get
            {
                return _EditableJsonPropertyName;
            }
            set
            {
                if (_EditableJsonPropertyName == value) return;
                _EditableJsonPropertyName = value;
                OnPropertyChanged();
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
