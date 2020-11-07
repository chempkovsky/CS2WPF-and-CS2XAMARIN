
using CS2ANGULAR.Helpers;
using System.ComponentModel;

namespace CS2ANGULAR.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ViewModelForeigKeyUITypeEnum: int
    {
        [Description("Default")]
        Default=0,
        [Description("By ComboBox")]
        ByComboBox=1,
        [Description("By LookUp dialog")]
        ByLookUpBox=2,
        [Description("By Hand")]
        ByHand=3
    }
}
