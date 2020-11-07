using CS2ANGULAR.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace CS2ANGULAR.Helpers
{
    public class SerializableViewModelMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string folder = parameter as string ?? "";
            var folders = folder.Split(',').Select(f => f.Trim()).ToList();
            while (values.Length > folders.Count) folders.Add(String.Empty);
            List<object> items = new List<object>();
            for (int i = 0; i < values.Length; i++)
            {
                //make sure were working with collections from here...
                IEnumerable childs = values[i] as IEnumerable ?? new List<object> { values[i] };

                string folderName = folders[i];
                if (folderName != String.Empty)
                {
                    //create folder item and assign childs
                    TreeViewFolderItem folderItem = new TreeViewFolderItem { FolderName = folderName, FolderItems = childs };
                    items.Add(folderItem);
                }
                else
                {
                    //if no folder name was specified, move the item directly to the root item
                    foreach (var child in childs) { items.Add(child); }
                }
            }
            return items;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot perform reverse-conversion");
        }
    }
}
