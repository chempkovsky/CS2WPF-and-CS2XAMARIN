using CS2WPF.Helpers.UI;
using System.Collections.ObjectModel;
using System.IO;

namespace CS2WPF.ViewModel
{
    public class T4EditorViewModel: IsReadyViewModel
    {
        #region Fields
        string _T4TempateCaption = "T4 Tempate";
        string _T4TempateText;
        string _T4TempatePath;
        string _T4TemplateFolder;
        string _T4SelectedTemplate;
        string _T4TemplateExtention= "*.t4";
        #endregion

        public T4EditorViewModel(string templateFolder) : base()
        {
            T4TemplateFolder = templateFolder;
        }
        public ObservableCollection<string> Templates { get; set; }
        public string T4TemplateFolder
        {
            get
            {
                return _T4TemplateFolder;
            }
            set
            {
                if (_T4TemplateFolder == value) return;
                _T4TemplateFolder = value;
                if (Templates == null) Templates = new ObservableCollection<string>();
                Templates.Clear();
                T4SelectedTemplate = "";
                OnPropertyChanged();
                OnPropertyChanged("Templates");
                string[] files = Directory.GetFiles(_T4TemplateFolder, T4TemplateExtention);
                if (files != null)
                {
                    foreach (string f in files)
                    {
                        Templates.Add(Path.GetFileName(f));
                    }
                }
            }
        }
        public string T4SelectedTemplate
        {
            get
            {
                return _T4SelectedTemplate;
            }
            set
            {
                if (_T4SelectedTemplate == value) return;
                _T4SelectedTemplate = value;
                OnPropertyChanged();
                OnT4SelectedTemplateChanged();
            }
        }
        public string T4TempatePath
        {
            get
            {
                return _T4TempatePath;
            }
            set
            {
                _T4TempatePath = value;
                OnPropertyChanged();
            }
        }
        public string T4TempateCaption 
        {
            get { 
                return _T4TempateCaption; 
            }
            set {
                _T4TempateCaption = value;
                OnPropertyChanged();
            } 
        }
        public string T4TempateText
        {
            get
            {
                return _T4TempateText;
            }
            set
            {
                _T4TempateText = value;
                OnPropertyChanged();
            }
        }
        public string T4TemplateExtention
        {
            get
            {
                return _T4TemplateExtention;
            }
            set
            {
                _T4TemplateExtention = value;
                OnPropertyChanged();
            }
        }
        public void OnT4SelectedTemplateChanged()
        {
            T4TempateText = "";
            if (!string.IsNullOrEmpty(T4SelectedTemplate))
            {
                T4TempatePath = Path.Combine(T4TemplateFolder, T4SelectedTemplate);
                ReadTemplate();
            }
            CheckIsReady();
        }
        public void CheckIsReady()
        {
            IsReady.DoNotify(this, (!string.IsNullOrEmpty(this.T4TempateText)) && (!string.IsNullOrEmpty(T4TempatePath))  );
        }
        public void ReadTemplate()
        {
            T4TempateText = File.ReadAllText(T4TempatePath);
        }
    }
}
