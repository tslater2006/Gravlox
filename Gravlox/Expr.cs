namespace Gravlox
{
    abstract class Expr
    {
        internal interface Visitor<T>
        {
            T visitBinaryExpr(Binary expr);
            T visitGroupingExpr(Grouping expr);
            T visitLiteralExpr(Literal expr);
            T visitUnaryExpr(Unary expr);
        }

        internal class Binary : Expr
        {
            internal Binary(Expr Left, Token Operator, Expr Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }

            override internal T accept<T>(Visitor<T> visitor)
            {
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

            override internal T accept<T>(Visitor<T> visitor)
            {
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

            override internal T accept<T>(Visitor<T> visitor)
            {
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

            override internal T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
            internal readonly Token Operator;
            internal readonly Expr Right;
        }

        internal abstract T accept<T>(Visitor<T> visitor);
    }
}
