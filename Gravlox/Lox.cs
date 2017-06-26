using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    class Lox
    {
        static bool HadError;
        static bool HadRuntimeError;
        private static readonly Interpreter interpreter = new Interpreter();

        static void Main(string[] args)
        {
            /* Runs the AstPrinter main from end of chapter 5 */
            //AstPrinter.Ast_Main(args);

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: gravlox [script]");
            } else if (args.Length == 1)
            {
                RunFile(args[0]);
            } else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string file)
        {
            var scriptText = File.ReadAllText(file);
            Run(scriptText);

            if (HadError)
            {
                Environment.Exit(65);
            }

            if (HadRuntimeError)
            {
                Environment.Exit(70);
            }

        }

        private static void RunPrompt()
        {
            for(;;)
            {
                Console.Write("> ");
                Run(Console.ReadLine());
            }
        }

        private static void Run(String source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            Expr expression = parser.Parse();
            if (HadError)
            {
                return;
            }

            interpreter.interpret(expression);

        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void Error(Token token, String message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, "at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        internal static void runtimeError(RuntimeError error)
        {
            Console.WriteLine(error.Message + "\r\n[line " + error.Token.Line + "]");
            HadRuntimeError = true;
        }

        private static void Report (int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            HadError = true;
        }
    }
}
