using CS2WPF.Helpers.UI;
using System.Collections.Generic;

namespace CS2WPF.Model
{
    public class ModelViewUniqueKey : NotifyPropertyChangedViewModel
    {
        #region Fiedls
        protected string _UniqueKeyName;
        protected bool _IsPrimary;
        protected InfoSourceEnum _KeySource = InfoSourceEnum.ByOnModelCreating;
        protected List<ModelViewKeyProperty> _UniqueKeyProperties;
        #endregion

        public string UniqueKeyName
        {
            get
            {
                return _UniqueKeyName;
            }
            set
            {
                if (_UniqueKeyName != value)
                {
                    _UniqueKeyName = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsPrimary
        {
            get
            {
                return _IsPrimary;
            }
            set
            {
                if (_IsPrimary != value)
                {
                    _IsPrimary = value;
                    OnPropertyChanged();
                }
            }
        }
        public InfoSourceEnum KeySource
        {
            get
            {
                return _KeySource;
            }
            set
            {
                if (_KeySource != value)
                {
                    _KeySource = value;
                    OnPropertyChanged();
                    OnPropertyChanged("KeySourceDisplay");
                }
            }
        }
        public string KeySourceDisplay
        {
            get { return KeySource.ToString("g"); }
        }
        public List<ModelViewKeyProperty> UniqueKeyProperties
        {
            get
            {
                return _UniqueKeyProperties;
            }
            set
            {
                if (_UniqueKeyProperties != value)
                {
                    _UniqueKeyProperties = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
