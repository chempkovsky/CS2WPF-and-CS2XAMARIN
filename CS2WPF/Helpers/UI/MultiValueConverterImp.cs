using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CS2WPF.Helpers.UI
{
    public class MultiValueConverterImp: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string folder = parameter as string ?? "";
            var folders = folder.Split(',').Select(f => f.Trim()).ToList();
            int cnt = values.Count() / 2;
            while (values.Length*2 > folders.Count) folders.Add(String.Empty);
            List<object> items = new List<object>();
            for (int i = 0; i < values.Length; i++)
            {
                //make sure were working with collections from here...
                IEnumerable childs = values[i] as IEnumerable ?? new List<object> { values[i] };

                string folderName = folders[i*2];
                string checkPropertyName = folders[i * 2 + 1];
                if (folderName != String.Empty)
                {
                    if (!string.IsNullOrEmpty(checkPropertyName))
                    {
                        checkPropertyName = checkPropertyName.Trim();
                    }
                    TreeViewFolderItemBase folderItem = null;
                    if (string.IsNullOrEmpty(checkPropertyName))
                    {
                        folderItem = new TreeViewFolderItem { FolderName = folderName, FolderItems = childs };
                    } else
                    {
                        folderItem = new TreeViewFolderItemWithCheck { FolderName = folderName, CheckPropertyName = checkPropertyName, FolderItems = childs };
                    }
                    
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
