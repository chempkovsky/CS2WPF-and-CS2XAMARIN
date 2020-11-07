using CS2WPF.Helpers.UI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using System;
using System.IO;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class InvitationViewModel : IsReadyViewModel
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/visualstudio/extensibility/ide-guids?view=vs-2019
        /// </summary>
        /// Guid PhysicalFile_guid   = new Guid("6bb5f8ee-4483-11d3-8bcf-00c04f8ec28c");
        /// Guid PhysicalFolder_guid = new Guid("6bb5f8ef-4483-11d3-8bcf-00c04f8ec28c");
        /// Guid SubProject_guid     = new Guid("EA6618E8-6E24-4528-94BE-6889FE16485C");
        /// hGuid VirtualFolder_guid  = new Guid("6bb5f8f0-4483-11d3-8bcf-00c04f8ec28c");
        /// 
        #region Fieds
        string _ErrorText;
        string _InvitationHint;
        string _DestinationProject;
        string _DefaultProjectNameSpace;
        string _DestinationFolder;
        string _WizardName;
        string _DestinationProjectRootFolder="";

        string InvitationHintStr = "Press Next button to get started";
        string DteNotDefinedError = "Can not continue: Dte is not defined.";
        string NoDestinationError = "Can not continue: You have not selected destination folder or project.";
        string OneItemSelectedError = "Can not continue: Only one folder or project can be selected as destination.";
        string ItemTypeError = "Can not continue: Folder or project must be selected as destination.";
        #endregion



        
        public string InvitationHint 
        { 
            get { 
                return _InvitationHint; 
            }
            set {
                if (_InvitationHint == value) return;
                _InvitationHint = value;
                OnPropertyChanged();
            }
        }
        public string ErrorText
        {
            get
            {
                return _ErrorText;
            }
            set
            {
                if (_ErrorText == value) return;
                _ErrorText = value;
                OnPropertyChanged();
            }
        }
        public string DestinationProject
        {
            get
            {
                return _DestinationProject;
            }
            set
            {
                if (_DestinationProject == value) return;
                _DestinationProject = value;
                OnPropertyChanged();
            }
        }
        public string DefaultProjectNameSpace
        {
            get
            {
                return _DefaultProjectNameSpace;
            }
            set
            {
                if (_DefaultProjectNameSpace == value) return;
                _DefaultProjectNameSpace = value;
                OnPropertyChanged();
            }
        }
        public string DestinationFolder
        {
            get
            {
                return _DestinationFolder;
            }
            set
            {
                if (_DestinationFolder == value) return;
                _DestinationFolder = value;
                OnPropertyChanged();
            }
        }
        public string DestinationProjectRootFolder
        {
            get
            {
                return _DestinationProjectRootFolder;
            }
            set
            {
                if (_DestinationProjectRootFolder == value) return;
                _DestinationProjectRootFolder = value;
                OnPropertyChanged();
            }
        }

        public string WizardName
        {
            get
            {
                return _WizardName;
            }
            set
            {
                if (_WizardName == value) return;
                _WizardName = value;
                OnPropertyChanged();
            }
        }


        public InvitationViewModel(): base()
        {
        }

        protected void DefineDestinationProject(Project project)
        {
            if (project == null) return;
            DestinationProject = project.UniqueName;
        }

        protected void DefineDestinationProjectRootFolder(Project project)
        {
            if (project == null) return;
            DestinationProjectRootFolder = "";
            Property prop = project.Properties.Item("FullPath");
            if(prop != null)
            {
                DestinationProjectRootFolder = prop.Value as string;
            }
            if (string.IsNullOrEmpty(DestinationProjectRootFolder))
            {
                string fld = project.FullName;
                DestinationProjectRootFolder = Path.GetFileNameWithoutExtension(fld);
            }
        }


        protected void DefineDestinationFoldersChain(ProjectItem prjItm)
        {
            if (!string.IsNullOrEmpty(DestinationFolder)) DestinationFolder = "\\" + DestinationFolder;
            DestinationFolder = prjItm.Name + DestinationFolder;
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
        protected void DefineDestinationNameSpace(SelectedItem selItem)
        {
            if (selItem == null) return;
            if (selItem.Project != null)
            {
                // the project is selected as a root
                DefineDestinationProject(selItem.Project);
                DefineDestinationProjectRootFolder(selItem.Project);
                try
                {
                    DefaultProjectNameSpace = selItem.Project.Properties.Item("DefaultNamespace").Value as string;
                }
                catch
                {
                    DefaultProjectNameSpace = "";
                }
                DestinationFolder = "";
                OnPropertyChanged("DestinationFolder");
                OnPropertyChanged("DefaultProjectNameSpace");
            } else
            {
                DefineDestinationProject(selItem.ProjectItem.ContainingProject);
                DefineDestinationProjectRootFolder(selItem.ProjectItem.ContainingProject);
                try
                {
                    DefaultProjectNameSpace = selItem.ProjectItem.ContainingProject.Properties.Item("DefaultNamespace").Value as string;
                } catch
                {
                    DefaultProjectNameSpace = "";
                }
                OnPropertyChanged("DefaultProjectNameSpace");
                DestinationFolder = "";
                DefineDestinationFoldersChain(selItem.ProjectItem);
                OnPropertyChanged("DestinationFolder");
            }
        }
        public void DoAnalise(DTE2 Dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (Dte == null)
            {
                ErrorText = DteNotDefinedError;
                IsReady.DoNotify(this, false);
                return;
            }

            SelectedItems selItems = Dte.SelectedItems;
            if (selItems == null)
            {
                ErrorText = NoDestinationError;
                IsReady.DoNotify(this, false);
                return;
            }
            if (selItems.Count < 1)
            {
                ErrorText = NoDestinationError;
                IsReady.DoNotify(this, false);
                return;
            }
            if (selItems.MultiSelect)
            {
                ErrorText = OneItemSelectedError;
                IsReady.DoNotify(this, false);
                return;
            }

            SelectedItem selItem = selItems.Item(1); // Number 1 is a first item
            
            if (selItem.Project != null)
            {
                // the project is selected as a root
                InvitationHint = InvitationHintStr;
                DefineDestinationNameSpace(selItem);
                IsReady.DoNotify(this, true);
                return;
            }
            if (selItem.ProjectItem == null)
            {
                ErrorText = NoDestinationError;
                IsReady.DoNotify(this, false);
                return;
            }
            Guid ItemGuid;
            if (!Guid.TryParse(selItem.ProjectItem.Kind, out ItemGuid))
            {
                ErrorText = ItemTypeError;
                IsReady.DoNotify(this, false);
                return;
            }
            if (!VSConstants.ItemTypeGuid.PhysicalFolder_guid.Equals(ItemGuid))
            {
                ErrorText = ItemTypeError;
                IsReady.DoNotify(this, false);
                return;
            }
            InvitationHint = InvitationHintStr;
            DefineDestinationNameSpace(selItem);
            IsReady.DoNotify(this, true);
        }
    }
}
