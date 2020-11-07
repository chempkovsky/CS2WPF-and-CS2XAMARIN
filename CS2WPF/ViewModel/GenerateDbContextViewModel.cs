using CS2WPF.TemplateProcessingHelpers;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.ViewModel
{
    public class GenerateDbContextViewModel : BaseGenerateViewModel
    {
        public GenerateDbContextViewModel() : base()
        {

        }
        public void DoGenerateDbContext(DTE2 Dte, ITextTemplating textTemplating, string templatePath, string DestinationNameSpace, string DestinationClassName)
        {
            this.GenerateText = "";
            this.GenerateError = "";
            OnPropertyChanged("GenerateText");
            OnPropertyChanged("GenerateError");
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            textTemplatingSessionHost.Session["DestinationNameSpace"] = DestinationNameSpace;
            textTemplatingSessionHost.Session["DestinationClassName"] = DestinationClassName;

            if (string.IsNullOrEmpty(GenText))
            {
                this.GenerateText = textTemplating.ProcessTemplate(templatePath, File.ReadAllText(templatePath), tpCallback);
            }
            else
            {
                this.GenerateText = textTemplating.ProcessTemplate(templatePath, GenText, tpCallback);
            }
            FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null)
            {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    this.GenerateError = tpError.ToString() + "\n";
                }
            }
            OnPropertyChanged("GenerateText");
            OnPropertyChanged("GenerateError");
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }
    }
}
