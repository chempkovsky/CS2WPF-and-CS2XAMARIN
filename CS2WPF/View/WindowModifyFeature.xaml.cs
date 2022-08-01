using CS2WPF.ViewModel;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Windows;

namespace CS2WPF.View
{
    /// <summary>
    /// Interaction logic for WindowModifyFeature.xaml
    /// </summary>
    public partial class WindowModifyFeature : Window
    {
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
            }
            this.DataContext = dataContext;
        }
    }
}
