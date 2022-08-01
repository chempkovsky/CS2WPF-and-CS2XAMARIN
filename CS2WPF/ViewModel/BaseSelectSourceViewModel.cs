using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class BaseSelectSourceViewModel : IsReadyViewModel
    {

        #region Fields
        protected DTE2 Dte;
        protected SolutionProject selectedProject;
        protected SolutionCodeElement selectedCodeElement;
        protected string selectedClassName = "";
        #endregion

        #region Properties
        public List<SolutionProject> ComboItemsSourceProjects { get; set; }
        public ObservableCollection<SolutionCodeElement> ComboItemsSourceCodeElements { get; set; }
        public SolutionProject SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                selectedProject = value;
                CollectProjectClasses(selectedProject);
                OnPropertyChanged("SelectedCodeElement");
                OnPropertyChanged("ComboItemsSourceCodeElements");
            }
        }
        public virtual SolutionCodeElement SelectedCodeElement
        {
            get
            {
                return selectedCodeElement;
            }
            set
            {
                selectedCodeElement = value;
                OnPropertyChanged("SelectedCodeElement");
            }
        }
        public string SelectedClassName
        {
            get
            {
                return selectedClassName;
            }
            set
            {
                selectedClassName = value;
                CheckIsReady();
            }
        }
        public string SelectedNameSpace { get; set; }
        public string SelectedProppertyName { get; set; }

        public bool IsSelectedClassNameReadOnly { get; set; } = false;
        public string SelectProjectCaption { get; set; } = "Select Root Class Project:";
        public string SelectClassCaption { get; set; } = "Select Root Class:";
        public string SelectedClassNameCaption { get; set; } = "Selected Class Name:";
        public string SelectedNameSpaceCaption { get; set; } = "Selected NameSpace:";
        public string SelectedProppertyNameCaption { get; set; } = "Selected Property Name:";
        public Visibility SelectedProppertyNameVisibility { get; set; } = Visibility.Collapsed;
        public UserControl ActionUserControl { get; set; } = null;
        #endregion

        #region Helper methods
        public void CollectProjects()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (Dte == null) return;
            foreach (Project p in Dte.Solution.Projects)
            {
                if (string.Compare(p.Kind, ProjectKinds.vsProjectKindSolutionFolder) == 0)
                {
                    CollectNestedProjects(p);
                }
                else
                {
                    SolutionProject sp = new SolutionProject()
                    {
                        ProjectName = p.Name,
                        ProjectUniqueName = p.UniqueName,
                        ProjectRef = p
                    };
                    ComboItemsSourceProjects.Add(sp);
                }
            }
        }
        public virtual void CollectNestedProjects(Project parentPrj)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (string.Compare(parentPrj.Kind, ProjectKinds.vsProjectKindSolutionFolder) != 0)
            {
                return;
            }
            for (var i = 1; i <= parentPrj.ProjectItems.Count; i++)
            {
                Project subProject = parentPrj.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    CollectNestedProjects(subProject);
                }
                else
                {
                    SolutionProject sp = new SolutionProject()
                    {
                        ProjectName = subProject.Name,
                        ProjectUniqueName = subProject.UniqueName,
                        ProjectRef = subProject
                    };
                    ComboItemsSourceProjects.Add(sp);
                }
            }
        }
        public virtual void CollectProjectClasses(SolutionProject sp)
        {
            SelectedCodeElement = null;
            if (ComboItemsSourceCodeElements == null)
            {
                ComboItemsSourceCodeElements = new ObservableCollection<SolutionCodeElement>();
            }
            else
            {
                ComboItemsSourceCodeElements.Clear();
            }
        }
        public virtual void CheckIsReady()
        {
            IsReady.DoNotify(this, false);
        }
        #endregion

        public BaseSelectSourceViewModel(DTE2 dte) : base()
        {
            ComboItemsSourceProjects = new List<SolutionProject>();
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Dte = dte;
            CollectProjects();
        }


    }
}
