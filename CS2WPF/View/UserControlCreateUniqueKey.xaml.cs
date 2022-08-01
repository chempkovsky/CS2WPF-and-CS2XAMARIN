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
    /// Interaction logic for UserControlCreateUniqueKey.xaml
    /// </summary>
    public partial class UserControlCreateUniqueKey : UserControl
    {
        public UserControlCreateUniqueKey(CreateUniqueKeyViewModel dataContext)
        {
            InitializeComponent();
            SetDataContext(dataContext);
        }
        public void SetDataContext(Object dataContext)
        {
            this.DataContext = dataContext;

        }
    }
}
