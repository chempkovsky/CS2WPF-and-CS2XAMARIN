using CS2WPF.Helpers.UI;
using CS2WPF.Model;
using CS2WPF.Model.Serializable;
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
    public class ModifyFeatureViewModel : NotifyPropertyChangedViewModel
    {
        public Window wnd;
        DbContextSerializable _DbContext;
        AllowedFileTypesSerializable _AllowedFileTypes;
        FeatureSerializable _Feature;
        string _FeatureName;
        public ButtonClickedNotificationService UiCommandButtonClicked = new ButtonClickedNotificationService();
        public ObservableCollection<FeatureItem> ContextItemsList { get; set; }
        public DbContextSerializable DbContext { 
            get
            {
                return _DbContext;
            } 
            set
            {
                if (_DbContext != value)
                {
                    _DbContext = value;
                    OnPropertyChanged();
                    this.OnDbContext();
                }
            }
        }
        public AllowedFileTypesSerializable AllowedFileTypes
        { 
            get
            {
                return _AllowedFileTypes;
            }
            set
            {
                if (_AllowedFileTypes != value)
                {
                    _AllowedFileTypes = value;
                    OnPropertyChanged();
                    this.OnDbContext();
                }
            }
        }
        public string FeatureName
        {
            get
            {
                return _FeatureName;
            }
            set
            {
                if (_FeatureName != value)
                {
                    _FeatureName = value;
                    OnPropertyChanged();
                }
            }
        }
        public FeatureSerializable Feature
        {
            get
            {
                return _Feature;
            }
            set
            {
                if (_Feature != value)
                {
                    _Feature = value;
                    if(_Feature == null)
                    {
                        _FeatureName = "";
                    }
                    else
                    {
                        _FeatureName = _Feature.FeatureName;
                    }
                    OnPropertyChanged();
                    this.OnFeature();
                }
            }
        }
        protected void OnDbContext()
        {
            if (this.ContextItemsList == null) ContextItemsList = new ObservableCollection<FeatureItem>();
            this.ContextItemsList.Clear();
            if (DbContext == null) return;
            if (AllowedFileTypes == null) return;
            if (AllowedFileTypes.Items == null) return;
            if (DbContext.ModelViews == null) return;
            foreach(ModelViewSerializable mv in DbContext.ModelViews)
            {
                if (mv.CommonStaffs == null) continue;
                foreach(CommonStaffSerializable cmstf in mv.CommonStaffs)
                {
                    if (AllowedFileTypes.Items.Any(i => i.FileType == cmstf.FileType)) {
                        ContextItemsList.Add(new FeatureItem() {
                                ViewName = mv.ViewName,
                                FileType = cmstf.FileType,
                                IsSelected = false
                        });
                    }
                }
            }
        }
        protected void OnFeature()
        {
            if (ContextItemsList == null) return;
            foreach(FeatureItem fi in ContextItemsList)
            {
                fi.IsSelected = false;
            }
            if (Feature == null) return;
            if (Feature.FeatureItems == null) return;
            if (Feature.FeatureItems.Count < 1) return;
            foreach (FeatureItem fi in ContextItemsList)
            {
                fi.IsSelected = Feature.FeatureItems.Any(i => (i.ViewName == fi.ViewName && i.FileType == fi.FileType));
            }
        }

        #region UiBtnCommandSave
        private ICommand _UiBtnCommandSave;
        public ICommand UiBtnCommandSave
        {
            get
            {
                return _UiBtnCommandSave ?? (_UiBtnCommandSave = new CommandHandler((param) => UiBtnCommandSaveAction(param), (param) => UiBtnCommandSaveCanExecute(param)));
            }
        }
        public bool UiBtnCommandSaveCanExecute(Object param)
        {
            return (Feature != null) && (!string.IsNullOrEmpty(FeatureName));

        }
        public virtual void UiBtnCommandSaveAction(Object param)
        {
            if (_Feature == null) return;
            if(ContextItemsList == null) return;
            if (ContextItemsList.Count < 1) return;
            if (_Feature.FeatureItems == null) _Feature.FeatureItems = new List<FeatureItemSerializable>();
            _Feature.FeatureName = FeatureName;
            _Feature.FeatureItems.Clear();
            foreach(FeatureItem fi in ContextItemsList)
            {
                if(fi.IsSelected)
                {
                    _Feature.FeatureItems.Add(new FeatureItemSerializable()
                    {
                        ViewName = fi.ViewName,
                        FileType = fi.FileType
                    });
                }
            }
            // UiCommandButtonClicked.DoNotify(this);
            if(wnd != null)
            {
                wnd.DialogResult = true;
            }
        }
        #endregion

    }
}
