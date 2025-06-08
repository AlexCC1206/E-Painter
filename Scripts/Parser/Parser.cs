using System;
using System.Collections.Generic;

namespace EPainter
{
    public class Parser
    {
        private List<Token> Tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.SPAWN)) return SpawnStatement();
                if (Match(TokenType.COLOR)) return ColorStatement();
                if (Match(TokenType.SIZE)) return SizeStatement();
                if (Match(TokenType.DRAW_LINE)) return DrawLineStatement();
                if (Match(TokenType.DRAW_CIRCLE)) return DrawCircleStatement();
                if (Match(TokenType.DRAW_RECTANGLE)) return DrawRectangleStatement();
                if (Match(TokenType.FILL)) return FillStatement();
                if (Match(TokenType.GOTO)) return GotoStatement();

                return AssignmenStatement();
            }
            catch (ParseError)
            {
                Syncronize();
                return null;
            }
        }

        private Stmt SpawnStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Spawn'.");
            Expr x = Expression();
            Consume(TokenType.COMMA, "Expect ',' between x and y coordinates.");
            Expr y = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Spawn arguments.");
            return new Stmt.Spawn(x, y);
        }

        private Stmt ColorStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Color'.");
            Token color = Consume(TokenType.COLOR_LITERAL, "Expect color literal.");
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after color.");
            return new Stmt.Color(color.Lexeme);
        }

        private Stmt SizeStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Size'");
            Expr size = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after size.");
            return new Stmt.Size(size);
        }

        private Stmt DrawLineStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawLine'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr distance = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawLine arguments.");
            return new Stmt.DrawLine(dirX, dirY, distance);
        }

        private Stmt DrawCircleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawCircle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr radius = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawCircle arguments.");
            return new Stmt.DrawCircle(dirX, dirY, radius);
        }

        private Stmt DrawRectangleStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'DrawRectangle'.");
            Expr dirX = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction X.");
            Expr dirY = Expression();
            Consume(TokenType.COMMA, "Expect ',' after direction Y.");
            Expr distance = Expression();
            Consume(TokenType.COMMA, "Expect ',' after distance.");
            Expr width = Expression();
            Consume(TokenType.COMMA, "Expect ',' after width.");
            Expr height = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after DrawRectangle arguments.");
            return new Stmt.DrawRectangle(dirX, dirY, distance, width, height);
        }

        private Stmt FillStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'Fill'.");
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after Fill.");
            return new Stmt.Fill();
        }
        
        private Stmt GotoStatement()
        {
            Consume(TokenType.LEFT_BRACKET, "Expect '[' after  'GoTo'.");
            Token label = Consume(TokenType.IDENTIFIER, "Expect label name.");
            Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name.");
            Consume(TokenType.LEFT_PAREN, "Expect '(' before condition.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            return new Stmt.Goto(label, condition);
        }

        private Stmt AssignmenStatement()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");
            Consume(TokenType.LEFT_ARROW, "Expect '<-' after variable name");
            Expr value = Expression();
            return new Stmt.Assignment(name, value);
        }

        private Expr Expression()
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
                expr = new Expr.Logical(expr, op, rigth);
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
                expr = new Expr.Logical(expr, op, rigth);
            }

            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.MIN, TokenType.SUM))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Pow();

            while (Match(TokenType.MULT, TokenType.DIV, TokenType.MOD))
            {
                Token op = Previous();
                Expr right = Pow();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Pow()
        {
            Expr Expr = Unary();

            while (Match(TokenType.POW))
            {
                Token op = Previous();
                Expr right = Primary();
                Expr = new Expr.Binary(Expr, op, right);
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
            if (Match(TokenType.NUMBER))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.COLOR_LITERAL))
            {
                return new Expr.Literal(Previous().Lexeme);
            }

            if (Match(TokenType.STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    return FunctionCall(Previous());
                }
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected expression");
        }

        private Expr FunctionCall(Token name)
        {
            var arguments = new List<Expr>();

            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after function arguments");

            return new Expr.Call(name, arguments);
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
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return Tokens[current];
        }

        private Token Previous()
        {
            return Tokens[current - 1];
        }

        private ParseError Error(Token token, string message)
        {
            ErrorReporter.Error(token, message);
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
                    case TokenType.SIZE:
                    case TokenType.DRAW_LINE:
                    case TokenType.DRAW_CIRCLE:
                    case TokenType.DRAW_RECTANGLE:
                    case TokenType.FILL:
                    case TokenType.GOTO:
                        return;
                }

                Advance();
            }
        }
    }
}