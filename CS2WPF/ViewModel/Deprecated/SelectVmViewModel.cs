using CS2ANGULAR.Helpers.UI;
using CS2ANGULAR.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CS2ANGULAR.ViewModel
{
    public class SelectVmViewModel : IsReadyViewModel
    {
        public List<SerializableViewModel> ViewModelList { get; set; }

        Object _selectedTreeViewItem = null;
        public Object SelectedTreeViewItem 
        { 
            get
            {
                return _selectedTreeViewItem;
            }
            set
            {
                _selectedTreeViewItem = value;
                OnPropertyChanged("SelectedTreeViewItem");
                OnSelectedItemChanged();
            }
        }

        public void VisibilityChangedNotification()
        {
            OnPropertyChanged("HintVisibility");
            OnPropertyChanged("SerializableViewModelVisibility");
            OnPropertyChanged("SerializableViewModelPropertyVisibility");
            OnPropertyChanged("SerializableViewModelForeignKeyVisibility");
        }
        public void OnSelectedItemChanged()
        {
            bool ready = SelectedTreeViewItem != null;
            if (ready)
            {
                ready = SelectedTreeViewItem is SerializableViewModel;
            } else
            {
                SerializableViewModelVisibility = Visibility.Collapsed;
                SerializableViewModelPropertyVisibility = Visibility.Collapsed;
                SerializableViewModelForeignKeyVisibility = Visibility.Collapsed;
                HintVisibility = Visibility.Visible;
                VisibilityChangedNotification();
            }
            IsReady.DoNotify(this, ready);
            if (SelectedTreeViewItem == null) return;
            if (ready)
            {
                HintVisibility = Visibility.Collapsed;
                SerializableViewModelPropertyVisibility = Visibility.Collapsed;
                SerializableViewModelForeignKeyVisibility = Visibility.Collapsed;
                SerializableViewModelVisibility = Visibility.Visible;
            } 
            else 
            { 
                if (SelectedTreeViewItem is SerializableViewModelProperty)
                {
                    HintVisibility = Visibility.Collapsed;
                    SerializableViewModelVisibility = Visibility.Collapsed;
                    SerializableViewModelForeignKeyVisibility = Visibility.Collapsed;
                    SerializableViewModelPropertyVisibility = Visibility.Visible;
                } else
                {
                    if (SelectedTreeViewItem is SerializableViewModelForeignKey)
                    {
                        HintVisibility = Visibility.Collapsed;
                        SerializableViewModelVisibility = Visibility.Collapsed;
                        SerializableViewModelPropertyVisibility = Visibility.Collapsed;
                        SerializableViewModelForeignKeyVisibility = Visibility.Visible;
                    } else
                    {
                        SerializableViewModelVisibility = Visibility.Collapsed;
                        SerializableViewModelPropertyVisibility = Visibility.Collapsed;
                        SerializableViewModelForeignKeyVisibility = Visibility.Collapsed;
                        HintVisibility = Visibility.Visible;
                    }
                }
            }
            VisibilityChangedNotification();
        }
        public SelectVmViewModel(List<SerializableViewModel> viewModelList):base()
        {
            ViewModelList = viewModelList;
        }
        public Visibility HintVisibility { get; set; } = Visibility.Visible;
        public Visibility SerializableViewModelVisibility { get; set; } = Visibility.Collapsed;
        public Visibility SerializableViewModelPropertyVisibility { get; set; } = Visibility.Collapsed;
        public Visibility SerializableViewModelForeignKeyVisibility { get; set; } = Visibility.Collapsed;
    }
}
