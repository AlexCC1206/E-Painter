using System;
using System.Collections.Generic;

namespace EPainter
{
    public class Parser
    {
        private List<Token> tokens;
        private int current = 0;


        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Statement> Parse()
        {
            List<Statement> statements = new List<Statement>();

            while (!IsAtEnd())
            {
                statements.Add(ParseStatement());
            }

            return statements;
        }

        private Statement ParseStatement()
        {
            try
            {
                if (Match(TokenType.SPAWN)) return ParseSpawn();
                if (Match(TokenType.COLOR)) return ParseColor();
                if (Match(TokenType.SIZE)) return ParseSize();
                if (Match(TokenType.DRAWLINE)) return ParseDrawLine();
                if (Match(TokenType.DRAWCIRCLE)) return ParseDrawCircle();
                if (Match(TokenType.DRAWRECTANGLE)) return ParseDrawRectangle();
                if (Match(TokenType.FILL)) return ParseFill();
                if (Match(TokenType.GOTO)) return ParseGoto();

                if (Check(TokenType.IDENTIFIER) && CheckNext(TokenType.LEFT_ARROW))
                {
                    return ParseAssignment();
                }

                if (Check(TokenType.IDENTIFIER) && CheckNext(TokenType.NEWLINE))
                {
                    return ParseLabel();
                }

                throw Error(Peek(), "Comando no reconocido");

            }
            catch (ParseError)
            {
                Syncronize();
                return null;
            }
        }

        private Statement ParseSpawn()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Spawn");
            Expr x = Expr();
            Consume(TokenType.COMMA, "Expect ',' between x and y coordinates");
            Expr y = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Spawn arguments");

            return new Statement.Spawn(x, y);
        }

        private Statement ParseColor()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Color");
            Expr color = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Color arguments");

            return new Statement.Color(color);
        }

        private Statement ParseSize()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Size");
            Expr k = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Size arguments");

            return new Statement.Size(k);
        }

        private Statement ParseDrawLine()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after DrawLine");
            Expr dirX = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirX");
            Expr dirY = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirY");
            Expr distance = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after distance");

            return new Statement.DrawLine(dirX, dirY, distance);
        }

        private Statement ParseDrawCircle()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after DrawCircle");
            Expr dirX = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirX");
            Expr dirY = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirY");
            Expr radius = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after radius");

            return new Statement.DrawCircle(dirX, dirY, radius);
        }

        private Statement ParseDrawRectangle()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after DrawRectangle");
            Expr dirX = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirX");
            Expr dirY = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirY");
            Expr distance = Expr();
            Consume(TokenType.COMMA, "Expect ',' between distance");
            Expr width = Expr();
            Consume(TokenType.COMMA, "Expect ',' between width");
            Expr height = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after height");

            return new Statement.DrawRectangle(dirX, dirY, distance, width, height);
        }

        private Statement ParseFill()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Fill");
            Consume(TokenType.RIGHT_PAREN, "Expect ')'");

            return new Statement.Fill();
        }

        private Statement ParseAssignment()
        {
            Token name = Previous();
            Consume(TokenType.LEFT_ARROW, "Expect '<-' after variable name");
            Expr value = Expr();

            return new Statement.Assignment(name, value);
        }

        private Statement ParseLabel()
        {
            Token name = Previous();
            Consume(TokenType.NEWLINE, "Expect 'Jumpline' after label name");

            return new Statement.Label(name);
        }

        private Statement ParseGoto()
        {
            Consume(TokenType.LEFT_BRACKET, "Expect '[' after  GoTo");
            Token label = Consume(TokenType.IDENTIFIER, "Expect label name");
            Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name");
            Consume(TokenType.LEFT_PAREN, "Expect '(' before condition");
            Expr condition = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition");

            return new Statement.Goto(label, condition);
        }

        private Expr Expr()
        {
            return LogicalAnd();
        }

        private Expr LogicalAnd()
        {
            Expr expr = LogicalOr();

            while (Match(TokenType.AND))
            {
                Token op = Previous();
                Expr rigth = LogicalOr();
                expr = new Expr.Binary(expr, op, rigth);
            }

            return expr;
        }

        private Expr LogicalOr()
        {
            Expr expr = Equality();

            while (Match(TokenType.OR))
            {
                Token op = Previous();
                Expr rigth = Equality();
                expr = new Expr.Binary(expr, op, rigth);
            }

            return expr;
        }

        private Expr Equality()
        {
            Expr Expr = Comparison();

            while (Match(TokenType.EQUAL_EQUAL))
            {
                Token Operator = Previous();
                Expr right = Comparison();
                Expr = new Expr.Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr Comparison()
        {
            Expr Expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token Operator = Previous();
                Expr right = Term();
                Expr = new Expr.Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr Term()
        {
            Expr Expr = Factor();

            while (Match(TokenType.MIN, TokenType.SUM))
            {
                Token Operator = Previous();
                Expr right = Factor();
                Expr = new Expr.Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr Factor()
        {
            Expr Expr = Pow();

            while (Match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token Operator = Previous();
                Expr right = Pow();
                Expr = new Expr.Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr Pow()
        {
            Expr Expr = Unary();

            while (Match(TokenType.POW))
            {
                Token Operator = Previous();
                Expr right = Primary();
                Expr = new Expr.Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.MIN))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Expr.Unary(op, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.NUMBER, TokenType.STRING,
                    TokenType.RED, TokenType.BLUE, TokenType.GREEN,
                    TokenType.YELLOW, TokenType.ORANGE, TokenType.PURPLE,
                    TokenType.BLACK, TokenType.WHITE, TokenType.TRANSPARENT))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.GETACTUALX, TokenType.GETACTUALY,
                     TokenType.GETCANVASIZE, TokenType.GETCOLORCOUNT,
                     TokenType.ISBRUSHCOLOR, TokenType.ISBRUSHSIZE,
                     TokenType.ISCANVASCOLOR)) return ParseFunctionCall();

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expr();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after Expr");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected expression");
        }

        private Expr ParseFunctionCall()
        {
            Token name = Previous();
            Consume(TokenType.LEFT_PAREN, "Expect '(' after function name");

            List<Expr> arguments = new List<Expr>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    arguments.Add(Expr());
                }
                while (Match(TokenType.COMMA));
            }

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after function arguments");
            return new Expr.FunctionCall(name, arguments);
        }

        private bool Match(params TokenType[] Types)
        {
            foreach (var Type in Types)
            {
                if (Check(Type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType Type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == Type;
        }

        private bool CheckNext(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (tokens[current + 1].Type == TokenType.EOF)
            {
                return false;
            }

            return tokens[current + 1].Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }

            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private Token Consume(TokenType Type, string message)
        {
            if (Check(Type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            string errorMessage = $"[Línea {token.Line}] Error: {message}";
            ErrorHandler.Error(token, errorMessage);
            return new ParseError();
        }

        private void Syncronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.NEWLINE)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.SPAWN:
                    case TokenType.COLOR:
                    case TokenType.DRAWLINE:
                    case TokenType.DRAWCIRCLE:
                    case TokenType.DRAWRECTANGLE:
                    case TokenType.FILL:
                    case TokenType.GOTO:
                        return;
                }

                Advance();
            }
        }

    }
}