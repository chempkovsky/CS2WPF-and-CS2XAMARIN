using System;
using System.Windows.Controls;

namespace CS2WPF.View
{
    /// <summary>
    /// Логика взаимодействия для UserControlInvitation.xaml
    /// </summary>
    public partial class UserControlInvitation : UserControl
    {
        public UserControlInvitation(Object dataContext)
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
