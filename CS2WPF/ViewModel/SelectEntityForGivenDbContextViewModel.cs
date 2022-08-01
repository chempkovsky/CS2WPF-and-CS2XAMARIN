using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.AnalyzeOnModelCreating;
using CS2WPF.View;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class SelectEntityForGivenDbContextViewModel : BaseSelectSourceViewModel
    {
        protected SolutionCodeElement _SelectedDbContext = null;
        protected UserControlOneCommand _ActionUserControl = null;
        protected UserControlOneCommand _PrimKeyUserControl = null;
        protected string _UiCommandProppertyName = "";
        public SelectEntityForGivenDbContextViewModel(DTE2 dte) : base(dte)
        {
            SelectProjectCaption = "Select Entity Project:";
            SelectClassCaption = "Select Entity Class:";
            SelectedProppertyNameCaption = "DbContext Property Name for selected class:";
            IsSelectedClassNameReadOnly = true;
            SelectedProppertyNameVisibility = Visibility.Visible;
            UiCommandButtonClicked = new ButtonClickedNotificationService();
        }

        public Visibility UiCommandButtonVisibility { get; set; } = Visibility.Visible;

        public ButtonClickedNotificationService UiCommandButtonClicked;
        public SolutionCodeElement SelectedDbContext
        {
            get { return _SelectedDbContext; }
            set
            {
                if (_SelectedDbContext == value) return;
                _SelectedDbContext = value;
                DoAnalize();

            }
        }

        public void DefineUiCaptions()
        {
            UiCommandCaption1 = "The current DbContext does not contain DbSet<> property";
            UiCommandCaption2 = "with the same type as the selected class has !";
            UiCommandCaption3 = null;
            UiCommandLabelCaption = "DbContext Property Name";
            UiCommandButtonCaption = "  Add Required Property to DbContext  ";
            UiCommandProppertyNameVisibility = Visibility.Visible;
            UiCommandCaption2BackGround = 0;
            OnPropertyChanged("UiCommandProppertySufix");
            OnPropertyChanged("UiCommandCaption1");
            OnPropertyChanged("UiCommandCaption2");
            OnPropertyChanged("UiCommandCaption3");
            OnPropertyChanged("UiCommandLabelCaption");
            OnPropertyChanged("UiCommandButtonCaption");
            OnPropertyChanged("UiCommandProppertyNameVisibility");
            OnPropertyChanged("UiCommandCaption2BackGround");
        }
        public void ReDefineUiCaptions(FluentAPIKey primKey)
        {
            if (HasPrimKey)
            {
                UiCommandCaption1 = "A Prim.Key was found for the selected Entity. Detection type [" + primKey.KeySourceDisplay + "]";
                UiCommandCaption2 = "Key Fileds [";
                foreach (FluentAPIProperty pp in primKey.KeyProperties)
                {
                    UiCommandCaption2 = UiCommandCaption2 + " " + pp.PropName;
                }
                UiCommandCaption2 += " ]";
                if (primKey.SourceCount > 1)
                {
                    UiCommandCaption3 = "The setting had met more than once: " + primKey.SourceCount.ToString() + " !!!!";
                }
                else
                {
                    UiCommandCaption3 = null;
                }
                UiCommandCaption2BackGround = 10 + (int)primKey.KeySource;
            }
            else
            {
                UiCommandCaption2BackGround = 9;
                UiCommandCaption1 = "A Primary Key was not found for the selected Entity";
                UiCommandCaption2 = "It is recommended to make the primary key settings in OnModelCreating(...)-method";
                UiCommandCaption3 = null;
            }

            UiCommandLabelCaption = " Add/Modify Primaky Key setting in OnModelCreating(...)-method ";
            UiCommandButtonCaption = "  Modify  ";
            UiCommandProppertyNameVisibility = Visibility.Collapsed;
            OnPropertyChanged("UiCommandProppertySufix");
            OnPropertyChanged("UiCommandCaption1");
            OnPropertyChanged("UiCommandCaption2");
            OnPropertyChanged("UiCommandCaption3");
            OnPropertyChanged("UiCommandLabelCaption");
            OnPropertyChanged("UiCommandButtonCaption");
            OnPropertyChanged("UiCommandProppertyNameVisibility");
            OnPropertyChanged("UiCommandCaption2BackGround");
            // we had to define UiCommandProppertyName for UiBtnCommandCanExecute() has to return true
            UiCommandProppertyName = "NoName";

        }

        #region Properties
        public Visibility UiCommandProppertyNameVisibility { get; set; } = Visibility.Visible;
        public string UiCommandProppertySufix { get; set; } = "DbSet";
        public string UiCommandCaption1 { get; set; }
        public string UiCommandCaption2 { get; set; }
        public string UiCommandCaption3 { get; set; }
        public string UiCommandLabelCaption { get; set; }
        public string UiCommandButtonCaption { get; set; }
        public string UiCommandProppertyName
        {
            get
            {
                return _UiCommandProppertyName;
            }
            set
            {
                _UiCommandProppertyName = value;
                OnPropertyChanged("UiCommandProppertyName");
            }
        }
        public int UiCommandCaption2BackGround { get; set; }
        public bool HasPrimKey { get; set; }
        public override SolutionCodeElement SelectedCodeElement
        {
            get
            {
                return selectedCodeElement;
            }
            set
            {
                if (selectedCodeElement == value) return;
                selectedCodeElement = value;
                string scn = "";
                string sns = "";
                if (selectedCodeElement != null)
                {
                    scn = selectedCodeElement.CodeElementName;
                    sns = selectedCodeElement.CodeElementFullName.Replace("." + scn, "");
                }
                SelectedClassName = scn;
                SelectedNameSpace = sns;
                OnPropertyChanged("SelectedClassName");
                OnPropertyChanged("SelectedNameSpace");
                OnPropertyChanged("SelectedCodeElement");
                DoAnalize();
            }
        }
        #endregion

        #region Helper methods
        public override void CollectProjectClasses(SolutionProject sp)
        {
            base.CollectProjectClasses(sp);
            if (sp == null) return;
            if (sp.ProjectRef == null) return;
            if (sp.ProjectRef.CodeModel == null) return;
            List<SolutionCodeElement> localList = new List<SolutionCodeElement>();

            foreach (EnvDTE.CodeElement ce in sp.ProjectRef.CodeModel.CodeElements)
            {
                if (ce.Kind == EnvDTE.vsCMElement.vsCMElementClass)
                {
                    if (ce.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
                    {
                        SolutionCodeElement sce = new SolutionCodeElement()
                        {
                            CodeElementName = ce.Name,
                            CodeElementFullName = ce.FullName,
                            CodeElementRef = ce
                        };
                        //ComboItemsSourceCodeElements.Add(sce);
                        localList.Add(sce);
                    }
                }
                else
                {
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        CodeNamespace cn = (CodeNamespace)ce;
                        InternalCollectProjectClasses(cn, localList);
                    }
                }
            }
            localList.Sort((a, b) => string.Compare(a.CodeElementFullName, b.CodeElementFullName));
            localList.ForEach(ce => ComboItemsSourceCodeElements.Add(ce));
        }
        protected bool InternalCollectProjectClasses(CodeNamespace parentCodeNamespace, List<SolutionCodeElement> list)
        {
            foreach (EnvDTE.CodeElement ce in parentCodeNamespace.Members)
            {
                if (ce.Kind == EnvDTE.vsCMElement.vsCMElementClass)
                {
                    if (ce.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
                    {
                        SolutionCodeElement sce = new SolutionCodeElement()
                        {
                            CodeElementName = ce.Name,
                            CodeElementFullName = ce.FullName,
                            CodeElementRef = ce
                        };
                        //ComboItemsSourceCodeElements.Add(sce);
                        list.Add(sce);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        CodeNamespace cn = (CodeNamespace)ce;
                        if (!InternalCollectProjectClasses(cn, list))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public virtual void DoAnalize()
        {
            HasPrimKey = false;
            SelectedProppertyName = "";
            ActionUserControl = null;

            if (SelectedDbContext != null)
            {
                if (SelectedDbContext.CodeElementRef != null)
                {
                    if (SelectedCodeElement != null)
                    {
                        if (SelectedCodeElement.CodeElementRef != null)
                        {
                            SelectedProppertyName =
                                (SelectedDbContext.CodeElementRef as CodeClass).GetDbContextDbSetPropertyNameHelper(SelectedCodeElement.CodeElementName, SelectedCodeElement.CodeElementFullName);
                            if (string.IsNullOrEmpty(SelectedProppertyName))
                            {
                                if (_ActionUserControl == null)
                                {
                                    _ActionUserControl = new UserControlOneCommand(this);
                                }
                                UiCommandProppertyName = SelectedCodeElement.CodeElementName + UiCommandProppertySufix;
                                DefineUiCaptions();
                                ActionUserControl = _ActionUserControl;
                            }
                            else
                            {
                                // Primary key analize here
                                FluentAPIKey primKey = new FluentAPIKey();
                                (SelectedCodeElement.CodeElementRef as CodeClass).CollectPrimaryKeyPropsHelper(primKey, SelectedDbContext.CodeElementRef as CodeClass);
                                if (primKey.KeyProperties != null)
                                {
                                    HasPrimKey = primKey.KeyProperties.Count > 0;
                                }
                                if (_PrimKeyUserControl == null)
                                {
                                    _PrimKeyUserControl = new UserControlOneCommand(this);
                                }
                                ReDefineUiCaptions(primKey);
                                ActionUserControl = _PrimKeyUserControl;
                            }
                        }
                    }
                }
            }


            OnPropertyChanged("ActionUserControl");
            OnPropertyChanged("SelectedProppertyName");

            CheckIsReady();
        }

        public override void CheckIsReady()
        {
            IsReady.DoNotify(this, !((!HasPrimKey) || string.IsNullOrEmpty(SelectedClassName) || (SelectedCodeElement == null) || string.IsNullOrEmpty(SelectedProppertyNameCaption)));
        }
        #endregion

        #region UiBtnCommand
        private ICommand _UiBtnCommand;
        public ICommand UiBtnCommand
        {
            get
            {
                return _UiBtnCommand ?? (_UiBtnCommand = new CommandHandler((param) => UiBtnCommandAction(param), (param) => UiBtnCommandCanExecute(param)));
            }
        }
        public bool UiBtnCommandCanExecute(Object param)
        {
            return !string.IsNullOrEmpty(UiCommandProppertyName);
        }
        public virtual void UiBtnCommandAction(Object param)
        {
            if (ActionUserControl == _ActionUserControl)
            {
                // add DBSet property to DB Context
                (SelectedDbContext.CodeElementRef as CodeClass).AddDbSetPropertyHelper(SelectedCodeElement.CodeElementRef as CodeClass, UiCommandProppertyName);
                DoAnalize();
            }
            else
            {
                UiCommandButtonClicked.DoNotify(this);
            }
        }
        #endregion

    }
}
