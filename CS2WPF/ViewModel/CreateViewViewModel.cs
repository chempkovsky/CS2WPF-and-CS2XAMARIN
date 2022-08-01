using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using EnvDTE;
using EnvDTE80;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CS2WPF.Model.Serializable;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class CreateViewViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected SolutionCodeElement _SelectedDbContext;
        protected SolutionCodeElement _SelectedEntity;
        protected string _EntityNameCaption;
        protected string _InvitationCaption;
        protected Object _selectedTreeViewItem;
        protected ModelView _SelectedModel;
        protected bool _SelectAllProperties = false;
        protected Visibility _HintVisibility = Visibility.Visible;
        protected Visibility _RootVisibility = Visibility.Collapsed;
        protected Visibility _KeyPropertyVisibility = Visibility.Collapsed;
        protected Visibility _PropertyVisibility = Visibility.Collapsed;
        protected Visibility _ForeignKeyVisibility = Visibility.Collapsed;
        protected Visibility _AttributeVisibility = Visibility.Collapsed;
        protected Visibility _AttributePropertyVisibility = Visibility.Collapsed;
        protected Visibility _FAPIAttributeVisibility = Visibility.Collapsed;
        protected Visibility _FAPIAttributePropertyVisibility = Visibility.Collapsed;
        protected Visibility _UnuqieKeyVisibility = Visibility.Collapsed;
        protected Object _SelectedItem;
        protected string _SelectedItemViewName = null;
        protected string _CheckErrorsText = null;

        #endregion
        public CreateViewViewModel(DTE2 dte) : base()
        {
            this.Dte = dte;
            SelectedModel = new ModelView();
            ModelViews = new ObservableCollection<ModelViewSerializable>();
        }
        public TreeViewItem MainTreeViewRootItem { get; set; }

        public SolutionCodeElement SelectedDbContext
        {
            get { return _SelectedDbContext; }
            set
            {
                if (_SelectedDbContext == value) return;
                _SelectedDbContext = value;
                OnPropertyChanged();
                OnSelectedDbContextChanged();
            }
        }
        public SolutionCodeElement SelectedEntity
        {
            get
            {
                return _SelectedEntity;
            }
            set
            {
                if (_SelectedEntity == value) return;
                _SelectedEntity = value;
                if (_SelectedEntity == null)
                {
                    EntityNameCaption = null;
                }
                else
                {
                    EntityNameCaption = _SelectedEntity.CodeElementFullName;
                }
                OnPropertyChanged();
                OnSelectedEntityChanged();
            }
        }
        public string EntityNameCaption
        {
            get
            {
                return _EntityNameCaption;
            }
            set
            {
                if (_EntityNameCaption == value) return;
                _EntityNameCaption = value;
                OnPropertyChanged();
            }
        }
        public string InvitationCaption
        {
            get
            {
                return _InvitationCaption;
            }
            set
            {
                if (_InvitationCaption == value) return;
                _InvitationCaption = value;
                OnPropertyChanged();
            }
        }
        public ModelView SelectedModel
        {
            get
            {
                return _SelectedModel;
            }
            set
            {
                if (_SelectedModel == value) return;
                _SelectedModel = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ModelViewSerializable> ModelViews { get; set; }
        public DbContextSerializable CurrentDbContext { get; set; }
        public void OnSelectedDbContextChanged()
        {
            if (SelectedModel == null)
            {
                SelectedModel = new ModelView();
            }
            SelectedTreeViewItem = null;
            SelectedModel.ClearModelView();
            if (this.MainTreeViewRootItem != null)
            {
                this.MainTreeViewRootItem.IsSelected = true;
                HintVisibility = Visibility.Collapsed;
                RootVisibility = Visibility.Visible;
            }
        }
        public void OnSelectedEntityChanged()
        {
            if (SelectedModel == null)
            {
                SelectedModel = new ModelView();
            }
            SelectedTreeViewItem = null;
            SelectedModel.ClearModelView();
            if (this.MainTreeViewRootItem != null)
            {
                this.MainTreeViewRootItem.IsSelected = true;
                HintVisibility = Visibility.Collapsed;
                RootVisibility = Visibility.Visible;
            }
        }
        public string ViewNameSufix { get; set; } = "View";
        public string PageViewNameSufix { get; set; } = "ViewPage";
        public string CheckErrorsText
        {
            get
            {
                return _CheckErrorsText;
            }
            set
            {
                if (_CheckErrorsText == value) return;
                _CheckErrorsText = value;
                OnPropertyChanged();
            }
        }
        public bool SelectAllProperties
        {
            get
            {
                return _SelectAllProperties;
            }
            set
            {
                if (_SelectAllProperties == value) return;
                _SelectAllProperties = value;
                OnPropertyChanged();
                OnSelectAllPropertiesChanged();
            }
        }
        public string DestinationProject { get; set; }
        public string DefaultProjectNameSpace { get; set; }
        public string DestinationFolder { get; set; }
        public string DestinationDbSetProppertyName { get; set; }
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
        public Visibility UnuqieKeyVisibility
        {
            get
            {
                return _UnuqieKeyVisibility;
            }
            set
            {
                if (_UnuqieKeyVisibility == value) return;
                _UnuqieKeyVisibility = value;
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

        public void DoAnalize(ModelViewSerializable srcModel)
        {
            if (SelectedModel == null) SelectedModel = new ModelView();
            if ((SelectedEntity == null) || (SelectedDbContext == null))
            {
                SelectedTreeViewItem = null;
                SelectedModel.ClearModelView();
            }
            if ((SelectedEntity.CodeElementRef == null) || (SelectedDbContext.CodeElementRef == null))
            {
                SelectedTreeViewItem = null;
                SelectedModel.ClearModelView();
            }
            (SelectedEntity.CodeElementRef as CodeClass).
                DefineModelView(SelectedDbContext.CodeElementRef as CodeClass, SelectedModel, ViewNameSufix, PageViewNameSufix);
            SelectedModel.ViewProject = DestinationProject;
            SelectedModel.ViewDefaultProjectNameSpace = DefaultProjectNameSpace;
            SelectedModel.ViewFolder = DestinationFolder;
            if (srcModel != null)
            {
                srcModel.ModelViewSerializableAssingTo(SelectedModel, CurrentDbContext, false);
            }
            OnPropertyChanged("SelectedModel");
        }
        public void OnSelectAllPropertiesChanged()
        {
            if (SelectedModel != null)
            {
                if (SelectedModel.ScalarProperties != null)
                {
                    foreach (ModelViewProperty modelViewProperty in SelectedModel.ScalarProperties)
                    {
                        modelViewProperty.IsSelected = SelectAllProperties;
                    }
                }
            }
        }
        public string SelectedItemViewName
        {
            get
            {
                return _SelectedItemViewName;
            }
            set
            {
                if (_SelectedItemViewName == value) return;
                _SelectedItemViewName = value;
                OnPropertyChanged();
                OnSelectedItemViewNameChanged();
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
                return _selectedTreeViewItem;
            }
            set
            {
                if (_selectedTreeViewItem != value)
                {
                    _selectedTreeViewItem = value;
                    if (_selectedTreeViewItem == null)
                    {
                        PropertyVisibility = Visibility.Collapsed;
                        KeyPropertyVisibility = Visibility.Collapsed;
                        RootVisibility = Visibility.Collapsed;
                        ForeignKeyVisibility = Visibility.Collapsed;
                        UnuqieKeyVisibility = Visibility.Collapsed;
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
                        ForeignKeyVisibility = Visibility.Collapsed;
                        UnuqieKeyVisibility = Visibility.Collapsed;
                        AttributePropertyVisibility = Visibility.Collapsed;
                        AttributeVisibility = Visibility.Collapsed;
                        FAPIAttributePropertyVisibility = Visibility.Collapsed;
                        FAPIAttributeVisibility = Visibility.Collapsed;

                        if (_selectedTreeViewItem is TreeViewItem)
                        {
                            TreeViewItem treeViewItem = _selectedTreeViewItem as TreeViewItem;
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
                            else if ("UniqueKeys".Equals(treeViewItem.Tag))
                            {
                                // show tabled data in the future releases
                                HintVisibility = Visibility.Visible;
                            }
                            else
                            {
                                HintVisibility = Visibility.Visible;
                            }
                        }
                        else if (_selectedTreeViewItem is ModelViewKeyProperty)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            KeyPropertyVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewProperty)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            PropertyVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewAttribute)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            AttributeVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewAttributeProperty)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            AttributePropertyVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewFAPIAttribute)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            FAPIAttributeVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewFAPIAttributeProperty)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            FAPIAttributePropertyVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewForeignKey)
                        {
                            SelectedItem = null; // do not change the code here
                            OnForeignKeySelected(_selectedTreeViewItem as ModelViewForeignKey);
                            SelectedItem = _selectedTreeViewItem;
                            SelectedItemViewName = (_selectedTreeViewItem as ModelViewForeignKey).ViewName;
                            ForeignKeyVisibility = Visibility.Visible;
                        }
                        else if (_selectedTreeViewItem is ModelViewUniqueKey)
                        {
                            SelectedItem = _selectedTreeViewItem;
                            UnuqieKeyVisibility = Visibility.Visible;
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
        public void OnForeignKeySelected(ModelViewForeignKey foreignKey)
        {
            if (ModelViews == null) ModelViews = new ObservableCollection<ModelViewSerializable>();
            if (foreignKey == null)
            {
                ModelViews.Clear();
                return;
            }
            if (ModelViews.Any(mv => ((mv.RootEntityFullClassName != foreignKey.NavigationEntityFullName) || (mv.RootEntityUniqueProjectName != foreignKey.NavigationEntityUniqueProjectName))))
            {
                ModelViews.Clear();
            }
            if (CurrentDbContext == null) return;
            if (CurrentDbContext.ModelViews == null) return;
            List<ModelViewSerializable> result = CurrentDbContext.ModelViews
                .Where(mv => (mv.RootEntityFullClassName == foreignKey.NavigationEntityFullName) && (mv.RootEntityUniqueProjectName == foreignKey.NavigationEntityUniqueProjectName))
                .ToList();
            if (result != null)
            {
                if (result.Count != ModelViews.Count)
                {
                    result.ForEach(r =>
                    {
                        if (!ModelViews.Any(mv => mv.ViewName == r.ViewName))
                        {
                            ModelViews.Add(r);
                        }
                    });
                }
            }
        }
        public void OnSelectedItemViewNameChanged()
        {
            if (SelectedItem == null) return;
            ModelViewForeignKey foreignKey = SelectedItem as ModelViewForeignKey;
            if (foreignKey == null) return;
            //foreignKey.IsAssinging = true;
            foreignKey.ViewName = SelectedItemViewName;
            //foreignKey.IsAssinging = false;
            if (foreignKey.ScalarProperties == null) foreignKey.ScalarProperties = new ObservableCollection<ModelViewProperty>();
            if (string.IsNullOrEmpty(foreignKey.ViewName)) return;
            if (foreignKey.ScalarProperties.Count > 0) return;
            ModelViewSerializable modelViewSerializable = ModelViews.FirstOrDefault(mv => mv.ViewName == foreignKey.ViewName);
            if (modelViewSerializable.ScalarProperties == null) return;
            modelViewSerializable.ScalarProperties.ForEach(mv => foreignKey.ScalarProperties.Add(mv.ModelViewPropertySerializableAssingTo(new ModelViewProperty())));
            bool isRequired = foreignKey.ModelViewForeignKeyIsRequired();
            foreach (ModelViewProperty prop in foreignKey.ScalarProperties)
            {
                prop.IsRequiredInView = prop.IsRequiredInView && isRequired;
            }
            foreignKey.IsAssinging = true;
            foreignKey.ForeignKeyPrefix = foreignKey.NavigationName + modelViewSerializable.ViewName;
            foreignKey.IsAssinging = false;
            foreignKey.OnModelViewForeignKeyPrefixChanged();
            foreignKey.ModelViewForeignKeyUpdateForeignKeyNameChain();
        }

        #region UiBtnCommandCheckErrors
        private ICommand _UiBtnCommandCheckErrors;
        public ICommand UiBtnCommandCheckErrors
        {
            get
            {
                return _UiBtnCommandCheckErrors ?? (_UiBtnCommandCheckErrors = new CommandHandler((param) => UiBtnCommandCheckErrorsAction(param), (param) => UiBtnCommandCheckErrorsCanExecute(param)));
            }
        }
        public bool UiBtnCommandCheckErrorsCanExecute(Object param)
        {
            return true;
        }
        public virtual void UiBtnCommandCheckErrorsAction(Object param)
        {
            CheckErrorsText = SelectedModel.CheckCorrect();
        }
        #endregion

        #region UiBtnCommandHints
        private ICommand _UiBtnCommandHints;
        public ICommand UiBtnCommandHints
        {
            get
            {
                return _UiBtnCommandHints ?? (_UiBtnCommandHints = new CommandHandler((param) => UiBtnCommandHintsAction(param), (param) => UiBtnCommandHintsCanExecute(param)));
            }
        }
        public bool UiBtnCommandHintsCanExecute(Object param)
        {
            return true;
        }
        public virtual void UiBtnCommandHintsAction(Object param)
        {
            CheckErrorsText = SelectedModel.CheckForHints();
        }
        #endregion

    }
}
