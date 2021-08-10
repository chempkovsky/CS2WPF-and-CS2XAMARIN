using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace CS2WPF.Helpers
{
    static class CustomPluralization
    {
        public static PluralizationService pluralizationservice = PluralizationService.CreateService(new CultureInfo("en-us"));

        public static string Pluralize(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (pluralizationservice.IsPlural(str)) return str;
            return pluralizationservice.Pluralize(str);
        }
        public static string Singularize(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (pluralizationservice.IsSingular(str)) return str;
            return pluralizationservice.Singularize(str);
        }
        public static bool IsSingular(string str)
        {
            return pluralizationservice.IsSingular(str);
        }
        public static bool IsPlural(string str)
        {
            return pluralizationservice.IsPlural(str);
        }
    }
}
