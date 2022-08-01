using CS2WPF.Helpers.UI;
using CS2WPF.Model.Serializable;
using CS2WPF.View;
using CS2WPF.Model;

using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.Windows.Forms;
using Newtonsoft.Json;
using static CS2WPF.Helpers.ContextChangedNotificationService;
using CS2WPF.Helpers;

namespace CS2WPF.ViewModel
{
    public class SelectFolderViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected ITextTemplating TextTemplating;
        protected SolutionCodeElement SelectedDbContext;
        protected IVsThreadedWaitDialogFactory DialogFactory;
        string _T4SelectedFolder = "";
        string _T4RootFolder = "";
        string _FileName = "";
        string BatchRootFolder = "";
        ModelViewSerializable _SelectedModel;
        List<CommonStaffSerializable> _CreatedActions;
        string _WebAddress = null;
        ObservableCollection<string> _T4Folders;

        #endregion
        public ContextChangedService OnContextChanged;
        public SelectFolderViewModel(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory, SolutionCodeElement selectedDbContext, string rootFolder, string JavaScriptsTmplst, string BatchJavaScriptsTmplst) : base()
        {
            this.T4Folders = new ObservableCollection<string>();
            this.Dte = dte;
            this.SelectedDbContext = selectedDbContext;
            this.DialogFactory = dialogFactory;
            this.TextTemplating = textTemplating;
            this.T4RootFolder = Path.Combine(rootFolder, JavaScriptsTmplst);
            this.BatchRootFolder = Path.Combine(rootFolder, BatchJavaScriptsTmplst);
            this.OnContextChanged = new ContextChangedService();
        }
        public string DestinationProjectName { get; set; }
        public Project DestinationProject { get; set; }
        public string DestinationProjectRootFolder { get; set; }
        public string DestinationFolder { get; set; }
        public string DefaultProjectNameSpace { get; set; }
        public ObservableCollection<ModelViewUIFormProperty> UIFormProperties { get; set; }
        public ObservableCollection<ModelViewUIListProperty> UIListProperties { get; set; }

        public ModelViewSerializable SelectedModel
        {
            get { return _SelectedModel; }
            set
            {
                if (_SelectedModel == value) return;
                _SelectedModel = value;
                OnPropertyChanged();
                if (_SelectedModel == null)
                {
                    CreatedActions = null;
                }
                else
                {
                    CreatedActions = _SelectedModel.CommonStaffs;
                }
                //OnSelectedModelChanged();
                UpdateFoldersList();
            }
        }
        public DbContextSerializable SerializableDbContext { get; set; }
        public string T4SelectedFolder
        {
            get
            {
                return _T4SelectedFolder;
            }
            set
            {
                if (_T4SelectedFolder == value) return;
                _T4SelectedFolder = value;
                OnPropertyChanged();
                OnSelectedModelChanged();
            }
        }
        public string T4RootFolder
        {
            get
            {
                return _T4RootFolder;
            }
            set
            {
                if (_T4RootFolder == value) return;
                _T4RootFolder = value;
                OnPropertyChanged();
                UpdateFoldersList();
            }
        }
        public void UpdateFoldersList()
        {
            if (T4Folders == null) T4Folders = new ObservableCollection<string>();
            T4SelectedFolder = "";
            T4Folders.Clear();
            bool isContextItem = false;
            string searchPattern = "00";
            if (SelectedModel != null)
            {
                isContextItem = SelectedModel.ViewName == ContextItemViewName;
            }
            string[] folders = Directory.GetDirectories(_T4RootFolder);
            if (folders != null)
            {
                foreach (string f in folders)
                {
                    string flnm = Path.GetFileName(f);
                    if (isContextItem)
                    {
                        if (flnm.StartsWith(searchPattern))
                        {
                            T4Folders.Add(flnm);
                        }
                    }
                    else
                    {
                        if (!flnm.StartsWith(searchPattern))
                        {
                            T4Folders.Add(flnm);
                        }
                    }
                }
            }
        }
        public string ContextItemViewName { get; set; } = "==Context==";
        public ObservableCollection<string> T4Folders
        {
            get
            {
                return _T4Folders;
            }
            set
            {
                if (_T4Folders == value) return;
                _T4Folders = value;
                OnPropertyChanged();
            }
        }
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (_FileName == value) return;
                _FileName = value;
                OnPropertyChanged();
                CheckIsReady();
            }
        }
        public void OnSelectedModelChanged()
        {
            //UpdateFoldersList();
            if (string.IsNullOrEmpty(_T4SelectedFolder) || (SelectedModel == null))
            {
                FileName = "";
                WebAddress = null;
                CheckIsReady();
                return;
            }
            bool isFileNameChanged = false;
            if (SelectedModel.CommonStaffs != null)
            {
                CommonStaffSerializable commonStaffSerializable =
                    SelectedModel.CommonStaffs.Where(c => c.FileType == _T4SelectedFolder).FirstOrDefault();
                if (commonStaffSerializable != null)
                {
                    FileName = commonStaffSerializable.FileName;
                    isFileNameChanged = true;
                }
            }
            string locFileName = null;
            if (!isFileNameChanged)
            {
                if (SelectedModel.ViewName == ContextItemViewName)
                {
                    locFileName =
                         TrimPrefix(Path.GetFileNameWithoutExtension(T4SelectedFolder));
                }
                else
                {
                    locFileName =
                        SelectedModel.ViewName + TrimPrefix(Path.GetFileNameWithoutExtension(T4SelectedFolder));
                }
                FileName = GetHyphenedName(locFileName);
            }
            locFileName = null;
            string[] files = Directory.GetFiles(Path.Combine(_T4RootFolder, T4SelectedFolder), "*.html");
            if (files != null)
            {
                if (files.Length > 0)
                {
                    locFileName = "file:///" + Path.GetFullPath(files[0]).Replace("\\", "/");
                }
            }
            WebAddress = locFileName;

            CheckIsReady();
        }
        public void CheckIsReady()
        {
            IsReady.DoNotify(this, (!string.IsNullOrEmpty(this.T4SelectedFolder)) && (!string.IsNullOrEmpty(FileName)));
        }
        public List<CommonStaffSerializable> CreatedActions
        {
            get
            {
                return _CreatedActions;
            }
            set
            {
                if (_CreatedActions == value) return;
                _CreatedActions = value;
                OnPropertyChanged();
            }
        }
        public string TrimPrefix(string srcStr)
        {
            if (string.IsNullOrEmpty(srcStr)) return "";
            int i = srcStr.IndexOf('-');
            if (i > -1)
            {
                return srcStr.Substring(i + 1);
            }
            else
            {
                return srcStr;
            }

        }
        public void OnCreatedActionsChanged()
        {
            CreatedActions = null;
            if (SelectedModel != null)
            {
                CreatedActions = SelectedModel.CommonStaffs;
            }
        }
        public string WebAddress
        {
            get
            {
                return _WebAddress;
            }
            set
            {
                if (_WebAddress == value) return;
                _WebAddress = value;
                OnPropertyChanged();
            }
        }

        public string GetHyphenedName(string src)
        {
            string result = "";
            if (string.IsNullOrEmpty(src))
            {
                return result;
            }
            int firstDelim = src.IndexOf('.');
            if (firstDelim > -1)
            {
                result =
                    Regex.Replace(src.Substring(0, firstDelim), @"\B[A-Z]", m => "-" + m.ToString().ToLower()).ToLower() +
                    src.Substring(firstDelim);
            }
            else
            {
                result =
                    Regex.Replace(src, @"\B[A-Z]", m => "-" + m.ToString().ToLower()).ToLower();

            }
            return result;
        }

        #region UiBtnFormCommandBatch
        private ICommand _UiBtnFormCommandBatch;
        public ICommand UiBtnFormCommandBatch
        {
            get
            {
                return _UiBtnFormCommandBatch ?? (_UiBtnFormCommandBatch = new CommandHandler((param) => UiBtnFormCommandBatchAction(param), (param) => UiBtnFormCommandBatchCanExecute(param)));
            }
        }
        public bool UiBtnFormCommandBatchCanExecute(Object param)
        {
            return true;
        }

        BatchProcessingViewModel dlgContext = null;
        public virtual void UiBtnFormCommandBatchAction(Object param)
        {
            if (dlgContext == null)
            {
                dlgContext = new BatchProcessingViewModel(new PrismModuleModifier(this.Dte), this.Dte, this.TextTemplating, this.DialogFactory, this.T4RootFolder, this.BatchRootFolder);
            }
            dlgContext.DestinationProjectRootFolder = this.DestinationProjectRootFolder;
            dlgContext.DestinationFolder = this.DestinationFolder;
            dlgContext.SerializableDbContext = this.SerializableDbContext;
            dlgContext.SerializableModel = this.SelectedModel;
            dlgContext.ContextItemViewName = this.ContextItemViewName;
            dlgContext.SelectedDbContext = this.SelectedDbContext;
            dlgContext.DestinationProjectName = this.DestinationProjectName;
            dlgContext.UIFormProperties = this.UIFormProperties;
            dlgContext.UIListProperties = this.UIListProperties;
            dlgContext.DestinationProject = this.DestinationProject;
            dlgContext.DefaultProjectNameSpace = this.DefaultProjectNameSpace;

            dlgContext.PrepareSettings();
            WindowBatch dlg = new WindowBatch(dlgContext);
            dlg.ShowDialog();
            OnCreatedActionsChanged();
        }

        #endregion

        #region UiBtnFormCommandImport
        private ICommand _UiBtnFormCommandImport;
        public ICommand UiBtnFormCommandImport
        {
            get
            {
                return _UiBtnFormCommandImport ?? (_UiBtnFormCommandImport = new CommandHandler((param) => UiBtnFormCommandImportAction(param), (param) => UiBtnFormCommandImportCanExecute(param)));
            }
        }
        public bool UiBtnFormCommandImportCanExecute(Object param)
        {
            return true;
        }
        public virtual void UiBtnFormCommandImportAction(Object param)
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
                if (srcContext.CommonStaffs != null)
                {
                    if (SerializableDbContext.CommonStaffs == null)
                    {
                        SerializableDbContext.CommonStaffs = new List<CommonStaffSerializable>();
                    }
                    foreach (CommonStaffSerializable commonStaffSerializable in srcContext.CommonStaffs)
                    {
                        if (!SerializableDbContext.CommonStaffs.Any(s => s.FileType == commonStaffSerializable.FileType))
                        {
                            SerializableDbContext.CommonStaffs.Add(commonStaffSerializable);
                        }
                    }
                    if (SelectedModel.ViewName == ContextItemViewName)
                    {
                        if (SelectedModel.CommonStaffs == null)
                        {
                            SelectedModel.CommonStaffs = new List<CommonStaffSerializable>();
                        }
                        foreach (CommonStaffSerializable commonStaffSerializable in srcContext.CommonStaffs)
                        {
                            if (!SelectedModel.CommonStaffs.Any(s => s.FileType == commonStaffSerializable.FileType))
                            {
                                SelectedModel.CommonStaffs.Add(commonStaffSerializable);
                            }
                        }

                    }
                }
                OnCreatedActionsChanged();
                System.Windows.Forms.MessageBox.Show("Data was imported successfully", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (SelectedModel.ViewName != ContextItemViewName)
                {
                    OnContextChanged.DoNotify(this);
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
