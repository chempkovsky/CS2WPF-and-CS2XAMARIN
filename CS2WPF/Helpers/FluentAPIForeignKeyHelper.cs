using CS2WPF.Model;
using CS2WPF.Model.AnalyzeOnModelCreating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.Helpers
{
    public static class FluentAPIForeignKeyHelper
    {
        public static FluentAPIForeignKey DefineErrorFlag(this FluentAPIForeignKey foreignKey)
        {
            if (foreignKey == null) return foreignKey;
            foreignKey.HasErrors = false;
            if (foreignKey.ForeignKeyProps == null)
            {
                foreignKey.HasErrors = true;
                if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                foreignKey.ErrorsText += "Foreign Key Props is not set.";
            }
            else
            {
                if (foreignKey.ForeignKeyProps.Count < 1)
                {
                    foreignKey.HasErrors = true;
                    if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                    foreignKey.ErrorsText += "Foreign Key Props is not set.";
                }
            }
            if (foreignKey.PrincipalKeyProps == null)
            {
                foreignKey.HasErrors = true;
                if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                foreignKey.ErrorsText += "Primary Key Props is not set.";

            }
            else
            {
                if (foreignKey.PrincipalKeyProps.Count < 1)
                {
                    foreignKey.HasErrors = true;
                    if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                    foreignKey.ErrorsText += "Primary Key Props is not set.";
                }
                else
                {
                    foreignKey.ErrorsText += "";
                }
            }
            if ((foreignKey.ForeignKeyProps != null) && (foreignKey.PrincipalKeyProps != null))
            {
                if (foreignKey.ForeignKeyProps.Count != foreignKey.PrincipalKeyProps.Count)
                {
                    foreignKey.HasErrors = true;
                    if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                    foreignKey.ErrorsText += "Primary Key Props Count \r\nis not equal to \r\nForeign Key Props Count.";
                }
            }
            if (string.IsNullOrEmpty(foreignKey.InverseNavigationName))
            {
                foreignKey.HasErrors = true;
                if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                foreignKey.ErrorsText += "Inverse Navigation is not defined";

            }

            if (foreignKey.NavigationType == NavigationTypeEnum.Unckown)
            {
                foreignKey.HasErrors = true;
                if (!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                foreignKey.ErrorsText += "Navigation Type is not defined";
            }

            return foreignKey;
        }
        public static bool IsTheListOfNamesIdentical(this FluentAPIKey fapks, ICollection<FluentAPIProperty> keyProperties)
        {
            bool result = false;
            if ((fapks == null) || (keyProperties == null)) return result;
            if (fapks.KeyProperties == null) return result;
            if (fapks.KeyProperties.Count != keyProperties.Count) return result;
            int i = 0;
            foreach (FluentAPIProperty property in keyProperties)
            {
                if (!string.Equals(property.PropName, fapks.KeyProperties[i].PropName)) return result;
                i++;
            }
            return true;
        }
        public static bool IsTheListOfNamesIdentical(this FluentAPIKey fapks, ICollection<string> keyProperties)
        {
            bool result = false;
            if ((fapks == null) || (keyProperties == null)) return result;
            if (fapks.KeyProperties == null) return result;
            if (fapks.KeyProperties.Count != keyProperties.Count) return result;
            int i = 0;
            foreach (string key in keyProperties)
            {
                if (!string.Equals(key, fapks.KeyProperties[i].PropName)) return result;
                i++;
            }
            return true;
        }
        public static FluentAPIKey GetFluentAPIKeyWithIdenticalListOfNames(this ICollection<FluentAPIKey> fapks, ICollection<FluentAPIProperty> keyProperties)
        {
            if ((fapks == null) || (keyProperties == null)) return null;
            foreach (FluentAPIKey r in fapks)
            {
                if (r.IsTheListOfNamesIdentical(keyProperties)) return r;
            }
            return null;
        }
        public static FluentAPIKey GetFluentAPIKeyWithIdenticalListOfNames(this ICollection<FluentAPIKey> fapks, ICollection<string> keyProperties)
        {
            if ((fapks == null) || (keyProperties == null)) return null;
            foreach (FluentAPIKey r in fapks)
            {
                if (r.IsTheListOfNamesIdentical(keyProperties)) return r;
            }
            return null;
        }
    }
}
