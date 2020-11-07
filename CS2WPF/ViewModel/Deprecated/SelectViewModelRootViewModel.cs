using CS2ANGULAR.Helpers;
using CS2ANGULAR.Model;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace CS2ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class SelectViewModelRootViewModel : BaseSelectSourceViewModel
    {
        public string DestinationSufix { get; set; } = "VM";



        #region Properties
        public override SolutionCodeElement SelectedCodeElement
        {
            get
            {
                return selectedCodeElement;
            }
            set
            {
                selectedCodeElement = value;
                string ocn = "";
                if (selectedCodeElement != null)
                {
                    ocn = selectedCodeElement.CodeElementName + this.DestinationSufix;
                }
                SelectedClassName = ocn;
                OnPropertyChanged("SelectedClassName");
                OnPropertyChanged("SelectedCodeElement");
            }
        }
        #endregion

        #region Constructors
        public SelectViewModelRootViewModel(DTE2 dte): base(dte)
        {
            IsSelectedClassNameReadOnly = false;
        }
        #endregion

        #region Helper methods
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
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        CodeNamespace cn = (CodeNamespace)ce;
                        InternalCollectProjectClasses(cn);
                    }
                }
            }
        }
        protected bool InternalCollectProjectClasses(CodeNamespace parentCodeNamespace)
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
                        ComboItemsSourceCodeElements.Add(sce);
                    } else
                    {
                        return false;
                    }
                }
                else
                {
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        CodeNamespace cn = (CodeNamespace)ce;
                        if( ! InternalCollectProjectClasses(cn) )
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public override void CheckIsReady()
        {
            IsReady.DoNotify(this, !(string.IsNullOrEmpty(SelectedClassName) || (SelectedCodeElement == null)));
        }
        #endregion
    }
}
