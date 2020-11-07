using CS2WPF.Helpers.BatchProcess;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.BatchProcess;
using CS2WPF.Model.Serializable;
using CS2WPF.Model.Serializable.UI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
    public class BatchProcessingViewModel : NotifyPropertyChangedViewModel
    {
        protected DTE2 Dte;
        protected ITextTemplating TextTemplating;
        protected IVsThreadedWaitDialogFactory DialogFactory;
        protected DbContextSerializable _SerializableDbContext = null;
        protected ModelViewSerializable _SerializableModel = null;
        protected string _BatchSettingRootFolder;
        protected ObservableCollection<string> _BatchSettingFiles;
        protected string _SelectedFileName = null;
        protected string _CurrentBatchSetting = null;
        protected string T4RootFolder;
        protected string BatchRootFolder;


        public string ContextItemViewName { get; set; } = "==Context==";
        public string ContextLevelTmplts { get; set; } = "00000-ContextLevelTmplts";
        public string ViewModelLevelTmplts { get; set; } = "01000-ViewModelLevelTmplts";
        protected string _LastError;
        public string LastError { 
            get
            {
                return _LastError;
            } 
            set
            {
                _LastError = value;
                OnPropertyChanged();
            } 
        }
        protected string _ReportText;
        public string ReportText
        {
            get
            {
                return _ReportText;
            }
            set
            {
                _ReportText = value;
                OnPropertyChanged();
            }

        }

        public BatchProcessingViewModel(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory, string t4RootFolder, string batchRootFolder)
        {
            this.Dte = dte;
            this.TextTemplating = textTemplating;
            this.T4RootFolder = t4RootFolder;
            this.BatchRootFolder = batchRootFolder;
            this.DialogFactory = dialogFactory;
        }
        public string DestinationProjectRootFolder { get; set; }
        public string DestinationProjectName { get; set; }
        public Project DestinationProject { get; set; }
        public string DestinationFolder { get; set; }
        public string DefaultProjectNameSpace { get; set; }
        public ObservableCollection<ModelViewUIFormProperty> UIFormProperties { get; set; }
        public ObservableCollection<ModelViewUIListProperty> UIListProperties { get; set; }

        public SolutionCodeElement SelectedDbContext { get; set; }
        public DbContextSerializable SerializableDbContext {
            get {
                return this._SerializableDbContext;
            }
            set{
                this._SerializableDbContext = value;
            }
        }
        public ModelViewSerializable SerializableModel
        {
            get
            {
                return this._SerializableModel;
            }
            set
            {
                this._SerializableModel = value;
            }
        }
        public string BatchSettingRootFolder
        {
            get
            {
                return _BatchSettingRootFolder;
            }
            set
            {
                if (_BatchSettingRootFolder == value) return;
                _BatchSettingRootFolder = value;
            }
        }
        public ObservableCollection<string> BatchSettingFiles
        {
            get
            {
                return _BatchSettingFiles;
            }
            set
            {
                if (_BatchSettingFiles == value) return;
                _BatchSettingFiles = value;
                OnPropertyChanged();
            }
        }
        public string CurrentBatchSetting 
        {
            get {
                return _CurrentBatchSetting;
            }
            set {
                _CurrentBatchSetting = value;
                OnPropertyChanged();
            }
        }
        public string SelectedFile
        {
            get
            {
                return _SelectedFileName;
            }
            set
            {
                if (_SelectedFileName == value) {
                    return;
                }
                _SelectedFileName = value;
                OnPropertyChanged();
                OnSelectedFileNameChanged();
            }
        }
        public void PrepareSettings()
        {
            SelectedFile = null;
            if (BatchSettingFiles == null) BatchSettingFiles = new ObservableCollection<string>();
            BatchSettingFiles.Clear();
            if (SerializableModel != null)
            {
                string folderNm = "";
                if ( SerializableModel.ViewName == ContextItemViewName )
                {
                    folderNm = Path.Combine(BatchRootFolder, ContextLevelTmplts);
                } else
                {
                    folderNm = Path.Combine(BatchRootFolder, ViewModelLevelTmplts);
                }
                string[] files = Directory.GetFiles(folderNm, "*.json");
                if (files != null)
                {
                    foreach (string f in files)
                    {
                        string flnm = Path.GetFileName(f);
                        BatchSettingFiles.Add(flnm);
                    }
                }
            }
        }
        public void OnSelectedFileNameChanged() {
            CurrentBatchSetting = "";
            if(string.IsNullOrEmpty(SelectedFile) || (SerializableDbContext == null) || (SerializableModel == null))
            {
                return;
            }
            string fileNm = "";
            if (SerializableModel.ViewName == ContextItemViewName)
            {
                fileNm = Path.Combine(BatchRootFolder, ContextLevelTmplts, SelectedFile);
            }
            else
            {
                fileNm = Path.Combine(BatchRootFolder, ViewModelLevelTmplts, SelectedFile);
            }
            CurrentBatchSetting = File.ReadAllText(fileNm);
        }

        #region UiBtnFormCommandBatch
        private ICommand _StartBtnCommand;
        public ICommand StartBtnCommand
        {
            get
            {
                return _StartBtnCommand ?? (_StartBtnCommand = new CommandHandler((param) => StartBtnCommandBatchAction(param), (param) => StartBtnCommandCanExecute(param)));
            }
        }
        public bool StartBtnCommandCanExecute(Object param)
        {
            return !(string.IsNullOrEmpty(CurrentBatchSetting) || (SerializableDbContext == null) || (SerializableModel == null));
        }
        public virtual void StartBtnCommandBatchAction(Object param)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            IVsThreadedWaitDialog2 aDialog = null;
            bool aDialogStarted = false;
            if (this.DialogFactory != null)
            {
                this.DialogFactory.CreateInstance(out aDialog);
                if (aDialog != null)
                {
                    aDialogStarted = aDialog.StartWaitDialog("Generation started", "VS is Busy", "Please wait", null, "Generation started", 0, false, true) == VSConstants.S_OK;
                }
            }
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine("Json parsing started");
                BatchSettings batchSettings = BatchSettingsHelper.ReadBatchSettingsFromString(CurrentBatchSetting);
                if(batchSettings == null)
                {
                    throw new Exception("Could not Deserialize Object");
                }
                sb.AppendLine("Json parsing finished");
                if (batchSettings.BatchItems == null)
                {
                    throw new Exception("Batch Items is empty");
                }
                sb.AppendLine("Batch Items processing started");
                
                foreach (BatchItem batchItem  in batchSettings.BatchItems)
                {
                    ModelViewSerializable currentSerializableModel = SerializableModel;
                    if (!string.IsNullOrEmpty( batchItem.ViewModel )) {
                        currentSerializableModel = SerializableDbContext.ModelViews.Where(m => m.ViewName == batchItem.ViewModel).FirstOrDefault();
                        if(currentSerializableModel == null)
                        {
                            throw new Exception("Could not find [" + batchItem.ViewModel + "] of the Batch Item = " + batchItem.GeneratorType);
                        }
                    }
                    sb.AppendLine("Processing Batch Item: [DestinationFolder]=["+batchItem.DestinationFolder+ "]");
                    sb.AppendLine("    [GeneratorType]=[" + batchItem.GeneratorType + "]");
                    sb.AppendLine("        [GeneratorSript]=[" + batchItem.GeneratorSript + "]");
                    string tmpltPath =  Path.Combine(T4RootFolder, batchItem.GeneratorType, batchItem.GeneratorSript);
                    string FileName = "";
                    if (currentSerializableModel.ViewName == ContextItemViewName)
                    {
                        FileName =
                             BatchSettingsHelper.TrimPrefix(Path.GetFileNameWithoutExtension(batchItem.GeneratorType));
                    }
                    else
                    {
                        FileName =
                            currentSerializableModel.ViewName + BatchSettingsHelper.TrimPrefix(Path.GetFileNameWithoutExtension(batchItem.GeneratorType));
                    }
                    FileName = BatchSettingsHelper.GetHyphenedName(FileName);
                    sb.AppendLine("    Batch Item: Creating Shallow Copy");
                    ModelViewSerializable ShallowCopy = null;
                    if (currentSerializableModel.ViewName == SerializableModel.ViewName)
                    {
                        ShallowCopy =
                            BatchSettingsHelper.GetSelectedModelCommonShallowCopy(currentSerializableModel,
                                                                    UIFormProperties, UIListProperties,
                                                                    DestinationProjectName, DefaultProjectNameSpace, DestinationFolder, batchItem.DestinationFolder,
                                                                    batchItem.GeneratorType, FileName);
                    } else
                    {
                        ShallowCopy =
                            BatchSettingsHelper.GetSelectedModelCommonShallowCopy(currentSerializableModel,
                                                                    null, null,
                                                                    DestinationProjectName, DefaultProjectNameSpace, DestinationFolder, batchItem.DestinationFolder,
                                                                    batchItem.GeneratorType, FileName);
                    }
                    sb.AppendLine("        Batch Item: Generating Code");
                    GeneratorBatchStep generatorBatchStep = BatchSettingsHelper.DoGenerateViewModel(Dte, TextTemplating, tmpltPath, SerializableDbContext, ShallowCopy, DefaultProjectNameSpace);
                    if (!string.IsNullOrEmpty(generatorBatchStep.GenerateError))
                    {
                        throw new Exception(generatorBatchStep.GenerateError);
                    }
                    sb.AppendLine("            Batch Item: Adding Generated file to project and Updating Wizard's Context Repository");
                    BatchSettingsHelper.UpdateDbContext(Dte, DestinationProject, SelectedDbContext, SerializableDbContext, ShallowCopy,
                                        ContextItemViewName, batchItem.GeneratorType,
                                        DestinationProjectRootFolder,
                                        DestinationFolder,
                                        batchItem.DestinationFolder,
                                        FileName, generatorBatchStep.FileExtension,
                                        generatorBatchStep.GenerateText);
                    currentSerializableModel.CommonStaffs =  ShallowCopy.CommonStaffs;
                    sb.AppendLine("Batch Item Processing finished");
                }
                sb.AppendLine("Batch Items processing finished");
            }
            catch (Exception e)
            {
                LastError = "Exception thrown: " + e.Message;
                return;
            }
            finally
            {
                if (aDialogStarted)
                {
                    int iOut;
                    aDialog.EndWaitDialog(out iOut);
                }
                ReportText = sb.ToString();
            }

        }
        #endregion
    }
}
