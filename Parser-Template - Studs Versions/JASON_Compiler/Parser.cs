using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            return root;
        }

        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }




        Node Program()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node program = new Node("program");
                program.Children.Add(Function_Statements());
                Node Tmain = Main_Function();
                if (Tmain == null)
                {
                    Errors.Error_List.Add("Parsing Error: Expected  main \r\n");
                }
                else { 
                program.Children.Add(Tmain);
                }
                return program;
            }
            else return null;

        }

        Node Main_Function()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node main_Function = new Node("main_Function");
                main_Function.Children.Add(Datatype());
                main_Function.Children.Add(match(Token_Class.T_Main));
                main_Function.Children.Add(match(Token_Class.T_LParanthesis));
                main_Function.Children.Add(match(Token_Class.T_RParanthesis));
                main_Function.Children.Add(Function_Body());
                return main_Function;
            }
            else return null;

        }

        Node Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node statement = new Node("statement");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Write)
                {
                    statement.Children.Add(Write_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Read)
                {
                    statement.Children.Add(Read_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
                {
                    statement.Children.Add(Assignment_Statement());
                }
                else if (isDataType(InputPointer))
                {
                    statement.Children.Add(Declaration_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Repeat)
                {
                    statement.Children.Add(Repeat_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_If)
                {
                    statement.Children.Add(If_Statement());
                }
                return statement;
            }
            else return null;

        }


        bool is_statement(int i)
        {
            if (InputPointer < TokenStream.Count)
            {
                bool is_statement = false;
                if (TokenStream[i].token_type == Token_Class.T_Write)
                {
                    is_statement = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_Read)
                {
                    is_statement = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_Identifier)
                {
                    is_statement = true;
                }
                else if (isDataType(i))
                {
                    is_statement = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_Repeat)
                {
                    is_statement = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_If)
                {
                    is_statement = true;
                }
                return is_statement;
            }
            else return false;


        }
        bool isDataType(int i)
        {
            if (InputPointer < TokenStream.Count)
            {
                bool isdataType = false;
                if (TokenStream[i].token_type == Token_Class.T_Int)
                {
                    isdataType = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_Float)
                {
                    isdataType = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_String)
                {
                    isdataType = true;
                }
                return isdataType;
            }
            else return false;
        }
        bool isTerm(int i)
        {
            if (InputPointer < TokenStream.Count)
            {
                bool isterm = false;
                if (TokenStream[i].token_type == Token_Class.T_Number)
                {
                    isterm = true;
                }else if (TokenStream[i].token_type == Token_Class.T_Identifier && TokenStream[i + 1].token_type == Token_Class.T_LParanthesis)
                {
                    isterm = true;
                }
                else if (TokenStream[i].token_type == Token_Class.T_Identifier)
                {
                    isterm = true;
                    
                }
                return isterm;
            }
            else return false;

        }
        Node Statements()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node statements = new Node("statements");
                if (is_statement(InputPointer))
                {
                    statements.Children.Add(Statement());
                    statements.Children.Add(Statements());
                    return statements;
                }
                else return null;
            }
            else return null;
        }

        Node Function_Statements()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Statement = new Node("function_Statement");
                if (isDataType(InputPointer) && (TokenStream[InputPointer + 1].token_type == Token_Class.T_Identifier))
                {
                    function_Statement.Children.Add(Function_Statement());
                    function_Statement.Children.Add(Function_Statements());
                    return function_Statement;
                }
                else return null;
            }
            else return null;

        }


        Node Function_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Statement = new Node("function_Statement");
                function_Statement.Children.Add(Function_Declaration());
                function_Statement.Children.Add(Function_Body());
                return function_Statement;
            }
            else return null;

        }

        Node Function_Body()    
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Body = new Node("function_Body");
                function_Body.Children.Add(match(Token_Class.T_LBraces));
                function_Body.Children.Add(Statements());
                function_Body.Children.Add(Return_Statement());
                function_Body.Children.Add(match(Token_Class.T_RBraces));
                return function_Body;
            }
            else return null;

        }


        Node Return_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node return_Statement = new Node("return_Statement");
                return_Statement.Children.Add(match(Token_Class.T_Return));
                return_Statement.Children.Add(Expression());
                return_Statement.Children.Add(match(Token_Class.T_Semicolon));
                return return_Statement;
            }
            else return null;

        }
        

        Node Function_Declaration()  
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Declaration = new Node("function_Declaration");
                function_Declaration.Children.Add(Datatype());
                function_Declaration.Children.Add(match(Token_Class.T_Identifier));
                function_Declaration.Children.Add(match(Token_Class.T_LParanthesis));
                function_Declaration.Children.Add(Function_Parameters_Dec());
                function_Declaration.Children.Add(match(Token_Class.T_RParanthesis));
                return function_Declaration;
            }
           else return null;

        }

        Node Parameter_Dec()   
        {
            if (InputPointer < TokenStream.Count)
            {
                Node parameter_Dec = new Node("parameter_Dec");
                parameter_Dec.Children.Add(Datatype());
                parameter_Dec.Children.Add(match(Token_Class.T_Identifier));
                return parameter_Dec;
            }
            else return null;

        }

        Node Function_Parameters_Dec()  
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Parameters_Dec = new Node("function_Parameters_Dec");
                if (isDataType(InputPointer))
                {
                    function_Parameters_Dec.Children.Add(Parameter_Dec());
                    function_Parameters_Dec.Children.Add(Parameters_Dec());
                    return function_Parameters_Dec;
                }
                else
                    return null;
            }
            else return null;
        }

        Node Parameters_Dec()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node parameters_Dec = new Node("parameters_Dec");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
                {
                    parameters_Dec.Children.Add(match(Token_Class.T_Comma));
                    parameters_Dec.Children.Add(Parameter_Dec());
                    parameters_Dec.Children.Add(Parameters_Dec());
                    return parameters_Dec;
                }
                else
                    return null;
            }
            else return null;

        }

        Node Function_Call()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Call = new Node("function_Call");
                function_Call.Children.Add(match(Token_Class.T_Identifier));
                function_Call.Children.Add(match(Token_Class.T_LParanthesis));
                function_Call.Children.Add(Function_Parameters_Call());
                function_Call.Children.Add(match(Token_Class.T_RParanthesis));
                return function_Call;
            }
            else return null;

        }

        Node Function_Parameters_Call()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node function_Parameters_Dec = new Node("function_Parameters_Dec");
                if (isTerm(InputPointer))
                {
                    function_Parameters_Dec.Children.Add(Term());
                    function_Parameters_Dec.Children.Add(Parameters_Call());
                    return function_Parameters_Dec;
                }else
                {
                    return null;
                }
               
            }
            else return null;

        }

        Node Parameters_Call()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node parameters_Call = new Node("parameters_Call");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
                {
                    parameters_Call.Children.Add(match(Token_Class.T_Comma));
                    parameters_Call.Children.Add(Term());
                    parameters_Call.Children.Add(Parameters_Call());
                    return parameters_Call;
                }
                else
                    return null;

            }
            else return null;
        }

        Node Term()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node term = new Node("term");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Number)
                {
                    term.Children.Add(match(Token_Class.T_Number));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.T_LParanthesis)
                {
                    term.Children.Add(Function_Call());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
                {
                    term.Children.Add(match(Token_Class.T_Identifier));
                }
                return term;
            }
            else return null;
        }

        Node Arithmetic_Operator()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node arithmetic_Operator = new Node("arithmetic_Operator");
                if (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp)
                {
                    arithmetic_Operator.Children.Add(match(Token_Class.T_PlusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_MinusOp)
                {
                    arithmetic_Operator.Children.Add(match(Token_Class.T_MinusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp)
                {
                    arithmetic_Operator.Children.Add(match(Token_Class.T_MultiplyOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
                {
                    arithmetic_Operator.Children.Add(match(Token_Class.T_DivideOp));
                }
                return arithmetic_Operator;
            }
            else return null;
        }

         Node Equation()
         {
            if (InputPointer < TokenStream.Count)
            {
                Node Equation = new Node("Equation");
                if (isTerm(InputPointer))
                {
                    Equation.Children.Add(TermAndOp());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_LParanthesis)
                {
                    Equation.Children.Add(BracketEquation());
                }
                return Equation;
            }
            else return null;
         }

        
        Node TermAndOp()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node TermAndOp = new Node("TermAndOp");
                TermAndOp.Children.Add(TermOpOnly());
                TermAndOp.Children.Add(TermAndOpd());
                TermAndOp.Children.Add(SubEquation());
                return TermAndOp;
            }
            else return null;
        }

        Node TermOpOnly()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node TermOpOnly = new Node("TermOpOnly");
                TermOpOnly.Children.Add(Term());
                TermOpOnly.Children.Add(Arithmetic_Operator());
                return TermOpOnly;
            }
            else return null;

        }

        Node TermAndOpd()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node TermAndOpd = new Node("TermAndOpd");
                if (isTerm(InputPointer))
                {
                    TermAndOpd.Children.Add(Term());
                }
                else
                    TermAndOpd.Children.Add(BracketEquation());
                return TermAndOpd;
            }
            else return null;
        }

        Node BracketEquation()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node BracketEquation = new Node("BracketEquation");
                BracketEquation.Children.Add(match(Token_Class.T_LParanthesis));
                BracketEquation.Children.Add(TermAndOp());
                BracketEquation.Children.Add(match(Token_Class.T_RParanthesis));
                BracketEquation.Children.Add(SubEquation());
                return BracketEquation;
            }
            else return null;
        }
       
        Node SubEquation()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node subEquation = new Node("SubEquation");
                if (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp || TokenStream[InputPointer].token_type == Token_Class.T_MinusOp || TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
                {
                    subEquation.Children.Add(SubEquationd());
                    subEquation.Children.Add(SubEquation());
                    return subEquation;
                }
                else return null;
            }
            else return null;

        }

        Node SubEquationd()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node subEquationd = new Node("SubEquation");
                subEquationd.Children.Add(Arithmetic_Operator());
                subEquationd.Children.Add(TermAndOpd());
                return subEquationd;
            }
            else return null;
        }

        Node Expression()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node expression = new Node("expression");
                if (TokenStream[InputPointer].token_type == Token_Class.T_StringT)
                {
                    expression.Children.Add(match(Token_Class.T_StringT));
                }
                else if (((TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Identifier) && (TokenStream[InputPointer + 1].token_type == Token_Class.T_PlusOp || TokenStream[InputPointer + 1].token_type == Token_Class.T_MinusOp || TokenStream[InputPointer + 1].token_type == Token_Class.T_MultiplyOp || TokenStream[InputPointer + 1].token_type == Token_Class.T_DivideOp)) || TokenStream[InputPointer].token_type == Token_Class.T_LParanthesis)
                {
                    expression.Children.Add(Equation());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.T_LParanthesis)
                {
                    expression.Children.Add(Function_Call());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
                {
                    expression.Children.Add(Term());
                }
                return expression;
            }else return null;
        }

        Node Assignment_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node assingment_Statemrnt = new Node("assingment_Statemrnt");
                assingment_Statemrnt.Children.Add(match(Token_Class.T_Identifier));
                assingment_Statemrnt.Children.Add(match(Token_Class.T_AssignOp));
                assingment_Statemrnt.Children.Add(Expression());
                assingment_Statemrnt.Children.Add(match(Token_Class.T_Semicolon));
                return assingment_Statemrnt;
            }
            else return null;
        }
        Node Assignment_Statementd() //for declaration statement
        {
            if (InputPointer < TokenStream.Count)
            {
                Node assingment_Statemrnt = new Node("assingment_Statemrnt");
                assingment_Statemrnt.Children.Add(match(Token_Class.T_Identifier));
                assingment_Statemrnt.Children.Add(match(Token_Class.T_AssignOp));
                assingment_Statemrnt.Children.Add(Expression());
                return assingment_Statemrnt;
            }
            else return null;
        }
        Node Datatype()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node datatype = new Node("datatype");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Int)
                {
                    datatype.Children.Add(match(Token_Class.T_Int));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Float)
                {
                    datatype.Children.Add(match(Token_Class.T_Float));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_String)
                {
                    datatype.Children.Add(match(Token_Class.T_String));
                }
                return datatype;
            }
            else return null;
        }

        Node Declaration_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node declaration_Statement = new Node("declaration_Statement");
                declaration_Statement.Children.Add(Datatype());
                declaration_Statement.Children.Add(Declaration());
                declaration_Statement.Children.Add(match(Token_Class.T_Semicolon));
                return declaration_Statement;
            }
            else return null;
        }

        Node Declaration()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node declaration = new Node("declaration");
                declaration.Children.Add(DeclarationTerm());
                declaration.Children.Add(Declarations());
                return declaration;
            }
            else return null;
        }
        Node DeclarationTerm()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node declarationTerm = new Node("declarationTerm");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.T_AssignOp)
                {
                    declarationTerm.Children.Add(Assignment_Statementd());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
                {
                    declarationTerm.Children.Add(match(Token_Class.T_Identifier));
                }

                return declarationTerm;
            }
            else return null;


        }


        Node Declarations()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node declarations = new Node("declarations");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
                {
                    declarations.Children.Add(match(Token_Class.T_Comma));
                    declarations.Children.Add(DeclarationTerm());
                    declarations.Children.Add(Declarations());
                    return declarations;
                }
                else return null;
            }else return null;
        }

        Node Condition_Operator()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node condition_Operator = new Node("condition_Operator");
                if (TokenStream[InputPointer].token_type == Token_Class.T_LessThanOp)
                {
                    condition_Operator.Children.Add(match(Token_Class.T_LessThanOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_GreaterThanOp)
                {
                    condition_Operator.Children.Add(match(Token_Class.T_GreaterThanOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_EqualOp)
                {
                    condition_Operator.Children.Add(match(Token_Class.T_EqualOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_NotEqualOp)
                {
                    condition_Operator.Children.Add(match(Token_Class.T_NotEqualOp));
                }
                return condition_Operator;
            }
            else return null;
        }

        Node Condition()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node condition = new Node("condition");
                condition.Children.Add(match(Token_Class.T_Identifier));
                condition.Children.Add(Condition_Operator());
                condition.Children.Add(Term());
                return condition;
            }
            else return null;

        }

        Node Boolean_Operator()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node boolean_Operator = new Node("boolean_Operator");
                if (TokenStream[InputPointer].token_type == Token_Class.T_And)
                {
                    boolean_Operator.Children.Add(match(Token_Class.T_And));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Or)
                {
                    boolean_Operator.Children.Add(match(Token_Class.T_Or));
                }
                return boolean_Operator;
            }
            else return null;
        }

        Node Condition_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node condition_Statement = new Node("condition_Statement");
                condition_Statement.Children.Add(Condition());
                condition_Statement.Children.Add(Condition_Statement_inside());
                return condition_Statement;
            }
            else return null;
        }

        Node Condition_Statement_inside()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node condition_Statement_inside = new Node("condition_Statement_inside");
                if (TokenStream[InputPointer].token_type == Token_Class.T_And || TokenStream[InputPointer].token_type == Token_Class.T_Or)
                {
                    condition_Statement_inside.Children.Add(Boolean_Operator());
                    condition_Statement_inside.Children.Add(Condition());
                    condition_Statement_inside.Children.Add(Condition_Statement_inside());
                    return condition_Statement_inside;
                }
                else return null;
            }
            else return null;

        }

        Node If_Statement()   
        {
            if (InputPointer < TokenStream.Count)
            {
                Node if_statement = new Node("if_statement");
                if_statement.Children.Add(match(Token_Class.T_If));
                if_statement.Children.Add(Condition_Statement());
                if_statement.Children.Add(match(Token_Class.T_Then));
                if_statement.Children.Add(Statements());
                if_statement.Children.Add(Else_Statements());
                return if_statement;
            }
            else return null;
        }

        Node Else_If_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node else_If_Statement = new Node("else_If_Statement");
                else_If_Statement.Children.Add(match(Token_Class.T_Elseif));
                else_If_Statement.Children.Add(Condition_Statement());
                else_If_Statement.Children.Add(match(Token_Class.T_Then));
                else_If_Statement.Children.Add(Statements());
                else_If_Statement.Children.Add(Else_Statements());
                return else_If_Statement;
            }else return null;



        }

        Node Else_Statements()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node else_Statements = new Node("else_Statements");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Elseif)
                {
                    else_Statements.Children.Add(Else_If_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.T_Else)
                {
                    else_Statements.Children.Add(Else_Statement());
                }
                else
                {
                    else_Statements.Children.Add(match(Token_Class.T_End));
                }
                return else_Statements;
            }
            else return null;
        }

        Node Else_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node else_Statement = new Node("else_Statement");
                else_Statement.Children.Add(match(Token_Class.T_Else));
                else_Statement.Children.Add(Statements());
                else_Statement.Children.Add(match(Token_Class.T_End));
                return else_Statement;
            }
            else return null;
        }

        Node Repeat_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node repeat_Statement = new Node("repeat_Statement");
                repeat_Statement.Children.Add(match(Token_Class.T_Repeat));
                repeat_Statement.Children.Add(Statements());
                repeat_Statement.Children.Add(match(Token_Class.T_Until));
                repeat_Statement.Children.Add(Condition_Statement());
                return repeat_Statement;
            }else return null;
        }
        Node Read_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node read_statement = new Node("read statement");
                read_statement.Children.Add(match(Token_Class.T_Read));
                read_statement.Children.Add(match(Token_Class.T_Identifier));
                read_statement.Children.Add(match(Token_Class.T_Semicolon));
                return read_statement;
            }
            else return null;

        }

        Node Write_Statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node write_Statement = new Node("write_Statement");
                write_Statement.Children.Add(match(Token_Class.T_Write));
                write_Statement.Children.Add(Write_Term());
                write_Statement.Children.Add(match(Token_Class.T_Semicolon));
                return write_Statement;
            }
            else return null;
        }
        Node Write_Term()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node write_Term = new Node("write_Term");
                if (TokenStream[InputPointer].token_type == Token_Class.T_Endl)
                {
                    write_Term.Children.Add(match(Token_Class.T_Endl));
                }
                else
                {
                    write_Term.Children.Add(Expression());
                }
                return write_Term;
            }
            else return null;

        }

    }
}