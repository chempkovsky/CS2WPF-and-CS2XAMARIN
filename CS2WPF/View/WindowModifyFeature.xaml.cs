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
    public partial class WindowModifyFeature : Window
    {
        //BackgroundWorker Worker;
        public WindowModifyFeature(ModifyFeatureViewModel context)
        {
            InitializeComponent();
            SetDataContext(context);
        }
        public void SetDataContext(ModifyFeatureViewModel dataContext)
        {
            if (dataContext != null)
            {
                dataContext.wnd = this;
                //dataContext.UiCommandButtonClicked.ButtonClickedEvent += UiCommandButtonClicked;
            }
            this.DataContext = dataContext;
        }
        //protected void UiCommandButtonClicked(Object sender)
        //{
        //    if (this.DataContext != null)
        //    {
        //        (this.DataContext as ModifyFeatureViewModel).UiCommandButtonClicked.ButtonClickedEvent -= UiCommandButtonClicked;
        //    }
        //    this.DialogResult = true;
        //}
    }
}
