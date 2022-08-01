using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.View;
using EnvDTE;
using EnvDTE80;
using System;
using System.Windows;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class SelectDbContextViewModel : BaseSelectSourceViewModel
    {
        protected UserControlOneCommand _ActionUserControl = null;


        protected string _UiCommandProppertyName = "";
        public SelectDbContextViewModel(DTE2 dte) : base(dte)
        {
            UiCommandButtonClicked = new ButtonClickedNotificationService();
            IsSelectedClassNameReadOnly = true;
            SelectedProppertyNameCaption = "DbContext PropertyName:";
            SelectedClassNameCaption = "DbContext Class Name:";
            SelectedNameSpaceCaption = "DbContext NameSpace:";
            SelectProjectCaption = "Select DbContext Project:";
            SelectClassCaption = "Select DbContext Class:";
            SelectedProppertyNameVisibility = Visibility.Collapsed;
            DefineUiCaptions();
        }
        public virtual void DefineUiCaptions()
        {
            UiCommandProppertySufix = "NonameDbContext";
            UiCommandCaption1 = "No DbContext selected";
            UiCommandCaption2 = "Do you want to create new DbContext class?";
            UiCommandCaption3 = null;
            UiCommandLabelCaption = "DbContext Name";
            UiCommandButtonCaption = "  Create New DbContext  ";
            UiCommandProppertyNameVisibility = Visibility.Visible;
            OnPropertyChanged("UiCommandProppertySufix");
            OnPropertyChanged("UiCommandCaption1");
            OnPropertyChanged("UiCommandCaption2");
            OnPropertyChanged("UiCommandCaption3");
            OnPropertyChanged("UiCommandLabelCaption");
            OnPropertyChanged("UiCommandButtonCaption");
            OnPropertyChanged("UiCommandProppertyName");
            OnPropertyChanged("UiCommandProppertyNameVisibility");
        }
        public virtual void ReDefineUiCaptions()
        {

            UiCommandCaption1 = "There is no \"override void OnModelCreating(...)\"-method in DbContext.";
            UiCommandCaption2 = "This method is essential for further steps.";
            UiCommandCaption3 = null;
            UiCommandLabelCaption = "     ";
            UiCommandButtonCaption = "  Create OnModelCreating() method  ";
            UiCommandProppertyNameVisibility = Visibility.Collapsed;

            OnPropertyChanged("UiCommandCaption1");
            OnPropertyChanged("UiCommandCaption2");
            OnPropertyChanged("UiCommandCaption3");
            OnPropertyChanged("UiCommandLabelCaption");
            OnPropertyChanged("UiCommandButtonCaption");
            OnPropertyChanged("UiCommandProppertyName");
            OnPropertyChanged("UiCommandProppertyNameVisibility");
        }
        public string UiCommandProppertySufix { get; set; }
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
        public Visibility UiCommandProppertyNameVisibility { get; set; } = Visibility.Visible;
        public Visibility UiCommandButtonVisibility { get; set; } = Visibility.Visible;

        public override SolutionCodeElement SelectedCodeElement
        {
            get
            {
                return selectedCodeElement;
            }
            set
            {
                selectedCodeElement = value;
                if (selectedCodeElement != null)
                {
                    SelectedClassName = selectedCodeElement.CodeElementName;
                    SelectedNameSpace = selectedCodeElement.CodeElementFullName.Replace("." + selectedCodeElement.CodeElementName, "");
                }
                else
                {
                    SelectedClassName = "";
                    SelectedNameSpace = "";
                    SelectedProppertyName = "";
                }
                OnPropertyChanged("SelectedCodeElement");
                OnPropertyChanged("SelectedClassName");
                OnPropertyChanged("SelectedNameSpace");
                OnPropertyChanged("SelectedProppertyName");
                DoAnaliseDbContext();
            }
        }

        public ButtonClickedNotificationService UiCommandButtonClicked;

        #region Helper methods
        public virtual void DoAnaliseDbContext()
        {
            if (SelectedCodeElement == null)
            {
                if (UiCommandButtonVisibility == Visibility.Visible)
                {
                    if (_ActionUserControl == null)
                    {
                        _ActionUserControl = new UserControlOneCommand(this);
                    }
                    DefineUiCaptions();
                    ActionUserControl = _ActionUserControl;
                    if (string.IsNullOrEmpty(UiCommandProppertyName))
                    {
                        UiCommandProppertyName = UiCommandProppertySufix;
                        OnPropertyChanged("UiCommandProppertyName");
                    }
                }
            }
            else
            {
                DoAnaliseSelectedCodeElement();
            }
            OnPropertyChanged("ActionUserControl");
            OnPropertyChanged("SelectedProppertyName");
            CheckIsReady();
        }
        public override void CollectProjectClasses(SolutionProject sp)
        {
            base.CollectProjectClasses(sp);
            if (sp == null) return;
            if (sp.ProjectRef == null) return;

            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();



            if (sp.ProjectRef.CodeModel == null) return;
            foreach (EnvDTE.CodeElement ce in sp.ProjectRef.CodeModel.CodeElements)
            {
                if (ce.Kind == EnvDTE.vsCMElement.vsCMElementClass)
                {
                    if (ce.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
                    {
                        if ((ce as CodeClass).IsDerivedFrom["System.Data.Entity.DbContext"] || (ce as CodeClass).IsDerivedFrom["Microsoft.EntityFrameworkCore.DbContext"])
                        {
                            SolutionCodeElement sce = new SolutionCodeElement()
                            {
                                CodeElementName = ce.Name,
                                CodeElementFullName = ce.FullName,
                                CodeElementRef = ce
                            };
                            ComboItemsSourceCodeElements.Add(sce);
                        }
                    }
                }
                else
                {
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        EnvDTE.CodeNamespace cn = (EnvDTE.CodeNamespace)ce;
                        InternalCollectProjectClasses(cn);
                    }
                }
            }
        }
        protected bool InternalCollectProjectClasses(EnvDTE.CodeNamespace parentCodeNamespace)
        {
            foreach (EnvDTE.CodeElement ce in parentCodeNamespace.Members)
            {
                if (ce.Kind == EnvDTE.vsCMElement.vsCMElementClass)
                {
                    if (ce.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
                    {
                        if ((ce as CodeClass).IsDerivedFrom["System.Data.Entity.DbContext"] || (ce as CodeClass).IsDerivedFrom["Microsoft.EntityFrameworkCore.DbContext"])
                        {
                            //////// additional checkup here
                            SolutionCodeElement sce = new SolutionCodeElement()
                            {
                                CodeElementName = ce.Name,
                                CodeElementFullName = ce.FullName,
                                CodeElementRef = ce
                            };
                            ComboItemsSourceCodeElements.Add(sce);
                        }
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
                        EnvDTE.CodeNamespace cn = (EnvDTE.CodeNamespace)ce;
                        if (!InternalCollectProjectClasses(cn))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public virtual void DoAnaliseSelectedCodeElement()
        {
            if (SelectedCodeElement == null)
            {
                return;
            }
            CodeClass cc = SelectedCodeElement.CodeElementRef as CodeClass;
            bool hasFunc = false;
            foreach (CodeElement ce in cc.Members)
            {
                if (ce.Kind != vsCMElement.vsCMElementFunction) continue;
                CodeFunction cf = ce as CodeFunction;
                if (cf.Access != vsCMAccess.vsCMAccessProtected) continue;
                // vsCMFunction fk = cf.FunctionKind;
                if ("OnModelCreating".Equals(cf.Name))
                {
                    hasFunc = true;
                    break;
                }
            }
            if (hasFunc)
            {
                ActionUserControl = null;
            }
            else
            {
                if (_ActionUserControl == null)
                {
                    _ActionUserControl = new UserControlOneCommand(this);
                }
                ReDefineUiCaptions();
                ActionUserControl = _ActionUserControl;
            }
        }
        public override void CheckIsReady()
        {
            IsReady.DoNotify(this, !(string.IsNullOrEmpty(SelectedClassName) || (SelectedCodeElement == null) || (ActionUserControl != null)));
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
            if (SelectedCodeElement == null)
            {
                UiCommandButtonClicked.DoNotify(this);
            }
            else
            {
                CodeClass cc = SelectedCodeElement.CodeElementRef as CodeClass;
                if (cc == null) return;
                CodeFunction cf = cc.AddMethodHelper("OnModelCreating", vsCMFunction.vsCMFunctionFunction, vsCMTypeRef.vsCMTypeRefVoid, vsCMAccess.vsCMAccessDefault);
                if (cf == null) return;
                string modelBuilderType = "System.Data.Entity.DbModelBuilder";
                if (cc.IsDerivedFrom["Microsoft.EntityFrameworkCore.DbContext"])
                {
                    modelBuilderType = "Microsoft.EntityFrameworkCore.ModelBuilder";
                }
                cf.AddParameter("modelBuilder", modelBuilderType, null);
                EditPoint editPoint = cf.StartPoint.CreateEditPoint();
                editPoint.Insert("protected override ");
                if (cc.ProjectItem != null)
                {
                    if (cc.ProjectItem.IsDirty)
                    {
                        cc.ProjectItem.Save();
                    }
                }
                DoAnaliseDbContext();

                //foreach(CodeElement ce in cf.Children)
                //{
                //    string nm = ce.FullName;
                //    vsCMElement kind = ce.Kind;
                //    if (nm == "") nm = null;
                //}

                //DoAnaliseDbContext();
                //editPoint = cf.EndPoint.CreateEditPoint();
                //editPoint.CharLeft(1);
                //editPoint.Insert("\n\r int i = 0; \n\r");
                //editPoint.SmartFormat(cf.StartPoint);




            }
        }
        #endregion

    }
}
