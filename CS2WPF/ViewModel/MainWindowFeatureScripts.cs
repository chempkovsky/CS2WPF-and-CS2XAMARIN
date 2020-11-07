using CS2WPF.Helpers;
using CS2WPF.Model.Serializable;
using CS2WPF.View;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    class MainWindowFeatureScripts : MainWindowBase
    {
        #region Fieds
        UserControlGenerate GenerateUC = null;
        UserControlT4Editor T4EditorUC = null;
        UserControlSelectSource SelectDbContextUC = null;
        UserControlFeature FeatureUC = null;
        UserControlSelectFeatureFolder SelectFeatureFolderUC = null;
        #endregion
        public MainWindowFeatureScripts(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory) : base(dte, textTemplating, dialogFactory)
        {
            InvitationViewModel InvitationVM = new InvitationViewModel();
            InvitationVM.WizardName = "#5 Feature Scripts Wizard";
            InvitationVM.IsReady.IsReadyEvent += InvitationViewModel_IsReady;
            this.InvitationUC = new UserControlInvitation(InvitationVM);
            this.CurrentUserControl = this.InvitationUC;
            InvitationVM.DoAnalise(dte);
        }

        #region PrevBtnCommand
        public override void PrevBtnCommandAction(Object param)
        {
            switch (CurrentUiStepId)
            {
                case 1:
                    CurrentUiStepId = 0;
                    PrevBtnEnabled = false;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = InvitationUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 2:
                    CurrentUiStepId = 1;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (FeatureUC.DataContext as FeatureViewModel).CheckIsReady();
                    this.CurrentUserControl = FeatureUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 4:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectFeatureFolderUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 5:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    (T4EditorUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region NextBtnCommand
        public override void NextBtnCommandAction(Object param)
        {
            switch (CurrentUiStepId)
            {
                case 0:
                    CurrentUiStepId = 1;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    if (SelectDbContextUC == null)
                    {
                        SelectDbContextViewModel dataContext = new SelectDbContextViewModel(Dte);
                        dataContext.UiCommandButtonVisibility = Visibility.Collapsed;
                        dataContext.UiCommandCaption3 = "NameSpace: " + (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        string folder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        if (!string.IsNullOrEmpty(folder))
                        {
                            dataContext.UiCommandCaption3 = dataContext.UiCommandCaption3 + "." + folder.Replace("\\", ".");
                        }
                        SelectDbContextUC = new UserControlSelectSource(dataContext);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                    }
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).DoAnaliseDbContext();
                    this.CurrentUserControl = SelectDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 1:
                    CurrentUiStepId = 2;
                    if (FeatureUC == null)
                    {
                        FeatureViewModel dataContext = new FeatureViewModel(this.Dte);
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        dataContext.AllowedFileTypesFolder = Path.Combine(TemplatesFld, "AllowedFileTypes");
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        FeatureUC = new UserControlFeature(dataContext);
                    }
                    (FeatureUC.DataContext as FeatureViewModel).SelectedDbContext = 
                        (SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedCodeElement;
                    (FeatureUC.DataContext as FeatureViewModel).CheckIsReady();
                    this.CurrentUserControl = FeatureUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 2:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (SelectFeatureFolderUC == null)  
                    {
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        SelectFeatureFolderViewModel dataContext = new SelectFeatureFolderViewModel(Dte, TextTemplating, DialogFactory, (FeatureUC.DataContext as FeatureViewModel).SelectedDbContext, TemplatesFld, "FeatureScriptsTmplst", "BatchScriptsTmplst");
                        dataContext.DestinationProjectRootFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationProjectRootFolder;
                        dataContext.DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        //dataContext.ContextItemFeatureName = 
                        dataContext.DestinationProjectName = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                        dataContext.DestinationProject = this.DestinationProject;
                        dataContext.DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        //dataContext.OnContextChanged.ContextChanged += OnContextChanged;
                        SelectFeatureFolderUC = new UserControlSelectFeatureFolder(dataContext);
                    }
                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SerializableDbContext =
                            (FeatureUC.DataContext as FeatureViewModel).DbContext;
                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SerializableFeatureContext =
                            (FeatureUC.DataContext as FeatureViewModel).FeatureContext;
                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SelectedFeature =
                            (FeatureUC.DataContext as FeatureViewModel).SelectedFeature;

                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectFeatureFolderUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (T4EditorUC == null)
                    {
                        string templatePath = Path.Combine((SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).T4RootFolder, (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).T4SelectedFolder);
                        T4EditorViewModel dataContext = new T4EditorViewModel(templatePath);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        T4EditorUC = new UserControlT4Editor(dataContext);
                    }
                    (T4EditorUC.DataContext as T4EditorViewModel).T4TemplateFolder =
                        Path.Combine((SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).T4RootFolder, (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).T4SelectedFolder);
                    (T4EditorUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 4:
                    CurrentUiStepId = 5;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
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
                    if (GenerateUC == null)
                    {
                        GenerateCommonStaffViewModel dataContext = new GenerateCommonStaffViewModel();
                        dataContext.IsReady.IsReadyEvent += GenerateWebApiViewModel_IsReady;
                        GenerateUC = new UserControlGenerate(dataContext);
                    }
                    (GenerateUC.DataContext as GenerateCommonStaffViewModel).GenText = (T4EditorUC.DataContext as T4EditorViewModel).T4TempateText;
                    try
                    {
                        (GenerateUC.DataContext as GenerateCommonStaffViewModel)
                            .DoGenerateFeature(Dte, TextTemplating,
                            (T4EditorUC.DataContext as T4EditorViewModel).T4TempatePath,
                            (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SerializableDbContext,
                            (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SerializableFeatureContext,
                            (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).GetSelectedFeatureCommonShallowCopy(
                                (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).T4SelectedFolder,
                                (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).FileName),
                                (FeatureUC.DataContext as FeatureViewModel).AllowedFileTypes,
                                (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace
                            );
                        if (aDialogStarted)
                        {
                            int iOut;
                            aDialog.EndWaitDialog(out iOut);
                        }
                    }
                    catch (Exception e)
                    {
                        if (aDialogStarted)
                        {
                            int iOut;
                            aDialog.EndWaitDialog(out iOut);
                        }
                        MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        this.CurrentUserControl = GenerateUC;
                        this.OnPropertyChanged("CurrentUserControl");
                    }
                    break;
                case 5:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (FeatureUC.DataContext as FeatureViewModel).CheckIsReady();
                    this.CurrentUserControl = FeatureUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void CallBack_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }
        private void GenerateWebApiViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            SaveBtnEnabled = ready;
            OnPropertyChanged("SaveBtnEnabled");
        }

        #region SaveBtnCommand
        public override void SaveBtnCommandAction(Object param)
        {

            FeatureContextSerializable localFeatureContext = (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SerializableFeatureContext;
            FeatureSerializable featureSerializable = (GenerateUC.DataContext as GenerateCommonStaffViewModel).GeneratedFeature;

            FeatureSerializable existedFeatureSerializable = (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).SelectedFeature;
            existedFeatureSerializable.CommonStaffs = featureSerializable.CommonStaffs;
            (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).OnCreatedActionsChanged();


            string projectName = "";
            if ((FeatureUC.DataContext as FeatureViewModel).SelectedDbContext.CodeElementRef != null)
            {
                if ((FeatureUC.DataContext as FeatureViewModel).SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        (FeatureUC.DataContext as FeatureViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, (FeatureUC.DataContext as FeatureViewModel).SelectedDbContext.CodeElementFullName, (FeatureUC.DataContext as FeatureViewModel).FeatureContextSufix, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(SolutionDirectory, locFileName);
                string jsonString = JsonConvert.SerializeObject(localFeatureContext);
                File.WriteAllText(locFileName, jsonString);
            }

            try
            {
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                string FlNm = Path.Combine(
                    (InvitationUC.DataContext as InvitationViewModel).DestinationProjectRootFolder,
                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).DestinationFolder,
                    (SelectFeatureFolderUC.DataContext as SelectFeatureFolderViewModel).FileName
                    + (GenerateUC.DataContext as GenerateCommonStaffViewModel).FileExtension);
                File.WriteAllText(FlNm, (GenerateUC.DataContext as GenerateCommonStaffViewModel).GenerateText);
                DestinationProject.ProjectItems.AddFromFile(FlNm);
                MessageBox.Show(SuccessNotification, "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


    }
}
