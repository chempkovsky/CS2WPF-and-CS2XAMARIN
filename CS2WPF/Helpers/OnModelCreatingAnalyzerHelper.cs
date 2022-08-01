using CS2WPF.Model.AnalyzeOnModelCreating;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS2WPF.Helpers
{

    // HasOne() is an Entity Framework Core method.
    // In prior versions you use HasOptional() or HasRequired().
#pragma warning disable VSTHRD010
    public static class OnModelCreatingAnalyzerHelper
    {

        public static MethodDeclarationSyntax GetOnModelCreatingParameterName(this SyntaxNode root, string parameterType1, string parameterType2, out string parameterName)
        {
            parameterName = null;
            if ((root == null) || string.IsNullOrEmpty(parameterType1) || string.IsNullOrEmpty(parameterType2)) return null;
            foreach (SyntaxNode nd in root.ChildNodes())
            {
                if (!nd.IsKind(SyntaxKind.MethodDeclaration)) continue;
                MethodDeclarationSyntax methodDeclaration = nd as MethodDeclarationSyntax;
                if (methodDeclaration == null) { continue; }
                // method name
                if (methodDeclaration.Identifier == null) continue;
                if (methodDeclaration.Identifier.ValueText != "OnModelCreating") continue;
                // method return type
                if (methodDeclaration.ReturnType == null) continue;
                if (methodDeclaration.ReturnType.Kind() != SyntaxKind.PredefinedType) continue;
                PredefinedTypeSyntax predefinedType = methodDeclaration.ReturnType as PredefinedTypeSyntax;
                if (predefinedType == null) continue;
                if (!predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword)) continue;
                // method modifiers
                if (methodDeclaration.Modifiers == null) continue;
                if (methodDeclaration.Modifiers.Count != 2) continue;
                if (!(methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword) && methodDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))) continue;
                // method parameters
                if (methodDeclaration.ParameterList == null) continue;
                if (methodDeclaration.ParameterList.Parameters == null) continue;
                if (methodDeclaration.ParameterList.Parameters.Count != 1) continue;
                ParameterSyntax parameterSyntax = methodDeclaration.ParameterList.Parameters[0];
                if (parameterSyntax.Type == null) continue;
                if ((parameterSyntax.Type.Kind() != SyntaxKind.IdentifierName) &&
                    (parameterSyntax.Type.Kind() != SyntaxKind.QualifiedName)) continue;
                string parameterTypeName = parameterSyntax.Type.ToString();
                if (string.IsNullOrEmpty(parameterTypeName)) continue;
                if ((!parameterTypeName.Contains(parameterType1)) && (!parameterTypeName.Contains(parameterType2))) continue;
                parameterName = parameterSyntax.Identifier.ValueText;
                return methodDeclaration;
            }
            return null;
        }
        public static void DoAnalyze(this string src, List<FluentAPIEntityNode> entityNodes)
        {
            if (entityNodes == null)
            {
                // "Error: Input param is not defined."
                return;
            }
            if (string.IsNullOrEmpty(src))
            {
                // "Error: There is no text to process."
                return;
            }
            SyntaxTree tree = CSharpSyntaxTree.ParseText(src);
            SyntaxNode root = tree.GetRoot();
            if (root == null)
            {
                // "Error: Could not get SyntaxTree Root node. Try to compile before run the wizard.";
                return;
            }

            //if (root.Kind() != SyntaxKind.CompilationUnit)
            //{
            //    // "Error: Parse expects CompilationUnit. Input text should contain complete definition of the method.";
            //    return;
            //}
            //if (!(root is MethodDeclarationSyntax))
            //{
            //    // "Error: Parse expects MethodDeclarationSyntax. Input text should contain complete definition of the method.";
            //    return;
            //}

            MethodDeclarationSyntax methodDeclaration = null;
            string parameterName = "";
            foreach (SyntaxNode nd in root.ChildNodes())
            {
                if (!nd.IsKind(SyntaxKind.MethodDeclaration)) continue;
                methodDeclaration = nd as MethodDeclarationSyntax;
                if (methodDeclaration == null) { continue; }
                // method name
                if (methodDeclaration.Identifier == null) continue;
                if (methodDeclaration.Identifier.ValueText != "OnModelCreating") continue;
                // method return type
                if (methodDeclaration.ReturnType == null) continue;
                if (methodDeclaration.ReturnType.Kind() != SyntaxKind.PredefinedType) continue;
                PredefinedTypeSyntax predefinedType = methodDeclaration.ReturnType as PredefinedTypeSyntax;
                if (predefinedType == null) continue;
                if (!predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword)) continue;
                // method modifiers
                if (methodDeclaration.Modifiers == null) continue;
                if (methodDeclaration.Modifiers.Count != 2) continue;
                if (!(methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword) && methodDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))) continue;
                // method parameters
                if (methodDeclaration.ParameterList == null) continue;
                if (methodDeclaration.ParameterList.Parameters == null) continue;
                if (methodDeclaration.ParameterList.Parameters.Count != 1) continue;
                ParameterSyntax parameterSyntax = methodDeclaration.ParameterList.Parameters[0];
                if (parameterSyntax.Type == null) continue;
                if ((parameterSyntax.Type.Kind() != SyntaxKind.IdentifierName) &&
                    (parameterSyntax.Type.Kind() != SyntaxKind.QualifiedName)) continue;
                string parameterTypeName = parameterSyntax.Type.ToString();
                if (string.IsNullOrEmpty(parameterTypeName)) continue;
                if ((!parameterTypeName.Contains("DbModelBuilder")) && (!parameterTypeName.Contains("ModelBuilder"))) continue;
                parameterName = parameterSyntax.Identifier.ValueText;
                break;
            }
            if (string.IsNullOrEmpty(parameterName))
            {
                //    // "Error: Could not find method Protected Override void OnModelCreating(DbModelBuilder ...).";
                return;
            }
            if (methodDeclaration.Body == null)
            {
                // "Error: OnModelCreating-body is not defined.";
                return;
            }
            foreach (StatementSyntax ss in methodDeclaration.Body.Statements)
            {
                if (ss.Kind() != SyntaxKind.ExpressionStatement) continue;
                ExpressionStatementSyntax expressionStatementSyntax = ss as ExpressionStatementSyntax;
                if (expressionStatementSyntax.Expression == null) continue;
                if (expressionStatementSyntax.Expression.Kind() != SyntaxKind.InvocationExpression) continue;
                InvocationExpressionSyntax invocationExpressionSyntax = expressionStatementSyntax.Expression as InvocationExpressionSyntax;
                if (!parameterName.Equals(invocationExpressionSyntax.InvocationExpressionRootName())) continue;
                string methodBodyStr = expressionStatementSyntax.ToString();
                methodBodyStr = methodBodyStr.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace(" ", "");
                FluentAPIEntityNode faen = expressionStatementSyntax.Expression.InvocationExpressionMethods();
                if (faen != null)
                {
                    faen.MethodBodyString = methodBodyStr;
                    entityNodes.Add(faen);
                }
            }
            // return null
            return;
        }
        public static string InvocationExpressionRootName(this ExpressionSyntax invocation)
        {
            if (invocation == null)
            {
                return "";
            }
            if (invocation is IdentifierNameSyntax)
            {
                if ((invocation as IdentifierNameSyntax).Identifier == null)
                {
                    return "";
                }
                return (invocation as IdentifierNameSyntax).Identifier.ValueText;
            }
            foreach (SyntaxNode invchnd in invocation.ChildNodes())
            {
                if (invchnd.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    ExpressionSyntax exst = (invchnd as MemberAccessExpressionSyntax).Expression;
                    if (exst != null)
                    {
                        return exst.InvocationExpressionRootName();
                    }
                }
            }
            return "";
        }

        public static string InvocationExpressionRootName(this ExpressionSyntax invocation, string[] classNames)
        {
            if (invocation == null)
            {
                return "";
            }
            if (invocation is IdentifierNameSyntax)
            {
                if ((invocation as IdentifierNameSyntax).Identifier == null)
                {
                    return "";
                }
                return (invocation as IdentifierNameSyntax).Identifier.ValueText;
            }
            foreach (SyntaxNode invchnd in invocation.ChildNodes())
            {
                if (invchnd.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    ExpressionSyntax exst = (invchnd as MemberAccessExpressionSyntax).Expression;
                    if (exst != null)
                    {
                        if (exst is IdentifierNameSyntax)
                        {
                            if ((exst as IdentifierNameSyntax).Identifier == null)
                            {
                                return "";
                            }
                            if (classNames != null)
                            {
                                if (classNames.Count() > 0)
                                {
                                    SimpleNameSyntax aname = (invchnd as MemberAccessExpressionSyntax).Name;
                                    if (aname.Kind() == SyntaxKind.GenericName)
                                    {
                                        if (!"Entity".Equals(aname.Identifier.ValueText))
                                        {
                                            return "";
                                        }

                                        TypeArgumentListSyntax typeArgumentList = (aname as GenericNameSyntax).TypeArgumentList;
                                        if (typeArgumentList != null)
                                        {
                                            if (typeArgumentList.Arguments != null)
                                            {
                                                string locClassName = "";
                                                foreach (TypeSyntax ts in typeArgumentList.Arguments)
                                                {
                                                    locClassName = ts.ToString();
                                                }
                                                if (classNames.Any(i => string.Equals(i, locClassName)))
                                                {
                                                    return (exst as IdentifierNameSyntax).Identifier.ValueText;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return "";
                        }
                        else
                        {
                            return exst.InvocationExpressionRootName(classNames);
                        }
                    }
                }
            }
            return "";
        }

        public static FluentAPIEntityNode InvocationExpressionMethods(this ExpressionSyntax invocation, FluentAPIEntityNode entityNode = null, string[] methodNames = null)
        {
            if (invocation == null)
            {
                return entityNode;
            }
            if (invocation is IdentifierNameSyntax)
            {
                return entityNode;
            }
            string methodName = "";
            string genericName = "";
            int cnt = invocation.ChildNodes().Count();
            foreach (SyntaxNode ndcn in invocation.ChildNodes())
            {
                if (ndcn.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    MemberAccessExpressionSyntax memberAccessExpressionSyntax = ndcn as MemberAccessExpressionSyntax;
                    entityNode = InvocationExpressionMethods(memberAccessExpressionSyntax.Expression, entityNode, methodNames);
                    methodName = memberAccessExpressionSyntax.Name.Identifier.ValueText;
                    if (memberAccessExpressionSyntax.Name.Kind() == SyntaxKind.GenericName)
                    {
                        string locEntityName = "";
                        TypeArgumentListSyntax typeArgumentList = (memberAccessExpressionSyntax.Name as GenericNameSyntax).TypeArgumentList;
                        if (typeArgumentList != null)
                        {
                            if (typeArgumentList.Arguments != null)
                            {
                                foreach (TypeSyntax ts in typeArgumentList.Arguments)
                                {
                                    locEntityName = ts.ToString();
                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(locEntityName))
                        {
                            if ("Entity".Equals(memberAccessExpressionSyntax.Name.Identifier.ValueText))
                            {
                                methodName = "";
                                genericName = "";
                                if (methodNames != null)
                                {
                                    if (methodNames.Count() > 0)
                                    {
                                        if (!methodNames.Any(i => string.Equals(i, "Entity"))) continue;
                                    }
                                }
                                if (entityNode == null)
                                {
                                    entityNode = new FluentAPIEntityNode() { EntityName = locEntityName };
                                }
                                else
                                {
                                    entityNode.EntityName = locEntityName;
                                }
                                continue;
                            }
                            else
                            {
                                genericName = locEntityName;
                            }
                        }
                        methodName = memberAccessExpressionSyntax.Name.Identifier.ValueText;
                    }
                }
                if (string.IsNullOrEmpty(methodName)) continue;
                if (methodNames != null)
                {
                    if (methodNames.Count() > 0)
                    {
                        if (!methodNames.Any(i => string.Equals(i, methodName))) continue;
                    }
                }
                if (ndcn.IsKind(SyntaxKind.ArgumentList))
                {
                    if (entityNode == null)
                    {
                        entityNode = new FluentAPIEntityNode();
                    }
                    if (entityNode.Methods == null)
                    {
                        entityNode.Methods = new List<FluentAPIMethodNode>();
                    }
                    FluentAPIMethodNode methodNode = new FluentAPIMethodNode() { MethodName = methodName, GenericName = genericName };
                    entityNode.Methods.Add(methodNode);
                    methodName = "";
                    genericName = "";
                    ArgumentListSyntax argumentListSyntax = ndcn as ArgumentListSyntax;
                    foreach (ArgumentSyntax argument in argumentListSyntax.Arguments)
                    {
                        if (argument.Expression.Kind() == SyntaxKind.SimpleLambdaExpression)
                        {
                            SimpleLambdaExpressionSyntax sles = argument.Expression as SimpleLambdaExpressionSyntax;
                            if (sles.Body.Kind() == SyntaxKind.AnonymousObjectCreationExpression)
                            {
                                AnonymousObjectCreationExpressionSyntax aoces = sles.Body as AnonymousObjectCreationExpressionSyntax;
                                foreach (AnonymousObjectMemberDeclaratorSyntax intlzrs in aoces.Initializers)
                                {
                                    if (methodNode.MethodArguments == null) methodNode.MethodArguments = new List<String>();
                                    methodNode.MethodArguments.Add((intlzrs.Expression as MemberAccessExpressionSyntax).Name.ToString());
                                }
                            }
                            else
                            {
                                if (sles.Body.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                                {
                                    if (methodNode.MethodArguments == null) methodNode.MethodArguments = new List<String>();
                                    methodNode.MethodArguments.Add((sles.Body as MemberAccessExpressionSyntax).Name.ToString());
                                }
                                else
                                {
                                    if (methodNode.MethodArguments == null) methodNode.MethodArguments = new List<String>();
                                    methodNode.MethodArguments.Add(sles.Body.ToString());

                                }
                            }
                        }
                        else
                        {
                            if (methodNode.MethodArguments == null) methodNode.MethodArguments = new List<String>();
                            methodNode.MethodArguments.Add(argument.Expression.ToString());
                        }

                    }
                }
            }
            return entityNode;
        }
        public static List<FluentAPIEntityNode> DoAnalyze(this CodeClass codeClass)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (codeClass == null)
            {
                return null;
            }
            foreach (CodeElement codeElement in codeClass.Children)
            {
                if (codeElement == null) continue;
                if (codeElement.Kind != vsCMElement.vsCMElementFunction) continue;
                string funkName = codeElement.FullName;
                if (funkName == null) continue;
                if (!funkName.Contains("OnModelCreating")) continue;
                CodeFunction codeFunction = codeElement as CodeFunction;
                if (codeFunction == null) continue;
                if (codeFunction.Access != vsCMAccess.vsCMAccessProtected) continue;
                //if (codeFunction.FunctionKind != vsCMFunction.vsCMFunctionVirtual) continue;

                string buff;
                //TextDocument textDocument = codeElement.StartPoint.Parent;
                //IVsTextLines lines = textDocument as IVsTextLines;
                //lines.GetLineText(codeElement.StartPoint.Line, codeElement.StartPoint.LineCharOffset, codeElement.EndPoint.Line, codeElement.EndPoint.LineCharOffset, out buff);
                buff = codeElement.StartPoint.CreateEditPoint().GetText(codeElement.EndPoint);
                if (buff == null) continue;
                List<FluentAPIEntityNode> result = new List<FluentAPIEntityNode>();
                buff.DoAnalyze(result);
                return result;
            }

            return null;
        }

        public static List<FluentAPIEntityNode> DoAnalyzeWithFilter(this CodeFunction codeFunction, string[] classNames, List<FluentAPIEntityNode> filter)
        {
            if (classNames == null) return null;
            if (codeFunction == null) return null;
            string buff = codeFunction.StartPoint.CreateEditPoint().GetText(codeFunction.EndPoint);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(buff);
            SyntaxNode root = tree.GetRoot();
            if (root == null)
            {
                return null;
            }
            MethodDeclarationSyntax methodDeclaration =
                root.GetOnModelCreatingParameterName("DbModelBuilder", "ModelBuilder", out string parameterName);
            if ((methodDeclaration == null) || string.IsNullOrEmpty(parameterName)) return null;
            if (methodDeclaration.Body == null)
            {
                return null;
            }
            List<FluentAPIEntityNode> entityNodes = null;
            foreach (StatementSyntax ss in methodDeclaration.Body.Statements)
            {
                if (ss.Kind() != SyntaxKind.ExpressionStatement) continue;
                ExpressionStatementSyntax expressionStatementSyntax = ss as ExpressionStatementSyntax;
                if (expressionStatementSyntax.Expression == null) continue;
                if (expressionStatementSyntax.Expression.Kind() != SyntaxKind.InvocationExpression) continue;
                InvocationExpressionSyntax invocationExpressionSyntax = expressionStatementSyntax.Expression as InvocationExpressionSyntax;
                if (!parameterName.Equals(invocationExpressionSyntax.InvocationExpressionRootName(classNames))) continue;
                string methodBodyStr = expressionStatementSyntax.ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace(" ", "");
                FluentAPIEntityNode faen = expressionStatementSyntax.Expression.InvocationExpressionMethods(null);
                if (faen == null) continue;
                if (faen.Methods == null) continue;
                if (faen.IsSatisfiedTheFilter(filter))
                {
                    if (entityNodes == null) entityNodes = new List<FluentAPIEntityNode>();
                    faen.MethodBodyString = methodBodyStr;
                    entityNodes.Add(faen);
                }
            }
            return entityNodes;
        }
        public static void DoRemoveInvocationWithFilter(this CodeFunction codeFunction, string[] classNames, List<FluentAPIEntityNode> filter)
        {
            if (classNames == null) return;
            if (codeFunction == null) return;
            EditPoint editPoint = codeFunction.StartPoint.CreateEditPoint();
            editPoint.SmartFormat(codeFunction.EndPoint);
            editPoint = codeFunction.StartPoint.CreateEditPoint();
            string buff = editPoint.GetText(codeFunction.EndPoint);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(buff);
            SyntaxNode root = tree.GetRoot();
            if (root == null)
            {
                return;
            }
            MethodDeclarationSyntax methodDeclaration =
                root.GetOnModelCreatingParameterName("DbModelBuilder", "ModelBuilder", out string parameterName);
            if ((methodDeclaration == null) || string.IsNullOrEmpty(parameterName)) return;
            if (methodDeclaration.Body == null)
            {
                return;
            }
            List<TextSpan> spans = new List<TextSpan>();
            foreach (StatementSyntax ss in methodDeclaration.Body.Statements)
            {
                if (ss.Kind() != SyntaxKind.ExpressionStatement) continue;
                ExpressionStatementSyntax expressionStatementSyntax = ss as ExpressionStatementSyntax;
                if (expressionStatementSyntax.Expression == null) continue;
                if (expressionStatementSyntax.Expression.Kind() != SyntaxKind.InvocationExpression) continue;
                InvocationExpressionSyntax invocationExpressionSyntax = expressionStatementSyntax.Expression as InvocationExpressionSyntax;
                if (!parameterName.Equals(invocationExpressionSyntax.InvocationExpressionRootName(classNames))) continue;
                FluentAPIEntityNode faen = expressionStatementSyntax.Expression.InvocationExpressionMethods(null);
                if (faen == null) continue;
                if (faen.Methods == null) continue;
                if (faen.IsSatisfiedTheFilter(filter))
                {
                    spans.Insert(0, expressionStatementSyntax.Span);
                }
            }

            foreach (TextSpan ts in spans)
            {
                buff = buff.Remove(ts.Start, ts.Length);
                //
                // the commented code does not work : ts.Start does not correctly point to begining of the operator
                //editPoint.CharRight(ts.Start);
                //editPoint.Delete(ts.Length);
                //editPoint.CharLeft(ts.Start);
                //if (codeFunction.ProjectItem != null)
                //{
                //   codeFunction.ProjectItem.Save();
                //}
            }
            buff = buff.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            editPoint = codeFunction.StartPoint.CreateEditPoint();
            editPoint.Delete(codeFunction.EndPoint);
            editPoint.Insert(buff);
            if (codeFunction.ProjectItem != null)
            {
                codeFunction.ProjectItem.Save();
            }
        }
        public static void AddStatementsToFunctionBody(this CodeFunction codeFunction, string statements, string className, bool insertAtEnd)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(statements) || (codeFunction == null)) return;

            EditPoint editPoint = codeFunction.StartPoint.CreateEditPoint();
            string buff = editPoint.GetText(codeFunction.EndPoint);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(buff);
            SyntaxNode root = tree.GetRoot();
            if (root == null)
            {
                return;
            }
            MethodDeclarationSyntax methodDeclaration =
                root.GetOnModelCreatingParameterName("DbModelBuilder", "ModelBuilder", out string parameterName);
            if ((methodDeclaration == null) || string.IsNullOrEmpty(parameterName)) return;
            if (methodDeclaration.Body == null)
            {
                return;
            }
            string strToInsert = //Environment.NewLine +
                parameterName + ".Entity<" + className + ">()" + statements;
            if (insertAtEnd)
            {
                buff = buff.Insert(methodDeclaration.Body.Span.End - 1, strToInsert);
            }
            else
            {
                buff = buff.Insert(methodDeclaration.Body.Span.Start + 1, strToInsert);
            }
            editPoint = codeFunction.StartPoint.CreateEditPoint();
            editPoint.Delete(codeFunction.EndPoint);
            editPoint.Insert(buff);
            if (codeFunction.ProjectItem != null)
            {
                codeFunction.ProjectItem.Save();
            }
        }
    }
}