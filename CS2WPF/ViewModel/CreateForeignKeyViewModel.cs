using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.AnalyzeOnModelCreating;
using CS2WPF.TemplateProcessingHelpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class CreateForeignKeyViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected ITextTemplating textTemplating;
        protected SolutionCodeElement _SelectedDbContext;
        protected SolutionCodeElement _SelectedEntity;
        protected string _InvitationCaption;
        protected string _EntityNameCaption;
        protected int _EntityPropertiesIndex = -1;
        protected int _ForeignKeyPropertiesIndex = -1;
        protected int _EntityNonScalarPropertiesIndex = -1;
        protected int _PrincipalNonScalarPropertiesIndex = -1;
        protected int _ForeignKeyTypesIndex = -1;
        protected string _templateFolder = "";
        protected string _templateOneToOneFolder = "";
        protected string _templateOneToCollectionFolder = "";
        protected string _selectedTemplate;
        protected string _T4TempateText;
        protected FluentAPIForeignKey _SelectedForeignKey = null;
        protected string currentTempateFolder = "";
        protected string masterCodeClassFullName = "";
        protected bool _RefreshCanExecute = false;
        protected string _ErrorsText = null;
        protected bool _IsCascadeOnDelete = false;
        protected int _PrimAndUniqueIndex = -1;
        protected CodeClass _MasterCodeClass = null;
        protected string _SelectedOnDeleteAction = "DeleteBehavior.NoAction";
        #endregion

        public CreateForeignKeyViewModel(DTE2 dte, ITextTemplating textTemplating) : base()
        {
            this.Dte = dte;
            this.textTemplating = textTemplating;
            EntityProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            ForeignKeyProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            EntityNonScalarProperties = new ObservableCollection<string>();
            Templates = new ObservableCollection<string>();
            PrimaryKeyProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            PrincipalNonScalarProperties = new ObservableCollection<FluentAPINavigationProperty>();
            ForeignKeyTypes = new ObservableCollection<NavigationTypeEnum>();
            PrimAndUniqueKeys = new ObservableCollection<FluentAPIKey>();
            TemplateExtention = "*.t4";
            InvitationCaption = "Create(Modify) Foreign key settings for:";
        }
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
        public FluentAPIForeignKey SelectedForeignKey
        {
            get
            {
                return _SelectedForeignKey;
            }
            set
            {
                if (_SelectedForeignKey == value) return;
                _SelectedForeignKey = value;
                OnPropertyChanged();
                OnSelectedForeignKeyChanged();
            }
        }
        public ObservableCollection<FluentAPIExtendedProperty> EntityProperties { get; set; }
        public ObservableCollection<string> EntityNonScalarProperties { get; set; }
        public ObservableCollection<FluentAPINavigationProperty> PrincipalNonScalarProperties { get; set; }
        public ObservableCollection<FluentAPIExtendedProperty> ForeignKeyProperties { get; set; }
        public ObservableCollection<FluentAPIExtendedProperty> PrimaryKeyProperties { get; set; }
        public ObservableCollection<NavigationTypeEnum> ForeignKeyTypes { get; set; }
        public ObservableCollection<FluentAPIKey> PrimAndUniqueKeys { get; set; }
        public List<string> OnDeleteActions { get; set; } = new List<string>()
        {
            "DeleteBehavior.ClientSetNull",
            "DeleteBehavior.Restrict",
            "DeleteBehavior.SetNull",
            "DeleteBehavior.Cascade",
            "DeleteBehavior.ClientCascade",
            "DeleteBehavior.NoAction",
            "DeleteBehavior.ClientNoAction"
        };
        public string SelectedOnDeleteAction
        {
            get { return _SelectedOnDeleteAction; }
            set
            {
                if (_SelectedOnDeleteAction != value)
                {
                    _SelectedOnDeleteAction = value;
                    OnPropertyChanged();
                }
            }
        }
        public int PrimAndUniqueKeysIndex
        {
            get { return _PrimAndUniqueIndex; }
            set
            {
                if (_PrimAndUniqueIndex != value)
                {
                    _PrimAndUniqueIndex = value;
                    OnPropertyChanged();
                    OnPrimUniqueKeysIndexChanged();
                }
            }
        }
        public ObservableCollection<string> Templates { get; set; }
        public bool IsCascadeOnDelete
        {
            get
            {
                return _IsCascadeOnDelete;
            }
            set
            {
                if (_IsCascadeOnDelete == value) return;
                _IsCascadeOnDelete = value;
                OnPropertyChanged();
            }
        }
        public string TemplateExtention { get; set; }
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
        public int EntityPropertiesIndex
        {
            get
            {
                return _EntityPropertiesIndex;
            }
            set
            {
                if (_EntityPropertiesIndex == value) return;
                _EntityPropertiesIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("IsEnabledToForeign");
                OnPropertyChanged("IsEnabledAllFromForeign");
            }
        }
        public int ForeignKeyPropertiesIndex
        {
            get
            {
                return _ForeignKeyPropertiesIndex;
            }
            set
            {
                if (_ForeignKeyPropertiesIndex == value) return;
                _ForeignKeyPropertiesIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("IsEnabledFromForeign");
                OnPropertyChanged("IsEnabledAllFromForeign");
            }
        }
        public int EntityNonScalarPropertiesIndex
        {
            get
            {
                return _EntityNonScalarPropertiesIndex;
            }
            set
            {
                if (_EntityNonScalarPropertiesIndex == value) return;
                _EntityNonScalarPropertiesIndex = value;
                OnPropertyChanged();
                OnEntityNonScalarPropertiesIndexChanged();
            }
        }
        public int PrincipalNonScalarPropertiesIndex
        {
            get
            {
                return _PrincipalNonScalarPropertiesIndex;
            }
            set
            {
                if (_PrincipalNonScalarPropertiesIndex == value) return;
                _PrincipalNonScalarPropertiesIndex = value;
                OnPropertyChanged();
                OnPrincipalNonScalarPropertiesIndexChanged();
            }
        }
        public int ForeignKeyTypesIndex
        {
            get
            {
                return _ForeignKeyTypesIndex;
            }
            set
            {
                if (_ForeignKeyTypesIndex == value) return;
                _ForeignKeyTypesIndex = value;
                OnPropertyChanged();
                OnForeignKeyTypesIndexChanged();
            }
        }
        public string T4TempateText
        {
            get { return _T4TempateText; }
            set
            {
                if (_T4TempateText == value) return;
                _T4TempateText = value;
                OnPropertyChanged();
            }
        }
        public string SelectedTemplate
        {
            get
            {
                return _selectedTemplate;
            }
            set
            {
                if (_selectedTemplate == value) return;
                _selectedTemplate = value;
                OnPropertyChanged();
                OnSelectedTemplateChanged();
            }
        }
        public string TemplateFolder
        {
            get
            {
                return _templateFolder;
            }
            set
            {
                if (_templateFolder == value) return;
                _templateFolder = value;
                if (Templates == null) Templates = new ObservableCollection<string>();
                Templates.Clear();
                SelectedTemplate = "";
                OnPropertyChanged();
                OnPropertyChanged("Templates");
            }
        }
        public string TemplateOneToOneFolder
        {
            get
            {
                return _templateOneToOneFolder;
            }
            set
            {
                if (_templateOneToOneFolder == value) return;
                _templateOneToOneFolder = value;
                if (Templates == null) Templates = new ObservableCollection<string>();
                Templates.Clear();
                SelectedTemplate = "";
                OnPropertyChanged();
                OnPropertyChanged("Templates");
            }
        }
        public string TemplateOneToCollectionFolder
        {
            get
            {
                return _templateOneToCollectionFolder;
            }
            set
            {
                if (_templateOneToCollectionFolder == value) return;
                _templateOneToCollectionFolder = value;
                if (Templates == null) Templates = new ObservableCollection<string>();
                Templates.Clear();
                SelectedTemplate = "";
                OnPropertyChanged();
                OnPropertyChanged("Templates");
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
        public string ErrorsText
        {
            get
            {
                return _ErrorsText;
            }
            set
            {
                if (_ErrorsText == value) return;
                _ErrorsText = value;
                OnPropertyChanged();
            }
        }
        public void DoAnalise(string navigationName = null)
        {
            OnForeignKeyTypesIndexChanged();
            if (EntityNonScalarProperties.Count < 1)
            {
                CollectEntityNonScalarProperties();
            }
            if (EntityProperties.Count < 1)
            {
                CollectEntityScalarProperties();
            }
            if (string.IsNullOrEmpty(navigationName)) return;
            if (EntityNonScalarProperties == null) return;
            if (EntityNonScalarProperties == null) return;
            int i = EntityNonScalarProperties.IndexOf(navigationName);
            if (i > -1)
            {
                int oldval = EntityNonScalarPropertiesIndex;
                EntityNonScalarPropertiesIndex = i;
                if (oldval == i)
                {
                    OnEntityNonScalarPropertiesIndexChanged();
                }
            }
        }
        public void OnSelectedDbContextChanged()
        {
            ForeignKeyPropertiesIndex = -1;
            ForeignKeyProperties.Clear();
        }
        public void OnSelectedEntityChanged()
        {
            PrimAndUniqueKeysIndex = -1;
            ForeignKeyPropertiesIndex = -1;
            EntityPropertiesIndex = -1;
            EntityNonScalarPropertiesIndex = -1;
            EntityProperties.Clear();
            ForeignKeyProperties.Clear();
            PrimAndUniqueKeys.Clear();
            EntityNonScalarProperties.Clear();
        }
        // ready
        public void OnSelectedTemplateChanged()
        {
            T4TempateText = "";
            if (string.IsNullOrEmpty(SelectedTemplate)) return;
            string tempatePath = Path.Combine(currentTempateFolder, SelectedTemplate);
            T4TempateText = File.ReadAllText(tempatePath);
        }
        // ready
        public void CollectEntityScalarProperties()
        {
            ForeignKeyProperties.Clear();
            ForeignKeyPropertiesIndex = -1;
            EntityProperties.Clear();
            EntityPropertiesIndex = -1;
            if (SelectedEntity == null) return;
            if (SelectedEntity.CodeElementRef == null) return;
            CodeClass dbContext = null;
            if (SelectedDbContext != null)
            {
                dbContext = SelectedDbContext.CodeElementRef as CodeClass;
            }
            (SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassAllMappedScalarPropertiesWithDbContext(EntityProperties, null, dbContext);
        }
        // ready
        public void CollectEntityNonScalarProperties()
        {
            EntityNonScalarPropertiesIndex = -1;
            EntityNonScalarProperties.Clear();
            if (SelectedEntity == null) return;
            if (SelectedEntity.CodeElementRef == null) return;
            CodeClass dbContext = null;
            if (SelectedDbContext != null)
            {
                dbContext = SelectedDbContext.CodeElementRef as CodeClass;
            }
            List<string> properties = new List<string>();
            (SelectedEntity.CodeElementRef as CodeClass).
                CollectCodeClassAllMappedNonScalarPropertiesWithDbContext(properties, dbContext);
            properties.ForEach(s => EntityNonScalarProperties.Add(s));
        }
        public void OnSelectedForeignKeyChanged()
        {
            if (SelectedForeignKey == null)
            {
                EntityNonScalarPropertiesIndex = -1;
                return;
            }
            if (EntityNonScalarProperties != null)
            {
                int oldVal = EntityNonScalarPropertiesIndex;
                EntityNonScalarPropertiesIndex = EntityNonScalarProperties.IndexOf(SelectedForeignKey.NavigationName);
                // if ((EntityNonScalarPropertiesIndex == oldVal) && (EntityNonScalarPropertiesIndex > -1))
                // {
                OnEntityNonScalarPropertiesIndexChanged();
                // }
            }
        }
        public void OnEntityNonScalarPropertiesIndexChanged()
        {
            _RefreshCanExecute = false;
            _MasterCodeClass = null;
            masterCodeClassFullName = "";
            PrimaryKeyProperties.Clear();
            PrincipalNonScalarPropertiesIndex = -1;
            PrincipalNonScalarProperties.Clear();
            PrimAndUniqueKeysIndex = -1;
            PrimAndUniqueKeys.Clear();

            _RefreshCanExecute = true;
            if (EntityNonScalarPropertiesIndex < 0) return;
            if (EntityNonScalarProperties.Count <= EntityNonScalarPropertiesIndex) return;
            string navigationName = EntityNonScalarProperties[EntityNonScalarPropertiesIndex];


            if (SelectedEntity == null) return;
            CodeClass currentCodeClass = (SelectedEntity.CodeElementRef as CodeClass);
            if (currentCodeClass == null) return;
            CodeClass dbContext = null;
            if (SelectedDbContext != null)
            {
                dbContext = SelectedDbContext.CodeElementRef as CodeClass;
            }
            CodeProperty codeProperty = currentCodeClass.GetPublicMappedNonScalarPropertyByNameWithDbContext(navigationName, dbContext);
            if (codeProperty == null) return;
            if (codeProperty.Type != null)
            {
                _MasterCodeClass = codeProperty.Type.CodeType as CodeClass;
            }
            if (_MasterCodeClass == null) return;
            masterCodeClassFullName = _MasterCodeClass.Name;





            _MasterCodeClass.CollectAllUniqueKeysHelper(PrimAndUniqueKeys, dbContext);
            FluentAPIKey primKey = new FluentAPIKey();
            _MasterCodeClass.CollectPrimaryKeyPropsHelper(primKey, dbContext);
            if (primKey.KeyProperties != null)
            {
                int order = 0;
                primKey.KeyProperties.ForEach(i => i.PropOrder = order++);
                PrimAndUniqueKeys.Add(primKey);
            }


            List<CodeProperty> masterNavigations =
                _MasterCodeClass.GetPublicMappedNonScalarPropertiesByTypeFullNameWithDbContext(currentCodeClass.FullName, dbContext);
            if (masterNavigations == null) return;
            foreach (CodeProperty masterNavigation in masterNavigations)
            {
                PrincipalNonScalarProperties.Add(new FluentAPINavigationProperty()
                {
                    PropName = masterNavigation.Name,
                    UnderlyingTypeName = currentCodeClass.FullName,
                    FullTypeName = masterNavigation.Type.AsFullName,
                    IsCollection = masterNavigation.IsOfCollectionType()
                });
            }
            DoRefresh();
        }
        public void OnPrincipalNonScalarPropertiesIndexChanged()
        {
            if ((PrincipalNonScalarPropertiesIndex < 0) || (PrincipalNonScalarPropertiesIndex >= PrincipalNonScalarProperties.Count))
            {
                ForeignKeyTypesIndex = -1;
                ForeignKeyTypes.Clear();
                ForeignKeyTypes.Add(NavigationTypeEnum.Unckown);
                return;
            }
            if (PrincipalNonScalarProperties[PrincipalNonScalarPropertiesIndex].IsCollection)
            {
                if (ForeignKeyTypes.Count == 3)
                {
                    if (ForeignKeyTypes.IndexOf(NavigationTypeEnum.OptionalToMany) > -1) return;
                }
                ForeignKeyTypesIndex = -1;
                ForeignKeyTypes.Clear();
                ForeignKeyTypes.Add(NavigationTypeEnum.Unckown);
                ForeignKeyTypes.Add(NavigationTypeEnum.OneToMany);
                ForeignKeyTypes.Add(NavigationTypeEnum.OptionalToMany);
            }
            else
            {
                if (ForeignKeyTypes.Count == 3)
                {
                    if (ForeignKeyTypes.IndexOf(NavigationTypeEnum.OneToOne) > -1) return;
                }
                ForeignKeyTypesIndex = -1;
                ForeignKeyTypes.Clear();
                ForeignKeyTypes.Add(NavigationTypeEnum.OneToOne);
                ForeignKeyTypes.Add(NavigationTypeEnum.OptionalToOne);
            }
        }
        public void OnForeignKeyTypesIndexChanged()
        {
            if (Templates == null) Templates = new ObservableCollection<string>();
            if ((ForeignKeyTypesIndex < 0) || (ForeignKeyTypes.Count <= ForeignKeyTypesIndex))
            {
                SelectedTemplate = "";
                Templates.Clear();
                return;
            }
            switch (ForeignKeyTypes[ForeignKeyTypesIndex])
            {
                case NavigationTypeEnum.OneToMany:
                case NavigationTypeEnum.OptionalToMany:
                    currentTempateFolder = TemplateOneToCollectionFolder;
                    break;
                case NavigationTypeEnum.OneToOne:
                case NavigationTypeEnum.OptionalToOne:
                    currentTempateFolder = TemplateOneToOneFolder;
                    break;
                default:
                    currentTempateFolder = TemplateFolder;
                    break;
            }
            Templates.Clear();
            SelectedTemplate = "";
            string[] files = Directory.GetFiles(currentTempateFolder, TemplateExtention);
            if (files != null)
            {
                foreach (string f in files)
                {
                    Templates.Add(Path.GetFileName(f));
                }
            }
        }
        public string GenerateForeignKeyStatement()
        {
            if ((EntityNonScalarPropertiesIndex < 0) || (EntityNonScalarProperties.Count <= EntityNonScalarPropertiesIndex))
            {
                return null;
            }
            if ((PrincipalNonScalarPropertiesIndex < 0) || (PrincipalNonScalarProperties.Count <= PrincipalNonScalarPropertiesIndex))
            {
                return null;
            }
            if ((ForeignKeyTypesIndex < 0) || (ForeignKeyTypes.Count <= ForeignKeyTypesIndex))
            {
                return null;
            }

            if (SelectedEntity == null) return null;
            CodeClass currentCodeClass = (SelectedEntity.CodeElementRef as CodeClass);
            if (currentCodeClass == null) return null;
            string detailCodeClassFullName = currentCodeClass.Name;

            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            List<string> foreignKeyProperties = new List<string>();
            foreach (FluentAPIExtendedProperty itm in this.ForeignKeyProperties)
            {
                foreignKeyProperties.Add(itm.PropName);
            }
            List<string> masterKeyProperties = new List<string>();
            foreach (FluentAPIExtendedProperty itm in this.PrimaryKeyProperties)
            {
                masterKeyProperties.Add(itm.PropName);
            }
            string tempatePath = ""; //Path.Combine(TemplateFolder, SelectedTemplate);

            textTemplatingSessionHost.Session["DetailClassFullName"] = detailCodeClassFullName;
            textTemplatingSessionHost.Session["MasterClassFullName"] = masterCodeClassFullName;
            textTemplatingSessionHost.Session["IsRequired"] = (ForeignKeyTypes[ForeignKeyTypesIndex] == NavigationTypeEnum.OneToMany) ||
                                                              (ForeignKeyTypes[ForeignKeyTypesIndex] == NavigationTypeEnum.OneToOne);
            textTemplatingSessionHost.Session["WillCascadeOnDelete"] = SelectedOnDeleteAction == "DeleteBehavior.Cascade";
            textTemplatingSessionHost.Session["NavigationName"] = EntityNonScalarProperties[EntityNonScalarPropertiesIndex];
            textTemplatingSessionHost.Session["InverseNavigationName"] = PrincipalNonScalarProperties[PrincipalNonScalarPropertiesIndex].PropName;
            textTemplatingSessionHost.Session["ForeignKeyProperties"] = foreignKeyProperties;
            textTemplatingSessionHost.Session["MasterKeyProperties"] = masterKeyProperties;
            textTemplatingSessionHost.Session["DeleteBehavior"] = SelectedOnDeleteAction;


            string result = textTemplating.ProcessTemplate(tempatePath, T4TempateText, tpCallback);
            if (tpCallback.ProcessingErrors != null)
            {
                if (tpCallback.ProcessingErrors.Count > 0)
                {
                    string GenerateError = "Error: ";
                    foreach (TPError tpError in tpCallback.ProcessingErrors)
                    {
                        GenerateError = GenerateError + tpError.ToString() + "\n";
                    }
                    MessageBox.Show(GenerateError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            return result;
        }
        public void DoRefresh()
        {
            if (EntityNonScalarPropertiesIndex < 0) return;
            if (EntityNonScalarProperties.Count <= EntityNonScalarPropertiesIndex) return;
            if (SelectedEntity == null) return;
            if (SelectedEntity.CodeElementRef == null) return;
            string navigationName = EntityNonScalarProperties[EntityNonScalarPropertiesIndex];
            CodeClass dbContext = null;
            if (SelectedDbContext != null)
            {
                dbContext = SelectedDbContext.CodeElementRef as CodeClass;
            }
            List<FluentAPIForeignKey> frnKeys =
                (SelectedEntity.CodeElementRef as CodeClass).CollectForeignKeys(dbContext, new List<string> { navigationName }, true);
            if (frnKeys == null)
            {
                ErrorsText = "No Fk detected";
                return;
            }
            if (frnKeys.Count < 1)
            {
                ErrorsText = "No Fk detected";
                return;
            }
            string className = (SelectedEntity.CodeElementRef as CodeClass).Name;
            if (frnKeys[0].HasErrors)
            {
                ErrorsText = frnKeys[0].ErrorsText;
            }
            else
            {
                ErrorsText = null;
            }
            IsCascadeOnDelete = frnKeys[0].IsCascadeDelete;
            SelectedOnDeleteAction = frnKeys[0].DeleteBehavior;
            if (frnKeys[0].EntityName == className)
            {
                string navName = frnKeys[0].InverseNavigationName;
                if (PrincipalNonScalarProperties != null)
                {
                    FluentAPINavigationProperty np = PrincipalNonScalarProperties.FirstOrDefault(p => p.PropName == navName);
                    if (np != null)
                    {
                        PrincipalNonScalarPropertiesIndex = PrincipalNonScalarProperties.IndexOf(np);
                    }
                    else
                    {
                        PrincipalNonScalarPropertiesIndex = -1;
                    }
                }

                ForeignKeyProperties.Clear();
                if ((frnKeys[0].ForeignKeyProps != null) && (EntityProperties != null))
                {
                    foreach (FluentAPIProperty itm in frnKeys[0].ForeignKeyProps)
                    {
                        FluentAPIExtendedProperty currItm =
                        EntityProperties.FirstOrDefault(i => i.PropName == itm.PropName);
                        if (currItm != null)
                        {
                            ForeignKeyProperties.Add(currItm);
                        }
                    }
                }
                ForeignKeyTypesIndex = ForeignKeyTypes.IndexOf(frnKeys[0].NavigationType);

                // detect PrimAndUniqueKeysIndex
                if (frnKeys[0].PrincipalKeyProps == null)
                {
                    PrimAndUniqueKeysIndex = PrimAndUniqueKeys.IndexOf(PrimAndUniqueKeys.FirstOrDefault(p => p.IsPrimary));
                }
                else
                {
                    PrimAndUniqueKeysIndex = PrimAndUniqueKeys.IndexOf(PrimAndUniqueKeys.GetFluentAPIKeyWithIdenticalListOfNames(frnKeys[0].PrincipalKeyProps));
                }
            }
            else
            {
                PrincipalNonScalarPropertiesIndex = -1;
                ForeignKeyProperties.Clear();
            }

        }

        #region UiBtnCommandToForeign
        private ICommand _UiBtnCommandToForeign;
        public ICommand UiBtnCommandToForeign
        {
            get
            {
                return _UiBtnCommandToForeign ?? (_UiBtnCommandToForeign = new CommandHandler((param) => UiBtnCommandToForeignAction(param), (param) => UiBtnCommandToForeignCanExecute(param)));
            }
        }
        public bool UiBtnCommandToForeignCanExecute(Object param)
        {
            return (EntityPropertiesIndex > -1) && (EntityPropertiesIndex < EntityProperties.Count);
        }
        public virtual void UiBtnCommandToForeignAction(Object param)
        {
            if ((EntityPropertiesIndex > -1) && (EntityPropertiesIndex < EntityProperties.Count))
            {
                var itm = EntityProperties[EntityPropertiesIndex];
                if (!ForeignKeyProperties.Contains(itm))
                {
                    ForeignKeyProperties.Add(itm);
                }
            }
        }
        #endregion

        #region UiBtnCommandFromForeign
        private ICommand _UiBtnCommandFromForeign;
        public ICommand UiBtnCommandFromForeign
        {
            get
            {
                return _UiBtnCommandFromForeign ?? (_UiBtnCommandFromForeign = new CommandHandler((param) => UiBtnCommandFromForeignAction(param), (param) => UiBtnCommandFromForeignCanExecute(param)));
            }
        }
        public bool UiBtnCommandFromForeignCanExecute(Object param)
        {
            return (ForeignKeyPropertiesIndex > -1) && (ForeignKeyPropertiesIndex < ForeignKeyProperties.Count);
        }
        public virtual void UiBtnCommandFromForeignAction(Object param)
        {
            if ((ForeignKeyPropertiesIndex > -1) && (ForeignKeyPropertiesIndex < ForeignKeyProperties.Count))
            {
                ForeignKeyProperties.RemoveAt(ForeignKeyPropertiesIndex);
                if (ForeignKeyProperties.Count <= ForeignKeyPropertiesIndex)
                {
                    if (ForeignKeyPropertiesIndex > -1) ForeignKeyPropertiesIndex -= 1;
                }
            }
        }
        #endregion

        #region UiBtnCommandAllFromForeign
        private ICommand _UiBtnCommandAllFromForeign;
        public ICommand UiBtnCommandAllFromForeign
        {
            get
            {
                return _UiBtnCommandAllFromForeign ?? (_UiBtnCommandAllFromForeign = new CommandHandler((param) => UiBtnCommandAllFromForeignAction(param), (param) => UiBtnCommandAllFromForeignCanExecute(param)));
            }
        }
        public bool UiBtnCommandAllFromForeignCanExecute(Object param)
        {
            return ForeignKeyProperties.Count > 0;
        }
        public virtual void UiBtnCommandAllFromForeignAction(Object param)
        {
            ForeignKeyProperties.Clear();
            ForeignKeyPropertiesIndex = -1;
        }
        #endregion

        #region UiBtnCommandUp
        private ICommand _UiBtnCommandUp;
        public ICommand UiBtnCommandUp
        {
            get
            {
                return _UiBtnCommandUp ?? (_UiBtnCommandUp = new CommandHandler((param) => UiBtnCommandUpAction(param), (param) => UiBtnCommandUpCanExecute(param)));
            }
        }
        public bool UiBtnCommandUpCanExecute(Object param)
        {
            return (ForeignKeyPropertiesIndex > 0) && (ForeignKeyPropertiesIndex < ForeignKeyProperties.Count);
        }
        public virtual void UiBtnCommandUpAction(Object param)
        {
            if ((ForeignKeyPropertiesIndex > 0) && (ForeignKeyPropertiesIndex < ForeignKeyProperties.Count))
            {
                ForeignKeyProperties.Move(ForeignKeyPropertiesIndex, ForeignKeyPropertiesIndex - 1);
            }
        }
        #endregion

        #region UiBtnCommandDown
        private ICommand _UiBtnCommandDown;
        public ICommand UiBtnCommandDown
        {
            get
            {
                return _UiBtnCommandDown ?? (_UiBtnCommandDown = new CommandHandler((param) => UiBtnCommandDownAction(param), (param) => UiBtnCommandDownCanExecute(param)));
            }
        }
        public bool UiBtnCommandDownCanExecute(Object param)
        {
            return (ForeignKeyPropertiesIndex > -1) && (ForeignKeyPropertiesIndex < ForeignKeyProperties.Count - 1);
        }
        public virtual void UiBtnCommandDownAction(Object param)
        {
            if ((ForeignKeyPropertiesIndex > -1) && (ForeignKeyPropertiesIndex < ForeignKeyProperties.Count - 1))
            {
                ForeignKeyProperties.Move(ForeignKeyPropertiesIndex, ForeignKeyPropertiesIndex + 1);
            }
        }
        #endregion

        #region UiBtnCommandRefresh
        private ICommand _UiBtnCommandRefresh;
        public ICommand UiBtnCommandRefresh
        {
            get
            {
                return _UiBtnCommandRefresh ?? (_UiBtnCommandRefresh = new CommandHandler((param) => UiBtnCommandRefreshAction(param), (param) => UiBtnCommandRefreshCanExecute(param)));
            }
        }
        public bool UiBtnCommandRefreshCanExecute(Object param)
        {
            return _RefreshCanExecute;
        }
        public virtual void UiBtnCommandRefreshAction(Object param)
        {
            DoRefresh();
        }
        #endregion

        #region UiBtnCommandCreate
        private ICommand _UiBtnCommandCreate;
        public ICommand UiBtnCommandCreate
        {
            get
            {
                return _UiBtnCommandCreate ?? (_UiBtnCommandCreate = new CommandHandler((param) => UiBtnCommandCreateAction(param), (param) => UiBtnCommandCreateCanExecute(param)));
            }
        }
        public bool UiBtnCommandCreateCanExecute(Object param)
        {
            return (ForeignKeyProperties.Count > 0) &&
                    (PrimaryKeyProperties.Count == ForeignKeyProperties.Count) &&
                    (!string.IsNullOrEmpty(SelectedTemplate)) &&
                    (PrincipalNonScalarPropertiesIndex > -1) &&
                    (EntityNonScalarPropertiesIndex > -1) &&
                    (!string.IsNullOrEmpty(T4TempateText));
        }
        public virtual void UiBtnCommandCreateAction(Object param)
        {
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CodeElementRef != null)
                {
                    if (SelectedDbContext != null)
                    {
                        if (SelectedDbContext.CodeElementRef != null)
                        {
                            string ForeignKeyStatement = GenerateForeignKeyStatement();
                            if (!string.IsNullOrEmpty(ForeignKeyStatement))
                            {
                                (SelectedEntity.CodeElementRef as CodeClass)
                                    .AddForeignKeyDeclaration(SelectedDbContext.CodeElementRef as CodeClass, ForeignKeyStatement, EntityNonScalarProperties[EntityNonScalarPropertiesIndex]);
                                MessageBox.Show("Operation completed successfully", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                                // CollectForeignKeyProperties();
                            }
                        }
                    }

                }
            }
        }
        #endregion

        //
        // EntityNonScalarPropertiesIndex
        //
        protected void OnPrimUniqueKeysIndexChanged()
        {
            PrimaryKeyProperties.Clear();
            if ((PrimAndUniqueKeysIndex < 0) || (PrimAndUniqueKeys == null) || (PrincipalNonScalarProperties == null) || (PrincipalNonScalarPropertiesIndex < 0))
            {
                return;
            }
            if ((PrimAndUniqueKeys.Count <= PrimAndUniqueKeysIndex) || (PrincipalNonScalarProperties.Count <= PrincipalNonScalarPropertiesIndex)) return;
            if (_MasterCodeClass == null) return;
            CodeClass dbContext = null;
            if (SelectedDbContext != null)
            {
                dbContext = SelectedDbContext.CodeElementRef as CodeClass;
            }
            _MasterCodeClass.CollectCodeClassAllMappedScalarPropertiesWithDbContext(PrimaryKeyProperties, PrimAndUniqueKeys[PrimAndUniqueKeysIndex].KeyProperties, dbContext);
        }

    }
}
