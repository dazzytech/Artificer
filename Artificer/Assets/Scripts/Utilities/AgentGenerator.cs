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
            samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            samples.Imports.Add(new CodeNamespaceImport("Data.UI"));

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

            LoopMethod.Comments.Add(new CodeCommentStatement
                (new CodeComment("The main execution loop of the NPC script")));

            LoopMethod.ReturnType =
                new CodeTypeReference(typeof(void));

            LoopMethod.Statements.AddRange(GenerateNode(start));

            targetClass.Members.Add(LoopMethod);
        }

        /// <summary>
        /// Redirects the node to be generated
        /// based in its category
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static CodeStatement[] GenerateNode(NodeData node)
        {
            switch(node.Category)
            {
                case "Events":
                    return GenerateEvent(node);
                case "Conditional":
                    return GenerateCondtional(node);
                case "Sequence":
                    return GenerateSequence(node);
                case "Debug":
                    return GenerateDebug(node);
                default:
                    return null;
            }
        }

        #region NODE GENERATORS

        private static CodeStatement[] GenerateEvent(NodeData node)
        {
            // Generate code based on the linked node
            foreach(NodeData.IO link in node.Output)
            {
                if(link.LinkedIO != null && link.CurrentType == NodeData.IO.IOType.LINK)
                {
                    return GenerateNode(link.LinkedIO.Node);
                }
            }

            return null;
        }

        private static CodeStatement[] GenerateCondtional(NodeData node)
        {
            if (node.Label == "IF")
            {
                // only create one condition if the node has the IF label
                // input [1] is the boolean variable
                CodeExpression condition = new CodeSnippetExpression(node.Input[1].GetValue);

                // create the script
                CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                    condition,              // The condition to test.
                    new CodeStatement[]     // The statements to execute if the condition evaluates to true.
                        { new CodeCommentStatement("If condition is true, execute these statements.") },
                    new CodeStatement[]     // The statements to execute if the condition evalues to false.
                        { new CodeCommentStatement("Else block. If condition is false, execute these statements.") });
                // return the statement that is created
                return new CodeStatement[] { conditionalStatement };
            }
            else
            {
                // This is a switch, add a comparison for
                // each comparison value
                string Comparison = node.Input[1].GetValue;

                CodeStatement[] ifs = new CodeStatement[node.Input.Count-2];

                // Input[2] is the start of the comparison values
                for (int i = 2; i < node.Input.Count; i++)
                {
                    CodeBinaryOperatorExpression ifMatch = new CodeBinaryOperatorExpression
                        (new CodeVariableReferenceExpression(Comparison), CodeBinaryOperatorType.IdentityEquality, 
                            new CodeVariableReferenceExpression(node.Input[1].GetValue));

                    CodeConditionStatement conditionStatement = new CodeConditionStatement(
                        ifMatch,
                        new CodeStatement[]     // The statements to execute if the condition evaluates to true.
                            { new CodeCommentStatement("If condition is true, execute these statements.") });

                    ifs[i-2] = conditionStatement;
                }

                return ifs;
            }
        }

        /// <summary>
        /// Builds the different types of sequence nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static CodeStatement[] GenerateSequence(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            if (node.Label == "FOR")
            {
                if (node.Output[0].LinkedIO != null)
                {
                    // Create index name and assign to output
                    string indexName = "index_" + node.InstanceID.ToString();

                    // any child nodes may now access the index here
                    node.Output[1].Value = indexName;

                    // The output int of this node is assigned to the node output
                    CodeVariableDeclarationStatement indexInt =
                        new CodeVariableDeclarationStatement(typeof(int),
                        indexName, new CodePrimitiveExpression(0));
                    statements.Add(indexInt);
                    // input[0] exec     
                    // input[1] min input     
                    // input[2] max input
                    // input[3] step input
                    CodeIterationStatement forLoop = new CodeIterationStatement(
                        new CodeAssignStatement(new CodeVariableReferenceExpression(indexName),
                            new CodeSnippetExpression(node.Input[1].GetValue)),
                        new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(indexName),
                            CodeBinaryOperatorType.LessThan, new CodeSnippetExpression(node.Input[2].GetValue)),
                        new CodeAssignStatement(new CodeVariableReferenceExpression(indexName),
                            new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(indexName), CodeBinaryOperatorType.Add,
                            new CodeSnippetExpression(node.Input[3].GetValue))),
                            GenerateNode(node.Output[0].LinkedIO.Node)); // The statements to execute if the condition evaluates to true.
                    statements.Add(forLoop);
                }

                // if there is an end node then generate the end node - Output[2]
                if(node.Output[2].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[2].LinkedIO.Node));
            }
            else if(node.Label == "FOREACH")
            {
                // Create index name and assign to output
                string indexName = "index_" + node.InstanceID.ToString();
                // any child nodes may now access the index here
                node.Output[1].Value = indexName;

                // Create item name and assign to output
                string itemName = "item_" + node.InstanceID.ToString();
                // access the item via the output
                node.Output[2].Value = itemName;

                // The output int of this node is assigned to the node output
                CodeVariableDeclarationStatement indexInt =
                    new CodeVariableDeclarationStatement(typeof(int),
                    indexName, new CodePrimitiveExpression(0));
                statements.Add(indexInt);

                // The output item of this node is assigned to the node output
                CodeVariableDeclarationStatement itemRef =
                    new CodeVariableDeclarationStatement(node.Input[1].IOGetType,
                    itemName);
                statements.Add(itemRef);

                List<CodeStatement> loopBody = new List<CodeStatement>();
                loopBody.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(itemName),
                                new CodeSnippetExpression(string.Format("{0}[{1}]", node.Input[1].GetValue, indexName))));
                if (node.Output[0].LinkedIO != null)
                    loopBody.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

                CodeIterationStatement forLoop = new CodeIterationStatement(
                    new CodeAssignStatement(new CodeVariableReferenceExpression(indexName),
                        new CodePrimitiveExpression(0)),
                    new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(indexName),
                        CodeBinaryOperatorType.LessThan, new CodeSnippetExpression(node.Input[1].GetValue + ".Count()")),
                    new CodeAssignStatement(new CodeVariableReferenceExpression(indexName),
                        new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(indexName), CodeBinaryOperatorType.Add,
                        new CodePrimitiveExpression(1))),
                    loopBody.ToArray());

                statements.Add(forLoop);
            }
            else if(node.Label == "WHILE")
            {
                CodeIterationStatement whileLoop = new CodeIterationStatement(new CodeSnippetStatement(), new CodeSnippetExpression(node.Input[1].GetValue), 
                    new CodeSnippetStatement(), new CodeStatement[]     
                                { new CodeCommentStatement("Execute the loop body") });
                statements.Add(whileLoop);
            }
            else if(node.Label == "SEQUENCE")
            {
                // loop through each output and add the node statement to list
                foreach(NodeData.IO output in node.Output)
                {
                    if (output.LinkedIO != null)
                        statements.AddRange(GenerateNode(output.LinkedIO.Node));
                }
            }

            return statements.ToArray();
        }

        /// <summary>
        /// Generates nodes which purposes are entirely for testing purposes
        /// including created values and outputs
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static CodeStatement[] GenerateDebug(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            if (node.Label == "LOG")
            {
                // Creates a debug.log to output a message
                if (node.Input[1].GetValue != null)
                {
                    // generate an input string here
                    string input = node.Input[1].GetValue;
                    // if the input is not a string then it will need to be converted
                    if (node.Input[1].CurrentType != NodeData.IO.IOType.STRING)
                        input = string.Concat(input, ".ToString()");

                    CodeSnippetStatement code =
                        new CodeSnippetStatement("Debug.Log(" + input + ");");

                    statements.Add(code);
                }
                if(node.Output[0].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
            }
            else if(node.Label == "CREATESTRING")
            {
                // Create string name and assign to output
                string stringName = "string_" + node.InstanceID.ToString();
                node.Output[1].Value = stringName;

                // The output item of this node is assigned to the node output
                CodeVariableDeclarationStatement itemRef =
                    new CodeVariableDeclarationStatement(typeof(string),
                    stringName, node.Input[1].GetValue != null? new CodeSnippetExpression(node.Input[1].GetValue) : null);

                statements.Add(itemRef);

                if(node.Output[0].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
            }
            else if(node.Label == "CREATENUM")
            {
                // Create string name and assign to output
                string numName = "number_" + node.InstanceID.ToString();
                node.Output[1].Value = numName;

                // The output item of this node is assigned to the node output
                CodeVariableDeclarationStatement itemRef =
                    new CodeVariableDeclarationStatement(typeof(float),
                    numName, node.Input[1].GetValue != null ? new CodeSnippetExpression(node.Input[1].GetValue) : null);

                statements.Add(itemRef);

                if (node.Output[0].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
            }
            else if(node.Label == "CREATENUMARRAY")
            {
                string arrayName = "array_" + node.InstanceID.ToString();
                node.Output[1].Value = arrayName;

                // The output item of this node is assigned to the node output
                CodeVariableDeclarationStatement arrayRef =
                    new CodeVariableDeclarationStatement(typeof(List<float>),
                    arrayName, new CodeSnippetExpression("new List<float>()"));

                statements.Add(arrayRef);

                if (node.Output[0].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

            }
            else if (node.Label == "CREATESTRINGARRAY")
            {
                string arrayName = "array_" + node.InstanceID.ToString();
                node.Output[1].Value = arrayName;

                // The output item of this node is assigned to the node output
                CodeVariableDeclarationStatement arrayRef =
                    new CodeVariableDeclarationStatement(typeof(List<string>),
                    arrayName, new CodeSnippetExpression("new List<string>()"));

                statements.Add(arrayRef);

                if (node.Output[0].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

            }
            else if(node.Label == "CREATEOBJARRAY")
            {
                string arrayName = "array_" + node.InstanceID.ToString();
                node.Output[1].Value = arrayName;

                // The output item of this node is assigned to the node output
                CodeVariableDeclarationStatement arrayRef =
                    new CodeVariableDeclarationStatement(typeof(List<IDEObjectData>),
                    arrayName, new CodeSnippetExpression("new List<IDEObjectData>()"));

                statements.Add(arrayRef);

                if (node.Output[0].LinkedIO != null)
                    statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
            }

            return statements.ToArray();
        }

        #endregion

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