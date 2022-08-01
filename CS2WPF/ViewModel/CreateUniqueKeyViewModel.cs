using CS2WPF.Helpers.UI;
using CS2WPF.Model.AnalyzeOnModelCreating;
using CS2WPF.Model;
using CS2WPF.TemplateProcessingHelpers;


using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using CS2WPF.Helpers;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class CreateUniqueKeyViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected ITextTemplating textTemplating;
        protected SolutionCodeElement _SelectedDbContext;
        protected SolutionCodeElement _SelectedEntity;
        protected string _InvitationCaption;
        protected string _EntityNameCaption;
        protected string _UniqueKeyName;
        protected int _EntityPropertiesIndex = -1;
        protected int _UniqueKeyPropertiesIndex = -1;
        protected string _templateFolder;
        protected string _selectedTemplate;
        protected string _T4TempateText;
        protected List<FluentAPIEntityNode> InternalUniqueKeys = new List<FluentAPIEntityNode>();
        #endregion
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
        public string TemplateExtention { get; set; }
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
                OnPropertyChanged("TemplateFolder");
                OnPropertyChanged("Templates");
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
                OnPropertyChanged("IsEnabledToUnique");
                OnPropertyChanged("IsEnabledAllFromUnique");
            }
        }
        public int UniqueKeyPropertiesIndex
        {
            get
            {
                return _UniqueKeyPropertiesIndex;
            }
            set
            {
                if (_UniqueKeyPropertiesIndex == value) return;
                _UniqueKeyPropertiesIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("IsEnabledFromUnique");
                OnPropertyChanged("IsEnabledAllFromUnique");
            }
        }
        public ObservableCollection<string> Templates { get; set; }
        public CreateUniqueKeyViewModel(DTE2 dte, ITextTemplating textTemplating) : base()
        {
            this.Dte = dte;
            this.textTemplating = textTemplating;
            EntityProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            UniqueKeyProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            Templates = new ObservableCollection<string>();
            _UniqueKeyName = "";
            TemplateExtention = "*.t4";
            InvitationCaption = "Create(Modify) unique key settings";
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
        public string UniqueKeyName
        {
            get
            {
                return _UniqueKeyName;
            }
            set
            {
                if (_UniqueKeyName == value) return;
                _UniqueKeyName = value;
                OnPropertyChanged();
                CollectUniqueKeyProperties();
            }
        }

        public ObservableCollection<string> UniqueKeys { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<FluentAPIExtendedProperty> EntityProperties { get; set; }
        public ObservableCollection<FluentAPIExtendedProperty> UniqueKeyProperties { get; set; }
        public void OnSelectedDbContextChanged()
        {
            UniqueKeyPropertiesIndex = -1;
            UniqueKeyProperties.Clear();
        }
        public void OnSelectedEntityChanged()
        {
            UniqueKeyPropertiesIndex = -1;
            EntityPropertiesIndex = -1;
            EntityProperties.Clear();
            UniqueKeyProperties.Clear();
        }
        public void DoAnalise()
        {
            if (Templates == null) Templates = new ObservableCollection<string>();
            if (Templates.Count < 1)
            {
                Templates.Clear();
                SelectedTemplate = "";
                string[] files = Directory.GetFiles(TemplateFolder, TemplateExtention);
                if (files != null)
                {
                    foreach (string f in files)
                    {
                        Templates.Add(Path.GetFileName(f));
                    }
                }
            }


            if (EntityProperties.Count < 1)
            {
                if (SelectedEntity != null)
                {
                    if (SelectedEntity.CodeElementRef != null)
                    {
                        CodeClass locDbContext = null;
                        if (SelectedDbContext != null)
                        {
                            locDbContext = (SelectedDbContext.CodeElementRef as CodeClass);
                        }
                        (SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassAllMappedScalarPropertiesWithDbContext(EntityProperties, null, locDbContext);
                        OnPropertyChanged("EntityProperties");
                    }
                }
                CollectInternalUniqueKeys();
            }
            if (UniqueKeyProperties.Count < 1)
            {
                CollectUniqueKeyProperties();
            }
        }
        public void CollectInternalUniqueKeys()
        {
            InternalUniqueKeys.Clear();
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CodeElementRef != null)
                {
                    CodeClass locDbContext = null;
                    if (SelectedDbContext != null)
                    {
                        locDbContext = (SelectedDbContext.CodeElementRef as CodeClass);
                    }
                    (SelectedEntity.CodeElementRef as CodeClass).CollectAllUniqueKeysHelper(InternalUniqueKeys, locDbContext);
                }
            }
            string locUniqueKeyName = UniqueKeyName;
            UniqueKeys.Clear();
            foreach (FluentAPIEntityNode iuk in InternalUniqueKeys)
            {
                if (iuk.Methods != null)
                {
                    FluentAPIMethodNode mthd = iuk.Methods.FirstOrDefault(m => "HasName".Equals(m.MethodName));
                    if (mthd != null)
                    {
                        if (mthd.MethodArguments != null)
                        {
                            string ukNm = mthd.MethodArguments.FirstOrDefault();
                            if (!string.IsNullOrEmpty(ukNm))
                            {
                                UniqueKeys.Add(ukNm.Replace("\"", ""));
                            }
                        }
                    }
                }
            }
            UniqueKeyName = locUniqueKeyName;
        }
        public void CollectUniqueKeyProperties()
        {
            UniqueKeyPropertiesIndex = -1;
            UniqueKeyProperties.Clear();
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CodeElementRef != null)
                {
                    if (SelectedDbContext != null)
                    {
                        if (SelectedDbContext.CodeElementRef != null)
                        {
                            string nameToFilter = "";
                            if (!string.IsNullOrEmpty(UniqueKeyName)) nameToFilter = UniqueKeyName;
                            FluentAPIEntityNode enttNd = InternalUniqueKeys.HoldsHasName("\"" + nameToFilter.Replace("\"", "") + "\"");
                            if (enttNd != null)
                            {
                                if (enttNd.Methods != null)
                                {
                                    FluentAPIMethodNode mthdNd = enttNd.Methods.FirstOrDefault(m => m.MethodName == "HasAlternateKey");
                                    if (mthdNd != null)
                                    {
                                        if (mthdNd.MethodArguments != null)
                                        {
                                            FluentAPIKey unkKey = new FluentAPIKey();
                                            unkKey.KeyProperties = new List<FluentAPIProperty>();
                                            int ord = 0;
                                            foreach (string arg in mthdNd.MethodArguments)
                                            {
                                                ord++;
                                                unkKey.KeyProperties.Add(new FluentAPIProperty() { PropOrder = ord, PropName = arg.Replace("\"", "") });
                                            }
                                            (SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassAllMappedScalarPropertiesWithDbContext(UniqueKeyProperties, unkKey.KeyProperties, SelectedDbContext.CodeElementRef as CodeClass);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            OnPropertyChanged("UniqueKeyProperties");
        }
        public void RemoveUniqueKeyInvocations()
        {
            UniqueKeyPropertiesIndex = -1;
            UniqueKeyProperties.Clear();
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CodeElementRef != null)
                {
                    if (SelectedDbContext != null)
                    {
                        if (SelectedDbContext.CodeElementRef != null)
                        {
                            (SelectedEntity.CodeElementRef as CodeClass).RemoveUniqueKeyDeclarations(SelectedDbContext.CodeElementRef as CodeClass, UniqueKeyName);
                            CollectInternalUniqueKeys();
                            CollectUniqueKeyProperties();
                        }
                    }
                }
            }
            OnPropertyChanged("UniqueKeyProperties");
        }
        public void OnSelectedTemplateChanged()
        {
            T4TempateText = "";
            if (string.IsNullOrEmpty(SelectedTemplate)) return;
            string tempatePath = Path.Combine(TemplateFolder, SelectedTemplate);
            T4TempateText = File.ReadAllText(tempatePath);
        }
        public string GenerateUniqueKeyStatement()
        {
            if (string.IsNullOrEmpty(UniqueKeyName))
            {
                MessageBox.Show("Unique Key Name is not defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            List<string> uniqueKeyProperties = new List<string>();
            foreach (FluentAPIExtendedProperty itm in this.UniqueKeyProperties)
            {
                uniqueKeyProperties.Add(itm.PropName);
            }
            string tempatePath = Path.Combine(TemplateFolder, SelectedTemplate);
            textTemplatingSessionHost.Session["UniqueKeyProperties"] = uniqueKeyProperties;
            textTemplatingSessionHost.Session["UniqueKeyName"] = UniqueKeyName;
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

        #region UiBtnCommandToUnique
        private ICommand _UiBtnCommandToUnique;
        public ICommand UiBtnCommandToUnique
        {
            get
            {
                return _UiBtnCommandToUnique ?? (_UiBtnCommandToUnique = new CommandHandler((param) => UiBtnCommandToUniqueAction(param), (param) => UiBtnCommandToUniqueCanExecute(param)));
            }
        }
        public bool UiBtnCommandToUniqueCanExecute(Object param)
        {
            return (EntityPropertiesIndex > -1) && (EntityPropertiesIndex < EntityProperties.Count);
        }
        public virtual void UiBtnCommandToUniqueAction(Object param)
        {
            if ((EntityPropertiesIndex > -1) && (EntityPropertiesIndex < EntityProperties.Count))
            {
                var itm = EntityProperties[EntityPropertiesIndex];
                if (!UniqueKeyProperties.Contains(itm))
                {
                    UniqueKeyProperties.Add(itm);
                }
            }
        }
        #endregion

        #region UiBtnCommandFromUnique
        private ICommand _UiBtnCommandFromUnique;
        public ICommand UiBtnCommandFromUnique
        {
            get
            {
                return _UiBtnCommandFromUnique ?? (_UiBtnCommandFromUnique = new CommandHandler((param) => UiBtnCommandFromUniqueAction(param), (param) => UiBtnCommandFromUniqueCanExecute(param)));
            }
        }
        public bool UiBtnCommandFromUniqueCanExecute(Object param)
        {
            return (UniqueKeyPropertiesIndex > -1) && (UniqueKeyPropertiesIndex < UniqueKeyProperties.Count);
        }
        public virtual void UiBtnCommandFromUniqueAction(Object param)
        {
            if ((UniqueKeyPropertiesIndex > -1) && (UniqueKeyPropertiesIndex < UniqueKeyProperties.Count))
            {
                UniqueKeyProperties.RemoveAt(UniqueKeyPropertiesIndex);
                if (UniqueKeyProperties.Count <= UniqueKeyPropertiesIndex)
                {
                    if (UniqueKeyPropertiesIndex > -1) UniqueKeyPropertiesIndex -= 1;
                }
            }
        }
        #endregion

        #region UiBtnCommandAllFromUnique
        private ICommand _UiBtnCommandAllFromUnique;
        public ICommand UiBtnCommandAllFromUnique
        {
            get
            {
                return _UiBtnCommandAllFromUnique ?? (_UiBtnCommandAllFromUnique = new CommandHandler((param) => UiBtnCommandAllFromUniqueAction(param), (param) => UiBtnCommandAllFromUniqueCanExecute(param)));
            }
        }
        public bool UiBtnCommandAllFromUniqueCanExecute(Object param)
        {
            return UniqueKeyProperties.Count > 0;
        }
        public virtual void UiBtnCommandAllFromUniqueAction(Object param)
        {
            UniqueKeyProperties.Clear();
            UniqueKeyPropertiesIndex = -1;
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
            return (UniqueKeyPropertiesIndex > 0) && (UniqueKeyPropertiesIndex < UniqueKeyProperties.Count);
        }
        public virtual void UiBtnCommandUpAction(Object param)
        {
            if ((UniqueKeyPropertiesIndex > 0) && (UniqueKeyPropertiesIndex < UniqueKeyProperties.Count))
            {
                UniqueKeyProperties.Move(UniqueKeyPropertiesIndex, UniqueKeyPropertiesIndex - 1);
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
            return (UniqueKeyPropertiesIndex > -1) && (UniqueKeyPropertiesIndex < UniqueKeyProperties.Count - 1);
        }
        public virtual void UiBtnCommandDownAction(Object param)
        {
            if ((UniqueKeyPropertiesIndex > -1) && (UniqueKeyPropertiesIndex < UniqueKeyProperties.Count - 1))
            {
                UniqueKeyProperties.Move(UniqueKeyPropertiesIndex, UniqueKeyPropertiesIndex + 1);
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
            return true;
        }
        public virtual void UiBtnCommandRefreshAction(Object param)
        {
            CollectInternalUniqueKeys();
            CollectUniqueKeyProperties();
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
            return (UniqueKeyProperties.Count > 0) && (!string.IsNullOrEmpty(SelectedTemplate)) && (!string.IsNullOrEmpty(T4TempateText));
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
                            string UniqueKeyStatement = GenerateUniqueKeyStatement();
                            if (!string.IsNullOrEmpty(UniqueKeyStatement))
                            {
                                (SelectedEntity.CodeElementRef as CodeClass)
                                    .AddUniqueKeyDeclaration(SelectedDbContext.CodeElementRef as CodeClass, UniqueKeyStatement, UniqueKeyName);
                                MessageBox.Show("Operation completed successfully", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                                CollectInternalUniqueKeys();
                                CollectUniqueKeyProperties();
                            }
                        }
                    }
                }
            }
        }
        #endregion

    }
}
