using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.Serializable;
using EnvDTE80;
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

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class SelectExistingViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected SolutionCodeElement _SelectedDbContext;
        protected SolutionCodeElement _SelectedEntity;
        protected ObservableCollection<ModelViewSerializable> _ModelViews;
        protected ModelViewSerializable _SelectedModel;
        protected DbContextSerializable _CurrentDbContext;
        protected bool _IsSelectExisting;
        #endregion

        public SelectExistingViewModel(DTE2 dte) : base()
        {
            this.Dte = dte;
            ModelViews = new ObservableCollection<ModelViewSerializable>();
        }
        public DbContextSerializable CurrentDbContext
        {
            get
            {
                return _CurrentDbContext;
            }
            set
            {
                if (_CurrentDbContext == value) return;
                _CurrentDbContext = value;
                OnPropertyChanged();
                OnCurrentDbContextChanged();
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
                OnPropertyChanged();
                OnSelectedEntityChanged();
            }
        }
        public ObservableCollection<ModelViewSerializable> ModelViews
        {
            get { return _ModelViews; }
            set
            {
                if (_ModelViews == value) return;
                _ModelViews = value;
                OnPropertyChanged();
            }
        }
        public ModelViewSerializable SelectedModel
        {
            get { return _SelectedModel; }
            set
            {
                if (_SelectedModel == value) return;
                _SelectedModel = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public bool IsSelectExisting 
        {
            get
            {
                return _IsSelectExisting;
            }
            set
            {
                if (_IsSelectExisting == value) return;
                _IsSelectExisting = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public void OnCurrentDbContextChanged()
        {
            SelectedModel = null;
            ModelViews.Clear();
        }
        public void OnSelectedEntityChanged()
        {
            SelectedModel = null;
            ModelViews.Clear();
        }
        public void DoAnalize()
        {
            if (ModelViews == null) ModelViews = new ObservableCollection<ModelViewSerializable>();
            if ((ModelViews.Count > 0) || (SelectedEntity == null))
            {
                CheckIsReady();
                return;
            }
            if (CurrentDbContext.ModelViews != null)
            {
                foreach (ModelViewSerializable itm in CurrentDbContext.ModelViews)
                {
                    if (itm.RootEntityFullClassName == SelectedEntity.CodeElementFullName)
                    {
                        ModelViews.Add(itm);
                    }
                }
            }
            CheckIsReady();
        }
        public void UpdateModelViews(ModelViewSerializable mvs)
        {
            if (ModelViews == null) ModelViews = new ObservableCollection<ModelViewSerializable>();
            if ((mvs != null) && (SelectedEntity != null) && (CurrentDbContext != null))
            {
                if (mvs.RootEntityFullClassName == SelectedEntity.CodeElementFullName)
                {
                    if(CurrentDbContext.ModelViews != null)
                    {
                        ModelViewSerializable result =
                         CurrentDbContext.ModelViews.FirstOrDefault(m => m.ViewName == mvs.ViewName);
                        if(result != null)
                        {
                            if(!ModelViews.Any(m => m.ViewName == mvs.ViewName))
                            {
                                ModelViews.Add(mvs);
                            }
                        }
                    }
                }
            }
            OnPropertyChanged("SelectedEntity");
        }
        public bool CanShow()
        {
            if (ModelViews != null)
            {
                return ModelViews.Count > 0;
            }
            return false;
        }
        public void CheckIsReady()
        {
            IsReady.DoNotify(this, (SelectedModel != null) || (!IsSelectExisting));
        }

        public string DestinationProject { get; set; }
        public string DefaultProjectNameSpace { get; set; }
        public string DestinationFolder { get; set; }
        public string DbSetProppertyName { get; set; }

        #region ImportBtnCommand
        private ICommand _ImportBtnCommand;
        public ICommand ImportBtnCommand
        {
            get
            {
                return _ImportBtnCommand ?? (_ImportBtnCommand = new CommandHandler((param) => ImportBtnCommandAction(param), (param) => ImportBtnCommandCanExecute(param)));
            }
        }
        public bool ImportBtnCommandCanExecute(Object param)
        {
            return true;
        }
        public virtual void ImportBtnCommandAction(Object param)
        {
            OpenFileDialog ofdlg = new OpenFileDialog();
            ofdlg.Filter = "JSON-files(*.json)|*.json";
            ofdlg.DefaultExt = "json";
            ofdlg.Title = "Select a source to import";
            if (ofdlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            try
            {
                string jsonString = File.ReadAllText(ofdlg.FileName);
                DbContextSerializable srcContext = JsonConvert.DeserializeObject<DbContextSerializable>(jsonString);
                if(CurrentDbContext == null)
                {
                    return;
                }
                if(CurrentDbContext.ModelViews == null)
                {
                    CurrentDbContext.ModelViews = new List<ModelViewSerializable>();
                }
                foreach (ModelViewSerializable itm in srcContext.ModelViews)
                {
                    if (itm.RootEntityClassName != SelectedEntity.CodeElementName)
                    {
                        continue;
                    }
                    if (CurrentDbContext.ModelViews.Any(m => m.ViewName == itm.ViewName)) continue;
                    ModelViewSerializable destItm = itm.ModelViewSerializableGetCopy(this.DestinationProject, this.DefaultProjectNameSpace, this.DestinationFolder, this.DbSetProppertyName, this.SelectedEntity);
                    CurrentDbContext.ModelViews.Add(destItm);
                    ModelViews.Add(destItm);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error:" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        #endregion

        #region ImportAllBtnCommand
        private ICommand _ImportAllBtnCommand;
        public ICommand ImportAllBtnCommand
        {
            get
            {
                return _ImportAllBtnCommand ?? (_ImportAllBtnCommand = new CommandHandler((param) => ImportAllBtnCommandAction(param), (param) => ImportAllBtnCommandCanExecute(param)));
            }
        }
        public bool ImportAllBtnCommandCanExecute(Object param)
        {
            return true;
        }
        public virtual void ImportAllBtnCommandAction(Object param)
        {
            if(System.Windows.Forms.MessageBox.Show("Warning: ==Importing all== clears all existing settings for the selected context. Information about generated code will be lost.  Are you sure you want to continue?" , "Warning", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            OpenFileDialog ofdlg = new OpenFileDialog();
            ofdlg.Filter = "JSON-files(*.json)|*.json";
            ofdlg.DefaultExt = "json";
            ofdlg.Title = "Select a source to import";
            if (ofdlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            try
            {
                string jsonString = File.ReadAllText(ofdlg.FileName);
                DbContextSerializable srcContext = JsonConvert.DeserializeObject<DbContextSerializable>(jsonString);
                if (CurrentDbContext == null)
                {
                    return;
                }
                CurrentDbContext.CommonStaffs = new List<CommonStaffSerializable>();
                CurrentDbContext.ModelViews = new List<ModelViewSerializable>();
                SelectedModel = null;
                ModelViews.Clear();
                foreach (ModelViewSerializable itm in srcContext.ModelViews)
                {
                    ModelViewSerializable destItm = itm.ModelViewSerializableGetCopy(this.DestinationProject, this.DefaultProjectNameSpace, this.DestinationFolder, this.DbSetProppertyName, this.SelectedEntity);
                    CurrentDbContext.ModelViews.Add(destItm);
                }

                string projectName = "";
                if (_SelectedDbContext.CodeElementRef != null)
                {
                    if (_SelectedDbContext.CodeElementRef.ProjectItem != null)
                    {
                        projectName =
                           _SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                    }
                }
                string SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                if (!string.IsNullOrEmpty(projectName))
                {
                    string locFileName = Path.Combine(projectName, _SelectedDbContext.CodeElementFullName, "json");
                    locFileName = locFileName.Replace("\\", ".");
                    locFileName = Path.Combine(SolutionDirectory, locFileName);
                    jsonString = JsonConvert.SerializeObject(CurrentDbContext);
                    File.WriteAllText(locFileName, jsonString);
                    System.Windows.Forms.MessageBox.Show("Information: New setting have been saved onto disk. The file name: " + locFileName + " Please restart the Wizard", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    foreach (ModelViewSerializable itm in CurrentDbContext.ModelViews)
                    {
                        if (itm.RootEntityClassName == SelectedEntity.CodeElementName)
                        {
                            ModelViews.Add(itm);
                        }
                    }

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error: ProjectName is not defined. Please restart the Wizard", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error:" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        #endregion

    }
}
