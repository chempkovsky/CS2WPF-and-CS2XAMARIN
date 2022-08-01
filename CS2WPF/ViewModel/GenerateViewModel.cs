﻿using CS2WPF.Helpers;
using CS2WPF.Model;
using CS2WPF.Model.Serializable;
using CS2WPF.TemplateProcessingHelpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.IO;


namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class GenerateViewModel : BaseGenerateViewModel
    {
        public GenerateViewModel() : base()
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
            //textTemplatingSessionHost.Session["PrismModifier"] = prismModuleModifier;
            if (string.IsNullOrEmpty(GenText))
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, File.ReadAllText(T4TempatePath), tpCallback);
            }
            else
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, GenText, tpCallback);
            }
            FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null)
            {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    this.GenerateError = tpError.ToString() + "\n";
                }
            }
            if (string.IsNullOrEmpty(this.GenerateError))
            {
                if (string.Compare(FileExtension, ".jsonpmm2txt", true) == 0)
                {
                    FileExtension = ".txt";
                    this.GenerateText = prismModuleModifier.ExecuteJsonScript(this.GenerateText);
                }
            }
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }
    }
}
