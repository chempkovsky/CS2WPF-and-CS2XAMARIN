using CS2ANGULAR.Helpers.UI;
using CS2ANGULAR.Model;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace CS2ANGULAR.ViewModel
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema?view=netframework-4.8
    /// </summary>
    #pragma warning disable VSTHRD010
    public class ClassFiledSelectorRootViewModel : IsReadyViewModel
    {


        #region Fields
        DTE2 Dte = null;
        SolutionCodeElement selectedCodeElement = null;
        SolutionProject selectedProject = null;
        Project destinationProject = null;
        #endregion

        public ClassFiledSelectorRootViewModel(DTE2 dte): base()
        {
            this.Dte = dte;
        }

        public string DisplayName
        {
            get
            {
                return OutputClassName;
            }
        }

        bool _includeInView = false;
        public bool IncludeInView
        {
            get
            {
                return _includeInView;
            }
            set
            {
                if (_includeInView != value)
                {
                    _includeInView = value;
                    OnPropertyChanged("IncludeInView");
                    if (ForeigKeyParentProperties == null) return;
                    foreach (PropertySelectorViewModel itm in ForeigKeyParentProperties)
                    {
                        itm.IncludeInView = value;
                    }
                }
            }
        }



        public string RootNodeClassName { get; set; } = "";
        public string RootNodeNameSapce { get; set; } = "";
        public string RootNodeProjectName { get; set; } = "";
        public SolutionCodeElement SelectedCodeElement 
        { 
            get
            {
                return selectedCodeElement;
            }
            set
            {
                if (selectedCodeElement != value)
                {
                    ClearClassFiledSelectorData();
                    IncludeInView = false;
                }
                selectedCodeElement = value;
                RootNodeClassName = "";
                RootNodeNameSapce = "";
                if (selectedCodeElement != null)
                {
                    if(selectedCodeElement.CodeElementRef != null)
                    {
                        RootNodeClassName = selectedCodeElement.CodeElementRef.Name;
                        RootNodeNameSapce = (selectedCodeElement.CodeElementRef as CodeClass).Namespace.FullName;
                    }
                }
            } 
        }
        public SolutionProject SelectedProject 
        {
            get { 
                return selectedProject; 
            } 
            set
            {
                if (selectedProject == value) return;
                selectedProject = value;
                RootNodeProjectName = "";
                if (selectedProject != null)
                {
                    if(selectedProject.ProjectRef != null)
                    {
                        RootNodeProjectName = selectedProject.ProjectRef.UniqueName;
                    }
                }
            }
        }
        public string DestinationNameSpace { get; set; }
        public string DestinationFoldersChain { get; set; }
        string _outputClassName = "";
        public string OutputClassName 
        { 
            get 
            {
                return _outputClassName;
            }
            set
            {
                _outputClassName = value;
                OnPropertyChanged("OutputClassName");
                OnPropertyChanged("DisplayName");
            } 
        }
        public string DestinationProjectName { get; set; }
        public Project DestinationProject
        {
            get
            {
                return destinationProject;
            }
            set
            {
                if (destinationProject == value) return;
                destinationProject = value;
                DestinationProjectName = "";
                if (destinationProject != null)
                {
                    DestinationProjectName = destinationProject.UniqueName;
                }
            }
        }
        public Visibility PropertyDetailsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility ForeifKeyDetailsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility HintVisibility { get; set; } = Visibility.Visible;
        public Visibility RootVisibility { get; set; } = Visibility.Collapsed;
        public bool GenerateJSonAttribute { get; set; }
        public int MaxNestedLevel { get; set; } = 3;
        IList<LookUpViewModel> lookUpViewModels;
        public IList<LookUpViewModel> LookUpViewModels { 
            get { 
                if (lookUpViewModels == null)
                {
                    lookUpViewModels = new List<LookUpViewModel>() { 
                        new LookUpViewModel()
                        {
                            LookUpViewModelName = "LookUpViewModelName1",
                            JSLookUpViewModelName = "JSLookUpViewModelName1",
                            LookUpClassFields = new List<LookUpClassField>()
                            {
                                new LookUpClassField() { ViewModelFieldName = "ViewModelFiledName11"},
                                new LookUpClassField() { ViewModelFieldName = "ViewModelFiledName12"}
                            }
                        },
                        new LookUpViewModel()
                        {
                            LookUpViewModelName = "LookUpViewModelName2",
                            JSLookUpViewModelName = "JSLookUpViewModelName2",
                            LookUpClassFields = new List<LookUpClassField>()
                            {
                                new LookUpClassField() { ViewModelFieldName = "ViewModelFiledName21"},
                                new LookUpClassField() { ViewModelFieldName = "ViewModelFiledName22"}
                            }
                        }
                    };
                }
                return lookUpViewModels;
            }  
        }
        Object selectedTreeViewItem;
        public Object SelectedTreeViewItem
        {
            get
            {
                return selectedTreeViewItem;
            }
            set
            {
                if (selectedTreeViewItem != value)
                {
                    selectedTreeViewItem = value;
                    if (selectedTreeViewItem == null)
                    {
                        PropertyDetailsVisibility = Visibility.Collapsed;
                        ForeifKeyDetailsVisibility = Visibility.Collapsed;
                        RootVisibility = Visibility.Collapsed;
                        HintVisibility = Visibility.Visible;
                    } else
                    {
                        HintVisibility = Visibility.Collapsed;
                        if (selectedTreeViewItem is ClassFiledSelectorViewModel)
                        {
                            PropertyDetailsVisibility = Visibility.Visible;
                            ForeifKeyDetailsVisibility = Visibility.Collapsed;
                            RootVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            if (selectedTreeViewItem is PropertySelectorViewModel)
                            {
                                PropertyDetailsVisibility = Visibility.Collapsed;
                                ForeifKeyDetailsVisibility = Visibility.Visible;
                                RootVisibility = Visibility.Collapsed;
                            } 
                            else
                            {
                                RootVisibility = Visibility.Visible;
                                PropertyDetailsVisibility = Visibility.Collapsed;
                                ForeifKeyDetailsVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    OnPropertyChanged("PropertyDetailsVisibility");
                    OnPropertyChanged("ForeifKeyDetailsVisibility");
                    OnPropertyChanged("RootVisibility");
                    OnPropertyChanged("HintVisibility");
                    OnPropertyChanged("SelectedTreeViewItem");
                }
            }
        }
        public ObservableCollection<PropertySelectorViewModel> ForeigKeyParentProperties { get; set; }
        #region helper methods
        public void ClearClassFiledSelectorData()
        {
            if (ForeigKeyParentProperties == null)
            {
                ForeigKeyParentProperties = new ObservableCollection<PropertySelectorViewModel>();
            } else
            {
                ForeigKeyParentProperties.Clear();
                OnPropertyChanged("ForeigKeyParentProperties");
            }
        }
        public void PrepareClassFiledSelectorData()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (ForeigKeyParentProperties.Count > 0) return;
            if (SelectedCodeElement == null) return;
            if (SelectedCodeElement.CodeElementRef == null) return;
            if (SelectedCodeElement.CodeElementRef.Kind != vsCMElement.vsCMElementClass) return;
            DoPrepareClassFiledSelectorData("","",ForeigKeyParentProperties, SelectedCodeElement.CodeElementRef,null,0);
        }
        protected void DoPrepareClassFiledSelectorData(string childForeignKeyPrefix, string childForeignKeyName, ObservableCollection<PropertySelectorViewModel> fkpp, CodeElement currentCodeElement, List<SolutionCodeElement> primKeyProps, int currentNestedLevel)
        {
            currentNestedLevel++;
            if (this.MaxNestedLevel < currentNestedLevel)
            {
                return;
            }
            if (fkpp == null) return;
            if (fkpp.Count > 0) return;
            if (currentCodeElement.Kind != vsCMElement.vsCMElementClass) return;
            CodeClass currentCodeClass = currentCodeElement as CodeClass;
            if (primKeyProps == null)
            {
                primKeyProps = GetPrimaryKeyProperties(currentCodeClass);
            }
            if (primKeyProps.Count < 1)
            {
                // throw an exception here
                return;
            }

            IList<CodeProperty> navigationProps = new List<CodeProperty>();
            IList<String> fkeys = new List<String>();
            int columnOrder = 0;
            foreach(CodeElement ce in currentCodeClass.Members)
            {
                fkeys.Clear();
                bool isNotMapped = false;
                //bool keyAttrib = false;
                bool hasForeignAttrib = false;
                string foreignKeyName = "";
                bool inversePropertyAttribute = false;
                bool requiredAttribute = false;
                // bool columnAttribute = false;
                int columnAttributeOrder = -1;
                String typeName = "";
                bool typeIsNullable = false;
                if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty cp = ce as CodeProperty;
                if (cp.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (cp.Type == null) continue;
                if (cp.Type.CodeType == null) continue;
                foreach(CodeElement cea in cp.Attributes)
                {
                    CodeAttribute ca = cea as CodeAttribute;
                    if(ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                    {
                        isNotMapped = true;
                    }
                    //if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.KeyAttribute"))
                    //{
                    //    keyAttrib = true;
                    //}
                    if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute"))
                    {
                        hasForeignAttrib = true;
                        foreach (CodeElement chld in ca.Children)
                        {
                            if (chld is CodeAttributeArgument)
                            {
                                foreignKeyName = (chld as CodeAttributeArgument).Value;
                            }
                            foreignKeyName = foreignKeyName.Replace("\"", "");
                            if (!string.IsNullOrEmpty(foreignKeyName)) {
                                if (!fkeys.Contains(foreignKeyName))
                                {
                                    fkeys.Add(foreignKeyName);
                                }
                            }
                        }
                    }
                    if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute"))
                    {
                        inversePropertyAttribute = true;
                    }
                    if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.RequiredAttribute"))
                    {
                        requiredAttribute = true;
                    }
                    if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"))
                    {
                        // columnAttribute = true;
                        foreach (CodeElement chld in ca.Children)
                        {
                            
                            if("Order".Equals( chld.Name, System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (chld is CodeAttributeArgument)
                                {
                                    int val;
                                    if(int.TryParse((chld as CodeAttributeArgument).Value, out val))
                                    {
                                        columnAttributeOrder = val;
                                    }
                                }
                            }
                        }
                    }
                }
                if (inversePropertyAttribute || isNotMapped) continue;

                CodeTypeRef ctf = cp.Type;
                // if CodeTypeRef.TypeKind = vsCMTypeRef.vsCMTypeRefArray the 'CodeTypeRef.ElementType' holds the type of the array Item
                // but in our case we will ignory Arrays
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType)
                {
                    
                    // this is ForeignKey-navigation property so it must be saved for a while
                    if (cp.Type.CodeType.Kind == vsCMElement.vsCMElementClass)
                    {
                        navigationProps.Add(cp);
                        continue;
                    }
                    // ICollection<T> property: this is InverseProperty(...) so it must be ignored
                    if (cp.Type.CodeType.Kind == vsCMElement.vsCMElementInterface)
                    {
                        string fl = cp.Type.CodeType.FullName;
                        if (fl != null) fl = "";
                        continue;
                    }
                    if (!ctf.AsFullName.StartsWith("System.Nullable")) continue;
                    typeName = ctf.AsFullName.Replace("System.Nullable", "").Replace("<", "").Replace(">", "").Trim();
                    typeIsNullable = true;
                } else
                {
                    typeName = ctf.AsFullName;
                }



                


                if (columnAttributeOrder < 0) columnAttributeOrder = columnOrder;
                ClassFiledSelectorViewModel aProperty = new ClassFiledSelectorViewModel()
                {
                    OriginalPropertyName = cp.Name,
                    ViewModelFieldName = childForeignKeyPrefix + childForeignKeyName + cp.Name,
                    JsonPropertyFieldName = childForeignKeyPrefix + childForeignKeyName + cp.Name,
                    FieldOrder = columnAttributeOrder,
                    IsForeignKeyField = hasForeignAttrib,
                    ForeignKeyName = childForeignKeyName,
                    ForeignKeyAlias = childForeignKeyName,
                    ChildForeignKeyPrefix = childForeignKeyPrefix,
                    TypeFullName = ctf.AsFullName,
                    UnderlyingTypeName = typeName,
                    TypeIsNullable = typeIsNullable,
                    UpdateDependent = true,
                    UpdateNested = true,
                    PocoName = currentCodeClass.Name,
                    PocoFullName = currentCodeClass.FullName,
                    HasRequiredAttribute = requiredAttribute
                };
                if(hasForeignAttrib)
                {
                    foreach(string fk in fkeys)
                    {
                        if(aProperty.ForeigKeyParentProperties == null)
                        {
                            aProperty.ForeigKeyParentProperties = new ObservableCollection<PropertySelectorViewModel>();
                        }
                        PropertySelectorViewModel fko = new PropertySelectorViewModel()
                        {
                            ForeignKeyName = fk,
                            ChildForeignKeyPrefix = childForeignKeyPrefix + childForeignKeyName,
                            ForeignKeyAlias = fk,
                            UpdateDependent = true,
                            UpdateNested = true,
                        };
                        aProperty.ForeigKeyParentProperties.Add(fko);
                    }
                }
                fkpp.Add(aProperty);
                columnOrder++;
            }

            foreach (SolutionCodeElement cp in primKeyProps)
            {
                string propNmae = cp.CodeElementName;
                foreach (ClassFiledSelectorViewModel itm in fkpp)
                {
                    if(propNmae.Equals( itm.OriginalPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        itm.IsKeyField = true;
                        break;
                    }
                }
            }

            foreach (CodeProperty cp in navigationProps)
            {
                DefineForeignKeyNodes(childForeignKeyPrefix, childForeignKeyName, cp, fkpp, currentCodeClass.Name, currentNestedLevel);     
            }

            OnPropertyChanged("ForeigKeyParentProperties");
        }
        /// <summary>
        /// GetPrimaryKeyProperties returns the list of properties which are used as a Primary Key
        /// SolutionCodeElement.CodeElementRef holds CodeElement object that can be typecasted to CodeProperty
        /// </summary>
        /// <remarks>
        /// the following rules are applied:
        /// if KeyAttribute is found then "Id"-property and "ClassName"+"Id"-property will be ignoried
        /// if KeyAttribute is not found but there is "Id"-property then "ClassName"+"Id"-property will be ignoried
        /// if there is no KeyAttribute, "Id"-property and "ClassName"+"Id"-property then List<SolutionCodeElement> will be returned with Count==0
        /// </remarks>
        protected List<SolutionCodeElement> GetPrimaryKeyProperties(CodeClass cc)
        {
            List<SolutionCodeElement> result = new List<SolutionCodeElement>();
            int currOrder = -1;
            foreach (CodeElement ce in cc.Members)
            {
                currOrder++;
                if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty cp = ce as CodeProperty;
                if (cp.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (cp.Type == null) continue;
                if (cp.Type.CodeType == null) continue;
                bool isNotMapped = false;
                foreach (CodeElement cea in cp.Attributes)
                {
                    CodeAttribute ca = cea as CodeAttribute;
                    if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                    {
                        isNotMapped = true;
                        break;
                    }
                }
                if (isNotMapped) continue;
                CodeTypeRef ctf = cp.Type;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType) continue;

                foreach (CodeElement cea in cp.Attributes)
                {
                    CodeAttribute ca = cea as CodeAttribute;
                    if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.KeyAttribute"))
                    {
                        SolutionCodeElement sce = new SolutionCodeElement()
                        {
                            Order = currOrder,
                            CodeElementName = ce.Name,
                            CodeElementFullName = ce.FullName,
                            CodeElementRef = ce
                        };
                        result.Add(sce);
                        break;
                    }
                }
            }
            if (result.Count > 0) return result;
            foreach (CodeElement ce in cc.Members)
            {
                if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty cp = ce as CodeProperty;
                if (cp.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (cp.Type == null) continue;
                if (cp.Type.CodeType == null) continue;
                CodeTypeRef ctf = cp.Type;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType) continue;

                if ("Id".Equals( cp.Name, StringComparison.OrdinalIgnoreCase)) {
                    SolutionCodeElement sce = new SolutionCodeElement()
                    {
                        Order = currOrder,
                        CodeElementName = ce.Name,
                        CodeElementFullName = ce.FullName,
                        CodeElementRef = ce
                    };

                    result.Add(sce);
                    return result;
                }
            }
            if (result.Count > 0) return result;
            string[] names = cc.FullName.Split(new char[] { '.' });
            string typeName = names[names.Length - 1];

            string propName = typeName + "Id";
            foreach (CodeElement ce in cc.Members)
            {
                if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                CodeProperty cp = ce as CodeProperty;
                if (cp.Access != vsCMAccess.vsCMAccessPublic) continue;
                if (cp.Type == null) continue;
                if (cp.Type.CodeType == null) continue;
                CodeTypeRef ctf = cp.Type;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                if (ctf.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType) continue;
                if (propName.Equals(cp.Name, StringComparison.OrdinalIgnoreCase))
                {
                    SolutionCodeElement sce = new SolutionCodeElement()
                    {
                        Order = currOrder,
                        CodeElementName = ce.Name,
                        CodeElementFullName = ce.FullName,
                        CodeElementRef = ce
                    };
                    result.Add(sce);
                    return result;
                }
            }
            return result;
        }
        protected void DefineForeignKeyNodes(string childForeignKeyPrefix, string childForeignKeyName, CodeProperty masterCodeProp, IList<PropertySelectorViewModel> list, string detailClassName, int currentNestedLevel)
        {

            CodeClass masterCodeClass = masterCodeProp.Type.CodeType as CodeClass;
            string masterCodePropName = masterCodeProp.Name;
            List<SolutionCodeElement> primKeyProps = GetPrimaryKeyProperties(masterCodeClass);
            // SolutionCodeElement.CodeElementRef holds 'CodeProperty'
            // collect ColumnAttributes to define order
            if (primKeyProps.Count < 1)
            {
                // throw an exception here
                return;
            }


            //////////////////////////////////////////////////
            ///  The 1st case:
            ///  ---------
            ///  public class DetailType {
            ///  
            ///     public int MasterRefId1 { get; set; }
            ///     public int MasterRefId2 { get; set; }
            ///     
            ///     [ForeignKey("MasterRefId1")]
            ///     [ForeignKey("MasterRefId2")]
            ///     public MasterType MasterProp { get; set; }
            ///  }
            //////////////////////////////////////////////////
            foreach (CodeElement cea in masterCodeProp.Attributes)
            {
                string foreignKeyName = "";
                CodeAttribute ca = cea as CodeAttribute;
                if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute"))
                {
                    foreach (CodeElement chld in ca.Children)
                    {
                        if (chld is CodeAttributeArgument)
                        {
                            foreignKeyName = (chld as CodeAttributeArgument).Value;
                        }
                        foreignKeyName = foreignKeyName.Replace("\"", "");
                    }
                    foreach (ClassFiledSelectorViewModel itm in list)
                    {
                        if(foreignKeyName.Equals(itm.OriginalPropertyName))
                        {
                            PropertySelectorViewModel fk = null;
                            if (itm.ForeigKeyParentProperties == null)
                            {
                                itm.ForeigKeyParentProperties = new ObservableCollection<PropertySelectorViewModel>();
                            } else
                            {
                                fk = itm.ForeigKeyPPByForeignKN(masterCodePropName);
                            }
                            if (fk == null)
                            {
                                fk = new PropertySelectorViewModel()
                                {
                                    ForeignKeyName = masterCodePropName
                                };
                                itm.IsForeignKeyField = true;
                                itm.ForeigKeyParentProperties.Add(fk);
                            }
                        }
                    }
                }
            }

            //////////////////////////////////////////////////
            ///  The 2nd case:
            ///  ---------
            ///  public class DetailType {
            ///  
            ///     public int MasterRefId1 { get; set; }
            ///     public int MasterRefId2 { get; set; }
            ///  }
            ///  public class MasterType {
            ///     [ForeignKey("MasterRefId1")]
            ///     [ForeignKey("MasterRefId2")]
            ///     public ICollection<DetailType>  DetailProps { get; set; }
            ///  }
            //////////////////////////////////////////////////

            if (!string.IsNullOrEmpty(detailClassName)) {
                foreach (CodeElement ce in masterCodeClass.Members)
                {
                    if (ce.Kind != vsCMElement.vsCMElementProperty) continue;
                    CodeProperty loopCodeProp = ce as CodeProperty;
                    if (loopCodeProp.Access != vsCMAccess.vsCMAccessPublic) continue;
                    if (loopCodeProp.Type == null) continue;
                    if (loopCodeProp.Type.CodeType == null) continue;
                    bool isNotMapped = false;
                    foreach (CodeElement cea in loopCodeProp.Attributes)
                    {
                        CodeAttribute ca = cea as CodeAttribute;
                        if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute"))
                        {
                            isNotMapped = true;
                            break;
                        }
                    }
                    if (isNotMapped) continue;
                    CodeTypeRef ctRef = loopCodeProp.Type;
                    if (ctRef.TypeKind == vsCMTypeRef.vsCMTypeRefArray) continue;
                    if (ctRef.TypeKind != vsCMTypeRef.vsCMTypeRefCodeType) continue;
                    if (ctRef.CodeType.Kind != vsCMElement.vsCMElementInterface) continue;
                    string className = ctRef.CodeType.FullName.Replace("System.Collections.Generic.ICollection<","").Replace(">","").Trim();
                    if (!detailClassName.Equals(className, StringComparison.OrdinalIgnoreCase)) continue;
                    // look for InversePropertyAttribute
                    string inversePropertyName = "";
                    foreach (CodeElement cea in loopCodeProp.Attributes)
                    {
                        CodeAttribute ca = cea as CodeAttribute;
                        if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute"))
                        {
                            foreach (CodeElement chld in ca.Children)
                            {
                                if (chld is CodeAttributeArgument)
                                {
                                    inversePropertyName = (chld as CodeAttributeArgument).Value;
                                }
                                inversePropertyName = inversePropertyName.Replace("\"", "");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(inversePropertyName))
                    {
                        if (!inversePropertyName.Equals(masterCodePropName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    foreach (CodeElement cea in loopCodeProp.Attributes)
                    {
                        string foreignKeyName = "";
                        CodeAttribute ca = cea as CodeAttribute;
                        if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute"))
                        {
                            foreach (CodeElement chld in ca.Children)
                            {
                                if (chld is CodeAttributeArgument)
                                {
                                    foreignKeyName = (chld as CodeAttributeArgument).Value;
                                }
                                foreignKeyName = foreignKeyName.Replace("\"", "");
                            }
                            foreach (ClassFiledSelectorViewModel itm in list)
                            {
                                if (foreignKeyName.Equals(itm.OriginalPropertyName))
                                {
                                    PropertySelectorViewModel fk = null;
                                    if (itm.ForeigKeyParentProperties == null)
                                    {
                                        itm.ForeigKeyParentProperties = new ObservableCollection<PropertySelectorViewModel>();
                                    }
                                    else
                                    {
                                        fk = itm.ForeigKeyPPByForeignKN(masterCodePropName);
                                    }
                                    if (fk == null)
                                    {
                                        fk = new PropertySelectorViewModel()
                                        {
                                            ForeignKeyName = masterCodePropName
                                        };
                                        itm.ForeigKeyParentProperties.Add(fk);
                                        itm.IsForeignKeyField = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }



            string[] names = masterCodeProp.Type.AsFullName.Split(new char[] { '.' });
            string masterTypeName = names[names.Length - 1];
            //masterCodePropName
            foreach(SolutionCodeElement primKeyProp in primKeyProps)
            {
                string primKeyPropName = primKeyProp.CodeElementName;
                if ("Id".Equals(primKeyPropName,StringComparison.OrdinalIgnoreCase))
                {
                    primKeyPropName = masterTypeName + primKeyPropName;
                }
                string fkNm = masterCodePropName + primKeyPropName;
                if (DefineForeigKeyNodeByForeigKeyFiledName(list, fkNm, masterCodePropName)) continue;
                fkNm = masterTypeName + primKeyPropName;
                if (DefineForeigKeyNodeByForeigKeyFiledName(list, fkNm, masterCodePropName)) continue;
                DefineForeigKeyNodeByForeigKeyFiledName(list, primKeyPropName, masterCodePropName);
            }
            // collect all foreign key fields for the given navigation property: masterCodePropName
            List<ClassFiledSelectorViewModel> fcflds = new List<ClassFiledSelectorViewModel>();
            foreach(ClassFiledSelectorViewModel itm in list)
            {
                if (itm.ForeigKeyParentProperties == null) continue;
                if (itm.ForeigKeyPPByForeignKN(masterCodePropName) == null) continue;
                fcflds.Add(itm);
            }
            if (fcflds.Count > 1)
            {
                fcflds.Sort((x, y) => x.FieldOrder - y.FieldOrder);
            }
            if (primKeyProps.Count > 1)
            {
                foreach(SolutionCodeElement sce in primKeyProps)
                {
                    CodeProperty cp = sce.CodeElementRef as CodeProperty;
                    foreach (CodeElement cea in cp.Attributes)
                    {
                        bool OrderIsFound = false;
                        CodeAttribute ca = cea as CodeAttribute;
                        if (ca.FullName.Contains("System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"))
                        {
                            foreach (CodeElement chld in ca.Children)
                            {
                                if ("Order".Equals(chld.Name, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    if (chld is CodeAttributeArgument)
                                    {
                                        int val;
                                        if (int.TryParse((chld as CodeAttributeArgument).Value, out val))
                                        {
                                            sce.Order = val;
                                            OrderIsFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (OrderIsFound) break;
                    }
                }
                primKeyProps.Sort((x, y) => x.Order - y.Order);
            }
            int Count = primKeyProps.Count;
            if (Count > fcflds.Count) Count = fcflds.Count;
            for(int i = 0; i < Count; i++)
            {
                ClassFiledSelectorViewModel cfsvm = fcflds[i];
                PropertySelectorViewModel psvm = cfsvm.ForeigKeyPPByForeignKN(masterCodePropName);
                psvm.OriginalPropertyName = primKeyProps[i].CodeElementName;
                CodeProperty cp = primKeyProps[i].CodeElementRef as CodeProperty;
                psvm.TypeFullName = cp.Type.AsFullName;
                psvm.UnderlyingTypeName = cp.Type.AsFullName;
                psvm.TypeIsNullable = false;
                psvm.PocoName = masterCodeClass.Name;
                psvm.PocoFullName = masterCodeClass.FullName;
                if (currentNestedLevel+1 <= this.MaxNestedLevel)
                {
                    psvm.ForeigKeyParentProperties = new ObservableCollection<PropertySelectorViewModel>();
                    DoPrepareClassFiledSelectorData(psvm.ChildForeignKeyPrefix, psvm.ForeignKeyName, psvm.ForeigKeyParentProperties, masterCodeClass as CodeElement, primKeyProps, currentNestedLevel);
                    
                }
            }
        }
        protected bool DefineForeigKeyNodeByForeigKeyFiledName(IList<PropertySelectorViewModel> list, string fkNm, string masterCodePropName)
        {
            foreach (ClassFiledSelectorViewModel itm in list)
            {
                if (fkNm.Equals(itm.OriginalPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    PropertySelectorViewModel fk = null;
                    if (itm.ForeigKeyParentProperties == null)
                    {
                        itm.ForeigKeyParentProperties = new ObservableCollection<PropertySelectorViewModel>();
                    }
                    else
                    {
                        fk = itm.ForeigKeyPPByForeignKN(masterCodePropName);
                    }
                    if (fk == null)
                    {
                        fk = new PropertySelectorViewModel()
                        {
                            ForeignKeyName = masterCodePropName
                        };
                        itm.ForeigKeyParentProperties.Add(fk);
                        itm.IsForeignKeyField = true;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
