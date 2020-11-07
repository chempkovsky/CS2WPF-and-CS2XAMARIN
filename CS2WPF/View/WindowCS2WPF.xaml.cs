using CS2WPF.ViewModel;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Windows;

namespace CS2WPF.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WindowCS2WPF : Window
    {

        //BackgroundWorker Worker;
        public WindowCS2WPF(MainWindowBase dataContext)
        {
            
            InitializeComponent();
            this.SetDataContext(dataContext);
        }
        public void SetDataContext(MainWindowBase dataContext)
        {
            if(dataContext != null)
            {
                dataContext.CancelClicked.ButtonClickedEvent += CS2ANGLARMainWindowViewModel_CancelClicked;
            }
            this.DataContext = dataContext;
        }

        private void CS2ANGLARMainWindowViewModel_CancelClicked(Object sender)
        {
                this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = MessageBox.Show("Do you want to exit ?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No;
        }
    }
}
