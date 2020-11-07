using CS2ANGULAR.Model;
using CS2ANGULAR.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CS2ANGULAR.Helpers
{
    public static class SerializableViewModelHelper
    {
        public static SerializableViewModel AssignFromRoot(this SerializableViewModel dest, ClassFiledSelectorRootViewModel src)
        {
            if (src == null) return dest;

            dest.ViewModelName = src.OutputClassName;
            dest.ViewModelNameSpace = src.DestinationNameSpace;
            dest.ViewModelProjectName = src.DestinationProjectName;
            dest.ViewModelFolderChain = src.DestinationFoldersChain;
            dest.RootNodeClassName = src.RootNodeClassName;
            dest.RootNodeNameSapce = src.RootNodeNameSapce;
            dest.RootNodeProjectName = src.RootNodeProjectName;
            dest.GenerateJSonAttribute = src.GenerateJSonAttribute;

            foreach (ClassFiledSelectorViewModel srcProp in src.ForeigKeyParentProperties)
            {
                if(!srcProp.TypeIsNullable)
                {
                    if (
                        (!"System.String".Equals(srcProp.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase)) 
                        ||
                        srcProp.HasRequiredAttribute
                       )
                    {
                        if (string.IsNullOrEmpty(dest.RequiredRootFields))
                        {
                            dest.RequiredRootFields = srcProp.OriginalPropertyName;
                        }
                        else
                        {
                            dest.RequiredRootFields = dest.RequiredRootFields + ";" + srcProp.OriginalPropertyName;
                        }
                    }
                }
                if (!srcProp.IsKeyField) continue;
                if(dest.PrimKeys == null)
                {
                    dest.PrimKeys = new List<SerializableViewModelProperty>();
                }
                SerializableViewModelProperty destPrimKey = new SerializableViewModelProperty()
                {
                    OriginalPropertyName = srcProp.OriginalPropertyName,
                    ViewModelFieldName = srcProp.ViewModelFieldName,
                    JsonPropertyFieldName = srcProp.JsonPropertyFieldName,
                    PocoName = srcProp.PocoName,
                    PocoFullName = srcProp.PocoFullName,
                    PropTypeFullName = srcProp.TypeFullName,
                    PropUnderlyingTypeName = srcProp.UnderlyingTypeName,
                    PropTypeIsNullable = srcProp.TypeIsNullable,
                    PropIsUIHidden = srcProp.IsUIHidden,
                    PropIsKey = srcProp.IsKeyField,
                    ForeignKeyName = "", //srcProp.ForeignKeyName,
                    ForeignKeyNameChain = "",
                    PropIsForeignKey = srcProp.IsForeignKeyField,
                    ForeignKeyUIType = srcProp.ForeignKeyUIType,
                    LookUpViewName = srcProp.LookUpViewName,
                    LookUpFieldName = srcProp.LookUpFieldName,
                    LookUpId = srcProp.LookUpId,
                };
                dest.PrimKeys.Add(destPrimKey);
                if (srcProp.ForeigKeyParentProperties == null) continue;
                foreach(PropertySelectorViewModel srcFkp in srcProp.ForeigKeyParentProperties)
                {
                    if(destPrimKey.Navigations == null)
                    {
                        destPrimKey.Navigations = new List<SerializableViewModelForeignKey>();
                    }
                    SerializableViewModelForeignKey dstFkp = new SerializableViewModelForeignKey()
                    {
                        ForeignKeyName = srcFkp.ForeignKeyName,
                        MasterOriginalPropertyName = srcFkp.OriginalPropertyName,
                        MasterPocoName = srcFkp.PocoName,
                        MasterPocoFullName = srcFkp.PocoFullName,
                        MasterTypeFullName = srcFkp.TypeFullName,
                        MasterUnderlyingTypeName = srcFkp.UnderlyingTypeName,
                        MasterTypeIsNullable = srcFkp.TypeIsNullable
                    };
                    destPrimKey.Navigations.Add(dstFkp);
                }
            }
            foreach (ClassFiledSelectorViewModel srcProp in src.ForeigKeyParentProperties)
            {
                SerializableViewModelProperty destProp = null;
                if (srcProp.IncludeInView)
                {
                    if (dest.Properties == null)
                    {
                        dest.Properties = new List<SerializableViewModelProperty>();
                    }
                    destProp = new SerializableViewModelProperty()
                    {
                        OriginalPropertyName = srcProp.OriginalPropertyName,
                        ViewModelFieldName = srcProp.ViewModelFieldName,
                        JsonPropertyFieldName = srcProp.JsonPropertyFieldName,
                        PocoName = srcProp.PocoName,
                        PocoFullName = srcProp.PocoFullName,
                        PropTypeFullName = srcProp.TypeFullName,
                        PropUnderlyingTypeName = srcProp.UnderlyingTypeName,
                        PropTypeIsNullable = srcProp.TypeIsNullable,
                        RefTypeIsNullable = false,
                        PropIsUIHidden = srcProp.IsUIHidden,
                        PropIsKey = srcProp.IsKeyField,
                        ForeignKeyName = "", //srcProp.ForeignKeyName,
                        ForeignKeyNameChain = "",
                        PropIsForeignKey = srcProp.IsForeignKeyField,
                        ForeignKeyUIType = srcProp.ForeignKeyUIType,
                        LookUpViewName = srcProp.LookUpViewName,
                        LookUpFieldName = srcProp.LookUpFieldName,
                        LookUpId = srcProp.LookUpId,
                    };
                    dest.Properties.Add(destProp);
                }
                if (srcProp.ForeigKeyParentProperties == null) continue;
                if (srcProp.IncludeInView)
                {
                    foreach (PropertySelectorViewModel srcFkp in srcProp.ForeigKeyParentProperties)
                    {
                        if (destProp.Navigations == null)
                        {
                            destProp.Navigations = new List<SerializableViewModelForeignKey>();
                        }
                        SerializableViewModelForeignKey dstFkp = new SerializableViewModelForeignKey()
                        {
                            ForeignKeyName = srcFkp.ForeignKeyName,
                            MasterOriginalPropertyName = srcFkp.OriginalPropertyName,
                            MasterPocoName = srcFkp.PocoName,
                            MasterPocoFullName = srcFkp.PocoFullName,
                            MasterTypeFullName = srcFkp.TypeFullName,
                            MasterUnderlyingTypeName = srcFkp.UnderlyingTypeName,
                            MasterTypeIsNullable = srcFkp.TypeIsNullable,
                            RefTypeIsNullable = srcProp.TypeIsNullable
                        };
                        destProp.Navigations.Add(dstFkp);
                    }
                }
                dest.AssignFromNavigation(srcProp.TypeIsNullable, "", srcProp, srcProp.ForeigKeyParentProperties);
            }
            return dest;
        }
        public static bool AssignFromNavigation(this SerializableViewModel dest, bool RefTypeIsNullable, string ForeignKeyNameChain, ClassFiledSelectorViewModel detailField, IList<PropertySelectorViewModel> ForeigKeyParentProps)
        {
            bool result = false;
            if (ForeigKeyParentProps == null) return result;
            foreach(PropertySelectorViewModel srcFkPrps in ForeigKeyParentProps)
            {
                bool loopResult = false;
                if (srcFkPrps.ForeigKeyParentProperties == null) continue;
                string currFk = "";
                if (string.IsNullOrEmpty(ForeignKeyNameChain))
                {
                    currFk = srcFkPrps.ForeignKeyName;
                } else
                {
                    currFk = ForeignKeyNameChain + "." + srcFkPrps.ForeignKeyName;
                }
                foreach (ClassFiledSelectorViewModel srcProp in srcFkPrps.ForeigKeyParentProperties)
                {
                    
                    
                    SerializableViewModelProperty destProp = null;
                    if (srcProp.IncludeInView)
                    {
                        if (dest.Properties == null)
                        {
                            dest.Properties = new List<SerializableViewModelProperty>();
                        }
                        destProp = new SerializableViewModelProperty()
                        {
                            OriginalPropertyName = srcProp.OriginalPropertyName,
                            ViewModelFieldName = srcProp.ViewModelFieldName,
                            JsonPropertyFieldName = srcProp.JsonPropertyFieldName,
                            PocoName = srcProp.PocoName,
                            PocoFullName = srcProp.PocoFullName,
                            PropTypeFullName = srcProp.TypeFullName,
                            PropUnderlyingTypeName = srcProp.UnderlyingTypeName,
                            PropTypeIsNullable = srcProp.TypeIsNullable,
                            RefTypeIsNullable = RefTypeIsNullable,
                            PropIsUIHidden = srcProp.IsUIHidden,
                            PropIsKey = srcProp.IsKeyField,
                            ForeignKeyName = srcFkPrps.ForeignKeyName, 
                            ForeignKeyNameChain = currFk,
                            PropIsForeignKey = srcProp.IsForeignKeyField,
                            ForeignKeyUIType = srcProp.ForeignKeyUIType,
                            LookUpViewName = srcProp.LookUpViewName,
                            LookUpFieldName = srcProp.LookUpFieldName,
                            LookUpId = srcProp.LookUpId,
                        };
                        dest.Properties.Add(destProp);
                        loopResult = true;
                    }
                    if (srcProp.IncludeInView)
                    {
                        if (srcProp.ForeigKeyParentProperties != null)
                        {
                            foreach (PropertySelectorViewModel srcFkp in srcProp.ForeigKeyParentProperties)
                            {
                                if (destProp.Navigations == null)
                                {
                                    destProp.Navigations = new List<SerializableViewModelForeignKey>();
                                }
                                SerializableViewModelForeignKey dstFkp = new SerializableViewModelForeignKey()
                                {
                                    ForeignKeyName = srcFkp.ForeignKeyName,
                                    MasterOriginalPropertyName = srcFkp.OriginalPropertyName,
                                    MasterPocoName = srcFkp.PocoName,
                                    MasterPocoFullName = srcFkp.PocoFullName,
                                    MasterTypeFullName = srcFkp.TypeFullName,
                                    MasterUnderlyingTypeName = srcFkp.UnderlyingTypeName,
                                    MasterTypeIsNullable = srcFkp.TypeIsNullable,
                                    RefTypeIsNullable = srcProp.TypeIsNullable || RefTypeIsNullable,

                                    ForeignKeyNameChain = ForeignKeyNameChain,
                                    DetailOriginalPropertyName = srcProp.OriginalPropertyName,
                                    DetailIsNullable = srcProp.TypeIsNullable,
                                    DetailTypeFullName = srcProp.TypeFullName,
                                    DetailUnderlyingTypeName = srcProp.UnderlyingTypeName,
                                };
                                destProp.Navigations.Add(dstFkp);
                            }
                        }
                    }
                    loopResult = loopResult || dest.AssignFromNavigation(srcProp.TypeIsNullable || RefTypeIsNullable, currFk, srcProp, srcProp.ForeigKeyParentProperties);
                }
                if (loopResult)
                {
                    result = true;
                    if (dest.ForeignKeys == null)
                    {
                        dest.ForeignKeys = new List<SerializableViewModelForeignKey>();
                    } else
                    {
                        if (string.IsNullOrEmpty(ForeignKeyNameChain)) {
                            if (dest.ForeignKeys.Exists(
                                                    fk => string.IsNullOrEmpty(fk.ForeignKeyNameChain) &&
                                                    string.Equals(srcFkPrps.ForeignKeyName, fk.ForeignKeyName) &&
                                                    string.Equals(detailField.OriginalPropertyName, fk.DetailOriginalPropertyName)
                                                  )) continue;
                        } 
                        else
                        {
                            if (dest.ForeignKeys.Exists(
                                                    fk => string.Equals(fk.ForeignKeyNameChain, ForeignKeyNameChain) &&
                                                    string.Equals(srcFkPrps.ForeignKeyName, fk.ForeignKeyName) &&
                                                    string.Equals(detailField.OriginalPropertyName, fk.DetailOriginalPropertyName)
                                                  )) continue;
                        }
                    }
                    SerializableViewModelForeignKey dstFkp = new SerializableViewModelForeignKey()
                    {
                        ForeignKeyName = srcFkPrps.ForeignKeyName,
                        MasterOriginalPropertyName = srcFkPrps.OriginalPropertyName,
                        MasterPocoName = srcFkPrps.PocoName,
                        MasterPocoFullName = srcFkPrps.PocoFullName,
                        MasterTypeFullName = srcFkPrps.TypeFullName,
                        MasterUnderlyingTypeName = srcFkPrps.UnderlyingTypeName,
                        MasterTypeIsNullable = srcFkPrps.TypeIsNullable,
                        RefTypeIsNullable = srcFkPrps.TypeIsNullable || RefTypeIsNullable,

                        ForeignKeyNameChain = ForeignKeyNameChain,
                        DetailOriginalPropertyName = detailField.OriginalPropertyName,
                        DetailIsNullable = detailField.TypeIsNullable,
                        DetailTypeFullName = detailField.TypeFullName,
                        DetailUnderlyingTypeName = detailField.UnderlyingTypeName
                    };
                    dest.ForeignKeys.Add(dstFkp);
                    dest.ProcessCompositeForeigKey(RefTypeIsNullable, dstFkp, ForeigKeyParentProps);
                }
            }
            return result;
        }
        public static void ProcessCompositeForeigKey(this SerializableViewModel dest, bool RefTypeIsNullable, SerializableViewModelForeignKey destFk,  IList<PropertySelectorViewModel> ForeigKeyParentProperties)
        {
            if (dest == null) return;
            if(dest.ForeignKeys == null) return;
            if (destFk == null) return;
            if (ForeigKeyParentProperties == null) return;
            List<SerializableViewModelForeignKey> locFrKs = new List<SerializableViewModelForeignKey>();
            bool locRefTypeIsNullable = false;

            string ForeignKeyNameChain = destFk.ForeignKeyNameChain;
            string ForeignKeyName = destFk.ForeignKeyName;
            if (string.IsNullOrEmpty(ForeignKeyName)) return;
            foreach (ClassFiledSelectorViewModel srcProp in ForeigKeyParentProperties)
            {
                if (srcProp.ForeigKeyParentProperties == null) continue;
                if (srcProp.ForeigKeyParentProperties.Count < 1) continue;
                PropertySelectorViewModel srcFkPrps = srcProp.ForeigKeyPPByForeignKN(ForeignKeyName);
                if (srcFkPrps == null) continue;
                SerializableViewModelForeignKey svmFk = null;
                if (string.IsNullOrEmpty(ForeignKeyNameChain))
                {
                    svmFk = 
                    dest.ForeignKeys.Where(
                                            fk => string.IsNullOrEmpty(fk.ForeignKeyNameChain) &&
                                            string.Equals(ForeignKeyName, fk.ForeignKeyName) &&
                                            string.Equals(srcFkPrps.OriginalPropertyName, fk.DetailOriginalPropertyName)
                                          ).FirstOrDefault();
                }
                else
                {
                    svmFk =
                        dest.ForeignKeys.Where(
                                            fk => string.Equals(fk.ForeignKeyNameChain, ForeignKeyNameChain) &&
                                            string.Equals(ForeignKeyName, fk.ForeignKeyName) &&
                                            string.Equals(srcFkPrps.OriginalPropertyName, fk.DetailOriginalPropertyName)
                                          ).FirstOrDefault();
                }
                if (svmFk != null)
                {
                    locRefTypeIsNullable  = locRefTypeIsNullable || svmFk.RefTypeIsNullable;
                    locFrKs.Add(svmFk);
                    continue;
                }

                SerializableViewModelForeignKey dstFkp = new SerializableViewModelForeignKey()
                {
                    ForeignKeyName = srcFkPrps.ForeignKeyName,
                    MasterOriginalPropertyName = srcFkPrps.OriginalPropertyName,
                    MasterPocoName = srcFkPrps.PocoName,
                    MasterPocoFullName = srcFkPrps.PocoFullName,
                    MasterTypeFullName = srcFkPrps.TypeFullName,
                    MasterUnderlyingTypeName = srcFkPrps.UnderlyingTypeName,
                    MasterTypeIsNullable = srcFkPrps.TypeIsNullable,
                    RefTypeIsNullable = srcFkPrps.TypeIsNullable || RefTypeIsNullable,

                    ForeignKeyNameChain = ForeignKeyNameChain,
                    DetailOriginalPropertyName = srcProp.OriginalPropertyName,
                    DetailIsNullable = srcProp.TypeIsNullable,
                    DetailTypeFullName = srcProp.TypeFullName,
                    DetailUnderlyingTypeName = srcProp.UnderlyingTypeName
                };
                dest.ForeignKeys.Add(dstFkp);
                locFrKs.Add(dstFkp);
                locRefTypeIsNullable  = locRefTypeIsNullable || dstFkp.RefTypeIsNullable;
            }
            foreach (SerializableViewModelForeignKey srcProp in locFrKs)
            {
                srcProp.RefTypeIsNullable = locRefTypeIsNullable;
            }
        }



        public static bool HasAllPrimaryKeyProperties(this SerializableViewModel dest)
        {
            bool result = false;
            if(dest == null) return result;
            if((dest.PrimKeys == null) || (dest.Properties == null)) return result;
            if ((dest.PrimKeys.Count < 1) || (dest.Properties.Count < 1))  return result;
            foreach(SerializableViewModelProperty pk in dest.PrimKeys)
            {
                if (dest.Properties.Where(p => p.OriginalPropertyName == pk.OriginalPropertyName).FirstOrDefault() == null) return result;

            }
            return true;
        }
        public static bool HasProperties(this SerializableViewModel dest)
        {
            bool result = false;
            if (dest == null) return result;
            if (dest.Properties == null) return result;
            if (dest.Properties.Count < 1) return result;
            return true;
        }
        public static bool HasAllRequiredProperties(this SerializableViewModel dest)
        {
            bool result = false;
            if (dest == null) return result;
            if (dest.Properties == null) return result;
            if (dest.Properties.Count < 1) return result;
            if (string.IsNullOrEmpty( dest.RequiredRootFields )) return true;
            string[] props = dest.RequiredRootFields.Split(new char[] { ';' });
            foreach(string prop in props)
            {
                if (string.IsNullOrEmpty(prop)) continue;
                if(!dest.Properties.Exists(p=> string.IsNullOrEmpty(p.ForeignKeyNameChain) && prop.Equals( p.OriginalPropertyName)))
                {
                    return result;
                }
            }
            return true;
        }
    }
}
