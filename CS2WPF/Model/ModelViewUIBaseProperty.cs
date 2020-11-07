using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewUIBaseProperty : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _OriginalPropertyName;
        protected string _ViewPropertyName;
        protected string _JsonPropertyName;
        protected string _ForeignKeyName;
        protected string _ForeignKeyNameChain;
        protected bool _IsShownInView;
        protected bool _IsNewLineAfter;
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
        public bool IsShownInView
        {
            get
            {
                return _IsShownInView;
            }
            set
            {
                if (_IsShownInView == value) return;
                _IsShownInView = value;
                OnPropertyChanged();
            }
        }
        public bool IsNewLineAfter
        {
            get
            {
                return _IsNewLineAfter;
            }
            set
            {
                if (_IsNewLineAfter == value) return;
                _IsNewLineAfter = value;
                OnPropertyChanged();
            }
        }

    }
}
