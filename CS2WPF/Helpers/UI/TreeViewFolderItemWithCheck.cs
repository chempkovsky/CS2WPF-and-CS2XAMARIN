using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers.UI
{
    public class TreeViewFolderItemWithCheck: TreeViewFolderItemBase
    {
        protected bool _IsChecked = false;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked == value) return;
                _IsChecked = value;
                if (!string.IsNullOrEmpty(CheckPropertyName))
                {
                    if (FolderItems != null)
                    {
                        foreach (Object itm in FolderItems)
                        {
                            PropertyInfo myPropInfo = itm.GetType().GetProperty(CheckPropertyName);
                            if (myPropInfo != null)
                            {
                                myPropInfo.SetValue(itm, _IsChecked);
                            }
                        }
                    }
                }
            }
        }
        public string CheckPropertyName { get; set; }
    }
}
