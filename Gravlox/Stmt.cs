using System.Collections.Generic;

namespace Gravlox
{
    abstract class Stmt{
    internal interface Visitor<T> {
        T visitBlockStmt(Block stmt);
        T visitExpressionStmt(Expression stmt);
        T visitIfStmt(If stmt);
        T visitPrintStmt(Print stmt);
        T visitVarStmt(Var stmt);
        T visitWhileStmt(While stmt);
    }

    internal class Block : Stmt
    {
        internal Block(List<Stmt> statements)
        {
            this.statements = statements;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitBlockStmt(this);
    }
        internal readonly List<Stmt> statements;
    }

    internal class Expression : Stmt
    {
        internal Expression(Expr expression)
        {
            this.expression = expression;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitExpressionStmt(this);
    }
        internal readonly Expr expression;
    }

    internal class If : Stmt
    {
        internal If(Expr condition, Stmt thenBranch, Stmt elseBranch)
        {
            this.condition = condition;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitIfStmt(this);
    }
        internal readonly Expr condition;
        internal readonly Stmt thenBranch;
        internal readonly Stmt elseBranch;
    }

    internal class Print : Stmt
    {
        internal Print(Expr expression)
        {
            this.expression = expression;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitPrintStmt(this);
    }
        internal readonly Expr expression;
    }

    internal class Var : Stmt
    {
        internal Var(Token name, Expr initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitVarStmt(this);
    }
        internal readonly Token name;
        internal readonly Expr initializer;
    }

    internal class While : Stmt
    {
        internal While(Expr condition, Stmt body)
        {
            this.condition = condition;
            this.body = body;
        }

    override internal T accept<T> (Visitor<T> visitor) {
        return visitor.visitWhileStmt(this);
    }
        internal readonly Expr condition;
        internal readonly Stmt body;
    }

    internal abstract T accept<T>(Visitor<T> visitor);
    }
}
