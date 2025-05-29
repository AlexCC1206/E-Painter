using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EPainter
{
    public class Parser
    {
        private List<Token> tokens;
        private int current = 0;
        private Dictionary<string, int> labels = new Dictionary<string, int>();

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            PreprocessLabels();
        }

        public void PreprocessLabels()
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].type == TokenType.IDENTIFIER && i + 1 < tokens.Count && tokens[i + 1].type == TokenType.NEWLINE)
                {
                    labels[tokens[i].lexeme] = 1;
                }
            }
        }

        public List<Command> Parse()
        {
            var commands = new List<Command>();

            while (!IsAtEnd())
            {
                try
                {
                    commands.Add(ParseCommand());
                }
                catch (ParseError error)
                {
                    Console.WriteLine($"Syntax error: {error.Message}");
                }
            }

            return commands;

        }

        private Command ParseCommand()
        {
            if (Match(TokenType.SPAWN)) return ParseSpawn();
            if (Match(TokenType.COLOR)) return ParseColor();
            if (Match(TokenType.SIZE)) return ParseSize();
            if (Match(TokenType.DRAWLINE)) return ParseDrawline();
            if (Match(TokenType.DRAWCIRCLE)) return ParseDrawCircle();
            if (Match(TokenType.DRAWRECTANGLE)) return ParseDrawRectangle();
            if (Match(TokenType.FILL)) return ParseFill();

            if (Check(TokenType.IDENTIFIER) && Peek().type == TokenType.LEFT_ARROW) return ParseAssignment();

            if (Check(TokenType.IDENTIFIER) && Peek().type != TokenType.LEFT_ARROW && Peek().type != TokenType.LEFT_PAREN) return ParseLabel();

            if (Match(TokenType.GOTO)) return ParseGoto();

            throw Error(Peek(), "Comando no reconocido");
        }

        private Command ParseSpawn()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Spawn");
            Expr x = Expr();
            Consume(TokenType.COMMA, "Expect ',' between x and y coordinates");
            Expr y = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Spawn arguments");
            return new Command.Spawn(x, y);
        }

        private Command ParseColor()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Color");
            Expr color = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Color arguments");
            return new Command.Color(color);
        }

        private Command ParseSize()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Size");
            Expr k = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Size arguments");
            return new Command.Size(k);
        }

        private Command ParseDrawline()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after DrawLine");
            Expr dirX = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirX");
            Expr dirY = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirY");
            Expr distance = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Spawn arguments");
            return new Command.DrawLine(dirX, dirY, distance);
        }

        private Command ParseDrawCircle()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after DrawCircle");
            Expr dirX = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirX");
            Expr dirY = Expr();
            Consume(TokenType.COMMA, "Expect ',' between dirY");
            Expr radius = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawCircle arguments");
            return new Command.DrawLine(dirX, dirY, radius);
        }

        private Command ParseDrawRectangle()
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
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawRectangle arguments");
            return new Command.DrawLine(dirX, dirY, distance);
        }

        private Command ParseFill()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after Fill");
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Fill arguments");
            return new Command.Fill();
        }
        
        private Command ParseAssignment()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name");
            Consume(TokenType.LEFT_ARROW, "Expect '<-' after variable name");
            Expr value = Expr();
            return new Command.Assignment(name, value);
        }

        private Command ParseLabel()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect label name");
            //Consume(TokenType.COLON, "Expect ':' after label name");
            return new Command.Label(name);
        }

        private Command ParseGoto()
        {
            Consume(TokenType.LEFT_BRACKET, "Expect '[' after  GoTo");
            Token label = Consume(TokenType.IDENTIFIER, "Expect label name");
            Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name");
            Consume(TokenType.LEFT_PAREN, "Expect '(' before condition");
            Expr condition = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition");
            return new Command.Goto(label, condition);
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
            Expr Expr = Primary();

            while (Match(TokenType.POW))
            {
                Token Operator = Previous();
                Expr right = Primary();
                Expr = new Expr.Binary(Expr, Operator, right);
            }

            return Expr;
        }

        private Expr Primary()
        {
            if (Match(TokenType.NUMBER, TokenType.STRING,
                    TokenType.RED, TokenType.BLUE, TokenType.GREEN,
                    TokenType.YELLOW, TokenType.ORANGE, TokenType.PURPLE,
                    TokenType.BLACK, TokenType.WHITE, TokenType.TRANSPARENT))
            {
                return new Expr.Literal(Previous().literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                if (Check(TokenType.LEFT_PAREN))
                {
                    return ParseFunctionCall();
                }

                return new Expr.Variable(Previous());
            }

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

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            ErrorHandler.Error(token, message);
            return new ParseError();
        }

        private void Syncronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.NEWLINE)
                {
                    return;
                }

                switch (Peek().type)
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

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().type == type;
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
            return Peek().type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }
    }
}