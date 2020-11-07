using System.Collections;

namespace CS2ANGULAR.Model
{
    public class TreeViewFolderItem
    {
        public string FolderName { get; set; }
        public IEnumerable FolderItems { get; set; }
    }
}
