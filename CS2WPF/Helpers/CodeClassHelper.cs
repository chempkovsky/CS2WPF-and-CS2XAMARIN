using CS2WPF.Model;
using CS2WPF.Model.AnalyzeOnModelCreating;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VSLangProj;

namespace CS2WPF.Helpers
{
#pragma warning disable VSTHRD010
    public static class CodeClassHelper
    {
        // System.DateTime ss; //not null (struct)
        // System.DateTimeOffset ss; //not null (struct)
        // System.TimeSpan ss; //not null (struct)
        // System.Guid ss; //not null (struct)
        // Blob ->	System.Byte[] <- System.TimeSpan

        public static bool IsCodePropertyNullable(this CodeProperty codeProperty)
        {
            if (codeProperty == null) return true;
            int absoluteParentOffset = codeProperty.StartPoint.AbsoluteCharOffset;
            int absoluteAttributesOffset = absoluteParentOffset;
            CodeElements attribs = codeProperty.Attributes;
            foreach (CodeElement attrib in attribs)
            {
                int i = attrib.GetEndPoint().AbsoluteCharOffset;
                if (i > absoluteAttributesOffset) absoluteAttributesOffset = i;
            }
            string textToAnalise = codeProperty.StartPoint.CreateEditPoint().GetText(codeProperty.EndPoint)
                .Substring(absoluteAttributesOffset - absoluteParentOffset).Replace("\n", " ").Replace("\r", " ").Replace("\t", " ").Replace("{", " ");
            bool result = textToAnalise.IndexOf("?" + codeProperty.Name + " ") > 0;
            if (!result)
            {
                int i = textToAnalise.IndexOf("?");
                result = (i > -1) && (i < textToAnalise.IndexOf(" " + codeProperty.Name + " "));
            }
            return result;
        }
        public static List<FluentAPIEntityNode> GetFAPIAttributesForScalarProperties(this CodeClass srcClass, CodeClass dbContext)
        {
            if ((srcClass == null) || (dbContext == null)) return null;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction == null) return null;
            List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "Property"
                                }
                            }
                        }
                    };
            return codeFunction.DoAnalyzeWithFilter(classNames, filter);
        }
        public static List<FluentAPIEntityNode> GetFAPIAttributesAndIgnoreForScalarProperties(this CodeClass srcClass, CodeClass dbContext)
        {
            if ((srcClass == null) || (dbContext == null)) return null;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction == null) return null;
            List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "Property"
                                }
                            }
                        },
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "Ignore"
                                }
                            }
                        }
                    };
            return codeFunction.DoAnalyzeWithFilter(classNames, filter);
        }
        public static List<FluentAPIEntityNode> GetIgnoreNodes(this CodeClass srcClass, CodeClass dbContext)
        {
            if ((srcClass == null) || (dbContext == null)) return null;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction == null) return null;
            List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
            {
                new FluentAPIEntityNode()
                {
                    Methods = new List<FluentAPIMethodNode>()
                    {
                        new FluentAPIMethodNode() {
                            MethodName = "Ignore"
                        }
                    }
                }
            };
            return codeFunction.DoAnalyzeWithFilter(classNames, filter);
        }
        public static bool IsScalarTypeOrString(this CodeTypeRef codeTypeRef)
        {
            if (codeTypeRef == null) return true;
            vsCMTypeRef typeKind = codeTypeRef.TypeKind;
            if (typeKind == vsCMTypeRef.vsCMTypeRefString) return true;
            if (typeKind == vsCMTypeRef.vsCMTypeRefArray) return false;
            if (typeKind == vsCMTypeRef.vsCMTypeRefCodeType)
            {
                vsCMElement kind = codeTypeRef.CodeType.Kind;
                if (kind == vsCMElement.vsCMElementStruct)
                {
                    return true;
                }
                if (codeTypeRef.AsFullName.StartsWith("System.Nullable"))
                {
                    return true;
                }
                //if (kind == vsCMElement.vsCMElementClass)
                //{
                //    return false;
                //}
                //if (kind == vsCMElement.vsCMElementInterface)
                //{
                //    return false;
                //}
                return false;
            }
            return true;
        }
        public static bool IsNotNullableScalarTypeOrString(this CodeTypeRef codeTypeRef)
        {
            if (codeTypeRef == null) return true;
            vsCMTypeRef typeKind = codeTypeRef.TypeKind;
            if (typeKind == vsCMTypeRef.vsCMTypeRefString) return true;
            if (typeKind == vsCMTypeRef.vsCMTypeRefArray) return false;
            if (typeKind == vsCMTypeRef.vsCMTypeRefCodeType)
            {
                // vsCMElement kind = codeTypeRef.CodeType.Kind;
                if (codeTypeRef.CodeType.Kind == vsCMElement.vsCMElementStruct)
                {
                    return true;
                }
                //if (codeTypeRef.AsFullName.StartsWith("System.Nullable"))
                //{
                //    return false;
                //}
                //if (kind == vsCMElement.vsCMElementClass)
                //{
                //    return false;
                //}
                //if (kind == vsCMElement.vsCMElementInterface)
                //{
                //    return false;
                //}
                return false;
            }
            return true;
        }
        public static CodeFunction AddMethodHelper(this CodeClass destClass, string methodName, vsCMFunction Kind, vsCMTypeRef returnType, vsCMAccess Access)
        {
            if (destClass == null) return null;
            return destClass.AddFunction(methodName, Kind, returnType, -1, Access, null);
        }
        public static string GetDbContextDbSetPropertyNameHelper(this CodeClass destClass, string className, string classFullName)
        {
            if (destClass == null) return "";
            string nameLookFor1 = "System.Data.Entity.DbSet<" + className + ">";
            string nameLookFor2 = "Microsoft.EntityFrameworkCore.DbSet<" + className + ">";
            string nameLookFor3 = "System.Data.Entity.DbSet<" + classFullName + ">";
            string nameLookFor4 = "Microsoft.EntityFrameworkCore.DbSet<" + classFullName + ">";

            foreach (CodeElement ce in destClass.Members)
            {
                if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty cp = ce as CodeProperty;
                if (cp.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (cp.Type == null) continue;
                if (cp.Type.CodeType == null) continue;
                if (cp.Type.CodeType.Kind != vsCMElement.vsCMElementClass) continue;
                if (nameLookFor1.Equals(cp.Type.AsFullName, StringComparison.OrdinalIgnoreCase) ||
                    nameLookFor2.Equals(cp.Type.AsFullName, StringComparison.OrdinalIgnoreCase) ||
                    nameLookFor3.Equals(cp.Type.AsFullName, StringComparison.OrdinalIgnoreCase) ||
                    nameLookFor4.Equals(cp.Type.AsFullName, StringComparison.OrdinalIgnoreCase)
                    )
                {
                    return cp.Name;
                }
            }
            return "";
        }
        public static bool AddNameSpace(this CodeClass destClass, string nameSpace)
        {
            if (string.IsNullOrEmpty(nameSpace) || destClass == null) return false;
            if (destClass.ProjectItem != null)
            {
                if (destClass.ProjectItem.FileCodeModel != null)
                {
                    CodeElement usingDirective = null;
                    foreach (CodeElement ce in destClass.ProjectItem.FileCodeModel.CodeElements)
                    {
                        if (ce.Kind == vsCMElement.vsCMElementImportStmt)
                        {
                            usingDirective = ce;
                            string usingStr = ce.StartPoint.CreateEditPoint().GetText(ce.EndPoint);
                            usingStr = usingStr.Replace("using", "").Replace(" ", "").Replace(";", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                            if (nameSpace.Equals(usingStr, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                    if (usingDirective != null)
                    {
                        EditPoint ePoint = usingDirective.GetEndPoint().CreateEditPoint();
                        ePoint.Insert(Environment.NewLine + "using " + nameSpace + ";" + Environment.NewLine);

                        if (destClass.ProjectItem.IsDirty)
                        {
                            destClass.ProjectItem.Save();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public static CodeProperty AddDbSetPropertyHelper(this CodeClass destClass, CodeClass propertyClass, string PropertyName)
        {
            if ((destClass == null) || (propertyClass == null)) return null;
            if ((destClass.ProjectItem != null) && (propertyClass.ProjectItem != null))
            {
                if ((destClass.ProjectItem.ContainingProject != null) && (propertyClass.ProjectItem.ContainingProject != null))
                {
                    if (!string.Equals(destClass.ProjectItem.ContainingProject.UniqueName, propertyClass.ProjectItem.ContainingProject.UniqueName, StringComparison.OrdinalIgnoreCase))
                    {
                        VSProject destProject = destClass.ProjectItem.ContainingProject.Object as VSProject;
                        if (destProject != null)
                        {
                            destProject.References.AddProject(propertyClass.ProjectItem.ContainingProject);
                            destClass.ProjectItem.ContainingProject.Save();
                        }
                    }
                }
            }

            string propertyNameSpace = propertyClass.FullName;
            string propertyClassName = propertyClass.Name;
            propertyNameSpace = propertyNameSpace.Replace("." + propertyClassName, "");


            if (!destClass.AddNameSpace(propertyNameSpace))
            {
                propertyClassName = propertyClass.FullName;
            }


            CodeProperty codeProperty =
                destClass.AddProperty(PropertyName, PropertyName, "DbSet<" + propertyClassName + ">", -1, vsCMAccess.vsCMAccessPublic, null);
            EditPoint editPoint = codeProperty.Getter.StartPoint.CreateEditPoint();
            editPoint.Delete(codeProperty.Getter.EndPoint);
            editPoint.Insert("get => Set<" + propertyClassName + ">();");
            //            editPoint.Insert("get ;");

            editPoint = codeProperty.Setter.StartPoint.CreateEditPoint();
            editPoint.Delete(codeProperty.Setter.EndPoint);
            //            editPoint.Insert("set ;");
            if (destClass.ProjectItem != null)
            {
                if (destClass.ProjectItem.IsDirty)
                {
                    destClass.ProjectItem.Save();
                }
            }
            return codeProperty;
        }
        public static CodeFunction GetCodeFunctionByName(this CodeClass srcClass, vsCMAccess access, string functionName)
        {
            if ((srcClass == null) || (string.IsNullOrEmpty(functionName))) return null;
            foreach (CodeElement ce in srcClass.Members)
            {
                if (ce.Kind != vsCMElement.vsCMElementFunction) continue;
                CodeFunction codeFunction = ce as CodeFunction;
                if (codeFunction.Access != access) continue;
                if (functionName.Equals(codeFunction.Name))
                {
                    return codeFunction;
                }
            }
            return null;
        }
        public static CodeAttribute GetCodePropertyAttributeByFullName(this CodeProperty codeProperty, string fullName)
        {
            if ((codeProperty == null) || (string.IsNullOrEmpty(fullName))) return null;
            foreach (CodeElement codeElement in codeProperty.Attributes)
            {
                CodeAttribute codeAttribute = codeElement as CodeAttribute;
                if (codeAttribute.FullName.Contains(fullName))
                {
                    return codeAttribute;
                }
            }
            return null;
        }
        // ready
        public static FluentAPIKey CollectPrimaryKeyPropsHelper(this CodeClass srcClass, FluentAPIKey primKey, CodeClass dbContext = null)
        {
            if ((srcClass == null) || (primKey == null)) return null;
            primKey.KeySource = InfoSourceEnum.ByAttribute;
            primKey.SourceCount = 0;
            primKey.IsPrimary = true;
            if (primKey.KeyProperties != null) primKey.KeyProperties.Clear();
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            List<FluentAPIEntityNode> ignoreNodes = null;
            if (dbContext != null)
            {
                CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
                if (codeFunction != null)
                {
                    List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasKey"
                                }
                            }
                        }
                    };
                    List<FluentAPIEntityNode> entityNodes = codeFunction.DoAnalyzeWithFilter(classNames, filter);
                    filter[0].Methods[0].MethodName = "Ignore";
                    ignoreNodes = codeFunction.DoAnalyzeWithFilter(classNames, filter);
                    if (entityNodes != null)
                    {
                        primKey.SourceCount = entityNodes.Count;
                        if (primKey.SourceCount > 0)
                        {
                            FluentAPIEntityNode node = entityNodes.Last();
                            primKey.KeySource = InfoSourceEnum.ByOnModelCreating;
                            if (primKey.KeyProperties == null) primKey.KeyProperties = new List<FluentAPIProperty>();
                            if (node.Methods != null)
                            {
                                FluentAPIMethodNode mthd = node.Methods.FirstOrDefault(m => "HasName".Equals(m.MethodName));
                                if (mthd != null)
                                {
                                    if (mthd.MethodArguments != null)
                                    {
                                        primKey.KeyName = mthd.MethodArguments.FirstOrDefault();
                                    }
                                }
                                mthd = node.Methods.FirstOrDefault(m => "HasKey".Equals(m.MethodName));
                                if (mthd != null)
                                {
                                    if (mthd.MethodArguments != null)
                                    {
                                        int i = 0;
                                        foreach (string ma in mthd.MethodArguments)
                                        {
                                            if (ignoreNodes.HoldsIgnore(ma)) continue;
                                            primKey.KeyProperties.Add(new FluentAPIProperty()
                                            {
                                                PropName = ma,
                                                PropOrder = i
                                            });
                                            i++;
                                        }
                                        if (primKey.KeyProperties.Count > 0)
                                        {
                                            return primKey;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            primKey.SourceCount = 0;
            primKey.KeySource = InfoSourceEnum.ByConvention;
            int currOrder = -1;
            List<CodeProperty> ConventionProps = null;
            foreach (CodeElement codeElement in srcClass.Members)
            {
                currOrder++;
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;

                if (ignoreNodes != null)
                {
                    if (ignoreNodes.HoldsIgnore(codeProperty.Name)) continue;
                }
                // nullable columns can be part of the primary key
                // if (!codeProperty.Type.IsNotNullableScalarTypeOrString()) continue;
                // instead of IsNotNullableScalarTypeOrString-method we should call IsScalarTypeOrString
                if (!codeProperty.Type.IsScalarTypeOrString()) continue;

                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null)
                {
                    continue;
                }
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.KeyAttribute") == null)
                {
                    bool doAdd = "Id".Equals(codeProperty.Name, StringComparison.OrdinalIgnoreCase);
                    if (!doAdd)
                    {
                        doAdd = (classNames[0] + "Id").Equals(codeProperty.Name, StringComparison.OrdinalIgnoreCase);
                    }
                    if (doAdd)
                    {
                        if (ConventionProps == null) ConventionProps = new List<CodeProperty>();
                        ConventionProps.Add(codeProperty);
                    }

                    continue;
                }
                int locOrder = codeProperty.GetColumnOrderByAttributes();
                if (locOrder < 0)
                {
                    locOrder = currOrder;
                }
                if (primKey.KeyProperties == null) primKey.KeyProperties = new List<FluentAPIProperty>();
                primKey.KeySource = InfoSourceEnum.ByAttribute;
                primKey.KeyProperties.Add(new FluentAPIProperty()
                {
                    PropOrder = locOrder,
                    PropName = codeElement.Name
                });
            }
            if (primKey.KeyProperties != null)
            {
                if (primKey.KeyProperties.Count > 0)
                {
                    if (primKey.KeyProperties.Count > 1)
                    {
                        primKey.KeyProperties.Sort((a, b) => a.PropOrder - b.PropOrder);
                    }
                    return primKey;
                }
            }
            //string[] propNames = new string[] { "Id", srcClass.Name + "Id", };
            if (ConventionProps != null)
            {
                string propName = ConventionProps[0].Name;
                if (ConventionProps.Count > 1)
                {
                    foreach (CodeProperty conventionProp in ConventionProps)
                    {
                        if ("Id".Equals(conventionProp.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            propName = conventionProp.Name;
                        }
                    }
                }
                if (primKey.KeyProperties == null) primKey.KeyProperties = new List<FluentAPIProperty>();
                primKey.KeySource = InfoSourceEnum.ByAttribute;
                primKey.KeyProperties.Add(new FluentAPIProperty()
                {
                    PropOrder = 0,
                    PropName = propName
                });
            }
            return primKey;
        }
        public static IList<FluentAPIEntityNode> CollectAllUniqueKeysHelper(this CodeClass srcClass, IList<FluentAPIEntityNode> UniqueKeys, CodeClass dbContext = null)
        {
            if ((srcClass == null) || (UniqueKeys == null)) return null;
            UniqueKeys.Clear();
            if (dbContext == null) return UniqueKeys;
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction == null) return UniqueKeys;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
            {
                new FluentAPIEntityNode()
                {
                    Methods = new List<FluentAPIMethodNode>()
                    {
                        new FluentAPIMethodNode() {
                            MethodName = "HasAlternateKey"
                        }
                    }
                }
            };
            List<FluentAPIEntityNode> entityNodes = codeFunction.DoAnalyzeWithFilter(classNames, filter);
            if (entityNodes == null) return UniqueKeys;
            if (entityNodes.Count < 1) return UniqueKeys;
            filter[0].Methods[0].MethodName = "Ignore";
            List<FluentAPIEntityNode> ignoreNodes = codeFunction.DoAnalyzeWithFilter(classNames, filter);
            List<string> mappedProps = new List<string>();
            srcClass.CollectCodeClassAllMappedScalarProperties(mappedProps);
            foreach (FluentAPIEntityNode enttnd in entityNodes)
            {
                if (enttnd.Methods == null) continue;
                if (enttnd.Methods.Count < 1) continue;
                foreach (FluentAPIMethodNode mthd in enttnd.Methods)
                {
                    if (!("HasAlternateKey".Equals(mthd.MethodName, StringComparison.OrdinalIgnoreCase))) continue;
                    if (mthd.MethodArguments == null) continue;
                    if (mthd.MethodArguments.Count < 1) continue;
                    List<String> newLst = new List<String>();
                    foreach (string mthdarg in mthd.MethodArguments)
                    {
                        if (ignoreNodes != null)
                        {
                            if (ignoreNodes.HoldsIgnore(mthdarg)) continue;
                        }
                        if (!mappedProps.Any(x => x.Equals(mthdarg))) continue;
                        newLst.Add(mthdarg);
                    }
                    mthd.MethodArguments = newLst;
                }
                UniqueKeys.Add(enttnd);
            }
            return UniqueKeys;
        }
        public static IList<FluentAPIKey> CollectAllUniqueKeysHelper(this CodeClass srcClass, IList<FluentAPIKey> UniqueKeys, CodeClass dbContext = null)
        {
            if ((srcClass == null) || (UniqueKeys == null)) return null;
            UniqueKeys.Clear();
            IList<FluentAPIEntityNode> fApinds = srcClass.CollectAllUniqueKeysHelper(new List<FluentAPIEntityNode>(), dbContext);
            if (fApinds == null) return UniqueKeys;
            foreach (FluentAPIEntityNode fApind in fApinds)
            {
                FluentAPIMethodNode mthd = fApind.Methods.FirstOrDefault(m => "HasAlternateKey".Equals(m.MethodName));
                if (mthd == null) continue;
                if (mthd.MethodArguments == null) continue;
                if (mthd.MethodArguments.Count < 1) continue;

                FluentAPIKey key = new FluentAPIKey() { KeyProperties = new List<FluentAPIProperty>() };
                int ord = 0;
                foreach (string mthdarg in mthd.MethodArguments)
                {
                    ord++;
                    key.KeyProperties.Add(new FluentAPIProperty() { PropOrder = ord, PropName = mthdarg });
                }
                mthd = fApind.Methods.FirstOrDefault(m => "HasName".Equals(m.MethodName));
                if (mthd != null)
                {
                    if (mthd.MethodArguments != null)
                    {
                        if (mthd.MethodArguments.Count > 0)
                        {
                            key.KeyName = mthd.MethodArguments[0];
                            if (!string.IsNullOrEmpty(key.KeyName)) key.KeyName = key.KeyName.Replace("\"", "");
                        }
                    }
                }
                key.SourceCount = key.KeyProperties.Count;
                key.KeySource = InfoSourceEnum.ByOnModelCreating;
                UniqueKeys.Add(key);
            }
            return UniqueKeys;
        }
        // ready
        public static IList<string> CollectCodeClassMappedScalarNotNullProperties(this CodeClass srcClass, IList<string> properties)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            foreach (CodeElement codeElement in srcClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                if (!codeProperty.Type.IsNotNullableScalarTypeOrString())
                {
                    if (codeProperty.Type.TypeKind == vsCMTypeRef.vsCMTypeRefString)
                    {
                        if (codeProperty.IsCodePropertyNullable()) continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;

                if (codeProperty.Type.TypeKind == vsCMTypeRef.vsCMTypeRefString)
                {
                    if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.RequiredAttribute") == null)
                    {
                        if (codeProperty.IsCodePropertyNullable()) continue;
                    }
                }
                properties.Add(codeProperty.Name);
            }
            return properties;
        }
        // ready
        public static IList<FluentAPIExtendedProperty> CollectCodeClassMappedScalarNotNullProperties(this CodeClass srcClass, IList<FluentAPIExtendedProperty> properties, List<FluentAPIProperty> filter = null)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            int order = -1;
            foreach (CodeElement codeElement in srcClass.Members)
            {
                order++;
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                string codePropertyName = codeProperty.Name;
                FluentAPIProperty filterItem = null;
                if (filter != null)
                {
                    filterItem = filter.FirstOrDefault(f => f.PropName == codePropertyName);
                    if (filterItem == null) continue;
                }
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                if (!codeProperty.Type.IsNotNullableScalarTypeOrString())
                {
                    if (codeProperty.Type.TypeKind == vsCMTypeRef.vsCMTypeRefString)
                    {
                        if (codeProperty.IsCodePropertyNullable()) continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;
                bool IsNullable = false;
                if (codeTypeRef.TypeKind == vsCMTypeRef.vsCMTypeRefString)
                {
                    if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.RequiredAttribute") == null)
                    {
                        if (codeProperty.IsCodePropertyNullable()) continue;
                    }
                    IsNullable = true;
                }
                int locOrder = codeProperty.GetColumnOrderByAttributes();
                if (locOrder < 0) locOrder = order;
                string ShortTypeName = codeTypeRef.AsFullName;
                if (filterItem != null) locOrder = filterItem.PropOrder;
                FluentAPIExtendedProperty outProp = new FluentAPIExtendedProperty()
                {
                    PropOrder = locOrder,
                    PropName = codePropertyName,
                    UnderlyingTypeName = ShortTypeName,
                    TypeFullName = ShortTypeName,
                    IsNullable = IsNullable,
                    IsRequired = true
                };
                properties.Add(outProp);
            }
            return properties;
        }
        // ready
        public static IList<string> CollectCodeClassAllMappedScalarProperties(this CodeClass srcClass, IList<string> properties)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            foreach (CodeElement codeElement in srcClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                if (!codeProperty.Type.IsScalarTypeOrString()) continue;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;
                properties.Add(codeProperty.Name);
            }
            return properties;
        }
        // ready
        public static IList<FluentAPIExtendedProperty> CollectCodeClassAllMappedScalarProperties(this CodeClass srcClass, IList<FluentAPIExtendedProperty> properties, List<FluentAPIProperty> filter = null)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            int order = -1;
            foreach (CodeElement codeElement in srcClass.Members)
            {
                order++;
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                string codePropertyName = codeProperty.Name;
                FluentAPIProperty filterItem = null;
                if (filter != null)
                {
                    filterItem = filter.FirstOrDefault(f => f.PropName == codePropertyName);
                    if (filterItem == null) continue;
                }

                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (!codeTypeRef.IsScalarTypeOrString()) continue;
                vsCMTypeRef codeTypeRefTypeKind = codeTypeRef.TypeKind;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;
                int locOrder = codeProperty.GetColumnOrderByAttributes();
                if (locOrder < 0) locOrder = order;
                string ShortTypeName = "";
                bool IsNullable = false;
                bool IsRequired = true;

                ShortTypeName = codeTypeRef.AsFullName;
                if (codeTypeRefTypeKind == vsCMTypeRef.vsCMTypeRefCodeType)
                {
                    if (ShortTypeName.Contains("System.Nullable"))
                    {
                        ShortTypeName = ShortTypeName.Replace("System.Nullable", "").Replace("<", "").Replace(">", "").Replace(">", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                        IsNullable = true;
                        IsRequired = false;
                    }
                }
                else
                {
                    if (codeTypeRefTypeKind == vsCMTypeRef.vsCMTypeRefString)
                    {
                        // IsNullable = true;
                        IsNullable = codeProperty.IsCodePropertyNullable();
                        IsRequired = (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.RequiredAttribute") != null);
                    }
                }

                if (filterItem != null) locOrder = filterItem.PropOrder;
                FluentAPIExtendedProperty outProp = new FluentAPIExtendedProperty()
                {
                    PropOrder = locOrder,
                    PropName = codePropertyName,
                    UnderlyingTypeName = ShortTypeName,
                    TypeFullName = codeTypeRef.AsFullName,
                    IsNullable = IsNullable,
                    IsRequired = IsRequired
                };
                properties.Add(outProp);
            }
            return properties;
        }
        // ready
        public static IList<FluentAPIExtendedProperty> CollectCodeClassAllMappedScalarPropertiesWithDbContext(this CodeClass srcClass, IList<FluentAPIExtendedProperty> properties, List<FluentAPIProperty> filter = null, CodeClass dbContext = null)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            if (dbContext == null)
                return srcClass.CollectCodeClassAllMappedScalarProperties(properties, filter);
            List<FluentAPIEntityNode> localFAENs = srcClass.GetFAPIAttributesAndIgnoreForScalarProperties(dbContext);
            int order = -1;
            foreach (CodeElement codeElement in srcClass.Members)
            {
                order++;
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                string codePropertyName = codeProperty.Name;
                FluentAPIProperty filterItem = null;
                if (filter != null)
                {
                    filterItem = filter.FirstOrDefault(f => f.PropName == codePropertyName);
                    if (filterItem == null) continue;
                }
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (!codeTypeRef.IsScalarTypeOrString()) continue;
                vsCMTypeRef codeTypeRefTypeKind = codeTypeRef.TypeKind;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;
                if (localFAENs.HoldsIgnore(codePropertyName)) continue;
                int locOrder = codeProperty.GetColumnOrderByAttributes();
                if (locOrder < 0) locOrder = order;
                string ShortTypeName = "";
                bool IsNullable = false;
                bool IsRequired = true;
                ShortTypeName = codeTypeRef.AsFullName;
                if (codeTypeRefTypeKind == vsCMTypeRef.vsCMTypeRefCodeType)
                {
                    if (ShortTypeName.Contains("System.Nullable"))
                    {
                        ShortTypeName = ShortTypeName.Replace("System.Nullable", "").Replace("<", "").Replace(">", "").Replace(">", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                        IsNullable = true;
                        IsRequired = false;
                    }
                }
                else
                {
                    if (codeTypeRefTypeKind == vsCMTypeRef.vsCMTypeRefString)
                    {
                        // IsNullable = true;
                        IsNullable = codeProperty.IsCodePropertyNullable();
                        IsRequired = (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.RequiredAttribute") != null);
                    }
                }
                bool locIsRequired;
                if (localFAENs.HoldsIsRequired(codePropertyName, out locIsRequired)) IsRequired = locIsRequired;
                if (filterItem != null) locOrder = filterItem.PropOrder;
                FluentAPIExtendedProperty outProp = new FluentAPIExtendedProperty()
                {
                    PropOrder = locOrder,
                    PropName = codePropertyName,
                    UnderlyingTypeName = ShortTypeName,
                    TypeFullName = codeTypeRef.AsFullName,
                    IsNullable = IsNullable,
                    IsRequired = IsRequired
                };
                properties.Add(outProp);
            }
            return properties;
        }
        // ready
        public static IList<string> CollectCodeClassAllMappedNonScalarProperties(this CodeClass srcClass, IList<string> properties)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            properties.Clear();
            foreach (CodeElement codeElement in srcClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (codeTypeRef.IsScalarTypeOrString()) continue;
                //if (codeTypeRef.TypeKind != vsCMTypeRef.vsCMTypeRefCodeType) continue;
                if (codeTypeRef.TypeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                vsCMElement codeTypeKind = codeTypeRef.CodeType.Kind;
                if (codeTypeKind == vsCMElement.vsCMElementInterface) continue;
                if (!(codeTypeKind == vsCMElement.vsCMElementClass)) continue;

                string codeTypeRefFullName = codeTypeRef.CodeType.FullName;
                if (codeTypeRefFullName.StartsWith("System.Collections.Generic.List<"))
                {
                    continue;
                }
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;
                properties.Add(codeProperty.Name);
            }
            return properties;
        }
        public static IList<string> CollectCodeClassAllMappedNonScalarPropertiesWithDbContext(this CodeClass srcClass, IList<string> properties, CodeClass dbContext = null)
        {
            if ((srcClass == null) || (properties == null))
            {
                return null;
            }
            srcClass.CollectCodeClassAllMappedNonScalarProperties(properties);
            List<FluentAPIEntityNode> localFAENs = srcClass.GetFAPIAttributesAndIgnoreForScalarProperties(dbContext);
            for (int i = properties.Count - 1; i >= 0; i--)
            {
                if (localFAENs.HoldsIgnore(properties[i])) properties.RemoveAt(i);
            }
            return properties;
        }
        public static void RemoveUniqueKeyDeclarations(this CodeClass srcClass, CodeClass dbContext, string UniqueKeyName)
        {
            if ((srcClass == null) || (dbContext == null)) return;
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction != null)
            {
                string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
                if (!string.IsNullOrEmpty(UniqueKeyName)) UniqueKeyName = "\"" + UniqueKeyName.Replace("\"", "") + "\"";
                List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasAlternateKey"
                                },
                                new FluentAPIMethodNode() {
                                    MethodName = "HasName",
                                    MethodArguments = new List<string> { UniqueKeyName }
                                },
                            }
                        }
                    };
                codeFunction.DoRemoveInvocationWithFilter(classNames, filter);
                dbContext.StartPoint.CreateEditPoint().SmartFormat(dbContext.EndPoint);
                if (dbContext.ProjectItem != null) dbContext.ProjectItem.Save();
            }
        }
        public static void AddUniqueKeyDeclaration(this CodeClass srcClass, CodeClass dbContext, string UniqueKeyStatement, string UniqueKeyName)
        {
            if (string.IsNullOrEmpty(UniqueKeyStatement) || string.IsNullOrEmpty(UniqueKeyName) || (srcClass == null) || (dbContext == null)) return;
            srcClass.RemoveUniqueKeyDeclarations(dbContext, "\"" + UniqueKeyName.Replace("\"", "") + "\"");

            string propertyNameSpace = srcClass.FullName;
            string propertyClassName = srcClass.Name;
            propertyNameSpace = propertyNameSpace.Replace("." + propertyClassName, "");
            if (!dbContext.AddNameSpace(propertyNameSpace))
            {
                propertyClassName = srcClass.FullName;
            }

            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction != null)
            {
                codeFunction.AddStatementsToFunctionBody(UniqueKeyStatement, propertyClassName, false);
                dbContext.StartPoint.CreateEditPoint().SmartFormat(dbContext.EndPoint);
                if (dbContext.ProjectItem != null)
                {
                    dbContext.ProjectItem.Save();
                }
            }
        }
        public static void RemovePrimaryKeyDeclarations(this CodeClass srcClass, CodeClass dbContext)
        {
            if ((srcClass == null) || (dbContext == null)) return;
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction != null)
            {
                string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
                List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasKey"
                                }
                            }
                        }
                    };
                codeFunction.DoRemoveInvocationWithFilter(classNames, filter);
                dbContext.StartPoint.CreateEditPoint().SmartFormat(dbContext.EndPoint);
                if (dbContext.ProjectItem != null) dbContext.ProjectItem.Save();
            }
        }
        public static void AddPrimaryKeyDeclaration(this CodeClass srcClass, CodeClass dbContext, string PrimKeyStatement)
        {
            if (string.IsNullOrEmpty(PrimKeyStatement) || (srcClass == null) || (dbContext == null)) return;
            srcClass.RemovePrimaryKeyDeclarations(dbContext);

            string propertyNameSpace = srcClass.FullName;
            string propertyClassName = srcClass.Name;
            propertyNameSpace = propertyNameSpace.Replace("." + propertyClassName, "");
            if (!dbContext.AddNameSpace(propertyNameSpace))
            {
                propertyClassName = srcClass.FullName;
            }

            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction != null)
            {
                codeFunction.AddStatementsToFunctionBody(PrimKeyStatement, propertyClassName, false);
                dbContext.StartPoint.CreateEditPoint().SmartFormat(dbContext.EndPoint);
                if (dbContext.ProjectItem != null)
                {
                    dbContext.ProjectItem.Save();
                }
            }
        }
        public static void RemoveForeignKeyDeclarations(this CodeClass srcClass, CodeClass dbContext, string navigationName)
        {
            if ((srcClass == null) || (dbContext == null)) return;
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction != null)
            {
                string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
                List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        // 
                        // EF.Net 6
                        //
                        
                       // or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasRequired",
                                    MethodArguments =  string.IsNullOrEmpty(navigationName) ? null: new List<string>() { navigationName }
                                }
                            }
                        },
                        // or 
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasOptional",
                                    MethodArguments =  string.IsNullOrEmpty(navigationName) ? null: new List<string>() { navigationName }
                                }
                            }
                        },

                        //
                        //  EF.Core 6
                        //

                        // or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasOne",
                                    MethodArguments =  string.IsNullOrEmpty(navigationName) ? null: new List<string>() { navigationName }
                                }
                            }
                        },
                        // or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasMany",
                                    MethodArguments =  string.IsNullOrEmpty(navigationName) ? null: new List<string>() { navigationName }
                                }
                            }
                        }
                    };
                codeFunction.DoRemoveInvocationWithFilter(classNames, filter);
                dbContext.StartPoint.CreateEditPoint().SmartFormat(dbContext.EndPoint);
                if (dbContext.ProjectItem != null) dbContext.ProjectItem.Save();
            }
        }
        public static void AddForeignKeyDeclaration(this CodeClass srcClass, CodeClass dbContext, string ForeignKeyStatement, string navigationName)
        {
            if (string.IsNullOrEmpty(navigationName) || string.IsNullOrEmpty(ForeignKeyStatement) || (srcClass == null) || (dbContext == null)) return;
            srcClass.RemoveForeignKeyDeclarations(dbContext, navigationName);

            string propertyNameSpace = srcClass.FullName;
            string propertyClassName = srcClass.Name;
            propertyNameSpace = propertyNameSpace.Replace("." + propertyClassName, "");
            if (!dbContext.AddNameSpace(propertyNameSpace))
            {
                propertyClassName = srcClass.FullName;
            }

            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction != null)
            {
                codeFunction.AddStatementsToFunctionBody(ForeignKeyStatement, propertyClassName, true);
                dbContext.StartPoint.CreateEditPoint().SmartFormat(dbContext.EndPoint);
                if (dbContext.ProjectItem != null)
                {
                    dbContext.ProjectItem.Save();
                }
            }
        }
        public static string GetAttributeArgument(this CodeAttribute codeAttribute)
        {
            if (codeAttribute == null) return null;
            foreach (CodeElement chld in codeAttribute.Children)
            {
                if (chld is CodeAttributeArgument)
                {
                    return (chld as CodeAttributeArgument).Value.Replace("\"", "");
                }
            }
            return null;
        }
        public static string GetAttributeArgumentByArgumentName(this CodeAttribute codeAttribute, string argumentName)
        {
            if ((codeAttribute == null) || (string.IsNullOrEmpty(argumentName))) return null;
            foreach (CodeElement chld in codeAttribute.Children)
            {
                if (chld is CodeAttributeArgument)
                {
                    if (argumentName.Equals(chld.Name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return (chld as CodeAttributeArgument).Value.Replace("\"", "");
                    }
                }
            }
            return null;
        }
        public static int GetColumnOrderByAttributes(this CodeProperty codeProperty)
        {
            if (codeProperty == null) return -1;
            foreach (CodeElement lcea in codeProperty.Attributes)
            {
                CodeAttribute lca = lcea as CodeAttribute;
                if (lca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"))
                {
                    foreach (CodeElement chld in lca.Children)
                    {

                        if ("Order".Equals(chld.Name, System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (chld is CodeAttributeArgument)
                            {
                                int val;
                                if (int.TryParse((chld as CodeAttributeArgument).Value, out val))
                                {
                                    return val;
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }
        // ready
        public static CodeProperty GetPublicMappedScalarPropertyByName(this CodeClass srcClass, string propertyName, out int columnOrder)
        {
            columnOrder = -1;
            if ((srcClass == null) || (string.IsNullOrEmpty(propertyName))) return null;
            int order = -1;
            foreach (CodeElement codeElement in srcClass.Members)
            {
                order++;
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                if (!codeProperty.Type.IsScalarTypeOrString()) continue;

                bool isNotMapped = false;
                int locColOrder = -1;
                foreach (CodeElement lcea in codeProperty.Attributes)
                {
                    CodeAttribute lca = lcea as CodeAttribute;
                    if (lca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                    {
                        isNotMapped = true;
                        break;
                    }
                    if (lca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"))
                    {
                        string strVal = lca.GetAttributeArgumentByArgumentName("Order");
                        if (int.TryParse(strVal, out int colOrder))
                        {
                            locColOrder = colOrder;
                        }
                    }
                }
                if (isNotMapped)
                {
                    continue;
                }
                if (propertyName.Equals(codeElement.Name))
                {
                    if (locColOrder < 0)
                    {
                        columnOrder = order;
                    }
                    else
                    {
                        columnOrder = locColOrder;
                    }
                    return codeProperty;
                }
            }
            return null;
        }
        public static List<FluentAPIProperty> GetPublicMappedScalarPropertyByAttributeArgument(this CodeClass srcClass, string attributeFullName, string attributeArgument, string splitter = null)
        {
            List<FluentAPIProperty> result = null;
            if ((srcClass == null) || string.IsNullOrEmpty(attributeFullName) || string.IsNullOrEmpty(attributeArgument)) return null;
            int order = -1;
            foreach (CodeElement codeElement in srcClass.Members)
            {
                order++;
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                if (!codeProperty.Type.IsScalarTypeOrString()) continue;
                bool isNotMapped = false;
                int locOrder = -1;
                bool mustInclude = false;
                foreach (CodeElement lcea in codeProperty.Attributes)
                {
                    CodeAttribute lca = lcea as CodeAttribute;
                    if (lca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                    {
                        isNotMapped = true;
                        break;
                    }
                    if (lca.FullName.Contains(attributeFullName))
                    {
                        foreach (CodeElement chld in lcea.Children)
                        {
                            if (chld is CodeAttributeArgument)
                            {
                                string argumentValue = (chld as CodeAttributeArgument).Value.Replace("\"", "");
                                if (splitter != null)
                                {
                                    string[] argVals = argumentValue.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
                                    int cnt = argVals.Length;
                                    for (int i = 0; i < cnt; i++)
                                    {
                                        mustInclude = attributeArgument.Equals(argVals[i]);
                                        if (mustInclude)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    mustInclude = attributeArgument.Equals(argumentValue);
                                }
                            }
                            if (mustInclude) break;
                        }
                    }

                    if (mustInclude)
                    {
                        if (lca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"))
                        {
                            lca.GetAttributeArgumentByArgumentName("Order");
                            string strVal = lca.GetAttributeArgumentByArgumentName("Order");
                            if (int.TryParse(strVal, out int val))
                            {
                                locOrder = val;
                            }
                        }
                        break;
                    }
                }
                if (isNotMapped || (!mustInclude))
                {
                    continue;
                }
                if (locOrder < 0) locOrder = order;
                if (result == null)
                {
                    result = new List<FluentAPIProperty>();
                }
                result.Add(new FluentAPIProperty()
                {
                    PropOrder = locOrder,
                    PropName = codeElement.Name
                });
            }
            return result;
        }
        // ready
        public static CodeProperty GetPublicMappedNonScalarPropertyByAttributeArgument(this CodeClass masterCodeClass, string attributeFullName, string attributeArgument)
        {
            if ((masterCodeClass == null) || string.IsNullOrEmpty(attributeFullName) || string.IsNullOrEmpty(attributeArgument)) return null;
            foreach (CodeElement codeElement in masterCodeClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;

                if (codeTypeRef.IsScalarTypeOrString()) continue;

                vsCMTypeRef typeKind = codeTypeRef.TypeKind;
                if (typeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (typeKind != vsCMTypeRef.vsCMTypeRefCodeType) continue;
                //if (codeTypeRef.AsFullName.StartsWith("System.Nullable")) continue;
                vsCMElement codeTypeKind = codeTypeRef.CodeType.Kind;
                if (!((codeTypeKind == vsCMElement.vsCMElementClass) || (codeTypeKind == vsCMElement.vsCMElementInterface))) continue;
                bool isNotMapped = false;
                bool isFound = false;
                foreach (CodeElement lcea in codeProperty.Attributes)
                {
                    CodeAttribute lca = lcea as CodeAttribute;
                    if (lca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                    {
                        isNotMapped = true;
                        break;
                    }
                    if (!isFound)
                    {
                        if (lca.FullName.Contains(attributeFullName))
                        {
                            foreach (CodeElement chld in lcea.Children)
                            {
                                if (chld is CodeAttributeArgument)
                                {
                                    string attributeArgVal = (chld as CodeAttributeArgument).Value.Replace("\"", "");
                                    isFound = attributeArgument.Equals(attributeArgVal);
                                }
                            }
                        }
                    }
                }
                if (isNotMapped) continue;
                if (isFound) return codeProperty;
            }
            return null;
        }
        // ready
        public static CodeProperty GetPublicMappedNonScalarPropertyByName(this CodeClass masterCodeClass, string propertyName)
        {
            if ((masterCodeClass == null) || string.IsNullOrEmpty(propertyName)) return null;
            foreach (CodeElement codeElement in masterCodeClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                if (!propertyName.Equals(codeElement.Name)) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (codeTypeRef.IsScalarTypeOrString()) continue;
                vsCMTypeRef typeKind = codeTypeRef.TypeKind;
                if (typeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (typeKind != vsCMTypeRef.vsCMTypeRefCodeType) continue;
                //if (codeTypeRef.AsFullName.StartsWith("System.Nullable")) continue;
                vsCMElement codeTypeKind = codeTypeRef.CodeType.Kind;
                if (!((codeTypeKind == vsCMElement.vsCMElementClass) || (codeTypeKind == vsCMElement.vsCMElementInterface))) continue;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;
                return codeProperty;
            }
            return null;
        }
        public static CodeProperty GetPublicMappedNonScalarPropertyByNameWithDbContext(this CodeClass masterCodeClass, string propertyName, CodeClass dbContext = null)
        {
            if (masterCodeClass == null) return null;
            CodeProperty result = masterCodeClass.GetPublicMappedNonScalarPropertyByName(propertyName);
            if (result == null) return null;
            if (dbContext == null) return result;
            List<FluentAPIEntityNode> localFAENs = masterCodeClass.GetIgnoreNodes(dbContext);
            if (localFAENs != null)
            {
                if (localFAENs.HoldsIgnore(propertyName)) return null;
            }
            return result;
        }
        // ready
        public static List<CodeProperty> GetPublicMappedNonScalarPropertiesByTypeFullName(this CodeClass masterCodeClass, string typeFullName)
        {
            if ((masterCodeClass == null) || string.IsNullOrEmpty(typeFullName)) return null;
            string collectionTypeName1 = "System.Collections.Generic.IEnumerable<" + typeFullName + ">";
            string collectionTypeName2 = "System.Collections.Generic.List<" + typeFullName + ">";
            string collectionTypeName3 = "System.Collections.Generic.ICollection<" + typeFullName + ">";
            List<CodeProperty> result = null;
            foreach (CodeElement codeElement in masterCodeClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (codeTypeRef.IsScalarTypeOrString()) continue;
                vsCMTypeRef typeKind = codeTypeRef.TypeKind;
                if (typeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (typeKind != vsCMTypeRef.vsCMTypeRefCodeType) continue;

                //if (codeTypeRef.AsFullName.StartsWith("System.Nullable")) continue;

                vsCMElement codeTypeKind = codeTypeRef.CodeType.Kind;
                if (!((codeTypeKind == vsCMElement.vsCMElementClass) || (codeTypeKind == vsCMElement.vsCMElementInterface))) continue;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;

                string codeTypeFullName = codeTypeRef.CodeType.FullName;
                if (codeTypeKind == vsCMElement.vsCMElementClass)
                {
                    if (typeFullName.Equals(codeTypeFullName))
                    {
                        if (result == null)
                        {
                            result = new List<CodeProperty>();
                        }
                        result.Add(codeProperty);
                        continue;
                    }
                }
                if (collectionTypeName1.Equals(codeTypeFullName) ||
                    collectionTypeName2.Equals(codeTypeFullName) ||
                    collectionTypeName3.Equals(codeTypeFullName))
                {
                    if (result == null)
                    {
                        result = new List<CodeProperty>();
                    }
                    result.Add(codeProperty);
                }
            }
            return result;
        }

        public static List<CodeProperty> GetPublicMappedNonScalarPropertiesByTypeFullNameWithDbContext(this CodeClass masterCodeClass, string typeFullName, CodeClass dbContext = null)
        {
            if (masterCodeClass == null) return null;
            List<CodeProperty> result = masterCodeClass.GetPublicMappedNonScalarPropertiesByTypeFullName(typeFullName);
            if (result == null) return null;
            if (dbContext == null) return result;
            List<FluentAPIEntityNode> localFAENs = masterCodeClass.GetIgnoreNodes(dbContext);
            if (localFAENs != null)
            {
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    if (localFAENs.HoldsIgnore(result[i].Name)) result.RemoveAt(i);
                }
            }
            return result;
        }

        public static FluentAPIKey CollectForeignKeyByAttributes(this CodeProperty codeProperty)
        {
            FluentAPIKey result = null;
            if (codeProperty == null) return result;
            if (codeProperty.Type == null) return result;
            if (codeProperty.Type.CodeType == null) return result;
            CodeClass srcClass = codeProperty.Parent;
            CodeAttribute codeAttribute = codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute");
            if (codeAttribute != null)
            {
                string attributeArgument = codeAttribute.GetAttributeArgument();
                if (!string.IsNullOrEmpty(attributeArgument))
                {
                    string[] propNames = attributeArgument.Split(new char[] { ',' });
                    int cnt = propNames.Length;
                    for (int i = 0; i < cnt; i++)
                    {
                        if (string.IsNullOrEmpty(propNames[i])) continue;
                        CodeProperty locCodeProperty = srcClass.GetPublicMappedScalarPropertyByName(propNames[i].Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", ""), out int colOrder);
                        if (locCodeProperty != null)
                        {
                            int attrColOrder = locCodeProperty.GetColumnOrderByAttributes();
                            if (attrColOrder < 0) attrColOrder = colOrder;
                            if (result == null)
                            {
                                result = new FluentAPIKey()
                                {
                                    KeySource = InfoSourceEnum.ByAttribute,
                                    KeyProperties = new List<FluentAPIProperty>()
                                };
                            }
                            result.KeyProperties.Add(
                                            new FluentAPIProperty()
                                            {
                                                PropOrder = i, // importantly: PropOrder = i
                                                PropName = locCodeProperty.Name
                                            });
                        }
                    }
                }
            }
            if (result != null)
            {
                return result;
            }
            List<FluentAPIProperty> props =
            srcClass.GetPublicMappedScalarPropertyByAttributeArgument("System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute", codeProperty.Name, ",");
            if (props != null)
            {
                if (props.Count > 0)
                {
                    if (result == null)
                    {
                        result = new FluentAPIKey();
                    }
                    result.KeySource = InfoSourceEnum.ByAttribute;
                    result.KeyProperties = props;
                    return result;
                }
            }
            if (result != null)
            {
                if (result.KeyProperties != null)
                {
                    result.KeyProperties.Sort((a, b) => a.PropOrder - b.PropOrder);
                }
            }
            return result;
        }
        public static FluentAPIKey CollectForeignKeyByConventions(this CodeProperty codeProperty, FluentAPIKey principalKey = null, CodeClass dbContext = null)
        {
            FluentAPIKey result = null;
            if (codeProperty == null) return result;
            if (codeProperty.Type == null) return result;
            if (codeProperty.Type.CodeType == null) return result;

            CodeClass srcClass = codeProperty.Parent;
            CodeClass masterCodeClass = codeProperty.Type.CodeType as CodeClass;
            if (principalKey == null)
            {
                principalKey = new FluentAPIKey();
                masterCodeClass.CollectPrimaryKeyPropsHelper(principalKey, dbContext);
            }
            //
            // If the dependent entity contains a property with a name matching one (one !!!!) of these patterns 
            // then it will be configured as the foreign key:
            // <navigation property name><principal key property name>
            // <navigation property name> Id
            // <principal entity name><principal key property name>
            // <principal entity name> Id
            //

            string navigationName = codeProperty.Name;
            string masterClassName = masterCodeClass.Name;

            // case #1 : <navigation property name><principal key property name>
            // case #3 : <principal entity name><principal key property name>
            string[] prefix = new string[] { navigationName, masterClassName };
            for (int i = 0; i < 2; i++)
            {
                if (principalKey.KeyProperties != null)
                {
                    if (principalKey.KeyProperties.Count > 0)
                    {
                        bool isFound = true;
                        foreach (FluentAPIProperty primKeyProp in principalKey.KeyProperties)
                        {
                            isFound = (srcClass.GetPublicMappedScalarPropertyByName(prefix[i] + primKeyProp.PropName, out int colOrder) != null);
                            if (!isFound) break;
                        }
                        if (isFound)
                        {
                            if (result == null) result = new FluentAPIKey();
                            result.KeySource = InfoSourceEnum.ByConvention;
                            if (result.KeyProperties == null) result.KeyProperties = new List<FluentAPIProperty>();
                            int order = 0;
                            foreach (FluentAPIProperty primKeyProp in principalKey.KeyProperties)
                            {
                                result.KeyProperties.Add(new FluentAPIProperty()
                                {
                                    PropName = prefix[i] + primKeyProp.PropName,
                                    PropOrder = order
                                });
                                order++;
                            }
                            return result;
                        }
                    }
                }
            }
            // case #2: <navigation property name>Id
            // case #4: <principal entity name>Id
            for (int i = 0; i < 2; i++)
            {
                CodeProperty cp = srcClass.GetPublicMappedScalarPropertyByName(prefix[i] + "Id", out int colOrder);
                if (cp != null)
                {
                    if (result == null) result = new FluentAPIKey();
                    result.KeySource = InfoSourceEnum.ByConvention;
                    if (result.KeyProperties == null) result.KeyProperties = new List<FluentAPIProperty>();
                    result.KeyProperties.Add(new FluentAPIProperty()
                    {
                        PropName = cp.Name
                    });
                    return result;
                }
            }
            return result;
        }
        public static bool CheckIsAllRequired(this CodeClass srcClass, List<FluentAPIProperty> props)
        {
            if ((srcClass == null) || (props == null)) return false;
            if (props.Count < 1) return false;
            foreach (FluentAPIProperty prop in props)
            {
                CodeProperty codeProperty = srcClass.GetPublicMappedScalarPropertyByName(prop.PropName, out int order);
                if (codeProperty == null) return false;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (codeTypeRef.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType)
                {
                    if (codeTypeRef.AsFullName.StartsWith("System.Nullable")) return false;
                }
                else
                {
                    if (codeTypeRef.TypeKind == vsCMTypeRef.vsCMTypeRefString)
                    {
                        if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.RequiredAttribute") == null)
                        {
                            if (codeProperty.IsCodePropertyNullable()) return false;
                        }
                    }
                }
            }
            return true;
        }
        public static void DefineNavigationType(this CodeProperty masterCodeProperty, FluentAPIForeignKey fluentAPIForeignKey, CodeClass srcClass)
        {
            if ((masterCodeProperty == null) || (fluentAPIForeignKey == null) || (srcClass == null)) return;
            if (masterCodeProperty.Type != null)
            {
                if (masterCodeProperty.Type.CodeType != null)
                {
                    CodeTypeRef masterCodeTypeRef = masterCodeProperty.Type;
                    if (masterCodeTypeRef.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType)
                    {
                        bool isMany = (masterCodeTypeRef.CodeType.Kind == vsCMElement.vsCMElementInterface);
                        if (!isMany)
                        {
                            isMany = masterCodeTypeRef.CodeType.FullName.StartsWith("System.Collections.Generic.List<");
                        }
                        bool isRequired = srcClass.CheckIsAllRequired(fluentAPIForeignKey.ForeignKeyProps);
                        if (isMany)
                        {
                            if (isRequired)
                            {
                                fluentAPIForeignKey.NavigationType = NavigationTypeEnum.OneToMany;
                            }
                            else
                            {
                                fluentAPIForeignKey.NavigationType = NavigationTypeEnum.OptionalToMany;
                            }
                        }
                        else
                        {
                            if (isRequired)
                            {
                                fluentAPIForeignKey.NavigationType = NavigationTypeEnum.OneToOne;
                            }
                            else
                            {
                                fluentAPIForeignKey.NavigationType = NavigationTypeEnum.OptionalToOne;
                            }
                        }
                    }
                }
            }
            fluentAPIForeignKey.InverseNavigationName = masterCodeProperty.Name;
        }
        public static void DefineFluentAPIForeignKey(this FluentAPIForeignKey dest, CodeClass srcClass, CodeClass masterCodeClass, FluentAPIKey foreignKey, FluentAPIKey masterPrimKey)
        {
            if (dest == null) return;
            if (srcClass != null)
            {
                dest.EntityName = srcClass.Name;
                dest.EntityFullName = srcClass.FullName;
                dest.CodeElementEntityRef = srcClass as CodeElement;
            }
            if (masterCodeClass != null)
            {
                dest.NavigationEntityName = masterCodeClass.Name;
                dest.NavigationEntityFullName = masterCodeClass.FullName;
                dest.CodeElementNavigationRef = masterCodeClass as CodeElement;
            }

            if (masterPrimKey != null)
            {
                dest.PrincipalKeySource = masterPrimKey.KeySource;
                dest.PrincipalKeySourceCount = masterPrimKey.SourceCount;
                if (masterPrimKey.KeyProperties != null)
                {
                    if (masterPrimKey.KeyProperties.Count > 0)
                    {
                        dest.PrincipalKeyProps = masterPrimKey.KeyProperties;
                    }
                }
            }

            if (foreignKey != null)
            {
                dest.ForeignKeySource = foreignKey.KeySource;
                dest.ForeignKeySourceCount = foreignKey.SourceCount;
                if (foreignKey.KeyProperties != null)
                {
                    dest.ForeignKeyProps = foreignKey.KeyProperties;
                }
            }

        }
        public static bool IsOfCollectionType(this CodeProperty codeProperty)
        {
            if (codeProperty == null) return true;
            if (codeProperty.Type == null) return true;
            if (codeProperty.Type.CodeType == null) return true;
            CodeTypeRef codeTypeRef = codeProperty.Type;
            if (codeTypeRef.TypeKind != vsCMTypeRef.vsCMTypeRefCodeType) return false;
            if (codeTypeRef.CodeType.Kind == vsCMElement.vsCMElementInterface) return true;
            if (codeTypeRef.CodeType.Kind != vsCMElement.vsCMElementClass) return false;
            if (codeTypeRef.AsFullName.StartsWith("System.Collections.Generic.List<")) return true;
            return false;
        }
        public static FluentAPIForeignKey FluentAPIEntityNodeToForeignKey(this FluentAPIEntityNode entityNode, bool ignoreTheOpposite = false)
        {
            FluentAPIForeignKey result = null;
            if (entityNode == null) return result;
            if (entityNode.Methods == null) return result;
            int methodsCount = entityNode.Methods.Count;
            if (methodsCount < 1) return result;
            FluentAPIMethodNode firstMethodNode = entityNode.Methods[0];
            if (string.IsNullOrEmpty(firstMethodNode.MethodName)) return result;
            if (firstMethodNode.MethodArguments == null) return result;
            if (firstMethodNode.MethodArguments.Count < 1) return result;
            //
            // EF.Net
            //
            if ("HasRequired".Equals(firstMethodNode.MethodName))
            {
                string InverseNavigationName = null;
                List<String> foreignKeyProps = null;
                bool IsOneToMany = false;
                bool IsWithOptional = false;
                bool IsCascadeDelete = false;
                for (int i = 1; i < methodsCount; i++)
                {
                    FluentAPIMethodNode methodNode = entityNode.Methods[i];
                    if (string.IsNullOrEmpty(methodNode.MethodName)) return result;
                    if ("WillCascadeOnDelete".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments != null)
                        {
                            IsCascadeDelete = methodNode.MethodArguments.Any(ii => ii.Contains("true") || ii.Contains("True"));
                        }
                    }
                    if ("WithMany".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        InverseNavigationName = methodNode.MethodArguments[0];
                        IsOneToMany = true;
                    }
                    if ("WithOptional".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        InverseNavigationName = methodNode.MethodArguments[0];
                        IsWithOptional = true;
                    }
                    if ("HasForeignKey".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        foreignKeyProps = methodNode.MethodArguments;
                    }
                }
                // if both are equal false or both are equal true then ignore
                if (IsOneToMany == IsWithOptional) return result;
                if (IsOneToMany)
                {
                    if (foreignKeyProps == null) return result;
                    if (foreignKeyProps.Count < 1) return result;
                    result = new FluentAPIForeignKey()
                    {
                        NavigationName = firstMethodNode.MethodArguments[0],
                        InverseNavigationName = InverseNavigationName,
                        ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                        PrincipalKeySource = InfoSourceEnum.ByConvention,
                        InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                        NavigationType = NavigationTypeEnum.OneToMany,
                        IsCascadeDelete = IsCascadeDelete
                    };
                    result.ForeignKeyProps = new List<FluentAPIProperty>();
                    int ord = 0;
                    foreach (string prop in foreignKeyProps)
                    {
                        result.ForeignKeyProps.Add(
                        new FluentAPIProperty()
                        {
                            PropName = prop,
                            PropOrder = ord++
                        });
                    }
                    return result;
                }
                if (IsWithOptional)
                {
                    result = new FluentAPIForeignKey()
                    {
                        NavigationName = firstMethodNode.MethodArguments[0],
                        InverseNavigationName = InverseNavigationName,
                        ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                        PrincipalKeySource = InfoSourceEnum.ByConvention,
                        InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                        NavigationType = NavigationTypeEnum.OneToOne,
                        IsCascadeDelete = IsCascadeDelete
                    };
                    if (foreignKeyProps == null) return result;
                    if (foreignKeyProps.Count < 1) return result;
                    result.ForeignKeyProps = new List<FluentAPIProperty>();
                    int ord = 0;
                    foreach (string prop in foreignKeyProps)
                    {
                        result.ForeignKeyProps.Add(
                        new FluentAPIProperty()
                        {
                            PropName = prop,
                            PropOrder = ord++
                        });
                    }
                }
                return result;
            }
            if ("HasOptional".Equals(firstMethodNode.MethodName))
            {
                string InverseNavigationName = null;
                List<String> foreignKeyProps = null;
                bool IsOneToMany = false;
                bool IsWithRequired = false;
                bool IsCascadeDelete = false;
                for (int i = 1; i < methodsCount; i++)
                {
                    FluentAPIMethodNode methodNode = entityNode.Methods[i];
                    if (string.IsNullOrEmpty(methodNode.MethodName)) return result;
                    if ("WillCascadeOnDelete".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments != null)
                        {
                            IsCascadeDelete = methodNode.MethodArguments.Any(ii => ii.Contains("true") || ii.Contains("True"));
                        }
                    }

                    if ("WithMany".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        InverseNavigationName = methodNode.MethodArguments[0];
                        IsOneToMany = true;
                    }
                    if ("WithRequired".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        InverseNavigationName = methodNode.MethodArguments[0];
                        IsWithRequired = true;
                    }
                    if ("HasForeignKey".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        foreignKeyProps = methodNode.MethodArguments;
                    }
                }
                // if both are equal false or both are equal true then ignore
                if (IsOneToMany == IsWithRequired) return result;
                if (IsOneToMany)
                {
                    if (foreignKeyProps == null) return result;
                    if (foreignKeyProps.Count < 1) return result;
                    result = new FluentAPIForeignKey()
                    {
                        NavigationName = firstMethodNode.MethodArguments[0],
                        InverseNavigationName = InverseNavigationName,
                        ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                        PrincipalKeySource = InfoSourceEnum.ByConvention,
                        InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                        NavigationType = NavigationTypeEnum.OptionalToMany,
                        IsCascadeDelete = IsCascadeDelete
                    };
                    result.ForeignKeyProps = new List<FluentAPIProperty>();
                    int ord = 0;
                    foreach (string prop in foreignKeyProps)
                    {
                        result.ForeignKeyProps.Add(
                        new FluentAPIProperty()
                        {
                            PropName = prop,
                            PropOrder = ord++
                        });
                    }
                    return result;
                }
                if (IsWithRequired)
                {
                    // this is a foreign key for inverse navigation
                    result = new FluentAPIForeignKey()
                    {

                        InverseNavigationName = firstMethodNode.MethodArguments[0], // correct !!!
                        NavigationName = InverseNavigationName, // correct !!!

                        ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                        PrincipalKeySource = InfoSourceEnum.ByConvention,
                        InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                        NavigationType = NavigationTypeEnum.OptionalToOne,
                        IsCascadeDelete = IsCascadeDelete
                    };
                    if (foreignKeyProps == null) return result;
                    if (foreignKeyProps.Count < 1) return result;
                    result.PrincipalKeyProps = new List<FluentAPIProperty>(); // correct !!!
                    int ord = 0;
                    foreach (string prop in foreignKeyProps)
                    {
                        result.PrincipalKeyProps.Add(
                        new FluentAPIProperty()
                        {
                            PropName = prop,
                            PropOrder = ord++
                        });
                    }
                }
                return result;
            }
            //
            // EF.Core
            //
            if ("HasOne".Equals(firstMethodNode.MethodName))
            {
                string InverseNavigationName = null;
                List<String> foreignKeyProps = null;
                List<String> principalKeyProps = null;
                bool IsOneToMany = false;
                bool IsWithOne = false;
                bool IsRequired = false;
                bool IsCascadeDelete = false;
                string GenericForeignKeyClassName = null;
                string GenericPrincipalKeyClassName = null;
                string deleteBehavior = "DeleteBehavior.NoAction";
                for (int i = 1; i < methodsCount; i++)
                {
                    FluentAPIMethodNode methodNode = entityNode.Methods[i];
                    if (string.IsNullOrEmpty(methodNode.MethodName)) return result;
                    if ("OnDelete".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments != null)
                        {
                            if (methodNode.MethodArguments.Contains("DeleteBehavior.Cascade"))
                            {
                                IsCascadeDelete = true;
                                deleteBehavior = "DeleteBehavior.Cascade";
                            }
                            else if (methodNode.MethodArguments.Contains("DeleteBehavior.ClientSetNull"))
                            {
                                deleteBehavior = "DeleteBehavior.ClientSetNull";
                            }
                            else if (methodNode.MethodArguments.Contains("DeleteBehavior.Restrict"))
                            {
                                deleteBehavior = "DeleteBehavior.Restrict";
                            }
                            else if (methodNode.MethodArguments.Contains("DeleteBehavior.SetNull"))
                            {
                                deleteBehavior = "DeleteBehavior.SetNull";
                            }
                            else if (methodNode.MethodArguments.Contains("DeleteBehavior.ClientCascade"))
                            {
                                deleteBehavior = "DeleteBehavior.ClientCascade";
                            }
                            else if (methodNode.MethodArguments.Contains("DeleteBehavior.NoAction"))
                            {
                                deleteBehavior = "DeleteBehavior.NoAction";
                            }
                            else if (methodNode.MethodArguments.Contains("DeleteBehavior.ClientNoAction"))
                            {
                                deleteBehavior = "DeleteBehavior.ClientNoAction";
                            }
                        }
                        continue;
                    }
                    if ("WithMany".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        InverseNavigationName = methodNode.MethodArguments[0];
                        IsOneToMany = true;
                        continue;
                    }
                    if ("WithOne".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        InverseNavigationName = methodNode.MethodArguments[0];
                        IsWithOne = true;
                        continue;
                    }
                    if ("HasForeignKey".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments == null) return result;
                        if (methodNode.MethodArguments.Count < 1) return result;
                        GenericForeignKeyClassName = methodNode.GenericName;
                        foreignKeyProps = methodNode.MethodArguments;
                        continue;
                    }
                    if ("HasPrincipalKey".Equals(methodNode.MethodName))
                    {
                        principalKeyProps = methodNode.MethodArguments;
                        GenericPrincipalKeyClassName = methodNode.GenericName;
                        continue;
                    }
                    if ("IsRequired".Equals(methodNode.MethodName))
                    {
                        if (methodNode.MethodArguments != null)
                        {
                            if (methodNode.MethodArguments.Count > 0)
                            {
                                if (bool.TryParse(methodNode.MethodArguments[0], out bool val))
                                {
                                    IsRequired = val;
                                }
                            }
                            else
                            {
                                IsRequired = true;
                            }
                        }
                        else
                        {
                            IsRequired = true;
                        }
                    }
                }
                // if both are equal false or both are equal true then ignore
                if (IsOneToMany == IsWithOne) return result;
                if (IsOneToMany)
                {
                    NavigationTypeEnum NavigationType = NavigationTypeEnum.OptionalToMany;
                    if (foreignKeyProps != null)
                    {
                        if (foreignKeyProps.Count > 0)
                        {
                            if (IsRequired)
                            {
                                NavigationType = NavigationTypeEnum.OneToMany;
                            }

                            result = new FluentAPIForeignKey()
                            {
                                NavigationName = firstMethodNode.MethodArguments[0],
                                InverseNavigationName = InverseNavigationName,
                                ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                                PrincipalKeySource = InfoSourceEnum.ByConvention,
                                InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                                NavigationType = NavigationType,
                                IsCascadeDelete = IsCascadeDelete,
                                DeleteBehavior = deleteBehavior,
                            };
                            result.ForeignKeyProps = new List<FluentAPIProperty>();
                            int ord = 0;
                            foreach (string prop in foreignKeyProps)
                            {
                                result.ForeignKeyProps.Add(
                                new FluentAPIProperty()
                                {
                                    PropName = prop,
                                    PropOrder = ord++
                                });
                            }
                        }
                    }
                    if (principalKeyProps != null)
                    {
                        if (principalKeyProps.Count > 0)
                        {
                            if (result == null)
                            {
                                if (IsRequired)
                                {
                                    NavigationType = NavigationTypeEnum.OneToMany;
                                }
                                result = new FluentAPIForeignKey()
                                {
                                    NavigationName = firstMethodNode.MethodArguments[0],
                                    InverseNavigationName = InverseNavigationName,
                                    ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                                    PrincipalKeySource = InfoSourceEnum.ByConvention,
                                    InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                                    NavigationType = NavigationType,
                                    IsCascadeDelete = IsCascadeDelete,
                                    DeleteBehavior = deleteBehavior,
                                };
                            }
                            result.PrincipalKeyProps = new List<FluentAPIProperty>();
                            int ord = 0;
                            foreach (string prop in principalKeyProps)
                            {
                                result.PrincipalKeyProps.Add(
                                new FluentAPIProperty()
                                {
                                    PropName = prop,
                                    PropOrder = ord++
                                });
                            }
                        }
                    }
                    return result;
                }
                if (IsWithOne)
                {
                    if (ignoreTheOpposite)
                    {
                        if (string.IsNullOrEmpty(entityNode.EntityName)) return null;
                        if (!string.IsNullOrEmpty(GenericPrincipalKeyClassName))
                        {
                            if ((GenericPrincipalKeyClassName == entityNode.EntityName) ||
                                GenericPrincipalKeyClassName.EndsWith("." + entityNode.EntityName) ||
                                entityNode.EntityName.EndsWith("." + GenericPrincipalKeyClassName)) return null;
                        }
                        if (!string.IsNullOrEmpty(GenericForeignKeyClassName))
                        {
                            if (!((GenericForeignKeyClassName == entityNode.EntityName) ||
                                GenericForeignKeyClassName.EndsWith("." + entityNode.EntityName) ||
                                entityNode.EntityName.EndsWith("." + GenericForeignKeyClassName))) return null;
                        }
                    }

                    NavigationTypeEnum NavigationType = NavigationTypeEnum.OptionalToOne;
                    if (IsRequired)
                    {
                        NavigationType = NavigationTypeEnum.OneToOne;
                    }


                    result = new FluentAPIForeignKey()
                    {
                        NavigationName = firstMethodNode.MethodArguments[0],
                        InverseNavigationName = InverseNavigationName,
                        GenericForeignKeyClassName = GenericForeignKeyClassName, //
                        ForeignKeySource = InfoSourceEnum.ByOnModelCreating,
                        PrincipalKeySource = InfoSourceEnum.ByConvention,
                        InverseNavigationSource = InfoSourceEnum.ByOnModelCreating,
                        NavigationType = NavigationType,
                        IsCascadeDelete = IsCascadeDelete,
                        DeleteBehavior = deleteBehavior,
                    };
                    if (principalKeyProps != null)
                    {
                        if (principalKeyProps.Count > 0)
                        {
                            result.PrincipalKeyProps = new List<FluentAPIProperty>(); // correct !!!
                            int ord = 0;
                            foreach (string prop in principalKeyProps)
                            {
                                result.PrincipalKeyProps.Add(
                                new FluentAPIProperty()
                                {
                                    PropName = prop,
                                    PropOrder = ord++
                                });
                            }
                        }
                    }
                    if (foreignKeyProps != null)
                    {
                        if (foreignKeyProps.Count > 0)
                        {
                            result.ForeignKeyProps = new List<FluentAPIProperty>();
                            int ord = 0;
                            foreach (string prop in foreignKeyProps)
                            {
                                result.ForeignKeyProps.Add(
                                new FluentAPIProperty()
                                {
                                    PropName = prop,
                                    PropOrder = ord++
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }
        public static List<FluentAPIForeignKey> CollectForeignKeysByDbContext(this CodeClass srcClass, CodeClass dbContext, bool ignoreTheOpposite = false)
        {
            List<FluentAPIForeignKey> result = null;
            if ((srcClass == null) || (dbContext == null)) return result;
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction == null) result = null;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        //   
                        // EF.Net
                        //

                        // or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasRequired",
                                }
                            }
                        },
                        // or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasOptional",
                                }
                            }
                        },

                        //   
                        // EF.Core
                        //


                        //or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasOne",
                                }
                            }
                        },
                        // or
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "HasMany",
                                }
                            }
                        }
                    };
            List<FluentAPIEntityNode> entityNodes =
                codeFunction.DoAnalyzeWithFilter(classNames, filter);
            if (entityNodes == null) return result;
            if (entityNodes.Count < 1) return result;

            result = new List<FluentAPIForeignKey>();
            foreach (FluentAPIEntityNode entityNode in entityNodes)
            {
                FluentAPIForeignKey foreignKey = entityNode.FluentAPIEntityNodeToForeignKey(ignoreTheOpposite);
                if (foreignKey != null)
                {
                    if (!string.IsNullOrEmpty(foreignKey.GenericForeignKeyClassName))
                    {
                        if ((!classNames[0].Equals(foreignKey.GenericForeignKeyClassName)) &&
                            (!classNames[1].Equals(foreignKey.GenericForeignKeyClassName)))
                        {
                            string str = foreignKey.InverseNavigationName;
                            foreignKey.InverseNavigationName = foreignKey.NavigationName;
                            foreignKey.NavigationName = str;
                            foreignKey.NavigationEntityName = classNames[0];
                            foreignKey.NavigationEntityFullName = classNames[1];
                        }
                        else
                        {
                            foreignKey.EntityName = classNames[0];
                            foreignKey.EntityFullName = classNames[1];
                        }
                    }
                    else
                    {
                        if (foreignKey.NavigationType == NavigationTypeEnum.OptionalToOne)
                        {
                            foreignKey.NavigationEntityName = classNames[0];
                            foreignKey.NavigationEntityFullName = classNames[1];
                        }
                        else
                        {
                            foreignKey.EntityName = classNames[0];
                            foreignKey.EntityFullName = classNames[1];
                        }
                    }
                    FluentAPIForeignKey existedForeignKey = null;
                    if (string.IsNullOrEmpty(foreignKey.EntityName))
                    {
                        existedForeignKey =
                            result.FirstOrDefault(r => (r.NavigationName == foreignKey.NavigationName) && (r.InverseNavigationName == foreignKey.InverseNavigationName) && (r.NavigationEntityName == foreignKey.NavigationEntityName));
                    }
                    else
                    {
                        existedForeignKey =
                            result.FirstOrDefault(r => (r.NavigationName == foreignKey.NavigationName) && (r.InverseNavigationName == foreignKey.InverseNavigationName) && (r.EntityName == foreignKey.EntityName));
                    }
                    if (existedForeignKey != null)
                    {
                        existedForeignKey.SourceCount += 1;
                    }
                    else
                    {
                        foreignKey.ForeignKeySource = InfoSourceEnum.ByOnModelCreating;
                        foreignKey.InverseNavigationSource = InfoSourceEnum.ByOnModelCreating;
                        foreignKey.PrincipalKeySource = InfoSourceEnum.ByOnModelCreating;
                        result.Add(foreignKey);
                    }
                }
            }
            return result;
        }
        // ready
        public static List<FluentAPIForeignKey> CollectForeignKeys(this CodeClass srcClass, CodeClass dbContext, List<string> propNameFilter = null, bool ignoreTheOpposite = false)
        {
            List<FluentAPIForeignKey> result = null;
            List<FluentAPIEntityNode> ignoreNodes = null;
            if (srcClass == null) return result;
            bool isCollectedByDbContext = false;
            result = srcClass.CollectForeignKeysByDbContext(dbContext, ignoreTheOpposite);
            if (result != null)
            {
                isCollectedByDbContext = result.Count > 0;
            }
            FluentAPIKey srcPrimKey = null;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            if (dbContext != null)
            {
                CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
                if (codeFunction != null)
                {
                    List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "Ignore"
                                }
                            }
                        }
                    };
                    ignoreNodes = codeFunction.DoAnalyzeWithFilter(classNames, filter);
                }
            }
            foreach (CodeElement codeElement in srcClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                string codePropertyName = codeProperty.Name;
                if ((propNameFilter != null) && (!isCollectedByDbContext)) // (!isCollectedByDbContext) to remove from the result in the code below
                {
                    if (!propNameFilter.Contains(codePropertyName)) continue;
                }
                if (codeProperty.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (codeProperty.Type == null) continue;
                if (codeProperty.Type.CodeType == null) continue;
                CodeTypeRef codeTypeRef = codeProperty.Type;
                if (codeTypeRef.IsScalarTypeOrString()) continue;
                if (codeTypeRef.TypeKind != vsCMTypeRef.vsCMTypeRefCodeType) continue;
                //if (codeTypeRef.AsFullName.StartsWith("System.Nullable")) continue;
                if (codeTypeRef.CodeType.Kind == vsCMElement.vsCMElementInterface) continue;
                if (codeTypeRef.CodeType.Kind != vsCMElement.vsCMElementClass) continue;
                string codeTypeRefFullName = codeTypeRef.AsFullName;
                if (codeTypeRefFullName.StartsWith("System.Collections.Generic.List<")) continue;
                if (codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute") != null) continue;

                CodeProperty masterCodeProperty = null;
                CodeClass masterCodeClass = codeProperty.Type.CodeType as CodeClass;

                if (isCollectedByDbContext)
                {
                    if (ignoreNodes != null)
                    {
                        if (ignoreNodes.HoldsIgnore(codePropertyName)) continue;
                    }

                    FluentAPIForeignKey existedForeignKey =
                    result.FirstOrDefault(r =>
                        (r.NavigationName == codePropertyName) && (r.EntityName == classNames[0]) ||
                        (r.InverseNavigationName == codePropertyName) && (r.NavigationEntityName == classNames[0]));



                    if (existedForeignKey != null)
                    {
                        if (propNameFilter != null)
                        {
                            if (!propNameFilter.Contains(codePropertyName))
                            {
                                result.Remove(existedForeignKey);
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(existedForeignKey.EntityName))
                        {
                            existedForeignKey.EntityName = masterCodeClass.Name;
                            existedForeignKey.EntityFullName = masterCodeClass.FullName;
                            existedForeignKey.CodeElementEntityRef = masterCodeClass as CodeElement;
                            existedForeignKey.CodeElementNavigationRef = srcClass as CodeElement;
                            if (srcPrimKey == null)
                            {
                                srcPrimKey = new FluentAPIKey();
                                srcClass.CollectPrimaryKeyPropsHelper(srcPrimKey, dbContext);
                            }
                            if (srcPrimKey.KeyProperties != null)
                            {
                                if (srcPrimKey.KeyProperties.Count > 0)
                                {
                                    existedForeignKey.PrincipalKeyProps = srcPrimKey.KeyProperties;
                                    existedForeignKey.PrincipalKeySource = srcPrimKey.KeySource;
                                    existedForeignKey.PrincipalKeySourceCount = srcPrimKey.SourceCount;
                                }
                            }
                            bool isNotDefined = existedForeignKey.ForeignKeyProps == null;
                            if (!isNotDefined)
                            {
                                isNotDefined = existedForeignKey.ForeignKeyProps.Count < 1;
                            }
                            if (isNotDefined)
                            {
                                FluentAPIKey locPrimKey = new FluentAPIKey();
                                masterCodeClass.CollectPrimaryKeyPropsHelper(locPrimKey, dbContext);
                                if (locPrimKey.KeyProperties != null)
                                {
                                    if (locPrimKey.KeyProperties.Count > 0)
                                    {
                                        existedForeignKey.ForeignKeyProps = locPrimKey.KeyProperties;
                                        existedForeignKey.ForeignKeySource = locPrimKey.KeySource;
                                        existedForeignKey.ForeignKeySourceCount = locPrimKey.SourceCount;
                                    }
                                }
                            }
                        }
                        else
                        {
                            existedForeignKey.NavigationEntityName = masterCodeClass.Name;
                            existedForeignKey.NavigationEntityFullName = masterCodeClass.FullName;
                            existedForeignKey.CodeElementNavigationRef = masterCodeClass as CodeElement;
                            existedForeignKey.CodeElementEntityRef = srcClass as CodeElement;
                            bool isNotDefined = existedForeignKey.PrincipalKeyProps == null;
                            if (!isNotDefined)
                            {
                                isNotDefined = existedForeignKey.PrincipalKeyProps.Count < 1;
                            }
                            FluentAPIKey locPrimKey = new FluentAPIKey();
                            masterCodeClass.CollectPrimaryKeyPropsHelper(locPrimKey, dbContext);
                            if (isNotDefined)
                            {
                                if (locPrimKey.KeyProperties != null)
                                {
                                    if (locPrimKey.KeyProperties.Count > 0)
                                    {
                                        existedForeignKey.PrincipalKeyProps = locPrimKey.KeyProperties;
                                        existedForeignKey.PrincipalKeySource = locPrimKey.KeySource;
                                        existedForeignKey.PrincipalKeySourceCount = locPrimKey.SourceCount;
                                    }
                                }
                            }
                            else
                            {
                                bool isIdentical = false;
                                if (locPrimKey.KeyProperties != null)
                                {
                                    if (locPrimKey.KeyProperties.Count > 0)
                                    {
                                        isIdentical = locPrimKey.IsTheListOfNamesIdentical(existedForeignKey.PrincipalKeyProps);
                                        if (isIdentical)
                                        {
                                            existedForeignKey.PrincipalKeyProps = locPrimKey.KeyProperties;
                                            existedForeignKey.PrincipalKeySource = locPrimKey.KeySource;
                                            existedForeignKey.PrincipalKeySourceCount = locPrimKey.SourceCount;
                                        }
                                    }
                                }
                                if (!isIdentical)
                                {
                                    List<FluentAPIKey> masterUniqueKeys = new List<FluentAPIKey>();
                                    masterCodeClass.CollectAllUniqueKeysHelper(masterUniqueKeys, dbContext);
                                    FluentAPIKey masterUniqueKey = masterUniqueKeys.GetFluentAPIKeyWithIdenticalListOfNames(existedForeignKey.PrincipalKeyProps);
                                    if (masterUniqueKey != null)
                                    {
                                        existedForeignKey.PrincipalKeyProps = masterUniqueKey.KeyProperties;
                                        existedForeignKey.PrincipalKeySource = masterUniqueKey.KeySource;
                                        existedForeignKey.PrincipalKeySourceCount = masterUniqueKey.SourceCount;
                                    }
                                    else
                                    {

                                        existedForeignKey.PrincipalKeySourceCount = 0;
                                        existedForeignKey.HasErrors = true;
                                        if (string.IsNullOrEmpty(existedForeignKey.ErrorsText)) existedForeignKey.ErrorsText = "";
                                        existedForeignKey.ErrorsText += "For the PrincipalKeyProps \r\n {";
                                        string prfx = "";
                                        foreach (FluentAPIProperty pkp in existedForeignKey.PrincipalKeyProps)
                                        {
                                            existedForeignKey.ErrorsText += prfx + pkp.PropName;
                                            prfx = ", ";
                                        }
                                        existedForeignKey.ErrorsText += "}\r\n Could not find Prim or Uniq key";
                                        existedForeignKey.PrincipalKeyProps = null;
                                    }
                                }
                            }


                            isNotDefined = existedForeignKey.ForeignKeyProps == null;
                            if (!isNotDefined)
                            {
                                isNotDefined = existedForeignKey.ForeignKeyProps.Count < 1;
                            }
                            if (isNotDefined)
                            {
                                if (srcPrimKey == null)
                                {
                                    srcPrimKey = new FluentAPIKey();
                                    srcClass.CollectPrimaryKeyPropsHelper(srcPrimKey, dbContext);
                                }

                                if (srcPrimKey.KeyProperties != null)
                                {
                                    if (srcPrimKey.KeyProperties.Count > 0)
                                    {
                                        existedForeignKey.ForeignKeyProps = srcPrimKey.KeyProperties;
                                        existedForeignKey.ForeignKeySource = srcPrimKey.KeySource;
                                        existedForeignKey.ForeignKeySourceCount = srcPrimKey.SourceCount;
                                    }
                                }
                            }

                        }
                        continue;
                    }
                }


                FluentAPIKey masterPrimKey = new FluentAPIKey();
                masterCodeClass.CollectPrimaryKeyPropsHelper(masterPrimKey, dbContext);
                InfoSourceEnum InverseNavigationSource = InfoSourceEnum.ByConvention;

                FluentAPIKey foreignKey = codeProperty.CollectForeignKeyByAttributes();

                if (foreignKey == null)
                {
                    foreignKey = codeProperty.CollectForeignKeyByConventions(masterPrimKey, dbContext);
                }


                masterCodeProperty =
                    masterCodeClass.GetPublicMappedNonScalarPropertyByAttributeArgument("System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute", codeElement.Name);
                if (masterCodeProperty == null)
                {
                    CodeAttribute codeAttribute = codeProperty.GetCodePropertyAttributeByFullName("System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute");
                    if (codeAttribute != null)
                    {
                        masterCodeProperty = masterCodeClass.GetPublicMappedNonScalarPropertyByName(codeAttribute.GetAttributeArgument());
                    }
                }
                if (masterCodeProperty == null)
                {
                    List<CodeProperty> masterCodeProperties =
                        masterCodeClass.GetPublicMappedNonScalarPropertiesByTypeFullName(srcClass.FullName);
                    if (masterCodeProperties != null)
                    {
                        if (masterCodeProperties.Count == 1)
                        {
                            masterCodeProperty = masterCodeProperties[0];
                        }
                    }
                }
                else InverseNavigationSource = InfoSourceEnum.ByAttribute;
                if (masterCodeProperty == null)
                {
                    FluentAPIForeignKey errorfluentAPIForeignKey = new FluentAPIForeignKey();
                    errorfluentAPIForeignKey.DefineFluentAPIForeignKey(srcClass, masterCodeClass, foreignKey, masterPrimKey);
                    errorfluentAPIForeignKey.NavigationName = codeElement.Name;


                    errorfluentAPIForeignKey.HasErrors = true;
                    errorfluentAPIForeignKey.ErrorsText += "No Inverse Navigation found";

                    if (result == null) result = new List<FluentAPIForeignKey>();
                    result.Add(errorfluentAPIForeignKey);
                    continue;
                }


                if (foreignKey == null)
                {
                    if (!masterCodeProperty.IsOfCollectionType())
                    {
                        FluentAPIKey masterForeignKey = masterCodeProperty.CollectForeignKeyByAttributes();
                        FluentAPIKey deatilPrimKey = new FluentAPIKey();
                        srcClass.CollectPrimaryKeyPropsHelper(deatilPrimKey, dbContext);
                        if (masterForeignKey == null)
                        {
                            masterForeignKey = masterCodeProperty.CollectForeignKeyByConventions(deatilPrimKey, dbContext);
                        }
                        if (masterForeignKey != null)
                        {
                            FluentAPIForeignKey masterFluentAPIForeignKey = new FluentAPIForeignKey();
                            masterFluentAPIForeignKey.DefineFluentAPIForeignKey(masterCodeClass, srcClass, masterForeignKey, deatilPrimKey);
                            masterFluentAPIForeignKey.NavigationName = masterCodeProperty.Name;
                            masterFluentAPIForeignKey.InverseNavigationSource = InverseNavigationSource;

                            codeProperty.DefineNavigationType(masterFluentAPIForeignKey, masterCodeClass);

                            if (result == null) result = new List<FluentAPIForeignKey>();
                            result.Add(masterFluentAPIForeignKey);
                            continue;
                        }
                    }
                }

                FluentAPIForeignKey fluentAPIForeignKey = new FluentAPIForeignKey();
                fluentAPIForeignKey.NavigationName = codeElement.Name;

                fluentAPIForeignKey.EntityName = srcClass.Name;
                fluentAPIForeignKey.EntityFullName = srcClass.FullName;
                fluentAPIForeignKey.CodeElementEntityRef = srcClass as CodeElement;

                fluentAPIForeignKey.NavigationEntityName = masterCodeClass.Name;
                fluentAPIForeignKey.NavigationEntityFullName = masterCodeClass.FullName;
                fluentAPIForeignKey.CodeElementNavigationRef = masterCodeClass as CodeElement;

                fluentAPIForeignKey.PrincipalKeySource = masterPrimKey.KeySource;
                fluentAPIForeignKey.PrincipalKeySourceCount = masterPrimKey.SourceCount;
                if (masterPrimKey.KeyProperties != null)
                {
                    if (masterPrimKey.KeyProperties.Count > 0)
                    {
                        fluentAPIForeignKey.PrincipalKeyProps = masterPrimKey.KeyProperties;
                    }
                }
                if (foreignKey != null)
                {
                    fluentAPIForeignKey.ForeignKeySource = foreignKey.KeySource;
                    fluentAPIForeignKey.ForeignKeySourceCount = foreignKey.SourceCount;
                    if (foreignKey.KeyProperties != null)
                    {
                        fluentAPIForeignKey.ForeignKeyProps = foreignKey.KeyProperties;
                    }
                }
                if (masterCodeProperty != null)
                {
                    masterCodeProperty.DefineNavigationType(fluentAPIForeignKey, srcClass);
                    fluentAPIForeignKey.InverseNavigationSource = InverseNavigationSource;
                }

                if (result == null) result = new List<FluentAPIForeignKey>();
                result.Add(fluentAPIForeignKey);

            }
            if (result != null)
            {
                result.ForEach(itm => itm.DefineErrorFlag());
            }
            return result;
        }
        public static ModelView DefineAttributesForScalarProperties(this CodeClass srcClass, ModelView SelectedModel)
        {
            if ((srcClass == null) || (SelectedModel == null)) return SelectedModel;
            if (SelectedModel.ScalarProperties == null) return SelectedModel;

            foreach (CodeElement codeElement in srcClass.Members)
            {
                if (codeElement.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty codeProperty = codeElement as CodeProperty;
                ModelViewProperty modelViewProperty = SelectedModel.ScalarProperties.FirstOrDefault(sp => sp.OriginalPropertyName == codeProperty.Name);
                if (modelViewProperty == null) continue;
                foreach (CodeElement attrCodeElement in codeProperty.Attributes)
                {
                    if (!(attrCodeElement is CodeAttribute)) continue;
                    CodeAttribute codeAttribute = attrCodeElement as CodeAttribute;
                    if (modelViewProperty.Attributes == null)
                    {
                        modelViewProperty.Attributes = new ObservableCollection<ModelViewAttribute>();
                    }
                    ModelViewAttribute modelViewAttribute = new ModelViewAttribute()
                    {
                        AttrName = codeAttribute.Name,
                        AttrFullName = codeAttribute.FullName
                    };
                    modelViewProperty.Attributes.Add(modelViewAttribute);
                    foreach (CodeElement chld in codeAttribute.Children)
                    {
                        if (!(chld is CodeAttributeArgument)) continue;
                        CodeAttributeArgument codeAttributeArgument = chld as CodeAttributeArgument;
                        if (modelViewAttribute.VaueProperties == null)
                        {
                            modelViewAttribute.VaueProperties = new ObservableCollection<ModelViewAttributeProperty>();
                        }
                        ModelViewAttributeProperty modelViewAttributePropert = new ModelViewAttributeProperty()
                        {
                            PropName = codeAttributeArgument.Name,
                            PropValue = codeAttributeArgument.Value
                        };
                        if (string.IsNullOrEmpty(modelViewAttributePropert.PropName))
                        {
                            modelViewAttributePropert.PropName = "...";
                        }
                        modelViewAttribute.VaueProperties.Add(modelViewAttributePropert);
                    }
                }
            }
            return SelectedModel;
        }
        public static ModelView DefineFAPIAttributesForScalarProperties(this CodeClass srcClass, ModelView SelectedModel, CodeClass dbContext)
        {
            if ((srcClass == null) || (SelectedModel == null) || (dbContext == null)) return SelectedModel;
            if (SelectedModel.ScalarProperties == null) return SelectedModel;
            if (SelectedModel.ScalarProperties.Count < 1) return SelectedModel;
            string[] classNames = new string[] { srcClass.Name, srcClass.FullName };
            CodeFunction codeFunction = dbContext.GetCodeFunctionByName(vsCMAccess.vsCMAccessProtected, "OnModelCreating");
            if (codeFunction == null) return SelectedModel;
            List<FluentAPIEntityNode> filter = new List<FluentAPIEntityNode>()
                    {
                        new FluentAPIEntityNode()
                        {
                            Methods = new List<FluentAPIMethodNode>()
                            {
                                new FluentAPIMethodNode() {
                                    MethodName = "Property"
                                }
                            }
                        }
                    };
            List<FluentAPIEntityNode> entityNodes = codeFunction.DoAnalyzeWithFilter(classNames, filter);
            if (entityNodes == null) return SelectedModel;
            if (entityNodes.Count < 1) return SelectedModel;
            foreach (FluentAPIEntityNode fluentAPIEntityNode in entityNodes)
            {
                if (fluentAPIEntityNode.Methods == null) continue;
                int methodsCount = fluentAPIEntityNode.Methods.Count;
                if (methodsCount < 2) continue;
                FluentAPIMethodNode fluentAPIMethodNode = fluentAPIEntityNode.Methods[0]; // Property - node
                if (fluentAPIMethodNode.MethodArguments == null) continue;
                if (fluentAPIMethodNode.MethodArguments.Count < 1) continue;
                string propName = fluentAPIMethodNode.MethodArguments[0];
                ModelViewProperty modelViewProperty = SelectedModel.ScalarProperties.FirstOrDefault(sp => sp.OriginalPropertyName == propName);
                if (modelViewProperty == null) continue;
                if (modelViewProperty.FAPIAttributes == null) modelViewProperty.FAPIAttributes = new ObservableCollection<ModelViewFAPIAttribute>();
                for (int i = 1; i < methodsCount; i++)
                {
                    ModelViewFAPIAttribute modelViewFAPIAttribute = new ModelViewFAPIAttribute()
                    {
                        AttrName = fluentAPIEntityNode.Methods[i].MethodName
                    };
                    modelViewProperty.FAPIAttributes.Add(modelViewFAPIAttribute);
                    if (fluentAPIEntityNode.Methods[i].MethodArguments == null) continue;
                    if (fluentAPIEntityNode.Methods[i].MethodArguments.Count < 1) continue;
                    modelViewFAPIAttribute.VaueProperties = new ObservableCollection<ModelViewFAPIAttributeProperty>();
                    foreach (string methodArgument in fluentAPIEntityNode.Methods[i].MethodArguments)
                    {
                        modelViewFAPIAttribute.VaueProperties.Add(new ModelViewFAPIAttributeProperty()
                        {
                            PropValue = methodArgument
                        });
                    }
                }
            }
            return SelectedModel;
        }
        public static ModelView DefineModelView(this CodeClass srcClass, CodeClass dbContext, ModelView SelectedModel, string viewSufix = null, string pageViewNameSufix = null)
        {
            if ((srcClass == null) || (dbContext == null) || (SelectedModel == null)) return null;
            SelectedModel.ClearModelView();
            string srcClassFullName = srcClass.FullName;
            string uniqueProjectName = null;
            if (srcClass.ProjectItem != null)
            {
                if (srcClass.ProjectItem.ContainingProject != null)
                {
                    uniqueProjectName = srcClass.ProjectItem.ContainingProject.UniqueName;
                }
            }

            FluentAPIKey primKey = new FluentAPIKey();
            srcClass.CollectPrimaryKeyPropsHelper(primKey, dbContext);

            List<FluentAPIExtendedProperty> properties = new List<FluentAPIExtendedProperty>();
            srcClass.CollectCodeClassAllMappedScalarPropertiesWithDbContext(properties, null, dbContext);

            properties.Sort((a, b) => a.PropOrder - b.PropOrder);
            if (SelectedModel.ScalarProperties == null)
            {
                SelectedModel.ScalarProperties = new ObservableCollection<ModelViewProperty>();
            }
            else
            {
                SelectedModel.ScalarProperties.Clear();
            }
            if (properties.Count > 0)
            {
                properties.ForEach(prop => {
                    SelectedModel.ScalarProperties.Add(new ModelViewProperty()
                    {
                        OriginalPropertyName = prop.PropName,
                        TypeFullName = prop.TypeFullName,
                        IsNullable = prop.IsNullable,
                        IsRequired = prop.IsRequired,
                        IsRequiredInView = prop.IsRequired,
                        UnderlyingTypeName = prop.UnderlyingTypeName,
                        ViewPropertyName = prop.PropName,
                        EditableViewPropertyName = prop.PropName,
                        JsonPropertyName = prop.PropName.FirstLetterToLower(),
                        EditableJsonPropertyName = prop.PropName.FirstLetterToLower(),
                    });

                });
                srcClass.DefineAttributesForScalarProperties(SelectedModel);
                srcClass.DefineFAPIAttributesForScalarProperties(SelectedModel, dbContext);
                foreach (ModelViewProperty modelViewProperty in SelectedModel.ScalarProperties)
                {
                    if (modelViewProperty.FAPIAttributes == null) continue;
                    if (modelViewProperty.FAPIAttributes.Count < 1) continue;

                    if (modelViewProperty.FAPIAttributes.Any(a => a.AttrName == "IsOptional"))
                    {
                        modelViewProperty.IsRequired = false;
                        modelViewProperty.IsRequiredInView = false;
                    }
                    ModelViewFAPIAttribute isReqAttr = modelViewProperty.FAPIAttributes.Where(a => a.AttrName == "IsRequired").FirstOrDefault();
                    //if (modelViewProperty.FAPIAttributes.Any(a => a.AttrName == "IsRequired"))
                    if (isReqAttr != null)
                    {
                        if (isReqAttr.VaueProperties.Any(a => ((a.PropValue == "false") || (a.PropValue == "False") || (a.PropValue == "\"" + "false" + "\""))))
                        {
                            modelViewProperty.IsRequired = false;
                            modelViewProperty.IsRequiredInView = false;
                        }
                        else
                        {
                            modelViewProperty.IsRequired = true;
                            modelViewProperty.IsRequiredInView = true;
                        }
                    }
                }
            }

            if (SelectedModel.PrimaryKeyProperties == null)
            {
                SelectedModel.PrimaryKeyProperties = new ObservableCollection<ModelViewKeyProperty>();
            }
            else
            {
                SelectedModel.PrimaryKeyProperties.Clear();
            }
            if (primKey.KeyProperties != null)
            {
                if (primKey.KeyProperties.Count > 0)
                {
                    primKey.KeyProperties.ForEach(primKeyProp =>
                    {
                        ModelViewProperty pkp =
                            SelectedModel.ScalarProperties.FirstOrDefault(sp => sp.OriginalPropertyName == primKeyProp.PropName);
                        if (pkp != null)
                        {
                            SelectedModel.PrimaryKeyProperties.Add(new ModelViewKeyProperty()
                            {
                                OriginalPropertyName = pkp.OriginalPropertyName,
                                TypeFullName = pkp.TypeFullName,
                                IsNullable = pkp.IsNullable,
                                IsRequired = pkp.IsRequired,
                                UnderlyingTypeName = pkp.UnderlyingTypeName,
                                ViewPropertyName = pkp.ViewPropertyName
                            });
                        }
                    });
                }
            }

            if (SelectedModel.ForeignKeys == null)
            {
                SelectedModel.ForeignKeys = new ObservableCollection<ModelViewForeignKey>();
            }
            else
            {
                SelectedModel.ForeignKeys.Clear();
            }
            List<FluentAPIForeignKey> foreignKeys =
                srcClass.CollectForeignKeys(dbContext, null);
            if (foreignKeys != null)
            {
                if (foreignKeys.Count > 0)
                {
                    foreach (FluentAPIForeignKey foreignKey in foreignKeys)
                    {
                        if (foreignKey.HasErrors) continue;
                        if (!srcClassFullName.Equals(foreignKey.EntityFullName)) continue;
                        string containingProjectUniqueName = null;
                        CodeClass masterEntity = foreignKey.CodeElementNavigationRef as CodeClass;
                        if (masterEntity != null)
                        {
                            if (masterEntity.ProjectItem != null)
                            {
                                if (masterEntity.ProjectItem.ContainingProject != null)
                                {
                                    containingProjectUniqueName = masterEntity.ProjectItem.ContainingProject.UniqueName;
                                }
                            }
                        }



                        ModelViewForeignKey destForeignKey = new ModelViewForeignKey(true)
                        {
                            NavigationName = foreignKey.NavigationName,
                            InverseNavigationName = foreignKey.InverseNavigationName,
                            EntityName = foreignKey.EntityName,
                            EntityFullName = foreignKey.EntityFullName,
                            EntityUniqueProjectName = uniqueProjectName,

                            NavigationEntityName = foreignKey.NavigationEntityName,
                            NavigationEntityFullName = foreignKey.NavigationEntityFullName,
                            NavigationEntityUniqueProjectName = containingProjectUniqueName,
                            NavigationType = foreignKey.NavigationType,

                            ForeignKeySource = foreignKey.ForeignKeySource,
                            PrincipalKeySource = foreignKey.PrincipalKeySource,
                            InverseNavigationSource = foreignKey.InverseNavigationSource,
                            IsCascadeDelete = foreignKey.IsCascadeDelete,
                        };
                        destForeignKey.ViewName = "";
                        destForeignKey.ForeignKeyPrefix = "";
                        destForeignKey.IsAssinging = false;
                        if (foreignKey.ForeignKeyProps != null)
                        {
                            if (foreignKey.ForeignKeyProps.Count > 0)
                            {
                                destForeignKey.ForeignKeyProps = new List<ModelViewKeyProperty>();
                                foreignKey.ForeignKeyProps.ForEach(fkp =>
                                {
                                    ModelViewProperty pkp =
                                        SelectedModel.ScalarProperties.FirstOrDefault(sp => sp.OriginalPropertyName == fkp.PropName);
                                    if (pkp != null)
                                    {
                                        destForeignKey.ForeignKeyProps.Add(new ModelViewKeyProperty()
                                        {
                                            OriginalPropertyName = pkp.OriginalPropertyName,
                                            TypeFullName = pkp.TypeFullName,
                                            IsNullable = pkp.IsNullable,
                                            IsRequired = pkp.IsRequired,
                                            UnderlyingTypeName = pkp.UnderlyingTypeName,
                                            ViewPropertyName = pkp.ViewPropertyName
                                        });
                                    }
                                    else
                                    {
                                        destForeignKey.ForeignKeyProps.Add(new ModelViewKeyProperty()
                                        {
                                            OriginalPropertyName = fkp.PropName,
                                            ViewPropertyName = fkp.PropName
                                        });
                                    }
                                });
                            }
                        }
                        if (foreignKey.PrincipalKeyProps != null)
                        {
                            if (foreignKey.PrincipalKeyProps.Count > 0)
                            {
                                List<FluentAPIExtendedProperty> masterKeys = null;
                                if (masterEntity != null)
                                {
                                    masterKeys = new List<FluentAPIExtendedProperty>();
                                    masterEntity.CollectCodeClassAllMappedScalarPropertiesWithDbContext(masterKeys, foreignKey.PrincipalKeyProps, dbContext);
                                }
                                destForeignKey.PrincipalKeyProps = new List<ModelViewKeyProperty>();
                                foreignKey.PrincipalKeyProps.ForEach(fkp =>
                                {
                                    FluentAPIExtendedProperty pkp = null;
                                    if (masterKeys != null)
                                    {
                                        pkp = masterKeys.FirstOrDefault(sp => sp.PropName == fkp.PropName);
                                    }
                                    if (pkp != null)
                                    {
                                        destForeignKey.PrincipalKeyProps.Add(new ModelViewKeyProperty()
                                        {
                                            OriginalPropertyName = pkp.PropName,
                                            TypeFullName = pkp.TypeFullName,
                                            IsNullable = pkp.IsNullable,
                                            IsRequired = pkp.IsRequired,
                                            UnderlyingTypeName = pkp.UnderlyingTypeName,
                                            ViewPropertyName = pkp.PropName
                                        });
                                    }
                                    else
                                    {
                                        destForeignKey.PrincipalKeyProps.Add(new ModelViewKeyProperty()
                                        {
                                            OriginalPropertyName = fkp.PropName,
                                            ViewPropertyName = fkp.PropName
                                        });
                                    }
                                });

                            }
                        }
                        SelectedModel.ForeignKeys.Add(destForeignKey);
                    }
                }
            }
            SelectedModel.RootEntityClassName = srcClass.Name;
            SelectedModel.RootEntityFullClassName = srcClassFullName;
            SelectedModel.RootEntityUniqueProjectName = uniqueProjectName;
            SelectedModel.ViewName = srcClass.Name + viewSufix;
            SelectedModel.PageViewName = srcClass.Name + pageViewNameSufix;

            SelectedModel.PluralTitle = SelectedModel.ViewName + "s";
            SelectedModel.Title = SelectedModel.ViewName;


            if (SelectedModel.AllProperties == null)
            {
                SelectedModel.AllProperties = new ObservableCollection<ModelViewEntityProperty>();
            }
            else
            {
                SelectedModel.AllProperties.Clear();
            }
            foreach (ModelViewProperty scalarProperty in SelectedModel.ScalarProperties)
            {
                SelectedModel.AllProperties.Add(new ModelViewEntityProperty()
                {
                    OriginalPropertyName = scalarProperty.OriginalPropertyName,
                    TypeFullName = scalarProperty.TypeFullName,
                    IsNullable = scalarProperty.IsNullable,
                    IsRequired = scalarProperty.IsRequired,
                    UnderlyingTypeName = scalarProperty.UnderlyingTypeName,
                    ViewPropertyName = scalarProperty.ViewPropertyName,
                    JsonPropertyName = scalarProperty.JsonPropertyName,
                    Attributes = scalarProperty.Attributes.CloneModelViewAttributeCollection(),
                    FAPIAttributes = scalarProperty.FAPIAttributes.CloneModelViewFAPIAttributeCollection()
                });
            }
            if (SelectedModel.UniqueKeys == null)
            {
                SelectedModel.UniqueKeys = new ObservableCollection<ModelViewUniqueKey>();
            }
            else
            {
                SelectedModel.UniqueKeys.Clear();
            }
            IList<FluentAPIKey> UniqueKeys = new List<FluentAPIKey>();
            srcClass.CollectAllUniqueKeysHelper(UniqueKeys, dbContext);
            foreach (FluentAPIKey key in UniqueKeys)
            {
                ModelViewUniqueKey modelViewUniqueKey = new ModelViewUniqueKey();
                modelViewUniqueKey.IsPrimary = key.IsPrimary;
                modelViewUniqueKey.UniqueKeyName = key.KeyName;
                modelViewUniqueKey.KeySource = key.KeySource;
                if (key.KeyProperties != null)
                {
                    if (modelViewUniqueKey.UniqueKeyProperties == null) modelViewUniqueKey.UniqueKeyProperties = new List<ModelViewKeyProperty>();
                    foreach (var property in key.KeyProperties)
                    {
                        ModelViewProperty pkp =
                            SelectedModel.ScalarProperties.FirstOrDefault(sp => sp.OriginalPropertyName == property.PropName);
                        if (pkp != null)
                        {
                            modelViewUniqueKey.UniqueKeyProperties.Add(new ModelViewKeyProperty()
                            {
                                OriginalPropertyName = pkp.OriginalPropertyName,
                                TypeFullName = pkp.TypeFullName,
                                IsNullable = pkp.IsNullable,
                                IsRequired = pkp.IsRequired,
                                UnderlyingTypeName = pkp.UnderlyingTypeName,
                                ViewPropertyName = pkp.ViewPropertyName
                            });
                        }
                    }
                }
                SelectedModel.UniqueKeys.Add(modelViewUniqueKey);
            }
            return SelectedModel;
        }
    }
}


