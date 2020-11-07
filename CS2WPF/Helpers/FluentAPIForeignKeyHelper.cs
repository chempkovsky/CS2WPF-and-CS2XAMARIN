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
                if(!string.IsNullOrEmpty(foreignKey.ErrorsText)) foreignKey.ErrorsText += "\r\n";
                foreignKey.ErrorsText += "Primary Key Props is not set.";

            } else
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
                if(foreignKey.ForeignKeyProps.Count != foreignKey.PrincipalKeyProps.Count)
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
    }
}
