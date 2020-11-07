using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers.UI
{
    public class TreeViewFolderItemBase: NotifyPropertyChangedViewModel
    {
        public string FolderName { get; set; }
        public IEnumerable FolderItems { get; set; }
    }
}
