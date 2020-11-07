using System;
using System.Collections.Generic;

namespace CS2ANGULAR.Model
{
    [Serializable]
    public class SerializableViewModelProperty
    {
        public string   OriginalPropertyName { get; set; }
        public string   ViewModelFieldName { get; set; }
        public string   JsonPropertyFieldName { get; set; }
        public string   PocoName { get; set; }
        public string   PocoFullName { get; set; }
        public string   PropTypeFullName { get; set; }
        public string   PropUnderlyingTypeName { get; set; }
        public bool     PropTypeIsNullable { get; set; }
        public bool     RefTypeIsNullable { get; set; }
        public bool     PropIsUIHidden { get; set; }
        public bool     PropIsKey { get; set; }
        public string   ForeignKeyName { get; set; }
        public string   ForeignKeyNameChain { get; set; }
        public bool     PropIsForeignKey { get; set; }
        public List<SerializableViewModelForeignKey> Navigations { get; set; }
        public ViewModelForeigKeyUITypeEnum ForeignKeyUIType { get; set; }
        public string   LookUpViewName { get; set; }
        public string   LookUpFieldName { get; set; }
        public int      LookUpId { get; set; }

    }
}
