﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAST
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> expressions = new List<string>()
            {
                "Assign   : Token name, Expr value",
                "Binary   : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                "Unary    : Token Operator, Expr Right",
                "Variable : Token name"
            };
            var outputDir = Directory.GetCurrentDirectory();

            DefineAST(outputDir, "Expr", expressions);

            List<string> statements = new List<string>()
            {
                "Block      : List<Stmt> statements",
                "Expression : Expr expression",
                "Print      : Expr expression",
                "Var        : Token name, Expr initializer"
            };

            /* Statements */
            DefineAST(outputDir, "Stmt", statements);
        }

        private static void DefineAST(string outDir, string baseName, List<string> types)
        {
            using (var fs = new StreamWriter(outDir + Path.DirectorySeparatorChar + baseName + ".cs"))
            {
                fs.WriteLine("using System.Collections.Generic;");
                fs.WriteLine("");
                fs.WriteLine("namespace Gravlox");
                fs.WriteLine("{");
                fs.WriteLine("    abstract class " + baseName + "{");

                DefineVisitor(fs, baseName, types);

                foreach( var type in types)
                {
                    fs.WriteLine("");
                    var parts = type.Split(':');
                    var className = parts[0].Trim();
                    var fields = parts[1].Trim();
                    DefineType(fs, baseName, className, fields);
                }
                fs.WriteLine("");
                fs.WriteLine("    internal abstract T accept<T>(Visitor<T> visitor);");
                fs.WriteLine("    }");
                fs.WriteLine("}");
            }
        }

        private static void DefineVisitor(StreamWriter fs, string baseName, List<string> types)
        {
            fs.WriteLine("    internal interface Visitor<T> {");
            foreach(string type in types)
            {
                var typeName = type.Split(':')[0].Trim();
                fs.WriteLine("        T visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
            }

            fs.WriteLine("    }");
        }

        private static void DefineType(StreamWriter fs, string baseName, string className, string fieldList)
        {
            fs.WriteLine($"    internal class {className} : {baseName}");
            fs.WriteLine("    {");

            fs.WriteLine($"        internal {className}({fieldList})");
            fs.WriteLine("        {");

            var fields = fieldList.Split(',');
            foreach (var field in fields)
            {
                var fieldName = field.Trim().Split(' ')[1];
                fs.WriteLine($"            this.{fieldName} = {fieldName};");
            }

            fs.WriteLine("        }");

            fs.WriteLine("");
            fs.WriteLine("    override internal T accept<T> (Visitor<T> visitor) {");
            fs.WriteLine("        return visitor.visit" + className + baseName + "(this);");
            fs.WriteLine("    }");

            foreach (var field in fields)
            {
                fs.WriteLine($"        internal readonly {field.Trim()};");
            }
            fs.WriteLine("    }");

        }
    }
}
