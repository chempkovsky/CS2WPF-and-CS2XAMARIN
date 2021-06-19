using CS2WPF.Helpers;
using CS2WPF.Model;
using CS2WPF.Model.Serializable;
using CS2WPF.TemplateProcessingHelpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace CS2WPF.ViewModel
{
    public class GenerateViewModel : BaseGenerateViewModel
    {
        public GenerateViewModel(): base()
        {
            
        }
        public void DoGenerateViewModel(PrismModuleModifier prismModuleModifier, DTE2 Dte, ITextTemplating textTemplating, SelectedItem DestinationSelectedItem, string T4TempatePath, ModelView modelView)
        {
            this.GenerateText = "";
            this.GenerateError = "";


            GeneratedModelView = new ModelViewSerializable();
            modelView.ModelViewAssingTo(GeneratedModelView);


            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            textTemplatingSessionHost.Session["Model"] = GeneratedModelView;
            textTemplatingSessionHost.Session["PrismModifier"] = prismModuleModifier;
            if (string.IsNullOrEmpty(GenText))
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, File.ReadAllText(T4TempatePath), tpCallback);
            }
            else
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, GenText, tpCallback);
            }
            FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null) {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    this.GenerateError = tpError.ToString() + "\n";
                }
            }
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }
    }
}
