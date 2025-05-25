using System;
using System.Collections.Generic;

namespace EPainter
{
    public class Parser
    {
        private class ParseError : System.Exception{}
        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Expression expression()
        {
            return equality();
        }

        private Expression equality()
        {
            Expression expression = comparison();

            while(match(TokenType.EQUAL_EQUAL))
            {
                Token Operator = previous();
                Expression right = comparison();
                expression = new Binary(expression, Operator, right);
            }

            return expression;
        }

        private Expression comparison()
        {
            Expression expression = term();

            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL,TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token Operator = previous();
                Expression right = term();
                expression = new Binary(expression, Operator, right);
            }

            return expression;
        }

        private Expression term()
        {
            Expression expression = factor();

            while(match(TokenType.MIN, TokenType.SUM))
            {
                Token Operator = previous();
                Expression right = factor();
                expression = new Binary(expression, Operator, right);
            }

            return expression;
        }

        private Expression factor()
        {
            Expression expression = pow();

            while(match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token Operator = previous();
                Expression right = pow();
                expression = new Binary(expression, Operator, right);
            }

            return expression;
        }

        private Expression pow()
        {
            Expression expression = unary();

            while(match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token Operator = previous();
                Expression right = unary();
                expression = new Binary(expression, Operator, right);
            }

            return expression;
        }

        private Expression unary()
        {
            Expression expression = unary();

            if (match(TokenType.MIN))
            {
                Token Operator = previous();
                Expression right = unary();

                return new Unary(Operator, right);
            }

            return primary();
        }

        private Expression primary()
        {
            if(match(TokenType.STRING, TokenType.NUMBER))
            {
                return new Literal(previous().literal);
            }

            if(match(TokenType.LEFT_PAREN))
            {
                Expression expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Grouping(expr);
            }

            throw Error(peek(), "Expected expression");
        }

        Expression Parse()
        {
            try
            {
                return expression();
            }
            catch (ParseError Error)
            {
                return null;
            }
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
            EPainter.Error(token, message);
            return new ParseError();
        }

        private void error(Token token, string message)
        {
            if(token.type == TokenType.EOF)
            {
                report(token.line, " at end", message);
            }
            else
            {
                report(token.line, " at " + token.lexeme + "'", message);
            }
        }

        private void report(int line, string v, string message)
        {
            throw new NotImplementedException();
        }

        private void syncronize()
        {
            advance();

            while(!isAtEnd())
            {
                if(previous().type == TokenType.SEMICOLON)
                {
                    return;
                }

                switch(peek().type)
                {
                    case TokenType.SPAWN :
                    case TokenType.COLOR :
                    case TokenType.SIZE :
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
            return peek().type = TokenType.EOF;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }
    }
}