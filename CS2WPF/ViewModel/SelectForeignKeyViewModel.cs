using CS2WPF.Helpers;
using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.AnalyzeOnModelCreating;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CS2WPF.ViewModel
{
    #pragma warning disable VSTHRD010
    public class SelectForeignKeyViewModel : IsReadyViewModel
    {
        #region Fields
        protected DTE2 Dte;
        protected SolutionCodeElement _SelectedDbContext;
        protected SolutionCodeElement _SelectedEntity;
        protected FluentAPIForeignKey _SelectedForeignKey;
        protected string _SelectedEntityCaption;
        #endregion


        public SelectForeignKeyViewModel(DTE2 dte) : base()
        {
            this.Dte = dte;
            ForeignKeys = new ObservableCollection<FluentAPIForeignKey>();
            PrincipalKeyButtonClicked = new ButtonClickedNotificationService();
            ForeignKeyButtonClicked = new ButtonClickedNotificationService();
        }

        public ButtonClickedNotificationService PrincipalKeyButtonClicked;
        public ButtonClickedNotificationService ForeignKeyButtonClicked;
        public SolutionCodeElement SelectedDbContext
        {
            get { return _SelectedDbContext; }
            set
            {
                if (_SelectedDbContext == value) return;
                _SelectedDbContext = value;
                OnPropertyChanged();
                OnSelectedDbContextChanged();
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
        public string SelectForeignCaption { get; set; } = "Foreign Keys for:";
        public string SelectedEntityCaption 
        { 
            get
            {
                return _SelectedEntityCaption;
            }
            set
            {
                if (_SelectedEntityCaption == value) return;
                _SelectedEntityCaption = value;
                OnPropertyChanged();
            } 
        }
        public ObservableCollection<FluentAPIForeignKey> ForeignKeys { get; set; }
        public FluentAPIForeignKey SelectedForeignKey 
        { 
            get { 
                return _SelectedForeignKey;
            } 
            set
            {
                if (_SelectedForeignKey == value) return;
                _SelectedForeignKey = value;
                CheckIsReady();
                OnPropertyChanged();
            }
        }
        public void OnSelectedDbContextChanged()
        {
            SelectedForeignKey = null;
            ForeignKeys.Clear();
        }
        public void OnSelectedEntityChanged()
        {
            SelectedForeignKey = null;
            ForeignKeys.Clear();
            if (SelectedEntity == null)
            {
                SelectedEntityCaption = "";
            } else
            {
                SelectedEntityCaption = SelectedEntity.CodeElementFullName;
            }
        }
        public void DoAnalise()
        {
            if (ForeignKeys.Count > 0)
            {
                CheckIsReady();
                return;
            }
            if ((SelectedEntity == null) || (SelectedDbContext == null))
            {
                SelectedForeignKey = null;
                ForeignKeys.Clear();
                CheckIsReady();
                return;
            }
            DoAnaliseEx();
        }
        public void DoAnaliseEx()
        {
            SelectedForeignKey = null;
            ForeignKeys.Clear();
            if ((SelectedEntity == null) || (SelectedDbContext == null))
            {
                CheckIsReady();
                return;
            }

            List<FluentAPIForeignKey> result =
                (SelectedEntity.CodeElementRef as CodeClass).CollectForeignKeys(SelectedDbContext.CodeElementRef as CodeClass);
            if (result != null)
            {
                foreach (FluentAPIForeignKey itm in result)
                {
                    ForeignKeys.Add(itm);
                }
            }
            CheckIsReady();
        }


        #region UiBtnCommandPrincipalKey
        private ICommand _UiBtnCommandPrincipalKey;
        public ICommand UiBtnCommandPrincipalKey
        {
            get
            {
                return _UiBtnCommandPrincipalKey ?? (_UiBtnCommandPrincipalKey = new CommandHandler((param) => UiBtnCommandPrincipalKeyAction(param), (param) => UiBtnCommandPrincipalKeyCanExecute(param)));
            }
        }
        public bool UiBtnCommandPrincipalKeyCanExecute(Object param)
        {
            return true;
        }
        public virtual void UiBtnCommandPrincipalKeyAction(Object param)
        {
            PrincipalKeyButtonClicked.DoNotify(param);
        }
        #endregion

        #region UiBtnCommandPrincipalKey
        private ICommand _UiBtnCommandForeignKey;
        public ICommand UiBtnCommandForeignKey
        {
            get
            {
                return _UiBtnCommandForeignKey ?? (_UiBtnCommandForeignKey = new CommandHandler((param) => UiBtnCommandForeignKeyAction(param), (param) => UiBtnCommandForeignKeyCanExecute(param)));
            }
        }
        public bool UiBtnCommandForeignKeyCanExecute(Object param)
        {
            return true;
        }
        public virtual void UiBtnCommandForeignKeyAction(Object param)
        {
            SelectedForeignKey = param as FluentAPIForeignKey;
            ForeignKeyButtonClicked.DoNotify(param);
        }
        #endregion

        public void CheckIsReady()
        {
            IsReady.DoNotify(this, (SelectedDbContext != null) && (SelectedEntity != null) && (SelectedForeignKey != null));
        }

    }
}
