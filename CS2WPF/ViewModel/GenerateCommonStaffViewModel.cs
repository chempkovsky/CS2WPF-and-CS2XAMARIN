﻿using CS2WPF.Helpers;
using CS2WPF.Model.Serializable;
using CS2WPF.TemplateProcessingHelpers;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.IO;

namespace CS2WPF.ViewModel
{
#pragma warning disable VSTHRD010
    public class GenerateCommonStaffViewModel : BaseGenerateViewModel
    {
        public GenerateCommonStaffViewModel() : base()
        {

        }



        public void DoGenerateViewModel(PrismModuleModifier prismModuleModifier, DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, ModelViewSerializable model, string defaultProjectNameSpace = null)
        {

            this.GenerateText = "";
            this.GenerateError = "";
            IsReady.DoNotify(this, false);
            if ((model == null) || (SerializableDbContext == null)) return;
            GeneratedModelView = model;

            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();

            textTemplatingSessionHost.Session["Model"] = GeneratedModelView;
            textTemplatingSessionHost.Session["Context"] = SerializableDbContext;
            textTemplatingSessionHost.Session["DefaultProjectNameSpace"] = string.IsNullOrEmpty(defaultProjectNameSpace) ? "" : defaultProjectNameSpace;
            // textTemplatingSessionHost.Session["PrismModifier"] = prismModuleModifier;
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
                    this.GenerateError += tpError.ToString() + "\n";
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

        public void DoGenerateFeature(PrismModuleModifier prismModuleModifier, DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, FeatureContextSerializable SerializableFeatureContext, FeatureSerializable feature, AllowedFileTypesSerializable AllowedFileTypes, string defaultProjectNameSpace = null)
        {

            this.GenerateText = "";
            this.GenerateError = "";
            IsReady.DoNotify(this, false);
            if ((feature == null) || (SerializableDbContext == null) || (SerializableFeatureContext == null)) return;
            GeneratedFeature = feature;

            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();

            textTemplatingSessionHost.Session["AllowedFileTypes"] = AllowedFileTypes;
            textTemplatingSessionHost.Session["Feature"] = GeneratedFeature;
            textTemplatingSessionHost.Session["FeatureContext"] = SerializableFeatureContext;
            textTemplatingSessionHost.Session["Context"] = SerializableDbContext;
            textTemplatingSessionHost.Session["DefaultProjectNameSpace"] = string.IsNullOrEmpty(defaultProjectNameSpace) ? "" : defaultProjectNameSpace;
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
                    this.GenerateError += tpError.ToString() + "\n";
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
