using CS2WPF.Helpers.UI;
using CS2WPF.Model.Serializable;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;
using CS2WPF.View;
using CS2WPF.Model;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.Windows.Forms;
using Newtonsoft.Json;
//using Microsoft.Build.Tasks;
using CS2WPF.Helpers;
using static CS2WPF.Helpers.ContextChangedNotificationService;

namespace CS2WPF.ViewModel
{
    public class SelectFeatureFolderViewModel : IsReadyViewModel
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
        FeatureSerializable _SelectedFeature;
        List<CommonStaffSerializable> _CreatedActions;
        string _WebAddress = null;
        ObservableCollection<string> _T4Folders;

        #endregion
        public ContextChangedService OnContextChanged;
        public SelectFeatureFolderViewModel(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory, SolutionCodeElement selectedDbContext, string rootFolder, string JavaScriptsTmplst, string BatchJavaScriptsTmplst) : base()
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
        public DbContextSerializable SerializableDbContext { get; set; }
        public FeatureContextSerializable SerializableFeatureContext { get; set; }
        public FeatureSerializable SelectedFeature
        {
            get
            {
                return _SelectedFeature;
            }
            set
            {
                if (_SelectedFeature == value) return;
                _SelectedFeature = value;
                OnPropertyChanged();
                if (_SelectedFeature == null)
                {
                    CreatedActions = null;
                }
                else
                {
                    CreatedActions = _SelectedFeature.CommonStaffs;
                }
                UpdateFoldersList();
            }
        }
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
            if (SelectedFeature != null)
            {
                isContextItem = SelectedFeature.FeatureName == ContextItemFeatureName;
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
        public string ContextItemFeatureName { get; set; } = "==Context==";
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
            if (string.IsNullOrEmpty(_T4SelectedFolder) || (SelectedFeature == null))
            {
                FileName = "";
                WebAddress = null;
                CheckIsReady();
                return;
            }
            bool isFileNameChanged = false;
            if (SelectedFeature.CommonStaffs != null)
            {
                CommonStaffSerializable commonStaffSerializable =
                    SelectedFeature.CommonStaffs.Where(c => c.FileType == _T4SelectedFolder).FirstOrDefault();
                if (commonStaffSerializable != null)
                {
                    FileName = commonStaffSerializable.FileName;
                    isFileNameChanged = true;
                }
            }
            string locFileName = null;
            if (!isFileNameChanged)
            {
                if (SelectedFeature.FeatureName == ContextItemFeatureName)
                {
                    locFileName =
                         TrimPrefix(Path.GetFileNameWithoutExtension(T4SelectedFolder));
                }
                else
                {
                    locFileName =
                        SelectedFeature.FeatureName + TrimPrefix(Path.GetFileNameWithoutExtension(T4SelectedFolder));
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
            if (SelectedFeature != null)
            {
                CreatedActions = SelectedFeature.CommonStaffs;
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
            return false;
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
            /*
                        dlgContext.SerializableModel = this.SelectedModel;
                        dlgContext.ContextItemViewName = this.ContextItemViewName;
            */
            dlgContext.SelectedDbContext = this.SelectedDbContext;
            dlgContext.DestinationProjectName = this.DestinationProjectName;
            /*
                        dlgContext.UIFormProperties = this.UIFormProperties;
                        dlgContext.UIListProperties = this.UIListProperties;
            */
            dlgContext.DestinationProject = this.DestinationProject;
            dlgContext.DefaultProjectNameSpace = this.DefaultProjectNameSpace;

            dlgContext.PrepareSettings();
            WindowBatch dlg = new WindowBatch(dlgContext);
            dlg.ShowDialog();
            OnCreatedActionsChanged();
        }

        #endregion

        public FeatureSerializable GetSelectedFeatureCommonShallowCopy(string FileType, string FileName, string T4Template)
        {
            FeatureSerializable result = null;
            if (SelectedFeature == null) return result;
            result = SelectedFeature.FeatureContextSerializableGetShallowCopy();

            CommonStaffSerializable commonStaffItem =
                result.CommonStaffs.Where(c => c.FileType == FileType).FirstOrDefault();
            if (commonStaffItem == null)
            {
                result.CommonStaffs.Add(
                    commonStaffItem = new CommonStaffSerializable()
                    {
                        FileType = FileType
                    });
            }
            commonStaffItem.FileName = FileName;
            commonStaffItem.FileProject = this.DestinationProjectName;
            commonStaffItem.FileDefaultProjectNameSpace = this.DefaultProjectNameSpace;
            commonStaffItem.FileFolder = this.DestinationFolder;
            commonStaffItem.T4Template = T4Template;
            return result;
        }

    }
}
