using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.Serializable;
using CS2WPF.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using EnvDTE80;
using CS2WPF.Helpers;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class FeatureViewModel : IsReadyViewModel
    {

        DTE2 Dte;
        public FeatureViewModel(DTE2 dte) : base()
        {
            this.Dte = dte;
        }

        public ModifyFeatureViewModel ModifyFeatureVM;

        public ObservableCollection<FeatureSerializable> Features { get; set; }
        public ObservableCollection<FeatureItemSerializable> FeatureItemsList { get; set; } = new ObservableCollection<FeatureItemSerializable>();
        FeatureSerializable _SelectedFeature;
        public FeatureSerializable SelectedFeature
        {
            get
            {
                return _SelectedFeature;
            }
            set
            {
                if(_SelectedFeature != value)
                {
                    _SelectedFeature = value;
                    OnPropertyChanged();
                    OnSelectedFeaturePropertyChanged();
                }
            }
        }
        AllowedFileTypesSerializable _AllowedFileTypes;
        public AllowedFileTypesSerializable AllowedFileTypes
        {
            get
            {
                return _AllowedFileTypes;
            }
            set
            {
                if (_AllowedFileTypes != value)
                {
                    _AllowedFileTypes = value;
                    OnPropertyChanged();
                }
            }
        }
        DbContextSerializable _DbContext;
        public DbContextSerializable DbContext
        {
            get
            {
                return _DbContext;
            }
            set
            {
                if(_DbContext != value)
                {
                    _DbContext = value;
                    OnPropertyChanged();
                }
            }
        }
        FeatureContextSerializable _FeatureContext;
        public FeatureContextSerializable FeatureContext
        {
            get
            {
                return _FeatureContext;
            }
            set
            {
                if (_FeatureContext != value)
                {
                    _FeatureContext = value;
                    OnPropertyChanged();
                    OnFeatureContext();
                }
            }
        }
        public void OnSelectedFeaturePropertyChanged()
        {
            if (FeatureItemsList == null) FeatureItemsList = new ObservableCollection<FeatureItemSerializable>();
            FeatureItemsList.Clear();
            if (SelectedFeature == null) return;
            if (SelectedFeature.FeatureItems == null) return;
            foreach(FeatureItemSerializable fi in SelectedFeature.FeatureItems)
            {
                FeatureItemsList.Add(fi);
            }
            CheckIsReady();
        }
        public void OnFeatureContext()
        {
            if (Features == null) Features = new ObservableCollection<FeatureSerializable>();
            SelectedFeature = null;
            Features.Clear();
            if (FeatureContext == null) return;
            if (FeatureContext.Features == null) return;
            foreach(FeatureSerializable fi in FeatureContext.Features)
            {
                Features.Add(fi);
            }
        }

        #region UiBtnCommandAdd
        private ICommand _UiBtnCommandAdd;
        public ICommand UiBtnCommandAdd
        {
            get
            {
                return _UiBtnCommandAdd ?? (_UiBtnCommandAdd = new CommandHandler((param) => UiBtnCommandAddAction(param), (param) => UiBtnCommandAddCanExecute(param)));
            }
        }
        public bool UiBtnCommandAddCanExecute(Object param)
        {
            return (DbContext != null) && (AllowedFileTypes != null);

        }
        public virtual void UiBtnCommandAddAction(Object param)
        {
            if (this.ModifyFeatureVM == null) this.ModifyFeatureVM = new ModifyFeatureViewModel();
            this.ModifyFeatureVM.DbContext = this.DbContext;
            this.ModifyFeatureVM.AllowedFileTypes = this.AllowedFileTypes;
            this.ModifyFeatureVM.Feature = new FeatureSerializable()
            {
                FeatureName = "NoNameFeature"
            };
            WindowModifyFeature dlg = new WindowModifyFeature(this.ModifyFeatureVM);
            Nullable<bool> dialogResult = dlg.ShowDialog();
            if(dialogResult.HasValue)
            {
                if (dialogResult.Value) {
                    if (FeatureContext == null) FeatureContext = new FeatureContextSerializable();
                    if (FeatureContext.Features == null) FeatureContext.Features = new List<FeatureSerializable>();
                    FeatureContext.Features.Add(this.ModifyFeatureVM.Feature);
                    Features.Add(this.ModifyFeatureVM.Feature);
                    SelectedFeature = this.ModifyFeatureVM.Feature;
                }
            }
        }
        #endregion

        #region UiBtnCommandUpdate
        private ICommand _UiBtnCommandUpdate;
        public ICommand UiBtnCommandUpdate
        {
            get
            {
                return _UiBtnCommandUpdate ?? (_UiBtnCommandUpdate = new CommandHandler((param) => UiBtnCommandUpdateAction(param), (param) => UiBtnCommandUpdateCanExecute(param)));
            }
        }
        public bool UiBtnCommandUpdateCanExecute(Object param)
        {
            return (DbContext != null) && (AllowedFileTypes != null) && (SelectedFeature != null); 

        }
        public virtual void UiBtnCommandUpdateAction(Object param)
        {
            if (this.ModifyFeatureVM == null) this.ModifyFeatureVM = new ModifyFeatureViewModel();
            this.ModifyFeatureVM.DbContext = this.DbContext;
            this.ModifyFeatureVM.AllowedFileTypes = this.AllowedFileTypes;
            this.ModifyFeatureVM.Feature = SelectedFeature;
            WindowModifyFeature dlg = new WindowModifyFeature(this.ModifyFeatureVM);
            Nullable<bool> dialogResult = dlg.ShowDialog();
            if (dialogResult.HasValue)
            {
                if (dialogResult.Value)
                {
                    _SelectedFeature = null;
                    SelectedFeature = this.ModifyFeatureVM.Feature;
                }
            }
        }
        #endregion

        #region UiBtnCommandDelete
        private ICommand _UiBtnCommandDelete;
        public ICommand UiBtnCommandDelete
        {
            get
            {
                return _UiBtnCommandDelete ?? (_UiBtnCommandDelete = new CommandHandler((param) => UiBtnCommandDeleteAction(param), (param) => UiBtnCommandDeleteCanExecute(param)));
            }
        }
        public bool UiBtnCommandDeleteCanExecute(Object param)
        {
            return (DbContext != null) && (AllowedFileTypes != null) && (SelectedFeature != null);

        }
        public virtual void UiBtnCommandDeleteAction(Object param)
        {
            if(MessageBox.Show("Do you want to delete selected Feature?", "Delete Item", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (SelectedFeature == null) return;
                if (FeatureContext != null)
                {
                    if(FeatureContext.Features != null)
                    {
                        int ind = FeatureContext.Features.FindIndex(i => i == SelectedFeature);
                        if(ind > -1)
                        {
                            FeatureContext.Features.RemoveAt(ind);
                        }
                    }
                }
                if(Features != null)
                {
                    int ind = Features.IndexOf(SelectedFeature);
                    if(ind > -1)
                    {
                        Features.RemoveAt(ind);
                    }
                }
                SelectedFeature = null;
            }
        }
        #endregion

        public string JsonExtension { get; set; } = "json";
        public string FeatureContextSufix { get; set; } = "Feature";

        SolutionCodeElement _SelectedDbContext;
        public SolutionCodeElement SelectedDbContext
        {
            get
            {
                return _SelectedDbContext;
            }
            set
            {
                if(_SelectedDbContext != value)
                {
                    _SelectedDbContext = value;
                    OnSelectedDbContextChanged();
                }
            }
        }
        public void OnSelectedDbContextChanged()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if(SelectedDbContext == null)
            {
                DbContext = null;
                OnSelectedFeatureContext();
            }
            string projectName = "";
            if (SelectedDbContext.CodeElementRef != null)
            {
                if (SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, SelectedDbContext.CodeElementFullName, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                string solutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(solutionDirectory, locFileName);
                if (File.Exists(locFileName))
                {
                    string jsonString = File.ReadAllText(locFileName);
                    DbContext = JsonConvert.DeserializeObject<DbContextSerializable>(jsonString);
                    OnSelectedFeatureContext();
                }
                else
                {
                    DbContext = new DbContextSerializable();
                    OnSelectedFeatureContext();
                }
            } else
            {
                DbContext = null;
                OnSelectedFeatureContext();
            }
        }
        public void OnSelectedFeatureContext()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if(DbContext == null)
            {
                FeatureContext = null;
            }
            string projectName = "";
            if (SelectedDbContext.CodeElementRef != null)
            {
                if (SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, SelectedDbContext.CodeElementFullName, FeatureContextSufix, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                string solutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(solutionDirectory, locFileName);
                if (File.Exists(locFileName))
                {
                    string jsonString = File.ReadAllText(locFileName);
                    FeatureContext = JsonConvert.DeserializeObject<FeatureContextSerializable>(jsonString);
                }
                else
                {
                    FeatureContext = new FeatureContextSerializable();
                }
            }

        }
        public void CheckIsReady()
        {
            bool rslt = (this.SelectedFeature != null) && (DbContext != null) && (AllowedFileTypes != null) && (FeatureItemsList != null);
            if (rslt)
            {
                rslt = (AllowedFileTypes.Items != null);
            }
            if (rslt)
            {
                rslt = (AllowedFileTypes.Items.Count > 0);
            }
            IsReady.DoNotify(this, rslt);
        }

        public string AllowedFileTypesFileName { get; set; } = "00000-AllowedFileTypes.json";

        string _AllowedFileTypesFolder = "";
        public string AllowedFileTypesFolder
        {
            get
            {
                return _AllowedFileTypesFolder;
            }
            set
            {
                if(_AllowedFileTypesFolder != value)
                {
                    _AllowedFileTypesFolder = value;
                    OnAllowedFileTypesFolder();
                }
            }
        }
        public void OnAllowedFileTypesFolder()
        {
            if (AllowedFileTypes != null) AllowedFileTypes = null;
            if (string.IsNullOrEmpty(AllowedFileTypesFolder)) AllowedFileTypesFolder = "";
            
            string locFileName = Path.Combine(AllowedFileTypesFolder, AllowedFileTypesFileName);
            if (File.Exists(locFileName))
            {
                string jsonString = File.ReadAllText(locFileName);
                AllowedFileTypes = JsonConvert.DeserializeObject<AllowedFileTypesSerializable>(jsonString);
            }
            CheckIsReady();
        }

    }
}
