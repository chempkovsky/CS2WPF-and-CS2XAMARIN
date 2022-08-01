using CS2WPF.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Model
{
    public class TemplateCode
    {
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
        string GetInterfaceEDlgName(CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if (model == null)
            {
                return "IEdlg";
            }
            return "I" + model.ViewName + "Edlg";
        }
        string GetInterfaceName(CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if (model == null)
            {
                return "I";
            }
            return "I" + model.ViewName;
        }
        string GetInterfaceNameEx(CS2WPF.Model.Serializable.DbContextSerializable context, string viewName)
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
        string GetInterfacePageName(CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if (model == null)
            {
                return "IPage";
            }
            return "I" + model.PageViewName;
        }
        string GetInterfacePageNameEx(CS2WPF.Model.Serializable.DbContextSerializable context, string viewName)
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
            return GetInterfacePageName(model);
        }
        string GetInterfaceFilterName(CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Filter";
        }
        string GetInterfaceFilterNameEx(CS2WPF.Model.Serializable.DbContextSerializable context, string viewName)
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
            return GetInterfaceFilterName(model);
        }
        string GetModelClassName(CS2WPF.Model.Serializable.DbContextSerializable context, string fileType)
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
            string fn = refItem.FileName.Replace(".interface", "");
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
            return "I" + sb.ToString();
        }

        string GetPropertyTypeScriptTypeName(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop)
        {
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.boolean":
                    result = "boolean";
                    break;
                case "system.guid":
                case "system.string":
                    result = "string";
                    break;
                default:
                    result = "number";
                    break;
            }
            if (prop.IsNullable || (!prop.IsRequiredInView))
            {
                return result + " | null";
            }
            return result;
        }
        string GetJavaScriptToStringMethod(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop)
        {
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.datetime":
                    result = ".toString()"; // .toDateString()
                    break;
                case "system.guid":
                case "system.string":
                    result = "";
                    break;
                default:
                    result = ".toString()";
                    break;
            }
            return result;
        }
        string GetCCharpDatatype(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            return prop.UnderlyingTypeName.ToLower().Replace("system.", "");
        }
        string GetCCharpDatatypeEx(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetCCharpDatatype(sclrProp, model);
        }
        string GetCCharpDatatypeEx2(CS2WPF.Model.Serializable.ModelViewUIListPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetCCharpDatatype(sclrProp, model);
        }
        string GetPropertyTypeName(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop)
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
        string GetFolderName(CS2WPF.Model.Serializable.ModelViewSerializable model, string refFolder, string currFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }
        String GetWebApiServicePrefix(CS2WPF.Model.Serializable.ModelViewSerializable model)
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
        bool IsPrimaryKeyProperty(CS2WPF.Model.Serializable.ModelViewPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return false;
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null))
            {
                return false;
            }
            if (string.IsNullOrEmpty(prop.ForeignKeyNameChain))
            {
                return model.PrimaryKeyProperties.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName);
            }
            if (model.ForeignKeys == null)
            {
                return false;
            }
            if (model.ForeignKeys.Count < 1)
            {
                return false;
            }
            foreach (ModelViewKeyPropertySerializable pk in model.PrimaryKeyProperties)
            {
                foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
                {
                    if ((fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null))
                    {
                        continue;
                    }
                    int cnt = fk.ForeignKeyProps.Count;
                    if (cnt < fk.PrincipalKeyProps.Count)
                    {
                        cnt = fk.PrincipalKeyProps.Count;
                    }
                    for (int i = 0; i < cnt; i++)
                    {
                        if (fk.ForeignKeyProps[i].OriginalPropertyName == pk.OriginalPropertyName)
                        {
                            if ((prop.OriginalPropertyName == fk.PrincipalKeyProps[i].OriginalPropertyName) && (prop.ForeignKeyNameChain == fk.NavigationName))
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            return false;
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
        string GetTypeScriptPropertyName(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return "Noname";
            }
            if (model.GenerateJSonAttribute)
            {
                return prop.JsonPropertyName;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName);
            }
        }
        string GetFilterPropertyOperatorName(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string operatorSufix)
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
        string GetTypeScriptPropertyNameEx(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetTypeScriptPropertyName(sclrProp, model);
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
        List<string> GetValidators(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, Dictionary<string, string> regExps)
        {
            List<string> result = new List<string>();
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return result;
            }
            if (sclrProp.IsRequiredInView)
            {
                result.Add("Validators.required");
            }
            bool hasCurrencyAttr = false;
            if (sclrProp.Attributes != null)
            {
                hasCurrencyAttr = sclrProp.Attributes.Any(a => a.AttrName == "DataType" && a.VaueProperties.Any(p => p.PropValue == "DataType.Currency"));
            }
            string propValue = null;
            switch (sclrProp.UnderlyingTypeName.ToLower())
            {
                case "system.int16":
                case "system.int32":
                case "system.int64":
                case "system.uint16":
                case "system.uint32":
                case "system.uint64":
                    if (hasCurrencyAttr)
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpCurrency"] + ")");
                    }
                    else
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpInteger"] + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "IntegerValidator", 0);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.min(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "IntegerValidator", 1);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.max(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 0);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.min(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 1);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.max(" + propValue.Replace("\"", "") + ")");
                    }
                    break;
                case "system.guid":
                    result.Add("Validators.pattern(" + regExps["RegExpGuid"] + ")");
                    break;
                case "system.double":
                case "system.decimal":
                case "system.single":
                    if (hasCurrencyAttr)
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpCurrency"] + ")");
                    }
                    else
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpFloat"] + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 0);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.min(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 1);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.max(" + propValue.Replace("\"", "") + ")");
                    }
                    break;
                case "system.string":
                    propValue = GetUnNamedAtributeValue(sclrProp, "StringLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.maxLength(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetUnNamedAtributeValue(sclrProp, "MaxLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.maxLength(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetUnNamedAtributeValue(sclrProp, "MinLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.minLength(" + propValue.Replace("\"", "") + ")");
                    }
                    break;
            }
            return result;
        }
        bool HasCombo(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                    (prop.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                    (prop.InputTypeWhenDelete == InputTypeEnum.Combo);
        }
        bool HasButton(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog);
        }
        bool HasTypeahead(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead);
        }
        bool HasInitMethod(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return HasCombo(prop, model) || HasButton(prop, model) || HasTypeahead(prop, model);
        }
        bool HasInitMethodForInputMode(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return
                        (prop.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead);
                case 2:
                    return
                        (prop.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead);
                case 3:
                    return
                        (prop.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead);
            }
            return false;
        }
        bool HasModelInitMethodForInputMode(CS2WPF.Model.Serializable.ModelViewSerializable model, int inputType)
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
        string GetExpressionForControlList(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + "Vals";
        }

        string GetExpressionForOnFilterTypeaheadControlListMethod(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string typeaheadSufix)
        {
            return "OnFilter" + GetExpressionForControlList(prop, model, typeaheadSufix);
        }
        string GetExpressionForOnUpdateComboControlListMethod(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string comboSufix)
        {
            return "OnUpdate" + GetExpressionForControlList(prop, model, comboSufix);
        }
        string GetExpressionForOnValChangedMethod(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return "OnValChanged" + GetTypeScriptPropertyNameEx(prop, model);
        }
        string GetTypeScriptPropertyNameWithSufixBase(CS2WPF.Model.Serializable.ModelViewPropertyOfVwSerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyName(prop, model) + sufix;
        }
        string GetTypeScriptPropertyNameWithSufix(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameEx(prop, model) + sufix;
        }
        string GetExpressionForOnInitMethod(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            return "OnInit" + GetTypeScriptPropertyNameEx(prop, model);
        }


        List<string> CollectComboListInterfaces(CS2WPF.Model.Serializable.DbContextSerializable context,
                                                CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                                                CS2WPF.Model.Serializable.ModelViewSerializable model)
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
        List<string> CollectButtonItemInterfaces(CS2WPF.Model.Serializable.DbContextSerializable context,
                                                CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                                                CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;

            if (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog)
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
                        result.Add(GetInterfaceName(mv));
                    }
                }
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)
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
                        intrfsNm = GetInterfaceName(mv);
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog)
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
                        intrfsNm = GetInterfaceName(mv);
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            return result;
        }
        List<string> CollectTypeaheadListInterfaces(CS2WPF.Model.Serializable.DbContextSerializable context,
                                                CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                                                CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;

            if (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead)
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
                        result.Add("Observable<Array<" + GetInterfaceName(mv) + ">>");
                    }
                }
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead)
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
                        intrfsNm = "Observable<Array<" + GetInterfaceName(mv) + ">>";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead)
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
                        intrfsNm = "Observable<Array<" + GetInterfaceName(mv) + ">>";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            return result;
        }
        string GetFormControlHiddenCondition(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string eformModePropName)
        {
            if ((prop.InputTypeWhenAdd == InputTypeEnum.Hidden) &&
                (prop.InputTypeWhenUpdate == InputTypeEnum.Hidden) &&
                (prop.InputTypeWhenDelete == InputTypeEnum.Hidden))
            {
                return "";
            }
            string result = "*ngIf = \"";
            bool setOr = false;
            if (prop.InputTypeWhenAdd == InputTypeEnum.Hidden)
            {
                result = result + "(" + eformModePropName + "==1)";
                setOr = true;
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.Hidden)
            {
                if (setOr)
                {
                    result = result + "||";
                }
                result = result + "(" + eformModePropName + "==2)";
                setOr = true;
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.Hidden)
            {
                if (setOr)
                {
                    result = result + "||";
                }
                result = result + "(" + eformModePropName + "==3)";
            }
            return result + "\"";
        }
        int GetGreaterThanPercent(int currCnt, int maxCnt, int[] wdths)
        {
            if (currCnt < maxCnt)
            {
                return wdths[0];
            }
            return wdths[1];
        }
        string GetDisplayAttributeValueString(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, string propName)
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
        bool IsDateInput(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return "System.DateTime".Equals(sclrProp.UnderlyingTypeName) || "DateTime".Equals(sclrProp.UnderlyingTypeName);
        }
        string GetCommonEnumClassName(CS2WPF.Model.Serializable.DbContextSerializable context, string fileType)
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
            string fn = refItem.FileName.Replace(".enum", "");
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
            return sb.ToString();
        }
        string GetCommonServiceClassName(CS2WPF.Model.Serializable.DbContextSerializable context, string fileType)
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
            string fn = refItem.FileName.Replace(".service", "Service");
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
            return sb.ToString();
        }

        string GetCommonFolderName(CS2WPF.Model.Serializable.ModelViewSerializable model, CS2WPF.Model.Serializable.DbContextSerializable context, string refFolder, string currFolder)
        {
            string result = "./";
            if ((model == null) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }

        string GetCrossComponentFolderName(CS2WPF.Model.Serializable.ModelViewSerializable model, string currFolder, CS2WPF.Model.Serializable.DbContextSerializable context, string refViewName, string refFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(currFolder) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(refViewName))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.ModelViews == null))
            {
                return result;
            }
            ModelViewSerializable refModel = context.ModelViews.Where(v => v.ViewName == refViewName).FirstOrDefault();
            if (refModel == null)
            {
                return result;
            }
            if (refModel.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                refModel.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }

        string GetComponentSelectorCommonPart(CS2WPF.Model.Serializable.ModelViewSerializable model, string fileType)
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
            return refItem.FileName.Replace(".component", "");
        }

        string GetComponentClassName(CS2WPF.Model.Serializable.ModelViewSerializable model, string fileType)
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
            string fn = refItem.FileName.Replace(".component", "Component");
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
            return sb.ToString();
        }
        string GetComponentClassNameEx(CS2WPF.Model.Serializable.DbContextSerializable context, string viewName, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType) || string.IsNullOrEmpty(viewName))
            {
                return result;
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetComponentClassName(model, fileType);
        }

        string GetServiceClassName(CS2WPF.Model.Serializable.ModelViewSerializable model, string fileType)
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
            string fn = refItem.FileName.Replace(".service", "Service");
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
            return sb.ToString();
        }

        string GetServiceClassNameEx(CS2WPF.Model.Serializable.DbContextSerializable context, string ViewName, string fileType)
        {
            if ((context == null) || string.IsNullOrEmpty(ViewName) || string.IsNullOrEmpty(fileType))
            {
                return "";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (model == null)
            {
                return "";
            }
            return GetServiceClassName(model, fileType);
        }

        string GetViewByForeignNameChain(CS2WPF.Model.Serializable.DbContextSerializable context, string ViewName, string foreignKeyNameChain)
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

        string GetPrimKeyFilterForFindIndexMethod(CS2WPF.Model.Serializable.DbContextSerializable context, string ViewName, string srcPrefix, string destPrefix)
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
                    string proName = GetTypeScriptPropertyName(modelViewPropertyOfVwSerializable, model);
                    if (result != "")
                    {
                        result += " && ";
                    }
                    result += "(" + srcPrefix + "." + proName + " === " + destPrefix + "." + proName + ")";
                }
            }
            if (result == "")
            {
                return "false";
            }
            return result;
        }

        string GetControlListPropertyName(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, CS2WPF.Model.Serializable.DbContextSerializable context, int inputType)
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
            return GetTypeScriptPropertyName(propForSel, mv);
        }

        CS2WPF.Model.Serializable.ModelViewSerializable
            GetViewForControlList(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, CS2WPF.Model.Serializable.DbContextSerializable context, int inputType)
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

        string GetViewNameForControlList(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop, CS2WPF.Model.Serializable.ModelViewSerializable model, CS2WPF.Model.Serializable.DbContextSerializable context, int inputType)
        {
            CS2WPF.Model.Serializable.ModelViewSerializable mv =
                GetViewForControlList(prop, model, context, inputType);
            if (mv == null)
            {
                return "NoName";
            }
            return mv.ViewName;
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
                ModelViewPropertyOfVwSerializable prop =
                    model.ScalarProperties.Where(p => p.ViewPropertyName == modelViewKeyPropertySerializable.ViewPropertyName).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
                else
                {
                    if (model.ForeignKeys != null)
                    {
                        foreach (ModelViewForeignKeySerializable modelViewForeignKeySerializable in model.ForeignKeys)
                        {
                            if ((modelViewForeignKeySerializable.PrincipalKeyProps != null) && (modelViewForeignKeySerializable.ForeignKeyProps != null))
                            {
                                for (int i = 0; i < modelViewForeignKeySerializable.ForeignKeyProps.Count; i++)
                                {
                                    if (modelViewForeignKeySerializable.ForeignKeyProps[i].OriginalPropertyName == modelViewKeyPropertySerializable.OriginalPropertyName)
                                    {
                                        if (i < modelViewForeignKeySerializable.PrincipalKeyProps.Count)
                                        {
                                            prop =
                                            model.ScalarProperties.Where(p =>
                                                (p.OriginalPropertyName == modelViewForeignKeySerializable.PrincipalKeyProps[i].OriginalPropertyName)
                                                &&
                                                (p.ForeignKeyName == modelViewForeignKeySerializable.NavigationName)
                                            ).FirstOrDefault();
                                        }
                                    }
                                    if (prop != null) break;
                                }
                            }
                            if (prop != null) break;
                        }
                        if (prop != null)
                        {
                            result.Add(prop);
                        }
                    }
                }
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

        List<ModelViewPropertyOfVwSerializable> GetPrimaryKeyProps(CS2WPF.Model.Serializable.DbContextSerializable context, string viewName)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return result;
            }
            return GetModelPrimaryKeyProps(context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault());
        }

        List<ModelViewPropertyOfVwSerializable> GetForeignKeyProps(CS2WPF.Model.Serializable.DbContextSerializable context, CS2WPF.Model.Serializable.ModelViewSerializable model,
                                                ModelViewUIFormPropertySerializable masterProp, ModelViewUIFormPropertySerializable detailProp, int inputType)
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
            CS2WPF.Model.Serializable.ModelViewSerializable detailModel =
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

        List<ModelViewPropertyOfVwSerializable> GetForeignKeyPropsBase(CS2WPF.Model.Serializable.DbContextSerializable context, CS2WPF.Model.Serializable.ModelViewSerializable model,
                                                ModelViewUIFormPropertySerializable masterProp)
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

        List<string> GetFKViewsList(CS2WPF.Model.Serializable.ModelViewSerializable model,
                                         CS2WPF.Model.Serializable.DbContextSerializable context,
                                         List<string> fkViewsDict)
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

        List<string> GetSearchDialogViewsList(CS2WPF.Model.Serializable.ModelViewSerializable model,
                                             CS2WPF.Model.Serializable.DbContextSerializable context,
                                             List<string> sdViewsDict)
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


        List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable>
            GetDirectMasters(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                             CS2WPF.Model.Serializable.ModelViewSerializable model,
                             CS2WPF.Model.Serializable.DbContextSerializable context, int inputType)
        {
            List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable> result = new List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable>();
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

        List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable>
            GetDependentScalarProps(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                                    CS2WPF.Model.Serializable.ModelViewSerializable model,
                                    CS2WPF.Model.Serializable.DbContextSerializable context, int inputType)
        {
            List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable> result = new List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable>();
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
            List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable> masters = GetDirectMasters(prop, model, context, inputType);
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

        List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable>
            GetDirectDetails(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                             CS2WPF.Model.Serializable.ModelViewSerializable model,
                             CS2WPF.Model.Serializable.DbContextSerializable context, int inputType)
        {
            List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable> result = new List<CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable>();
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

        bool MustHaveDirectDetails(CS2WPF.Model.Serializable.ModelViewUIFormPropertySerializable prop,
                             CS2WPF.Model.Serializable.ModelViewSerializable model,
                             CS2WPF.Model.Serializable.DbContextSerializable context)
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

        string GetPrimKeyVarName(ModelViewPropertyOfVwSerializable pkpModelViewUIFormPropertySerializable)
        {
            return "pkp" + pkpModelViewUIFormPropertySerializable.ViewPropertyName;
        }

        List<ModelViewPropertyOfVwSerializable> GetAllForeignKeyProps(CS2WPF.Model.Serializable.ModelViewSerializable model)
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
        ModelViewPropertyOfVwSerializable GetOnValChangeViewPropName(CS2WPF.Model.Serializable.DbContextSerializable context, CS2WPF.Model.Serializable.ModelViewSerializable model,
                                    ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable, ModelViewUIFormPropertySerializable dependentScalarProp, int inputType)
        {
            if (dependentScalarProp == null)
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
                if (foreignKeyNameChain != "") foreignKeyNameChain += ".";
                dependentForeignKeyNameChain = dependentForeignKeyNameChain.Replace(foreignKeyNameChain, "");
            }
            if (string.IsNullOrEmpty(dependentForeignKeyNameChain))
            {
                return
                    view.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain)).FirstOrDefault();
            }
            return
                view.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && (p.ForeignKeyNameChain == dependentForeignKeyNameChain)).FirstOrDefault();
        }

        bool HasOnValChangedMethod(CS2WPF.Model.Serializable.DbContextSerializable context, CS2WPF.Model.Serializable.ModelViewSerializable model,
                                    ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable)
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
                            string propName = GetTypeScriptPropertyName(prop, model);
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

            int ind = fk.PrincipalKeyProps.FindIndex(p => p.ViewPropertyName == primKey.ViewPropertyName);
            if (ind < 0)
            {
                return result;
            }
            ModelViewKeyPropertySerializable ForeignKeyProp = ForeignKey.ForeignKeyProps[ind];
            ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializableEx =
                detail.ScalarProperties.FirstOrDefault(s => (s.OriginalPropertyName == ForeignKeyProp.OriginalPropertyName) && (string.IsNullOrEmpty(s.ForeignKeyNameChain)));
            if (modelViewPropertyOfVwSerializableEx != modelViewPropertyOfVwSerializable)
            {
                result.Add(modelViewPropertyOfVwSerializableEx);
            }
            return result;
        }
        ModelViewSerializable GetFirstModelViewByType(DbContextSerializable context, string fileType)
        {
            if ((context == null) || (string.IsNullOrEmpty(fileType)))
            {
                return null;
            }
            if (context.ModelViews == null)
            {
                return null;
            }
            return context.ModelViews.Where(v => v.CommonStaffs.Any(f => f.FileType == fileType)).FirstOrDefault();
        }

    }
}
