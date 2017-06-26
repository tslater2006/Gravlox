using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    class Interpreter : Expr.Visitor<Object>
    {
        public object visitBinaryExpr(Expr.Binary expr)
        {
            object left = evaluate(expr.Left);
            object right = evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.GREATER:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.MINUS:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is Double && right is Double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is String && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeError(expr.Operator, "Operans must be two numbers or two strings.");

                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);

            }

            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.Expression);
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            object right = evaluate(expr.Right);
            switch (expr.Operator.Type)
            {
                case TokenType.BANG:
                    return !isTruthy(right);
                case TokenType.MINUS:
                    checkNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            return null;
        }

        internal void interpret(Expr expression)
        {
            try
            {
                object value = evaluate(expression);
                Console.WriteLine(stringify(value));
            } catch (RuntimeError error)
            {
                Lox.runtimeError(error);
            }
        }

        private string stringify(object obj)
        {
            if (obj == null)
            {
                return "nil";
            }

            if (obj is double)
            {
                string text = obj.ToString();
                return text;
            }

            return obj.ToString();
        }

        private object evaluate(Expr expr)
        {
            return expr.accept(this);
        }

        private Boolean isTruthy(Object obj)
        {
            if (obj == null)
            {
                return false;
            } else if (obj is Boolean)
            {
                return (Boolean)obj;
            } else
            {
                return true;
            }
        }

        private Boolean isEqual(Object a, Object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private void checkNumberOperand(Token opr, object operand)
        {
            if (operand is Double) return;
            throw new RuntimeError(opr, "Operand must be a number.");
        }

        private void checkNumberOperands(Token opr, object left, object right)
        {
            if (left is Double && right is Double) return;
            throw new RuntimeError(opr, "Operands must be a number.");
        }
    }
}
