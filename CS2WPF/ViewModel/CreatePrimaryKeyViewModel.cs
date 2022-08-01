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
    public class CreatePrimaryKeyViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected ITextTemplating textTemplating;
        protected SolutionCodeElement _SelectedDbContext;
        protected SolutionCodeElement _SelectedEntity;
        protected string _InvitationCaption;
        protected string _EntityNameCaption;
        protected int _EntityPropertiesIndex = -1;
        protected int _PrimaryKeyPropertiesIndex = -1;
        protected string _templateFolder;
        protected string _selectedTemplate;
        protected string _T4TempateText;
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
                OnPropertyChanged("IsEnabledToPrimary");
                OnPropertyChanged("IsEnabledAllFromPrimary");
            }
        }
        public int PrimaryKeyPropertiesIndex
        {
            get
            {
                return _PrimaryKeyPropertiesIndex;
            }
            set
            {
                if (_PrimaryKeyPropertiesIndex == value) return;
                _PrimaryKeyPropertiesIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("IsEnabledFromPrimary");
                OnPropertyChanged("IsEnabledAllFromPrimary");
            }
        }
        public ObservableCollection<string> Templates { get; set; }
        public CreatePrimaryKeyViewModel(DTE2 dte, ITextTemplating textTemplating) : base()
        {
            this.Dte = dte;
            this.textTemplating = textTemplating;
            EntityProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            PrimaryKeyProperties = new ObservableCollection<FluentAPIExtendedProperty>();
            Templates = new ObservableCollection<string>();
            TemplateExtention = "*.t4";
            InvitationCaption = "Create(Modify) primary key settings";
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
        public ObservableCollection<FluentAPIExtendedProperty> EntityProperties { get; set; }
        public ObservableCollection<FluentAPIExtendedProperty> PrimaryKeyProperties { get; set; }
        public void OnSelectedDbContextChanged()
        {
            PrimaryKeyPropertiesIndex = -1;
            PrimaryKeyProperties.Clear();
        }
        public void OnSelectedEntityChanged()
        {
            PrimaryKeyPropertiesIndex = -1;
            EntityPropertiesIndex = -1;
            EntityProperties.Clear();
            PrimaryKeyProperties.Clear();
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
                        //(SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassMappedScalarNotNullProperties(EntityProperties);
                        CodeClass locDbContext = null;
                        if (SelectedDbContext != null)
                        {
                            locDbContext = (SelectedDbContext.CodeElementRef as CodeClass);
                        }
                        (SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassAllMappedScalarPropertiesWithDbContext(EntityProperties, null, locDbContext);
                        OnPropertyChanged("EntityProperties");
                    }
                }
            }
            if (PrimaryKeyProperties.Count < 1)
            {
                CollectPrimaryKeyProperties();
            }
        }
        public void CollectPrimaryKeyProperties()
        {
            PrimaryKeyPropertiesIndex = -1;
            PrimaryKeyProperties.Clear();
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CodeElementRef != null)
                {
                    if (SelectedDbContext != null)
                    {
                        if (SelectedDbContext.CodeElementRef != null)
                        {
                            FluentAPIKey primKey = new FluentAPIKey();
                            (SelectedEntity.CodeElementRef as CodeClass).CollectPrimaryKeyPropsHelper(primKey, SelectedDbContext.CodeElementRef as CodeClass);
                            if (primKey.KeyProperties != null) //&& (EntityProperties != null))
                            {
                                int order = 0;
                                primKey.KeyProperties.ForEach(i => i.PropOrder = order++);
                                (SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassAllMappedScalarPropertiesWithDbContext(PrimaryKeyProperties, primKey.KeyProperties, SelectedDbContext.CodeElementRef as CodeClass);
                                // (SelectedEntity.CodeElementRef as CodeClass).CollectCodeClassAllMappedScalarProperties(PrimaryKeyProperties, primKey.KeyProperties);
                                //foreach (FluentAPIProperty itm in primKey.KeyProperties)
                                //{
                                //    FluentAPIExtendedProperty primKeyProp = EntityProperties.FirstOrDefault(e => e.PropName == itm.PropName);
                                //    if (primKeyProp != null)
                                //    {
                                //        PrimaryKeyProperties.Add(primKeyProp);
                                //    } else
                                //    {
                                //        PrimaryKeyProperties.Add(
                                //            new FluentAPIExtendedProperty()
                                //            {
                                //                PropName = itm.PropName
                                //            });
                                //    }
                                //}
                            }
                        }
                    }

                }
            }
            OnPropertyChanged("PrimaryKeyProperties");
        }
        public void RemovePrimaryKeyInvocations()
        {
            PrimaryKeyPropertiesIndex = -1;
            PrimaryKeyProperties.Clear();
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CodeElementRef != null)
                {
                    if (SelectedDbContext != null)
                    {
                        if (SelectedDbContext.CodeElementRef != null)
                        {
                            (SelectedEntity.CodeElementRef as CodeClass).RemovePrimaryKeyDeclarations(SelectedDbContext.CodeElementRef as CodeClass);
                            CollectPrimaryKeyProperties();
                        }
                    }

                }
            }
            OnPropertyChanged("PrimaryKeyProperties");
        }
        public void OnSelectedTemplateChanged()
        {
            T4TempateText = "";
            if (string.IsNullOrEmpty(SelectedTemplate)) return;
            string tempatePath = Path.Combine(TemplateFolder, SelectedTemplate);
            T4TempateText = File.ReadAllText(tempatePath);
        }
        public string GeneratePrimKeyStatement()
        {
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            List<string> primKeyProperties = new List<string>();
            foreach (FluentAPIExtendedProperty itm in this.PrimaryKeyProperties)
            {
                primKeyProperties.Add(itm.PropName);
            }
            string tempatePath = Path.Combine(TemplateFolder, SelectedTemplate);
            textTemplatingSessionHost.Session["PrimKeyProperties"] = primKeyProperties;
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

        #region UiBtnCommandToPrimary
        private ICommand _UiBtnCommandToPrimary;
        public ICommand UiBtnCommandToPrimary
        {
            get
            {
                return _UiBtnCommandToPrimary ?? (_UiBtnCommandToPrimary = new CommandHandler((param) => UiBtnCommandToPrimaryAction(param), (param) => UiBtnCommandToPrimaryCanExecute(param)));
            }
        }
        public bool UiBtnCommandToPrimaryCanExecute(Object param)
        {
            return (EntityPropertiesIndex > -1) && (EntityPropertiesIndex < EntityProperties.Count);
        }
        public virtual void UiBtnCommandToPrimaryAction(Object param)
        {
            if ((EntityPropertiesIndex > -1) && (EntityPropertiesIndex < EntityProperties.Count))
            {
                var itm = EntityProperties[EntityPropertiesIndex];
                if (!PrimaryKeyProperties.Contains(itm))
                {
                    PrimaryKeyProperties.Add(itm);
                }
            }
        }
        #endregion

        #region UiBtnCommandFromPrimary
        private ICommand _UiBtnCommandFromPrimary;
        public ICommand UiBtnCommandFromPrimary
        {
            get
            {
                return _UiBtnCommandFromPrimary ?? (_UiBtnCommandFromPrimary = new CommandHandler((param) => UiBtnCommandFromPrimaryAction(param), (param) => UiBtnCommandFromPrimaryCanExecute(param)));
            }
        }
        public bool UiBtnCommandFromPrimaryCanExecute(Object param)
        {
            return (PrimaryKeyPropertiesIndex > -1) && (PrimaryKeyPropertiesIndex < PrimaryKeyProperties.Count);
        }
        public virtual void UiBtnCommandFromPrimaryAction(Object param)
        {
            if ((PrimaryKeyPropertiesIndex > -1) && (PrimaryKeyPropertiesIndex < PrimaryKeyProperties.Count))
            {
                PrimaryKeyProperties.RemoveAt(PrimaryKeyPropertiesIndex);
                if (PrimaryKeyProperties.Count <= PrimaryKeyPropertiesIndex)
                {
                    if (PrimaryKeyPropertiesIndex > -1) PrimaryKeyPropertiesIndex -= 1;
                }
            }
        }
        #endregion

        #region UiBtnCommandAllFromPrimary
        private ICommand _UiBtnCommandAllFromPrimary;
        public ICommand UiBtnCommandAllFromPrimary
        {
            get
            {
                return _UiBtnCommandAllFromPrimary ?? (_UiBtnCommandAllFromPrimary = new CommandHandler((param) => UiBtnCommandAllFromPrimaryAction(param), (param) => UiBtnCommandAllFromPrimaryCanExecute(param)));
            }
        }
        public bool UiBtnCommandAllFromPrimaryCanExecute(Object param)
        {
            return PrimaryKeyProperties.Count > 0;
        }
        public virtual void UiBtnCommandAllFromPrimaryAction(Object param)
        {
            PrimaryKeyProperties.Clear();
            PrimaryKeyPropertiesIndex = -1;
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
            return (PrimaryKeyPropertiesIndex > 0) && (PrimaryKeyPropertiesIndex < PrimaryKeyProperties.Count);
        }
        public virtual void UiBtnCommandUpAction(Object param)
        {
            if ((PrimaryKeyPropertiesIndex > 0) && (PrimaryKeyPropertiesIndex < PrimaryKeyProperties.Count))
            {
                PrimaryKeyProperties.Move(PrimaryKeyPropertiesIndex, PrimaryKeyPropertiesIndex - 1);
                //PrimaryKeyPropertiesIndex = PrimaryKeyPropertiesIndex - 1;
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
            return (PrimaryKeyPropertiesIndex > -1) && (PrimaryKeyPropertiesIndex < PrimaryKeyProperties.Count - 1);
        }
        public virtual void UiBtnCommandDownAction(Object param)
        {
            if ((PrimaryKeyPropertiesIndex > -1) && (PrimaryKeyPropertiesIndex < PrimaryKeyProperties.Count - 1))
            {
                PrimaryKeyProperties.Move(PrimaryKeyPropertiesIndex, PrimaryKeyPropertiesIndex + 1);
                //PrimaryKeyPropertiesIndex = PrimaryKeyPropertiesIndex + 1;
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
            CollectPrimaryKeyProperties();
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
            return (PrimaryKeyProperties.Count > 0) && (!string.IsNullOrEmpty(SelectedTemplate)) && (!string.IsNullOrEmpty(T4TempateText));
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
                            string PrimKeyStatement = GeneratePrimKeyStatement();
                            if (!string.IsNullOrEmpty(PrimKeyStatement))
                            {
                                (SelectedEntity.CodeElementRef as CodeClass)
                                    .AddPrimaryKeyDeclaration(SelectedDbContext.CodeElementRef as CodeClass, PrimKeyStatement);
                                MessageBox.Show("Operation completed successfully", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                                CollectPrimaryKeyProperties();
                            }
                        }
                    }

                }
            }
        }
        #endregion



    }
}
