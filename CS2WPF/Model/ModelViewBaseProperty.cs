using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewBaseProperty : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _OriginalPropertyName;
        protected string _TypeFullName;
        protected bool _IsNullable;
        protected bool _IsRequired;
        protected string _UnderlyingTypeName;
        protected string _ViewPropertyName;
        protected string _JsonPropertyName;
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
        public string ViewPropertyName
        {
            get
            {
                return _ViewPropertyName;
            }
            set
            {
                if (_ViewPropertyName == value) return;
                _ViewPropertyName = value;
                OnPropertyChanged();
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
                if (_JsonPropertyName == value) return;
                _JsonPropertyName = value;
                OnPropertyChanged();
            }
        }

    }
}
