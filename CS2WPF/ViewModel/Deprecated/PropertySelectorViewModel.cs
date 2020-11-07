using CS2ANGULAR.Helpers.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CS2ANGULAR.ViewModel
{
    public class PropertySelectorViewModel : NotifyPropertyChangedViewModel
    {
        protected string _originalPropertyName = "";
        protected string _foreignKeyName = "";
        protected string _foreignKeyAlias = "";
        protected string _childForeignKeyPrefix = "";
        protected string _foreignKeyPrefix = "";
        protected bool _includeInView = false;

        public PropertySelectorViewModel()
        {
            UpdateDependent = false;
        }
        public PropertySelectorViewModel(bool updateDependent)
        {
            UpdateDependent = updateDependent;
        }

        public string PocoName { get; set; }
        public string PocoFullName { get; set; }
        public virtual string DisplayName {
            get
            {
                return "FK: " + ForeignKeyName;
            }
        }
        public bool UpdateDependent { get; set; }
        public bool UpdateNested { get; set; }
        public bool IncludeInView
        {
            get
            {
                return _includeInView;
            }
            set
            {
                if (_includeInView != value)
                {
                    _includeInView = value;
                    OnPropertyChanged("IncludeInView");
                    UpdateNestedIncludeInView(value);
                }
            }
        }
        public string OriginalPropertyName
        {
            get
            {
                return _originalPropertyName;
            }
            set
            {
                string newVal = "";
                if (!String.IsNullOrEmpty(value))
                {
                    newVal = value;
                }
                int doReset = String.Compare(_originalPropertyName, newVal, StringComparison.OrdinalIgnoreCase);
                _originalPropertyName = value;
                if ((doReset != 0) && UpdateDependent)
                {
                    UpdateViewModelFieldName();
                }
            }
        }
        public string ForeignKeyName
        {
            get
            {
                return _foreignKeyName;
            }
            set
            {
                string newVal = "";
                if (!String.IsNullOrEmpty(value))
                {
                    newVal = value;
                }
                _foreignKeyName = newVal;
                int doReset = String.Compare(_foreignKeyName, newVal, StringComparison.OrdinalIgnoreCase);
                if (String.IsNullOrEmpty(ForeignKeyAlias))
                {
                    ForeignKeyAlias = _foreignKeyName;
                }
            }
        }
        public string ForeignKeyAlias
        {
            get
            {
                return _foreignKeyAlias;
            }
            set
            {
                string newVal = "";
                if (!String.IsNullOrEmpty(value))
                {
                    newVal = value;
                }
                int doReset = String.Compare(_foreignKeyAlias, newVal, StringComparison.OrdinalIgnoreCase);
                _foreignKeyAlias = newVal;
                if (doReset != 0)
                {
                    ForeignKeyPrefix = _childForeignKeyPrefix + _foreignKeyAlias;
                }
            }
        }
        public string ForeignKeyPrefix
        {
            get { return _foreignKeyPrefix; }
            set
            {
                string newVal = "";
                if (!String.IsNullOrEmpty(value))
                {
                    newVal = value;
                }
                int doReset = String.Compare(_foreignKeyPrefix, newVal, StringComparison.OrdinalIgnoreCase);
                _foreignKeyPrefix = newVal;
                if ((doReset != 0) && UpdateDependent)
                {
                    UpdateViewModelFieldName();
                }
                if ((doReset != 0) && UpdateNested)
                {
                    DoUpdateNested();
                }
                if (doReset != 0)
                {
                    OnPropertyChanged("ForeignKeyPrefix");
                }
            }
        }
        public string ChildForeignKeyPrefix
        {
            get { return _childForeignKeyPrefix; }
            set
            {
                string newVal = "";
                if (!String.IsNullOrEmpty(value))
                {
                    newVal = value;
                }

                int doReset = String.Compare(_childForeignKeyPrefix, newVal, StringComparison.OrdinalIgnoreCase);
                _childForeignKeyPrefix = newVal;
                if (doReset != 0)
                {
                    ForeignKeyPrefix = _childForeignKeyPrefix + _foreignKeyAlias;
                }
            }
        }
        public virtual void DoUpdateNested()
        {
            if (ForeigKeyParentProperties == null) return;
            foreach (PropertySelectorViewModel itm in ForeigKeyParentProperties)
            {
                itm.ChildForeignKeyPrefix = ChildForeignKeyPrefix;
                if (itm.UpdateDependent)
                {
                    itm.ForeignKeyAlias = ForeignKeyAlias;
                }

            }
        }
        public virtual void UpdateViewModelFieldName()
        {

        }
        public ObservableCollection<PropertySelectorViewModel> ForeigKeyParentProperties { get; set; }
        public string TypeFullName { get; set; }
        public string UnderlyingTypeName { get; set; }
        public bool TypeIsNullable { get; set; }

        public virtual void UpdateNestedIncludeInView(bool newVal) 
        {
            if (ForeigKeyParentProperties == null) return;
            foreach(PropertySelectorViewModel itm in ForeigKeyParentProperties)
            {
                itm.IncludeInView = newVal;
            }
        }
        public PropertySelectorViewModel ForeigKeyPPByOriginalPN(string originalPropertyName)
        {
            if (ForeigKeyParentProperties == null) return null;
            return ForeigKeyParentProperties.Where(t => t.OriginalPropertyName == originalPropertyName).FirstOrDefault();
        }
        public PropertySelectorViewModel ForeigKeyPPByForeignKN(string foreignKeyName)
        {
            if (ForeigKeyParentProperties == null) return null;
            return ForeigKeyParentProperties.Where(t => String.Equals(t.ForeignKeyName, foreignKeyName, StringComparison.CurrentCultureIgnoreCase) ).FirstOrDefault();
        }
    }
}
