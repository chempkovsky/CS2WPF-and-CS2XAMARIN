using CS2WPF.Helpers.UI;
using System.Collections.ObjectModel;
using System.IO;

namespace CS2WPF.ViewModel
{
    public class Selectt4TemplateViewModel : IsReadyViewModel
    {
        protected string _templateFolder = "";

        public string TemplateExtention { get; set; }
        public string TemplateFolder 
        { 
            get
            {
                return _templateFolder;
            }
            set
            {
                if (_templateFolder == value) return;
                _templateFolder = value;
                if (Templates == null) Templates = new ObservableCollection<string>();
                Templates.Clear();
                SelectedTemplate = "";
                OnPropertyChanged("TemplateFolder");
                OnPropertyChanged("Templates");
            } 
        }

        protected string _selectedTemplate = "";
        public string SelectedTemplate
        {
            get
            {
                return _selectedTemplate;
            }
            set
            {
                if (_selectedTemplate == value) return;
                _selectedTemplate = value;
                OnPropertyChanged("SelectedTemplate");
                CheckIsReady();
            }
        }
        public bool ShowT4Template { get; set; }
        public string ControlCaption { get; set; }
        public ObservableCollection<string> Templates { get; set; }
        public Selectt4TemplateViewModel(): base()
        {
            Templates = new ObservableCollection<string>();
            ControlCaption = "Template Selection";
            TemplateExtention = "*.t4";
            ShowT4Template = true;
        }
        public void DoAnalise()
        {
            if (Templates == null) Templates = new ObservableCollection<string>();
            if (Templates.Count < 1)
            {
                Templates.Clear();
                SelectedTemplate = "";
                string[] files = Directory.GetFiles(TemplateFolder, TemplateExtention);
                if (files != null)
                {
                    foreach (string f in files)
                    {
                        Templates.Add(Path.GetFileName(f));
                    }
                }
            }
            CheckIsReady();
        }
        public virtual void CheckIsReady()
        {
            IsReady.DoNotify(this, !string.IsNullOrEmpty(SelectedTemplate));
        }

    }
}
