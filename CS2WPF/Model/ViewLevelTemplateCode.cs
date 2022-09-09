using CS2WPF.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CS2WPF.Model
{
    public class ViewLevelTemplateCode
    {
        //
        // GetModelPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        //      ===
        //      GetTypeScriptPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        //
        // GetPropertyTypeName(ModelViewPropertyOfVwSerializable prop)
        //      ==
        //      GetPropertyTypeScriptTypeName(ModelViewPropertyOfVwSerializable prop)
        //
        // GetModelPropertyNameByKeyProperty(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        //      =
        //      GetTypeScriptPropertyNameByKeyProperty(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        //
        // GetModelPropertyNameWithSufix(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        //      =
        //      GetTypeScriptPropertyNameWithSufix(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        //


        string GetModelName(ModelViewSerializable model)
        {
            return FirstLetterToUpper(model.ViewName);
        }
        string GetInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelName(model);
        }
        string GetModelNameSpace(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            string delim = "";
            string filePath = "";
            string defaultNameSpace = "";
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                filePath = refItem.FileFolder.Replace("\\", ".");
            }
            if (!string.IsNullOrEmpty(refItem.FileDefaultProjectNameSpace))
            {
                defaultNameSpace = refItem.FileDefaultProjectNameSpace;
            }
            if (!(string.IsNullOrEmpty(defaultNameSpace) || string.IsNullOrEmpty(filePath)))
            {
                delim = ".";
            }
            return defaultNameSpace + delim + filePath;
        }
        string GetModelClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName;
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

            }
            return sb.ToString().Replace(".xaml", "").Replace(".Xaml", "").Replace(".XAML", "");
        }
        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }
        public string FirstLetterToLower(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);
            return str.ToUpper();
        }
        string GetModelPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return "Noname";
            }
            if (model.GenerateJSonAttribute)
            {
                return prop.ViewPropertyName;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName);
            }
        }
        string AttribToString(ModelViewAttributeSerializable attr)
        {
            if (attr == null) return "";
            string result = "[" + attr.AttrName;
            if (attr.VaueProperties == null)
            {
                return result + "]";
            }
            if (attr.VaueProperties.Count < 1)
            {
                return result + "]";
            }
            result = result + "(";
            bool insComma = false;
            foreach (ModelViewAttributePropertySerializable valProp in attr.VaueProperties)
            {
                if (insComma)
                {
                    result = result + ",";
                }
                else
                {
                    insComma = true;
                }
                if (!string.IsNullOrEmpty(valProp.PropName))
                {
                    if (!valProp.PropName.Contains("..."))
                    {
                        result = result + valProp.PropName + "=";
                    }
                }
                result = result + valProp.PropValue;
            }
            return result + ")]";
        }
        string GetPropertyTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return prop.UnderlyingTypeName;
            }
            if (prop.IsNullable || (!prop.IsRequiredInView))
            {
                return prop.UnderlyingTypeName + " ?";
            }
            return prop.UnderlyingTypeName;
        }
        string GetPropertyConvertMethod(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model) {
            switch(GetCCharpDatatype(prop, model))
            {
                case "int16":
                    return "Convert.ToInt16";
                case "int32":
                    return "Convert.ToInt32";
                case "int64":
                    return "Convert.ToInt64";
                case "uint16":
                    return "Convert.ToUInt16";
                case "uint32":
                    return "Convert.ToUInt32";
                case "uint64":
                    return "Convert.ToUInt64";
                case "double":
                    return "Convert.ToDouble";
                case "decimal":
                    return "Convert.ToDecimal";
                case "single":
                    return "Convert.ToSingle";
                case "guid":
                    return "Guid.Parse";
                case "date":
                case "datetime":
                    return "Convert.ToDateTime";
                default:
                    return "Convert.ToString";
            }
        }
        string GetPropertyConvertMethodEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetPropertyConvertMethod(sclrProp, model);
        }
        string GetModelNotifyName(ModelViewSerializable model)
        {
            return GetModelName(model) + "Notify";
        }
        string GetNotifyInterfaceName(ModelViewSerializable model)
        {
            return GetInterfaceName(model) + "Notify";
        }
        string GetModelPermissionInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelPermissionName(model);
        }
        string GetModelPermissionName(ModelViewSerializable model)
        {
            return GetModelName(model) + "Permission";
        }
        string GetPermissionInterfaceName(ModelViewSerializable model)
        {
            return GetInterfaceName(model) + "Permission";
        }
        string GetModelPermissionNotifyInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelPermissionNotifyName(model);
        }
        string GetModelPermissionNotifyName(ModelViewSerializable model)
        {
            return GetModelName(model) + "PermissionNotify";
        }
        string GetPageInterfaceName(ModelViewSerializable model)
        {
            return "I" + model.PageViewName;
        }
        string GetModelPageName(ModelViewSerializable model)
        {
            return FirstLetterToUpper(model.PageViewName);
        }
        string GetFilterPropertyOperatorName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model, string operatorSufix)
        {
            if (model.GenerateJSonAttribute)
            {
                return prop.JsonPropertyName + operatorSufix;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName) + operatorSufix;
            }
        }
        string GetFilterInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetFilterName(model);
        }
        string GetFilterName(ModelViewSerializable model)
        {
            return GetModelName(model) + "Filter";
        }
        string GetModelServiceInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelServiceName(model);
        }
        string GetModelServiceName(ModelViewSerializable model)
        {
            return GetModelName(model) + "Service";
        }
        string GetModelDatasourceName(ModelViewSerializable model)
        {
            return GetModelName(model) + "Datasource";
        }
        string GetModelDatasourceInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelDatasourceName(model);
        }
        string GetModelDatasourceInterfaceNameEx(DbContextSerializable context, string viewName)
        {
            return "I" + GetModelDatasourceName(GetModelByName(context, viewName));
        }
        string GetModelDatasourceVarName(ModelViewSerializable model, string varSuffix)
        {
            return GetModelDatasourceName(model) + varSuffix;
        }
        string GetModelDatasourceVarName(DbContextSerializable context, string viewName, string varSuffix)
        {
            return GetModelDatasourceVarName(GetModelByName(context, viewName), varSuffix);
        }

        string GetWebApiServicePrefix(ModelViewSerializable model)
        {
            string result = model.WebApiServiceName;
            if (!string.IsNullOrEmpty(result))
            {
                if (result.EndsWith("Controller"))
                {
                    result = result.Substring(0, result.LastIndexOf("Controller"));
                }
                result = result.ToLower();
            }
            return result;
        }
        bool HasAtributeWithValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName, string attrVal)
        {
            if ((sclrProp != null) && (!string.IsNullOrEmpty(attrName)) && (!string.IsNullOrEmpty(attrVal)))
            {
                if (sclrProp.Attributes != null)
                {
                    foreach (ModelViewAttributeSerializable a in sclrProp.Attributes)
                    {
                        if (attrName.Equals(a.AttrName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (a.VaueProperties != null)
                            {
                                foreach (ModelViewAttributePropertySerializable v in a.VaueProperties)
                                {
                                    if (!string.IsNullOrEmpty(v.PropValue))
                                    {
                                        if (v.PropValue.ToLower().Contains(attrVal))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        bool HasAtribute(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if ((sclrProp != null) && (!string.IsNullOrEmpty(attrName)))
            {
                if (sclrProp.Attributes != null)
                {
                    foreach (ModelViewAttributeSerializable a in sclrProp.Attributes)
                    {
                        if (attrName.Equals(a.AttrName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool HasFluentAtributeWithValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName, string attrVal)
        {
            if ((sclrProp != null) && (!string.IsNullOrEmpty(attrName)) && (!string.IsNullOrEmpty(attrVal)))
            {
                if (sclrProp.FAPIAttributes != null)
                {
                    foreach (ModelViewFAPIAttributeSerializable a in sclrProp.FAPIAttributes)
                    {
                        if (attrName.Equals(a.AttrName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (a.VaueProperties != null)
                            {
                                foreach (ModelViewFAPIAttributePropertySerializable v in a.VaueProperties)
                                {
                                    if (!string.IsNullOrEmpty(v.PropValue))
                                    {
                                        if (v.PropValue.ToLower().Contains(attrVal))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        bool HasFluentAtribute(ModelViewPropertyOfVwSerializable sclrProp, string[] attrName)
        {
            if ((sclrProp != null) && (attrName != null))
            {
                if ((sclrProp.FAPIAttributes != null) && (attrName.Length > 0))
                {
                    return sclrProp.FAPIAttributes.Any(a => attrName.Contains(a.AttrName));
                }
            }
            return false;
        }
        bool IsIdentityProperty(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (HasAtribute(prop, "ConcurrencyCheck") || HasAtribute(prop, "Timestamp"))
            {
                return true;
            }
            if (HasAtributeWithValue(prop, "DatabaseGenerated", "identity") || HasAtributeWithValue(prop, "DatabaseGenerated", "computed"))
            {
                return true;
            }
            if (HasFluentAtribute(prop, new string[] { "UseSqlServerIdentityColumn", "ForSqlServerUseSequenceHiLo", "ValueGeneratedOnAdd", "ValueGeneratedOnAddOrUpdate", "IsConcurrencyToken", "IsRowVersion" }))
            {
                return true;
            }
            return HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "identity") || HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "computed");
        }
        List<ModelViewPropertyOfVwSerializable> GetDatabaseGeneratedProp(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> rslt = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null) return null;
            if (model.ScalarProperties == null) return null;
            foreach (ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializable in model.ScalarProperties)
            {
                if (IsIdentityProperty(modelViewPropertyOfVwSerializable, model))
                {
                    rslt.Add(modelViewPropertyOfVwSerializable);
                }
            }
            return rslt;
        }
        string GetDefaultVal(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null)
            {
                return "0";
            }
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.boolean":
                    result = "false";
                    break;
                case "system.guid":
                case "system.string":
                    result = "\"0\"";
                    break;
                default:
                    result = "0";
                    break;
            }
            return result;
        }
        string GetContextModelNameSpace(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            string delim = "";
            string filePath = "";
            string defaultNameSpace = "";
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                filePath = refItem.FileFolder.Replace("\\", ".");
            }
            if (!string.IsNullOrEmpty(refItem.FileDefaultProjectNameSpace))
            {
                defaultNameSpace = refItem.FileDefaultProjectNameSpace;
            }
            if (!(string.IsNullOrEmpty(defaultNameSpace) || string.IsNullOrEmpty(filePath)))
            {
                delim = ".";
            }
            return defaultNameSpace + delim + filePath;
        }
        string GetContextModelClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName;
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

            }
            return sb.ToString().Replace(".xaml", "").Replace(".Xaml", "").Replace(".XAML", "");
        }
        bool IsPropertyNullable(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return (prop.IsNullable || (!prop.IsRequiredInView));
        }
        bool IsPropertyTypeNullable(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return prop.IsNullable;
        }
        string GetFilterPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (model.GenerateJSonAttribute)
            {
                return prop.JsonPropertyName;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName);
            }
        }
        string GetModelProjectUniqueName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileProject))
            {
                return result;
            }
            return refItem.FileProject.Replace(@"\", @"\\");
        }
        string GetModelServicePermissionInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelServicePermissionName(model);
        }
        string GetModelServicePermissionName(ModelViewSerializable model)
        {
            return GetModelServiceName(model) + "Permission";
        }
        string GetPermissionNotifyInterfaceName(ModelViewSerializable model)
        {
            return GetPermissionInterfaceName(model) + "Notify";
        }
        bool IsPropInteger(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null) return false;
            if (string.IsNullOrEmpty(prop.TypeFullName)) return false;
            string tpNm = prop.TypeFullName.ToLower();
            return
                "system.int16".Equals(tpNm) ||
                "system.int32".Equals(tpNm) ||
                "system.int64".Equals(tpNm) ||
                "system.uint16".Equals(tpNm) ||
                "system.uint32".Equals(tpNm) ||
                "system.uint64".Equals(tpNm);
        }
        string GetModelServiceCopyPermissionName(ModelViewSerializable model)
        {
            return GetModelServiceName(model) + "CopyPermission";
        }
        string GetModelServiceCopyPermissionInterfaceName(ModelViewSerializable model)
        {
            return "I" + GetModelServiceCopyPermissionName(model);
        }
        string GetModelProjectName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileProject))
            {
                return result;
            }
            string[] fnArr = refItem.FileProject.Split(new char[] { '\\' }, 100, System.StringSplitOptions.None);
            string fn = fnArr[fnArr.Length - 1];

            return fn.Replace(".csproj", "");
        }
        string GetContextModelProjectName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileProject))
            {
                return result;
            }
            string[] fnArr = refItem.FileProject.Split(new char[] { '\\' }, 100, System.StringSplitOptions.None);
            string fn = fnArr[fnArr.Length - 1];

            return fn.Replace(".csproj", "");
        }
        string GetModelPropertyNameEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetModelPropertyName(sclrProp, model);
        }
        string GetModelPropertyNameEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetModelPropertyName(sclrProp, model);
        }
        string GetCCharpDatatype(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            return prop.UnderlyingTypeName.ToLower().Replace("system.", "");
        }
        string GetCCharpDatatypeEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetCCharpDatatype(sclrProp, model);
        }
        string GetCCharpDatatypeEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetCCharpDatatype(sclrProp, model);
        }
        string GetDisplayAttributeValueString(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model, string propName)
        {
            if (prop == null)
            {
                return "";
            }
            if (prop.Attributes == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributeSerializable attr =
                prop.Attributes.Where(a => a.AttrName == "Display").FirstOrDefault();
            if (attr == null)
            {
                return prop.ViewPropertyName;
            }
            if (attr.VaueProperties == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributePropertySerializable attrProp =
                attr.VaueProperties.Where(v => v.PropName == propName).FirstOrDefault();
            if (attrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (string.IsNullOrEmpty(attrProp.PropValue))
            {
                return prop.ViewPropertyName;
            }
            else
            {
                char[] charsToTrim = { '"', ' ' };
                return attrProp.PropValue.Trim(charsToTrim);
            }
        }
        string GetDisplayAttributeValueStringEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            if ((prop == null) || (model == null))
            {
                return "";
            }
            if (model.ScalarProperties == null)
            {
                return "";
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetDisplayAttributeValueString(sclrProp, model, propName);
        }
        string GetDisplayAttributeValueStringEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            if ((prop == null) || (model == null))
            {
                return "";
            }
            if (model.ScalarProperties == null)
            {
                return "";
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetDisplayAttributeValueString(sclrProp, model, propName);
        }
        string GetMaxLen(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            if (prop.UnderlyingTypeName.ToLower() == "system.string")
            {
                string propValue = GetUnNamedAtributeValue(prop, "StringLength");
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
                propValue = GetUnNamedAtributeValue(prop, "MaxLength");
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
            }
            return "null";
        }
        string GetMaxLenEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMaxLen(sclrProp, model);
        }
        string GetMaxLenEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMaxLen(sclrProp, model);
        }
        string GetMinVal(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            string propValue = GetAtributeValueByNo(prop, "IntegerValidator", 0);
            if (!string.IsNullOrEmpty(propValue))
            {
                propValue = propValue.Replace("\"", "");
                if (!string.IsNullOrEmpty(propValue))
                {
                    return propValue;
                }
            }
            if (prop.UnderlyingTypeName.ToLower() == "system.datetime")
            {
                propValue = GetAtributeValueByNo(prop, "Range", 1);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return "new Date(\"" + propValue + "\")";
                    }
                }
            }
            else
            {
                propValue = GetAtributeValueByNo(prop, "Range", 0);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
            }
            return "null";
        }
        string GetMaxVal(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            string propValue = GetAtributeValueByNo(prop, "IntegerValidator", 1);
            if (!string.IsNullOrEmpty(propValue))
            {
                propValue = propValue.Replace("\"", "");
                if (!string.IsNullOrEmpty(propValue))
                {
                    return propValue;
                }
            }
            if (prop.UnderlyingTypeName.ToLower() == "system.datetime")
            {
                propValue = GetAtributeValueByNo(prop, "Range", 2);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return "new Date(\"" + propValue + "\")";
                    }
                }
            }
            else
            {
                propValue = GetAtributeValueByNo(prop, "Range", 1);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
            }
            return "null";
        }
        string GetMaxValEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMinVal(sclrProp, model);
        }
        string GetMaxValEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMaxVal(sclrProp, model);
        }
        string GetUnNamedAtributeValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if (sclrProp != null)
            {
                if (sclrProp.Attributes != null)
                {
                    ModelViewAttributeSerializable modelViewAttributeSerializable =
                        sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                    if (modelViewAttributeSerializable != null)
                    {
                        if (modelViewAttributeSerializable.VaueProperties != null)
                        {

                            ModelViewAttributePropertySerializable modelViewAttributePropertySerializable =
                                modelViewAttributeSerializable.VaueProperties.Where(p => (string.IsNullOrEmpty(p.PropName) || (p.PropName == "..."))).FirstOrDefault();
                            if (modelViewAttributePropertySerializable != null)
                            {
                                return modelViewAttributePropertySerializable.PropValue;
                            }
                        }
                    }
                }
            }
            return null;
        }
        string GetAtributeValueByNo(ModelViewPropertyOfVwSerializable sclrProp, string attrName, int itemNo)
        {
            if (itemNo > -1)
            {
                if (sclrProp != null)
                {
                    if (sclrProp.Attributes != null)
                    {
                        ModelViewAttributeSerializable modelViewAttributeSerializable =
                            sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                        if (modelViewAttributeSerializable != null)
                        {
                            if (modelViewAttributeSerializable.VaueProperties != null)
                            {
                                if (modelViewAttributeSerializable.VaueProperties.Count > itemNo)
                                {
                                    return modelViewAttributeSerializable.VaueProperties[itemNo].PropValue;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        List<ModelViewPropertyOfVwSerializable> GetPropsByForeignKey(ModelViewSerializable model, ModelViewForeignKeySerializable foreignKey)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((model == null) || (foreignKey == null))
            {
                return result;
            }
            if (foreignKey.PrincipalKeyProps == null || foreignKey.ForeignKeyProps == null || model.ScalarProperties == null)
            {
                return result;
            }
            if ((foreignKey.PrincipalKeyProps.Count != foreignKey.ForeignKeyProps.Count) || (foreignKey.ForeignKeyProps.Count < 1))
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable fkProp in foreignKey.PrincipalKeyProps)
            {
                ModelViewPropertyOfVwSerializable prop =
                    model.ScalarProperties.Where(p => (p.OriginalPropertyName == fkProp.OriginalPropertyName) && (foreignKey.NavigationName == p.ForeignKeyNameChain)).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            foreach (ModelViewKeyPropertySerializable fkProp in foreignKey.ForeignKeyProps)
            {
                ModelViewPropertyOfVwSerializable prop =
                    model.ScalarProperties.Where(p => (p.OriginalPropertyName == fkProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain)).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            return result;
        }
        string GetDisplayAttributeValueString2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (sclrProp.Attributes == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributeSerializable attr =
                sclrProp.Attributes.Where(a => a.AttrName == "Display").FirstOrDefault();
            if (attr == null)
            {
                return prop.ViewPropertyName;
            }
            if (attr.VaueProperties == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributePropertySerializable attrProp =
                attr.VaueProperties.Where(v => v.PropName == propName).FirstOrDefault();
            if (attrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (string.IsNullOrEmpty(attrProp.PropValue))
            {
                return prop.ViewPropertyName;
            }
            else
            {
                char[] charsToTrim = { '"', ' ' };
                return attrProp.PropValue.Trim(charsToTrim);
            }
        }
        bool IsBooleanInput(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return "System.Boolean".Equals(sclrProp.UnderlyingTypeName) || "Boolean".Equals(sclrProp.UnderlyingTypeName) || "bool".Equals(sclrProp.UnderlyingTypeName);
        }
        bool IsStringInput(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return ("System.String".Equals(sclrProp.UnderlyingTypeName) || "String".Equals(sclrProp.UnderlyingTypeName) || "string".Equals(sclrProp.UnderlyingTypeName));
        }
        bool IsStringInputEx(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            return ("System.String".Equals(prop.UnderlyingTypeName) || "String".Equals(prop.UnderlyingTypeName) || "string".Equals(prop.UnderlyingTypeName));
        }
        int GetGridColumnWidth(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            string s = GetDisplayAttributeValueString2(prop, model, "ShortName");
            if (string.IsNullOrEmpty(s)) return 100;
            int sl = s.Length;
            if (IsBooleanInput(prop, model))
            {
                return sl > 3 ? sl * 12 : 40;
            }
            if (IsDateInput(prop, model))
            {
                return sl > 20 ? sl * 12 : 250;
            }
            if (IsStringInput(prop, model))
            {
                s = GetMaxLenEx2(prop, model);
                int sl1 = 0;
                if (int.TryParse(s, out sl1))
                {
                    if (sl1 > 40) sl1 = 40;
                    return sl > sl1 ? sl * 12 : sl1 * 12;
                }
                return sl > 35 ? sl * 12 : 350;
            }
            return sl > 20 ? sl * 12 : 250;
        }
        bool hasSortHeader(ModelViewUIListPropertySerializable modelViewUIListPropertySerializable, ModelViewSerializable model)
        {
            if ((model == null) || (modelViewUIListPropertySerializable == null))
            {
                return false;
            }
            if ((model.UIListProperties == null) || (model.ScalarProperties == null))
            {
                return false;
            }
            return model.ScalarProperties.Any(s => s.ViewPropertyName == modelViewUIListPropertySerializable.ViewPropertyName && s.IsUsedBySorting);
        }
        bool IsDateTimeInput(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return ("System.DateTime".Equals(sclrProp.UnderlyingTypeName) || "DateTime".Equals(sclrProp.UnderlyingTypeName));
        }
        bool IsDateInput(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if (IsDateTimeInput(prop, model))
            {
                ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
                if (sclrProp == null) return false;
                string rsltStr = GetAtributeUnNamedValue(sclrProp, "DataType");
                if (string.IsNullOrEmpty(rsltStr)) return false;
                return ((String.Compare("DataType.Date", rsltStr) == 0) || (String.Compare("Date", rsltStr) == 0));
            }
            return false;
        }
        bool IsCurrencyInput(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null) return false;
            string rsltStr = GetAtributeUnNamedValue(sclrProp, "DataType");
            if (string.IsNullOrEmpty(rsltStr)) return false;
            return (rsltStr.IndexOf("Currency", StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
        string GetAtributeUnNamedValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if (sclrProp != null)
            {
                if (sclrProp.Attributes != null)
                {
                    ModelViewAttributeSerializable modelViewAttributeSerializable =
                        sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                    if (modelViewAttributeSerializable != null)
                    {
                        if (modelViewAttributeSerializable.VaueProperties != null)
                        {

                            ModelViewAttributePropertySerializable modelViewAttributePropertySerializable =
                                modelViewAttributeSerializable.VaueProperties.Where(p => (string.IsNullOrEmpty(p.PropName) || (p.PropName == "..."))).FirstOrDefault();
                            if (modelViewAttributePropertySerializable != null)
                            {
                                return modelViewAttributePropertySerializable.PropValue;
                            }
                        }
                    }
                }
            }
            return null;
        }
        string GetModelPropertyNameWithSufixForInputTypeMode(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix, int inputTypeId)
        {
            string inputTypeMode;
            switch (inputTypeId)
            {
                case 2:
                    inputTypeMode = "UpdMode";
                    break;
                case 3:
                    inputTypeMode = "DelMode";
                    break;
                default:
                    inputTypeMode = "AddMode";
                    break;
            }
            return GetModelPropertyNameEx(prop, model) + sufix + inputTypeMode;
        }
        string GetModelPropertyNameWithSufix(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetModelPropertyNameEx(prop, model) + sufix;
        }
        string GetExpressionForControlList(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetModelPropertyNameWithSufix(prop, model, sufix) + "Vals";
        }
        string GetExpressionForOnFilterTypeaheadControlListMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string typeaheadSufix)
        {
            return "OnFilter" + GetExpressionForControlList(prop, model, typeaheadSufix);
        }
        bool HasTypeahead(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead);
        }
        string GetGridFlexRowDefs(ModelViewSerializable model, int curIngex)
        {
            string rslt = "Auto";
            if ((model.UIFormProperties == null) || (curIngex < 0)) return rslt;
            if (model.UIFormProperties.Count <= curIngex) return rslt;
            if (model.UIFormProperties[curIngex].IsNewLineAfter) return rslt;
            for (int i = curIngex + 1; i < model.UIFormProperties.Count; i++)
            {
                rslt += ",Auto";
                if (model.UIFormProperties[i].IsNewLineAfter) return rslt;
            }
            return rslt;
        }
        string GetDisplayAttributeValueString(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (sclrProp.Attributes == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributeSerializable attr =
                sclrProp.Attributes.Where(a => a.AttrName == "Display").FirstOrDefault();
            if (attr == null)
            {
                return prop.ViewPropertyName;
            }
            if (attr.VaueProperties == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributePropertySerializable attrProp =
                attr.VaueProperties.Where(v => v.PropName == propName).FirstOrDefault();
            if (attrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (string.IsNullOrEmpty(attrProp.PropValue))
            {
                return prop.ViewPropertyName;
            }
            else
            {
                char[] charsToTrim = { '"', ' ' };
                return attrProp.PropValue.Trim(charsToTrim);
            }
        }
        string GetViewByForeignNameChain(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return "";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (mv == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return ViewName;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (foreignKeys.Length < 1)
            {
                return "";
            }
            ModelViewForeignKeySerializable fk =
                mv.ForeignKeys.Where(f => f.NavigationName == foreignKeys[0]).FirstOrDefault();
            if (fk == null)
            {
                return "";
            }
            if (foreignKeys.Length == 1)
            {
                return GetViewByForeignNameChain(context, fk.ViewName, "");
            }
            return GetViewByForeignNameChain(context, fk.ViewName, string.Join(".", foreignKeys, 1, foreignKeys.Length - 1));
        }
        string GetComboControlListPropertyName(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return "NoName";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (mv == null)
            {
                return "NoName";
            }
            ModelViewPropertyOfVwSerializable propForSel =
                mv.ScalarProperties.Where(p => (string.IsNullOrEmpty(p.ForeignKeyNameChain) && p.OriginalPropertyName == prop.OriginalPropertyName)).FirstOrDefault();
            if (propForSel == null)
            {
                return "NoName";
            }
            return GetModelPropertyName(propForSel, mv);
        }
        string GetControlListPropertyName(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return "NoName";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (mv == null)
            {
                return "NoName";
            }
            ModelViewPropertyOfVwSerializable propForSel =
                mv.ScalarProperties.Where(p => (string.IsNullOrEmpty(p.ForeignKeyNameChain) && p.OriginalPropertyName == prop.OriginalPropertyName)).FirstOrDefault();
            if (propForSel == null)
            {
                return "NoName";
            }
            return GetModelPropertyName(propForSel, mv);
        }
        bool IsBooleanInput(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return "System.Boolean".Equals(sclrProp.UnderlyingTypeName) || "Boolean".Equals(sclrProp.UnderlyingTypeName) || "bool".Equals(sclrProp.UnderlyingTypeName);
        }
        bool IsDateInput(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return ("System.DateTime".Equals(sclrProp.UnderlyingTypeName) || "DateTime".Equals(sclrProp.UnderlyingTypeName));
        }
        bool IsMemoInput(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            if (!("System.String".Equals(sclrProp.UnderlyingTypeName) || "String".Equals(sclrProp.UnderlyingTypeName)))
            {
                return false;
            }
            if (sclrProp.Attributes != null)
            {
                if (sclrProp.Attributes.Where(a => (a.AttrName == "MaxLength") || (a.AttrName == "StringLength")).Any())
                {
                    return false;
                }
            }
            if (sclrProp.FAPIAttributes != null)
            {
                if (sclrProp.FAPIAttributes.Where(a => a.AttrName == "HasMaxLength").Any())
                {
                    return false;
                }
            }
            return true;
        }
        string GetInputTypeToEnumName(int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return "AddMode";
                case 2:
                    return "UpdateMode";
                default:
                    return "DeleteMode";
            }
        }
        string GetNullableConverterDecl(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string CoverterName)
        {
            if ((prop == null) || (model == null) || CoverterName == null)
            {
                return "";
            }
            if (model.ScalarProperties == null)
            {
                return "";
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return "";
            }
            if ("System.String".Equals(sclrProp.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }
            return ", Converter={StaticResource " + CoverterName + "}, ConverterParameter=\'" + sclrProp.UnderlyingTypeName + "\'";
        }
        string GetUnderlyingTypeNameAsNullable(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return prop.UnderlyingTypeName;
            }
            return prop.UnderlyingTypeName + " ?";
        }
        string GetUnderlyingTypeNameAsNullableEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null))
            {
                return "";
            }
            if (model.ScalarProperties == null)
            {
                return "";
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetUnderlyingTypeNameAsNullable(sclrProp);
        }
        string NullableValueSuffix(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }
            return ".Value";
        }
        string NullableValueSuffixEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return NullableValueSuffix(sclrProp);
        }
        bool IsStringProperty(ModelViewPropertyOfVwSerializable prop)
        {
            return "System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase);
        }
        bool IsStringPropertyEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return IsStringProperty(sclrProp);
        }
        bool IsPropertyRequiredInViewEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp != null)
            {
                return sclrProp.IsRequiredInView;
            }
            return false;
        }
        bool IsPropertyString(ModelViewPropertyOfVwSerializable prop)
        {
            return "System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase);
        }
        List<string> GetFKViewsList(ModelViewSerializable model, DbContextSerializable context, List<string> fkViewsDict)
        {
            if ((model == null) || (context == null) || (fkViewsDict == null))
            {
                return fkViewsDict;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return fkViewsDict;
            }
            string viewNameForSel = null;
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if ((modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Typeahead))
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!fkViewsDict.Contains(viewNameForSel))
                            {
                                fkViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if ((modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Typeahead))
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!fkViewsDict.Contains(viewNameForSel))
                            {
                                fkViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if ((modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Typeahead))
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!fkViewsDict.Contains(viewNameForSel))
                            {
                                fkViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
            }
            return fkViewsDict;
        }
        List<string> GetSearchDialogViewsList(ModelViewSerializable model, DbContextSerializable context, List<string> sdViewsDict)
        {
            if ((model == null) || (context == null) || (sdViewsDict == null))
            {
                return sdViewsDict;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return sdViewsDict;
            }
            string viewNameForSel = null;
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!sdViewsDict.Contains(viewNameForSel))
                            {
                                sdViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!sdViewsDict.Contains(viewNameForSel))
                            {
                                sdViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!sdViewsDict.Contains(viewNameForSel))
                            {
                                sdViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
            }
            return sdViewsDict;
        }
        ModelViewSerializable GetModelByName(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
        }
        List<string> CollectComboListInterfaces(DbContextSerializable context, ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;

            if (prop.InputTypeWhenAdd == InputTypeEnum.Combo)
            {
                viewNameForSel = prop.ForeifKeyViewNameForAdd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        result.Add("Array<" + GetInterfaceName(mv) + ">");
                    }
                }
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.Combo)
            {
                viewNameForSel = prop.ForeifKeyViewNameForUpd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = "Array<" + GetInterfaceName(mv) + ">";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.Combo)
            {
                viewNameForSel = prop.ForeifKeyViewNameForDel;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = "Array<" + GetInterfaceName(mv) + ">";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            return result;
        }
        string GetModelServiceInterfaceNameEx(DbContextSerializable context, string viewName)
        {
            return GetModelServiceInterfaceName(context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault());
        }
        bool HasCombo(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                    (prop.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                    (prop.InputTypeWhenDelete == InputTypeEnum.Combo);
        }
        bool HasButton(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog);
        }
        string GetExpressionForOnValChangedMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return "OnValChanged" + GetModelPropertyNameEx(prop, model);
        }
        List<ModelViewUIFormPropertySerializable> GetDirectDetails(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            string foreignKeyNameChain = prop.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return result;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (foreignKeys.Length < 2)
            {
                return result;
            }
            string fltFKNameChain = string.Join(".", foreignKeys, 0, foreignKeys.Length - 1);
            List<ModelViewUIFormPropertySerializable> propLst = null;
            switch (inputType)
            {
                case 1:
                    propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                ((p.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                                (p.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                                (p.InputTypeWhenAdd == InputTypeEnum.SearchDialog))).ToList();
                    break;
                case 2:
                    propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                ((p.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                                (p.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                                (p.InputTypeWhenUpdate == InputTypeEnum.SearchDialog))).ToList();
                    break;
                case 3:
                    propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                ((p.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                                (p.InputTypeWhenDelete == InputTypeEnum.Typeahead) ||
                                (p.InputTypeWhenDelete == InputTypeEnum.SearchDialog))).ToList();
                    break;
                default:
                    break;
            }
            if (propLst != null)
            {
                return propLst;
            }
            return result;
        }
        List<ModelViewUIFormPropertySerializable> GetDependentScalarProps(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            if (!HasInitMethodForInputMode(prop, model, inputType))
            {
                return result;
            }
            string currentPropChain = string.IsNullOrEmpty(prop.ForeignKeyNameChain) ? "" : prop.ForeignKeyNameChain;
            List<ModelViewUIFormPropertySerializable> masters = GetDirectMasters(prop, model, context, inputType);
            foreach (ModelViewUIFormPropertySerializable dependentProp in model.UIFormProperties)
            {
                if (prop.ViewPropertyName == dependentProp.ViewPropertyName)
                {
                    result.Add(dependentProp);
                    continue;
                }
                if (HasInitMethodForInputMode(dependentProp, model, inputType))
                {
                    continue;
                }
                string dependentPropChain = string.IsNullOrEmpty(dependentProp.ForeignKeyNameChain) ? "" : dependentProp.ForeignKeyNameChain;
                if (dependentPropChain == currentPropChain)
                {
                    result.Add(dependentProp);
                    continue;
                }
                string locCurrentPropChain = currentPropChain;
                if (!string.IsNullOrEmpty(locCurrentPropChain)) locCurrentPropChain += ".";
                if (!dependentPropChain.StartsWith(locCurrentPropChain))
                {
                    continue;
                }
                if (!masters.Where(p => dependentPropChain.StartsWith(p.ForeignKeyNameChain)).Any())
                {
                    result.Add(dependentProp);
                }
            }
            return result;
        }
        bool HasOnValChangedMethod(DbContextSerializable context, ModelViewSerializable model, ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable)
        {
            bool result = false;
            for (int inputType = 1; inputType < 4; inputType++)
            {
                result =
                    (GetDirectDetails(modelViewUIFormPropertySerializable, model, context, inputType).Count > 0) ||
                    (GetDependentScalarProps(modelViewUIFormPropertySerializable, model, context, inputType).Count > 0);
                if (result)
                {
                    return result;
                }
            }
            return result;
        }
        bool HasInitMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return HasCombo(prop, model) || HasButton(prop, model) || HasTypeahead(prop, model);
        }
        bool HasInitMethodForInputMode(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return
                        (prop.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead);
//                    break;
                case 2:
                    return
                        (prop.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead);
//                    break;
                case 3:
                    return
                        (prop.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead);
//                    break;
            }
            return false;
        }
        string GetExpressionForOnInitMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return "OnInit" + GetModelPropertyNameEx(prop, model);
        }
        InputTypeEnum GetInputTypeWhenXXX(ModelViewUIFormPropertySerializable prop, int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return prop.InputTypeWhenAdd;
                case 2:
                    return prop.InputTypeWhenUpdate;
                default:
                    return prop.InputTypeWhenDelete;
            }
        }
        ModelViewSerializable GetViewForControlList(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            if ((prop == null) || (model == null) || (context == null))
            {
                return null;
            }
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
        }
        string GetViewNameForControlList(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            ModelViewSerializable mv = GetViewForControlList(prop, model, context, inputType);
            if (mv == null)
            {
                return "NoName";
            }
            return mv.ViewName;
        }
        List<ModelViewUIFormPropertySerializable> GetDirectMasters(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            string viewNameForSel = GetViewNameForControlList(prop, model, context, inputType);
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return result;
            }
            ModelViewSerializable modelViewSerializable = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (modelViewSerializable == null)
            {
                return result;
            }
            if (modelViewSerializable.ForeignKeys == null)
            {
                return result;
            }
            string foreignKeyNameChain = prop.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                foreignKeyNameChain = "";
            }
            else
            {
                foreignKeyNameChain += ".";
            }
            foreach (ModelViewForeignKeySerializable modelViewForeignKeySerializable in modelViewSerializable.ForeignKeys)
            {
                if (!string.IsNullOrEmpty(modelViewForeignKeySerializable.ViewName))
                {
                    string fltFKNameChain = foreignKeyNameChain + modelViewForeignKeySerializable.NavigationName;
                    List<ModelViewUIFormPropertySerializable> propLst = null;
                    switch (inputType)
                    {
                        case 1:
                            propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                       ((p.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                                        (p.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                                        (p.InputTypeWhenAdd == InputTypeEnum.SearchDialog))).ToList();
                            break;
                        case 2:
                            propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                       ((p.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                                        (p.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                                        (p.InputTypeWhenUpdate == InputTypeEnum.SearchDialog))).ToList();
                            break;
                        case 3:
                            propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                       ((p.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                                        (p.InputTypeWhenDelete == InputTypeEnum.Typeahead) ||
                                        (p.InputTypeWhenDelete == InputTypeEnum.SearchDialog))).ToList();
                            break;
                        default:
                            break;
                    }
                    if (propLst != null)
                    {
                        result.AddRange(propLst);
                    }
                }
            }
            return result;
        }
        bool HasModelInitMethodForInputMode(ModelViewSerializable model, int inputType)
        {
            if (model == null)
            {
                return false;
            }
            if (model.UIFormProperties == null)
            {
                return false;
            }
            foreach (ModelViewUIFormPropertySerializable prop in model.UIFormProperties)
            {
                if (HasInitMethodForInputMode(prop, model, inputType))
                {
                    return true;
                }
            }
            return false;
        }
        List<ModelViewPropertyOfVwSerializable> GetPrimaryKeyProps(DbContextSerializable context, string viewName)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return result;
            }
            return GetModelPrimaryKeyProps(context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault());
        }
        List<ModelViewPropertyOfVwSerializable> GetForeignKeyProps(DbContextSerializable context, ModelViewSerializable model, ModelViewUIFormPropertySerializable masterProp, ModelViewUIFormPropertySerializable detailProp, int inputType)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((context == null) || (masterProp == null) || (detailProp == null) || (model == null))
            {
                return result;
            }
            string masterForeignKeyNameChain = masterProp.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(masterForeignKeyNameChain))
            {
                return result;
            }
            string detailForeignKeyNameChain = detailProp.ForeignKeyNameChain;
            if (!string.IsNullOrEmpty(detailProp.ForeignKeyNameChain))
            {
                masterForeignKeyNameChain = masterForeignKeyNameChain.Replace(detailProp.ForeignKeyNameChain + ".", "");
            }
            string[] fKchain = masterForeignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (fKchain.Length < 1)
            {
                return result;
            }
            ModelViewSerializable detailModel =
                GetViewForControlList(detailProp, model, context, inputType);
            if (detailModel == null)
            {
                return result;
            }
            if (detailModel.ForeignKeys == null)
            {
                return result;
            }
            ModelViewForeignKeySerializable foreignKey =
                detailModel.ForeignKeys.Where(f => f.NavigationName == fKchain[0]).FirstOrDefault();
            if (foreignKey == null)
            {
                return result;
            }
            if (foreignKey.ForeignKeyProps == null)
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable fkProp in foreignKey.ForeignKeyProps)
            {
                ModelViewPropertyOfVwSerializable scProp =
                    detailModel.ScalarProperties.Where(p => p.ViewPropertyName == fkProp.ViewPropertyName).FirstOrDefault();
                if (scProp != null)
                {
                    result.Add(scProp);
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetForeignKeyPropsBase(DbContextSerializable context, ModelViewSerializable model, ModelViewUIFormPropertySerializable masterProp)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((context == null) || (masterProp == null) || (model == null))
            {
                return result;
            }
            string masterForeignKeyNameChain = masterProp.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(masterForeignKeyNameChain))
            {
                return GetModelPrimaryKeyProps(model);
            }
            else
            {
                if (model.ForeignKeys == null)
                {
                    return result;
                }
                string[] fKchain = masterForeignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
                if (fKchain.Length != 1)
                {
                    return result;
                }
                ModelViewForeignKeySerializable foreignKey =
                    model.ForeignKeys.Where(f => f.NavigationName == fKchain[0]).FirstOrDefault();
                if (foreignKey == null)
                {
                    return result;
                }
                if ((foreignKey.ForeignKeyProps == null) || (foreignKey.PrincipalKeyProps == null))
                {
                    return result;
                }
                if (foreignKey.ForeignKeyProps.Count != foreignKey.PrincipalKeyProps.Count)
                {
                    return result;
                }
                for (int i = 0; i < foreignKey.ForeignKeyProps.Count; i++)
                {
                    ModelViewKeyPropertySerializable fkProp = foreignKey.ForeignKeyProps[i];
                    ModelViewPropertyOfVwSerializable scProp =
                        model.ScalarProperties.Where(p => (p.OriginalPropertyName == fkProp.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyName))).FirstOrDefault();
                    if (scProp != null)
                    {
                        result.Add(scProp);
                    }
                    else
                    {
                        ModelViewKeyPropertySerializable pkProp = foreignKey.PrincipalKeyProps[i];
                        scProp =
                            model.ScalarProperties.Where(p => (p.OriginalPropertyName == pkProp.OriginalPropertyName) && (p.ForeignKeyName == foreignKey.NavigationName)).FirstOrDefault();
                        if (scProp != null)
                        {
                            result.Add(scProp);
                        }
                    }
                }
            }
            return result;
        }
        string GetPrimKeyVarName(ModelViewPropertyOfVwSerializable pkpModelViewUIFormPropertySerializable)
        {
            return "pkp" + pkpModelViewUIFormPropertySerializable.ViewPropertyName;
        }
        bool MustHaveDirectDetails(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context)
        {
            bool result = false;
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            string foreignKeyNameChain = prop.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return result;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (foreignKeys.Length < 2)
            {
                return result;
            }
            return true;
        }
        string GetExpressionForOnUpdateComboControlListMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string comboSufix)
        {
            return "OnUpdate" + GetExpressionForControlList(prop, model, comboSufix);
        }
        string GetInterfaceNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "I";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "I";
            }
            return GetInterfaceName(model);
        }
        string GetFilterInterfaceNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "I";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "I";
            }
            return GetFilterInterfaceName(model);
        }
        string GetFilterNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "";
            }
            return GetFilterName(model);
        }
        string GetPageInterfaceNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "";
            }
            return GetPageInterfaceName(model);
        }
        string GetOrderBy(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType, string prefix)
        {
            string propName = GetControlListPropertyName(prop, model, context, inputType);
            if ("Noname".Equals(propName, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(propName))
            {
                return "";
            }
            return prefix + ".orderby= new List<string>() {\"" + propName + "\"};";
        }
        string GetPrimKeyFilterForFindIndexMethod(DbContextSerializable context, string ViewName, string srcPrefix, string destPrefix)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return "false";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (model == null)
            {
                return "false";
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null))
            {
                return "false";
            }
            string result = "";
            foreach (ModelViewKeyPropertySerializable keyProp in model.PrimaryKeyProperties)
            {
                ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializable =
                    model.ScalarProperties.Where(p => p.ViewPropertyName == keyProp.ViewPropertyName).FirstOrDefault();
                if (modelViewPropertyOfVwSerializable != null)
                {
                    string proName = GetModelPropertyName(modelViewPropertyOfVwSerializable, model);
                    if (result != "")
                    {
                        result += " && ";
                    }
                    result += "(" + srcPrefix + "." + proName + " == " + destPrefix + "." + proName + ")";
                }
            }
            if (result == "")
            {
                return "false";
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelForeignKeyProps(ModelViewSerializable model, string detailFkChain, string masterFkChain)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((model == null) || string.IsNullOrEmpty(masterFkChain))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null) || (model.ForeignKeys == null))
            {
                return result;
            }
            if (string.IsNullOrEmpty(detailFkChain))
            {
                detailFkChain = "";
            }
            else
            {
                detailFkChain += ".";
            }
            string[] chain = masterFkChain.Replace(detailFkChain, "").Split(new string[] { "." }, StringSplitOptions.None);
            if (chain.Length < 1)
            {
                return result;
            }
            ModelViewForeignKeySerializable foreignKeySerializable =
                model.ForeignKeys.Where(f => f.NavigationName == chain[0]).FirstOrDefault();
            if (foreignKeySerializable == null)
            {
                return result;
            }
            if ((foreignKeySerializable.ForeignKeyProps == null) || (foreignKeySerializable.PrincipalKeyProps == null))
            {
                return result;
            }
            if (foreignKeySerializable.ForeignKeyProps.Count != foreignKeySerializable.PrincipalKeyProps.Count)
            {
                return result;
            }
            for (int i = 0; i < foreignKeySerializable.ForeignKeyProps.Count; i++)
            {
                ModelViewKeyPropertySerializable modelViewKeyPropertySerializable = foreignKeySerializable.ForeignKeyProps[i];
                ModelViewPropertyOfVwSerializable prop =
                        model.ScalarProperties.Where(p => ((p.OriginalPropertyName == modelViewKeyPropertySerializable.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
                else
                {
                    modelViewKeyPropertySerializable = foreignKeySerializable.PrincipalKeyProps[i];
                    prop =
                        model.ScalarProperties.Where(p => ((p.OriginalPropertyName == modelViewKeyPropertySerializable.OriginalPropertyName) && (p.ForeignKeyName == foreignKeySerializable.NavigationName))).FirstOrDefault();
                    if (prop != null)
                    {
                        result.Add(prop);
                    }
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetAllForeignKeyProps(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null)
            {
                return result;
            }
            if ((model.ForeignKeys == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
            {
                if (!string.IsNullOrEmpty(fk.ViewName))
                {
                    if (fk.ForeignKeyProps != null)
                    {
                        foreach (ModelViewKeyPropertySerializable fkProp in fk.ForeignKeyProps)
                        {
                            ModelViewPropertyOfVwSerializable sclrProp =
                                model.ScalarProperties.Where(p => p.ViewPropertyName == fkProp.ViewPropertyName).FirstOrDefault();
                            if (sclrProp != null)
                            {
                                result.Add(sclrProp);
                            }
                        }
                    }
                }
            }
            return result;
        }
        ModelViewPropertyOfVwSerializable GetTypeAheadMasterProp(ModelViewSerializable model, ModelViewPropertyOfVwSerializable dependentScalarProp, ModelViewSerializable master)
        {
            if ((dependentScalarProp == null) || (model == null) || (master == null))
            {
                return null;
            }
            //string masterForeignKeyNameChain = "";
            //if(!string.IsNullOrEmpty( dependentScalarProp.ForeignKeyName )) {
            //if (dependentScalarProp.ForeignKeyName != dependentScalarProp.ForeignKeyNameChain) {
            //masterForeignKeyNameChain = dependentScalarProp.ForeignKeyNameChain.Replace(dependentScalarProp.ForeignKeyName + ".", "");
            //}
            //}
            //if (string.IsNullOrEmpty(masterForeignKeyNameChain)) {
            return
                master.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain)).FirstOrDefault();
            //}
            //return
            //master.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && (p.ForeignKeyNameChain == masterForeignKeyNameChain)).FirstOrDefault();
        }
        ModelViewPropertyOfVwSerializable GetTypeAheadMasterPropEx(ModelViewSerializable model, ModelViewUIFormPropertySerializable dependentScalarProp, ModelViewSerializable master)
        {
            if ((dependentScalarProp == null) || (model == null) || (master == null))
            {
                return null;
            }
            ModelViewPropertyOfVwSerializable prop =
                model.ScalarProperties.Where(p => p.ViewPropertyName == dependentScalarProp.ViewPropertyName).FirstOrDefault();
            return GetTypeAheadMasterProp(model, prop, master);
        }
        List<string> GetHiddenFilterDisablingFields(ModelViewSerializable model, DbContextSerializable context, string foreignKeyNameChain, List<string> result)
        {
            if (result == null)
            {
                result = new List<string>();
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain) || (model == null) || (context == null))
            {
                return result;
            }
            string masterNm = GetViewByForeignNameChain(context, model.ViewName, foreignKeyNameChain);
            if (string.IsNullOrEmpty(masterNm))
            {
                return result;
            }
            ModelViewSerializable master = context.ModelViews.Where(m => m.ViewName == masterNm).FirstOrDefault();
            if (master == null)
            {
                return result;
            }
            if ((master.PrimaryKeyProperties != null) && (master.ScalarProperties != null))
            {
                foreach (ModelViewKeyPropertySerializable pkProp in master.PrimaryKeyProperties)
                {
                    ModelViewPropertyOfVwSerializable masterProp = master.ScalarProperties.Where(s => s.ViewPropertyName == pkProp.ViewPropertyName).FirstOrDefault();
                    List<ModelViewPropertyOfVwSerializable> props =
                        GetForeignKeyPropByIndirectPrimaryKeyProp(model, foreignKeyNameChain, master, context, masterProp);
                    if (props != null)
                    {
                        foreach (ModelViewPropertyOfVwSerializable prop in props)
                        {
                            string propName = GetModelPropertyName(prop, model);
                            if (!result.Contains(propName))
                            {
                                result.Add(propName);
                            }
                        }
                    }
                }
            }
            string[] fKchain = foreignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (fKchain.Length < 2)
            {
                return result;
            }
            string newChain = string.Join(".", fKchain, 0, fKchain.Length - 1);
            return GetHiddenFilterDisablingFields(model, context, newChain, result);
        }
        ModelViewPropertyOfVwSerializable GetOnValChangeViewPropName(DbContextSerializable context, ModelViewSerializable model,
                                    ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable, ModelViewUIFormPropertySerializable dependentScalarProp, int inputType)
        {
            if ((dependentScalarProp == null) || (modelViewUIFormPropertySerializable == null))
            {
                return null;
            }
            ModelViewSerializable view = GetViewForControlList(modelViewUIFormPropertySerializable, model, context, inputType);
            if (view == null)
            {
                return null;
            }
            string foreignKeyNameChain =
                string.IsNullOrEmpty(modelViewUIFormPropertySerializable.ForeignKeyNameChain) ? "" : modelViewUIFormPropertySerializable.ForeignKeyNameChain;


            string dependentForeignKeyNameChain =
                (string.IsNullOrEmpty(dependentScalarProp.ForeignKeyNameChain) ? "" : dependentScalarProp.ForeignKeyNameChain);
            if (foreignKeyNameChain == dependentForeignKeyNameChain)
            {
                dependentForeignKeyNameChain = "";
            }
            else
            {
                if (foreignKeyNameChain != "")
                {
                    foreignKeyNameChain += ".";
                    dependentForeignKeyNameChain = dependentForeignKeyNameChain.Replace(foreignKeyNameChain, "");
                }
            }
            if (string.IsNullOrEmpty(dependentForeignKeyNameChain))
            {
                return
                    view.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain)).FirstOrDefault();
            }
            return
                view.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && (p.ForeignKeyNameChain == dependentForeignKeyNameChain)).FirstOrDefault();
        }
        List<ModelViewPropertyOfVwSerializable> GetForeignKeyPropByIndirectPrimaryKeyProp(ModelViewSerializable model, string foreignKeyNameChain, ModelViewSerializable master, DbContextSerializable context, ModelViewPropertyOfVwSerializable masterProp)
        {
            if ((model == null) || (master == null) || (context == null) || (masterProp == null))
            {
                return null;
            }
            if ((master.ScalarProperties == null) || (master.PrimaryKeyProperties == null) || (model.ScalarProperties == null) || (model.PrimaryKeyProperties == null) || (model.ForeignKeys == null))
            {
                return null;
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return null;
            }
            string[] fKchain = foreignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (fKchain.Length < 1)
            {
                return null;
            }

            List<ModelViewSerializable> modelChain = new List<ModelViewSerializable>();
            ModelViewSerializable currModel = model;
            ModelViewForeignKeySerializable fk = null;
            modelChain.Add(currModel);
            for (int i = 0; i < fKchain.Length; i++)
            {
                if ((currModel.ForeignKeys == null) || (currModel.ScalarProperties == null))
                {
                    return null;
                }
                fk = currModel.ForeignKeys.Where(f => f.NavigationName == fKchain[i]).FirstOrDefault();
                if (fk == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(fk.ViewName))
                {
                    return null;
                }
                currModel = context.ModelViews.Where(m => m.ViewName == fk.ViewName).FirstOrDefault();
                if (currModel == null)
                {
                    return null;
                }
                modelChain.Add(currModel);
            }
            if (currModel != master)
            {
                if ((currModel.RootEntityFullClassName != master.RootEntityFullClassName) || (currModel.RootEntityUniqueProjectName != master.RootEntityUniqueProjectName))
                {
                    return null;
                }
                masterProp = currModel.ScalarProperties.Where(p => (p.OriginalPropertyName == masterProp.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyName))).FirstOrDefault();
            }

            List<ModelViewPropertyOfVwSerializable> currProps = new List<ModelViewPropertyOfVwSerializable>();
            List<ModelViewPropertyOfVwSerializable> destProps = new List<ModelViewPropertyOfVwSerializable>();
            currProps.Add(masterProp);
            for (int i = fKchain.Length - 1; i >= 0; i--)
            {
                destProps.Clear();
                fk = modelChain[i].ForeignKeys.Where(f => f.NavigationName == fKchain[i]).FirstOrDefault();
                if (fk == null)
                {
                    return null;
                }
                foreach (ModelViewPropertyOfVwSerializable currProp in currProps)
                {
                    ModelViewPropertyOfVwSerializable destProp = null;
                    if ((fk.PrincipalKeyProps != null) && (fk.ForeignKeyProps != null))
                    {
                        ModelViewKeyPropertySerializable primKey = fk.PrincipalKeyProps.Where(p => p.ViewPropertyName == currProp.ViewPropertyName).FirstOrDefault();
                        if (primKey != null)
                        {
                            int ind = fk.PrincipalKeyProps.IndexOf(primKey);
                            if ((ind > -1) && (ind < fk.ForeignKeyProps.Count))
                            {
                                destProp =
                                    modelChain[i].ScalarProperties.Where(p => p.ViewPropertyName == fk.ForeignKeyProps[ind].ViewPropertyName).FirstOrDefault();
                                if (destProp != null)
                                {
                                    if (!destProps.Contains(destProp))
                                    {
                                        destProps.Add(destProp);
                                    }
                                }
                            }
                        }
                    }
                    destProp =
                        modelChain[i].ScalarProperties.Where(p => (p.OriginalPropertyName == currProp.OriginalPropertyName) && (p.ForeignKeyName == fKchain[i])).FirstOrDefault();
                    if (destProp != null)
                    {
                        if (!destProps.Contains(destProp))
                        {
                            destProps.Add(destProp);
                        }
                    }
                }
                if (destProps.Count < 1)
                {
                    return null;
                }
                else
                {
                    List<ModelViewPropertyOfVwSerializable> tmp = destProps;
                    destProps = currProps;
                    currProps = tmp;
                }
            }
            return currProps;
        }
        List<ModelViewPropertyOfVwSerializable> GetForeignKeyPropByIndirectPrimaryKeyPropEx(ModelViewSerializable model, ModelViewUIFormPropertySerializable uiProp, ModelViewSerializable master, DbContextSerializable context, ModelViewPropertyOfVwSerializable masterProp)
        {
            if ((uiProp == null) || (model == null) || (master == null) || (context == null) || (masterProp == null))
            {
                return null;
            }
            if (model.ScalarProperties == null)
            {
                return null;
            }
            ModelViewPropertyOfVwSerializable sUiProp = model.ScalarProperties.Where(p => p.ViewPropertyName == uiProp.ViewPropertyName).FirstOrDefault();
            return GetForeignKeyPropByIndirectPrimaryKeyProp(model, sUiProp.ForeignKeyNameChain, master, context, masterProp);
        }
        bool IsIdentityPropertyEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return false;
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return IsIdentityProperty(sclrProp, model);
        }
        List<string> GetDetailViews(ModelViewSerializable model, DbContextSerializable context, List<string> result)
        {
            if (result == null)
            {
                result = new List<string>();
            }
            if ((model == null) || (context == null))
            {
                return result;
            }
            if ((model.ScalarProperties == null) || (model.PrimaryKeyProperties == null) || (context.ModelViews == null))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1))
            {
                return result;
            }
            List<ModelViewPropertyOfVwSerializable> primKeys = GetModelPrimaryKeyProps(model);
            if (primKeys == null)
            {
                return result;
            }
            if (primKeys.Count != model.PrimaryKeyProperties.Count)
            {
                return result;
            }
            string RootEntityFullClassName = model.RootEntityFullClassName;
            string RootEntityUniqueProjectName = model.RootEntityUniqueProjectName;
            List<ModelViewSerializable> details =
                context.ModelViews.Where(m => m.ForeignKeys.Any(f => (f.NavigationEntityFullName == RootEntityFullClassName) && (f.NavigationEntityUniqueProjectName == RootEntityUniqueProjectName))).ToList();
            if (details.Count < 1)
            {
                return result;
            }
            foreach (ModelViewSerializable detail in details)
            {
                if (detail.ScalarProperties == null) continue;
                if (detail.ForeignKeys == null) continue;
                if (detail.ForeignKeys.Count < 1) continue;
                List<ModelViewForeignKeySerializable> ForeignKeys =
                    detail.ForeignKeys.Where(f => (f.NavigationEntityFullName == RootEntityFullClassName) && (f.NavigationEntityUniqueProjectName == RootEntityUniqueProjectName)).ToList();
                if (ForeignKeys.Count < 1) continue;
                bool canBeUsed = false;
                foreach (ModelViewForeignKeySerializable ForeignKey in ForeignKeys)
                {
                    bool hasForeignKeyProps = true;
                    if (ForeignKey.ForeignKeyProps != null)
                    {
                        for (int i = 0; i < ForeignKey.ForeignKeyProps.Count; i++)
                        {
                            ModelViewKeyPropertySerializable ForeignKeyProp = ForeignKey.ForeignKeyProps[i];
                            if (!(detail.ScalarProperties.Any(s => (s.OriginalPropertyName == ForeignKeyProp.OriginalPropertyName) && (string.IsNullOrEmpty(s.ForeignKeyNameChain)))))
                            {
                                hasForeignKeyProps = false;
                            }
                            if (!hasForeignKeyProps)
                            {
                                ModelViewKeyPropertySerializable PrincipalKeyProp = ForeignKey.PrincipalKeyProps[i];
                                if (detail.ScalarProperties.Any(s => (s.OriginalPropertyName == PrincipalKeyProp.OriginalPropertyName) && (s.ForeignKeyNameChain == ForeignKey.NavigationName)))
                                {
                                    hasForeignKeyProps = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        hasForeignKeyProps = false;
                    }
                    if (hasForeignKeyProps)
                    {
                        canBeUsed = true;
                        break;
                    }
                }
                if (canBeUsed)
                {
                    if (!result.Contains(detail.ViewName))
                    {
                        result.Add(detail.ViewName);
                    }
                }
            }
            return result;
        }
        ModelViewSerializable GetModelViewByName(DbContextSerializable context, string viewName)
        {
            if ((context == null) || (string.IsNullOrEmpty(viewName)))
            {
                return null;
            }
            if (context.ModelViews == null)
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
        }
        List<ModelViewForeignKeySerializable> GetDetailViewForeignKeys(ModelViewSerializable model, ModelViewSerializable detail, List<ModelViewForeignKeySerializable> result)
        {
            if (result == null) result = new List<ModelViewForeignKeySerializable>();
            if ((model == null) || (detail == null))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties == null) || (detail.ScalarProperties == null) || (detail.ForeignKeys == null))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1))
            {
                return result;
            }
            List<ModelViewPropertyOfVwSerializable> primKeys = GetModelPrimaryKeyProps(model);
            if (primKeys == null)
            {
                return result;
            }
            if (primKeys.Count != model.PrimaryKeyProperties.Count)
            {
                return result;
            }
            string RootEntityFullClassName = model.RootEntityFullClassName;
            string RootEntityUniqueProjectName = model.RootEntityUniqueProjectName;
            List<ModelViewForeignKeySerializable> ForeignKeys =
                detail.ForeignKeys.Where(f => (f.NavigationEntityFullName == RootEntityFullClassName) && (f.NavigationEntityUniqueProjectName == RootEntityUniqueProjectName)).ToList();
            if (ForeignKeys.Count < 1)
            {
                return result;
            }
            foreach (ModelViewForeignKeySerializable ForeignKey in ForeignKeys)
            {
                bool hasForeignKeyProps = true;
                if (ForeignKey.ForeignKeyProps != null)
                {
                    for (int i = 0; i < ForeignKey.ForeignKeyProps.Count; i++)
                    {
                        ModelViewKeyPropertySerializable ForeignKeyProp = ForeignKey.ForeignKeyProps[i];
                        hasForeignKeyProps =
                            detail.ScalarProperties.Any(s => (s.OriginalPropertyName == ForeignKeyProp.OriginalPropertyName) && (string.IsNullOrEmpty(s.ForeignKeyNameChain)));
                        if (!hasForeignKeyProps)
                        {
                            ModelViewKeyPropertySerializable PrincipalKeyProp = ForeignKey.PrincipalKeyProps[i];
                            hasForeignKeyProps = detail.ScalarProperties.Any(s => (s.OriginalPropertyName == PrincipalKeyProp.OriginalPropertyName) && (s.ForeignKeyNameChain == ForeignKey.NavigationName));
                            {
                                hasForeignKeyProps = true;
                            }
                            if (!hasForeignKeyProps)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    hasForeignKeyProps = false;
                }
                if (hasForeignKeyProps)
                {
                    result.Add(ForeignKey);
                    break;
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetDetailViewForeignKeyProps(ModelViewSerializable model, ModelViewPropertyOfVwSerializable primKey, ModelViewSerializable detail, ModelViewForeignKeySerializable ForeignKey, List<ModelViewPropertyOfVwSerializable> result)
        {
            if (result == null)
            {
                result = new List<ModelViewPropertyOfVwSerializable>();
            }
            if ((model == null) || (detail == null) || (primKey == null) || (ForeignKey == null))
            {
                return result;
            }
            if ((model.ScalarProperties == null) || (model.PrimaryKeyProperties == null) || (detail.ScalarProperties == null) || (detail.ForeignKeys == null) || (ForeignKey == null))
            {
                return result;
            }
            List<ModelViewPropertyOfVwSerializable> primKeys = GetModelPrimaryKeyProps(model);
            if (primKeys == null)
            {
                return result;
            }
            if (primKeys.Count < 1)
            {
                return result;
            }
            if (!primKeys.Any(p => (p.ViewPropertyName == primKey.ViewPropertyName) && (p.OriginalPropertyName == primKey.OriginalPropertyName)))
            {
                return result;
            }
            ModelViewForeignKeySerializable fk = detail.ForeignKeys.Where(f => f.NavigationName == ForeignKey.NavigationName).FirstOrDefault();
            if (fk == null)
            {
                return result;
            }
            ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializable =
                detail.ScalarProperties.Where(s => (s.OriginalPropertyName == primKey.OriginalPropertyName) && (s.ForeignKeyNameChain == fk.NavigationName)).FirstOrDefault();
            if (modelViewPropertyOfVwSerializable != null)
            {
                result.Add(modelViewPropertyOfVwSerializable);
            }
            if (fk.ForeignKeyProps == null)
            {
                return result;
            }

            int ind = fk.PrincipalKeyProps.FindIndex(p => p.OriginalPropertyName == primKey.OriginalPropertyName);
            if (ind < 0)
            {
                return result;
            }
            ModelViewKeyPropertySerializable ForeignKeyProp = ForeignKey.ForeignKeyProps[ind];
            ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializableEx =
                detail.ScalarProperties.FirstOrDefault(s => (s.OriginalPropertyName == ForeignKeyProp.OriginalPropertyName) && (string.IsNullOrEmpty(s.ForeignKeyNameChain)));
            if (modelViewPropertyOfVwSerializableEx != null)
            {
                if (modelViewPropertyOfVwSerializableEx != modelViewPropertyOfVwSerializable)
                {
                    result.Add(modelViewPropertyOfVwSerializableEx);
                }
            }
            return result;
        }
        List<ModelViewSerializable> CollectClientToMasterFieldsMapModelViews(ModelViewSerializable model, DbContextSerializable context, List<string> errors)
        {
            List<ModelViewSerializable> rslt = null;
            if ((model == null) || (context == null))
            {
                if (errors != null)
                {
                    errors.Add("// CollectClientToMasterFieldsMapModelViews: Input param is not defined");
                }
                return rslt;
            }
            if (context.ModelViews == null)
            {
                if (errors != null)
                {
                    errors.Add("// CollectClientToMasterFieldsMapModelViews: Input param is not defined : Context.ModelViews is empty");
                }
                return rslt;
            }
            foreach (ModelViewSerializable modelView in context.ModelViews)
            {
                if (modelView.ForeignKeys == null) continue;
                if (modelView.ForeignKeys.Any(f => f.ViewName == model.ViewName))
                {
                    if (rslt == null) rslt = new List<ModelViewSerializable>();
                    rslt.Add(modelView);
                }
            }
            return rslt;
        }
        bool IsUsedByForeignKey(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            if (model.ForeignKeys == null) return false;
            if (model.ForeignKeys.Count < 1) return false;
            if (string.IsNullOrEmpty(prop.ForeignKeyName))
            {
                foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
                {
                    if (fk.ForeignKeyProps != null)
                    {
                        if (fk.ForeignKeyProps.Any(k => k.OriginalPropertyName == prop.OriginalPropertyName)) return true;
                    }
                }
            }
            else if (prop.ForeignKeyName == prop.ForeignKeyNameChain)
            {
                ModelViewForeignKeySerializable fk01 = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyName).FirstOrDefault();
                if (fk01 == null) return false;
                if ((fk01.PrincipalKeyProps != null) && (fk01.ForeignKeyProps != null))
                {
                    if (fk01.PrincipalKeyProps.Count == fk01.ForeignKeyProps.Count)
                    {
                        if (fk01.PrincipalKeyProps.Any(k => k.OriginalPropertyName == prop.OriginalPropertyName)) return true;
                    }
                }
            }
            return false;
        }
        ///////////////////////////////////////////////////////////////
        bool IsDatabaseGeneratedProperty(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (HasAtribute(prop, "ConcurrencyCheck") || HasAtribute(prop, "Timestamp"))
            {
                return true;
            }
            if (HasAtributeWithValue(prop, "DatabaseGenerated", "identity") || HasAtributeWithValue(prop, "DatabaseGenerated", "computed"))
            {
                return true;
            }
            if (HasFluentAtribute(prop, new string[] { "UseSqlServerIdentityColumn", "ForSqlServerUseSequenceHiLo", "ValueGeneratedOnAdd", "ValueGeneratedOnAddOrUpdate", "IsConcurrencyToken", "IsRowVersion" }))
            {
                return true;
            }
            return HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "identity") || HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "computed");
        }
        bool IsDatabaseGeneratedPropertyEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return false;
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return IsDatabaseGeneratedProperty(sclrProp, model);
        }
        ModelViewPropertyOfVwSerializable GetScalarPropByOriginaPropName(string origPropName, ModelViewSerializable model)
        {
            if (string.IsNullOrEmpty(origPropName) || (model == null)) return null;
            if ((model.AllProperties == null) || (model.ScalarProperties == null)) return null;
            ModelViewPropertyOfVwSerializable sprop = model.ScalarProperties.Where(p =>
                    (p.OriginalPropertyName == origPropName) &&
                    string.IsNullOrEmpty(p.ForeignKeyName)).FirstOrDefault();
            if (sprop != null) return sprop;
            if (model.ForeignKeys == null) return null;
            if (model.ForeignKeys.Count < 1) return null;
            foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
            {
                if ((fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) continue;
                if ((fk.PrincipalKeyProps.Count != fk.ForeignKeyProps.Count) || (fk.ForeignKeyProps.Count < 1)) continue;
                for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
                {
                    if (fk.ForeignKeyProps[i].OriginalPropertyName == origPropName)
                    {
                        sprop = model.ScalarProperties.Where(p =>
                            (p.OriginalPropertyName == fk.PrincipalKeyProps[i].OriginalPropertyName) &&
                            (p.ForeignKeyName == fk.NavigationName) &&
                            (p.ForeignKeyName == p.ForeignKeyNameChain)
                        ).FirstOrDefault();
                        if (sprop != null) return sprop;
                    }
                }
            }
            return null;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelPrimaryKeyProps(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null)
            {
                return result;
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable modelViewKeyPropertySerializable in model.PrimaryKeyProperties)
            {
                ModelViewPropertyOfVwSerializable prop = GetScalarPropByOriginaPropName(modelViewKeyPropertySerializable.OriginalPropertyName, model);
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelUniqueKeyProps(ModelViewSerializable model, ModelViewUniqueKeySerializable uk)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((model == null) || (uk == null))
            {
                return result;
            }
            if ((uk.UniqueKeyProperties == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable modelViewKeyPropertySerializable in uk.UniqueKeyProperties)
            {
                ModelViewPropertyOfVwSerializable prop = GetScalarPropByOriginaPropName(modelViewKeyPropertySerializable.OriginalPropertyName, model);
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelAllUniqueKeysProps(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null)
            {
                return result;
            }
            if (model.UniqueKeys == null) return result;
            foreach (ModelViewUniqueKeySerializable uk in model.UniqueKeys)
            {
                List<ModelViewPropertyOfVwSerializable> ukprops = GetModelUniqueKeyProps(model, uk);
                foreach (ModelViewPropertyOfVwSerializable ukp in ukprops)
                {
                    if (!result.Any(p => p == ukp)) result.Add(ukp);
                }
            }
            return result;
        }
        ModelViewSerializable GetModelViewByViewName(DbContextSerializable context, string ViewName)
        {
            if (context == null) return null;
            if (context.ModelViews == null) return null;
            return context.ModelViews.Where(mv => mv.ViewName == ViewName).FirstOrDefault();
        }
        List<ModelViewForeignKeySerializable> CollectMasterToClientFieldsMap(ModelViewSerializable model, List<string> errors)
        {
            List<ModelViewForeignKeySerializable> rslt = null;
            if (model == null)
            {
                if (errors != null)
                {
                    errors.Add("// CollectMasterToClientFieldsMap: Input param is not defined");
                }
                return rslt;
            }
            if (model.ForeignKeys == null)
            {
                return rslt;
            }
            List<string> passedViews = new List<string>();
            foreach (ModelViewForeignKeySerializable mlFk in model.ForeignKeys)
            {
                if (string.IsNullOrEmpty(mlFk.ViewName))
                {
                    if (errors != null)
                    {
                        errors.Add("//");
                        errors.Add("// Error: CollectMasterToClientFieldsMap:");
                        errors.Add("//        Foreigkey is not completely defined");
                        errors.Add("//        ViewName = " + mlFk.ViewName);
                        errors.Add("//        NavigationName = " + mlFk.NavigationName);
                        errors.Add("//        NavigationEntityName = " + mlFk.NavigationEntityName);
                        errors.Add("//");
                    }
                    continue;
                }
                if (passedViews.Any(e => mlFk.ViewName == e))
                {
                    continue;
                }
                passedViews.Add(mlFk.ViewName);
                List<ModelViewForeignKeySerializable> intlpFks = model.ForeignKeys.Where(f => f.ViewName == mlFk.ViewName).ToList();
                foreach (ModelViewForeignKeySerializable intlpFk in intlpFks)
                {
                    if (
                        string.IsNullOrEmpty(intlpFk.NavigationName) ||
                        string.IsNullOrEmpty(intlpFk.NavigationEntityName) ||
                        string.IsNullOrEmpty(intlpFk.ViewName) ||
                        intlpFk.PrincipalKeyProps == null ||
                        intlpFk.ForeignKeyProps == null
                       )
                    {
                        if (errors != null)
                        {
                            errors.Add("//");
                            errors.Add("// Error: CollectMasterToClientFieldsMap:");
                            errors.Add("//        Foreigkey is not completely defined");
                            errors.Add("//        ViewName = " + intlpFk.ViewName);
                            errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                            errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                            errors.Add("//");
                        }
                        continue;
                    }
                    if (
                        (intlpFk.PrincipalKeyProps.Count != intlpFk.ForeignKeyProps.Count) ||
                        (intlpFk.PrincipalKeyProps.Count < 1) ||
                        (intlpFk.ForeignKeyProps.Count < 1)
                        )
                    {
                        if (errors != null)
                        {
                            errors.Add("//");
                            errors.Add("// Error: CollectMasterToClientFieldsMap:");
                            errors.Add("//        Foreigkey is not completely defined");
                            errors.Add("//        ViewName = " + intlpFk.ViewName);
                            errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                            errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                            errors.Add("//");
                        }
                        continue;
                    }
                    if (rslt == null)
                    {
                        rslt = new List<ModelViewForeignKeySerializable>();
                    }
                    rslt.Add(intlpFk);
                }
            }
            return rslt;
        }
        ModelViewPropertyOfVwSerializable GetModelScalarPropByKeyProp(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        {
            //ModelViewPropertyOfVwSerializable rslt = null;
            if ((model == null) || (pk == null)) return null;
            if (model.ScalarProperties == null) return null;
            if (model.AllProperties == null) return null;
            ModelViewEntityPropertySerializable aprop = model.AllProperties.Where(p => p.OriginalPropertyName == pk.OriginalPropertyName).FirstOrDefault();
            if (aprop == null) return null;
            ModelViewPropertyOfVwSerializable scProp =
                model.ScalarProperties.Where(p => ((p.OriginalPropertyName == pk.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))).FirstOrDefault();
            if (scProp != null) return scProp;
            if (model.ForeignKeys != null)
            {
                foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
                {
                    scProp = null;
                    if ((fk.ForeignKeyProps != null) && (fk.PrincipalKeyProps != null))
                    {
                        int cnt = fk.ForeignKeyProps.Count;
                        if (cnt < fk.PrincipalKeyProps.Count)
                        {
                            cnt = fk.PrincipalKeyProps.Count;
                        }
                        for (int i = 0; i < cnt; i++)
                        {
                            if (fk.ForeignKeyProps[i].OriginalPropertyName == pk.OriginalPropertyName)
                            {
                                scProp =
                                    model.ScalarProperties.Where(p =>
                                    ((p.OriginalPropertyName == fk.PrincipalKeyProps[i].OriginalPropertyName) && (p.ForeignKeyNameChain == fk.NavigationName))).FirstOrDefault();
                            }
                            if (scProp != null) return scProp;
                        }
                    }
                }
            }
            return null;
        }
        string GetModelPropertyNameByKeyProperty(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        {
            ModelViewPropertyOfVwSerializable prop = GetModelScalarPropByKeyProp(model, pk);
            return GetModelPropertyName(prop, model);
        }
        ModelViewPropertyOfVwSerializable GetScalarPropertyByViewPropertyName(ModelViewSerializable model, string viewPropertyName)
        {
            if ((model == null) || (string.IsNullOrEmpty(viewPropertyName))) return null;
            return model.ScalarProperties.Where(p => p.ViewPropertyName == viewPropertyName).FirstOrDefault();
        }
        ModelViewPropertyOfVwSerializable GetDirectMasterScalarPropertyByViewPropertyName(ModelViewSerializable model, string viewPropertyName, DbContextSerializable context)
        {
            if (context == null) return null;
            ModelViewPropertyOfVwSerializable prop = GetScalarPropertyByViewPropertyName(model, viewPropertyName);
            if ((prop == null) || (model.ForeignKeys == null) || (context.ModelViews == null)) return null;
            if (string.IsNullOrEmpty(prop.ForeignKeyName) || string.IsNullOrEmpty(prop.ForeignKeyNameChain)) return null;
            ModelViewForeignKeySerializable fk = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyName).FirstOrDefault();
            if (fk == null) return null;
            ModelViewSerializable masterVw = context.ModelViews.Where(m => m.ViewName == fk.ViewName).FirstOrDefault();
            if (masterVw == null) return null;
            //ModelViewPropertyOfVwSerializable masterProp = null;
            if (prop.ForeignKeyName == prop.ForeignKeyNameChain)
            {
                return masterVw.ScalarProperties.Where(p => ((p.OriginalPropertyName == prop.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain))).FirstOrDefault();
            }
            else
            {
                string flt = "";
                if (prop.ForeignKeyNameChain.StartsWith(prop.ForeignKeyName + "."))
                {
                    flt = prop.ForeignKeyNameChain.Substring(prop.ForeignKeyName.Length + 1);
                }
                return masterVw.ScalarProperties.Where(p => ((p.OriginalPropertyName == prop.OriginalPropertyName) && (p.ForeignKeyNameChain == flt))).FirstOrDefault();
            }
        }
        bool CheckModelIfIndexIsCorrect(ModelViewSerializable model, ModelViewUniqueKeyOfVwSerializable indx, out string error)
        {
            if ((model == null) || (indx == null))
            {
                error = "Input params is not defined";
                return false;
            }
            if (indx.UniqueKeyProperties == null)
            {
                error = "UniqueKeyProperties of the Index are not defined";
                return false;
            }
            if (indx.UniqueKeyProperties.Count < 1)
            {
                if (indx.IsPrimary)
                    error = "UniqueKeyProperties.Count of the Primary Index is less than 1";
                else
                    error = "UniqueKeyProperties.Count of the Unique Index (UniqueKeyName == " + indx.UniqueKeyName + ") is less than 1";
                return false;
            }

            if (indx.IsPrimary)
            {
                if (model.PrimaryKeyProperties == null)
                {
                    error = "PrimaryKeyProperties of the model are not defined";
                    return false;
                }
                if (model.PrimaryKeyProperties.Count != indx.UniqueKeyProperties.Count)
                {
                    error = "Not all Index fields are included in the Model";
                    return false;
                }
            }
            else
            {
                if (model.UniqueKeys == null)
                {
                    error = "UniqueKeys of the model are not defined (UniqueKeyName == " + indx.UniqueKeyName + ")";
                    return false;
                }
                if (string.IsNullOrEmpty(indx.UniqueKeyName))
                {
                    error = "The Name of the Index is not defined (UniqueKeyName)";
                    return false;
                }
                ModelViewUniqueKeySerializable mindx = model.UniqueKeys.Where(i => i.UniqueKeyName == indx.UniqueKeyName).FirstOrDefault();
                if (mindx == null)
                {
                    error = "Could not find index in model by name (Unique Index Name == " + indx.UniqueKeyName + ")";
                    return false;
                }
                if (mindx.UniqueKeyProperties == null)
                {
                    error = "UniqueKeyProperties of the Unique Index (Unique Index Name == " + indx.UniqueKeyName + ") are not defined";
                    return false;
                }
                if (mindx.UniqueKeyProperties.Count != indx.UniqueKeyProperties.Count)
                {
                    error = "Not all Unique Index fields are included in the Model (Unique Index Name == " + indx.UniqueKeyName + ")";
                    return false;
                }
            }
            error = "";
            return true;
        }
        List<ModelViewForeignKeySerializable> CollectMasterToClientFieldsMapForMasterView(ModelViewSerializable clentModel, string masterViewName, List<string> errors)
        {
            List<ModelViewForeignKeySerializable> rslt = null;
            if (clentModel == null)
            {
                if (errors != null)
                {
                    errors.Add("// CollectMasterToClientFieldsMapForMasterView: Input param is not defined");
                }
                return rslt;
            }
            if ((clentModel.ForeignKeys == null) || (string.IsNullOrEmpty(masterViewName)))
            {
                return rslt;
            }
            List<ModelViewForeignKeySerializable> intlpFks = clentModel.ForeignKeys.Where(f => f.ViewName == masterViewName).ToList();
            foreach (ModelViewForeignKeySerializable intlpFk in intlpFks)
            {
                if (
                    string.IsNullOrEmpty(intlpFk.NavigationName) ||
                    string.IsNullOrEmpty(intlpFk.NavigationEntityName) ||
                    string.IsNullOrEmpty(intlpFk.ViewName) ||
                    intlpFk.PrincipalKeyProps == null ||
                    intlpFk.ForeignKeyProps == null
                    )
                {
                    if (errors != null)
                    {
                        errors.Add("//");
                        errors.Add("// Error: CollectMasterToClientFieldsMapForMasterView:");
                        errors.Add("//        Foreigkey is not completely defined");
                        errors.Add("//        ViewName = " + intlpFk.ViewName);
                        errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                        errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                        errors.Add("//");
                    }
                    continue;
                }
                if (
                    (intlpFk.PrincipalKeyProps.Count != intlpFk.ForeignKeyProps.Count) ||
                    (intlpFk.PrincipalKeyProps.Count < 1) ||
                    (intlpFk.ForeignKeyProps.Count < 1)
                    )
                {
                    if (errors != null)
                    {
                        errors.Add("//");
                        errors.Add("// Error: CollectMasterToClientFieldsMapForMasterView:");
                        errors.Add("//        Foreigkey is not completely defined");
                        errors.Add("//        ViewName = " + intlpFk.ViewName);
                        errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                        errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                        errors.Add("//");
                    }
                    continue;
                }
                if (rslt == null)
                {
                    rslt = new List<ModelViewForeignKeySerializable>();
                }
                rslt.Add(intlpFk);
            }
            return rslt;
        }
        List<ModelViewUniqueKeyOfVwSerializable> GetModelUniqueKeys(ModelViewSerializable model, List<ModelViewUniqueKeyOfVwSerializable> rsltKeys)
        {
            if ((model == null) || (rsltKeys == null)) return rsltKeys;
            if ((model.UniqueKeys == null) || (model.ScalarProperties == null)) return rsltKeys;
            foreach (ModelViewUniqueKeySerializable uk in model.UniqueKeys)
            {
                if (uk.UniqueKeyProperties == null) continue;
                if (uk.UniqueKeyProperties.Count < 1) continue;
                List<ModelViewPropertyOfVwSerializable> ukprops = GetModelUniqueKeyProps(model, uk);
                if (ukprops.Count == uk.UniqueKeyProperties.Count)
                {
                    rsltKeys.Add(new ModelViewUniqueKeyOfVwSerializable()
                    {
                        UniqueKeyName = uk.UniqueKeyName,
                        IsPrimary = false,
                        UniqueKeyProperties = ukprops
                    });
                }
            }
            return rsltKeys;
        }
        ModelViewUniqueKeyOfVwSerializable GetModelPrimaryKey(ModelViewSerializable model)
        {
            if (model == null) return null;
            if (model.PrimaryKeyProperties == null) return null;
            if (model.PrimaryKeyProperties.Count < 1) return null;
            List<ModelViewPropertyOfVwSerializable> props = GetModelPrimaryKeyProps(model);
            if (props.Count != model.PrimaryKeyProperties.Count) return null;
            return new ModelViewUniqueKeyOfVwSerializable()
            {
                UniqueKeyName = null,
                IsPrimary = true,
                UniqueKeyProperties = props
            };
        }
        ModelViewUniqueKeyOfVwSerializable GetModelPrimKeyFromList(List<ModelViewUniqueKeyOfVwSerializable> inuniqueKeys)
        {
            if (inuniqueKeys == null) return null;
            return inuniqueKeys.Where(u => u.IsPrimary).FirstOrDefault();
        }
        ModelViewEntityPropertySerializable GetRootEntityProperty(ModelViewPropertySerializable prop, ModelViewSerializable model)
        {
            if (string.IsNullOrEmpty(prop.ForeignKeyNameChain))
            {
                if (model.AllProperties != null)
                {
                    return model.AllProperties.Where(p => p.OriginalPropertyName == prop.OriginalPropertyName).FirstOrDefault();
                }
                return null;
            }
            if (model.ForeignKeys == null) return null;
            ModelViewForeignKeySerializable fk = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyNameChain).FirstOrDefault();
            if (fk == null) return null;
            if ((fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) return null;
            int cnt = fk.PrincipalKeyProps.Count;
            if (cnt > fk.ForeignKeyProps.Count) cnt = fk.ForeignKeyProps.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (fk.PrincipalKeyProps[i].OriginalPropertyName == prop.OriginalPropertyName)
                {
                    return model.AllProperties.Where(p => p.OriginalPropertyName == fk.ForeignKeyProps[i].OriginalPropertyName).FirstOrDefault();
                }
            }
            return null;
        }
        ModelViewUniqueKeyOfVwSerializable GetIndexByEntityProps(List<ModelViewUniqueKeyOfVwSerializable> inuniqueKeys, List<ModelViewKeyPropertySerializable> entityProps, ModelViewSerializable model)
        {
            if ((inuniqueKeys == null) || (entityProps == null)) return null;
            if (entityProps.Count < 1) return null;
            foreach (ModelViewUniqueKeyOfVwSerializable uk in inuniqueKeys)
            {
                if (uk.UniqueKeyProperties == null) continue;
                if (uk.UniqueKeyProperties.Count != entityProps.Count) continue;
                bool isFound = true;
                foreach (ModelViewPropertyOfVwSerializable ukp in uk.UniqueKeyProperties)
                {
                    ModelViewEntityPropertySerializable entityProp = GetRootEntityProperty(ukp, model);
                    if (!entityProps.Any(p => p.OriginalPropertyName == entityProp.OriginalPropertyName))
                    {
                        isFound = false;
                        break;
                    }
                }
                if (isFound) return uk;
            }
            return null;
        }
        bool IsBooleanTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null) return false;
            return
                "System.Boolean".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) ||
                "Boolean".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) ||
                "bool".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase);
        }
        bool IsStringTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null) return false;
            return ("System.String".Equals(prop.UnderlyingTypeName) || "String".Equals(prop.UnderlyingTypeName) || "string".Equals(prop.UnderlyingTypeName));
        }
        List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> GetCurrentDirectNavList(List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList, string chain)
        {
            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> rslt = new List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>>();
            if (foreignKeyNameChainList == null) return rslt;
            int len = 1;
            string[] fks = null;
            if (!string.IsNullOrEmpty(chain))
            {
                fks = chain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                len = fks.Length + 1;
            }
            foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in foreignKeyNameChainList)
            {
                fks = fk.Item1.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (fks.Length != len) continue;
                if (len == 1)
                {
                    rslt.Add(fk);
                }
                else
                {
                    if (fk.Item1.StartsWith(chain + "."))
                    {
                        rslt.Add(fk);
                    }
                }
            }
            return rslt;
        }
        string GetCurrentDirectNavs(List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList, string chain)
        {
            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> lst = GetCurrentDirectNavList(foreignKeyNameChainList, chain);
            int len = 0;
            if (!string.IsNullOrEmpty(chain))
            {
                len = chain.Length + 1;
            }
            string rslt = "";
            if (len == 0)
            {
                foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in lst)
                {
                    if (rslt != "") rslt += ", ";
                    rslt += "\"" + fk.Item1 + "\"";
                }
            }
            else
            {
                foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in lst)
                {
                    if (rslt != "") rslt += ", ";
                    rslt += "\"" + fk.Item1.Substring(len) + "\"";
                }
            }
            return rslt;
        }
        ModelViewSerializable GetViewByForeignNameChainEx(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return null;
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (mv == null)
            {
                return mv;
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return mv;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (foreignKeys.Length < 1)
            {
                return mv;
            }
            ModelViewForeignKeySerializable fk = mv.ForeignKeys.Where(f => f.NavigationName == foreignKeys[0]).FirstOrDefault();
            if (fk == null)
            {
                return mv;
            }
            if (foreignKeys.Length == 1)
            {
                return GetViewByForeignNameChainEx(context, fk.ViewName, "");
            }
            return GetViewByForeignNameChainEx(context, fk.ViewName, string.Join(".", foreignKeys, 1, foreignKeys.Length - 1));
        }
        List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> GetForeignKeyNameChainStructList(ModelViewSerializable model,
                                                                                            DbContextSerializable context,
                                                                                            int currentInputTypeId,
                                                                                            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList)
        {
            if ((model == null) || (context == null) || (foreignKeyNameChainList == null))
            {
                return foreignKeyNameChainList;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return foreignKeyNameChainList;
            }
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if (!modelViewUIFormPropertySerializable.IsShownInView) continue;
                if (currentInputTypeId == 1)
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Typeahead))
                    {
                        mv = GetViewByForeignNameChainEx(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        foreignKeyNameChainList.Add(new Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>(
                                modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                                mv,
                                modelViewUIFormPropertySerializable,
                                modelViewUIFormPropertySerializable.InputTypeWhenAdd));
                    }
                }
                else if (currentInputTypeId == 2)
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Typeahead))
                    {
                        mv = GetViewByForeignNameChainEx(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        foreignKeyNameChainList.Add(new Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>(
                                modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                                mv,
                                modelViewUIFormPropertySerializable,
                                modelViewUIFormPropertySerializable.InputTypeWhenUpdate));
                    }
                }
                else
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Typeahead))
                    {
                        mv = GetViewByForeignNameChainEx(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        foreignKeyNameChainList.Add(new Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>(
                                modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                                mv,
                                modelViewUIFormPropertySerializable,
                                modelViewUIFormPropertySerializable.InputTypeWhenDelete));
                    }
                }
            }
            return foreignKeyNameChainList;
        }
        Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> GetDirectDetail(List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList, string chain)
        {
            if (foreignKeyNameChainList == null) return null;
            if (string.IsNullOrEmpty(chain)) return null; // this is RootDataSource
            string[] fks = chain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            int len = fks.Length;
            if (len == 1) return null; // direct detail is RootDataSource
            foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in foreignKeyNameChainList)
            {
                if (chain.StartsWith(fk.Item1 + "."))
                {
                    fks = fk.Item1.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fks.Length == len - 1) return fk;
                }
            }
            return null;
        }
        string GetDirectNavName(string chain)
        {
            if (string.IsNullOrEmpty(chain)) return "";
            string[] fks = chain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return fks[fks.Length - 1];
        }
        bool IsMasterDefinedProperty(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            if (!string.IsNullOrEmpty(prop.ForeignKeyName)) return true;
            if (model.ForeignKeys == null) return false;
            foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
            {
                if (fk.ForeignKeyProps != null)
                {
                    if (fk.ForeignKeyProps.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName)) return true;
                }
            }
            return false;
        }
        bool IsTableMatchesIndex(ModelViewSerializable model)
        {
            if (model == null) return false;
            if ((model.ScalarProperties == null) || (model.PrimaryKeyProperties == null)) return false;
            if ((model.ScalarProperties.Count != model.PrimaryKeyProperties.Count) || (model.ScalarProperties.Count < 1)) return false;
            foreach (ModelViewKeyPropertySerializable pkp in model.PrimaryKeyProperties)
            {
                if (GetScalarPropByOriginaPropName(pkp.OriginalPropertyName, model) == null) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToTailOfPrimKey(ModelViewForeignKeySerializable fk, ModelViewSerializable model)
        {
            if ((model == null) || (fk == null)) return false;
            if ((model.PrimaryKeyProperties == null) || (model.ForeignKeys == null) || (fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) return false;
            if ((model.PrimaryKeyProperties.Count < 1) || (fk.PrincipalKeyProps.Count != fk.ForeignKeyProps.Count) || (fk.ForeignKeyProps.Count < 1) || (fk.ForeignKeyProps.Count >= model.PrimaryKeyProperties.Count)) return false;
            for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
            {
                if (fk.ForeignKeyProps[fk.ForeignKeyProps.Count - (1 + i)].OriginalPropertyName != model.PrimaryKeyProperties[model.PrimaryKeyProperties.Count - (1 + i)].OriginalPropertyName) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToScalars(ModelViewForeignKeySerializable fk, ModelViewSerializable model)
        {
            if ((model == null) || (fk == null)) return false;
            if ((model.ScalarProperties == null) || (fk.ForeignKeyProps == null)) return false;
            if (fk.ForeignKeyProps.Count < 1) return false;
            foreach (ModelViewKeyPropertySerializable fkp in fk.ForeignKeyProps)
            {
                if (GetScalarPropByOriginaPropName(fkp.OriginalPropertyName, model) == null) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToScalarsEx(ModelViewForeignKeySerializable detailFk, ModelViewSerializable detailModel, ModelViewSerializable model)
        {
            if ((detailModel == null) || (detailFk == null) || (model == null)) return false;
            if ((detailModel.ScalarProperties == null) || (detailFk.ForeignKeyProps == null) || (model.ScalarProperties == null) || (detailFk.PrincipalKeyProps == null) || (model.PrimaryKeyProperties == null)) return false;
            if ((detailFk.ForeignKeyProps.Count < 1) || (model.ScalarProperties.Count < 1) || (model.PrimaryKeyProperties.Count != detailFk.ForeignKeyProps.Count) || (detailFk.PrincipalKeyProps.Count != detailFk.ForeignKeyProps.Count) || (model.PrimaryKeyProperties.Count != detailFk.ForeignKeyProps.Count)) return false;
            for (int i = 0; i < detailFk.ForeignKeyProps.Count; i++)
            {
                ModelViewPropertyOfVwSerializable detailsprp = GetScalarPropByOriginaPropName(detailFk.ForeignKeyProps[i].OriginalPropertyName, detailModel);
                if (detailsprp == null) return false;
                ModelViewPropertyOfVwSerializable modelsprp = GetScalarPropByOriginaPropName(detailFk.PrincipalKeyProps[i].OriginalPropertyName, model);
                if (modelsprp == null) return false;
                if (modelsprp.ViewPropertyName != detailsprp.ViewPropertyName) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToScalarsExEx(ModelViewForeignKeySerializable detailFk, ModelViewSerializable detailModel, ModelViewSerializable model)
        {
            if ((detailModel == null) || (detailFk == null) || (model == null)) return false;
            if ((detailModel.ScalarProperties == null) || (detailFk.ForeignKeyProps == null) || (model.ScalarProperties == null) || (detailFk.PrincipalKeyProps == null) || (model.ForeignKeys == null)) return false;
            if ((detailFk.ForeignKeyProps.Count < 1) || (model.ScalarProperties.Count < 1) || (detailFk.PrincipalKeyProps.Count != detailFk.ForeignKeyProps.Count)) return false;
            List<ModelViewForeignKeySerializable> modelFks = model.ForeignKeys.Where(f => f.ViewName == detailFk.ViewName).ToList();
            if (modelFks.Count < 1) return false;
            for (int i = 0; i < detailFk.ForeignKeyProps.Count; i++)
            {
                ModelViewPropertyOfVwSerializable detailsprp = GetScalarPropByOriginaPropName(detailFk.ForeignKeyProps[i].OriginalPropertyName, detailModel);
                if (detailsprp == null) return false;
            }
            foreach (ModelViewForeignKeySerializable modelFk in modelFks)
            {
                if (modelFk.ForeignKeyProps == null) continue;
                if (modelFk.ForeignKeyProps.Count != detailFk.ForeignKeyProps.Count) continue;
                bool passed = false;
                for (int i = 0; i < detailFk.ForeignKeyProps.Count; i++)
                {
                    ModelViewPropertyOfVwSerializable detailsprp = GetScalarPropByOriginaPropName(detailFk.ForeignKeyProps[i].OriginalPropertyName, detailModel);
                    ModelViewPropertyOfVwSerializable modelsprp = GetScalarPropByOriginaPropName(modelFk.ForeignKeyProps[i].OriginalPropertyName, model);
                    passed = modelsprp != null;
                    if (!passed) break;
                    passed = modelsprp.ViewPropertyName == detailsprp.ViewPropertyName;
                    if (!passed) break;
                }
                if (passed) return true;
            }
            return false;
        }
        bool IsOnePropForeigKey(ModelViewForeignKeySerializable searchFk)
        {
            if (searchFk == null) return false;
            if ((searchFk.PrincipalKeyProps == null) || (searchFk.ForeignKeyProps == null)) return false;
            if ((searchFk.PrincipalKeyProps.Count == searchFk.ForeignKeyProps.Count) && (searchFk.ForeignKeyProps.Count == 1)) return true;
            return false;
        }
        bool IsLookUpTable(ModelViewSerializable searchMdl)
        {
            if (searchMdl == null) return false;
            if ((searchMdl.ScalarProperties == null) || (searchMdl.PrimaryKeyProperties == null) || (searchMdl.UniqueKeys == null)) return false;
            if ((searchMdl.ScalarProperties.Count != 2) || (searchMdl.PrimaryKeyProperties.Count != 1) || (searchMdl.UniqueKeys.Count != 1)) return false;
            if (searchMdl.UniqueKeys[0].UniqueKeyProperties == null) return false;
            if (searchMdl.UniqueKeys[0].UniqueKeyProperties.Count != 1) return false;
            if (searchMdl.UniqueKeys[0].UniqueKeyProperties[0].OriginalPropertyName == searchMdl.PrimaryKeyProperties[0].OriginalPropertyName) return false;
            if ((GetScalarPropByOriginaPropName(searchMdl.UniqueKeys[0].UniqueKeyProperties[0].OriginalPropertyName, searchMdl) == null) ||
                (GetScalarPropByOriginaPropName(searchMdl.PrimaryKeyProperties[0].OriginalPropertyName, searchMdl) == null)) return false;
            return true;
        }
        bool IsUniqKeyMapedToScalarsEx(ModelViewUniqueKeySerializable searchUk, ModelViewSerializable searchModel, ModelViewSerializable model)
        {
            if ((searchModel == null) || (searchUk == null) || (model == null)) return false;
            if ((searchModel.ScalarProperties == null) || (searchUk.UniqueKeyProperties == null) || (model.ScalarProperties == null)) return false;
            if ((searchUk.UniqueKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1)) return false;
            foreach (ModelViewKeyPropertySerializable ukp in searchUk.UniqueKeyProperties)
            {
                ModelViewPropertyOfVwSerializable sprp = GetScalarPropByOriginaPropName(ukp.OriginalPropertyName, searchModel);
                if (sprp == null) return false;
                if (!model.ScalarProperties.Any(p => p.ViewPropertyName == sprp.ViewPropertyName)) return false;
            }
            return true;
        }
        int GetForeigKeyMaxPropsPosition(ModelViewForeignKeySerializable otherFk, ModelViewSerializable m2mMdl)
        {
            if ((m2mMdl == null) || (otherFk == null)) return -1;
            if ((m2mMdl.PrimaryKeyProperties == null) || (otherFk.ForeignKeyProps == null)) return -1;
            if ((m2mMdl.PrimaryKeyProperties.Count < 1) || (otherFk.ForeignKeyProps.Count < 1)) return -1;
            int rslt = 0;
            foreach (ModelViewKeyPropertySerializable fkp in otherFk.ForeignKeyProps)
            {
                int index = m2mMdl.PrimaryKeyProperties.FindIndex(delegate (ModelViewKeyPropertySerializable pkp) { return pkp.OriginalPropertyName == fkp.OriginalPropertyName; });
                if (index < 0) return -1;
                if (rslt < index) rslt = index;
            }
            return rslt;
        }
        bool IsForeigKeyMapedToPrimKey(ModelViewForeignKeySerializable fk, ModelViewSerializable model)
        {
            if ((model == null) || (fk == null)) return false;
            if ((model.PrimaryKeyProperties == null) || (model.ForeignKeys == null) || (fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) return false;
            if ((model.PrimaryKeyProperties.Count < 1) || (fk.PrincipalKeyProps.Count != fk.ForeignKeyProps.Count) || (fk.ForeignKeyProps.Count < 1) || (fk.ForeignKeyProps.Count >= model.PrimaryKeyProperties.Count)) return false;
            foreach (ModelViewKeyPropertySerializable fkp in fk.ForeignKeyProps)
            {
                if (!model.PrimaryKeyProperties.Any(p => p.OriginalPropertyName == fkp.OriginalPropertyName)) return false;
            }
            return true;
        }
        bool IsForeigKeyWithCorrectPropsOrder(ModelViewForeignKeySerializable otherFk, ModelViewSerializable m2mMdl)
        {
            if ((m2mMdl == null) || (otherFk == null)) return false;
            if ((m2mMdl.PrimaryKeyProperties == null) || (otherFk.ForeignKeyProps == null)) return false;
            if ((m2mMdl.PrimaryKeyProperties.Count < 1) || (otherFk.ForeignKeyProps.Count < 1)) return false;
            List<int> positions = new List<int>();
            foreach (ModelViewKeyPropertySerializable fkp in otherFk.ForeignKeyProps)
            {
                int index = m2mMdl.PrimaryKeyProperties.FindIndex(delegate (ModelViewKeyPropertySerializable pkp) { return pkp.OriginalPropertyName == fkp.OriginalPropertyName; });
                if (index < 0) return false;
                positions.Add(index);
            }
            positions.Sort();
            for (int i = 0; i < positions.Count - 1; i++)
            {
                if (positions[i] + 1 != positions[i + 1]) return false;
            }
            return true;
        }
        List<Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>> GetSearchResources(ModelViewSerializable model, DbContextSerializable context)
        {
            if ((context == null) || (model == null)) return null;
            if ((context.ModelViews == null) || (model.PrimaryKeyProperties == null) || (model.ScalarProperties == null)) return null;
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1)) return null;
            List<ModelViewSerializable> m2mMdls = context.ModelViews.Where(p => (p.ForeignKeys.Any(f => f.ViewName == model.ViewName) && (p.ForeignKeys.Count > 1))).ToList();
            if (m2mMdls.Count < 1)
            {
                return null;
            }
            // m2mModel, m2mForeignKey, List<Tuple< model.ForeignKey, m2mModel.additionalForeignKey >>, searchModel, searchFk, searchUk, ukpropsMpt, ukpropsToFrgn
            List<Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>> result = null;
            foreach (ModelViewSerializable m2mMdl in m2mMdls)
            {
                if (!IsTableMatchesIndex(m2mMdl))
                {
                    continue;
                }
                List<ModelViewForeignKeySerializable> m2mFks = m2mMdl.ForeignKeys.Where(f => f.ViewName == model.ViewName).ToList();
                foreach (ModelViewForeignKeySerializable m2mFk in m2mFks)
                {
                    if (!IsForeigKeyMapedToTailOfPrimKey(m2mFk, m2mMdl)) continue;
                    if (!IsForeigKeyMapedToScalarsEx(m2mFk, m2mMdl, model)) continue;
                    // m2mMdl - m2mModel, m2mFk - m2mForeignKey,
                    List<KeyValuePair<ModelViewForeignKeySerializable, int>> searchFks = null;
                    //List<int> searchFkPosition = null;
                    foreach (ModelViewForeignKeySerializable searchFk in m2mMdl.ForeignKeys)
                    {
                        if (m2mFk == searchFk) continue;
                        if (!IsOnePropForeigKey(searchFk)) continue;
                        ModelViewSerializable searchMdl = context.ModelViews.Where(mv => (mv.ViewName == searchFk.ViewName)).FirstOrDefault();
                        if (searchMdl == null) continue;
                        if (!IsLookUpTable(searchMdl)) continue;
                        if (!IsUniqKeyMapedToScalarsEx(searchMdl.UniqueKeys[0], searchMdl, model)) continue;
                        if (searchFks == null) searchFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                        searchFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(searchFk, GetForeigKeyMaxPropsPosition(searchFk, m2mMdl)));
                    }
                    if (searchFks == null)
                    {
                        continue;
                    }
                    int lastValidPosition = m2mMdl.PrimaryKeyProperties.Count - model.PrimaryKeyProperties.Count - 1;
                    if (searchFks != null)
                    {
                        if (searchFks.Any(p => p.Value < 0)) continue;
                        searchFks = searchFks.OrderBy(p => p.Value).ToList();
                        bool IsCorrect = true;
                        for (int i = 0; i < searchFks.Count - 1; i++)
                        {
                            IsCorrect = searchFks[i].Value == searchFks[i + 1].Value - 1;
                            if (!IsCorrect) break;
                        }
                        if (!IsCorrect)
                        {
                            continue;
                        }
                        if (searchFks[searchFks.Count - 1].Value != lastValidPosition)
                        {
                            continue;
                        }
                        lastValidPosition = searchFks[0].Value - 1;
                    }


                    List<KeyValuePair<ModelViewForeignKeySerializable, int>> otherFks = null;
                    foreach (ModelViewForeignKeySerializable otherFk in m2mMdl.ForeignKeys)
                    {
                        if (m2mFk == otherFk) continue;
                        if (searchFks != null)
                        {
                            if (searchFks.Any(p => p.Key == otherFk)) continue;
                        }
                        if (!IsForeigKeyMapedToPrimKey(otherFk, m2mMdl)) continue;
                        if (!IsForeigKeyMapedToScalarsExEx(otherFk, m2mMdl, model)) continue;
                        if (!IsForeigKeyWithCorrectPropsOrder(otherFk, m2mMdl)) continue;
                        int mxPs = GetForeigKeyMaxPropsPosition(otherFk, m2mMdl);
                        if ((mxPs < 0) || (mxPs > lastValidPosition)) continue;
                        if (otherFks == null) otherFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                        otherFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(otherFk, mxPs));
                    }
                    if (otherFks != null)
                    {
                        otherFks = otherFks.OrderBy(p => p.Value).ToList();
                        bool IsCorrect = true;
                        for (int i = 0; i < otherFks.Count - 1; i++)
                        {
                            IsCorrect = otherFks[i].Value == otherFks[i + 1].Value - otherFks[i].Key.ForeignKeyProps.Count;
                            if (!IsCorrect) break;
                        }
                        if (!IsCorrect) continue;
                        if (otherFks[otherFks.Count - 1].Value != lastValidPosition)
                            continue;
                        lastValidPosition = otherFks[0].Value - otherFks[0].Key.ForeignKeyProps.Count;
                    }


                    List<KeyValuePair<ModelViewForeignKeySerializable, int>> externalFks = null;
                    foreach (ModelViewForeignKeySerializable externalFk in m2mMdl.ForeignKeys)
                    {
                        if (externalFk == m2mFk) continue;
                        if (searchFks != null)
                        {
                            if (searchFks.Any(p => p.Key == externalFk)) continue;
                        }
                        if (otherFks != null)
                        {
                            if (otherFks.Any(p => p.Key == externalFk)) continue;
                        }
                        if (!IsForeigKeyMapedToPrimKey(externalFk, m2mMdl)) continue;
                        if (!IsForeigKeyMapedToScalars(externalFk, m2mMdl)) continue;
                        if (!IsForeigKeyWithCorrectPropsOrder(externalFk, m2mMdl)) continue;
                        int mxPs = GetForeigKeyMaxPropsPosition(externalFk, m2mMdl);
                        if ((mxPs < 0) || (mxPs > lastValidPosition)) continue;
                        if (externalFks == null) externalFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                        externalFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(externalFk, mxPs));
                    }
                    if (externalFks != null)
                    {
                        externalFks = externalFks.OrderBy(p => p.Value).ToList();
                        bool IsCorrect = true;
                        for (int i = 0; i < externalFks.Count - 1; i++)
                        {
                            IsCorrect = externalFks[i].Value == externalFks[i + 1].Value - externalFks[i].Key.ForeignKeyProps.Count;
                            if (!IsCorrect) break;
                        }
                        if (!IsCorrect) continue;
                        if (externalFks[externalFks.Count - 1].Value != lastValidPosition) continue;
                        lastValidPosition = externalFks[0].Value - externalFks[0].Key.ForeignKeyProps.Count;
                    }
                    if (lastValidPosition != -1) continue;
                    int AllFkCount = (searchFks == null ? 0 : searchFks.Count) +
                                    (otherFks == null ? 0 : otherFks.Count) +
                                    (externalFks == null ? 0 : externalFks.Count);
                    if (AllFkCount != (m2mMdl.ForeignKeys.Count - 1)) continue;
                    if (result == null)
                    {
                        result = new List<Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>>();
                    }
                    result.Add(new Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>(
                        m2mMdl, m2mFk, searchFks, otherFks, externalFks
                    ));
                }
            }
            return result;
        }
        ModelViewSerializable GetViewByName(DbContextSerializable context, string ViewName)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
        }
        ModelViewPropertyOfVwSerializable GetFirstPropOfFirstUniqueKey(ModelViewSerializable model)
        {
            if (model == null) return null;
            if (model.UniqueKeys == null) return null;
            if (model.UniqueKeys.Count < 1) return null;
            if (model.UniqueKeys[0].UniqueKeyProperties == null) return null;
            if (model.UniqueKeys[0].UniqueKeyProperties.Count < 1) return null;
            return GetScalarPropByOriginaPropName(model.UniqueKeys[0].UniqueKeyProperties[0].OriginalPropertyName, model);
        }
        bool IsUsebByForeignKey(ModelViewSerializable model, ModelViewPropertyOfVwSerializable sclProp)
        {
            if ((model == null) || (sclProp == null)) return false;
            if (model.ForeignKeys == null) return false;
            if (!string.IsNullOrEmpty(sclProp.ForeignKeyName)) return true;
            return model.ForeignKeys.Any(f => f.ForeignKeyProps.Any(p => p.OriginalPropertyName == sclProp.OriginalPropertyName));
        }

    }
}
