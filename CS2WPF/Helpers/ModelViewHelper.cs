using CS2WPF.Model;
using CS2WPF.Model.Serializable;
using CS2WPF.Model.Serializable.UI;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers
{
    public static class ModelViewHelper
    {
        public static string InputParamNotDefined = "Input param is not defined";
        public static string ScalarPropertiesNotDefined = "The collection of Properties is not defined";
        public static string PropertyNameNotDefined = "The Name of the Property is not defined: Inherited Property Name = ";
        public static string JSonPropertyNameNotDefined = "The Json Name of the Property is not defined: Inherited Property Name = ";
        public static string NoPropertiesSelected = "At least one property must be selected to generate the View.";
        public static string DuplicationOfPropertyNames = "Duplicate property names detected";
        public static string DuplicationOfJsonPropertyNames = "Duplicate json property names detected";

        public static string UnderForeignKey = "Under Foreign Key :";
        public static string PrincipalKeyNotDefined = "Principal Key is not defined";
        public static string ForeignKeyNotDefined = "Foreign Key is not defined";
        public static string PrincipalKeyCountNotEqualForeignKeyCount = "The number of Principal Key Properties is not equal to the number of Foreign Key Properties";
        public static string PrincipalKeyPropertyIsIncluded = "Principal Key Property is included in the list of View: ";
        public static string ForeignKeyPropertyIsNotIncluded = "Foreign Key Property is not included in the list of View: ";
        public static string RecommendedSwapSelection = "Recommended To swap selection !!!";
        public static string ForeignKeyPropertyIsIncluded = "Foreign Key Property is included in the list of View: ";
        public static string RecommendedRemovePrincipalSelection = "It is recommended to deselect the Principal property !!!";
        public static ModelView ClearModelView(this ModelView selectedModel)
        {
            if (selectedModel == null) return selectedModel;
            selectedModel.ViewName = "";
            selectedModel.PageViewName = "";
            selectedModel.RootEntityClassName = "";
            selectedModel.RootEntityFullClassName = "";
            selectedModel.RootEntityUniqueProjectName = "";
            if (selectedModel.ScalarProperties == null) {
                selectedModel.ScalarProperties = new ObservableCollection<ModelViewProperty>();
            }
            selectedModel.ScalarProperties.Clear();
            if (selectedModel.ForeignKeys == null)
            {
                selectedModel.ForeignKeys = new ObservableCollection<ModelViewForeignKey>();
            }
            selectedModel.ForeignKeys.Clear();
            if (selectedModel.PrimaryKeyProperties == null)
            {
                selectedModel.PrimaryKeyProperties = new ObservableCollection<ModelViewKeyProperty>();
            }
            selectedModel.PrimaryKeyProperties.Clear();
            if (selectedModel.AllProperties == null)
            {
                selectedModel.AllProperties = new ObservableCollection<ModelViewEntityProperty>();
            }
            selectedModel.AllProperties.Clear();
            return selectedModel;
        }
        public static ModelViewSerializable ClearModelViewSerializable(this ModelViewSerializable selectedModel)
        {
            if (selectedModel == null) return selectedModel;
            selectedModel.ViewName = "";
            selectedModel.RootEntityClassName = "";
            selectedModel.RootEntityFullClassName = "";
            selectedModel.RootEntityUniqueProjectName = "";
            if (selectedModel.ScalarProperties == null)
            {
                selectedModel.ScalarProperties = new List<ModelViewPropertyOfVwSerializable>();
            }
            selectedModel.ScalarProperties.Clear();
            if (selectedModel.ForeignKeys == null)
            {
                selectedModel.ForeignKeys = new List<ModelViewForeignKeySerializable>();
            }
            selectedModel.ForeignKeys.Clear();
            if (selectedModel.PrimaryKeyProperties == null)
            {
                selectedModel.PrimaryKeyProperties = new List<ModelViewKeyPropertySerializable>();
            }
            selectedModel.PrimaryKeyProperties.Clear();
            if (selectedModel.AllProperties == null)
            {
                selectedModel.AllProperties = new List<ModelViewEntityPropertySerializable>();
            }
            selectedModel.AllProperties.Clear();
            if (selectedModel.UIFormProperties == null)
            {
                selectedModel.UIFormProperties = new List<ModelViewUIFormPropertySerializable>();
            }
            selectedModel.UIFormProperties.Clear();
            if (selectedModel.UIListProperties == null)
            {
                selectedModel.UIListProperties = new List<ModelViewUIListPropertySerializable>();
            }
            selectedModel.UIListProperties.Clear();

            return selectedModel;
        }
        public static ModelViewFAPIAttributePropertySerializable ModelViewFAPIAttributePropertyAssingTo(this ModelViewFAPIAttributeProperty srcAttributeProperty, ModelViewFAPIAttributePropertySerializable destAttributeProperty)
        {
            if ((srcAttributeProperty == null) || (destAttributeProperty == null)) return null;
            destAttributeProperty.PropValue = srcAttributeProperty.PropValue;
            return destAttributeProperty;
        }
        public static ModelViewFAPIAttributeSerializable ModelViewFAPIAttributeAssingTo(this ModelViewFAPIAttribute srcAttr, ModelViewFAPIAttributeSerializable destAttr)
        {
            if ((srcAttr == null) || (destAttr == null)) return null;
            destAttr.AttrName = srcAttr.AttrName;
            if (srcAttr.VaueProperties != null)
            {
                if (srcAttr.VaueProperties.Count > 0)
                {
                    destAttr.VaueProperties = new List<ModelViewFAPIAttributePropertySerializable>();
                    foreach (ModelViewFAPIAttributeProperty srcAttributeProperty in srcAttr.VaueProperties)
                    {
                        destAttr.VaueProperties.Add(srcAttributeProperty.ModelViewFAPIAttributePropertyAssingTo(new ModelViewFAPIAttributePropertySerializable()));
                    }
                }
            }
            return destAttr;
        }
        public static ModelViewFAPIAttributeProperty ModelViewFAPIAttributePropertySerializableAssingTo(this ModelViewFAPIAttributePropertySerializable srcAttributeProperty, ModelViewFAPIAttributeProperty destAttributeProperty)
        {
            if ((srcAttributeProperty == null) || (destAttributeProperty == null)) return null;
            destAttributeProperty.PropValue = srcAttributeProperty.PropValue;
            return destAttributeProperty;
        }
        public static ModelViewFAPIAttribute ModelViewFAPIAttributeSerializableAssingTo(this ModelViewFAPIAttributeSerializable srcAttr, ModelViewFAPIAttribute destAttr)
        {
            if ((srcAttr == null) || (destAttr == null)) return null;
            destAttr.AttrName = srcAttr.AttrName;
            if (srcAttr.VaueProperties != null)
            {
                if (srcAttr.VaueProperties.Count > 0)
                {
                    destAttr.VaueProperties = new ObservableCollection<ModelViewFAPIAttributeProperty>();
                    foreach (ModelViewFAPIAttributePropertySerializable srcAttributeProperty in srcAttr.VaueProperties)
                    {
                        destAttr.VaueProperties.Add(srcAttributeProperty.ModelViewFAPIAttributePropertySerializableAssingTo(new ModelViewFAPIAttributeProperty()));
                    }
                }
            }
            return destAttr;
        }
        public static ModelViewAttributePropertySerializable ModelViewAttributePropertyAssingTo(this ModelViewAttributeProperty srcAttributeProperty, ModelViewAttributePropertySerializable destAttributeProperty)
        {
            if ((srcAttributeProperty == null) || (destAttributeProperty == null)) return null;
            destAttributeProperty.PropName = srcAttributeProperty.PropName;
            destAttributeProperty.PropValue = srcAttributeProperty.PropValue;
            return destAttributeProperty;
        }
        public static ModelViewAttributeSerializable ModelViewAttributeAssingTo(this ModelViewAttribute srcAttr, ModelViewAttributeSerializable destAttr)
        {
            if ((srcAttr == null) || (destAttr == null)) return null;
            destAttr.AttrName = srcAttr.AttrName;
            destAttr.AttrFullName = srcAttr.AttrFullName;
            if (srcAttr.VaueProperties != null)
            {
                if(srcAttr.VaueProperties.Count > 0)
                {
                    destAttr.VaueProperties = new List<ModelViewAttributePropertySerializable>();
                    foreach(ModelViewAttributeProperty srcAttributeProperty in srcAttr.VaueProperties)
                    {
                        destAttr.VaueProperties.Add(srcAttributeProperty.ModelViewAttributePropertyAssingTo(new ModelViewAttributePropertySerializable()));
                    }
                }
            }
            return destAttr;
        }
        public static ModelViewAttributeProperty ModelViewAttributePropertySerializableAssingTo(this ModelViewAttributePropertySerializable srcAttributeProperty, ModelViewAttributeProperty destAttributeProperty)
        {
            if ((srcAttributeProperty == null) || (destAttributeProperty == null)) return null;
            destAttributeProperty.PropName = srcAttributeProperty.PropName;
            destAttributeProperty.PropValue = srcAttributeProperty.PropValue;
            return destAttributeProperty;
        }
        public static ModelViewAttribute ModelViewAttributeSerializableAssingTo(this ModelViewAttributeSerializable srcAttr, ModelViewAttribute destAttr)
        {
            if ((srcAttr == null) || (destAttr == null)) return null;
            destAttr.AttrName = srcAttr.AttrName;
            destAttr.AttrFullName = srcAttr.AttrFullName;
            if (srcAttr.VaueProperties != null)
            {
                if (srcAttr.VaueProperties.Count > 0)
                {
                    destAttr.VaueProperties = new ObservableCollection<ModelViewAttributeProperty>();
                    //List < ModelViewAttributePropertySerializable >
                    foreach (ModelViewAttributePropertySerializable srcAttributeProperty in srcAttr.VaueProperties)
                    {
                        destAttr.VaueProperties.Add(srcAttributeProperty.ModelViewAttributePropertySerializableAssingTo(new ModelViewAttributeProperty()));
                    }
                }
            }
            return destAttr;
        }
        public static ModelViewPropertyOfVwSerializable ModelViewPropertyAssingTo(this ModelViewProperty srcProp, ModelViewPropertyOfVwSerializable destProp)
        {
            if ((srcProp == null) || (destProp == null)) return null;
            destProp.OriginalPropertyName = srcProp.OriginalPropertyName;
            destProp.TypeFullName = srcProp.TypeFullName;
            destProp.IsNullable = srcProp.IsNullable;
            destProp.IsRequired = srcProp.IsRequired;
            destProp.UnderlyingTypeName = srcProp.UnderlyingTypeName;
            destProp.IsSelected = srcProp.IsSelected;
            destProp.ForeignKeyName = srcProp.ForeignKeyName;
            destProp.ForeignKeyNameChain = srcProp.ForeignKeyNameChain;
            destProp.ViewPropertyName = srcProp.EditableViewPropertyName;
            destProp.IsRequiredInView = srcProp.IsRequiredInView;
            destProp.JsonPropertyName = srcProp.EditableJsonPropertyName;
            if (srcProp.Attributes != null)
            {
                if(srcProp.Attributes.Count > 0)
                {
                    if(destProp.Attributes == null)
                    {
                        destProp.Attributes = new List<ModelViewAttributeSerializable>();
                        foreach (ModelViewAttribute srcAttr in srcProp.Attributes)
                        {
                            destProp.Attributes.Add(srcAttr.ModelViewAttributeAssingTo(new ModelViewAttributeSerializable()));
                        }

                    }
                }
            }
            if (srcProp.FAPIAttributes != null)
            {
                if (srcProp.FAPIAttributes.Count > 0)
                {
                    if (destProp.FAPIAttributes == null)
                    {
                        destProp.FAPIAttributes = new List<ModelViewFAPIAttributeSerializable>();
                        foreach (ModelViewFAPIAttribute srcAttr in srcProp.FAPIAttributes)
                        {
                            destProp.FAPIAttributes.Add(srcAttr.ModelViewFAPIAttributeAssingTo(new ModelViewFAPIAttributeSerializable()));
                        }

                    }
                }
            }
            return destProp;
        }
        public static ModelViewPropertyOfFkSerializable ModelViewPropertyAssingTo(this ModelViewProperty srcProp, ModelViewPropertyOfFkSerializable destProp)
        {
            if ((srcProp == null) || (destProp == null)) return null;
            destProp.OriginalPropertyName = srcProp.OriginalPropertyName;
            destProp.TypeFullName = srcProp.TypeFullName;
            destProp.IsNullable = srcProp.IsNullable;
            destProp.IsRequired = srcProp.IsRequired;
            destProp.UnderlyingTypeName = srcProp.UnderlyingTypeName;
            destProp.IsSelected = srcProp.IsSelected;
            destProp.ForeignKeyName = srcProp.ForeignKeyName;
            destProp.ForeignKeyNameChain = srcProp.ForeignKeyNameChain;
            destProp.ViewPropertyName = srcProp.EditableViewPropertyName;
            destProp.IsRequiredInView = srcProp.IsRequiredInView;
            destProp.JsonPropertyName = srcProp.EditableJsonPropertyName;
            return destProp;
        }
        public static ModelViewKeyPropertySerializable ModelViewKeyPropertyAssingTo(this ModelViewKeyProperty srcProp, ModelViewKeyPropertySerializable destProp)
        {
            if ((srcProp == null) || (destProp == null)) return null;
            destProp.OriginalPropertyName = srcProp.OriginalPropertyName;
            destProp.TypeFullName = srcProp.TypeFullName;
            destProp.IsNullable = srcProp.IsNullable;
            destProp.IsRequired = srcProp.IsRequired;
            destProp.UnderlyingTypeName = srcProp.UnderlyingTypeName;
            destProp.ViewPropertyName = srcProp.ViewPropertyName;
            destProp.JsonPropertyName = srcProp.JsonPropertyName;
            return destProp;
        }
        public static ModelViewEntityPropertySerializable ModelViewEntityPropertyAssingTo(this ModelViewEntityProperty srcProp, ModelViewEntityPropertySerializable destProp)
        {
            if ((srcProp == null) || (destProp == null)) return null;
            destProp.OriginalPropertyName = srcProp.OriginalPropertyName;
            destProp.TypeFullName = srcProp.TypeFullName;
            destProp.IsNullable = srcProp.IsNullable;
            destProp.IsRequired = srcProp.IsRequired;
            destProp.UnderlyingTypeName = srcProp.UnderlyingTypeName;
            destProp.ViewPropertyName = srcProp.ViewPropertyName;
            destProp.JsonPropertyName = srcProp.JsonPropertyName;
            if (srcProp.Attributes != null)
            {
                if (srcProp.Attributes.Count > 0)
                {
                    if (destProp.Attributes == null)
                    {
                        destProp.Attributes = new List<ModelViewAttributeSerializable>();
                        foreach (ModelViewAttribute srcAttr in srcProp.Attributes)
                        {
                            destProp.Attributes.Add(srcAttr.ModelViewAttributeAssingTo(new ModelViewAttributeSerializable()));
                        }

                    }
                }
            }
            if (srcProp.FAPIAttributes != null)
            {
                if (srcProp.FAPIAttributes.Count > 0)
                {
                    if (destProp.FAPIAttributes == null)
                    {
                        destProp.FAPIAttributes = new List<ModelViewFAPIAttributeSerializable>();
                        foreach (ModelViewFAPIAttribute srcAttr in srcProp.FAPIAttributes)
                        {
                            destProp.FAPIAttributes.Add(srcAttr.ModelViewFAPIAttributeAssingTo(new ModelViewFAPIAttributeSerializable()));
                        }

                    }
                }
            }
            return destProp;
        }


        public static ModelViewForeignKeySerializable ModelViewForeignKeyAssingTo(this ModelViewForeignKey srcForeignKey, ModelViewForeignKeySerializable destForeignKey)
        {
            if ((srcForeignKey == null) || (destForeignKey == null)) return null;
            destForeignKey.NavigationName = srcForeignKey.NavigationName;
            destForeignKey.InverseNavigationName = srcForeignKey.InverseNavigationName;
            destForeignKey.EntityName = srcForeignKey.EntityName;
            destForeignKey.EntityFullName = srcForeignKey.EntityFullName;
            destForeignKey.EntityUniqueProjectName = srcForeignKey.EntityUniqueProjectName;
            destForeignKey.NavigationEntityName = srcForeignKey.NavigationEntityName;
            destForeignKey.NavigationEntityFullName = srcForeignKey.NavigationEntityFullName;
            destForeignKey.NavigationEntityUniqueProjectName = srcForeignKey.NavigationEntityUniqueProjectName;
            destForeignKey.NavigationType = srcForeignKey.NavigationType;
            destForeignKey.ForeignKeySource = srcForeignKey.ForeignKeySource;
            destForeignKey.PrincipalKeySource = srcForeignKey.PrincipalKeySource;
            destForeignKey.InverseNavigationSource = srcForeignKey.InverseNavigationSource;
            destForeignKey.IsCascadeDelete = srcForeignKey.IsCascadeDelete;
            destForeignKey.ForeignKeyPrefix = srcForeignKey.ForeignKeyPrefix;

            destForeignKey.ViewName = srcForeignKey.ViewName;


            if (destForeignKey.ScalarProperties == null)
            {
                destForeignKey.ScalarProperties = new List<ModelViewPropertyOfFkSerializable>();
            }
            else
            {
                destForeignKey.ScalarProperties.Clear();
            }
            if (srcForeignKey.ScalarProperties != null)
            {
                foreach (ModelViewProperty prop in srcForeignKey.ScalarProperties)
                {
                    // all propertyies to copy for future use
                    //if (prop.IsSelected)
                    //{
                        destForeignKey.ScalarProperties.Add(prop.ModelViewPropertyAssingTo(new ModelViewPropertyOfFkSerializable()));
                    //}
                }
            }

            if (destForeignKey.PrincipalKeyProps == null)
            {
                destForeignKey.PrincipalKeyProps = new List<ModelViewKeyPropertySerializable>();
            }
            else
            {
                destForeignKey.PrincipalKeyProps.Clear();
            }
            if (srcForeignKey.ScalarProperties != null)
            {
                foreach (ModelViewKeyProperty prop in srcForeignKey.PrincipalKeyProps)
                {
                    destForeignKey.PrincipalKeyProps.Add(prop.ModelViewKeyPropertyAssingTo(new ModelViewKeyPropertySerializable()));
                }
            }

            if (destForeignKey.ForeignKeyProps == null)
            {
                destForeignKey.ForeignKeyProps = new List<ModelViewKeyPropertySerializable>();
            }
            else
            {
                destForeignKey.ForeignKeyProps.Clear();
            }
            if (srcForeignKey.ForeignKeyProps != null)
            {
                foreach (ModelViewKeyProperty prop in srcForeignKey.ForeignKeyProps)
                {
                    destForeignKey.ForeignKeyProps.Add(prop.ModelViewKeyPropertyAssingTo(new ModelViewKeyPropertySerializable()));
                }
            }
            return destForeignKey;
        }
        public static bool ModelViewForeignKeyIsRequired(this ModelViewForeignKey srcForeignKey)
        {
            if (srcForeignKey == null) return false;
            if(srcForeignKey.ForeignKeyProps == null) return false;
            if (srcForeignKey.ForeignKeyProps.Count < 1) return false;
            foreach(ModelViewKeyProperty prop in srcForeignKey.ForeignKeyProps)
            {
                if (!prop.IsRequired) return false;
            }
            return true;
        }
        public static ModelViewSerializable ModelViewAssingTo(this ModelView srcModel, ModelViewSerializable destModel)
        {
            if ((srcModel == null) || (destModel == null)) return null;
            destModel.ClearModelViewSerializable();
            destModel.ViewName = srcModel.ViewName;
            destModel.RootEntityClassName = srcModel.RootEntityClassName;
            destModel.RootEntityFullClassName = srcModel.RootEntityFullClassName;
            destModel.RootEntityUniqueProjectName = srcModel.RootEntityUniqueProjectName;
            destModel.RootEntityDbContextPropertyName = srcModel.RootEntityDbContextPropertyName;
            destModel.ViewProject = srcModel.ViewProject;
            destModel.ViewDefaultProjectNameSpace = srcModel.ViewDefaultProjectNameSpace;
            destModel.ViewFolder = srcModel.ViewFolder;
            destModel.GenerateJSonAttribute = srcModel.GenerateJSonAttribute;
            destModel.PageViewName = srcModel.PageViewName;
            if (srcModel.ScalarProperties != null)
            {
                foreach(ModelViewProperty prop in srcModel.ScalarProperties)
                {
                    if (prop.IsSelected)
                    {
                        destModel.ScalarProperties.Add(prop.ModelViewPropertyAssingTo(new ModelViewPropertyOfVwSerializable()));
                    }
                }
            }
            if (srcModel.AllProperties != null)
            {
                foreach (ModelViewEntityProperty prop in srcModel.AllProperties)
                {
                    destModel.AllProperties.Add(prop.ModelViewEntityPropertyAssingTo(new ModelViewEntityPropertySerializable()));
                }
            }
            if (srcModel.PrimaryKeyProperties != null)
            {
                foreach (ModelViewKeyProperty prop in srcModel.PrimaryKeyProperties)
                {
                    destModel.PrimaryKeyProperties.Add(prop.ModelViewKeyPropertyAssingTo(new ModelViewKeyPropertySerializable()));
                }
            }
            if (srcModel.ForeignKeys != null)
            {
                foreach (ModelViewForeignKey foreignKey in srcModel.ForeignKeys)
                {
                    destModel.ForeignKeys.Add(foreignKey.ModelViewForeignKeyAssingTo(new ModelViewForeignKeySerializable()));
                    if (foreignKey.ScalarProperties == null) continue;
                    if(string.IsNullOrEmpty( foreignKey.ViewName)) continue;
                    bool isRequired = foreignKey.ModelViewForeignKeyIsRequired();
                    foreach (ModelViewProperty prop in foreignKey.ScalarProperties)
                    {
                        if (prop.IsSelected)
                        {
                            ModelViewPropertyOfVwSerializable modelViewPropertySerializable = prop.ModelViewPropertyAssingTo(new ModelViewPropertyOfVwSerializable());
                            modelViewPropertySerializable.IsRequiredInView = modelViewPropertySerializable.IsRequired && isRequired;
                            destModel.ScalarProperties.Add(modelViewPropertySerializable);
                        }
                    }
                }
            }
            if (srcModel.UIFormProperties != null)
            {
                foreach (ModelViewUIFormProperty prop in srcModel.UIFormProperties)
                {
                    destModel.UIFormProperties.Add(prop.ModelViewUIFormPropertyAssignTo(new ModelViewUIFormPropertySerializable()));
                }
            }
            if (srcModel.UIListProperties != null)
            {
                foreach (ModelViewUIListProperty prop in srcModel.UIListProperties)
                {
                    destModel.UIListProperties.Add(prop.ModelViewUIListPropertyAssignTo(new ModelViewUIListPropertySerializable()));
                }
            }


            return destModel;
        }
        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }
        public static string FirstLetterToLower(this string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);
            return str.ToLower();
        }
        public static ModelViewForeignKey OnModelViewForeignKeyPrefixChanged(this ModelViewForeignKey srcForeignKey)
        {
            if (srcForeignKey == null) return null;
            if(srcForeignKey.ScalarProperties != null)
            {
                string prefix = srcForeignKey.ForeignKeyPrefix;
                if (string.IsNullOrEmpty(prefix)) prefix = "";
                foreach (ModelViewProperty prop in srcForeignKey.ScalarProperties)
                {
                    prop.EditableViewPropertyName = prefix + prop.ViewPropertyName;
                    prop.EditableJsonPropertyName = prefix.FirstLetterToLower() + prop.JsonPropertyName.FirstLetterToUpper();
                }
            }
            return srcForeignKey;
        }
        public static ModelViewProperty ModelViewPropertySerializableAssingTo(this ModelViewPropertyOfFkSerializable srcProp, ModelViewProperty destProp)
        {
            if ((srcProp == null) || (destProp == null)) return null;
            destProp.OriginalPropertyName = srcProp.OriginalPropertyName;
            destProp.TypeFullName = srcProp.TypeFullName;
            destProp.IsNullable = srcProp.IsNullable;
            destProp.IsRequired = srcProp.IsRequired;
            destProp.UnderlyingTypeName = srcProp.UnderlyingTypeName;
            destProp.IsSelected = srcProp.IsSelected;
            destProp.ForeignKeyName = srcProp.ForeignKeyName;
            destProp.ForeignKeyNameChain = srcProp.ForeignKeyNameChain;
            destProp.ViewPropertyName = srcProp.ViewPropertyName;
            destProp.JsonPropertyName = srcProp.JsonPropertyName;
            destProp.IsRequiredInView = srcProp.IsRequiredInView;
            return destProp;
        }
        public static ModelViewProperty ModelViewPropertySerializableAssingTo(this ModelViewPropertyOfVwSerializable srcProp, ModelViewProperty destProp)
        {
            if ((srcProp == null) || (destProp == null)) return null;
            destProp.OriginalPropertyName = srcProp.OriginalPropertyName;
            destProp.TypeFullName = srcProp.TypeFullName;
            destProp.IsNullable = srcProp.IsNullable;
            destProp.IsRequired = srcProp.IsRequired;
            destProp.UnderlyingTypeName = srcProp.UnderlyingTypeName;
            destProp.IsSelected = srcProp.IsSelected;
            destProp.ForeignKeyName = srcProp.ForeignKeyName;
            destProp.ForeignKeyNameChain = srcProp.ForeignKeyNameChain;
            destProp.ViewPropertyName = srcProp.ViewPropertyName;
            destProp.JsonPropertyName = srcProp.JsonPropertyName;
            destProp.IsRequiredInView = srcProp.IsRequiredInView;
            if (srcProp.Attributes != null)
            {
                if (srcProp.Attributes.Count > 0)
                {
                    if (destProp.Attributes == null)
                    {
                        destProp.Attributes = new ObservableCollection<ModelViewAttribute>();
                        foreach (ModelViewAttributeSerializable srcAttr in srcProp.Attributes)
                        {
                            destProp.Attributes.Add(srcAttr.ModelViewAttributeSerializableAssingTo(new ModelViewAttribute()));
                        }

                    }
                }
            }
            if (srcProp.FAPIAttributes != null)
            {
                if (srcProp.FAPIAttributes.Count > 0)
                {
                    if (destProp.FAPIAttributes == null)
                    {
                        destProp.FAPIAttributes = new ObservableCollection<ModelViewFAPIAttribute>();
                        foreach (ModelViewFAPIAttributeSerializable srcAttr in srcProp.FAPIAttributes)
                        {
                            destProp.FAPIAttributes.Add(srcAttr.ModelViewFAPIAttributeSerializableAssingTo(new ModelViewFAPIAttribute()));
                        }
                    }
                }
            }
            return destProp;
        }
        public static void ModelViewForeignKeyUpdateForeignKeyNameChain(this ModelViewForeignKey srcForeignKey)
        {
            if (srcForeignKey == null) return;
            if (srcForeignKey.ScalarProperties == null) return;
            foreach(ModelViewProperty prop in srcForeignKey.ScalarProperties)
            {
                prop.ForeignKeyName = srcForeignKey.NavigationName;
                if(string.IsNullOrEmpty( prop.ForeignKeyNameChain))
                {
                    prop.ForeignKeyNameChain = srcForeignKey.NavigationName;
                } else
                {
                    if (!string.IsNullOrEmpty(srcForeignKey.NavigationName))
                    {
                        prop.ForeignKeyNameChain = srcForeignKey.NavigationName + "." + prop.ForeignKeyNameChain;
                    }
                }
            }
        }
        public static string CheckCorrect(this ModelView srcModel)
        {
            if (srcModel == null)
            {
                return InputParamNotDefined;
            }
            if(srcModel.ScalarProperties == null)
            {
                return ScalarPropertiesNotDefined;
            }
            List<string> props = new List<string>();
            List<string> jsonprops = new List<string>();
            foreach (ModelViewProperty prop in srcModel.ScalarProperties)
            {
                    if (string.IsNullOrEmpty(prop.EditableViewPropertyName))
                    {
                        return PropertyNameNotDefined + prop.ViewPropertyName + ":" + prop.OriginalPropertyName;
                    }
                    if (string.IsNullOrEmpty(prop.EditableJsonPropertyName))
                    {
                        return JSonPropertyNameNotDefined + prop.ViewPropertyName + ":" + prop.OriginalPropertyName;
                    }
            }
            foreach (ModelViewProperty prop in srcModel.ScalarProperties)
            {
                if (prop.IsSelected)
                {
                    props.Add(prop.EditableViewPropertyName);
                    jsonprops.Add(prop.EditableJsonPropertyName);
                }
            }

            if (srcModel.ForeignKeys != null)
            {
                foreach (ModelViewForeignKey foreignKey in srcModel.ForeignKeys)
                {
                    if (!string.IsNullOrEmpty(foreignKey.ViewName))
                    {
                        if ((foreignKey.ScalarProperties != null) && (foreignKey.PrincipalKeyProps == null))
                        {
                            return UnderForeignKey + foreignKey.NavigationName + " " + PrincipalKeyNotDefined;
                        }

                        if (foreignKey.ScalarProperties != null)
                        {
                            if (foreignKey.ScalarProperties.Count > 0)
                            {
                                if (foreignKey.PrincipalKeyProps == null)
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " + PrincipalKeyNotDefined;
                                }
                                if (foreignKey.PrincipalKeyProps.Count < 1)
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " + PrincipalKeyNotDefined;
                                }
                                if (foreignKey.ForeignKeyProps == null)
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " + ForeignKeyNotDefined;
                                }
                                if (foreignKey.ForeignKeyProps.Count < 1)
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " + ForeignKeyNotDefined;
                                }
                                if (foreignKey.ForeignKeyProps.Count != foreignKey.PrincipalKeyProps.Count)
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " + PrincipalKeyCountNotEqualForeignKeyCount;
                                }
                            }


                            foreach (ModelViewProperty prop in foreignKey.ScalarProperties)
                            {
                                if (string.IsNullOrEmpty(prop.EditableViewPropertyName))
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " +
                                        PropertyNameNotDefined + prop.ViewPropertyName + ":" + prop.OriginalPropertyName;
                                }
                                if (string.IsNullOrEmpty(prop.EditableJsonPropertyName))
                                {
                                    return UnderForeignKey + foreignKey.NavigationName + " " +
                                        JSonPropertyNameNotDefined + prop.ViewPropertyName + ":" + prop.OriginalPropertyName;
                                }
                            }


                            foreach (ModelViewProperty prop in foreignKey.ScalarProperties)
                            {
                                if (prop.IsSelected)
                                {
                                    props.Add(prop.EditableViewPropertyName);
                                    jsonprops.Add(prop.EditableJsonPropertyName);
                                }
                            }
                        }
                    }
                }
            }
            var duplicateProps = props.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();
            if (duplicateProps.Count > 0)
            {
                return DuplicationOfPropertyNames + ": [" + String.Join(", ", duplicateProps.ToArray()) + "]";
            }
            var jsonDuplicateProps = jsonprops.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();
            if (jsonDuplicateProps.Count > 0)
            {
                return DuplicationOfJsonPropertyNames + ": [" + String.Join(", ", jsonDuplicateProps.ToArray()) + "]";
            }
            if (props.Count < 1)
            {
                return NoPropertiesSelected;
            }
            return null;
        }
        public static string CheckForHints(this ModelView srcModel)
        {
            if (srcModel == null)
            {
                return InputParamNotDefined;
            }
            string result = srcModel.CheckCorrect();
            if (!string.IsNullOrEmpty(result)) return result;
            result = "";

            if (srcModel.ForeignKeys != null)
            {
                foreach (ModelViewForeignKey foreignKey in srcModel.ForeignKeys)
                {
                    if (!string.IsNullOrEmpty(foreignKey.ViewName))
                    {
                        if (foreignKey.ScalarProperties != null)
                        {
                            if (foreignKey.ScalarProperties.Count > 0)
                            {
                                for(int i = 0; i <  foreignKey.PrincipalKeyProps.Count; i++)
                                {
                                    ModelViewKeyProperty princProp = foreignKey.PrincipalKeyProps[i];
                                    ModelViewKeyProperty foreignProp = foreignKey.ForeignKeyProps[i];

                                    if (foreignKey.ScalarProperties.Any(p => p.IsSelected
                                         && (p.ForeignKeyNameChain == foreignKey.NavigationName)
                                         && (p.OriginalPropertyName == princProp.OriginalPropertyName))
                                         &&
                                         (!srcModel.ScalarProperties.Any(p => p.IsSelected
                                         && (p.OriginalPropertyName == foreignProp.OriginalPropertyName))))
                                    {
                                        result += "\n" + UnderForeignKey + foreignKey.NavigationName + " " +
                                            PrincipalKeyPropertyIsIncluded + "[" + princProp.OriginalPropertyName + "]" +
                                            "\n" + ForeignKeyPropertyIsNotIncluded + "[" + foreignProp.OriginalPropertyName + "]" +
                                            "\n" + RecommendedSwapSelection;
                                    }
                                    else if (foreignKey.ScalarProperties.Any(p => p.IsSelected
                                         && (p.ForeignKeyNameChain == foreignKey.NavigationName)
                                         && (p.OriginalPropertyName == princProp.OriginalPropertyName))
                                         &&
                                         (srcModel.ScalarProperties.Any(p => p.IsSelected
                                         && (p.OriginalPropertyName == foreignProp.OriginalPropertyName))))
                                    {
                                        
                                        result += "\n" + UnderForeignKey + foreignKey.NavigationName + "  both " +
                                            PrincipalKeyPropertyIsIncluded + "[" + princProp.OriginalPropertyName + "]" +
                                            "\n" + ForeignKeyPropertyIsIncluded + "[" + foreignProp.OriginalPropertyName + "]" +
                                            "\n" + RecommendedRemovePrincipalSelection;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public static ModelViewSerializable ModelViewSerializableGetShallowCopy(this ModelViewSerializable srcModelViewSerializable)
        {
            if (srcModelViewSerializable == null) return null;
            return new ModelViewSerializable()
            {
                ViewName = srcModelViewSerializable.ViewName,
                PageViewName = srcModelViewSerializable.PageViewName,
                RootEntityClassName = srcModelViewSerializable.RootEntityClassName,
                RootEntityFullClassName = srcModelViewSerializable.RootEntityFullClassName,
                RootEntityUniqueProjectName = srcModelViewSerializable.RootEntityUniqueProjectName,
                ViewProject = srcModelViewSerializable.ViewProject,
                ViewDefaultProjectNameSpace = srcModelViewSerializable.ViewDefaultProjectNameSpace,
                ViewFolder = srcModelViewSerializable.ViewFolder,
                GenerateJSonAttribute = srcModelViewSerializable.GenerateJSonAttribute,
                ScalarProperties = srcModelViewSerializable.ScalarProperties,
                ForeignKeys = srcModelViewSerializable.ForeignKeys,
                PrimaryKeyProperties = srcModelViewSerializable.PrimaryKeyProperties,
                AllProperties = srcModelViewSerializable.AllProperties,
                CommonStaffs = srcModelViewSerializable.CommonStaffs,
                UIFormProperties = srcModelViewSerializable.UIFormProperties,
                UIListProperties = srcModelViewSerializable.UIListProperties,

                RootEntityDbContextPropertyName = srcModelViewSerializable.RootEntityDbContextPropertyName,
                WebApiServiceName = srcModelViewSerializable.WebApiServiceName,
                IsWebApiSelectAll = srcModelViewSerializable.IsWebApiSelectAll,
                IsWebApiSelectManyWithPagination = srcModelViewSerializable.IsWebApiSelectManyWithPagination,
                IsWebApiSelectOneByPrimarykey = srcModelViewSerializable.IsWebApiSelectOneByPrimarykey, 
                IsWebApiAdd = srcModelViewSerializable.IsWebApiAdd, 
                IsWebApiUpdate = srcModelViewSerializable.IsWebApiUpdate, 
                IsWebApiDelete = srcModelViewSerializable.IsWebApiDelete, 
                WebApiServiceProject = srcModelViewSerializable.WebApiServiceProject, 
                WebApiServiceDefaultProjectNameSpace = srcModelViewSerializable.WebApiServiceDefaultProjectNameSpace, 
                WebApiServiceFolder = srcModelViewSerializable.WebApiServiceFolder 
            };
        }
        public static ModelView ModelViewSerializableAssingTo(this ModelViewSerializable srcModelView, ModelView destModelView, DbContextSerializable CurrentDbContext, bool copyHeader = true)
        {
            if ((srcModelView == null) || (destModelView == null) || (CurrentDbContext == null)) return destModelView;
            if (copyHeader)
            {
                destModelView.ViewProject = srcModelView.ViewProject;
                destModelView.ViewDefaultProjectNameSpace = srcModelView.ViewDefaultProjectNameSpace;
                destModelView.ViewFolder = srcModelView.ViewFolder;
                destModelView.RootEntityClassName = srcModelView.RootEntityClassName;
                destModelView.RootEntityFullClassName = srcModelView.RootEntityFullClassName;
                destModelView.RootEntityUniqueProjectName = srcModelView.RootEntityUniqueProjectName;
                destModelView.RootEntityDbContextPropertyName = srcModelView.RootEntityDbContextPropertyName;
            }
            destModelView.ViewName = srcModelView.ViewName;
            destModelView.GenerateJSonAttribute = srcModelView.GenerateJSonAttribute;
            destModelView.PageViewName = srcModelView.PageViewName;
            if (srcModelView.ScalarProperties != null)
            {
                if (destModelView.ScalarProperties == null) {
                    destModelView.ScalarProperties = new ObservableCollection<ModelViewProperty>();
                }
                foreach(ModelViewPropertyOfVwSerializable srcProp in srcModelView.ScalarProperties)
                {
                    if (!string.IsNullOrEmpty(srcProp.ForeignKeyNameChain)) continue;
                    ModelViewProperty destProp = 
                        destModelView.ScalarProperties
                        .Where(p => p.OriginalPropertyName == srcProp.OriginalPropertyName).FirstOrDefault();
                    if (destProp != null)
                    {
                        destProp.IsSelected = true;
                    }
                }
            }
            if (srcModelView.ForeignKeys != null)
            {
                if (destModelView.ForeignKeys == null)
                {
                    destModelView.ForeignKeys = new ObservableCollection<ModelViewForeignKey>();
                }
                foreach(ModelViewForeignKeySerializable srcForeignKey in srcModelView.ForeignKeys)
                {
                    if (string.IsNullOrEmpty(srcForeignKey.ViewName)) continue;
                    ModelViewForeignKey destForeignKey =
                        destModelView.ForeignKeys.Where(f => f.NavigationName == srcForeignKey.NavigationName).FirstOrDefault();
                    if (destForeignKey == null) continue;
                    destForeignKey.ViewName = srcForeignKey.ViewName;
                    destForeignKey.ForeignKeyPrefix = srcForeignKey.ForeignKeyPrefix;
                    if (CurrentDbContext.ModelViews == null) continue;
                    ModelViewSerializable fkModelView =
                        CurrentDbContext.ModelViews.Where(m => m.ViewName == srcForeignKey.ViewName).FirstOrDefault();
                    if (fkModelView == null) continue;
                    if (fkModelView.ScalarProperties == null) continue;
                    if (destForeignKey.ScalarProperties == null)
                    {
                        destForeignKey.ScalarProperties = new ObservableCollection<ModelViewProperty>();
                    }
                    fkModelView.ScalarProperties.ForEach(mv => destForeignKey.ScalarProperties.Add(mv.ModelViewPropertySerializableAssingTo(new ModelViewProperty())));
                    bool isRequired = destForeignKey.ModelViewForeignKeyIsRequired();
                    foreach (ModelViewProperty prop in destForeignKey.ScalarProperties)
                    {
                        prop.IsRequiredInView = prop.IsRequiredInView && isRequired;
                    }
                    destForeignKey.OnModelViewForeignKeyPrefixChanged();
                    destForeignKey.ModelViewForeignKeyUpdateForeignKeyNameChain();
                    if (srcForeignKey.ScalarProperties == null) continue;
                    foreach(ModelViewPropertyOfFkSerializable srcProp in srcForeignKey.ScalarProperties)
                    {
                        ModelViewProperty destProp =
                            destForeignKey.ScalarProperties
                                .Where(p => (p.ForeignKeyNameChain == srcProp.ForeignKeyNameChain) && (p.OriginalPropertyName == srcProp.OriginalPropertyName))
                                .FirstOrDefault();
                        if (destProp != null)
                        {
                            destProp.IsSelected = srcProp.IsSelected;
                            destProp.JsonPropertyName = srcProp.JsonPropertyName;
                            destProp.ViewPropertyName = srcProp.ViewPropertyName;
                        }
                    }
                }
            }
            if (destModelView.UIFormProperties == null)
            {
                destModelView.UIFormProperties = new ObservableCollection<ModelViewUIFormProperty>();
            }
            destModelView.UIFormProperties.Clear();
            if (destModelView.UIListProperties == null)
            {
                destModelView.UIListProperties = new ObservableCollection<ModelViewUIListProperty>();
            }
            destModelView.UIListProperties.Clear();
            if (destModelView.ScalarProperties != null)
            {
                if(srcModelView.UIFormProperties != null)
                {
                    foreach(ModelViewUIFormPropertySerializable prop in srcModelView.UIFormProperties)
                    {
                        ModelViewProperty srcProp =
                         destModelView.ScalarProperties.FirstOrDefault(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain);
                        
                        destModelView.UIFormProperties.Add(new ModelViewUIFormProperty() {
                            OriginalPropertyName = prop.OriginalPropertyName,
                            InputTypeWhenAdd = prop.InputTypeWhenAdd,
                            InputTypeWhenUpdate = prop.InputTypeWhenUpdate,
                            InputTypeWhenDelete = prop.InputTypeWhenDelete,
                            ForeifKeyViewNameForAdd = prop.ForeifKeyViewNameForAdd,
                            ForeifKeyViewNameForUpd = prop.ForeifKeyViewNameForUpd,
                            ForeifKeyViewNameForDel = prop.ForeifKeyViewNameForDel,
                            ForeignKeyName = (srcProp == null) ? null : srcProp.ForeignKeyName,
                            ForeignKeyNameChain = prop.ForeignKeyNameChain,
                            ViewPropertyName = (srcProp == null) ? null : srcProp.ViewPropertyName,
                            JsonPropertyName = (srcProp == null) ? null : srcProp.JsonPropertyName,
                            IsShownInView = prop.IsShownInView,
                            IsNewLineAfter = prop.IsNewLineAfter
                        });
                    }
                }
                if (srcModelView.ScalarProperties != null)
                {
                    foreach (ModelViewPropertyOfVwSerializable prop in srcModelView.ScalarProperties)
                    {
                        if (!destModelView.UIFormProperties.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain))
                        {
                            destModelView.UIFormProperties.Add(new ModelViewUIFormProperty()
                            {
                                OriginalPropertyName = prop.OriginalPropertyName,
                                ForeignKeyName = prop.ForeignKeyName,
                                ForeignKeyNameChain = prop.ForeignKeyNameChain,
                                ViewPropertyName = prop.ViewPropertyName,
                                JsonPropertyName = prop.JsonPropertyName,
                            });

                        }
                    }
                }

                if (srcModelView.UIListProperties != null)
                {
                    foreach (ModelViewUIListPropertySerializable prop in srcModelView.UIListProperties)
                    {
                        ModelViewProperty srcProp =
                         destModelView.ScalarProperties.FirstOrDefault(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain);
                        destModelView.UIListProperties.Add(new ModelViewUIListProperty()
                        {
                            OriginalPropertyName = prop.OriginalPropertyName,
                            ForeignKeyName = (srcProp == null) ? null : srcProp.ForeignKeyName,
                            ForeignKeyNameChain = prop.ForeignKeyNameChain,
                            ViewPropertyName = (srcProp == null) ? null : srcProp.ViewPropertyName,
                            JsonPropertyName = (srcProp == null) ? null : srcProp.JsonPropertyName,
                            IsShownInView = prop.IsShownInView,
                            IsNewLineAfter = prop.IsNewLineAfter
                        });
                    }
                }
                if (srcModelView.ScalarProperties != null)
                {
                    foreach (ModelViewPropertyOfVwSerializable prop in srcModelView.ScalarProperties)
                    {
                        if (!destModelView.UIListProperties.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName && p.ForeignKeyNameChain == prop.ForeignKeyNameChain))
                        {
                            destModelView.UIListProperties.Add(new ModelViewUIListProperty()
                            {
                                OriginalPropertyName = prop.OriginalPropertyName,
                                ForeignKeyName = prop.ForeignKeyName,
                                ForeignKeyNameChain = prop.ForeignKeyNameChain,
                                ViewPropertyName = prop.ViewPropertyName,
                                JsonPropertyName = prop.JsonPropertyName,
                            });
                        }
                    }
                }

            }
            return destModelView;
        }
        public static ModelViewPropertyOfVwSerializable ModelViewPropertyOfVwNotifiedAssignTo(this ModelViewPropertyOfVwNotified src, ModelViewPropertyOfVwSerializable dest)
        {
            if ((src == null) || (dest == null)) return dest;
            dest.OriginalPropertyName = src.OriginalPropertyName;
            dest.TypeFullName = src.TypeFullName;
            dest.IsNullable = src.IsNullable;
            dest.IsRequired = src.IsRequired;
            dest.IsRequiredInView = src.IsRequiredInView;
            dest.UnderlyingTypeName = src.UnderlyingTypeName;
            dest.IsSelected = src.IsSelected;
            dest.ForeignKeyName = src.ForeignKeyName;
            dest.ForeignKeyNameChain = src.ForeignKeyNameChain;
            dest.ViewPropertyName = src.ViewPropertyName;
            dest.JsonPropertyName = src.JsonPropertyName;
            dest.IsUsedByfilter = src.IsUsedByfilter;
            dest.IsUsedBySorting = src.IsUsedBySorting;
            if (src.Attributes != null)
            {
                if (src.Attributes.Count > 0)
                {
                    if (dest.Attributes == null)
                    {
                        dest.Attributes = new List<ModelViewAttributeSerializable>();
                        foreach (ModelViewAttribute srcAttr in src.Attributes)
                        {
                            dest.Attributes.Add(srcAttr.ModelViewAttributeAssingTo(new ModelViewAttributeSerializable()));
                        }

                    }
                }
            }
            if (src.FAPIAttributes != null)
            {
                if (src.FAPIAttributes.Count > 0)
                {
                    if (dest.FAPIAttributes == null)
                    {
                        dest.FAPIAttributes = new List<ModelViewFAPIAttributeSerializable>();
                        foreach (ModelViewFAPIAttribute srcAttr in src.FAPIAttributes)
                        {
                            dest.FAPIAttributes.Add(srcAttr.ModelViewFAPIAttributeAssingTo(new ModelViewFAPIAttributeSerializable()));
                        }

                    }
                }
            }
            return dest;
        }
        public static ModelViewPropertyOfVwNotified ModelViewPropertyOfVwSerializableAssignTo(this ModelViewPropertyOfVwSerializable src, ModelViewPropertyOfVwNotified dest)
        {
            if ((src == null) || (dest == null)) return dest;
            dest.OriginalPropertyName = src.OriginalPropertyName;
            dest.TypeFullName = src.TypeFullName;
            dest.IsNullable = src.IsNullable;
            dest.IsRequired = src.IsRequired;
            dest.IsRequiredInView = src.IsRequiredInView;
            dest.UnderlyingTypeName = src.UnderlyingTypeName;
            dest.IsSelected = src.IsSelected;
            dest.ForeignKeyName = src.ForeignKeyName;
            dest.ForeignKeyNameChain = src.ForeignKeyNameChain;
            dest.ViewPropertyName = src.ViewPropertyName;
            dest.JsonPropertyName = src.JsonPropertyName;
            dest.IsUsedByfilter = src.IsUsedByfilter;
            dest.IsUsedBySorting = src.IsUsedBySorting;
            if (src.Attributes != null)
            {
                if (src.Attributes.Count > 0)
                {
                    if (dest.Attributes == null)
                    {
                        dest.Attributes = new ObservableCollection<ModelViewAttribute>();
                        foreach (ModelViewAttributeSerializable srcAttr in src.Attributes)
                        {
                            dest.Attributes.Add(srcAttr.ModelViewAttributeSerializableAssingTo(new ModelViewAttribute()));
                        }

                    }
                }
            }
            if (src.FAPIAttributes != null)
            {
                if (src.FAPIAttributes.Count > 0)
                {
                    if (dest.FAPIAttributes == null)
                    {
                        dest.FAPIAttributes = new ObservableCollection<ModelViewFAPIAttribute>();
                        foreach (ModelViewFAPIAttributeSerializable srcAttr in src.FAPIAttributes)
                        {
                            dest.FAPIAttributes.Add(srcAttr.ModelViewFAPIAttributeSerializableAssingTo(new ModelViewFAPIAttribute()));
                        }

                    }
                }
            }
            return dest;
        }
        public static ModelViewUIFormProperty ModelViewUIFormPropertySerializableAssignTo(this ModelViewUIFormPropertySerializable src, ModelViewUIFormProperty dest)
        {
            if ((src == null) || (dest == null)) return dest;
            dest.OriginalPropertyName = src.OriginalPropertyName;
            dest.InputTypeWhenAdd = src.InputTypeWhenAdd;
            dest.InputTypeWhenUpdate = src.InputTypeWhenUpdate;
            dest.InputTypeWhenDelete = src.InputTypeWhenDelete;
            dest.ForeifKeyViewNameForAdd = src.ForeifKeyViewNameForAdd;
            dest.ForeifKeyViewNameForUpd = src.ForeifKeyViewNameForUpd;
            dest.ForeifKeyViewNameForDel = src.ForeifKeyViewNameForDel;
            dest.ForeignKeyName = src.ForeignKeyName;
            dest.ForeignKeyNameChain = src.ForeignKeyNameChain;
            dest.ViewPropertyName = src.ViewPropertyName;
            dest.JsonPropertyName = src.JsonPropertyName;
            dest.IsShownInView = src.IsShownInView;
            dest.IsNewLineAfter = src.IsNewLineAfter;

            return dest;
        }
        public static ModelViewUIListProperty ModelViewUIListPropertySerializableAssignTo(this ModelViewUIListPropertySerializable src, ModelViewUIListProperty dest)
        {
            if ((src == null) || (dest == null)) return dest;
            dest.OriginalPropertyName = src.OriginalPropertyName;
            dest.ForeignKeyName = src.ForeignKeyName;
            dest.ForeignKeyNameChain = src.ForeignKeyNameChain;
            dest.ViewPropertyName = src.ViewPropertyName;
            dest.JsonPropertyName = src.JsonPropertyName;
            dest.IsShownInView = src.IsShownInView;
            dest.IsNewLineAfter = src.IsNewLineAfter;
            return dest;
        }
        public static ModelViewUIFormPropertySerializable ModelViewUIFormPropertyAssignTo(this ModelViewUIFormProperty src, ModelViewUIFormPropertySerializable dest)
        {
            if ((src == null) || (dest == null)) return dest;
            dest.OriginalPropertyName = src.OriginalPropertyName;
            dest.InputTypeWhenAdd = src.InputTypeWhenAdd;
            dest.InputTypeWhenUpdate = src.InputTypeWhenUpdate;
            dest.InputTypeWhenDelete = src.InputTypeWhenDelete;
            dest.ForeifKeyViewNameForAdd = src.ForeifKeyViewNameForAdd;
            dest.ForeifKeyViewNameForUpd = src.ForeifKeyViewNameForUpd;
            dest.ForeifKeyViewNameForDel = src.ForeifKeyViewNameForDel;
            dest.ForeignKeyName = src.ForeignKeyName;
            dest.ForeignKeyNameChain = src.ForeignKeyNameChain;
            dest.ViewPropertyName = src.ViewPropertyName;
            dest.JsonPropertyName = src.JsonPropertyName;
            dest.IsShownInView = src.IsShownInView;
            dest.IsNewLineAfter = src.IsNewLineAfter;

            return dest;
        }
        public static ModelViewUIListPropertySerializable ModelViewUIListPropertyAssignTo(this ModelViewUIListProperty src, ModelViewUIListPropertySerializable dest)
        {
            if ((src == null) || (dest == null)) return dest;
            dest.OriginalPropertyName = src.OriginalPropertyName;
            dest.ForeignKeyName = src.ForeignKeyName;
            dest.ForeignKeyNameChain = src.ForeignKeyNameChain;
            dest.ViewPropertyName = src.ViewPropertyName;
            dest.JsonPropertyName = src.JsonPropertyName;
            dest.IsShownInView = src.IsShownInView;
            dest.IsNewLineAfter = src.IsNewLineAfter;
            return dest;
        }
        public static List<ModelViewSerializable> GetViewsByForeignNameChain(this DbContextSerializable context, string ViewName, string foreignKeyNameChain) {
            if ( (context == null) || (string.IsNullOrEmpty(ViewName)) ) {
                return new List<ModelViewSerializable>();
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (mv == null)
            {
                return new List<ModelViewSerializable>();
            }
            
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return context.ModelViews
                    .Where(v => (v.RootEntityFullClassName == mv.RootEntityFullClassName) && (v.RootEntityUniqueProjectName == mv.RootEntityUniqueProjectName))
                    .ToList();
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (foreignKeys.Length < 1)
            {
                return new List<ModelViewSerializable>();
            }
            ModelViewForeignKeySerializable fk = 
                mv.ForeignKeys.Where(f => f.NavigationName == foreignKeys[0]).FirstOrDefault();
            if (fk == null)
            {
                return new List<ModelViewSerializable>();
            }
            if (foreignKeys.Length == 1)
            {
                return GetViewsByForeignNameChain(context, fk.ViewName, "");
            }
            return GetViewsByForeignNameChain(context, fk.ViewName, string.Join(".", foreignKeys, 1, foreignKeys.Length - 1));
        }

        public static List<ModelViewAttributeSerializable> ListModelViewAttributeSerializableGetCopy(this List<ModelViewAttributeSerializable> Attributes)
        {
            List<ModelViewAttributeSerializable> result = new List<ModelViewAttributeSerializable>();
            if (Attributes == null) return result;
            foreach (ModelViewAttributeSerializable modelViewAttributeSerializable in Attributes)
            {
                ModelViewAttributeSerializable destAttr = new ModelViewAttributeSerializable()
                {
                    AttrName = modelViewAttributeSerializable.AttrName,
                    AttrFullName = modelViewAttributeSerializable.AttrName
                };
                result.Add(destAttr);
                if (modelViewAttributeSerializable.VaueProperties != null)
                {
                    destAttr.VaueProperties = new List<ModelViewAttributePropertySerializable>();
                    foreach (ModelViewAttributePropertySerializable modelViewAttributePropertySerializable in modelViewAttributeSerializable.VaueProperties)
                    {
                        destAttr.VaueProperties.Add(new ModelViewAttributePropertySerializable()
                        {
                            PropName = modelViewAttributePropertySerializable.PropName,
                            PropValue = modelViewAttributePropertySerializable.PropValue,
                        });
                    }
                }
            }
            return result;
        }
        public static List<ModelViewFAPIAttributeSerializable> ListModelViewFAPIAttributeSerializableGetCopy(this List<ModelViewFAPIAttributeSerializable> FAPIAttributes)
        {
            List<ModelViewFAPIAttributeSerializable> result = new List<ModelViewFAPIAttributeSerializable>();
            if (FAPIAttributes == null) return result;
            foreach (ModelViewFAPIAttributeSerializable modelViewFAPIAttributeSerializable in FAPIAttributes)
            {
                ModelViewFAPIAttributeSerializable destFAttr = new ModelViewFAPIAttributeSerializable()
                {
                    AttrName = modelViewFAPIAttributeSerializable.AttrName
                };
                result.Add(destFAttr);
                if (modelViewFAPIAttributeSerializable.VaueProperties != null)
                {
                    destFAttr.VaueProperties = new List<ModelViewFAPIAttributePropertySerializable>();
                    foreach (ModelViewFAPIAttributePropertySerializable modelViewFAPIAttributePropertySerializable in modelViewFAPIAttributeSerializable.VaueProperties)
                    {
                        destFAttr.VaueProperties.Add(new ModelViewFAPIAttributePropertySerializable()
                        {
                            PropValue = modelViewFAPIAttributePropertySerializable.PropValue,
                        });
                    }
                }
            }
            return result;
        }
        public static List<ModelViewPropertyOfVwSerializable> ListModelViewPropertyOfVwSerializableGetCopy(this List<ModelViewPropertyOfVwSerializable> ScalarProperties)
        {
            List<ModelViewPropertyOfVwSerializable>  result = new List<ModelViewPropertyOfVwSerializable>();
            if (ScalarProperties == null) return result;
            foreach (ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializable in ScalarProperties)
            {
                ModelViewPropertyOfVwSerializable dest = new ModelViewPropertyOfVwSerializable()
                {
                    IsUsedByfilter = modelViewPropertyOfVwSerializable.IsUsedByfilter,
                    IsUsedBySorting = modelViewPropertyOfVwSerializable.IsUsedBySorting,
                    OriginalPropertyName = modelViewPropertyOfVwSerializable.OriginalPropertyName,
                    TypeFullName = modelViewPropertyOfVwSerializable.TypeFullName,
                    IsNullable = modelViewPropertyOfVwSerializable.IsNullable,
                    IsRequired = modelViewPropertyOfVwSerializable.IsRequired,
                    IsRequiredInView = modelViewPropertyOfVwSerializable.IsRequiredInView,
                    UnderlyingTypeName = modelViewPropertyOfVwSerializable.UnderlyingTypeName,
                    IsSelected = modelViewPropertyOfVwSerializable.IsSelected,
                    ForeignKeyName = modelViewPropertyOfVwSerializable.ForeignKeyName,
                    ForeignKeyNameChain = modelViewPropertyOfVwSerializable.ForeignKeyNameChain,
                    ViewPropertyName = modelViewPropertyOfVwSerializable.ViewPropertyName,
                    JsonPropertyName = modelViewPropertyOfVwSerializable.JsonPropertyName
                };
                result.Add(dest);
                dest.Attributes = ListModelViewAttributeSerializableGetCopy(modelViewPropertyOfVwSerializable.Attributes);
                dest.FAPIAttributes = ListModelViewFAPIAttributeSerializableGetCopy(modelViewPropertyOfVwSerializable.FAPIAttributes);
            }
            return result;
        }
        public static List<ModelViewKeyPropertySerializable> ListModelViewKeyPropertySerializableGetCopy(this List<ModelViewKeyPropertySerializable> PrimaryKeyProperties)
        {
            List<ModelViewKeyPropertySerializable> result = new List<ModelViewKeyPropertySerializable>();
            if (PrimaryKeyProperties == null) return result;
            foreach (ModelViewKeyPropertySerializable modelViewKeyPropertySerializable in PrimaryKeyProperties)
            {
                ModelViewKeyPropertySerializable dest = new ModelViewKeyPropertySerializable()
                {
                    OriginalPropertyName = modelViewKeyPropertySerializable.OriginalPropertyName,
                    TypeFullName = modelViewKeyPropertySerializable.TypeFullName,
                    IsNullable = modelViewKeyPropertySerializable.IsNullable,
                    IsRequired = modelViewKeyPropertySerializable.IsRequired,
                    UnderlyingTypeName = modelViewKeyPropertySerializable.UnderlyingTypeName,
                    ViewPropertyName = modelViewKeyPropertySerializable.ViewPropertyName,
                    JsonPropertyName = modelViewKeyPropertySerializable.JsonPropertyName,
                };
                result.Add(dest);
            }
            return result;
        }
        public static List<ModelViewEntityPropertySerializable> ListModelViewEntityPropertySerializableGetCopy(this List<ModelViewEntityPropertySerializable> PrimaryEntityProperties)
        {
            List<ModelViewEntityPropertySerializable> result = new List<ModelViewEntityPropertySerializable>();
            if (PrimaryEntityProperties == null) return result;
            foreach (ModelViewEntityPropertySerializable modelViewEntityPropertySerializable in PrimaryEntityProperties)
            {
                ModelViewEntityPropertySerializable dest = new ModelViewEntityPropertySerializable()
                {
                    OriginalPropertyName = modelViewEntityPropertySerializable.OriginalPropertyName,
                    TypeFullName = modelViewEntityPropertySerializable.TypeFullName,
                    IsNullable = modelViewEntityPropertySerializable.IsNullable,
                    IsRequired = modelViewEntityPropertySerializable.IsRequired,
                    UnderlyingTypeName = modelViewEntityPropertySerializable.UnderlyingTypeName,
                    ViewPropertyName = modelViewEntityPropertySerializable.ViewPropertyName,
                    JsonPropertyName = modelViewEntityPropertySerializable.JsonPropertyName,
                };
                result.Add(dest);
                dest.Attributes = ListModelViewAttributeSerializableGetCopy(modelViewEntityPropertySerializable.Attributes);
                dest.FAPIAttributes = ListModelViewFAPIAttributeSerializableGetCopy(modelViewEntityPropertySerializable.FAPIAttributes);
            }
            return result;
        }
        public static List<ModelViewPropertyOfFkSerializable> ListModelViewPropertyOfFkSerializableGetCopy(this List<ModelViewPropertyOfFkSerializable> ScalarProperties)
        {
            List<ModelViewPropertyOfFkSerializable> result = new List<ModelViewPropertyOfFkSerializable>();
            if (ScalarProperties == null) return result;
            foreach (ModelViewPropertyOfFkSerializable modelViewPropertyOfVwSerializable in ScalarProperties)
            {
                ModelViewPropertyOfFkSerializable dest = new ModelViewPropertyOfFkSerializable()
                {
                    OriginalPropertyName = modelViewPropertyOfVwSerializable.OriginalPropertyName,
                    TypeFullName = modelViewPropertyOfVwSerializable.TypeFullName,
                    IsNullable = modelViewPropertyOfVwSerializable.IsNullable,
                    IsRequired = modelViewPropertyOfVwSerializable.IsRequired,
                    IsRequiredInView = modelViewPropertyOfVwSerializable.IsRequiredInView,
                    UnderlyingTypeName = modelViewPropertyOfVwSerializable.UnderlyingTypeName,
                    IsSelected = modelViewPropertyOfVwSerializable.IsSelected,
                    ForeignKeyName = modelViewPropertyOfVwSerializable.ForeignKeyName,
                    ForeignKeyNameChain = modelViewPropertyOfVwSerializable.ForeignKeyNameChain,
                    ViewPropertyName = modelViewPropertyOfVwSerializable.ViewPropertyName,
                    JsonPropertyName = modelViewPropertyOfVwSerializable.JsonPropertyName
                };
                result.Add(dest);
            }
            return result;
        }
        public static string ReplaceLastName(string EntityFullName, string NavigationEntityName)
        {
            if (string.IsNullOrEmpty(EntityFullName)) return NavigationEntityName;
            int ind = EntityFullName.LastIndexOf('.');
            if(ind < 0) return NavigationEntityName;
            return EntityFullName.Substring(0, ind) + NavigationEntityName;
        }
        public static List<ModelViewForeignKeySerializable> ListModelViewForeignKeySerializableGetCopy(this List<ModelViewForeignKeySerializable> ForeignKeys, string uniqueProjectName, string entityFullName)
        {
            List<ModelViewForeignKeySerializable> result = new List<ModelViewForeignKeySerializable>();
            if (ForeignKeys == null) return result;
            foreach (ModelViewForeignKeySerializable modelViewForeignKeySerializable in ForeignKeys)
            {
                ModelViewForeignKeySerializable dest = new ModelViewForeignKeySerializable()
                {
                    NavigationName = modelViewForeignKeySerializable.NavigationName,
                    InverseNavigationName = modelViewForeignKeySerializable.InverseNavigationName,
                    EntityName = modelViewForeignKeySerializable.EntityName,
                    EntityFullName = ReplaceLastName(entityFullName, modelViewForeignKeySerializable.EntityName),
                    EntityUniqueProjectName = uniqueProjectName,
                    NavigationEntityName = modelViewForeignKeySerializable.NavigationEntityName,
                    NavigationEntityFullName = ReplaceLastName(entityFullName, modelViewForeignKeySerializable.NavigationEntityName),
                    NavigationEntityUniqueProjectName = uniqueProjectName,

                    NavigationType = modelViewForeignKeySerializable.NavigationType,
                    ForeignKeySource = modelViewForeignKeySerializable.ForeignKeySource,
                    PrincipalKeySource = modelViewForeignKeySerializable.PrincipalKeySource,

                    InverseNavigationSource = modelViewForeignKeySerializable.InverseNavigationSource,
                    IsCascadeDelete = modelViewForeignKeySerializable.IsCascadeDelete,
                    ViewName = modelViewForeignKeySerializable.ViewName,
                    ForeignKeyPrefix = modelViewForeignKeySerializable.ForeignKeyPrefix,
                };
                result.Add(dest);
                dest.ScalarProperties = ListModelViewPropertyOfFkSerializableGetCopy(modelViewForeignKeySerializable.ScalarProperties);
            }
            return result;
        }
        public static List<ModelViewUIFormPropertySerializable> ListModelViewForeignKeySerializableGetCopy(this List<ModelViewUIFormPropertySerializable> UIFormProperties)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if (UIFormProperties == null) return result;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in UIFormProperties)
            {
                ModelViewUIFormPropertySerializable dest = new ModelViewUIFormPropertySerializable()
                {
                    InputTypeWhenAdd = modelViewUIFormPropertySerializable.InputTypeWhenAdd,
                    InputTypeWhenUpdate = modelViewUIFormPropertySerializable.InputTypeWhenUpdate,
                    InputTypeWhenDelete = modelViewUIFormPropertySerializable.InputTypeWhenDelete,
                    ForeifKeyViewNameForAdd = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd,

                    ForeifKeyViewNameForUpd = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd,
                    ForeifKeyViewNameForDel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel,

                    OriginalPropertyName = modelViewUIFormPropertySerializable.OriginalPropertyName,

                    ForeignKeyName = modelViewUIFormPropertySerializable.ForeignKeyName,
                    ForeignKeyNameChain = modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                    ViewPropertyName = modelViewUIFormPropertySerializable.ViewPropertyName,
                    JsonPropertyName = modelViewUIFormPropertySerializable.JsonPropertyName,

                    IsShownInView = modelViewUIFormPropertySerializable.IsShownInView,
                    IsNewLineAfter = modelViewUIFormPropertySerializable.IsNewLineAfter,
                };
                result.Add(dest);
            }
            return result;
        }
        public static List<ModelViewUIListPropertySerializable> ListModelViewForeignKeySerializableGetCopy(this List<ModelViewUIListPropertySerializable> UIListProperties)
        {
            List<ModelViewUIListPropertySerializable> result = new List<ModelViewUIListPropertySerializable>();
            if (UIListProperties == null) return result;
            foreach (ModelViewUIListPropertySerializable modelViewUIListPropertySerializable in UIListProperties)
            {
                ModelViewUIListPropertySerializable dest = new ModelViewUIListPropertySerializable()
                {
                    OriginalPropertyName = modelViewUIListPropertySerializable.OriginalPropertyName,

                    ForeignKeyName = modelViewUIListPropertySerializable.ForeignKeyName,
                    ForeignKeyNameChain = modelViewUIListPropertySerializable.ForeignKeyNameChain,
                    ViewPropertyName = modelViewUIListPropertySerializable.ViewPropertyName,
                    JsonPropertyName = modelViewUIListPropertySerializable.JsonPropertyName,

                    IsShownInView = modelViewUIListPropertySerializable.IsShownInView,
                    IsNewLineAfter = modelViewUIListPropertySerializable.IsNewLineAfter,
                };
                result.Add(dest);
            }
            return result;
        }
        public static ModelViewSerializable ModelViewSerializableGetCopy(this ModelViewSerializable srcModelViewSerializable, string destinationProject, string defaultProjectNameSpace, string destinationFolder, string dbSetProppertyName, SolutionCodeElement SelectedEntity)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (srcModelViewSerializable == null) return null;
            string UniqueProjectName = (SelectedEntity.CodeElementRef as CodeClass).ProjectItem.ContainingProject.UniqueName;
            string EntityFullClassName = SelectedEntity.CodeElementFullName;
            ModelViewSerializable result = new ModelViewSerializable()
            {
                RootEntityClassName = SelectedEntity.CodeElementName,
                RootEntityFullClassName = EntityFullClassName,
                RootEntityDbContextPropertyName = dbSetProppertyName,
                RootEntityUniqueProjectName = UniqueProjectName,

                ViewName = srcModelViewSerializable.ViewName,
                PageViewName = srcModelViewSerializable.PageViewName, 
                ViewProject = destinationProject, 
                ViewDefaultProjectNameSpace = defaultProjectNameSpace,
                ViewFolder = destinationFolder,
                GenerateJSonAttribute = srcModelViewSerializable.GenerateJSonAttribute,

                WebApiServiceName = srcModelViewSerializable.WebApiServiceName,
                WebApiServiceProject = srcModelViewSerializable.WebApiServiceProject,
                WebApiServiceDefaultProjectNameSpace = srcModelViewSerializable.WebApiServiceDefaultProjectNameSpace,
                WebApiServiceFolder = srcModelViewSerializable.WebApiServiceFolder,

                IsWebApiSelectAll = srcModelViewSerializable.IsWebApiSelectAll,
                IsWebApiSelectManyWithPagination = srcModelViewSerializable.IsWebApiSelectManyWithPagination,
                IsWebApiSelectOneByPrimarykey = srcModelViewSerializable.IsWebApiSelectOneByPrimarykey,
                IsWebApiAdd = srcModelViewSerializable.IsWebApiAdd,
                IsWebApiUpdate = srcModelViewSerializable.IsWebApiUpdate,
                IsWebApiDelete = srcModelViewSerializable.IsWebApiDelete,
            };
            result.ScalarProperties = ListModelViewPropertyOfVwSerializableGetCopy(srcModelViewSerializable.ScalarProperties);
            result.PrimaryKeyProperties = ListModelViewKeyPropertySerializableGetCopy(srcModelViewSerializable.PrimaryKeyProperties);
            result.AllProperties = ListModelViewEntityPropertySerializableGetCopy(srcModelViewSerializable.AllProperties);
            result.ForeignKeys = ListModelViewForeignKeySerializableGetCopy(srcModelViewSerializable.ForeignKeys, UniqueProjectName, EntityFullClassName);
            result.UIFormProperties = ListModelViewForeignKeySerializableGetCopy(srcModelViewSerializable.UIFormProperties);
            result.UIListProperties = ListModelViewForeignKeySerializableGetCopy(srcModelViewSerializable.UIListProperties);
            return result;
        }
        public static ModelViewSerializable ModelViewSerializableSimpleGetCopy(this ModelViewSerializable srcModelViewSerializable, string destinationProject, string defaultProjectNameSpace, string destinationFolder)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (srcModelViewSerializable == null) return null;
            ModelViewSerializable result = new ModelViewSerializable()
            {
                RootEntityClassName = srcModelViewSerializable.RootEntityClassName,
                RootEntityFullClassName = srcModelViewSerializable.RootEntityFullClassName,
                RootEntityDbContextPropertyName = srcModelViewSerializable.RootEntityDbContextPropertyName,
                RootEntityUniqueProjectName = srcModelViewSerializable.RootEntityUniqueProjectName,

                ViewName = srcModelViewSerializable.ViewName,
                PageViewName = srcModelViewSerializable.PageViewName,
                ViewProject = destinationProject,
                ViewDefaultProjectNameSpace = defaultProjectNameSpace,
                ViewFolder = destinationFolder,
                GenerateJSonAttribute = srcModelViewSerializable.GenerateJSonAttribute,

                WebApiServiceName = srcModelViewSerializable.WebApiServiceName,
                WebApiServiceProject = srcModelViewSerializable.WebApiServiceProject,
                WebApiServiceDefaultProjectNameSpace = srcModelViewSerializable.WebApiServiceDefaultProjectNameSpace,
                WebApiServiceFolder = srcModelViewSerializable.WebApiServiceFolder,

                IsWebApiSelectAll = srcModelViewSerializable.IsWebApiSelectAll,
                IsWebApiSelectManyWithPagination = srcModelViewSerializable.IsWebApiSelectManyWithPagination,
                IsWebApiSelectOneByPrimarykey = srcModelViewSerializable.IsWebApiSelectOneByPrimarykey,
                IsWebApiAdd = srcModelViewSerializable.IsWebApiAdd,
                IsWebApiUpdate = srcModelViewSerializable.IsWebApiUpdate,
                IsWebApiDelete = srcModelViewSerializable.IsWebApiDelete,
            };
            result.ScalarProperties = ListModelViewPropertyOfVwSerializableGetCopy(srcModelViewSerializable.ScalarProperties);
            result.PrimaryKeyProperties = ListModelViewKeyPropertySerializableGetCopy(srcModelViewSerializable.PrimaryKeyProperties);
            result.AllProperties = ListModelViewEntityPropertySerializableGetCopy(srcModelViewSerializable.AllProperties);
            result.ForeignKeys = ListModelViewForeignKeySerializableGetCopy(srcModelViewSerializable.ForeignKeys, srcModelViewSerializable.RootEntityUniqueProjectName, srcModelViewSerializable.RootEntityFullClassName);
            result.UIFormProperties = ListModelViewForeignKeySerializableGetCopy(srcModelViewSerializable.UIFormProperties);
            result.UIListProperties = ListModelViewForeignKeySerializableGetCopy(srcModelViewSerializable.UIListProperties);
            return result;
        }


        public static ModelViewAttribute CloneModelViewAttribute(this ModelViewAttribute src)
        {
            ModelViewAttribute dest = new ModelViewAttribute();
            if (src == null) return dest;
            dest.AttrName = src.AttrName;
            dest.AttrFullName = src.AttrFullName;
            dest.VaueProperties = new ObservableCollection<ModelViewAttributeProperty>();
            if(src.VaueProperties != null)
            {
                foreach(ModelViewAttributeProperty vp in src.VaueProperties)
                {
                    if (vp != null)
                    {
                        dest.VaueProperties.Add(new ModelViewAttributeProperty()
                        {
                            PropName = vp.PropName,
                            PropValue = vp.PropValue
                        });
                    }
                }
            }
            return dest;
        }
        public static ObservableCollection<ModelViewAttribute> CloneModelViewAttributeCollection(this ObservableCollection<ModelViewAttribute> src)
        {
            ObservableCollection<ModelViewAttribute> dest = new ObservableCollection<ModelViewAttribute>();
            if (src == null) return dest;
            foreach(ModelViewAttribute srcItm in src)
            {
                if (srcItm != null)
                {
                    dest.Add(srcItm.CloneModelViewAttribute());
                }
            }
            return dest;
        }

        public static ModelViewFAPIAttribute CloneModelViewFAPIAttribute(this ModelViewFAPIAttribute src)
        {
            ModelViewFAPIAttribute dest = new ModelViewFAPIAttribute();
            if (src == null) return dest;
            dest.AttrName = src.AttrName;
            dest.VaueProperties = new ObservableCollection<ModelViewFAPIAttributeProperty>();
            if (src.VaueProperties != null)
            {
                foreach (ModelViewFAPIAttributeProperty vp in src.VaueProperties)
                {
                    if (vp != null)
                    {
                        dest.VaueProperties.Add(new ModelViewFAPIAttributeProperty()
                        {
                            PropValue = vp.PropValue
                        });
                    }
                }
            }
            return dest;
        }
        public static ObservableCollection<ModelViewFAPIAttribute> CloneModelViewFAPIAttributeCollection(this ObservableCollection<ModelViewFAPIAttribute> src)
        {
            ObservableCollection<ModelViewFAPIAttribute> dest = new ObservableCollection<ModelViewFAPIAttribute>();
            if (src == null) return dest;
            foreach (ModelViewFAPIAttribute srcItm in src)
            {
                if (srcItm != null)
                {
                    dest.Add(srcItm.CloneModelViewFAPIAttribute());
                }
            }
            return dest;
        }
    }
}
