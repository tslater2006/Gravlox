using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravlox
{
    class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        private LoxEnvironment environment = new LoxEnvironment();

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

        internal void interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    execute(statement);
                }
            } catch (RuntimeError error)
            {
                Lox.runtimeError(error);
            }
        }
        private void execute(Stmt stmt)
        {
            stmt.accept(this);
        }

        public object visitBlockStmt(Stmt.Block stmt)
        {
            executeBlock(stmt.statements, new LoxEnvironment(environment));
            return null;
        }

        void executeBlock(List<Stmt> statements, LoxEnvironment environment)
        {
            LoxEnvironment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Stmt statement in statements)
                {
                    execute(statement);
                }
            } finally
            {
                this.environment = previous;
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

        /* Statements */
        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            Object value = evaluate(stmt.expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        public object visitVarStmt(Stmt.Var stmt)
        {
            Object value = null;
            if (stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.Lexeme, value);

            return null;
        }

        public object visitAssignExpr(Expr.Assign expr)
        {
            Object value = evaluate(expr.value);

            environment.Assign(expr.name, value);
            return value;
        }

        public object visitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.name);
        }
    }
}
