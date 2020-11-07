using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class FeatureItem : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _ViewName;
        protected bool _IsSelected;
        protected string _FileType;
        #endregion

        public string ViewName
        {
            get
            {
                return _ViewName;
            }
            set
            {
                if (_ViewName != value)
                {
                    _ViewName = value;
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
        public string FileType
        {
            get
            {
                return _FileType;
            }
            set
            {
                if (_FileType != value)
                {
                    _FileType = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
