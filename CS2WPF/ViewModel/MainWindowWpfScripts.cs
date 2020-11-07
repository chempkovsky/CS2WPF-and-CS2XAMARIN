using CS2WPF.Helpers;
using CS2WPF.Model;
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
    public class MainWindowWpfScripts : MainWindowBase
    {
        #region Fieds
        UserControlGenerate GenerateUC = null;
        UserControlT4Editor T4EditorUC = null;
        UserControlSelectSource SelectDbContextUC = null;
        UserControlCreateWebApi CreateWebApiUC = null;
        UserControlSelectFolder SelectFolderUC = null;
        #endregion
        public MainWindowWpfScripts(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory) : base(dte, textTemplating, dialogFactory)
        {
            InvitationViewModel InvitationVM = new InvitationViewModel();
            InvitationVM.WizardName = "#4 Wpf Forms Wizard";
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
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (SelectFolderUC.DataContext as SelectFolderViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectFolderUC;
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
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (CreateWebApiUC == null)
                    {
                        CreateWebApiViewModel dataContext = new CreateWebApiViewModel(Dte);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        dataContext.DestinationProject = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                        dataContext.DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        dataContext.DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        dataContext.IsWebServiceEditable = false;
                        dataContext.UIFormPropertiesVisibility = Visibility.Visible;
                        dataContext.UIListPropertiesVisibility = Visibility.Visible;

                        CreateWebApiUC = new UserControlCreateWebApi(dataContext);
                    }
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext =
                        (SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedCodeElement;
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;

                case 2:
                    bool hasUIFormPropertiesError = false;
                    string textUIFormPropertiesError = "";
                    foreach (ModelViewUIFormProperty modelViewUIFormProperty in (CreateWebApiUC.DataContext as CreateWebApiViewModel).UIFormProperties)
                    {
                        if (
                            ((modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                            (modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                            (modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.Typeahead)) &&
                            string.IsNullOrEmpty(modelViewUIFormProperty.ForeignKeyNameChain)
                            )
                        {
                            hasUIFormPropertiesError = true;
                            textUIFormPropertiesError += "\n InputTypeWhenAdd for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                            textUIFormPropertiesError += "\n but ForeignKeyNameChain is empty for this property.";
                            textUIFormPropertiesError += "\n Generators will not work correctly.";
                            textUIFormPropertiesError += "\n ";
                        }
                        if (
                            ((modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                            (modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                            (modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.Typeahead)) &&
                            string.IsNullOrEmpty(modelViewUIFormProperty.ForeignKeyNameChain)
                            )
                        {
                            hasUIFormPropertiesError = true;
                            textUIFormPropertiesError += "\n InputTypeWhenUpdate for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                            textUIFormPropertiesError += "\n but ForeignKeyNameChain is empty for this property.";
                            textUIFormPropertiesError += "\n Generators will not work correctly.";
                            textUIFormPropertiesError += "\n ";
                        }
                        if (
                            ((modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                            (modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                            (modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.Typeahead)) &&
                            string.IsNullOrEmpty(modelViewUIFormProperty.ForeignKeyNameChain)
                            )
                        {
                            hasUIFormPropertiesError = true;
                            textUIFormPropertiesError += "\n InputTypeWhenUpdate for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                            textUIFormPropertiesError += "\n but ForeignKeyNameChain is empty for this property.";
                            textUIFormPropertiesError += "\n Generators will not work correctly.";
                            textUIFormPropertiesError += "\n ";
                        }


                        if (
                            ((modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                            (modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                            (modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.Typeahead)) &&
                            (!modelViewUIFormProperty.IsShownInView)
                            )
                        {
                            hasUIFormPropertiesError = true;
                            textUIFormPropertiesError += "\n InputTypeWhenAdd for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                            textUIFormPropertiesError += "\n but IsShown is not checked.";
                            textUIFormPropertiesError += "\n Generators will not work correctly.";
                            textUIFormPropertiesError += "\n ";
                        }
                        if (
                            ((modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                            (modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                            (modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.Typeahead)) &&
                            (!modelViewUIFormProperty.IsShownInView)
                            )
                        {
                            hasUIFormPropertiesError = true;
                            textUIFormPropertiesError += "\n InputTypeWhenUpdate for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                            textUIFormPropertiesError += "\n but IsShown is not checked.";
                            textUIFormPropertiesError += "\n Generators will not work correctly.";
                            textUIFormPropertiesError += "\n ";
                        }
                        if (
                            ((modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                            (modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                            (modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.Typeahead)) &&
                            (!modelViewUIFormProperty.IsShownInView)
                            )
                        {
                            hasUIFormPropertiesError = true;
                            textUIFormPropertiesError += "\n InputTypeWhenUpdate for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                            textUIFormPropertiesError += "\n but IsShown is not checked.";
                            textUIFormPropertiesError += "\n Generators will not work correctly.";
                            textUIFormPropertiesError += "\n ";
                        }

                    }
                    int uIFormPropertiesCount = (CreateWebApiUC.DataContext as CreateWebApiViewModel).UIFormProperties.Count;
                    for (int inpTp = 1; inpTp < 4; inpTp++)
                    {
                        for (int i = 0; i < uIFormPropertiesCount - 1; i++)
                        {

                            ModelViewUIFormProperty modelViewUIFormProperty =
                                (CreateWebApiUC.DataContext as CreateWebApiViewModel).UIFormProperties[i];
                            if (string.IsNullOrEmpty(modelViewUIFormProperty.ForeignKeyNameChain))
                            {
                                continue;
                            }
                            if (inpTp == 1) {
                                if (!((modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                                     (modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                                     (modelViewUIFormProperty.InputTypeWhenAdd == InputTypeEnum.SearchDialog)))
                                {
                                    continue;
                                }
                            } else if (inpTp == 2) {
                                if (!((modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                                     (modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                                     (modelViewUIFormProperty.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)))
                                {
                                    continue;
                                }

                            } else {
                                if (!((modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                                     (modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.Typeahead) ||
                                     (modelViewUIFormProperty.InputTypeWhenDelete == InputTypeEnum.SearchDialog)))
                                {
                                    continue;
                                }
                            }
                            for(int k = i+1; k < uIFormPropertiesCount; k++)
                            {
                                ModelViewUIFormProperty modelViewUIFormProperty2 =
                                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).UIFormProperties[k];
                                if (string.IsNullOrEmpty(modelViewUIFormProperty2.ForeignKeyNameChain))
                                {
                                    continue;
                                }
                                if(modelViewUIFormProperty2.ForeignKeyNameChain != modelViewUIFormProperty.ForeignKeyNameChain)
                                {
                                    continue;
                                }
                                if (inpTp == 1)
                                {
                                    if ((modelViewUIFormProperty2.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                                        (modelViewUIFormProperty2.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                                        (modelViewUIFormProperty2.InputTypeWhenAdd == InputTypeEnum.SearchDialog))
                                    {
                                        hasUIFormPropertiesError = true;
                                        textUIFormPropertiesError += "\n InputTypeWhenAdd for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                                        textUIFormPropertiesError += "\n InputTypeWhenAdd for UIFormProperty named [" + modelViewUIFormProperty2.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                                        textUIFormPropertiesError += "\n Both properties have the same ForeignKeyNameChain =["+ modelViewUIFormProperty.ForeignKeyNameChain + "].";
                                        textUIFormPropertiesError += "\n For InputTypeWhenAdd only one property for each ForeignKeyNameChain can be set to one of the values (Combo, SearchDialog, Typeahead).";
                                        textUIFormPropertiesError += "\n Generators will not work correctly.";
                                        textUIFormPropertiesError += "\n ";
                                    }
                                }
                                else if (inpTp == 2)
                                {
                                    if ((modelViewUIFormProperty2.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                                        (modelViewUIFormProperty2.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                                        (modelViewUIFormProperty2.InputTypeWhenUpdate == InputTypeEnum.SearchDialog))
                                    {
                                        textUIFormPropertiesError += "\n InputTypeWhenUpdate for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                                        textUIFormPropertiesError += "\n InputTypeWhenUpdate for UIFormProperty named [" + modelViewUIFormProperty2.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                                        textUIFormPropertiesError += "\n Both properties have the same ForeignKeyNameChain =[" + modelViewUIFormProperty.ForeignKeyNameChain + "].";
                                        textUIFormPropertiesError += "\n For InputTypeWhenUpdate only one property for each ForeignKeyNameChain can be set to one of the values (Combo, SearchDialog, Typeahead).";
                                        textUIFormPropertiesError += "\n Generators will not work correctly.";
                                        textUIFormPropertiesError += "\n ";
                                    }

                                }
                                else
                                {
                                    if ((modelViewUIFormProperty2.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                                         (modelViewUIFormProperty2.InputTypeWhenDelete == InputTypeEnum.Typeahead) ||
                                         (modelViewUIFormProperty2.InputTypeWhenDelete == InputTypeEnum.SearchDialog))
                                    {
                                        textUIFormPropertiesError += "\n InputTypeWhenDelete for UIFormProperty named [" + modelViewUIFormProperty.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                                        textUIFormPropertiesError += "\n InputTypeWhenDelete for UIFormProperty named [" + modelViewUIFormProperty2.ViewPropertyName + "] was set to one of the values (Combo, SearchDialog, Typeahead)";
                                        textUIFormPropertiesError += "\n Both properties have the same ForeignKeyNameChain =[" + modelViewUIFormProperty.ForeignKeyNameChain + "].";
                                        textUIFormPropertiesError += "\n For InputTypeWhenDelete only one property for each ForeignKeyNameChain can be set to one of the values (Combo, SearchDialog, Typeahead).";
                                        textUIFormPropertiesError += "\n Generators will not work correctly.";
                                        textUIFormPropertiesError += "\n ";
                                    }
                                }
                            }
                        }
                    }


                    if (hasUIFormPropertiesError)
                    {
                        textUIFormPropertiesError += "\n Would you like to continue ?";
                        if(MessageBox.Show(textUIFormPropertiesError, "Error", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if(SelectFolderUC == null)
                    {
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        SelectFolderViewModel dataContext = new SelectFolderViewModel(Dte, TextTemplating, DialogFactory, (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext, TemplatesFld, "WpfScriptsTmplst", "BatchScriptsTmplst");
                        dataContext.DestinationProjectRootFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationProjectRootFolder;
                        dataContext.DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        dataContext.ContextItemViewName = (CreateWebApiUC.DataContext as CreateWebApiViewModel).ContextItemViewName;
                        dataContext.DestinationProjectName = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                        dataContext.DestinationProject = this.DestinationProject;
                        dataContext.DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        dataContext.OnContextChanged.ContextChanged += OnContextChanged;
                        SelectFolderUC = new UserControlSelectFolder(dataContext);
                    }
                    (SelectFolderUC.DataContext as SelectFolderViewModel).SerializableDbContext =
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext;
                    (SelectFolderUC.DataContext as SelectFolderViewModel).SelectedModel =
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedModel;

                    (SelectFolderUC.DataContext as SelectFolderViewModel).UIFormProperties = (CreateWebApiUC.DataContext as CreateWebApiViewModel).UIFormProperties;
                    (SelectFolderUC.DataContext as SelectFolderViewModel).UIListProperties = (CreateWebApiUC.DataContext as CreateWebApiViewModel).UIListProperties;

                    (SelectFolderUC.DataContext as SelectFolderViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectFolderUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;

                case 3:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (T4EditorUC == null)
                    {
                        string templatePath = Path.Combine((SelectFolderUC.DataContext as SelectFolderViewModel).T4RootFolder, (SelectFolderUC.DataContext as SelectFolderViewModel).T4SelectedFolder);
                        T4EditorViewModel dataContext = new T4EditorViewModel(templatePath);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        T4EditorUC = new UserControlT4Editor(dataContext);
                    }
                    (T4EditorUC.DataContext as T4EditorViewModel).T4TemplateFolder =
                        Path.Combine((SelectFolderUC.DataContext as SelectFolderViewModel).T4RootFolder, (SelectFolderUC.DataContext as SelectFolderViewModel).T4SelectedFolder);
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
                            .DoGenerateViewModel(Dte, TextTemplating,
                            (T4EditorUC.DataContext as T4EditorViewModel).T4TempatePath,
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext,
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).GetSelectedModelCommonShallowCopy(
                                (SelectFolderUC.DataContext as SelectFolderViewModel).T4SelectedFolder,
                                (SelectFolderUC.DataContext as SelectFolderViewModel).FileName
                                ),
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
            if (modelViewSerializable.ViewName == (CreateWebApiUC.DataContext as CreateWebApiViewModel).ContextItemViewName)
            {
                localDbContext.CommonStaffs = modelViewSerializable.CommonStaffs;
                if( (SelectFolderUC.DataContext as SelectFolderViewModel).SelectedModel != null)
                {
                    (SelectFolderUC.DataContext as SelectFolderViewModel).SelectedModel.CommonStaffs = localDbContext.CommonStaffs;
                }
                if (localDbContext.CommonStaffs != null)
                {
                    CommonStaffSerializable commonStaffSerializable = localDbContext.CommonStaffs
                        .Where(c => c.FileType == (SelectFolderUC.DataContext as SelectFolderViewModel).T4SelectedFolder)
                        .FirstOrDefault();
                    if (commonStaffSerializable != null)
                    {
                        commonStaffSerializable.Extension =
                            (GenerateUC.DataContext as GenerateCommonStaffViewModel).FileExtension;
                    }
                }
            }
            else
            {
                ModelViewSerializable existedModelViewSerializable =
                    localDbContext.ModelViews.FirstOrDefault(mv => mv.ViewName == modelViewSerializable.ViewName);
                if (modelViewSerializable.CommonStaffs != null)
                {
                    CommonStaffSerializable commonStaffSerializable =
                        modelViewSerializable.CommonStaffs
                        .Where(c => c.FileType == (SelectFolderUC.DataContext as SelectFolderViewModel).T4SelectedFolder)
                        .FirstOrDefault();
                    if (commonStaffSerializable != null)
                    {
                        commonStaffSerializable.Extension =
                            (GenerateUC.DataContext as GenerateCommonStaffViewModel).FileExtension;
                    }
                }

                if (existedModelViewSerializable != null)
                {
                    existedModelViewSerializable.ScalarProperties = modelViewSerializable.ScalarProperties;
                    existedModelViewSerializable.CommonStaffs = modelViewSerializable.CommonStaffs;
                    existedModelViewSerializable.UIFormProperties = modelViewSerializable.UIFormProperties;
                    existedModelViewSerializable.UIListProperties = modelViewSerializable.UIListProperties;

                }
                else
                {
                    localDbContext.ModelViews.Add(modelViewSerializable);
                }
            }
            (SelectFolderUC.DataContext as SelectFolderViewModel).OnCreatedActionsChanged();
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
                // insert code here

                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                string FlNm = Path.Combine(
                    (InvitationUC.DataContext as InvitationViewModel).DestinationProjectRootFolder,
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).DestinationFolder,
                    (SelectFolderUC.DataContext as SelectFolderViewModel).FileName
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
        private void OnContextChanged(Object sender)
        {
            if (CreateWebApiUC == null) return;
            if(CreateWebApiUC.DataContext == null) return;
            if ((CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext == null) return;
            if ((CreateWebApiUC.DataContext as CreateWebApiViewModel).ModelViews == null) return;
            ModelViewSerializable mv = 
                (CreateWebApiUC.DataContext as CreateWebApiViewModel).ModelViews.Where(m => m.ViewName == (CreateWebApiUC.DataContext as CreateWebApiViewModel).ContextItemViewName).FirstOrDefault();
            if (mv == null) return;
            DbContextSerializable cn = (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext;
            if (cn.CommonStaffs == null) return;
            if (mv.CommonStaffs == null) mv.CommonStaffs = new List<CommonStaffSerializable>();
            mv.CommonStaffs.Clear();
            foreach(CommonStaffSerializable csf in cn.CommonStaffs)
            {
                mv.CommonStaffs.Add(csf);
            }
        }

    }
}
