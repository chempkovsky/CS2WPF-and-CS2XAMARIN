using CS2WPF.Helpers.UI;
using CS2WPF.Model.AnalyzeOnModelCreating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelView: NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _ViewName="";
        protected string _RootEntityClassName = "";
        protected string _RootEntityFullClassName = "";
        protected string _RootEntityUniqueProjectName = "";
        protected string _DestinationProject = "";
        protected string _DefaultProjectNameSpace = "";
        protected string _DestinationFolder = "";
        protected bool   _GenerateJSonAttribute;
        protected string _RootEntityDbContextPropertyName;
        protected string _PageViewName;
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
        public string PageViewName
        {
            get
            {
                return _PageViewName;
            }
            set
            {
                if (_PageViewName != value)
                {
                    _PageViewName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityDbContextPropertyName
        {
            get
            {
                return _RootEntityDbContextPropertyName;
            }
            set
            {
                if (_RootEntityDbContextPropertyName != value)
                {
                    _RootEntityDbContextPropertyName = value;
                    OnPropertyChanged();
                }
            }
        }
        

        public string RootEntityClassName
        {
            get
            {
                return _RootEntityClassName;
            }
            set
            {
                if (_RootEntityClassName != value)
                {
                    _RootEntityClassName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityFullClassName
        {
            get
            {
                return _RootEntityFullClassName;
            }
            set
            {
                if (_RootEntityFullClassName != value)
                {
                    _RootEntityFullClassName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityUniqueProjectName
        {
            get
            {
                return _RootEntityUniqueProjectName;
            }
            set
            {
                if (_RootEntityUniqueProjectName != value)
                {
                    _RootEntityUniqueProjectName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViewProject
        {
            get
            {
                return _DestinationProject;
            }
            set
            {
                if (_DestinationProject != value)
                {
                    _DestinationProject = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViewDefaultProjectNameSpace
        {
            get
            {
                return _DefaultProjectNameSpace;
            }
            set
            {
                if (_DefaultProjectNameSpace != value)
                {
                    _DefaultProjectNameSpace = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViewFolder
        {
            get
            {
                return _DestinationFolder;
            }
            set
            {
                if (_DestinationFolder != value)
                {
                    _DestinationFolder = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool GenerateJSonAttribute
        {
            get
            {
                return _GenerateJSonAttribute;
            }
            set
            {
                if (_GenerateJSonAttribute != value)
                {
                    _GenerateJSonAttribute = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ModelViewProperty> ScalarProperties { get; set; }
        public ObservableCollection<ModelViewForeignKey> ForeignKeys { get; set; }
        public ObservableCollection<ModelViewKeyProperty> PrimaryKeyProperties { get; set; }
        public ObservableCollection<ModelViewEntityProperty> AllProperties { get; set; }
        public ObservableCollection<ModelViewUIFormProperty> UIFormProperties { get; set; }
        public ObservableCollection<ModelViewUIListProperty> UIListProperties { get; set; }

    }
}
