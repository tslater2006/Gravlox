using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    internal class Scanner
    {
        private readonly string Source;
        private readonly List<Token> Tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private static readonly Dictionary<string, TokenType> Keywords;

        static Scanner() {
            Keywords = new Dictionary<string, TokenType>();
            Keywords.Add("and", TokenType.AND);
            Keywords.Add("class", TokenType.CLASS);
            Keywords.Add("else", TokenType.ELSE);
            Keywords.Add("false", TokenType.FALSE);
            Keywords.Add("for", TokenType.FOR);
            Keywords.Add("fun", TokenType.FUN);
            Keywords.Add("if", TokenType.IF);
            Keywords.Add("nil", TokenType.NIL);
            Keywords.Add("or", TokenType.OR);
            Keywords.Add("print", TokenType.PRINT);
            Keywords.Add("return", TokenType.RETURN);
            Keywords.Add("super", TokenType.SUPER);
            Keywords.Add("this", TokenType.THIS);
            Keywords.Add("true", TokenType.TRUE);
            Keywords.Add("var", TokenType.VAR);
            Keywords.Add("while", TokenType.WHILE);
        }

        internal Scanner(string src)
        {
            Source = src;
        }

        internal List<Token> ScanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, line));
            return Tokens;
        }

        private bool isAtEnd()
        {
            return current >= Source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        /* A comment goes until the end of the line. */
                        while (Peek() != '\n' && !isAtEnd())
                        {
                            Advance();
                        }
                    } else if (Match('*'))
                    {
                        /* C-Style block comment */
                        Advance();

                        while (Peek() != '*' && PeekNext() != '/')
                        {
                            Advance();
                        }

                        /* Eat the closing star and slash */
                        Advance();
                        Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    /* Ignore whitespace */
                    break;
                case '\n':
                    line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void Identifier()
        {

            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source.Substring(start, current - start);
            
            if (Keywords.ContainsKey(text))
            {
                AddToken(Keywords[text]);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER);
            }

        }

        private void Number()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                /* Consume the . */
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }

            }

            AddToken(TokenType.NUMBER, double.Parse(Source.Substring(start, current - start)));

        }

        private void String()
        {
            while(Peek() != '"' && !isAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }
                Advance();
            }

            if (isAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            /* The closing quote */
            Advance();

            string value = Source.Substring(start + 1, current - 1 - start - 1);
            AddToken(TokenType.STRING, value);
        }

        private bool Match(char expected)
        {
            if (isAtEnd())
            {
                return false;
            }
            if (Source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        private char Peek()
        {
            if (current >= Source.Length)
            {
                return '\0';
            }

            return Source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= Source.Length) return '\0';
            return Source[current + 1];
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private char Advance()
        {
            current++;
            return Source[current-1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = Source.Substring(start, current - start);
            Tokens.Add(new Token(type, text, literal, line));
        }

    }
}
