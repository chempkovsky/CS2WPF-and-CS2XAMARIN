using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.Serializable;
using CS2WPF.Model.Serializable.UI;
using EnvDTE80;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class CreateWebApiViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected SolutionCodeElement _SelectedDbContext;
        protected DbContextSerializable _SerializableDbContext;
        protected ObservableCollection<ModelViewSerializable> _ModelViews;
        protected ModelViewSerializable _SelectedModel = null;
        protected Object _SelectedTreeViewItem;
        protected Visibility _HintVisibility = Visibility.Visible;
        protected Visibility _RootVisibility = Visibility.Collapsed;
        protected Visibility _KeyPropertyVisibility = Visibility.Collapsed;
        protected Visibility _PropertyVisibility = Visibility.Collapsed;
        protected Visibility _ForeignKeyVisibility = Visibility.Collapsed;
        protected Visibility _ScalarPropertyVisibility = Visibility.Collapsed;
        protected Visibility _UIListPropertiesVisibility = Visibility.Collapsed;
        protected Visibility _UIFormPropertiesVisibility = Visibility.Collapsed;

        protected Visibility _UIFormPropListVisibility = Visibility.Collapsed;
        protected Visibility _UIListPropListVisibility = Visibility.Collapsed;

        protected Visibility _UIFormPropertyVisibility = Visibility.Collapsed;
        protected Visibility _UIListPropertyVisibility = Visibility.Collapsed;

        protected Visibility _AttributeVisibility = Visibility.Collapsed;
        protected Visibility _AttributePropertyVisibility = Visibility.Collapsed;

        protected Visibility _FAPIAttributeVisibility = Visibility.Collapsed;
        protected Visibility _FAPIAttributePropertyVisibility = Visibility.Collapsed;

        protected Object _SelectedItem;
        protected string _WebApiServiceName;
        protected string _WebApiSufix = "WebApiController";
        protected bool _IsWebApiDelete;
        protected bool _IsWebApiUpdate;
        protected bool _IsWebApiAdd;
        protected bool _IsWebApiSelectOneByPrimarykey;
        protected bool _IsWebApiSelectManyWithPagination;
        protected bool _IsWebApiSelectAll;
        protected bool _IsViewHasProperties;
        protected bool _IsViewHasRimaryKey;
        protected bool _IsViewHasAllRequiredProperties;
        protected bool _IsUsedByfilter;
        protected bool _IsUsedBySorting;
        protected bool _IsWebServiceEditable = true;
        protected ObservableCollection<ModelViewPropertyOfVwNotified> _ScalarProperties;

        protected bool _IsShownInListView;
        protected bool _IsNewLineAfterInListView;
        protected bool _IsShownInEditView;
        protected bool _IsNewLineAfterInEditView;
        protected ModelViewUIListProperty _SelectedUIListProperty;
        protected ModelViewUIFormProperty _SelectedUIFormProperty;
        protected ObservableCollection<ModelViewSerializable> _ForeignKeyChainViews;
        #endregion

        public CreateWebApiViewModel(DTE2 dte) : base()
        {
            this.Dte = dte;
            UiInputTypes = new ObservableCollection<InputTypeEnum>() { InputTypeEnum.Default, InputTypeEnum.ReadOnly, 
                InputTypeEnum.Combo, InputTypeEnum.Typeahead, InputTypeEnum.SearchDialog, InputTypeEnum.Hidden };
            ModelViews = new ObservableCollection<ModelViewSerializable>();
            ScalarProperties = new ObservableCollection<ModelViewPropertyOfVwNotified>();
            UIFormProperties = new ObservableCollection<ModelViewUIFormProperty>();
            UIListProperties = new ObservableCollection<ModelViewUIListProperty>();
            ForeignKeyChainViews = new ObservableCollection<ModelViewSerializable>();
        }
        public TreeViewItem MainTreeViewRootItem { get; set; }
        public ObservableCollection<InputTypeEnum> UiInputTypes { get; set; }
        public ObservableCollection<ModelViewSerializable> ForeignKeyChainViews
        {
            get { return _ForeignKeyChainViews; }
            set
            {
                if (_ForeignKeyChainViews == value) return;

                _ForeignKeyChainViews = value;
                OnPropertyChanged();
            }
        }
        public string JsonExtension { get; set; } = "json";
        public string ContextItemViewName { get; set; } = "==Context==";
        public string DestinationProject { get; set; }
        public string DefaultProjectNameSpace { get; set; }
        public string DestinationFolder { get; set; }
        public SolutionCodeElement SelectedDbContext
        {
            get { return _SelectedDbContext; }
            set
            {
                if (_SelectedDbContext == value) return;
                SelectedTreeViewItem = null;
                _SelectedDbContext = value;
                OnPropertyChanged();
                OnSelectedDbContextChanged();
                if (this.MainTreeViewRootItem != null)
                {
                    this.MainTreeViewRootItem.IsSelected = true;
                    RootVisibility = Visibility.Visible;
                    HintVisibility = Visibility.Collapsed;
                }
            }
        }
        public DbContextSerializable SerializableDbContext
        {
            get { return _SerializableDbContext; }
            set
            {
                if (_SerializableDbContext == value) return;
                SelectedTreeViewItem = null;
                _SerializableDbContext = value;
                OnPropertyChanged();
                OnSerializableDbContextChanged();
            }
        }
        public ObservableCollection<ModelViewSerializable> ModelViews
        {
            get { return _ModelViews; }
            set
            {
                if (_ModelViews == value) return;
                SelectedTreeViewItem = null;
                _ModelViews = value;
                OnPropertyChanged();
            }
        }
        public ModelViewSerializable SelectedModel
        {
            get { return _SelectedModel; }
            set
            {
                if (_SelectedModel == value) return;
                SelectedTreeViewItem = null;
                _SelectedModel = value;
                OnSelectedModelChanged();
                if (this.MainTreeViewRootItem != null)
                {
                    this.MainTreeViewRootItem.IsSelected = true;
                    RootVisibility = Visibility.Visible;
                    HintVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ModelViewPropertyOfVwNotified> ScalarProperties
        {
            get { return _ScalarProperties; }
            set
            {
                if (_ScalarProperties == value) return;

                _ScalarProperties = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ModelViewUIFormProperty> UIFormProperties { get; set; }
        public ObservableCollection<ModelViewUIListProperty> UIListProperties { get; set; }
        public ModelViewUIFormProperty SelectedUIFormProperty
        {
            get { return _SelectedUIFormProperty; }
            set
            {
                if (_SelectedUIFormProperty == value) return;

                _SelectedUIFormProperty = value;
                OnPropertyChanged();
            }
        }
        public ModelViewUIListProperty SelectedUIListProperty
        {
            get { return _SelectedUIListProperty; }
            set
            {
                if (_SelectedUIListProperty == value) return;

                _SelectedUIListProperty = value;
                OnPropertyChanged();
            }
        }
        public Object SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                if (_SelectedItem == value) return;
                _SelectedItem = value;
                OnPropertyChanged();
            }
        }
        public Object SelectedTreeViewItem
        {
            get
            {
                return _SelectedTreeViewItem;
            }
            set
            {
                if (_SelectedTreeViewItem != value)
                {
                    _SelectedTreeViewItem = value;
                    if ((_SelectedTreeViewItem == null) || (SelectedModel == null))
                    {
                        ScalarPropertyVisibility = Visibility.Collapsed;
                        PropertyVisibility = Visibility.Collapsed;
                        KeyPropertyVisibility = Visibility.Collapsed;
                        RootVisibility = Visibility.Collapsed;
                        ForeignKeyVisibility = Visibility.Collapsed;
                        UIFormPropListVisibility = Visibility.Collapsed;
                        UIListPropListVisibility = Visibility.Collapsed;
                        UIFormPropertyVisibility = Visibility.Collapsed;
                        UIListPropertyVisibility = Visibility.Collapsed;
                        AttributePropertyVisibility = Visibility.Collapsed;
                        AttributeVisibility = Visibility.Collapsed;
                        FAPIAttributePropertyVisibility = Visibility.Collapsed;
                        FAPIAttributeVisibility = Visibility.Collapsed;

                        HintVisibility = Visibility.Visible;
                    }
                    else
                    {
                        HintVisibility = Visibility.Collapsed;
                        RootVisibility = Visibility.Collapsed;
                        KeyPropertyVisibility = Visibility.Collapsed;
                        PropertyVisibility = Visibility.Collapsed;
                        ScalarPropertyVisibility = Visibility.Collapsed;
                        ForeignKeyVisibility = Visibility.Collapsed;
                        UIFormPropListVisibility = Visibility.Collapsed;
                        UIListPropListVisibility = Visibility.Collapsed;
                        UIFormPropertyVisibility = Visibility.Collapsed;
                        UIListPropertyVisibility = Visibility.Collapsed;
                        AttributePropertyVisibility = Visibility.Collapsed;
                        AttributeVisibility = Visibility.Collapsed;
                        FAPIAttributePropertyVisibility = Visibility.Collapsed;
                        FAPIAttributeVisibility = Visibility.Collapsed;

                        if (_SelectedTreeViewItem is TreeViewItem)
                        {
                            TreeViewItem treeViewItem = _SelectedTreeViewItem as TreeViewItem;
                            if ("Root".Equals(treeViewItem.Tag))
                            {
                                RootVisibility = Visibility.Visible;
                            }
                            else if ("ScalarProperties".Equals(treeViewItem.Tag))
                            {
                                // show tabled data in the future releases
                                HintVisibility = Visibility.Visible;
                            }
                            else if ("ForeignKeys".Equals(treeViewItem.Tag))
                            {
                                // show tabled data in the future releases
                                HintVisibility = Visibility.Visible;
                            }
                            else if ("PrimaryKeyProperties".Equals(treeViewItem.Tag))
                            {
                                // show tabled data in the future releases
                                HintVisibility = Visibility.Visible;
                            }
                            else if ("Entity Properties".Equals(treeViewItem.Tag))
                            {
                                // show tabled data in the future releases
                                HintVisibility = Visibility.Visible;
                            }
                            else if ("UI List Properties".Equals(treeViewItem.Tag))
                            {
                                UIListPropListVisibility = Visibility.Visible;
                            }
                            else if ("UI Form Properties".Equals(treeViewItem.Tag))
                            {
                                UIFormPropListVisibility = Visibility.Visible;
                            }
                            else
                            {
                                HintVisibility = Visibility.Visible;
                            }
                        }
                        else if (_SelectedTreeViewItem is ModelViewKeyPropertySerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            KeyPropertyVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewPropertyOfVwNotified)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            ScalarPropertyVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewPropertyOfFkSerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            PropertyVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewForeignKeySerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            ForeignKeyVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewAttribute)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            AttributeVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewAttributeProperty)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            AttributePropertyVisibility = Visibility.Visible;
                        }

                        else if (_SelectedTreeViewItem is ModelViewFAPIAttribute)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            FAPIAttributeVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewFAPIAttributeProperty)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            FAPIAttributePropertyVisibility = Visibility.Visible;
                        }

                        else if (_SelectedTreeViewItem is ModelViewAttributeSerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            AttributeVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewAttributePropertySerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            AttributePropertyVisibility = Visibility.Visible;
                        }


                        else if (_SelectedTreeViewItem is ModelViewFAPIAttributeSerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            AttributeVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewFAPIAttributePropertySerializable)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            AttributePropertyVisibility = Visibility.Visible;
                        }

                        else if (_SelectedTreeViewItem is ModelViewUIFormProperty)
                        {
                            ModelViewUIFormProperty modelViewUIFormProperty = _SelectedTreeViewItem as ModelViewUIFormProperty;
                            SelectedItem = null;
                            if (ForeignKeyChainViews == null) ForeignKeyChainViews = new ObservableCollection<ModelViewSerializable>();
                            ForeignKeyChainViews.Clear();
                            if(SerializableDbContext != null)
                            {
                                if(SelectedModel !=null)
                                {
                                    List<ModelViewSerializable> rslt = 
                                        SerializableDbContext.GetViewsByForeignNameChain(SelectedModel.ViewName, modelViewUIFormProperty.ForeignKeyNameChain);
                                    if(rslt != null)
                                    {
                                        rslt.ForEach(i => ForeignKeyChainViews.Add(i));
                                    }
                                }
                            }
                            SelectedItem = modelViewUIFormProperty;
                            UIFormPropertyVisibility = Visibility.Visible;
                        }
                        else if (_SelectedTreeViewItem is ModelViewUIListProperty)
                        {
                            SelectedItem = _SelectedTreeViewItem;
                            UIListPropertyVisibility = Visibility.Visible;
                        }
                        else
                        {
                            HintVisibility = Visibility.Visible;
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }
        public Visibility HintVisibility
        {
            get
            {
                return _HintVisibility;
            }
            set
            {
                if (_HintVisibility == value) return;
                _HintVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility RootVisibility
        {
            get
            {
                return _RootVisibility;
            }
            set
            {
                if (_RootVisibility == value) return;
                _RootVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility KeyPropertyVisibility
        {
            get
            {
                return _KeyPropertyVisibility;
            }
            set
            {
                if (_KeyPropertyVisibility == value) return;
                _KeyPropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility PropertyVisibility
        {
            get
            {
                return _PropertyVisibility;
            }
            set
            {
                if (_PropertyVisibility == value) return;
                _PropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility ForeignKeyVisibility
        {
            get
            {
                return _ForeignKeyVisibility;
            }
            set
            {
                if (_ForeignKeyVisibility == value) return;
                _ForeignKeyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility ScalarPropertyVisibility
        {
            get
            {
                return _ScalarPropertyVisibility;
            }
            set
            {
                if (_ScalarPropertyVisibility == value) return;
                _ScalarPropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility UIListPropertiesVisibility
        {
            get
            {
                return _UIListPropertiesVisibility;
            }
            set
            {
                if (_UIListPropertiesVisibility == value) return;
                _UIListPropertiesVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility UIFormPropertiesVisibility
        {
            get
            {
                return _UIFormPropertiesVisibility;
            }
            set
            {
                if (_UIFormPropertiesVisibility == value) return;
                _UIFormPropertiesVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility UIFormPropListVisibility
        {
            get
            {
                return _UIFormPropListVisibility;
            }
            set
            {
                if (_UIFormPropListVisibility == value) return;
                _UIFormPropListVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility UIListPropListVisibility
        {
            get
            {
                return _UIListPropListVisibility;
            }
            set
            {
                if (_UIListPropListVisibility == value) return;
                _UIListPropListVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility UIFormPropertyVisibility
        {
            get
            {
                return _UIFormPropertyVisibility;
            }
            set
            {
                if (_UIFormPropertyVisibility == value) return;
                _UIFormPropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility UIListPropertyVisibility
        {
            get
            {
                return _UIListPropertyVisibility;
            }
            set
            {
                if (_UIListPropertyVisibility == value) return;
                _UIListPropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility AttributeVisibility
        {
            get
            {
                return _AttributeVisibility;
            }
            set
            {
                if (_AttributeVisibility == value) return;
                _AttributeVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility AttributePropertyVisibility
        {
            get
            {
                return _AttributePropertyVisibility;
            }
            set
            {
                if (_AttributePropertyVisibility == value) return;
                _AttributePropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility FAPIAttributeVisibility
        {
            get
            {
                return _FAPIAttributeVisibility;
            }
            set
            {
                if (_FAPIAttributeVisibility == value) return;
                _FAPIAttributeVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility FAPIAttributePropertyVisibility
        {
            get
            {
                return _FAPIAttributePropertyVisibility;
            }
            set
            {
                if (_FAPIAttributePropertyVisibility == value) return;
                _FAPIAttributePropertyVisibility = value;
                OnPropertyChanged();
            }
        }
        public bool IsWebServiceEditable
        {
            get
            {
                return _IsWebServiceEditable;
            }
            set
            {
                if (_IsWebServiceEditable == value) return;
                _IsWebServiceEditable = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public string WebApiSufix
        {
            get
            {
                return _WebApiSufix;
            }
            set
            {
                if (_WebApiSufix == value) return;
                _WebApiSufix = value;
                OnPropertyChanged();
            }
        }
        public string WebApiServiceName
        {
            get
            {
                return _WebApiServiceName;
            }
            set
            {
                if (_WebApiServiceName == value) return;
                _WebApiServiceName = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsWebApiSelectAll
        {
            get
            {
                return _IsWebApiSelectAll;
            }
            set
            {
                if (_IsWebApiSelectAll == value) return;
                _IsWebApiSelectAll = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsWebApiSelectManyWithPagination
        {
            get
            {
                return _IsWebApiSelectManyWithPagination;
            }
            set
            {
                if (_IsWebApiSelectManyWithPagination == value) return;
                _IsWebApiSelectManyWithPagination = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsWebApiSelectOneByPrimarykey
        {
            get
            {
                return _IsWebApiSelectOneByPrimarykey;
            }
            set
            {
                if (_IsWebApiSelectOneByPrimarykey == value) return;
                _IsWebApiSelectOneByPrimarykey = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsWebApiAdd
        {
            get
            {
                return _IsWebApiAdd;
            }
            set
            {
                if (_IsWebApiAdd == value) return;
                _IsWebApiAdd = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsWebApiUpdate
        {
            get
            {
                return _IsWebApiUpdate;
            }
            set
            {
                if (_IsWebApiUpdate == value) return;
                _IsWebApiUpdate = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsWebApiDelete
        {
            get
            {
                return _IsWebApiDelete;
            }
            set
            {
                if (_IsWebApiDelete == value) return;
                _IsWebApiDelete = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsViewHasProperties
        {
            get
            {
                return _IsViewHasProperties && IsWebServiceEditable;
            }
            set
            {
                if (_IsViewHasProperties == value) return;
                _IsViewHasProperties = value;
                OnPropertyChanged();
            }
        }
        public bool IsViewHasRimaryKey
        {
            get
            {
                return _IsViewHasRimaryKey && IsWebServiceEditable;
            }
            set
            {
                if (_IsViewHasRimaryKey == value) return;
                _IsViewHasRimaryKey = value;
                OnPropertyChanged();
            }
        }
        public bool IsViewHasAllRequiredProperties
        {
            get
            {
                return _IsViewHasAllRequiredProperties && IsWebServiceEditable;
            }
            set
            {
                if (_IsViewHasAllRequiredProperties == value) return;
                _IsViewHasAllRequiredProperties = value;
                OnPropertyChanged();
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
                if (_IsUsedByfilter == value) return;
                _IsUsedByfilter = value;
                OnPropertyChanged();
                OnIsUsedByfilterChanged();
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
                if (_IsUsedBySorting == value) return;
                _IsUsedBySorting = value;
                OnPropertyChanged();
                OnIsUsedBySortingChanged();
            }
        }
        public bool IsShownInListView
        {
            get
            {
                return _IsShownInListView;
            }
            set
            {
                if (_IsShownInListView == value) return;
                _IsShownInListView = value;
                OnPropertyChanged();
                OnIsShownInListViewChanged();
            }
        }
        public bool IsNewLineAfterInListView
        {
            get
            {
                return _IsNewLineAfterInListView;
            }
            set
            {
                if (_IsNewLineAfterInListView == value) return;
                _IsNewLineAfterInListView = value;
                OnPropertyChanged();
                OnIsNewLineAfterInListViewChanged();
            }
        }
        public bool IsShownInEditView
        {
            get
            {
                return _IsShownInEditView;
            }
            set
            {
                if (_IsShownInEditView == value) return;
                _IsShownInEditView = value;
                OnPropertyChanged();
                OnIsShownInEditViewChanged();
            }
        }
        public bool IsNewLineAfterInEditView
        {
            get
            {
                return _IsNewLineAfterInEditView;
            }
            set
            {
                if (_IsNewLineAfterInEditView == value) return;
                _IsNewLineAfterInEditView = value;
                OnPropertyChanged();
                OnIsNewLineAfterInEditViewChanged();
            }
        }
        public ModelViewSerializable GetSelectedModelShallowCopy()
        {
            if (SelectedModel == null) return null;
            ModelViewSerializable result = SelectedModel.ModelViewSerializableGetShallowCopy();
            result.WebApiServiceName = this.WebApiServiceName;
            result.IsWebApiSelectAll = this.IsWebApiSelectAll;
            result.IsWebApiSelectManyWithPagination = this.IsWebApiSelectManyWithPagination;
            result.IsWebApiSelectOneByPrimarykey = this.IsWebApiSelectOneByPrimarykey;
            result.IsWebApiAdd = this.IsWebApiAdd;
            result.IsWebApiUpdate = this.IsWebApiUpdate;
            result.IsWebApiDelete = this.IsWebApiDelete;
            result.WebApiServiceProject = this.DestinationProject;
            result.WebApiServiceDefaultProjectNameSpace = this.DefaultProjectNameSpace;
            result.WebApiServiceFolder = this.DestinationFolder;
            result.ScalarProperties = new List<ModelViewPropertyOfVwSerializable>();
            foreach(ModelViewPropertyOfVwNotified srcProp in ScalarProperties)
            {
                result.ScalarProperties.Add(srcProp.ModelViewPropertyOfVwNotifiedAssignTo(new ModelViewPropertyOfVwSerializable()));
            }

            result.UIFormProperties = new List<ModelViewUIFormPropertySerializable>();
            if (UIFormProperties != null)
            {
                foreach (ModelViewUIFormProperty srcProp in UIFormProperties)
                {
                    result.UIFormProperties.Add(srcProp.ModelViewUIFormPropertyAssignTo(new ModelViewUIFormPropertySerializable()));
                }
            }

            result.UIListProperties = new List<ModelViewUIListPropertySerializable>();
            if (UIListProperties != null)
            {
                foreach (ModelViewUIListProperty srcProp in UIListProperties)
                {
                    result.UIListProperties.Add(srcProp.ModelViewUIListPropertyAssignTo(new ModelViewUIListPropertySerializable()));
                }
            }

            return result;
        }
        public ModelViewSerializable GetSelectedModelCommonShallowCopy(string FileType, string FileName)
        {
            if (SelectedModel == null) return null;
            ModelViewSerializable result = SelectedModel.ModelViewSerializableGetShallowCopy();

            if(result.CommonStaffs == null)
            {
                result.CommonStaffs = new List<CommonStaffSerializable>();
            } else
            {
                result.CommonStaffs = new List<CommonStaffSerializable>();
                SelectedModel.CommonStaffs.ForEach(c => result.CommonStaffs.Add(new CommonStaffSerializable()
                {
                    Extension = c.Extension,
                    FileType = c.FileType,
                    FileName = c.FileName,
                    FileProject = c.FileProject,
                    FileDefaultProjectNameSpace = c.FileDefaultProjectNameSpace,
                    FileFolder = c.FileFolder
                }));
            }
            CommonStaffSerializable commonStaffItem = 
                result.CommonStaffs.Where(c => c.FileType == FileType).FirstOrDefault();
            if (commonStaffItem == null)
            {
                result.CommonStaffs.Add(
                    commonStaffItem = new CommonStaffSerializable()
                    {
                        FileType = FileType
                    });
            }
            commonStaffItem.FileName = FileName;
            commonStaffItem.FileProject = this.DestinationProject;
            commonStaffItem.FileDefaultProjectNameSpace = this.DefaultProjectNameSpace;
            commonStaffItem.FileFolder = this.DestinationFolder;

            result.ScalarProperties = new List<ModelViewPropertyOfVwSerializable>();
            if (ScalarProperties != null)
            {
                foreach (ModelViewPropertyOfVwNotified srcProp in ScalarProperties)
                {
                    result.ScalarProperties.Add(srcProp.ModelViewPropertyOfVwNotifiedAssignTo(new ModelViewPropertyOfVwSerializable()));
                }
            }

            result.UIFormProperties = new List<ModelViewUIFormPropertySerializable>();
            if (UIFormProperties != null)
            {
                foreach (ModelViewUIFormProperty srcProp in UIFormProperties)
                {
                    result.UIFormProperties.Add(srcProp.ModelViewUIFormPropertyAssignTo(new ModelViewUIFormPropertySerializable()));
                }
            }

            result.UIListProperties = new List<ModelViewUIListPropertySerializable>();
            if (UIListProperties != null)
            {
                foreach (ModelViewUIListProperty srcProp in UIListProperties)
                {
                    result.UIListProperties.Add(srcProp.ModelViewUIListPropertyAssignTo(new ModelViewUIListPropertySerializable()));
                }
            }

            return result;
        }
        public void OnSelectedDbContextChanged()
        {
            string projectName = "";
            if (SelectedDbContext.CodeElementRef != null)
            {
                if (SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, SelectedDbContext.CodeElementFullName, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                string solutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(solutionDirectory, locFileName);
                if (File.Exists(locFileName))
                {
                    string jsonString = File.ReadAllText(locFileName);
                    SerializableDbContext = JsonConvert.DeserializeObject<DbContextSerializable>(jsonString);
                } else
                {
                    SerializableDbContext = new DbContextSerializable();
                }
            }
        }
        public void OnSerializableDbContextChanged()
        {
            SelectedTreeViewItem = null;
            ModelViews.Clear();
            if (SerializableDbContext == null) return;
            // if ((SerializableDbContext.ModelViews == null) && IsWebServiceEditable) return;
            if (SerializableDbContext.ModelViews == null) return;

            if ((SerializableDbContext.ModelViews.Count < 1) && IsWebServiceEditable) return;

            if(!IsWebServiceEditable)
            {
                ModelViewSerializable contextItm = new ModelViewSerializable() {
                    ViewName = ContextItemViewName,
                    CommonStaffs = SerializableDbContext.CommonStaffs
                };
                if (contextItm.CommonStaffs == null)
                {
                    contextItm.CommonStaffs = new List<CommonStaffSerializable>();
                }
                ModelViews.Add(contextItm);
            }

            foreach (ModelViewSerializable itm in SerializableDbContext.ModelViews)
            {
                ModelViews.Add(itm);
            }
        }
        public void ClearCheckBoxes()
        {
            IsWebApiSelectAll = false;
            IsWebApiSelectManyWithPagination = false;
            IsWebApiSelectOneByPrimarykey = false;
            IsWebApiAdd = false;
            IsWebApiUpdate = false;
            IsWebApiDelete = false;
            IsViewHasProperties = false;
            IsViewHasRimaryKey = false;
            IsViewHasAllRequiredProperties = false;
            IsUsedBySorting = false;
            IsUsedByfilter = false;
        }
        public void OnSelectedModelChanged()
        {
            SelectedTreeViewItem = null;
            if(ScalarProperties == null) ScalarProperties = new ObservableCollection<ModelViewPropertyOfVwNotified>();
            if (UIFormProperties == null) UIFormProperties = new ObservableCollection<ModelViewUIFormProperty>();
            if (UIListProperties == null) UIListProperties = new ObservableCollection<ModelViewUIListProperty>();

            SelectedUIListProperty = null;
            UIListProperties.Clear();
            SelectedUIFormProperty = null;
            UIFormProperties.Clear();
            ScalarProperties.Clear();
            ClearCheckBoxes();
            if (SelectedModel == null)
            {
                WebApiServiceName = "";
                return;
            }
            if (string.IsNullOrEmpty(SelectedModel.WebApiServiceName))
            {
                WebApiServiceName = SelectedModel.ViewName + WebApiSufix;
            } else
            {
                WebApiServiceName = SelectedModel.WebApiServiceName;
            }
            this.IsWebApiSelectAll = SelectedModel.IsWebApiSelectAll;
            this.IsWebApiSelectManyWithPagination = SelectedModel.IsWebApiSelectManyWithPagination;
            this.IsWebApiSelectOneByPrimarykey = SelectedModel.IsWebApiSelectOneByPrimarykey;
            this.IsWebApiAdd = SelectedModel.IsWebApiAdd;
            this.IsWebApiUpdate = SelectedModel.IsWebApiUpdate;
            this.IsWebApiDelete = SelectedModel.IsWebApiDelete;
            if (SelectedModel.ScalarProperties != null)
            {
                IsViewHasProperties = SelectedModel.ScalarProperties.Count > 0;
            }
            if(_IsViewHasProperties)
            {
                if (SelectedModel.PrimaryKeyProperties != null)
                {
                    if(SelectedModel.PrimaryKeyProperties.Count > 0)
                    {
                        foreach(ModelViewKeyPropertySerializable prop in SelectedModel.PrimaryKeyProperties)
                        {
                            IsViewHasRimaryKey = false;
                            if (!  SelectedModel.ScalarProperties.Any(p =>
                                 ((p.OriginalPropertyName == prop.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))) )
                            {
                                if (SelectedModel.ForeignKeys != null)
                                {
                                    foreach(ModelViewForeignKeySerializable fk in SelectedModel.ForeignKeys)
                                    {
                                        if ((fk.ForeignKeyProps != null) && (fk.PrincipalKeyProps != null))
                                        {
                                            int cnt = fk.ForeignKeyProps.Count;
                                            if (cnt < fk.PrincipalKeyProps.Count)
                                            {
                                                cnt = fk.PrincipalKeyProps.Count;
                                            }
                                            for(int i = 0; i < cnt; i++)
                                            {
                                                if (fk.ForeignKeyProps[i].OriginalPropertyName == prop.OriginalPropertyName)
                                                {
                                                    IsViewHasRimaryKey =
                                                        SelectedModel.ScalarProperties.Any(p =>
                                                        ((p.OriginalPropertyName == fk.PrincipalKeyProps[i].OriginalPropertyName) && (p.ForeignKeyNameChain == fk.NavigationName)));
                                                }
                                                if (_IsViewHasRimaryKey)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if(_IsViewHasRimaryKey)
                                        {
                                            break;
                                        }
                                    }
                                }
                            } else
                            {
                                IsViewHasRimaryKey = true;
                            }
                            
                            if (!_IsViewHasRimaryKey) break;
                        }
                    }
                }
                SelectedModel.ScalarProperties.ForEach(p => ScalarProperties.Add(p.ModelViewPropertyOfVwSerializableAssignTo(new ModelViewPropertyOfVwNotified())));
            }
            if (!_IsViewHasRimaryKey)
            {
                this.IsWebApiSelectOneByPrimarykey = false;
                this.IsWebApiAdd = false;
                this.IsWebApiUpdate = false;
                this.IsWebApiDelete = false;
            }
            IsViewHasAllRequiredProperties = false;
            if (_IsViewHasRimaryKey)
            {
                if(SelectedModel.AllProperties != null)
                {
                    if (SelectedModel.AllProperties.Count > 0)
                    {
                        IsViewHasAllRequiredProperties = true;
                        foreach (ModelViewEntityPropertySerializable prop in SelectedModel.AllProperties)
                        {
                            if (!prop.IsRequired) continue;
                            if(SelectedModel.PrimaryKeyProperties.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName)) continue;
                            IsViewHasAllRequiredProperties = 
                                SelectedModel.ScalarProperties.Any(p =>
                                ((p.OriginalPropertyName == prop.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain))));
                            if (!_IsViewHasAllRequiredProperties) break;
                        }
                    }
                }
            }
            if(!_IsViewHasAllRequiredProperties)
            {
                this.IsWebApiAdd = false;
            }
            if(SelectedModel.ScalarProperties != null)
            {
                if (SelectedModel.ScalarProperties.Count > 0)
                {
                    if (SelectedModel.UIListProperties != null)
                    {
                        foreach (ModelViewUIListPropertySerializable prop in SelectedModel.UIListProperties)
                        {
                            ModelViewPropertyOfVwSerializable srcProp =
                             SelectedModel.ScalarProperties.FirstOrDefault(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain);
                            if (srcProp != null)
                            {
                                UIListProperties.Add(new ModelViewUIListProperty()
                                {
                                    OriginalPropertyName = prop.OriginalPropertyName,
                                    ForeignKeyName = srcProp.ForeignKeyName,
                                    ForeignKeyNameChain = prop.ForeignKeyNameChain,
                                    ViewPropertyName = srcProp.ViewPropertyName,
                                    JsonPropertyName = srcProp.JsonPropertyName,
                                    IsShownInView = prop.IsShownInView,
                                    IsNewLineAfter = prop.IsNewLineAfter
                                });
                            }
                        }
                    }
                    foreach (ModelViewPropertyOfVwSerializable prop in SelectedModel.ScalarProperties)
                    {
                        if (!UIListProperties.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain))
                        {
                            UIListProperties.Add(new ModelViewUIListProperty()
                            {
                                OriginalPropertyName = prop.OriginalPropertyName,
                                ForeignKeyName = prop.ForeignKeyName,
                                ForeignKeyNameChain = prop.ForeignKeyNameChain,
                                ViewPropertyName = prop.ViewPropertyName,
                                JsonPropertyName = prop.JsonPropertyName,
                            });
                        }
                    }


                    if (SelectedModel.UIFormProperties != null)
                    {
                        foreach (ModelViewUIFormPropertySerializable prop in SelectedModel.UIFormProperties)
                        {
                            ModelViewPropertyOfVwSerializable srcProp =
                             SelectedModel.ScalarProperties.FirstOrDefault(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain);
                            if (srcProp != null)
                            {
                                UIFormProperties.Add(new ModelViewUIFormProperty()
                                {
                                    OriginalPropertyName = prop.OriginalPropertyName,
                                    InputTypeWhenAdd = prop.InputTypeWhenAdd,
                                    InputTypeWhenUpdate = prop.InputTypeWhenUpdate,
                                    InputTypeWhenDelete = prop.InputTypeWhenDelete,
                                    ForeifKeyViewNameForAdd = prop.ForeifKeyViewNameForAdd,
                                    ForeifKeyViewNameForUpd = prop.ForeifKeyViewNameForUpd,
                                    ForeifKeyViewNameForDel = prop.ForeifKeyViewNameForDel,
                                    ForeignKeyName = srcProp.ForeignKeyName,
                                    ForeignKeyNameChain = prop.ForeignKeyNameChain,
                                    ViewPropertyName = srcProp.ViewPropertyName,
                                    JsonPropertyName = srcProp.JsonPropertyName,
                                    IsShownInView = prop.IsShownInView,
                                    IsNewLineAfter = prop.IsNewLineAfter
                                });
                            }
                        }
                    }
                    foreach (ModelViewPropertyOfVwSerializable prop in SelectedModel.ScalarProperties)
                    {
                        if (!UIFormProperties.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain))
                        {
                            UIFormProperties.Add(new ModelViewUIFormProperty()
                            {
                                OriginalPropertyName = prop.OriginalPropertyName,
                                ForeignKeyName = prop.ForeignKeyName,
                                ForeignKeyNameChain = prop.ForeignKeyNameChain,
                                ViewPropertyName = prop.ViewPropertyName,
                                JsonPropertyName = prop.JsonPropertyName,
                            });
                        }
                    }
                }
            }
            CheckIsReady();
        }
        public void OnIsUsedByfilterChanged()
        {
            if (ScalarProperties == null) ScalarProperties = new ObservableCollection<ModelViewPropertyOfVwNotified>();
            foreach(ModelViewPropertyOfVwNotified prop in ScalarProperties)
            {
                prop.IsUsedByfilter = IsUsedByfilter;
            }
        }
        public void OnIsUsedBySortingChanged()
        {
            if (ScalarProperties == null) ScalarProperties = new ObservableCollection<ModelViewPropertyOfVwNotified>();
            foreach (ModelViewPropertyOfVwNotified prop in ScalarProperties)
            {
                prop.IsUsedBySorting = IsUsedBySorting;
            }
        }
        public void OnIsShownInListViewChanged()
        {
            if (UIListProperties == null) UIListProperties = new ObservableCollection<ModelViewUIListProperty>();
            foreach (ModelViewUIListProperty prop in UIListProperties)
            {
                prop.IsShownInView = IsShownInListView;
            }
        }
        public void OnIsNewLineAfterInListViewChanged()
        {
            if (UIListProperties == null) UIListProperties = new ObservableCollection<ModelViewUIListProperty>();
            foreach (ModelViewUIListProperty prop in UIListProperties)
            {
                prop.IsNewLineAfter = IsNewLineAfterInListView;
            }
        }
        public void OnIsShownInEditViewChanged()
        {
            if (UIFormProperties == null) UIFormProperties = new ObservableCollection<ModelViewUIFormProperty>();
            foreach (ModelViewUIFormProperty prop in UIFormProperties)
            {
                prop.IsShownInView = IsShownInEditView;
            }

        }
        public void OnIsNewLineAfterInEditViewChanged()
        {
            if (UIFormProperties == null) UIFormProperties = new ObservableCollection<ModelViewUIFormProperty>();
            foreach (ModelViewUIFormProperty prop in UIFormProperties)
            {
                prop.IsNewLineAfter = IsNewLineAfterInEditView;
            }
        }
        public void CheckIsReady()
        {
            if (SelectedModel != null)
            {
                if(SelectedModel.ViewName == ContextItemViewName)
                {
                    IsReady.DoNotify(this, true);
                    return;
                }
            }
            IsReady.DoNotify(this, (!string.IsNullOrEmpty(WebApiServiceName)) && 
                (IsWebApiSelectAll || IsWebApiSelectManyWithPagination || IsWebApiSelectOneByPrimarykey ||
                IsWebApiAdd || IsWebApiUpdate || IsWebApiDelete));
        }

        #region UiBtnFormCommandUp
        private ICommand _UiBtnFormCommandUp;
        public ICommand UiBtnFormCommandUp
        {
            get
            {
                return _UiBtnFormCommandUp ?? (_UiBtnFormCommandUp = new CommandHandler((param) => UiBtnFormCommandUpAction(param), (param) => UiBtnFormCommandUpCanExecute(param)));
            }
        }
        public bool UiBtnFormCommandUpCanExecute(Object param)
        {
            if(SelectedUIFormProperty == null) return false;
            int i = UIFormProperties.IndexOf(SelectedUIFormProperty);
            return i > 0;
        }
        public virtual void UiBtnFormCommandUpAction(Object param)
        {
            if (SelectedUIFormProperty == null) return;
            int i = UIFormProperties.IndexOf(SelectedUIFormProperty);

            if ((i > 0) && (i < UIFormProperties.Count))
            {
                UIFormProperties.Move(i, i - 1);
            }
        }
        #endregion

        #region UiBtnFormCommandDown
        private ICommand _UiBtnFormCommandDown;
        public ICommand UiBtnFormCommandDown
        {
            get
            {
                return _UiBtnFormCommandDown ?? (_UiBtnFormCommandDown = new CommandHandler((param) => UiBtnFormCommandDownAction(param), (param) => UiBtnFormCommandDownCanExecute(param)));
            }
        }
        public bool UiBtnFormCommandDownCanExecute(Object param)
        {
            if (SelectedUIFormProperty == null) return false;
            int i = UIFormProperties.IndexOf(SelectedUIFormProperty);
            return (i > -1) && (i < UIFormProperties.Count - 1);
        }
        public virtual void UiBtnFormCommandDownAction(Object param)
        {
            if (SelectedUIFormProperty == null) return;
            int i = UIFormProperties.IndexOf(SelectedUIFormProperty);

            if ((i > -1) && (i < UIFormProperties.Count - 1))
            {
                UIFormProperties.Move(i, i + 1);
            }
        }
        #endregion

        #region UiBtnListCommandUp
        private ICommand _UiBtnListCommandUp;
        public ICommand UiBtnListCommandUp
        {
            get
            {
                return _UiBtnListCommandUp ?? (_UiBtnListCommandUp = new CommandHandler((param) => UiBtnListCommandUpAction(param), (param) => UiBtnListCommandUpCanExecute(param)));
            }
        }
        public bool UiBtnListCommandUpCanExecute(Object param)
        {
            if (SelectedUIListProperty == null) return false;
            int i = UIListProperties.IndexOf(SelectedUIListProperty);
            return i > 0;
        }
        public virtual void UiBtnListCommandUpAction(Object param)
        {
            if (SelectedUIListProperty == null) return;
            int i = UIListProperties.IndexOf(SelectedUIListProperty);

            if ((i > 0) && (i < UIListProperties.Count))
            {
                UIListProperties.Move(i, i - 1);
            }
        }
        #endregion

        #region UiBtnFormCommandDown
        private ICommand _UiBtnListCommandDown;
        public ICommand UiBtnListCommandDown
        {
            get
            {
                return _UiBtnListCommandDown ?? (_UiBtnListCommandDown = new CommandHandler((param) => UiBtnListCommandDownAction(param), (param) => UiBtnListCommandDownCanExecute(param)));
            }
        }
        public bool UiBtnListCommandDownCanExecute(Object param)
        {
            if (SelectedUIListProperty == null) return false;
            int i = UIListProperties.IndexOf(SelectedUIListProperty);
            return (i > -1) && (i < UIListProperties.Count - 1);
        }
        public virtual void UiBtnListCommandDownAction(Object param)
        {
            if (SelectedUIListProperty == null) return;
            int i = UIListProperties.IndexOf(SelectedUIListProperty);

            if ((i > -1) && (i < UIListProperties.Count - 1))
            {
                UIListProperties.Move(i, i + 1);
            }
        }
        #endregion

    }
}
