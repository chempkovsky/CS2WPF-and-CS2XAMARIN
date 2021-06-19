using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.View;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class MainWindowBase: NotifyPropertyChangedViewModel
    {
        #region Fieds
        protected DTE2 Dte;
        protected PrismModuleModifier prismModuleModifier;
        protected ITextTemplating TextTemplating;
        protected IVsThreadedWaitDialogFactory DialogFactory;
        protected UserControlInvitation InvitationUC = null;
        #endregion

        public MainWindowBase(PrismModuleModifier prismModuleModifier, DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory)
        {
            CancelClicked = new ButtonClickedNotificationService();
            this.Dte = dte;
            this.prismModuleModifier = prismModuleModifier;
            this.TextTemplating = textTemplating;
            this.DialogFactory = dialogFactory;
            try
            {
                DefineDestinationProject();
            }
            catch
            {
                ;
            }
        }

        public string SuccessNotification { get; set; } = "Step completed successfully!";
        public UserControl CurrentUserControl { get; set; }
        public ButtonClickedNotificationService CancelClicked { get; set; }
        public SelectedItem DestinationSelectedItem { get; set; }
        public string CS2ANGULARViewModelsFileName { get; set; } = "CS2ANGULARViewModels.json";
        public string JsonExtension { get; set; } = "json";
        public string DestinationProjectDefaultNamespace { get; set; } = "";
        public Project DestinationProject { get; set; } = null;
        public string DestinationFoldersChain { get; set; } = "";
        public string DestinationNameSpace
        {
            get
            {
                string result = "";
                if ((!string.IsNullOrEmpty(DestinationProjectDefaultNamespace))
                    &&
                   (!string.IsNullOrEmpty(DestinationFoldersChain)))
                {
                    result = ".";
                }
                return DestinationProjectDefaultNamespace + result + DestinationFoldersChain;
            }
        }
        public string SolutionDirectory { get; set; } = "";


        #region Button Command States
        public int CurrentUiStepId { get; set; } = 0;
        public bool PrevBtnEnabled { get; set; } = false;
        public bool NextBtnEnabled { get; set; } = false;
        public bool SaveBtnEnabled { get; set; } = false;
        public bool CancelBtnEnabled { get; set; } = true;
        #endregion

        #region PrevBtnCommand
        private ICommand _PrevBtnCommand;
        public ICommand PrevBtnCommand
        {
            get
            {
                return _PrevBtnCommand ?? (_PrevBtnCommand = new CommandHandler((param) => PrevBtnCommandAction(param), (param) => PrevBtnCommandCanExecute(param)));
            }
        }
        public bool PrevBtnCommandCanExecute(Object param)
        {
            return PrevBtnEnabled;
        }
        public virtual void PrevBtnCommandAction(Object param)
        {
        }
        #endregion

        #region NextBtnCommand
        private ICommand _NextBtnCommand;
        public ICommand NextBtnCommand => _NextBtnCommand ?? (_NextBtnCommand = new CommandHandler((param) => NextBtnCommandAction(param), (param) => NextBtnCommandCanExecute(param)));
        public bool NextBtnCommandCanExecute(Object param)
        {
            return NextBtnEnabled;
        }
        public virtual void NextBtnCommandAction(Object param)
        {
        }
        #endregion

        #region SaveBtnCommand
        private ICommand _SaveBtnCommand;
        public ICommand SaveBtnCommand
        {
            get
            {
                return _SaveBtnCommand ?? (_SaveBtnCommand = new CommandHandler((param) => SaveBtnCommandAction(param), (param) => SaveBtnCommandCanExecute(param)));
            }
        }
        public bool SaveBtnCommandCanExecute(Object param)
        {
            return SaveBtnEnabled;
        }
        public virtual void SaveBtnCommandAction(Object param)
        {
        }
        #endregion

        #region CancelBtnCommand
        private ICommand _CancelBtnCommand;
        public ICommand CancelBtnCommand
        {
            get
            {
                return _CancelBtnCommand ?? (_CancelBtnCommand = new CommandHandler((param) => CancelBtnCommandAction(param), (param) => CancelBtnCommandCanExecute(param)));
            }
        }
        public bool CancelBtnCommandCanExecute(Object param)
        {
            return CancelBtnEnabled;
        }
        public void CancelBtnCommandAction(Object param)
        {
            CancelClicked.DoNotify(this);
        }
        #endregion


        protected void DefineDestinationProject()
        {
            if (Dte == null) return;
            SelectedItems selItems = Dte.SelectedItems;
            if (selItems == null) return;
            if (selItems.Count < 1) return;
            SelectedItem selItem = selItems.Item(1); // Number 1 is a first item
            if (selItem == null) return;
            if (selItem.Project != null)
            {
                DestinationProject = selItem.Project;
            } else
            {
                if (selItem.ProjectItem == null) return;
                DestinationProject = selItem.ProjectItem.ContainingProject;
            }
        }

        protected void DefineDestinationDefaultNamespace(Project prj)
        {
            this.DestinationProjectDefaultNamespace = "";
            try
            {
                DestinationProjectDefaultNamespace = prj.Properties.Item("DefaultNamespace").Value as string;
                DestinationProject = prj;

            }
            catch
            {
                ;
            }
        }
        protected void DefineDestinationFoldersChain(ProjectItem prjItm)
        {
            if (!string.IsNullOrEmpty(DestinationFoldersChain)) DestinationFoldersChain = "." + DestinationFoldersChain;
            DestinationFoldersChain = prjItm.Name + DestinationFoldersChain;
            if (prjItm.Collection != null)
            {
                if (prjItm.Collection.Parent is ProjectItem)
                {
                    ProjectItem parent = prjItm.Collection.Parent as ProjectItem;
                    Guid ItemGuid;
                    if (Guid.TryParse(parent.Kind, out ItemGuid))
                    {
                        if (VSConstants.ItemTypeGuid.PhysicalFolder_guid.Equals(ItemGuid))
                        {
                            DefineDestinationFoldersChain(parent);
                        }
                    }
                }
            }
        }
        protected virtual void InvitationViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

    }
}
