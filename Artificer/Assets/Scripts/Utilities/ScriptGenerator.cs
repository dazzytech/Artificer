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
using Space.AI;
using UnityEngine.UI;

namespace Generator
{
    /// <summary>
    /// Stores information about any erros occuring within
    /// the compiler to display
    /// </summary>
    public class DebugMessages
    {
        /// <summary>
        /// What is wrong
        /// </summary>
        public string Message;

        /// <summary>
        /// The node that triggered the issue -1 if none
        /// </summary>
        public string NodeInstance;
    }

    /// <summary>
    /// Uses CodeDOM to build the agent script
    /// </summary>
    public class ScriptGenerator:MonoBehaviour
    {
        #region ATTRIBUTES

        /// <summary>
        /// The graph object that generates the object
        /// </summary>
        private CodeCompileUnit m_compileUnit;

        /// <summary>
        /// List of errors with the compiler
        /// </summary>
        private List<DebugMessages> m_debugMsgs;

        #endregion

        #region ACCESSOR

        /// <summary>
        /// returns true if the script generator is
        /// compiled
        /// </summary>
        public bool IsCompiled
        {
            get
            {
                return m_compileUnit != null;
            }
        }

        public List<DebugMessages> DebugMsg
        {
            get
            {
                return m_debugMsgs;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Loads all the player nodes into a CodeDOM graph
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public ICustomScript GenerateCodeGraph(NodeData start, NodeData inRange)
        {
            m_compileUnit = new CodeCompileUnit();

            m_debugMsgs = new List<DebugMessages>();

            CodeNamespace samples = new CodeNamespace("Space.AI");
            samples.Imports.Add(new CodeNamespaceImport("System"));
            samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            CodeTypeDeclaration targetClass = new CodeTypeDeclaration("CustomScript");  
            
            targetClass.IsClass = true;
            targetClass.TypeAttributes =
                TypeAttributes.Public;
            targetClass.BaseTypes.Add(new CodeTypeReference
                ("ICustomScript"));

            samples.Types.Add(targetClass);
            m_compileUnit.Namespaces.Add(samples);

            Assembly compiled = null;

            bool Loop = GenerateLoop(targetClass, start);
            bool InRange = GenerateEvent(targetClass, inRange);

            if (Loop || InRange)
                compiled = CompileScript();
            else
                m_debugMsgs.Add(new DebugMessages()
                    { Message = "Script Failed to Compile", NodeInstance = "-1" });



            if(compiled != null)
            {
                ICustomScript newScript = 
                    compiled.CreateInstance("Space.AI.CustomScript") as ICustomScript;

                if (newScript != null)
                    return newScript;
            }

            GenerateCSharp();

            m_compileUnit = null;

            return null;
        }

        /// <summary>
        /// Converts the CodeDOM graph into C# code
        /// </summary>
        public void GenerateCSharp()
        { 
            using (StreamWriter sourceWriter = new StreamWriter("CustomState.cs"))
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();

                provider.GenerateCodeFromCompileUnit
                    (m_compileUnit, sourceWriter, 
                    new CodeGeneratorOptions() { BracingStyle = "C" });
            }
        }

        #endregion

        #region PRIVATE UTILTIES

        /// <summary>
        /// Builds the 
        /// </summary>
        /// <param name="targetClass"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private bool GenerateLoop(CodeTypeDeclaration targetClass, NodeData start)
        {
            CodeMemberMethod LoopMethod = new CodeMemberMethod();
            LoopMethod.Attributes =
                MemberAttributes.Public | MemberAttributes.Override;
            LoopMethod.Name = "PerformLoop";

            LoopMethod.Comments.Add(new CodeCommentStatement
                (new CodeComment("The main execution loop of the NPC script")));

            LoopMethod.ReturnType =
                new CodeTypeReference(typeof(void));

            targetClass.Members.Add(LoopMethod);

            CodeStatement[] codeStatement = GenerateNode(start);

            if(codeStatement == null)
            {
                return false;
            }

            // Create the statements for the body
            LoopMethod.Statements.AddRange(codeStatement);

            return true;
        }

        private bool GenerateEvent(CodeTypeDeclaration targetClass, NodeData inRange)
        {
            CodeMemberMethod enterMethod = new CodeMemberMethod();
            enterMethod.Attributes =
                MemberAttributes.Public | MemberAttributes.Override;
            enterMethod.Name = "EnterRange";
            enterMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                typeof(EntityObject), "entity"));

            inRange.Output[1].Value = "entity";

            enterMethod.Comments.Add(new CodeCommentStatement
                (new CodeComment("Triggered when an entity enters range")));

            enterMethod.ReturnType =
                new CodeTypeReference(typeof(void));

            targetClass.Members.Add(enterMethod);

            CodeStatement[] codeStatement = GenerateNode(inRange);

            if (codeStatement == null)
            {
                return false;
            }

            // Create the statements for the body
            enterMethod.Statements.AddRange(codeStatement);

            

            return true;
        }

        /// <summary>
        /// Redirects the node to be generated
        /// based in its category
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateNode(NodeData node)
        {
            switch(node.Category)
            {
                case "Events":
                    return GenerateEvent(node);
                case "Conditional":
                    return GenerateCondtional(node);
                case "Sequence":
                    return GenerateSequence(node);
                case "Variables":
                    return GenerateVariables(node);
                case "Ship Interact":
                    return GenerateShipInteract(node);
                case "Agent Accessor":
                    return GenerateAgentAccessor(node);
                case "Debug":
                    return GenerateDebug(node);
                default:
                    return null;
            }
        }

        #region NODE GENERATORS

        private CodeStatement[] GenerateEvent(NodeData node)
        {
            // Generate code based on the linked node
            foreach(NodeData.IO link in node.Output)
            {
                if(link.LinkedIO != null && link.CurrentType == NodeData.IO.IOType.LINK)
                {
                    return GenerateNode(link.LinkedIO.Node);
                }
            }

            // make sure to warn the player is entry node isn't assigned
            if (node.Label == "ENTRY")
                m_debugMsgs.Add(new DebugMessages()
                {
                    NodeInstance = node.InstanceID.ToString(),
                    Message = "Entry node not connected"
                });

            return null;
        }

        /// <summary>
        /// Direct the program flow based on which conditions are met
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateCondtional(NodeData node)
        {
            switch (node.Label)
            {
                case "IF":
                    {
                        #region ERROR CHECKING

                        // Error checking
                        if (node.Input[1].GetValue == null)
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "Bool variable not assigned to node"
                            });

                            return null;
                        }

                        #endregion 

                        // only create one condition if the node has the IF label
                        // input [1] is the boolean variable
                        CodeExpression condition = new CodeSnippetExpression(node.Input[1].GetValue);

                        CodeStatement[] trueStatements = null;
                        if (node.Output[0].LinkedIO != null)
                            trueStatements = GenerateNode(node.Output[0].LinkedIO.Node);
                        else
                            trueStatements = new CodeStatement[] { new CodeCommentStatement("Nothing here..") };

                        CodeStatement[] falseStatements = null;
                        if (node.Output[1].LinkedIO != null)
                            falseStatements = GenerateNode(node.Output[1].LinkedIO.Node);
                        else
                            falseStatements = new CodeStatement[] { new CodeCommentStatement("Nothing here..") };

                        if (trueStatements == null || falseStatements == null)
                            return null;

                        // create the script
                        CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                            condition, trueStatements, falseStatements);

                        // return the statement that is created
                        return new CodeStatement[] { conditionalStatement };
                    }
                case "SWITCH":
                    #region ERROR CHECKING

                    // Error checking
                    if (node.Input[1].GetValue == null)
                    {
                        m_debugMsgs.Add(new DebugMessages()
                        {
                            NodeInstance = node.InstanceID.ToString(),
                            Message = "Comparison variable not assigned to node"
                        });

                        return null;
                    }

                    #endregion

                    // This is a switch, add a comparison for
                    // each comparison value
                    string Comparison = node.Input[1].GetValue;

                    List<CodeStatement> ifs = new List<CodeStatement>();

                    ifs.Add(new CodeCommentStatement("Switch Statement is made of an IF statement list"));

                    // Input[2] is the start of the comparison values
                    for (int i = 2; i < node.Input.Count; i++)
                    {
                        if (node.Input[i].GetValue == null)
                            continue;

                        CodeBinaryOperatorExpression ifMatch = new CodeBinaryOperatorExpression
                            (new CodeVariableReferenceExpression(Comparison), CodeBinaryOperatorType.IdentityEquality,
                                new CodeVariableReferenceExpression(node.Input[i].GetValue));

                            CodeStatement[] statements = null;
                            if (node.Output[0].LinkedIO != null)
                                statements = GenerateNode(node.Output[i - 2].LinkedIO.Node);
                        else
                            statements = new CodeStatement[] { new CodeCommentStatement("Nothing here..") };

                        CodeConditionStatement conditionStatement = new CodeConditionStatement(
                            ifMatch,
                            statements);

                        ifs.Add(conditionStatement);
                    }

                    return ifs.ToArray();
                case "MORETHAN":
                case "LESSTHAN":
                case "EQUALS":
                    {
                        #region ERROR CHECKING

                        // Error checking
                        if (node.Input[1].GetValue == null || node.Input[2].GetValue == null)
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "both parameters not assigned to node"
                            });

                            return null;
                        }

                        #endregion

                        List<CodeStatement> statements = new List<CodeStatement>();

                        // Create item name and assign to output
                        string boolName = "result_" + node.InstanceID.ToString();
                        // access the item via the output
                        node.Output[1].Value = boolName;

                        // The output item of this node is assigned to the node output
                        CodeVariableDeclarationStatement boolRef =
                            new CodeVariableDeclarationStatement(typeof(Boolean),
                            boolName);

                        statements.Add(boolRef);

                        CodeExpression condition = new CodeBinaryOperatorExpression(new CodeSnippetExpression(node.Input[1].GetValue),
                                node.Label == "MORETHAN" ? CodeBinaryOperatorType.GreaterThan : node.Label == "LESSTHAN" ? CodeBinaryOperatorType.LessThan : CodeBinaryOperatorType.IdentityEquality,
                                new CodeSnippetExpression(node.Input[2].GetValue));

                        CodeStatement iftrue = new CodeAssignStatement(new CodeVariableReferenceExpression(boolName), new CodePrimitiveExpression(true));

                        CodeStatement iffalse = new CodeAssignStatement(new CodeVariableReferenceExpression(boolName), new CodePrimitiveExpression(false));

                        CodeConditionStatement conditionStatement = new CodeConditionStatement(
                            condition,
                            new CodeStatement[] { iftrue },
                            new CodeStatement[] { iffalse });

                        statements.Add(conditionStatement);

                        if (node.Output[0].LinkedIO != null)
                            statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

                        return statements.ToArray();
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Builds the different types of sequence nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateSequence(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            #region ERROR CHECKING

            if (node.Output[0].LinkedIO != null)
            {
                m_debugMsgs.Add(new DebugMessages()
                {
                    NodeInstance = node.InstanceID.ToString(),
                    Message = "loop output not assigned to node"
                });

                return null;
            }

            #endregion

            switch (node.Label)
            {
                case "FOR":
                {
                        #region ERROR CHECKING

                        // Error checking
                        if (node.Input[1].GetValue == null)
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "Min variable not assigned to node"
                            });

                            return null;
                        }

                        // Error checking
                        if (node.Input[2].GetValue == null)
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "Max variable not assigned to node"
                            });
                            return null;
                        }

                        // Error checking
                        if (node.Input[3].GetValue == null)
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "Step variable not assigned to node"
                            });
                            return null;
                        }

                        #endregion    

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

                    // if there is an end node then generate the end node - Output[2]
                    if (node.Output[2].LinkedIO != null)
                        statements.AddRange(GenerateNode(node.Output[2].LinkedIO.Node));

                    break;
                }
                case "FOREACH":
                {
                        #region ERROR CHECKING

                        // Error checking
                        if (node.Input[1].GetValue == null)
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "Array not connected to node"
                            });

                            return null;
                        }

                        #endregion

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

                    break;
                }
                case "WHILE":
                    #region ERROR CHECKING

                    // Error checking
                    if (node.Input[1].LinkedIO == null)
                    {
                        m_debugMsgs.Add(new DebugMessages()
                        {
                            NodeInstance = node.InstanceID.ToString(),
                            Message = "Input Boolean has to be linked node"
                        });

                        return null;
                    }

                    #endregion

                    statements.Add(new CodeCommentStatement("While statement is comprised of a for loop with a boolean condition."));
                    CodeIterationStatement whileLoop = new CodeIterationStatement(new CodeSnippetStatement(), new CodeSnippetExpression(node.Input[1].GetValue),
                        new CodeSnippetStatement(), GenerateNode(node.Output[0].LinkedIO.Node));
                    statements.Add(whileLoop);
                    break;
                case "SEQUENCE":
                    // loop through each output and add the node statement to list
                    foreach (NodeData.IO output in node.Output)
                    {
                        if (output.LinkedIO != null)
                            statements.AddRange(GenerateNode(output.LinkedIO.Node));
                    }
                    break;
            }
            return statements.ToArray();
        }

        /// <summary>
        /// Generates nodes which purposes are entirely for testing purposes
        /// including created values and outputs
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateDebug(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            switch (node.Label)
            {
                case "LOG":
                {
                    // Creates a debug.log to output a message
                    if (node.Input[1].GetValue == null)
                    {
                        #region ERROR CHECKING

                        // Error checking
                        if (node.Input[1].GetValue == "")
                        {
                            m_debugMsgs.Add(new DebugMessages()
                            {
                                NodeInstance = node.InstanceID.ToString(),
                                Message = "No object connected to output"
                            });

                            return null;
                        }

                            #endregion
                    }
                    else
                    {
                            // generate an input string here
                        string input = node.Input[1].GetValue;
                        // if the input is not a string then it will need to be converted
                        if (node.Input[1].CurrentType != NodeData.IO.IOType.STRING)
                            input = string.Concat(input, ".ToString()");

                        CodeSnippetStatement code =
                            new CodeSnippetStatement("UnityEngine.Debug.Log(" + input + ");");

                        statements.Add(code);
                    }
                    if (node.Output[0].LinkedIO != null)
                        statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

                    break;
                }
                case "CREATESTRING":
                {
                    // Create string name and assign to output
                    string stringName = "string_" + node.InstanceID.ToString();
                    node.Output[1].Value = stringName;

                    // The output item of this node is assigned to the node output
                    CodeVariableDeclarationStatement itemRef =
                        new CodeVariableDeclarationStatement(typeof(string),
                        stringName, node.Input[1].GetValue != null ? new CodeSnippetExpression(node.Input[1].GetValue) : null);

                    statements.Add(itemRef);

                        if (node.Output[0].LinkedIO != null)
                            statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
                        break;
                }
                case "CREATEVEC2":
                    {
                        // Create vector name and assign to output
                        string vecName = "vec2_" + node.InstanceID.ToString();
                        node.Output[1].Value = vecName;

                        // The output item of this node is assigned to the node output
                        CodeVariableDeclarationStatement itemRef =
                            new CodeVariableDeclarationStatement(typeof(Vector2),
                            vecName, new CodeSnippetExpression(string.Format("new Vector2({0}, {1})", node.Input[1].GetValue, node.Input[2].GetValue)));

                        statements.Add(itemRef);

                        if (node.Output[0].LinkedIO != null)
                            statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
                        break;
                    }
                case "CREATENUM":
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
                    break;
                }
                case "CREATENUMARRAY":
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

                    break;
                }
                case "CREATESTRINGARRAY":
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
                    break;
                }
                case "CREATEVEC2ARRAY":
                    {
                        string arrayName = "array_" + node.InstanceID.ToString();
                        node.Output[1].Value = arrayName;

                        // The output item of this node is assigned to the node output
                        CodeVariableDeclarationStatement arrayRef =
                            new CodeVariableDeclarationStatement(typeof(List<Vector2>),
                            arrayName, new CodeSnippetExpression("new List<Vector2>()"));

                        statements.Add(arrayRef);

                        if (node.Output[0].LinkedIO != null)
                            statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));
                        break;
                    }
                case "CREATEOBJARRAY":
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
                    break;
                }
            }

            return statements.ToArray();
        }

        /// <summary>
        /// Generate nodes that manipulate and create variables
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateVariables(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            switch (node.Label)
            {
                case "SETSTRING": case "SETNUM":
                {
                    // assigns a variable to the given attribute, both int and string use same value
                    CodeAssignStatement assignStatement =
                        new CodeAssignStatement
                        (new CodeArgumentReferenceExpression(node.Input[1].GetValue),
                        new CodeArgumentReferenceExpression(node.Input[2].GetValue));

                    node.Output[1].Value = node.Input[1].GetValue;

                    statements.Add(assignStatement);
                    break;
                }
                case "SETVEC2":
                {
                        statements.Add(new CodeSnippetStatement(string.Format("{0} = new Vector2({1}, {2});", 
                                    node.Input[1].GetValue, node.Input[2].GetValue, node.Input[3].GetValue)));

                        node.Output[1].Value = node.Input[3].GetValue;
                        break;
                }
                case "GETITEM":
                {
                    // Create item name and assign to output
                    string itemName = "item_" + node.InstanceID.ToString();
                    // access the item via the output
                    node.Output[1].Value = itemName;

                    // The output item of this node is assigned to the node output
                    CodeVariableDeclarationStatement itemRef =
                        new CodeVariableDeclarationStatement(node.Input[1].IOGetType,
                        itemName);

                    statements.Add(itemRef);

                    statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(itemName),
                                    new CodeSnippetExpression(string.Format("{0}[{1}]", node.Input[1].GetValue, node.Input[2].GetValue))));
                    break;
                }
                case "ADDITEM":
                {
                    statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(
                        string.Format("{0}.Add({1})", node.Input[1].GetValue,
                            node.Input[2].GetValue))));
                    break;
                }
                case "GETPOS":
                {
                    // Create item name and assign to output
                    string posName = "vec2_" + node.InstanceID.ToString();
                    // access the item via the output
                    node.Output[1].Value = posName;

                    // The output item of this node is assigned to the node output
                    CodeVariableDeclarationStatement posRef =
                        new CodeVariableDeclarationStatement(typeof(Vector2),
                        posName);

                    statements.Add(posRef);

                    statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(posName),
                                new CodeSnippetExpression(string.Format("{0}.Reference.position", 
                                node.Input[1].GetValue))));

                    break;
                }
                case "GETALIGN":
                {
                    // Create item name and assign to output
                    string alignName = "align_" + node.InstanceID.ToString();
                    // access the item via the output
                    node.Output[1].Value = alignName;

                    // The output item of this node is assigned to the node output
                    CodeVariableDeclarationStatement alignRef =
                        new CodeVariableDeclarationStatement(typeof(Alignment),
                        alignName);

                    statements.Add(alignRef);

                    statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(alignName),
                                new CodeSnippetExpression(string.Format("{0}.Alignment",
                                node.Input[1].GetValue))));

                    break;
                }
                case "VEC2DISTANCE":
                    {
                        // Create item name and assign to output
                        string distName = "distance_" + node.InstanceID.ToString();
                        // access the item via the output
                        node.Output[1].Value = distName;

                        // The output item of this node is assigned to the node output
                        CodeVariableDeclarationStatement distRef =
                            new CodeVariableDeclarationStatement(typeof(float),
                            distName);

                        statements.Add(distRef);

                        statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(distName),
                                    new CodeSnippetExpression(string.Format("UnityEngine.Vector2.Distance({0},{1})", 
                                    node.Input[1].GetValue, node.Input[2].GetValue))));

                        break;
                    }
            }

            if (node.Output[0].LinkedIO != null)
                statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

            return statements.ToArray();
        }

        /// <summary>
        /// Functions that make the ship perform an action
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateShipInteract(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            #region ERROR CHECKING

            bool linked = false;

            if (node.Input[0].LinkedIO != null)
                linked = true;

            // Error checking - all of these commands should only have one input 

            foreach(NodeData.IO io in node.Input)
            {
                if (io.Equals(node.Input[0]) || io.Type != NodeData.IO.IOType.LINK)
                    continue;

                if(io.LinkedIO != null)
                {
                    if (linked)
                    {
                        m_debugMsgs.Add(new DebugMessages()
                        {
                            NodeInstance = node.InstanceID.ToString(),
                            Message = "Ship interaction can can only have one input"
                        });
                        return null;
                    }
                    else
                        linked = true;
                }
            }
    
            #endregion

            switch (node.Label)
            {
                case "FORWARDENGINE":
                {
                    // define the action based on which node is
                    // linked
                    if (node.Input[0].LinkedIO != null)
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysPressed.Add(Control_Config.GetKey(\"moveUp\", \"ship\"))")));
                    else
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"moveUp\", \"ship\"));")));
                    break;
                }
                case "REVERSEENGINE":
                {
                    // define the action based on which node is
                    // linked
                    if (node.Input[0].LinkedIO != null)
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysPressed.Add(Control_Config.GetKey(\"moveDown\", \"ship\"));")));
                    else
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"moveDown\", \"ship\"));")));
                        break;
                }
                case "LEFTROTOR":
                {
                    // define the action based on which node is
                    // linked
                    if (node.Input[0].LinkedIO != null)
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysPressed.Add(Control_Config.GetKey(\"turnLeft\", \"ship\"))")));
                    else
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"turnLeft\", \"ship\"));")));

                        break;
                }
                case "RIGHTROTOR":
                {
                    // define the action based on which node is
                    // linked
                    if (node.Input[0].LinkedIO != null)
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysPressed.Add(Control_Config.GetKey(\"turnRight\", \"ship\"));")));
                    else
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"turnRight\", \"ship\"));")));
                        break;
                }
                case "FIRE":
                    {
                        // define the action based on which node is
                        // linked
                        if (node.Input[0].LinkedIO != null)
                            statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                                ("KeysPressed.Add(Control_Config.GetKey(\"fire\", \"ship\"));")));
                        else if (node.Input[1].LinkedIO != null)
                            statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                                ("KeysPressed.Add(Control_Config.GetKey(\"secondary\", \"ship\"));")));
                        else if (node.Input[2].LinkedIO != null)
                            statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                                ("KeysPressed.Add(Control_Config.GetKey(\"tertiary\", \"ship\"));")));
                        break;
                    }
                case "EJECT":
                    {
                        // define the action based on which node is
                        // linked
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysPressed.Add(Control_Config.GetKey(\"eject\", \"ship\"));")));
                        break;
                    }
                case "STOPMOVE":
                    {
                        // define the action based on which node is
                        // linked
                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"moveUp\", \"ship\"));")));

                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"moveDown\", \"ship\"));")));

                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"turnLeft\", \"ship\"));")));

                        statements.Add(new CodeExpressionStatement(new CodeSnippetExpression
                            ("KeysReleased.Add(Control_Config.GetKey(\"turnRight\", \"ship\"));")));

                        break;
                    }
                case "TURNTO":
                    {
                        // Create string name and assign to output
                        string resultName = "result_" + node.InstanceID.ToString();
                        node.Output[1].Value = resultName;

                        // The output item of this node is assigned to the node output
                        CodeVariableDeclarationStatement resultRef =
                            new CodeVariableDeclarationStatement(typeof(Boolean), resultName, 
                                new CodeMethodInvokeExpression(
                                    new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "TurnTo"), 
                                            new CodeExpression[] { new CodeVariableReferenceExpression(node.Input[1].GetValue) }));

                        statements.Add(resultRef);
                        break;
                    }
            }

            if (node.Output[0].LinkedIO != null)
                statements.AddRange(GenerateNode(node.Output[0].LinkedIO.Node));

            return statements.ToArray();
        }

        /// <summary>
        /// Retrieves attributes relating to the object
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CodeStatement[] GenerateAgentAccessor(NodeData node)
        {
            List<CodeStatement> statements = new List<CodeStatement>();

            /*switch(node.Label)
            {

            }*/

            return statements.ToArray();
        }


        #endregion

        /// <summary>
        /// Build 
        /// </summary>
        /// <returns></returns>
        private Assembly CompileScript()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider(
                new Dictionary<String, String> { { "CompilerVersion", "v3.5" } });
            ICodeCompiler codeCompiler = codeProvider.CreateCompiler();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add(typeof(ICustomScript).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(KeyCode).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Vector2).Assembly.Location);

            CompilerResults results = codeCompiler.CompileAssemblyFromDom(parameters, m_compileUnit);
            if (results.Errors.HasErrors)
            {
                Debug.Log("Error Count: " + results.Errors.Count.ToString());
                for (int x = 0; x < results.Errors.Count; x++)
                {
                    Debug.Log(string.Format("Warning? {0} - Line: {1} - {2}",
                        results.Errors[x].IsWarning, results.Errors[x].Line.ToString(),
                        results.Errors[x].ErrorText));
                }

                return null;
            }

            return results.CompiledAssembly;
        }

        #endregion
    }
}