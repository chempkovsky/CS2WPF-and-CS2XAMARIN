using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CS2WPF.Helpers
{
    #pragma warning disable VSTHRD010
    public static class PrismModuleAnalyzerHelper
    {
        public static Project ProjectByName(this DTE2 Dte, string projectName)
        {
            // Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((Dte == null) || string.IsNullOrEmpty(projectName)) return null;
            foreach (Project p in Dte.Solution.Projects)
            {
                if (string.Compare(p.Kind, ProjectKinds.vsProjectKindSolutionFolder) == 0)
                {
                    Project r = p.NestedProjectByName(projectName);
                    if (r != null) return r;
                }
                else
                {
                    if (string.Compare(projectName, p.UniqueName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return p;
                    }
                }
            }
            return null;
        }
        public static Project NestedProjectByName(this Project parentPrj, string projectName)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((parentPrj == null) || string.IsNullOrEmpty(projectName)) return null;
            if (string.Compare(parentPrj.Kind, ProjectKinds.vsProjectKindSolutionFolder) != 0)
            {
                return null;
            }
            for (var i = 1; i <= parentPrj.ProjectItems.Count; i++)
            {
                Project subProject = parentPrj.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    Project r = subProject.NestedProjectByName(projectName);
                    if (r != null) return r;
                }
                else
                {
                    if (string.Compare(projectName, subProject.UniqueName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return subProject;
                    }
                }
            }
            return null;
        }
        public static CodeClass ProjectClassByImplementedInterfaceName(this Project prj, string interfaceName)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((prj == null) || string.IsNullOrEmpty(interfaceName)) return null;
            if (prj.CodeModel == null) return null;
            foreach (EnvDTE.CodeElement ce in prj.CodeModel.CodeElements)
            {
                if (ce.Kind == EnvDTE.vsCMElement.vsCMElementClass)
                {
                    if (ce.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
                    {
                        foreach (EnvDTE.CodeElement cei in ((EnvDTE.CodeClass)ce).ImplementedInterfaces)
                        {
                            if (string.Compare(cei.FullName, interfaceName) == 0)
                            {
                                return (CodeClass)ce;
                            }
                        }
                    }
                }
                else
                {
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        EnvDTE.CodeClass cc = ((EnvDTE.CodeNamespace)ce).NamespaceClassByImplementedInterfaceName(interfaceName);
                        if (cc != null) return cc;
                    }
                }
            }
            return null;
        }
        public static CodeClass NamespaceClassByImplementedInterfaceName(this CodeNamespace parentCodeNamespace, string interfaceName)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((parentCodeNamespace == null) || string.IsNullOrEmpty(interfaceName)) return null;
            foreach (EnvDTE.CodeElement ce in parentCodeNamespace.Members)
            {
                if (ce.Kind == EnvDTE.vsCMElement.vsCMElementClass)
                {
                    if (ce.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
                    {
                        foreach (EnvDTE.CodeElement cei in ((EnvDTE.CodeClass)ce).ImplementedInterfaces)
                        {
                            if (string.Compare(cei.FullName, interfaceName) == 0)
                            {
                                return (CodeClass)ce;
                            }
                        }
                    }
                }
                else
                {
                    if (ce.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
                    {
                        CodeClass cc = ((EnvDTE.CodeNamespace)ce).NamespaceClassByImplementedInterfaceName(interfaceName);
                        if (cc != null) return cc;
                    }
                }
            }
            return null;
        }
        public static CodeFunction CodeClassMethodByName(this CodeClass codeClass, string methodName, vsCMAccess methodAccess, string[] methodPatamTypes)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((codeClass == null) || string.IsNullOrEmpty(methodName) || (methodPatamTypes == null)) return null;
            int paramcount = methodPatamTypes.Length;
            foreach (CodeElement codeElement in codeClass.Children)
            {
                if (codeElement == null) continue;
                if (codeElement.Kind != vsCMElement.vsCMElementFunction) continue;
                CodeFunction codeFunction = codeElement as CodeFunction;
                if (codeFunction == null) continue;
                if (codeFunction.Access != methodAccess) continue;
                if ((codeFunction.Parameters == null) && (paramcount == 0))
                {
                    return codeFunction;
                }
                if ((codeFunction.Parameters == null) && (paramcount > 0)) continue;
                if (codeFunction.Parameters.Count != paramcount) continue;
                int i = 0;
                bool paramsIdetical = true;
                foreach (CodeElement prm in codeFunction.Parameters)
                {
                    if (prm.Kind != vsCMElement.vsCMElementParameter)
                    {
                        paramsIdetical = false;
                        break;
                    }
                    CodeParameter cprm = prm as CodeParameter;
                    if (cprm == null)
                    {
                        paramsIdetical = false;
                        break;
                    }
                    CodeTypeRef cprmType = cprm.Type;
                    if (cprmType == null)
                    {
                        paramsIdetical = false;
                        break;
                    }
                    if (string.Compare(methodPatamTypes[i], cprmType.AsFullName) != 0)
                    {
                        paramsIdetical = false;
                        break;
                    }
                    i++;
                }
                if (paramsIdetical)
                {
                    return codeFunction;
                }
            }
            return null;
        }
        public static string GetVariableOfType(this CodeFunction codeFunction, string typeName)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((codeFunction == null) || string.IsNullOrEmpty(typeName)) return null;
            string buff = codeFunction.StartPoint.CreateEditPoint().GetText(codeFunction.EndPoint);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(buff);
            SyntaxNode root = tree.GetRoot();
            if (root == null)
            {
                return null;
            }
            GlobalStatementSyntax gss = null;
            LocalFunctionStatementSyntax lfss = null;
            foreach (SyntaxNode nd in root.ChildNodes())
            {
                if (nd.IsKind(SyntaxKind.GlobalStatement))
                {
                    gss = nd as GlobalStatementSyntax;
                    break;
                }
                if (nd.IsKind(SyntaxKind.LocalFunctionStatement))
                {
                    lfss = nd as LocalFunctionStatementSyntax;
                    break;
                }
            }
            if ((lfss == null) && (gss != null))
            {
                foreach (SyntaxNode nd in gss.ChildNodes())
                {
                    SyntaxKind k = nd.Kind();
                    if (nd.IsKind(SyntaxKind.LocalFunctionStatement))
                    {
                        lfss = nd as LocalFunctionStatementSyntax;
                        break;
                    }
                }
            }
            if (lfss == null) return null;
            if (lfss.Body == null) return null;

            foreach (StatementSyntax ss in lfss.Body.Statements)
            {
                SyntaxKind k = ss.Kind();
                if (k != SyntaxKind.LocalDeclarationStatement) continue;
                LocalDeclarationStatementSyntax lds = ss as LocalDeclarationStatementSyntax;
                if (lds == null) continue;
                VariableDeclarationSyntax vds = lds.Declaration as VariableDeclarationSyntax;
                if (vds == null) continue;
                if (vds.Type == null) continue;
                if (!IsEqualClassNames(vds.Type.ToString(), typeName)) continue;
                if (vds.Variables == null) continue;
                foreach (VariableDeclaratorSyntax vdsItm in vds.Variables)
                {
                    if (vdsItm.Identifier == null) continue;
                    return vdsItm.Identifier.ValueText;
                }
            }
            return null;
        }
        public static bool InsertToBody(this CodeFunction codeFunction, string codeText, bool insertAtEnd)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((codeFunction == null) || string.IsNullOrEmpty(codeText)) return false;
            string buff = codeFunction.StartPoint.CreateEditPoint().GetText(codeFunction.EndPoint);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(buff);
            SyntaxNode root = tree.GetRoot();
            if (root == null) return false;
            GlobalStatementSyntax gss = null;
            LocalFunctionStatementSyntax lfss = null;
            foreach (SyntaxNode nd in root.ChildNodes())
            {
                if (nd.IsKind(SyntaxKind.GlobalStatement))
                {
                    gss = nd as GlobalStatementSyntax;
                    break;
                }
                if (nd.IsKind(SyntaxKind.LocalFunctionStatement))
                {
                    lfss = nd as LocalFunctionStatementSyntax;
                    break;
                }
            }
            if ((lfss == null) && (gss != null))
            {
                foreach (SyntaxNode nd in gss.ChildNodes())
                {
                    SyntaxKind k = nd.Kind();
                    if (nd.IsKind(SyntaxKind.LocalFunctionStatement))
                    {
                        lfss = nd as LocalFunctionStatementSyntax;
                        break;
                    }
                }
            }
            if (lfss == null) return false;
            if (lfss.Body == null) return false;

            if (insertAtEnd)
            {
                buff = buff.Insert(lfss.Body.Span.End - 1, codeText);
            }
            else
            {
                buff = buff.Insert(lfss.Body.Span.Start + 1, codeText);
            }
            EditPoint editPoint = codeFunction.StartPoint.CreateEditPoint();
            editPoint.Delete(codeFunction.EndPoint);
            editPoint.Insert(buff);
            if (codeFunction.ProjectItem != null)
            {
                codeFunction.ProjectItem.Save();
                editPoint = codeFunction.StartPoint.CreateEditPoint();
                editPoint.SmartFormat(codeFunction.EndPoint);
                codeFunction.ProjectItem.Save();
            }
            else return false;
            return true;
        }
        public static string[] GetMethodParameterNames(this CodeFunction codeFunction)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (codeFunction == null) return null;
            if (codeFunction.Parameters == null) return null;
            List<string> lst = new List<string>();
            foreach (CodeElement prm in codeFunction.Parameters)
            {
                if (prm.Kind != vsCMElement.vsCMElementParameter) continue;
                CodeParameter cprm = prm as CodeParameter;
                if (cprm == null) continue;
                lst.Add(cprm.Name);
            }
            return lst.ToArray();
        }
        public static bool IsEqualClassNames(string className1, string className2)
        {
            if (string.IsNullOrEmpty(className1) || string.IsNullOrEmpty(className2)) return false;
            string[] className1parts = className1.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            string[] className2parts = className2.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if ((className1parts.Length == 1) || (className2parts.Length == 1))
            {
                return string.Equals(className1parts[className1parts.Length - 1], className2parts[className2parts.Length - 1]);
            }
            if (className1parts.Length != className2parts.Length) return false;
            for (int i = 0; i < className1parts.Length; i++)
            {
                if (!string.Equals(className1parts[i], className2parts[i])) return false;
            }
            return true;
        }
        public static bool IsEqualTypeOfExpressions(this TypeOfExpressionSyntax typeOfExpressionSyntax, string typeOfString)
        {
            if ((typeOfExpressionSyntax == null) || string.IsNullOrEmpty(typeOfString)) return false;
            if (typeOfExpressionSyntax.Type == null) return false;
            SyntaxTree tree = CSharpSyntaxTree.ParseText(typeOfString);
            SyntaxNode root = tree.GetRoot();
            GlobalStatementSyntax gss = null;
            TypeOfExpressionSyntax typeOfExpressionSyntax2 = null;
            foreach (SyntaxNode nd in root.ChildNodes())
            {
                if (nd.IsKind(SyntaxKind.GlobalStatement))
                {
                    gss = nd as GlobalStatementSyntax;
                    break;
                }
                if (nd.IsKind(SyntaxKind.TypeOfExpression))
                {
                    typeOfExpressionSyntax2 = nd as TypeOfExpressionSyntax;
                    break;
                }
            }
            if ((typeOfExpressionSyntax2 == null) && (gss != null))
            {
                foreach (SyntaxNode nd in gss.ChildNodes())
                {
                    SyntaxKind k = nd.Kind();
                    if (k == SyntaxKind.TypeOfExpression)
                    {
                        typeOfExpressionSyntax2 = nd as TypeOfExpressionSyntax;
                        break;
                    }
                    else if (k == SyntaxKind.ExpressionStatement)
                    {
                        ExpressionStatementSyntax expressionStatementSyntax = nd as ExpressionStatementSyntax;
                        if (expressionStatementSyntax != null)
                        {
                            if (expressionStatementSyntax.Expression != null)
                            {
                                if (expressionStatementSyntax.Expression.Kind() == SyntaxKind.TypeOfExpression)
                                {
                                    typeOfExpressionSyntax2 = expressionStatementSyntax.Expression as TypeOfExpressionSyntax;
                                    break;
                                }
                            }
                        }
                    }
                }
            }


            if (typeOfExpressionSyntax2 != null)
            {
                if (typeOfExpressionSyntax2.Type != null)
                    return IsEqualClassNames(typeOfExpressionSyntax2.Type.ToString(), typeOfExpressionSyntax.Type.ToString());
            }
            return false;
        }
        public static bool IsExist(this CodeFunction codeFunction, string invocationIdentifierName, string invocationMethodName, string[] invocationGenerics, string[] invocationParams)
        {
            //            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if ((codeFunction == null) || string.IsNullOrEmpty(invocationIdentifierName) || string.IsNullOrEmpty(invocationMethodName) || (invocationGenerics == null) || (invocationParams == null)) return false;
            foreach (string s in invocationGenerics)
            {
                if (string.IsNullOrEmpty(s)) return false;
            }
            foreach (string s in invocationParams)
            {
                if (string.IsNullOrEmpty(s)) return false;
            }
            string buff = codeFunction.StartPoint.CreateEditPoint().GetText(codeFunction.EndPoint);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(buff);
            SyntaxNode root = tree.GetRoot();
            if (root == null) return false;
            GlobalStatementSyntax gss = null;
            LocalFunctionStatementSyntax lfss = null;
            foreach (SyntaxNode nd in root.ChildNodes())
            {
                if (nd.IsKind(SyntaxKind.GlobalStatement))
                {
                    gss = nd as GlobalStatementSyntax;
                    break;
                }
                if (nd.IsKind(SyntaxKind.LocalFunctionStatement))
                {
                    lfss = nd as LocalFunctionStatementSyntax;
                    break;
                }
            }
            if ((lfss == null) && (gss != null))
            {
                foreach (SyntaxNode nd in gss.ChildNodes())
                {
                    SyntaxKind k = nd.Kind();
                    if (nd.IsKind(SyntaxKind.LocalFunctionStatement))
                    {
                        lfss = nd as LocalFunctionStatementSyntax;
                        break;
                    }
                }
            }
            if (lfss == null) return false;
            if (lfss.Body == null) return false;
            foreach (StatementSyntax ss in lfss.Body.Statements)
            {
                SyntaxKind k = ss.Kind();
                if (k != SyntaxKind.ExpressionStatement) continue;
                ExpressionStatementSyntax ess = ss as ExpressionStatementSyntax;
                if (ess == null) continue;
                if (ess.Expression == null) continue;
                if (ess.Expression.Kind() != SyntaxKind.InvocationExpression) continue;
                InvocationExpressionSyntax iess = ess.Expression as InvocationExpressionSyntax;
                if (iess == null) continue;
                if ((iess.ArgumentList == null) && (invocationParams.Length > 0)) continue;
                if (iess.ArgumentList != null)
                {
                    if ((iess.ArgumentList.Arguments == null) && (invocationParams.Length > 0)) continue;
                    if (iess.ArgumentList.Arguments != null)
                    {
                        if (iess.ArgumentList.Arguments.Count != invocationParams.Length) continue;
                    }
                    int i = 0;
                    foreach (ArgumentSyntax argumentSyntax in iess.ArgumentList.Arguments)
                    {
                        // argumentSyntax.Expression
                        if (argumentSyntax.Expression.Kind() == SyntaxKind.StringLiteralExpression)
                        {
                            LiteralExpressionSyntax literalExpressionSyntax = argumentSyntax.Expression as LiteralExpressionSyntax;
                            if (literalExpressionSyntax == null) continue;
                            if (!string.Equals(invocationParams[i], literalExpressionSyntax.ToString())) continue;
                            i++;
                        }
                        else if (argumentSyntax.Expression.Kind() == SyntaxKind.TypeOfExpression)
                        {
                            TypeOfExpressionSyntax typeOfExpressionSyntax = argumentSyntax.Expression as TypeOfExpressionSyntax;
                            if (typeOfExpressionSyntax == null) continue;
                            if (typeOfExpressionSyntax.IsEqualTypeOfExpressions(invocationParams[i])) i++;
                        }
                    }
                    if (i != invocationParams.Length) continue;
                }

                foreach (SyntaxNode syntaxNode in iess.ChildNodes())
                {
                    if (!syntaxNode.IsKind(SyntaxKind.SimpleMemberAccessExpression)) continue;
                    MemberAccessExpressionSyntax memberAccessExpressionSyntax = syntaxNode as MemberAccessExpressionSyntax;
                    if (memberAccessExpressionSyntax == null) continue;
                    ExpressionSyntax expressionSyntax = memberAccessExpressionSyntax.Expression;
                    if (expressionSyntax == null) continue;
                    if ((expressionSyntax.Kind() != SyntaxKind.IdentifierName) && (expressionSyntax.Kind() != SyntaxKind.SimpleMemberAccessExpression)) continue;
                    if (!PrismModuleAnalyzerHelper.IsEqualClassNames(invocationIdentifierName, expressionSyntax.ToString())) continue;
                    SimpleNameSyntax simpleNameSyntax = memberAccessExpressionSyntax.Name;
                    if (simpleNameSyntax == null) continue;
                    SyntaxKind simpleNameSyntaxKind = simpleNameSyntax.Kind();
                    if ((simpleNameSyntaxKind != SyntaxKind.GenericName) && (simpleNameSyntaxKind != SyntaxKind.IdentifierName)) continue;
                    if (simpleNameSyntax.Identifier == null) continue;
                    if (!invocationMethodName.Equals(simpleNameSyntax.Identifier.ValueText)) continue;
                    if ((simpleNameSyntaxKind != SyntaxKind.GenericName) && (invocationGenerics.Length < 1)) return true;
                    GenericNameSyntax genericNameSyntax = simpleNameSyntax as GenericNameSyntax;
                    if ((genericNameSyntax == null) && (invocationGenerics.Length > 0)) continue;
                    if (genericNameSyntax != null)
                    {
                        TypeArgumentListSyntax typeArgumentList = genericNameSyntax.TypeArgumentList;
                        if (typeArgumentList == null) continue;
                        if (typeArgumentList.Arguments == null) continue;
                        if (typeArgumentList.Arguments.Count != invocationGenerics.Length) continue;
                        int i = 0;
                        foreach (TypeSyntax ts in typeArgumentList.Arguments)
                        {
                            if (!PrismModuleAnalyzerHelper.IsEqualClassNames(invocationGenerics[i], ts.ToString())) break;
                            i++;
                        }
                        if (i == invocationGenerics.Length) return true;
                    }
                }

            }
            return false;
        }
        public static string GenerateLineOfCode(string invocationIdentifierName, string invocationMethodName, string[] invocationGenerics, string[] invocationParams)
        {
            if (string.IsNullOrEmpty(invocationIdentifierName) || string.IsNullOrEmpty(invocationMethodName) || (invocationGenerics == null) || (invocationParams == null)) return null;
            foreach (string s in invocationGenerics)
            {
                if (string.IsNullOrEmpty(s)) return null;
            }
            foreach (string s in invocationParams)
            {
                if (string.IsNullOrEmpty(s)) return null;
            }

            StringBuilder result = new StringBuilder(invocationIdentifierName);
            result.Append(".");
            result.Append(invocationMethodName);
            if (invocationGenerics.Length > 0)
            {
                result.Append("<");
                result.Append(invocationGenerics[0]);
                for (int i = 1; i < invocationGenerics.Length; i++)
                {
                    result.Append(", ");
                    result.Append(invocationGenerics[i]);
                }
                result.Append(">");
            }
            result.Append("(");
            if (invocationParams.Length > 0)
            {
                result.Append(invocationParams[0]);
                for (int i = 1; i < invocationParams.Length; i++)
                {
                    result.Append(", ");
                    result.Append(invocationParams[i]);
                }
            }
            result.Append(");");
            return result.ToString();
        }
    }
}
