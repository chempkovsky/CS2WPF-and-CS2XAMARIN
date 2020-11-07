
namespace CS2ANGULAR.Model
{
    public class LookUpClassField
    {
        public string OriginalFieldName { get; set; }
        public string ViewModelFieldName { get; set; }
        public string JsonPropertyFiledName { get; set; }
        public bool IsKeyField { get; set; }
        public int FieldOrder { get; set; }
        public bool IsForeignKeyField { get; set; }
        public string ForeignKeyName { get; set; }
        public string ForeignKeyAlias { get; set; }
        public string ChildForeignKeyPrefix { get; set; }
        public string ForeignKeyPrefix { get; set; }
        public ViewModelForeigKeyUITypeEnum ForeignKeyUIType { get; set; }
        public bool IsUIHidden { get; set; }
    }
}
