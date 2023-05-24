using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    /*
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant*/
    T_Int, T_Float, T_String, T_Read, T_Write, T_Repeat, T_Until, T_If, T_Elseif, T_Else, T_Then, T_Return, T_Endl, T_End, T_Number, T_CommentStatement,
    T_Identifier, T_PlusOp, T_MinusOp, T_MultiplyOp, T_DivideOp, T_EqualOp, T_NotEqualOp, T_AssignOp, T_StringT, T_GreaterThanOp, T_LessThanOp,
    T_And, T_Or, T_Semicolon, T_Comma, T_LParanthesis, T_RParanthesis, T_LBraces, T_RBraces, T_Main

}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        string splitters = "+-–*/=:><&|;(){}, \r\n";
        string all_operators = "+-–*/=:><&|!@#$%^_~?'";
        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.T_Int);
            ReservedWords.Add("float", Token_Class.T_Float);
            ReservedWords.Add("string", Token_Class.T_String);
            ReservedWords.Add("read", Token_Class.T_Read);
            ReservedWords.Add("write", Token_Class.T_Write);
            ReservedWords.Add("repeat", Token_Class.T_Repeat);
            ReservedWords.Add("until", Token_Class.T_Until);
            ReservedWords.Add("if", Token_Class.T_If);
            ReservedWords.Add("elseif", Token_Class.T_Elseif);
            ReservedWords.Add("else", Token_Class.T_Else);
            ReservedWords.Add("then", Token_Class.T_Then);
            ReservedWords.Add("return", Token_Class.T_Return);
            ReservedWords.Add("endl", Token_Class.T_Endl);
            ReservedWords.Add("end", Token_Class.T_End);


            Operators.Add("=", Token_Class.T_EqualOp);
            Operators.Add("<", Token_Class.T_LessThanOp);
            Operators.Add(">", Token_Class.T_GreaterThanOp);
            Operators.Add("<>", Token_Class.T_NotEqualOp);
            Operators.Add("+", Token_Class.T_PlusOp);
            Operators.Add("-", Token_Class.T_MinusOp);
            Operators.Add("–", Token_Class.T_MinusOp);
            Operators.Add("*", Token_Class.T_MultiplyOp);
            Operators.Add("/", Token_Class.T_DivideOp);
            Operators.Add(":=", Token_Class.T_AssignOp);
            Operators.Add("&&", Token_Class.T_And);
            Operators.Add("||", Token_Class.T_Or);
            Operators.Add(",", Token_Class.T_Comma);
            Operators.Add(";", Token_Class.T_Semicolon);

            Operators.Add("(", Token_Class.T_LParanthesis);
            Operators.Add(")", Token_Class.T_RParanthesis);

            Operators.Add("{", Token_Class.T_LBraces);
            Operators.Add("}", Token_Class.T_RBraces);

        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                string CurrentLexeme = "";
                if (SourceCode[i] == ' ' || SourceCode[i] == '\r' || SourceCode[i] == '\n')
                    continue;
                //number , reserved word , identifier
                if ((SourceCode[i] >= 'A' && SourceCode[i] <= 'z') || (SourceCode[i] >= '0' && SourceCode[i] <= '9') || (SourceCode[i] == '.')) //if you read a character
                {
                    while (!splitters.Contains(SourceCode[i]))
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                        if (i >= SourceCode.Length) break;
                    }
                    i--;
                    FindTokenClass(CurrentLexeme);

                }
                //string value ex:"1212 sdsdsd1 212"
                else if (SourceCode[i] == '"')
                {
                    i++;
                    CurrentLexeme += "\"";
                    while (true)
                    {
                        if (SourceCode[i] == '\n')
                            break;
                        if (SourceCode[i] == '"')
                        {
                            CurrentLexeme += "\"";
                            break;
                        }
                        CurrentLexeme += SourceCode[i];
                        i++;
                        if (i >= SourceCode.Length) break;
                    }
                    FindTokenClass(CurrentLexeme);
                }
                //comment /* s545 454 */
                else if (SourceCode[i] == '/' && SourceCode[i + 1] == '*' && i < SourceCode.Length - 1)
                {
                    i += 2;
                    CurrentLexeme += "/*";
                    while (true)
                    {
                        if (i == SourceCode.Length)
                            break;
                        if (SourceCode[i] == '*' && SourceCode[i + 1] == '/')
                        {
                            CurrentLexeme += "*/";
                            i++;
                            break;
                        }
                        if (SourceCode[i] == '/' && SourceCode[i + 1] == '*')
                        {
                            i--;
                            break;
                        }
                        CurrentLexeme += SourceCode[i];
                        i++;
                        if (i >= SourceCode.Length) break;
                    }
                }
                else if (all_operators.Contains(SourceCode[i]))
                {
                    while (all_operators.Contains(SourceCode[i]))
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                        if (i >= SourceCode.Length) break;
                    }
                    i--;
                    FindTokenClass(CurrentLexeme);
                }
                else
                {
                    CurrentLexeme += SourceCode[i];
                    FindTokenClass(CurrentLexeme);
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                if (Lex.Equals("main"))
                    Tok.token_type = Token_Class.T_Main;
                else
                    Tok.token_type = Token_Class.T_Identifier;
                Tok.lex = Lex;
                Tokens.Add(Tok);
            }
            //Is it a Number?
            else if (isNumber(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.T_Number;
                Tokens.Add(Tok);
            }
            //Is it a String ?
            else if (isString(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.T_StringT;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }



        bool isIdentifier(string lex)
        {
            bool isValid = false;
            // Check if the lex is an identifier or not.
            var s = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9])*$", RegexOptions.Compiled);
            if (s.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
        bool isNumber(string lex)
        {

            bool isValid = false;
            var s = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            // Check if the lex is a constant (Number) or not.
            if (s.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
        bool isString(string lex)
        {
            bool isValid = false;
            //var s = new Regex(@"\“([a-z]+|[A-Z]+|Number+|\*|\+|\.|\-)*\”", RegexOptions.Compiled);
            //Regex s = new Regex("^\"[^\"]*\"$", RegexOptions.Compiled);
            Regex s = new Regex("^\"(.*)\"$", RegexOptions.Compiled);
            if (s.IsMatch(lex))
            {
                isValid = true;
            }

            return isValid;
        }
    }
}

