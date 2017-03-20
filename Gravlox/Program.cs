﻿using System;
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

            // For now, just print the tokens.
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        internal static void Error(int line, String message)
        {
            Report(line, "", message);
        }

        private static void Report (int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            HadError = true;
        }
    }
}
