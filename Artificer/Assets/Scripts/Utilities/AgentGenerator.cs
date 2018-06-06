using Data.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using UnityEngine;
using System.Reflection;
using System;

namespace Generator
{
    /// <summary>
    /// Uses CodeDOM to build the agent script
    /// </summary>
    public class AgentGenerator:MonoBehaviour
    {
        static CodeCompileUnit cu;
        static CodeTypeDeclaration targetClass;

        public static void GenerateAgent(NodeData start)
        {
            cu = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace("Space.AI");
            samples.Imports.Add(new CodeNamespaceImport("System"));
            targetClass = new CodeTypeDeclaration("CustomState");
            targetClass.IsClass = true;
            targetClass.TypeAttributes =
                TypeAttributes.Public;
            targetClass.BaseTypes.Add(new CodeTypeReference
            { BaseType = "ICustomState", Options = CodeTypeReferenceOptions.GenericTypeParameter });
            samples.Types.Add(targetClass);
            cu.Namespaces.Add(samples);

            GenerateLoop(start);
            GenerateCSharp();
        }

        private static void GenerateLoop(NodeData start)
        {
            CodeMemberMethod LoopMethod = new CodeMemberMethod();
            LoopMethod.Attributes =
                MemberAttributes.Public | MemberAttributes.Override;
            LoopMethod.Name = "PerformLoop";
            LoopMethod.ReturnType =
                new CodeTypeReference(typeof(void));

            targetClass.Members.Add(LoopMethod);
        }

        public static void AddFields()
        {
            CodeMemberField widthValueField = new
                CodeMemberField();
            widthValueField.Attributes = MemberAttributes.Private;
            widthValueField.Name = "widthValue";
            widthValueField.Type = new CodeTypeReference(typeof(System.Double));
            widthValueField.Comments.Add(new CodeCommentStatement("The width of the object"));
            targetClass.Members.Add(widthValueField);

            CodeMemberField heightValueField = new CodeMemberField();
            heightValueField.Attributes = MemberAttributes.Private;
            heightValueField.Name = "heightValue";
            heightValueField.Type =
                new CodeTypeReference(typeof(System.Double));
            heightValueField.Comments.Add(new CodeCommentStatement("The height of the object"));
            targetClass.Members.Add(heightValueField);
        }

        public static void AddProperties()
        {
            CodeMemberProperty widthProperty = new CodeMemberProperty();
            widthProperty.Attributes =
                MemberAttributes.Public | MemberAttributes.Final;
            widthProperty.Name = "Width";
            widthProperty.HasGet = true;
            widthProperty.Type = new CodeTypeReference(typeof(System.Double));
            widthProperty.Comments.Add(new CodeCommentStatement("The width property for the object"));
            widthProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "widthValue")));
            targetClass.Members.Add(widthProperty);

            CodeMemberProperty heightProperty = new CodeMemberProperty();
            heightProperty.Attributes =
                MemberAttributes.Public | MemberAttributes.Final;
            heightProperty.Name = "Height";
            heightProperty.HasGet = true;
            heightProperty.Type = new CodeTypeReference(typeof(System.Double));
            heightProperty.Comments.Add(new CodeCommentStatement("The height property for the object"));
            heightProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "heightValue")));
            targetClass.Members.Add(heightProperty);

            CodeMemberProperty areaProperty = new CodeMemberProperty();
            areaProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            areaProperty.Name = "Area";
            areaProperty.HasGet = true;
            areaProperty.Type = new CodeTypeReference(typeof(System.Double));
            areaProperty.Comments.Add(new CodeCommentStatement
                ("The Area property for the object"));

            CodeBinaryOperatorExpression areaExpression =
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "widthValue"),
                    CodeBinaryOperatorType.Multiply,
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "heightValue"));
            areaProperty.GetStatements.Add(
                new CodeMethodReturnStatement(areaExpression));
            targetClass.Members.Add(areaProperty);
        }

        public static void AddMethod()
        {
            CodeMemberMethod toStringMethod = new CodeMemberMethod();
            toStringMethod.Attributes =
                MemberAttributes.Public | MemberAttributes.Override;
            toStringMethod.Name = "ToString";
            toStringMethod.ReturnType =
                new CodeTypeReference(typeof(System.String));

            CodeFieldReferenceExpression widthReference =
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "Width");
            CodeFieldReferenceExpression heightReference =
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "Height");
            CodeFieldReferenceExpression areaReference =
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "Area");

            CodeMethodReturnStatement returnStatement =
                new CodeMethodReturnStatement();

            string formattedOutput = "The object:" + Environment.NewLine +
                " width = {0}," + Environment.NewLine +
                " height = {1}," + Environment.NewLine +
                " area = {2}";
            returnStatement.Expression =
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression("System.String"), "Format",
                    new CodePrimitiveExpression(formattedOutput),
                    widthReference, heightReference, areaReference);
            toStringMethod.Statements.Add(returnStatement);
            targetClass.Members.Add(toStringMethod);
        }  

        public static void AddConstructor()
        {
            CodeConstructor con = new CodeConstructor();
            con.Attributes =
                MemberAttributes.Public | MemberAttributes.Final;

            con.Parameters.Add(new CodeParameterDeclarationExpression
                (typeof(System.Double), "width"));
            con.Parameters.Add(new CodeParameterDeclarationExpression
                (typeof(System.Double), "height"));

            CodeFieldReferenceExpression widthRef =
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "widthValue");
            con.Statements.Add(new CodeAssignStatement(widthRef,
                new CodeArgumentReferenceExpression("width")));
            CodeFieldReferenceExpression heightRef =
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), "heightValue");
            con.Statements.Add(new CodeAssignStatement(heightRef,
                new CodeArgumentReferenceExpression("height")));
            targetClass.Members.Add(con);
        }

        public static void AddEntryPoint()
        {
            CodeEntryPointMethod start = new CodeEntryPointMethod();
            CodeObjectCreateExpression create =
                new CodeObjectCreateExpression(
                    new CodeTypeReference("UserNPC"),
                    new CodePrimitiveExpression(5.3),
                    new CodePrimitiveExpression(6.9));

            start.Statements.Add(new CodeVariableDeclarationStatement(
                new CodeTypeReference("UserNPC"), "testNPC",
                create));

            CodeMethodInvokeExpression toStringInvoke =
                new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("testNPC"), "ToString");

            start.Statements.Add(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("System.Console"),
                "WriteLine", toStringInvoke));
            targetClass.Members.Add(start);
        }

        public static void GenerateCSharp()
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter("UserNPC.cs"))
            {
                provider.GenerateCodeFromCompileUnit
                    (cu, sourceWriter, options);
            }
        }
    }
}