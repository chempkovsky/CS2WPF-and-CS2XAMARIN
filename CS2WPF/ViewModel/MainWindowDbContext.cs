using CS2WPF.Helpers;
using CS2WPF.Model;
using CS2WPF.Model.AnalyzeOnModelCreating;
using CS2WPF.View;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
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
    public class MainWindowDbContext : MainWindowBase
    {
        #region Fieds
        UserControlSelectSource SelectDbContextUC = null;
        UserControlT4SelectTemplate T4SelectTemplateUC = null;
        UserControlT4Editor T4Editor = null;
        UserControlGenerate GenerateDbContextUC = null;
        UserControlSelectSource SelectSourceEntityUC = null;
        UserControlCreatePrimKey CreatePrimKeyUC = null;
        UserControlSelectForeignKey SelectForeignKeyUC = null;
        UserControlCreateForeignKey CreateForeignKeyUC = null;
        #endregion





        public MainWindowDbContext(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory) : base(dte, textTemplating, dialogFactory)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            InvitationViewModel InvitationVM = new InvitationViewModel();
            InvitationVM.WizardName = "#1 DbContext Wizard";
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
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = SelectDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;
                case 3:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).DoAnalize();
                    (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectSourceEntityUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;
                case 4:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).DoAnaliseEx();
                    this.CurrentUserControl = SelectForeignKeyUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 5:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    break;

                case 101:
                    CurrentUiStepId = 1;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = SelectDbContextUC;
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).DoAnaliseDbContext();
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 102:
                    CurrentUiStepId = 101;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = T4SelectTemplateUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 103:
                    if (!(T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).ShowT4Template)
                    {
                        CurrentUiStepId = 102;
                        PrevBtnCommandAction(param);
                        return;
                    }
                    CurrentUiStepId = 102;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    this.CurrentUserControl = T4Editor;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 201:
                    CurrentUiStepId = 3;
                    PrevBtnCommandAction(param);
                    return;
                case 301:
                    CurrentUiStepId = 4;
                    PrevBtnCommandAction(param);
                    return;
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
                        dataContext.UiCommandCaption3 = "NameSpace: " + (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        string folder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        if (!string.IsNullOrEmpty(folder))
                        {
                            dataContext.UiCommandCaption3 = dataContext.UiCommandCaption3 + "." + folder.Replace("\\", ".");
                        }
                        SelectDbContextUC = new UserControlSelectSource(dataContext);
                        dataContext.IsReady.IsReadyEvent += SelectDbContextViewModel_IsReady;
                        dataContext.UiCommandButtonClicked.ButtonClickedEvent += SelectDbContextViewModel_ButtonClicked;
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
                        dataContext.IsReady.IsReadyEvent += SelectEntityForGivenDbContextViewModel_IsReady;
                        dataContext.UiCommandButtonClicked.ButtonClickedEvent += SelectEntityForGivenDbContextViewModel_ButtonClicked;
                        SelectSourceEntityUC = new UserControlSelectSource(dataContext);
                    }
                    (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext =
                        (SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedCodeElement;
                    if ((SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedCodeElement != null)
                    {
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).DoAnalize();
                    }
                    else
                    {
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).CheckIsReady();
                    }
                    this.CurrentUserControl = SelectSourceEntityUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 2:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (SelectForeignKeyUC == null)
                    {
                        SelectForeignKeyViewModel dataContext = new SelectForeignKeyViewModel(Dte);
                        dataContext.IsReady.IsReadyEvent += SelectForeignKeyViewModel_IsReady;
                        SelectForeignKeyUC = new UserControlSelectForeignKey(dataContext);
                        dataContext.PrincipalKeyButtonClicked.ButtonClickedEvent += UiBtnCommandPrincipalKey_ButtonClicked;
                        dataContext.ForeignKeyButtonClicked.ButtonClickedEvent += UiBtnCommandForeignKey_ButtonClicked;
                    }
                    (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).SelectedDbContext =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext;
                    (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).SelectedEntity =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedCodeElement;
                    (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).DoAnaliseEx();
                    this.CurrentUserControl = SelectForeignKeyUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    if (CreateForeignKeyUC == null)
                    {
                        CreateForeignKeyViewModel dataContext = new CreateForeignKeyViewModel(Dte, TextTemplating);
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        dataContext.TemplateFolder = Path.Combine(TemplatesFld, "ForeignKeyTmplts");
                        dataContext.TemplateOneToOneFolder = Path.Combine(TemplatesFld, "ForeignKeyTmplts", "OneToOne");
                        dataContext.TemplateOneToCollectionFolder = Path.Combine(TemplatesFld, "ForeignKeyTmplts", "OneToCollection");
                        dataContext.IsReady.IsReadyEvent += SelectForeignKeyViewModel_IsReady;
                        CreateForeignKeyUC = new UserControlCreateForeignKey(dataContext);
                    }
                    (CreateForeignKeyUC.DataContext as CreateForeignKeyViewModel).SelectedDbContext =
                        (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).SelectedDbContext;
                    (CreateForeignKeyUC.DataContext as CreateForeignKeyViewModel).SelectedEntity =
                        (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).SelectedEntity;

                    string navName = null;
                    if (param is FluentAPIForeignKey)
                    {
                        if ((param as FluentAPIForeignKey) != null)
                        {
                            navName = (param as FluentAPIForeignKey).NavigationName;
                        }
                    }
                    (CreateForeignKeyUC.DataContext as CreateForeignKeyViewModel).DoAnalise(navName);
                    if (string.IsNullOrEmpty(navName))
                    {
                        (CreateForeignKeyUC.DataContext as CreateForeignKeyViewModel).SelectedForeignKey =
                            (SelectForeignKeyUC.DataContext as SelectForeignKeyViewModel).SelectedForeignKey;
                    }

                    this.CurrentUserControl = CreateForeignKeyUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;
                case 4:
                    CurrentUiStepId = 1;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    NextBtnCommandAction(param);
                    break;

                case 100:
                    CurrentUiStepId = 101;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if(T4SelectTemplateUC == null)
                    {
                        Selectt4TemplateViewModel dataContext = new Selectt4TemplateViewModel();
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        dataContext.TemplateFolder = Path.Combine(TemplatesFld, "DbContextTmplts");
                        dataContext.IsReady.IsReadyEvent += Selectt4TemplateViewModel_IsReady;
                        T4SelectTemplateUC = new UserControlT4SelectTemplate(dataContext);
                    }
                    (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).DoAnalise();
                    this.CurrentUserControl = T4SelectTemplateUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 101:
                    CurrentUiStepId = 102;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    string templatePath101 = Path.Combine(
                        (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).TemplateFolder,
                        (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).SelectedTemplate);
                    if (T4Editor == null) 
                    {
                        T4EditorViewModel dataContext = new T4EditorViewModel((T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).TemplateFolder);
                        dataContext.T4SelectedTemplate = (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).SelectedTemplate;
                        dataContext.T4TempateCaption = (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).SelectedTemplate;
                        T4Editor = new UserControlT4Editor(dataContext);
                    } 
                    else
                    {
                        if (!templatePath101.Equals((T4Editor.DataContext as T4EditorViewModel).T4TempatePath, StringComparison.OrdinalIgnoreCase))
                        {
                            (T4Editor.DataContext as T4EditorViewModel).T4SelectedTemplate = (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).SelectedTemplate;
                            (T4Editor.DataContext as T4EditorViewModel).T4TempateCaption = (T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).SelectedTemplate;
                            (T4Editor.DataContext as T4EditorViewModel).T4TempatePath = templatePath101;
                            (T4Editor.DataContext as T4EditorViewModel).ReadTemplate();
                        }
                    }
                    if (!(T4SelectTemplateUC.DataContext as Selectt4TemplateViewModel).ShowT4Template)
                    {
                        NextBtnCommandAction(param);
                        return;
                    }
                    this.CurrentUserControl = T4Editor;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 102:
                    CurrentUiStepId = 103;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    if (GenerateDbContextUC == null)
                    {
                        GenerateDbContextViewModel dataContext = new GenerateDbContextViewModel();
                        dataContext.IsReady.IsReadyEvent += GenerateDbContext_IsReady;
                        GenerateDbContextUC = new UserControlGenerate(dataContext);
                        
                    }
                    (GenerateDbContextUC.DataContext as GenerateDbContextViewModel).GenText =
                        (T4Editor.DataContext as T4EditorViewModel).T4TempateText;

                    string nameSpace102 = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                    string folder102 = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                    if (!string.IsNullOrEmpty(folder102))
                    {
                        nameSpace102 = nameSpace102 + "." + folder102.Replace("\\", ".");
                    }
                    (GenerateDbContextUC.DataContext as GenerateDbContextViewModel).DoGenerateDbContext(Dte, TextTemplating, (T4Editor.DataContext as T4EditorViewModel).T4TempatePath, nameSpace102, (SelectDbContextUC.DataContext as SelectDbContextViewModel).UiCommandProppertyName);
                    this.CurrentUserControl = GenerateDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");

                    break;
                case 103:
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).CollectProjectClasses((SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedProject);
                    CurrentUiStepId = 0;
                    NextBtnCommandAction(param);
                    break;
                case 200:
                    CurrentUiStepId = 201;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    if (CreatePrimKeyUC == null)
                    {
                        CreatePrimaryKeyViewModel dataContext = new CreatePrimaryKeyViewModel(Dte, TextTemplating);
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        dataContext.TemplateFolder = Path.Combine(TemplatesFld, "HasKeyTmplst");
                        CreatePrimKeyUC = new UserControlCreatePrimKey(dataContext);
                    }
                    (CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).SelectedDbContext =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext;
                    (CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).SelectedEntity =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedCodeElement;
                    (CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).DoAnalise();
                    this.CurrentUserControl = CreatePrimKeyUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 201:
                    CurrentUiStepId = 1;
                    NextBtnCommandAction(param);
                    break;

                case 300:
                    CurrentUiStepId = 301;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    if (CreatePrimKeyUC == null)
                    {
                        CreatePrimaryKeyViewModel dataContext = new CreatePrimaryKeyViewModel(Dte, TextTemplating);
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        dataContext.TemplateFolder = Path.Combine(TemplatesFld, "HasKeyTmplst");
                        CreatePrimKeyUC = new UserControlCreatePrimKey(dataContext);
                    }
                    (CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).SelectedDbContext =
                        (SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedDbContext;
                    //(CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).SelectedEntity = 
                        //(SelectSourceEntityUC.DataContext as SelectEntityForGivenDbContextViewModel).SelectedCodeElement;
                    (CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).DoAnalise();
                    this.CurrentUserControl = CreatePrimKeyUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 301:
                    CurrentUiStepId = 4;
                    PrevBtnCommandAction(param);
                    break;


                default:
                    break;
            }
        }
        #endregion
        #region SaveBtnCommand
        public override void SaveBtnCommandAction(Object param)
        {
            switch (CurrentUiStepId)
            {
                case 103:
                    SaveDbContextGeneratedText();
                    break;
                default:
                    break;
            }
        }
        #endregion


        protected string GetDestinationSelItemFolder()
        {
            if (DestinationSelectedItem != null)
            {
                if (DestinationSelectedItem.Project != null)
                {
                    return Path.GetDirectoryName(DestinationSelectedItem.Project.FullName);
                }
            }
            //return Path.Combine(Path.GetDirectoryName(DestinationProject.FullName),  DestinationFoldersChain.Replace(".", "\\"));
            return Path.Combine(Path.GetDirectoryName(DestinationProject.FullName), (InvitationUC.DataContext as InvitationViewModel).DestinationFolder.Replace(".", "\\"));
        }

        private void SaveDbContextGeneratedText()
        {

            string FlNm = Path.Combine(GetDestinationSelItemFolder(), (SelectDbContextUC.DataContext as SelectDbContextViewModel).UiCommandProppertyName 
                + (GenerateDbContextUC.DataContext as GenerateDbContextViewModel).FileExtension);
            File.WriteAllText(FlNm, (GenerateDbContextUC.DataContext as GenerateDbContextViewModel).GenerateText);
            try
            {
                DestinationProject.ProjectItems.AddFromFile(FlNm);
                MessageBox.Show(SuccessNotification, "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SelectDbContextViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

        private void SelectDbContextViewModel_ButtonClicked(Object sender)
        {
            CurrentUiStepId = 100;
            NextBtnCommandAction(sender);
        }
        private void SelectEntityForGivenDbContextViewModel_ButtonClicked(Object sender)
        {
            CurrentUiStepId = 200;
            NextBtnCommandAction(sender);
        }

        private void Selectt4TemplateViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

        private void GenerateDbContext_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
            SaveBtnEnabled = ready;
            OnPropertyChanged("SaveBtnEnabled");
        }

        private void SelectEntityForGivenDbContextViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }
        private void SelectForeignKeyViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }

        private void UiBtnCommandPrincipalKey_ButtonClicked(Object sender)
        {
            if (sender as FluentAPIForeignKey == null)
            {
                MessageBox.Show("Could not proceed. Selection type is not defined", "Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (CreatePrimKeyUC == null)
            {
                CreatePrimaryKeyViewModel dataContext = new CreatePrimaryKeyViewModel(Dte, TextTemplating);
                string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                dataContext.TemplateFolder = Path.Combine(TemplatesFld, "HasKeyTmplst");
                CreatePrimKeyUC = new UserControlCreatePrimKey(dataContext);
            }

            CodeElement codeElement = (sender as FluentAPIForeignKey).CodeElementNavigationRef;
            (CreatePrimKeyUC.DataContext as CreatePrimaryKeyViewModel).SelectedEntity = new SolutionCodeElement()
            {
                CodeElementName = codeElement.Name,
                CodeElementFullName = codeElement.FullName,
                CodeElementRef = codeElement
            };
                
            CurrentUiStepId = 300;
            NextBtnCommandAction(sender);
        }

        private void UiBtnCommandForeignKey_ButtonClicked(Object sender)
        {
            if (sender as FluentAPIForeignKey == null)
            {
                MessageBox.Show("Could not proceed. Selection type is not defined", "Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            CurrentUiStepId = 3;
            NextBtnCommandAction(sender);
        }


    }
}
