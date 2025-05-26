using System;
using System.Collections.Generic;

namespace EPainter
{
    public class Parser
    {
        private class ParseError : Exception{}
        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Expr Expr()
        {
            return LogicalOr();
        }

        private Expr LogicalOr()
        {
            Expr expr = LogicalAnd();

            while(match(TokenType.OR))
            {
                Token op = previous();
                Expr rigth = LogicalAnd();
                expr = new Binary(expr, op, rigth);
            }

            return expr;
        }

        private Expr LogicalAnd()
        {
            Expr expr = equality();

            while(match(TokenType.AND))
            {
                Token op = previous();
                Expr rigth = equality();
                expr = new Binary(expr, op, rigth);
            }

            return expr;
        }

        private Expr equality()
        {
            Expr Expr = comparison();

            while(match(TokenType.EQUAL_EQUAL))
            {
                Token Operator = previous();
                Expr right = comparison();
                Expr = new Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr comparison()
        {
            Expr Expr = term();

            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL,TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token Operator = previous();
                Expr right = term();
                Expr = new Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr term()
        {
            Expr Expr = factor();

            while(match(TokenType.MIN, TokenType.SUM))
            {
                Token Operator = previous();
                Expr right = factor();
                Expr = new Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr factor()
        {
            Expr Expr = pow();

            while(match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token Operator = previous();
                Expr right = pow();
                Expr = new Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr pow()
        {
            Expr Expr = unary();

            while(match(TokenType.POW))
            {
                Token Operator = previous();
                Expr right = unary();
                Expr = new Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr unary()
        {
            if (match(TokenType.MIN))
            {
                Token Operator = previous();
                Expr right = unary();

                return new Unary(Operator, right);
            }

            return primary();
        }

        private Expr primary()
        {
            if(match(TokenType.NUMBER, TokenType.STRING, 
                    TokenType.RED, TokenType.BLUE, TokenType.GREEN,
                    TokenType.YELLOW, TokenType.ORANGE, TokenType.PURPLE,
                    TokenType.BLACK, TokenType.WHITE, TokenType.TRANSPARENT))
            {
                return new Literal(previous().literal);
            }

            if(match(TokenType.IDENTIFIER))
            {
                return new Variable(previous().lexeme);
            }

            if(match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expr();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after Expr");
                return new Grouping(expr);
            }

            throw Error(peek(), "Expected Expr");
        }

        private bool match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if(check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private Token consume(TokenType type, string message)
        {
            if(check(type))
            {
                return advance();
            }

            throw Error(peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            error(token, message);
            return new ParseError();
        }

        private void error(Token token, string message)
        {
            if(token.type == TokenType.EOF)
            {
                Report(token.line, " at end", message);
            }
            else
            {
                Report(token.line, $" at '{token.lexeme}'", message);
            }
        }

        private static bool hadError = false;

        public static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }

        private void syncronize()
        {
            advance();

            while(!isAtEnd())
            {
                if(previous().type == TokenType.NEWLINE)
                {
                    return;
                }

                switch(peek().type)
                {
                    case TokenType.SPAWN :
                    case TokenType.COLOR :
                    case TokenType.DRAWLINE :
                    case TokenType.DRAWCIRCLE :
                    case TokenType.DRAWRECTANGLE :
                    case TokenType.FILL :
                    case TokenType.GOTO :
                    return;
                }

                advance();
            }
        }

        private bool check(TokenType type)
        {
            if(isAtEnd())
            {
                return false;
            } 

            return peek().type == type;
        }

        private Token advance()
        {
            if(!isAtEnd())
            {
                current ++;
            }
            
            return previous();
        }

        private bool isAtEnd()
        {
            return peek().type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }

        internal Expr parse()
        {
            try
            {
                return Expr();
            }
            catch (ParseError)
            {
                return null;
            }
        }
    }
}