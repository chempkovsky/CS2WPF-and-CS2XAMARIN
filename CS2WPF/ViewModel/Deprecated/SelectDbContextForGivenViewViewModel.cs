using CS2ANGULAR.Helpers;
using CS2ANGULAR.Model;
using CS2ANGULAR.View;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VSLangProj;
using Microsoft.CodeAnalysis;
using CS2ANGULAR.Model.AnalyzeOnModelCreating;

namespace CS2ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class SelectDbContextForGivenViewViewModel : SelectDbContextViewModel
    {
        protected SerializableViewModel _SelectedViewModel = null;
        public string SelectedViewModelRootClass { get; set; } = "";
        public SerializableViewModel SelectedViewModel
        {
            get
            {
                return _SelectedViewModel;
            }
            set
            {
                _SelectedViewModel = value;
                if (_SelectedViewModel == null)
                {
                    SelectedViewModelRootClass = "";
                }
                else
                {
                    SelectedViewModelRootClass = _SelectedViewModel.RootNodeNameSapce + "." + _SelectedViewModel.RootNodeClassName;
                }
            }
        }

        public SelectDbContextForGivenViewViewModel(DTE2 dte) : base(dte)
        {
            SelectedProppertyNameVisibility = Visibility.Visible;
        }
        //public Visibility UiCommandButtonVisibility { get; set; } = Visibility.Visible;
        public override void DefineUiCaptions()
        {
            UiCommandProppertySufix = "DbSet";
            UiCommandCaption1 = "The current DbContext does not contain DbSet<> property";
            UiCommandCaption2 = "with the same type as the root class of the selected ViewModel has !";
            UiCommandCaption3 = null;
            UiCommandLabelCaption = "DbContext Property Name";
            UiCommandButtonCaption = "  Add Required Property to DbContext  ";
        }

        #region Helper methods
        public override void DoAnaliseDbContext()
        {
            if (SelectedCodeElement == null)
            {
                ActionUserControl = null;
                SelectedProppertyName = "";
                OnPropertyChanged("ActionUserControl");
                OnPropertyChanged("SelectedProppertyName");
                CheckIsReady();
                return;
            }
            CodeClass cc = SelectedCodeElement.CodeElementRef as CodeClass;
            string nameLookFor1 = "System.Data.Entity.DbSet<" + SelectedViewModelRootClass + ">";
            string nameLookFor2 = "Microsoft.EntityFrameworkCore.DbSet<" + SelectedViewModelRootClass + ">";
            
            bool hasProperty = false;
            foreach (CodeElement ce in cc.Members)
            {
                if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty cp = ce as CodeProperty;
                if (cp.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (cp.Type == null) continue;
                if (cp.Type.CodeType == null) continue;
                if (cp.Type.CodeType.Kind != vsCMElement.vsCMElementClass) continue;
                if (nameLookFor1.Equals(cp.Type.AsFullName,StringComparison.OrdinalIgnoreCase) || nameLookFor2.Equals(cp.Type.AsFullName, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedProppertyName = cp.Name;
                    hasProperty = true;
                    break;
                }
            }
            if (hasProperty)
            {
                ActionUserControl = null;
            } else
            {
                if (_ActionUserControl == null)
                {
                    _ActionUserControl = new UserControlOneCommand(this);
                }
                ActionUserControl = _ActionUserControl;
                SelectedProppertyName = "";
                if (SelectedViewModel != null)
                {
                    UiCommandProppertyName = SelectedViewModel.RootNodeClassName + UiCommandProppertySufix;
                } else
                {
                    UiCommandProppertyName = "";
                }
            }
            OnPropertyChanged("UiCommandProppertyName");
            OnPropertyChanged("ActionUserControl");
            OnPropertyChanged("SelectedProppertyName");
            CheckIsReady();
        }
        public override void CheckIsReady()
        {
            IsReady.DoNotify(this, !(string.IsNullOrEmpty(SelectedClassName) || (SelectedCodeElement == null) || string.IsNullOrEmpty(SelectedProppertyName)));
        }
        #endregion

        #region UiBtnCommand
        public override void UiBtnCommandAction(Object param)
        {
            if (string.IsNullOrWhiteSpace(UiCommandProppertyName)) return;
            if (SelectedCodeElement == null) return;
            if (SelectedCodeElement.CodeElementRef == null) return;
            CodeClass cc = SelectedCodeElement.CodeElementRef as CodeClass;
            if (SelectedViewModel != null)
            {
                SolutionProject prj = ComboItemsSourceProjects.Where(p => string.Equals(p.ProjectUniqueName, SelectedViewModel.RootNodeProjectName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (prj != null)
                {
                    if (!string.Equals(SelectedProject.ProjectUniqueName, prj.ProjectUniqueName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (SelectedProject.ProjectRef.Object is VSProject)
                        {
                            (SelectedProject.ProjectRef.Object as VSProject).References.AddProject(prj.ProjectRef);
                            SelectedProject.ProjectRef.Save();
                        }
                    }
                }
            }



            //cc.AddProperty(UiCommandProppertyName , UiCommandProppertyName, "System.Data.Entity.DbSet<" + SelectedViewModelRootClass + ">", -1, vsCMAccess.vsCMAccessPublic, null);
            CodeProperty codeProperty =
                cc.AddProperty(UiCommandProppertyName, UiCommandProppertyName , "DbSet<" + SelectedViewModelRootClass + ">", -1, vsCMAccess.vsCMAccessPublic, null);
            EditPoint editPoint = codeProperty.Getter.StartPoint.CreateEditPoint();
            editPoint.Delete(codeProperty.Getter.EndPoint);
            editPoint.Insert("get ;");

            editPoint = codeProperty.Setter.StartPoint.CreateEditPoint();
            editPoint.Delete(codeProperty.Setter.EndPoint);
            editPoint.Insert("set ;");
            if (cc.ProjectItem != null)
            {
                if (cc.ProjectItem.IsDirty)
                {
                    cc.ProjectItem.Save();
                }
            }



            DoAnaliseDbContext();
        }
        #endregion


    }
}
