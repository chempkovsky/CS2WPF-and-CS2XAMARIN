using CS2WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CS2WPF.View
{
    /// <summary>
    /// Логика взаимодействия для UserControlCreateView.xaml
    /// </summary>
    public partial class UserControlCreateView : UserControl
    {
        public UserControlCreateView(CreateViewViewModel dataContext)
        {
            InitializeComponent();
            SetDataContext(dataContext);
        }
        public void SetDataContext(Object dataContext)
        {
            if (dataContext is CreateViewViewModel)
            {
                (dataContext as CreateViewViewModel).MainTreeViewRootItem = this.MainTreeViewRootItem;
            }
            this.DataContext = dataContext;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is CreateViewViewModel)
            {
                (this.DataContext as CreateViewViewModel).SelectedTreeViewItem = e.NewValue;
            }
        }

    }
}
