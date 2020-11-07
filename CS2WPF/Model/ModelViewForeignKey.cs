using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewForeignKey : NotifyPropertyChangedViewModel
    {
        #region Fiedls
        string _NavigationName;
        string _InverseNavigationName;
        string _EntityName;
        string _EntityFullName;
        string _EntityUniqueProjectName;
        string _NavigationEntityName;
        string _NavigationEntityFullName;
        string _NavigationEntityUniqueProjectName;
        NavigationTypeEnum _NavigationType;
        InfoSourceEnum _ForeignKeySource = InfoSourceEnum.ByConvention;
        InfoSourceEnum _PrincipalKeySource = InfoSourceEnum.ByConvention;
        InfoSourceEnum _InverseNavigationSource = InfoSourceEnum.ByConvention;
        bool _IsCascadeDelete;
        string _ViewName;
        string _ForeignKeyPrefix;
        ObservableCollection<ModelViewProperty> _ScalarProperties;
        List<ModelViewKeyProperty> _PrincipalKeyProps;
        List<ModelViewKeyProperty> _ForeignKeyProps;
        bool _IsAssinging = false;
        #endregion

        public ModelViewForeignKey() { }
        public ModelViewForeignKey(bool isAssinging) 
        {
            IsAssinging = isAssinging;
        }
        public bool IsAssinging
        {
            get
            {
                return _IsAssinging;
            }
            set
            {
                if (_IsAssinging != value)
                {
                    _IsAssinging = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NavigationName
        {
            get
            {
                return _NavigationName;
            }
            set
            {
                if (_NavigationName != value)
                {
                    _NavigationName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string InverseNavigationName
        {
            get
            {
                return _InverseNavigationName;
            }
            set
            {
                if (_InverseNavigationName != value)
                {
                    _InverseNavigationName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                if (_EntityName != value)
                {
                    _EntityName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EntityFullName
        {
            get
            {
                return _EntityFullName;
            }
            set
            {
                if (_EntityFullName != value)
                {
                    _EntityFullName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EntityUniqueProjectName
        {
            get
            {
                return _EntityUniqueProjectName;
            }
            set
            {
                if (_EntityUniqueProjectName != value)
                {
                    _EntityUniqueProjectName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NavigationEntityName
        {
            get
            {
                return _NavigationEntityName;
            }
            set
            {
                if (_NavigationEntityName != value)
                {
                    _NavigationEntityName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NavigationEntityFullName
        {
            get
            {
                return _NavigationEntityFullName;
            }
            set
            {
                if (_NavigationEntityFullName != value)
                {
                    _NavigationEntityFullName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NavigationEntityUniqueProjectName
        {
            get
            {
                return _NavigationEntityUniqueProjectName;
            }
            set
            {
                if (_NavigationEntityUniqueProjectName != value)
                {
                    _NavigationEntityUniqueProjectName = value;
                    OnPropertyChanged();
                }
            }
        }
        public NavigationTypeEnum NavigationType
        {
            get
            {
                return _NavigationType;
            }
            set
            {
                if (_NavigationType != value)
                {
                    _NavigationType = value;
                    OnPropertyChanged();
                }
            }
        }
        public InfoSourceEnum ForeignKeySource
        {
            get
            {
                return _ForeignKeySource;
            }
            set
            {
                if (_ForeignKeySource != value)
                {
                    _ForeignKeySource = value;
                    OnPropertyChanged();
                }
            }
        }
        public InfoSourceEnum PrincipalKeySource
        {
            get
            {
                return _PrincipalKeySource;
            }
            set
            {
                if (_PrincipalKeySource != value)
                {
                    _PrincipalKeySource = value;
                    OnPropertyChanged();
                }
            }
        }
        public InfoSourceEnum InverseNavigationSource
        {
            get
            {
                return _InverseNavigationSource;
            }
            set
            {
                if (_InverseNavigationSource != value)
                {
                    _InverseNavigationSource = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<ModelViewKeyProperty> PrincipalKeyProps
        {
            get
            {
                return _PrincipalKeyProps;
            }
            set
            {
                if (_PrincipalKeyProps == value) return;
                _PrincipalKeyProps = value;
                OnPropertyChanged();
            }
        }
        public List<ModelViewKeyProperty> ForeignKeyProps
        {
            get
            {
                return _ForeignKeyProps;
            }
            set
            {
                if (_ForeignKeyProps == value) return;
                _ForeignKeyProps = value;
                OnPropertyChanged();
            }
        }
        public bool IsCascadeDelete
        {
            get
            {
                return _IsCascadeDelete;
            }
            set
            {
                if (_IsCascadeDelete != value)
                {
                    _IsCascadeDelete = value;
                    OnPropertyChanged();
                }
            }
        }
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
                    if (IsAssinging) return;
                    if(ScalarProperties != null)
                    {
                        ScalarProperties.Clear();
                    }
                    ForeignKeyPrefix = NavigationName + _ViewName;
                }
            }
        }
        public string ForeignKeyPrefix
        {
            get
            {
                return _ForeignKeyPrefix;
            }
            set
            {
                if (_ForeignKeyPrefix != value)
                {
                    _ForeignKeyPrefix = value;
                    OnPropertyChanged();
                    if (IsAssinging) return;
                    this.OnModelViewForeignKeyPrefixChanged();
                }
            }
        }
        public ObservableCollection<ModelViewProperty> ScalarProperties 
        { 
            get
            {
                return _ScalarProperties;
            }
            set
            {
                if (_ScalarProperties == value) return;
                _ScalarProperties = value;
                OnPropertyChanged();
            } 
        }
    }
}
