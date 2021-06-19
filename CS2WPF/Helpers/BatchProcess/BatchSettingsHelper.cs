using CS2WPF.Model.BatchProcess;
using EnvDTE;
using EnvDTE80;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS2WPF.Model.Serializable;
using CS2WPF.TemplateProcessingHelpers;
using System.IO;
using System.Text.RegularExpressions;
using CS2WPF.Model;
using System.Collections.ObjectModel;
using CS2WPF.Model.Serializable.UI;

namespace CS2WPF.Helpers.BatchProcess
{
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
            if ((model == null) || (SerializableDbContext == null)) {
                result.GenerateError = "Model and/or Context is not defined";
                return result;
            }
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            textTemplatingSessionHost.Session["Model"] = model;
            textTemplatingSessionHost.Session["Context"] = SerializableDbContext;
            textTemplatingSessionHost.Session["DefaultProjectNameSpace"] = string.IsNullOrEmpty(defaultProjectNameSpace) ? "" : defaultProjectNameSpace;
            textTemplatingSessionHost.Session["PrismModifier"] = prismModuleModifier;
            result.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, File.ReadAllText(result.T4TempatePath), tpCallback);
            result.FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null)
            {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    result.GenerateError += tpError.ToString() + "\n";
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
            } else
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
                                            string FileType, string FileName)
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
            if (string.IsNullOrEmpty(DestinationSubFolder))
            {
                commonStaffItem.FileFolder = DestinationFolder;
            } else
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
            if(result.UIFormProperties == null)
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
                                        string FileName, string FileExtension,
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
            } else
            {
                FlNm = Path.Combine(
                DestinationProjectRootFolder,
                DestinationFolder,
                DestinationSubFolder);
            }
            System.IO.Directory.CreateDirectory(FlNm);
            FlNm = Path.Combine(FlNm, FileName + FileExtension);
            File.WriteAllText(FlNm, GenerateText);
            DestinationProject.ProjectItems.AddFromFile(FlNm);
        }

    }
}
