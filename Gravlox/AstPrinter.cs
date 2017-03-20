using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    class AstPrinter : Expr.Visitor<string> 
    {
        public string visitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string visitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string visitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value.ToString();
        }

        public string visitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        string Print(Expr expr)
        {
            return expr.accept(this);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }

        public static void Ast_Main(string[] args)
        {
            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Gravlox.Token(TokenType.MINUS, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.STAR, "*", null, 1),
                new Expr.Grouping(
                    new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));
        }
    }
}
