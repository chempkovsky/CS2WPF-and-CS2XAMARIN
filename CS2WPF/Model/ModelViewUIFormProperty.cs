using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class ModelViewUIFormProperty : ModelViewUIBaseProperty
    {
        #region Fields
        protected InputTypeEnum _InputTypeWhenAdd;
        protected InputTypeEnum _InputTypeWhenUpdate;
        protected InputTypeEnum _InputTypeWhenDelete;
        protected string _ForeifKeyViewNameForAdd;
        protected string _ForeifKeyViewNameForUpd;
        protected string _ForeifKeyViewNameForDel;
        #endregion

        public InputTypeEnum InputTypeWhenAdd
        {
            get
            {
                return _InputTypeWhenAdd;
            }
            set
            {
                if (_InputTypeWhenAdd != value)
                {
                    _InputTypeWhenAdd = value;
                    OnPropertyChanged();
                }
            }
        }
        public InputTypeEnum InputTypeWhenUpdate
        {
            get
            {
                return _InputTypeWhenUpdate;
            }
            set
            {
                if (_InputTypeWhenUpdate != value)
                {
                    _InputTypeWhenUpdate = value;
                    OnPropertyChanged();
                }
            }
        }
        public InputTypeEnum InputTypeWhenDelete
        {
            get
            {
                return _InputTypeWhenDelete;
            }
            set
            {
                if (_InputTypeWhenDelete != value)
                {
                    _InputTypeWhenDelete = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ForeifKeyViewNameForAdd
        {
            get
            {
                return _ForeifKeyViewNameForAdd;
            }
            set
            {
                if (_ForeifKeyViewNameForAdd != value)
                {
                    _ForeifKeyViewNameForAdd = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ForeifKeyViewNameForUpd
        {
            get
            {
                return _ForeifKeyViewNameForUpd;
            }
            set
            {
                if (_ForeifKeyViewNameForUpd != value)
                {
                    _ForeifKeyViewNameForUpd = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ForeifKeyViewNameForDel
        {
            get
            {
                return _ForeifKeyViewNameForDel;
            }
            set
            {
                if (_ForeifKeyViewNameForDel != value)
                {
                    _ForeifKeyViewNameForDel = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
