using System;
using System.Collections.Generic;

namespace Gravlox
{

    class Parser
    {
        internal class ParseError : Exception
        {

        }

        readonly List<Token> Tokens;
        int current = 0;

        internal Parser(List<Token> Tokens)
        {
            this.Tokens = Tokens;
        }

        internal Expr Parse()
        {
            try
            {
                return Expression();
            } catch(ParseError error)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token oper = previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token oper = previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while(match(TokenType.MINUS, TokenType.PLUS))
            {
                Token oper = previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while (match(TokenType.SLASH, TokenType.STAR))
            {
                Token oper = previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (match(TokenType.BANG, TokenType.MINUS))
            {
                Token oper = previous();
                Expr right = Unary();
                return new Expr.Unary(oper, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NIL)) return new Expr.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(previous().Literal);
            }

            if (match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw error(peek(), "Expect expression.");
        }

        private bool match(params TokenType[] types)
        {
            foreach(TokenType type in types) {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }

        private ParseError error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void synchronize()
        {
            advance();

            while (!isAtEnd())
            {
                if (previous().Type == TokenType.SEMICOLON) return;

                switch(peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                advance();
            }
        }

        private bool check(TokenType tokenType)
        {
            if (isAtEnd()) return false;
            return peek().Type == tokenType;
        }

        private Token advance()
        {
            if (!isAtEnd()) current++;

            return previous();
        }

        private bool isAtEnd()
        {
            return peek().Type == TokenType.EOF;
        }

        private Token peek()
        {
            return Tokens[current];
        }

        private Token previous()
        {
            return Tokens[current - 1];
        }
    }
}
