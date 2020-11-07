using CS2ANGULAR.Helpers;
using CS2ANGULAR.Helpers.UI;
using CS2ANGULAR.Model;

namespace CS2ANGULAR.ViewModel
{
    public class WebApiMethodSelectorViewModel : IsReadyViewModel
    {
        #region Fields
        protected SerializableViewModel _selectedViewModel = null;
        protected string WebApiSufixStr = "WebApiController";
        protected string _DestinationWebApiProjectName;
        protected string _DestinationWebApiNamespace;
        protected string _DestinationWebApiServiceName;
        protected bool _IsWebApiSelectAllMethod = false;
        protected bool _IsWebApiSelectManyWithPaginationMethod = false;
        protected bool _IsWebApiSelectOneByPrimarykeyMethod = false;
        protected bool _IsWebApiAddItemMethod = false;
        protected bool _IsWebApiUpdateItemMethod = false;
        protected bool _IsWebApiDeleteItemMethod = false;
        protected bool _IsWebApiHasPrimaryKeyProperties = false;
        protected bool _IsWebApiHasProperties = false;
        protected bool _IsHasAllRequiredProperties = false;
        #endregion


        public WebApiMethodSelectorViewModel(): base()
        {
        }


        public string DestinationWebApiProjectName
        {
            get
            {
                return _DestinationWebApiProjectName;
            }
            set
            {
                _DestinationWebApiProjectName = value;
                OnPropertyChanged("DestinationWebApiProjectName");
            }
        }
        public string DestinationWebApiNamespace
        {
            get
            {
                return _DestinationWebApiNamespace;
            }
            set
            {
                _DestinationWebApiNamespace = value;
                OnPropertyChanged("DestinationWebApiNamespace");
            }
        }
        public string DestinationWebApiServiceName
        {
            get
            {
                return _DestinationWebApiServiceName;
            }
            set
            {
                _DestinationWebApiServiceName = value;
                OnPropertyChanged("DestinationWebApiServiceName");
                CheckIsReady();
            }
        }





        public bool IsWebApiSelectAllMethod
        {
            get
            {
                return _IsWebApiSelectAllMethod;
            }
            set
            {
                _IsWebApiSelectAllMethod = value;
                OnPropertyChanged("IsWebApiSelectAllMethod");
                CheckIsReady();
            }
        }
        public bool IsWebApiSelectManyWithPaginationMethod
        {
            get
            {
                return _IsWebApiSelectManyWithPaginationMethod;
            }
            set
            {
                _IsWebApiSelectManyWithPaginationMethod = value;
                OnPropertyChanged("IsWebApiSelectManyWithPaginationMethod");
                CheckIsReady();
            }
        }
        public bool IsWebApiSelectOneByPrimarykeyMethod
        {
            get
            {
                return _IsWebApiSelectOneByPrimarykeyMethod;
            }
            set
            {
                _IsWebApiSelectOneByPrimarykeyMethod = value;
                OnPropertyChanged("IsWebApiSelectOneByPrimarykeyMethod");
                CheckIsReady();
            }
        }
        public bool IsWebApiAddItemMethod
        {
            get
            {
                return _IsWebApiAddItemMethod;
            }
            set
            {
                _IsWebApiAddItemMethod = value;
                OnPropertyChanged("IsWebApiAddItemMethod");
                CheckIsReady();
            }
        }
        public bool IsWebApiUpdateItemMethod
        {
            get
            {
                return _IsWebApiUpdateItemMethod;
            }
            set
            {
                _IsWebApiUpdateItemMethod = value;
                OnPropertyChanged("IsWebApiUpdateItemMethod");
                CheckIsReady();
            }
        }
        public bool IsWebApiDeleteItemMethod
        {
            get
            {
                return _IsWebApiDeleteItemMethod;
            }
            set
            {
                _IsWebApiDeleteItemMethod = value;
                OnPropertyChanged("IsWebApiDeleteItemMethod");
                CheckIsReady();
            }
        }
        public bool IsWebApiHasPrimaryKeyProperties
        {
            get
            {
                return _IsWebApiHasPrimaryKeyProperties;
            }
            set
            {
                _IsWebApiHasPrimaryKeyProperties = value;
                OnPropertyChanged("IsWebApiHasPrimaryKeyProperties");
                CheckIsReady();
            }
        }
        public bool IsWebApiHasProperties
        {
            get
            {
                return _IsWebApiHasProperties;
            }
            set
            {
                _IsWebApiHasProperties = value;
                OnPropertyChanged("IsWebApiHasProperties");
                CheckIsReady();
            }
        }
        public bool IsHasAllRequiredProperties
        {
            get
            {
                return _IsHasAllRequiredProperties;
            }
            set
            {
                _IsHasAllRequiredProperties = value;
                OnPropertyChanged("IsHasAllRequiredProperties");
                CheckIsReady();
            }
        }
        public SerializableViewModel SelectedViewModel
        {
            get
            {
                return _selectedViewModel;
            }
            set
            {
                if (_selectedViewModel != value)
                {
                    IsWebApiSelectAllMethod = false;
                    IsWebApiSelectManyWithPaginationMethod = false;
                    IsWebApiSelectOneByPrimarykeyMethod = false;
                    IsWebApiAddItemMethod = false;
                    IsWebApiUpdateItemMethod = false;
                    IsWebApiDeleteItemMethod = false;
                    IsWebApiHasPrimaryKeyProperties = false;
                    IsWebApiHasProperties = false;
                    DestinationWebApiServiceName = "";
                }
                _selectedViewModel = value;
                if (_selectedViewModel != null)
                {
                    bool IsFirstTimeGenerated = string.IsNullOrEmpty(_selectedViewModel.DestinationWebApiServiceName);
                    IsHasAllRequiredProperties = _selectedViewModel.HasAllRequiredProperties();
                    IsWebApiHasPrimaryKeyProperties = _selectedViewModel.HasAllPrimaryKeyProperties();
                    IsWebApiHasProperties = _selectedViewModel.HasProperties();
                    if (IsFirstTimeGenerated)
                    {
                        DestinationWebApiServiceName = _selectedViewModel.ViewModelName + WebApiSufixStr;

                        IsWebApiSelectAllMethod = IsWebApiHasProperties;
                        IsWebApiSelectManyWithPaginationMethod = IsWebApiHasProperties;
                        IsWebApiSelectOneByPrimarykeyMethod = IsWebApiHasPrimaryKeyProperties;
                        IsWebApiAddItemMethod = IsWebApiHasPrimaryKeyProperties && IsHasAllRequiredProperties;
                        IsWebApiUpdateItemMethod = IsWebApiHasPrimaryKeyProperties;
                        IsWebApiDeleteItemMethod = IsWebApiHasPrimaryKeyProperties;
                    }
                    else
                    {
                        DestinationWebApiServiceName = _selectedViewModel.DestinationWebApiServiceName;

                        IsWebApiSelectAllMethod = _selectedViewModel.IsWebApiSelectAllMethod && IsWebApiHasProperties;
                        IsWebApiSelectManyWithPaginationMethod = _selectedViewModel.IsWebApiSelectManyWithPaginationMethod && IsWebApiHasProperties;
                        IsWebApiSelectOneByPrimarykeyMethod = _selectedViewModel.IsWebApiSelectOneByPrimarykeyMethod && IsWebApiHasPrimaryKeyProperties;
                        IsWebApiAddItemMethod = _selectedViewModel.IsWebApiAddItemMethod && IsWebApiHasPrimaryKeyProperties && IsHasAllRequiredProperties;
                        IsWebApiUpdateItemMethod = _selectedViewModel.IsWebApiUpdateItemMethod && IsWebApiHasPrimaryKeyProperties;
                        IsWebApiDeleteItemMethod = _selectedViewModel.IsWebApiDeleteItemMethod && IsWebApiHasPrimaryKeyProperties;
                    }
                }
                // OnPropertyChanged("SelectedViewModel");
            }
        }
        public void CheckIsReady()
        {
            bool ready =
            _IsWebApiSelectAllMethod ||
            _IsWebApiSelectManyWithPaginationMethod ||
            _IsWebApiSelectOneByPrimarykeyMethod ||
            _IsWebApiAddItemMethod ||
            _IsWebApiUpdateItemMethod ||
            _IsWebApiDeleteItemMethod;
            if (ready)
            {
                ready = !string.IsNullOrEmpty(DestinationWebApiServiceName);
            }
            IsReady.DoNotify(this, ready);
        }
    }
}
