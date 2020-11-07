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
    public partial class WindowBatch : Window
    {

        //BackgroundWorker Worker;
        public WindowBatch(BatchProcessingViewModel dataContext)
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.SetDataContext(dataContext);
        }
        public void SetDataContext(BatchProcessingViewModel dataContext)
        {
            this.DataContext = dataContext;
        }


        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
