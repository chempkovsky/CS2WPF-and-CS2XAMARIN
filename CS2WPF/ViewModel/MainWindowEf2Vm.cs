using CS2WPF.View;
using System;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows;
using CS2WPF.Model.Serializable;
using System.Collections.Generic;
using CS2WPF.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class MainWindowEf2Vm : MainWindowBase
    {
        #region Fieds
        protected UserControlGenerate GenerateUC = null;
        protected UserControlGenerate GeneratePageUC = null;
        protected UserControlT4Editor T4EditorUC = null;
        protected UserControlT4Editor T4EditorPageUC = null;

        UserControlSelectSource SelectDbContextUC = null;
        UserControlSelectSource SelectSourceEntityUC = null;
        UserControlCreateView CreateViewUC = null;
        DbContextSerializable CurrentDbContext = null;

        UserControlSelectExisting SelectExistingUC = null;
        #endregion





        public MainWindowEf2Vm(PrismModuleModifier prismModuleModifier, DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory) : base(prismModuleModifier, dte, textTemplating, dialogFactory)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            InvitationViewModel InvitationVM = new InvitationViewModel();
            InvitationVM.WizardName = "#2 View Wizard";
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
                case 0:
                    break;
                case 1:
                    this.CurrentUserControl = this.InvitationUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    CurrentUiStepId = 0;
                    PrevBtnEnabled = false;
                    NextBtnEnabled = true;
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
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectSourceEntityUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;

                case 4:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    (SelectExistingUC.DataContext as SelectExistingViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectExistingUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;

                case 5:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = CreateViewUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;


                case 6:
                    CurrentUiStepId = 5;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;

                case 7:
                    CurrentUiStepId = 6;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = GenerateUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 8:
                    CurrentUiStepId = 7;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    (T4EditorPageUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorPageUC;
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
                        dataContext.IsReady.IsReadyEvent += SelectDbContextViewModel_IsReady;
                    }
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).DoAnaliseDbContext();
                    this.CurrentUserControl = SelectDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");


                    break;
                case 1:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    if (SelectSourceEntityUC == null)
                    {
                        SelectEntityForGivenDbContextViewModel dataContext = new SelectEntityForGivenDbContextViewModel(Dte);
                        dataContext.UiCommandButtonVisibility = Visibility.Collapsed;
                        dataContext.IsReady.IsReadyEvent += SelectEntityForGivenDbContextViewModel_IsReady;
                        SelectSourceEntityUC = new UserControlSelectSource(dataContext);
                    }
                    (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext =
                        (SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedCodeElement;
                    (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).CheckIsReady();
                    OnDbContextChanged();
                    this.CurrentUserControl = SelectSourceEntityUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;

                case 2:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    if(SelectExistingUC == null)
                    {
                        SelectExistingViewModel dataContext = new SelectExistingViewModel(Dte);
                        dataContext.IsReady.IsReadyEvent += SelectEntityForGivenDbContextViewModel_IsReady;
                        SelectExistingUC = new UserControlSelectExisting(dataContext);
                    }
                    (SelectExistingUC.DataContext as SelectExistingViewModel).CurrentDbContext = CurrentDbContext;
                    (SelectExistingUC.DataContext as SelectExistingViewModel).SelectedEntity =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedCodeElement;

                    (SelectExistingUC.DataContext as SelectExistingViewModel).DestinationProject = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                    (SelectExistingUC.DataContext as SelectExistingViewModel).DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                    (SelectExistingUC.DataContext as SelectExistingViewModel).DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                    (SelectExistingUC.DataContext as SelectExistingViewModel).DbSetProppertyName = (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedProppertyName;

                    (SelectExistingUC.DataContext as SelectExistingViewModel).DoAnalize();
                    this.CurrentUserControl = SelectExistingUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    if (CreateViewUC == null)
                    {
                        CreateViewViewModel dataContext = new CreateViewViewModel(Dte);
                        dataContext.IsReady.IsReadyEvent += SelectEntityForGivenDbContextViewModel_IsReady;
                        dataContext.DestinationProject = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                        dataContext.DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        dataContext.DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        CreateViewUC = new UserControlCreateView(dataContext);


                    }
                    (CreateViewUC.DataContext as CreateViewViewModel).SelectedDbContext =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext;
                    (CreateViewUC.DataContext as CreateViewViewModel).SelectedEntity =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedCodeElement;
                    (CreateViewUC.DataContext as CreateViewViewModel).CurrentDbContext = CurrentDbContext;
                    (CreateViewUC.DataContext as CreateViewViewModel).DestinationDbSetProppertyName =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedProppertyName;
                    ModelViewSerializable srcModel = null;
                    if ((SelectExistingUC.DataContext as SelectExistingViewModel).IsSelectExisting)
                    {
                        srcModel = (SelectExistingUC.DataContext as SelectExistingViewModel).SelectedModel;
                    }
                    (CreateViewUC.DataContext as CreateViewViewModel).DoAnalize(srcModel);
                    this.CurrentUserControl = CreateViewUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;

                case 4:
                    string checkErrorsText = (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.CheckCorrect();
                    if (!string.IsNullOrEmpty(checkErrorsText))
                    {
                        (CreateViewUC.DataContext as CreateViewViewModel).CheckErrorsText = checkErrorsText;
                        return;
                    }

                    CurrentUiStepId = 5;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    if (T4EditorUC == null)
                    {
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        string templatePath = Path.Combine(TemplatesFld, "ViewModelTmplst");
                        T4EditorViewModel dataContext = new T4EditorViewModel(templatePath);
                        dataContext.IsReady.IsReadyEvent += T4EditorViewModel_IsReady;
                        T4EditorUC = new UserControlT4Editor(dataContext);
                    }
                    (T4EditorUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;
                case 5:
                    CurrentUiStepId = 6;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
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
                        GenerateViewModel generateViewModel = new GenerateViewModel();
                        generateViewModel.IsReady.IsReadyEvent += GenerateViewModel_IsReady;
                        GenerateUC = new UserControlGenerate(generateViewModel);
                    }
                    (GenerateUC.DataContext as GenerateViewModel).GenText = (T4EditorUC.DataContext as T4EditorViewModel).T4TempateText;
                    try
                    {
                        (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.RootEntityDbContextPropertyName =
                            (CreateViewUC.DataContext as CreateViewViewModel).DestinationDbSetProppertyName;
                        (GenerateUC.DataContext as GenerateViewModel).DoGenerateViewModel(prismModuleModifier, Dte, TextTemplating, null,
                            (T4EditorUC.DataContext as T4EditorViewModel).T4TempatePath,
                            (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel);
                        NextBtnEnabled = true;
                        if (aDialogStarted)
                        {
                            int iOut;
                            aDialog.EndWaitDialog(out iOut);
                        }
                        MessageBox.Show(SuccessNotification, "Done", MessageBoxButton.OK, MessageBoxImage.Information);
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
                case 6:
                    CurrentUiStepId = 7;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    if (T4EditorPageUC == null)
                    {
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        string templatePath = Path.Combine(TemplatesFld, "ViewPageModelTmplst");
                        T4EditorViewModel dataContext = new T4EditorViewModel(templatePath);
                        dataContext.IsReady.IsReadyEvent += T4EditorViewModel_IsReady;
                        T4EditorPageUC = new UserControlT4Editor(dataContext);
                    }
                    (T4EditorPageUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorPageUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 7:
                    CurrentUiStepId = 8;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    IVsThreadedWaitDialog2 aaDialog = null;
                    bool aaDialogStarted = false;
                    if (this.DialogFactory != null)
                    {
                        this.DialogFactory.CreateInstance(out aaDialog);
                        if (aaDialog != null)
                        {
                            aaDialogStarted = aaDialog.StartWaitDialog("Generation started", "VS is Busy", "Please wait", null, "Generation started", 0, false, true) == VSConstants.S_OK;
                        }
                    }
                    if (GeneratePageUC == null)
                    {
                        GenerateViewPageModel generateViewModel = new GenerateViewPageModel();
                        generateViewModel.IsReady.IsReadyEvent += GenerateViewModel_IsReady;
                        GeneratePageUC = new UserControlGenerate(generateViewModel);
                    }
                    (GeneratePageUC.DataContext as GenerateViewPageModel).GenText = (T4EditorPageUC.DataContext as T4EditorViewModel).T4TempateText;
                    try
                    {
                        (GeneratePageUC.DataContext as GenerateViewPageModel).GeneratedModelView =
                            (GenerateUC.DataContext as GenerateViewModel).GeneratedModelView;
                        (GeneratePageUC.DataContext as GenerateViewPageModel).DoGenerateViewPageModel(Dte, TextTemplating, null,
                            (T4EditorPageUC.DataContext as T4EditorViewModel).T4TempatePath);
                        if (aaDialogStarted)
                        {
                            int iOut;
                            aaDialog.EndWaitDialog(out iOut);
                        }
                    }
                    catch (Exception e)
                    {
                        if (aaDialogStarted)
                        {
                            int iOut;
                            aaDialog.EndWaitDialog(out iOut);
                        }
                        MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        this.CurrentUserControl = GeneratePageUC;
                        this.OnPropertyChanged("CurrentUserControl");
                    }
                    break;
                case 8:
                    CurrentUiStepId = 1;
                    NextBtnCommandAction(param);
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region SaveBtnCommand
        public override void SaveBtnCommandAction(Object param)
        {
            ModelViewSerializable modelViewSerializable = new ModelViewSerializable();
            (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.ModelViewAssingTo(modelViewSerializable);
            ModelViewSerializable existedModelViewSerializable =
                CurrentDbContext.ModelViews.FirstOrDefault(mv => mv.ViewName == modelViewSerializable.ViewName);
            if (existedModelViewSerializable != null)
            {
                existedModelViewSerializable.PageViewName = modelViewSerializable.PageViewName;
                existedModelViewSerializable.RootEntityClassName = modelViewSerializable.RootEntityClassName;
                existedModelViewSerializable.RootEntityFullClassName = modelViewSerializable.RootEntityFullClassName;
                existedModelViewSerializable.RootEntityUniqueProjectName = modelViewSerializable.RootEntityUniqueProjectName;
                existedModelViewSerializable.RootEntityDbContextPropertyName = modelViewSerializable.RootEntityDbContextPropertyName;
                existedModelViewSerializable.ViewProject = modelViewSerializable.ViewProject;
                existedModelViewSerializable.ViewDefaultProjectNameSpace = modelViewSerializable.ViewDefaultProjectNameSpace;
                existedModelViewSerializable.ViewFolder = modelViewSerializable.ViewFolder;
                existedModelViewSerializable.GenerateJSonAttribute = modelViewSerializable.GenerateJSonAttribute;
                if ((existedModelViewSerializable.ScalarProperties != null) && (modelViewSerializable.ScalarProperties != null)) {
                    foreach (ModelViewPropertyOfVwSerializable srcProp in existedModelViewSerializable.ScalarProperties)
                    {
                        ModelViewPropertyOfVwSerializable dest = null;
                        if (string.IsNullOrEmpty(srcProp.ForeignKeyNameChain)) {
                            dest =
                                modelViewSerializable.ScalarProperties
                                .Where(p => ((p.OriginalPropertyName == srcProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain))).FirstOrDefault();
                        } else
                        {
                            dest =
                                modelViewSerializable.ScalarProperties
                                .Where(p => ((p.OriginalPropertyName == srcProp.OriginalPropertyName) && (p.ForeignKeyNameChain == srcProp.ForeignKeyNameChain))).FirstOrDefault();
                        }
                        if(dest != null)
                        {
                            dest.IsUsedBySorting = srcProp.IsUsedBySorting;
                            dest.IsUsedByfilter = srcProp.IsUsedByfilter;
                        }
                    }
                }
                existedModelViewSerializable.ScalarProperties = modelViewSerializable.ScalarProperties;
                existedModelViewSerializable.ForeignKeys = modelViewSerializable.ForeignKeys;
                existedModelViewSerializable.PrimaryKeyProperties = modelViewSerializable.PrimaryKeyProperties;
                existedModelViewSerializable.AllProperties = modelViewSerializable.AllProperties;
                existedModelViewSerializable.UIFormProperties = modelViewSerializable.UIFormProperties;
                existedModelViewSerializable.UIListProperties = modelViewSerializable.UIListProperties;
            }
            else
            {
                CurrentDbContext.ModelViews.Add(modelViewSerializable);
            }
            if (SelectExistingUC != null)
            {
                (SelectExistingUC.DataContext as SelectExistingViewModel).UpdateModelViews(modelViewSerializable);
            }


            string projectName = "";
            if ((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef != null)
            {
                if ((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementFullName, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(SolutionDirectory, locFileName);
                string jsonString = JsonConvert.SerializeObject(CurrentDbContext);
                File.WriteAllText(locFileName, jsonString);
            }
            try
            {
                
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                if (CurrentUiStepId == 6)
                {
                    string FlNm = Path.Combine(
                    SolutionDirectory,
                    Path.GetDirectoryName((CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.ViewProject),
                    (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.ViewFolder,
                    (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.ViewName
                    + (GenerateUC.DataContext as GenerateViewModel).FileExtension);
                    File.WriteAllText(FlNm, (GenerateUC.DataContext as GenerateViewModel).GenerateText);
                    DestinationProject.ProjectItems.AddFromFile(FlNm);
                } else if (CurrentUiStepId == 8)
                {
                    string FlNm = Path.Combine(
                    SolutionDirectory,
                    Path.GetDirectoryName((CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.ViewProject),
                    (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.ViewFolder,
                    (CreateViewUC.DataContext as CreateViewViewModel).SelectedModel.PageViewName
                    //+ "." 
                    + (GeneratePageUC.DataContext as GenerateViewPageModel).FileExtension);
                    File.WriteAllText(FlNm, (GeneratePageUC.DataContext as GenerateViewPageModel).GenerateText);
                    DestinationProject.ProjectItems.AddFromFile(FlNm);
                }

                MessageBox.Show(SuccessNotification, "Done",MessageBoxButton.OK,MessageBoxImage.Information );
            } catch(Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private object ModelViewSerializable(CreateViewViewModel createViewViewModel)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void OnDbContextChanged()
        {
            bool isChanged = CurrentDbContext == null;
            string locFileName = "";
            if (!isChanged)
            {
                isChanged = ((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext == null);
            }
            if (!isChanged)
            {
                isChanged = string.IsNullOrEmpty( CurrentDbContext.DbContextProjectUniqueName) || string.IsNullOrEmpty(CurrentDbContext.DbContextFullClassName);
            }
            if (!isChanged)
            {
                isChanged = !CurrentDbContext.DbContextFullClassName.Equals((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementFullName);
            }
            if (!isChanged)
            {
                //CurrentDbContext.DbContextProject
                isChanged = (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef == null;
                if (!isChanged) {
                    isChanged = (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem == null;
                }
                if (!isChanged)
                {
                    isChanged = (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject == null;
                }
                if (!isChanged)
                {
                    isChanged = !CurrentDbContext.DbContextProjectUniqueName.Equals(
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName,
                        StringComparison.OrdinalIgnoreCase);
                }
            }
            if (isChanged)
            {
                string projectName = "";
                if((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef != null)
                {
                    if ((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem != null)
                    {
                        projectName =
                            (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                    }
                }
                if (!string.IsNullOrEmpty(projectName))
                {
                    locFileName = Path.Combine(projectName,
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementFullName, JsonExtension);
                    locFileName = locFileName.Replace("\\", ".");
                    SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                    locFileName = Path.Combine(SolutionDirectory, locFileName);
                }
            }
            if (!string.IsNullOrEmpty(locFileName))
            {
                if (File.Exists(locFileName))
                {
                    string jsonString = System.IO.File.ReadAllText(locFileName);
                    CurrentDbContext = JsonConvert.DeserializeObject<DbContextSerializable>(jsonString);
                }
                else
                {
                    if (CurrentDbContext == null) CurrentDbContext = new DbContextSerializable();
                    CurrentDbContext.DbContextClassName =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementName;
                    CurrentDbContext.DbContextFullClassName =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementFullName;
                    CurrentDbContext.DbContextProjectUniqueName =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                    if (CurrentDbContext.ModelViews == null)
                    {
                        CurrentDbContext.ModelViews = new List<ModelViewSerializable>();
                    } else
                    {
                        CurrentDbContext.ModelViews.Clear();
                    }
                }
                return;
            }
            if (isChanged)
            {
                if (CurrentDbContext == null) CurrentDbContext = new DbContextSerializable();
                CurrentDbContext.DbContextClassName = "";
                CurrentDbContext.DbContextFullClassName = "";
                CurrentDbContext.DbContextProjectUniqueName = "";
                if (CurrentDbContext.ModelViews == null)
                {
                    CurrentDbContext.ModelViews = new List<ModelViewSerializable>();
                }
                else
                {
                    CurrentDbContext.ModelViews.Clear();
                }
            }
        }

        private void GenerateViewModel_IsReady(Object sender, bool ready)
        {
            SaveBtnEnabled = ready;
            OnPropertyChanged("SaveBtnEnabled");

        }

        private void SelectDbContextViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

        private void SelectEntityForGivenDbContextViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

        private void T4EditorViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

    }
}
