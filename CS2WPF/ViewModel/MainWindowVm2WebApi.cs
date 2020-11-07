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
using System.Windows;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class MainWindowVm2WebApi : MainWindowBase
    {
        #region Fieds
        UserControlGenerate GenerateUC = null;
        UserControlT4Editor T4EditorUC = null;
        UserControlSelectSource SelectDbContextUC = null;
        UserControlCreateWebApi CreateWebApiUC = null;
        #endregion

        public MainWindowVm2WebApi(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory) : base(dte, textTemplating, dialogFactory)
        {
            InvitationViewModel InvitationVM = new InvitationViewModel();
            InvitationVM.WizardName = "#3 WebApi Wizard";
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
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 4:
                    CurrentUiStepId = 3;
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
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (CreateWebApiUC == null)
                    {
                        CreateWebApiViewModel dataContext = new CreateWebApiViewModel(Dte);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        dataContext.DestinationProject = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                        dataContext.DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        dataContext.DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        CreateWebApiUC = new UserControlCreateWebApi(dataContext);
                    }
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext =
                        (SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedCodeElement;
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 2:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (T4EditorUC == null)
                    {
                        //string templatePath = Path.Combine("Templates", "ViewModel.cs.t4");
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        string templatePath = Path.Combine(TemplatesFld, "WebApiServiceTmplst");
                        T4EditorViewModel dataContext = new T4EditorViewModel(templatePath);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        T4EditorUC = new UserControlT4Editor(dataContext);
                    }
                    (T4EditorUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    IVsThreadedWaitDialog2 aDialog = null;
                    bool aDialogStarted = false;
                    if (this.DialogFactory != null)
                    {
                        this.DialogFactory.CreateInstance(out aDialog);
                        if (aDialog != null) {
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
                            .DoGenerateViewModel(Dte, TextTemplating,
                            (T4EditorUC.DataContext as T4EditorViewModel).T4TempatePath,
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext,
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).GetSelectedModelShallowCopy());
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
                case 4:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region SaveBtnCommand
        public override void SaveBtnCommandAction(Object param)
        {
            DbContextSerializable localDbContext = (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext;
            ModelViewSerializable modelViewSerializable = (GenerateUC.DataContext as GenerateCommonStaffViewModel).GeneratedModelView;
            ModelViewSerializable existedModelViewSerializable =
                localDbContext.ModelViews.FirstOrDefault(mv => mv.ViewName == modelViewSerializable.ViewName);

            if (existedModelViewSerializable != null)
            {
                existedModelViewSerializable.ScalarProperties = modelViewSerializable.ScalarProperties;
                existedModelViewSerializable.WebApiServiceName = modelViewSerializable.WebApiServiceName;
                existedModelViewSerializable.IsWebApiSelectAll = modelViewSerializable.IsWebApiSelectAll;
                existedModelViewSerializable.IsWebApiSelectManyWithPagination = modelViewSerializable.IsWebApiSelectManyWithPagination;
                existedModelViewSerializable.IsWebApiSelectOneByPrimarykey = modelViewSerializable.IsWebApiSelectOneByPrimarykey;
                existedModelViewSerializable.IsWebApiAdd = modelViewSerializable.IsWebApiAdd;
                existedModelViewSerializable.IsWebApiUpdate = modelViewSerializable.IsWebApiUpdate;
                existedModelViewSerializable.IsWebApiDelete = modelViewSerializable.IsWebApiDelete;
                existedModelViewSerializable.WebApiServiceProject = modelViewSerializable.WebApiServiceProject;
                existedModelViewSerializable.WebApiServiceDefaultProjectNameSpace = modelViewSerializable.WebApiServiceDefaultProjectNameSpace;
                existedModelViewSerializable.WebApiServiceFolder = modelViewSerializable.WebApiServiceFolder;

                existedModelViewSerializable.UIFormProperties = modelViewSerializable.UIFormProperties;
                existedModelViewSerializable.UIListProperties = modelViewSerializable.UIListProperties;

            }
            else
            {
                localDbContext.ModelViews.Add(modelViewSerializable);
            }

            string projectName = "";
            if ((CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementRef != null)
            {
                if ((CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementFullName, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(SolutionDirectory, locFileName);
                string jsonString = JsonConvert.SerializeObject(localDbContext);
                File.WriteAllText(locFileName, jsonString);
            }

            try
            {
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                string FlNm = Path.Combine(
                    SolutionDirectory,
                    Path.GetDirectoryName(modelViewSerializable.WebApiServiceProject),
                    modelViewSerializable.WebApiServiceFolder,
                    modelViewSerializable.WebApiServiceName
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
    }
}
