using CS2WPF.Model.BatchProcess;
using EnvDTE;
using EnvDTE80;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.Collections.Generic;
using System.Linq;
using CS2WPF.Model.Serializable;
using CS2WPF.TemplateProcessingHelpers;
using System.IO;
using System.Text.RegularExpressions;
using CS2WPF.Model;
using System.Collections.ObjectModel;

namespace CS2WPF.Helpers.BatchProcess
{
#pragma warning disable VSTHRD010

    public static class BatchSettingsHelper
    {
        public static BatchSettings ReadBatchSettingsFromString(string jsonString)
        {
            return JsonConvert.DeserializeObject<BatchSettings>(jsonString);
        }
        public static BatchSettings ReadBatchSettingsFromFile(string fileName)
        {
            string jsonString = System.IO.File.ReadAllText(fileName);
            return ReadBatchSettingsFromString(jsonString);
        }
        public static GeneratorBatchStep DoGenerateViewModel(PrismModuleModifier prismModuleModifier, DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, ModelViewSerializable model, string defaultProjectNameSpace = null)
        {
            GeneratorBatchStep result = new GeneratorBatchStep()
            {
                GenerateText = "",
                GenerateError = "",
                FileExtension = "",
                T4TempatePath = T4TempatePath,
            };
            if ((model == null) || (SerializableDbContext == null))
            {
                result.GenerateError = "Model and/or Context is not defined";
                return result;
            }
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            textTemplatingSessionHost.Session["Model"] = model;
            textTemplatingSessionHost.Session["Context"] = SerializableDbContext;
            textTemplatingSessionHost.Session["DefaultProjectNameSpace"] = string.IsNullOrEmpty(defaultProjectNameSpace) ? "" : defaultProjectNameSpace;
            // textTemplatingSessionHost.Session["PrismModifier"] = prismModuleModifier;
            result.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, File.ReadAllText(result.T4TempatePath), tpCallback);
            result.FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null)
            {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    result.GenerateError += tpError.ToString() + "\n";
                }
            }
            if (string.IsNullOrEmpty(result.GenerateError))
            {
                if (string.Compare(result.FileExtension, ".jsonpmm2txt", true) == 0)
                {
                    result.FileExtension = ".txt";
                    result.GenerateText = prismModuleModifier.ExecuteJsonScript(result.GenerateText);
                }
            }
            return result;
        }
        public static string GetHyphenedName(string src)
        {
            string result = "";
            if (string.IsNullOrEmpty(src))
            {
                return result;
            }
            int firstDelim = src.IndexOf('.');
            if (firstDelim > -1)
            {
                result =
                    Regex.Replace(src.Substring(0, firstDelim), @"\B[A-Z]", m => "-" + m.ToString().ToLower()).ToLower() +
                    src.Substring(firstDelim);
            }
            else
            {
                result =
                    Regex.Replace(src, @"\B[A-Z]", m => "-" + m.ToString().ToLower()).ToLower();
            }
            return result;
        }
        public static string TrimPrefix(string srcStr)
        {
            if (string.IsNullOrEmpty(srcStr)) return "";
            int i = srcStr.IndexOf('-');
            if (i > -1)
            {
                return srcStr.Substring(i + 1);
            }
            else
            {
                return srcStr;
            }
        }
        public static ModelViewSerializable GetSelectedModelCommonShallowCopy(ModelViewSerializable SelectedModel,
                                            ObservableCollection<ModelViewUIFormProperty> UIFormProperties,
                                            ObservableCollection<ModelViewUIListProperty> UIListProperties,
                                            string DestinationProject, string DefaultProjectNameSpace, string DestinationFolder, string DestinationSubFolder,
                                            string FileType, string FileName, string T4Template)
        {
            ModelViewSerializable result = SelectedModel.ModelViewSerializableGetShallowCopy();

            if (result.CommonStaffs == null)
            {
                result.CommonStaffs = new List<CommonStaffSerializable>();
            }
            else
            {
                result.CommonStaffs = new List<CommonStaffSerializable>();
                SelectedModel.CommonStaffs.ForEach(c => result.CommonStaffs.Add(new CommonStaffSerializable()
                {
                    Extension = c.Extension,
                    FileType = c.FileType,
                    FileName = c.FileName,
                    FileProject = c.FileProject,
                    FileDefaultProjectNameSpace = c.FileDefaultProjectNameSpace,
                    FileFolder = c.FileFolder,
                    T4Template = c.T4Template,
                    //FileTypeData = c.FileTypeData
                }));
            }
            CommonStaffSerializable commonStaffItem =
                result.CommonStaffs.Where(c => c.FileType == FileType).FirstOrDefault();
            if (commonStaffItem == null)
            {
                result.CommonStaffs.Add(
                    commonStaffItem = new CommonStaffSerializable()
                    {
                        FileType = FileType
                    });
            }
            commonStaffItem.FileName = FileName;
            commonStaffItem.FileProject = DestinationProject;
            commonStaffItem.FileDefaultProjectNameSpace = DefaultProjectNameSpace;
            commonStaffItem.T4Template = T4Template;
            if (string.IsNullOrEmpty(DestinationSubFolder))
            {
                commonStaffItem.FileFolder = DestinationFolder;
            }
            else
            {
                commonStaffItem.FileFolder = Path.Combine(DestinationFolder, DestinationSubFolder);
            }


            if (UIFormProperties != null)
            {
                result.UIFormProperties = new List<ModelViewUIFormPropertySerializable>();
                foreach (ModelViewUIFormProperty srcProp in UIFormProperties)
                {
                    result.UIFormProperties.Add(srcProp.ModelViewUIFormPropertyAssignTo(new ModelViewUIFormPropertySerializable()));
                }
            }
            if (result.UIFormProperties == null)
            {
                result.UIFormProperties = new List<ModelViewUIFormPropertySerializable>();
            }


            if (UIListProperties != null)
            {
                result.UIListProperties = new List<ModelViewUIListPropertySerializable>();
                foreach (ModelViewUIListProperty srcProp in UIListProperties)
                {
                    result.UIListProperties.Add(srcProp.ModelViewUIListPropertyAssignTo(new ModelViewUIListPropertySerializable()));
                }
            }
            if (result.UIListProperties == null)
            {
                result.UIListProperties = new List<ModelViewUIListPropertySerializable>();
            }

            return result;
        }

        public static void UpdateDbContext(DTE2 Dte, Project DestinationProject, SolutionCodeElement SelectedDbContext, DbContextSerializable dbContextSerializable, ModelViewSerializable modelViewSerializable,
                                        string ContextItemViewName, string T4SelectedFolder,
                                        string DestinationProjectRootFolder,
                                        string DestinationFolder,
                                        string DestinationSubFolder,
                                        string FileName, string FileExtension, string T4Template,
                                        string GenerateText)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (modelViewSerializable.ViewName == ContextItemViewName)
            {
                dbContextSerializable.CommonStaffs = modelViewSerializable.CommonStaffs;
                if (dbContextSerializable.CommonStaffs == null)
                {
                    dbContextSerializable.CommonStaffs = new List<CommonStaffSerializable>();
                }
                CommonStaffSerializable commonStaffSerializable = dbContextSerializable.CommonStaffs
                        .Where(c => c.FileType == T4SelectedFolder)
                        .FirstOrDefault();
                if (commonStaffSerializable != null)
                {
                    commonStaffSerializable.Extension = FileExtension;
                    commonStaffSerializable.T4Template = T4Template;
                }

            }
            else
            {
                ModelViewSerializable existedModelViewSerializable =
                    dbContextSerializable.ModelViews.FirstOrDefault(mv => mv.ViewName == modelViewSerializable.ViewName);
                if (modelViewSerializable.CommonStaffs != null)
                {
                    CommonStaffSerializable commonStaffSerializable =
                        modelViewSerializable.CommonStaffs
                        .Where(c => c.FileType == T4SelectedFolder)
                        .FirstOrDefault();
                    if (commonStaffSerializable != null)
                    {
                        commonStaffSerializable.Extension = FileExtension;
                    }
                }
                if (existedModelViewSerializable != null)
                {
                    existedModelViewSerializable.CommonStaffs = modelViewSerializable.CommonStaffs;
                    existedModelViewSerializable.UIFormProperties = modelViewSerializable.UIFormProperties;
                    existedModelViewSerializable.UIListProperties = modelViewSerializable.UIListProperties;

                }
                else
                {
                    dbContextSerializable.ModelViews.Add(modelViewSerializable);
                }
            }
            string projectName = "";
            if (SelectedDbContext.CodeElementRef != null)
            {
                if (SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                       SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            string SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, SelectedDbContext.CodeElementFullName, "json");
                locFileName = locFileName.Replace("\\", ".");
                locFileName = Path.Combine(SolutionDirectory, locFileName);
                string jsonString = JsonConvert.SerializeObject(dbContextSerializable);
                File.WriteAllText(locFileName, jsonString);
            }
            string FlNm = "";
            if (string.IsNullOrEmpty(DestinationSubFolder))
            {
                FlNm = Path.Combine(
                DestinationProjectRootFolder,
                DestinationFolder);
            }
            else
            {
                FlNm = Path.Combine(
                DestinationProjectRootFolder,
                DestinationFolder,
                DestinationSubFolder);
            }
            System.IO.Directory.CreateDirectory(FlNm);
            string lflnm = FileName;
            if ( 
                (string.Compare("php",FileExtension, System.StringComparison.OrdinalIgnoreCase) == 0 ) ||
                (string.Compare(".php", FileExtension, System.StringComparison.OrdinalIgnoreCase) == 0)
               )
            {
                lflnm = lflnm.FirstLetterToUpper();
            }
            FlNm = Path.Combine(FlNm, lflnm + FileExtension);
            File.WriteAllText(FlNm, GenerateText);
            DestinationProject.ProjectItems.AddFromFile(FlNm);
        }

    }
}
