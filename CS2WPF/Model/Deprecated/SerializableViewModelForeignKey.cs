using System;

namespace CS2ANGULAR.Model
{
    [Serializable]
    public class SerializableViewModelForeignKey
    {
        public string   ForeignKeyName { get; set; }
        public string   MasterOriginalPropertyName { get; set; }
        public string   MasterPocoName { get; set; }
        public string   MasterPocoFullName { get; set; }
        public string   MasterTypeFullName { get; set; }
        public string   MasterUnderlyingTypeName { get; set; }
        public bool     MasterTypeIsNullable { get; set; }
        public bool     RefTypeIsNullable { get; set; }

        public string   DetailOriginalPropertyName { get; set; }
        public bool     DetailIsNullable { get; set; }
        public string   ForeignKeyNameChain { get; set; }
        public string   DetailTypeFullName { get; set; }
        public string   DetailUnderlyingTypeName { get; set; }
    }
}
