using System.Collections.Generic;

namespace Gravlox
{
    abstract class Expr{
    internal interface Visitor<T> {
        T visitAssignExpr(Assign expr);
        T visitBinaryExpr(Binary expr);
        T visitGroupingExpr(Grouping expr);
        T visitLiteralExpr(Literal expr);
        T visitUnaryExpr(Unary expr);
        T visitVariableExpr(Variable expr);
    }

    internal class Assign : Expr
    {
        internal Assign(Token name, Expr value)
        {
            this.name = name;
            this.value = value;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitAssignExpr(this);
    }
        internal readonly Token name;
        internal readonly Expr value;
    }

    internal class Binary : Expr
    {
        internal Binary(Expr Left, Token Operator, Expr Right)
        {
            this.Left = Left;
            this.Operator = Operator;
            this.Right = Right;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitBinaryExpr(this);
    }
        internal readonly Expr Left;
        internal readonly Token Operator;
        internal readonly Expr Right;
    }

    internal class Grouping : Expr
    {
        internal Grouping(Expr Expression)
        {
            this.Expression = Expression;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitGroupingExpr(this);
    }
        internal readonly Expr Expression;
    }

    internal class Literal : Expr
    {
        internal Literal(object Value)
        {
            this.Value = Value;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitLiteralExpr(this);
    }
        internal readonly object Value;
    }

    internal class Unary : Expr
    {
        internal Unary(Token Operator, Expr Right)
        {
            this.Operator = Operator;
            this.Right = Right;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitUnaryExpr(this);
    }
        internal readonly Token Operator;
        internal readonly Expr Right;
    }

    internal class Variable : Expr
    {
        internal Variable(Token name)
        {
            this.name = name;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitVariableExpr(this);
    }
        internal readonly Token name;
    }

    internal abstract T accept<T>(Visitor<T> visitor);
    }
}
